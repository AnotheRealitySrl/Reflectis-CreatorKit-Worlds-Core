using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Reflectis.CreatorKit.Worlds.Core.HybridCLR.Editor
{
    public enum ViolationKind
    {
        DisallowedAssemblyReference,
        DisallowedType,
        DisallowedMember,
        DynamicDispatch,
        PInvoke,
        UnmanagedCalli,
        UnsafePointer,

        /// <summary>
        /// The walk gave up before finishing, on metadata nested deeper than
        /// <see cref="VerificationResult.MaxDepth"/>. No compiler emits that, so seeing it locally
        /// means something built the DLL other than the compiler.
        /// </summary>
        VerifierLimit,
    }

    public readonly struct Violation
    {
        public readonly ViolationKind Kind;
        public readonly string Detail;
        public readonly string Location;
        public readonly string SourceFile;
        public readonly int? SourceLine;

        public Violation(ViolationKind kind, string detail, string location,
                         string sourceFile = null, int? sourceLine = null)
        {
            Kind = kind;
            Detail = detail;
            Location = location;
            SourceFile = sourceFile;
            SourceLine = sourceLine;
        }

        public string Where
            => SourceFile != null && SourceLine.HasValue
                ? $"{Path.GetFileName(SourceFile)}:{SourceLine.Value}"
                : Location;

        public override string ToString() => $"[{Kind}] {Detail}  ({Where})";
    }

    public sealed class VerificationResult
    {
        /// <summary>
        /// How deep the walk follows a type into its generic arguments, element type, by-ref and
        /// pointer targets, and how deep it follows a type into its nested types. Real code spends
        /// single digits. Mirrors the backend's bound so a local pass and a server pass mean the
        /// same thing; the backend also refuses such metadata before Mono.Cecil parses it, which is
        /// a protection this side does not need — here the input is the creator's own compiler
        /// output, not an upload from someone else.
        /// </summary>
        public const int MaxDepth = 64;

        public readonly List<Violation> Violations = new();
        public bool Passed => Violations.Count == 0;

        public string Summarize()
            => Passed
                ? "OK — hot-update assembly satisfies the security policy."
                : $"REJECTED — {Violations.Count} policy violation(s):\n"
                  + string.Join("\n", Violations.Select(v => "  - " + v));
    }

    /// <summary>
    /// Editor-time static verifier for interpreted (HybridCLR) assemblies. Reads the DLL's
    /// metadata with Mono.Cecil and rejects anything the supplied <see cref="HotUpdatePolicy"/>
    /// (fetched from the platform's policy.json) does not explicitly allow. When a matching PDB
    /// is available it maps each method-body violation to source file + line.
    ///
    /// This is a fast local pre-check for authoring UX; the AUTHORITATIVE check is the backend.
    /// </summary>
    public static class HotUpdateAssemblyVerifier
    {
        public static VerificationResult VerifyFile(string dllPath, HotUpdatePolicy policy)
            => Run(readSymbols => ModuleDefinition.ReadModule(dllPath, MakeParams(readSymbols)), dllPath, policy);

        public static VerificationResult VerifyBytes(byte[] dll, HotUpdatePolicy policy, string label = "<bytes>")
            => Run(readSymbols => ModuleDefinition.ReadModule(new MemoryStream(dll, writable: false), MakeParams(readSymbols)), label, policy);

        private static ReaderParameters MakeParams(bool readSymbols)
        {
            ReaderParameters p = new(ReadingMode.Deferred);
            if (readSymbols)
            {
                p.ReadSymbols = true;
                p.ThrowIfSymbolsAreNotMatching = false;
            }
            return p;
        }

        private static VerificationResult Run(Func<bool, ModuleDefinition> open, string label, HotUpdatePolicy policy)
        {
            if (policy == null)
            {
                VerificationResult noPolicy = new();
                noPolicy.Violations.Add(new Violation(
                    ViolationKind.DisallowedType, "No policy provided (fetch failed).", label));
                return noPolicy;
            }

            ModuleDefinition module = null;
            try { module = open(true); }
            catch { module = null; }

            if (module == null)
            {
                try { module = open(false); }
                catch (Exception e)
                {
                    VerificationResult err = new();
                    err.Violations.Add(new Violation(
                        ViolationKind.DisallowedType, $"Unreadable assembly: {e.Message}", label));
                    return err;
                }
            }

            using (module)
            {
                return Scan(module, label, policy);
            }
        }

        private static VerificationResult Scan(ModuleDefinition module, string label, HotUpdatePolicy policy)
        {
            VerificationResult result = new();

            foreach (AssemblyNameReference asmRef in module.AssemblyReferences)
            {
                if (!policy.IsAssemblyAllowed(asmRef.Name))
                    result.Violations.Add(new Violation(
                        ViolationKind.DisallowedAssemblyReference, asmRef.Name, label));
            }

            foreach (TypeReference typeRef in module.GetTypeReferences())
                CheckType(typeRef, result, "typeref", policy);

            foreach (MemberReference memberRef in module.GetMemberReferences())
            {
                TypeReference declaring = memberRef.DeclaringType;
                if (declaring == null)
                    continue;

                CheckType(declaring, result, "memberref", policy);

                if (policy.IsMemberDenied(declaring.FullName, memberRef.Name))
                    result.Violations.Add(new Violation(
                        ViolationKind.DisallowedMember, $"{declaring.FullName}::{memberRef.Name}", "memberref"));

                if (memberRef is MethodReference method
                    && policy.IsDynamicDispatch(declaring.FullName, method.Name, ParameterTypes(method)))
                {
                    result.Violations.Add(new Violation(
                        ViolationKind.DynamicDispatch, $"{declaring.FullName}::{method.Name} by name", "memberref"));
                }
            }

            foreach (TypeDefinition type in AllTypes(module.Types, result))
            {
                foreach (MethodDefinition method in type.Methods)
                    CheckMethod(method, result, policy);
            }

            return result;
        }

        private static void CheckMethod(MethodDefinition method, VerificationResult result, HotUpdatePolicy policy)
        {
            string where = method.FullName;

            if (method.IsPInvokeImpl || method.HasPInvokeInfo)
            {
                (string f, int? l) = LineOf(method, null);
                result.Violations.Add(new Violation(ViolationKind.PInvoke, method.FullName, where, f, l));
            }

            if ((method.ImplAttributes & MethodImplAttributes.Native) != 0 ||
                (method.ImplAttributes & MethodImplAttributes.Unmanaged) != 0)
            {
                (string f, int? l) = LineOf(method, null);
                result.Violations.Add(new Violation(ViolationKind.UnmanagedCalli, "native method body", where, f, l));
            }

            if (IsPointer(method.ReturnType))
            {
                (string f, int? l) = LineOf(method, null);
                result.Violations.Add(new Violation(ViolationKind.UnsafePointer, "pointer return", where, f, l));
            }
            foreach (ParameterDefinition p in method.Parameters)
            {
                if (IsPointer(p.ParameterType))
                {
                    (string f, int? l) = LineOf(method, null);
                    result.Violations.Add(new Violation(ViolationKind.UnsafePointer, $"pointer param '{p.Name}'", where, f, l));
                }
            }

            if (method.Body == null)
                return;

            foreach (VariableDefinition v in method.Body.Variables)
            {
                if (IsPointer(v.VariableType))
                {
                    (string f, int? l) = LineOf(method, null);
                    result.Violations.Add(new Violation(ViolationKind.UnsafePointer, "pointer local", where, f, l));
                }
            }

            foreach (Instruction ins in method.Body.Instructions)
            {
                (string file, int? line) = LineOf(method, ins);

                if (ins.OpCode.Code == Code.Calli)
                    result.Violations.Add(new Violation(ViolationKind.UnmanagedCalli, "calli", where, file, line));

                switch (ins.Operand)
                {
                    case TypeReference tr:
                        CheckType(tr, result, where, policy, file, line);
                        break;
                    case MethodReference mr:
                        if (mr.DeclaringType != null)
                        {
                            CheckType(mr.DeclaringType, result, where, policy, file, line);
                            if (policy.IsMemberDenied(mr.DeclaringType.FullName, mr.Name))
                                result.Violations.Add(new Violation(
                                    ViolationKind.DisallowedMember, $"{mr.DeclaringType.FullName}::{mr.Name}", where, file, line));

                            if (policy.IsDynamicDispatch(mr.DeclaringType.FullName, mr.Name, ParameterTypes(mr)))
                                result.Violations.Add(new Violation(
                                    ViolationKind.DynamicDispatch, $"{mr.DeclaringType.FullName}::{mr.Name} by name", where, file, line));
                        }
                        break;
                    case FieldReference fr:
                        if (fr.DeclaringType != null)
                            CheckType(fr.DeclaringType, result, where, policy, file, line);
                        break;
                }
            }
        }

        private static void CheckType(TypeReference typeRef, VerificationResult result, string where,
                                      HotUpdatePolicy policy, string file = null, int? line = null,
                                      int depth = 0)
        {
            if (typeRef == null)
                return;

            if (depth > VerificationResult.MaxDepth)
            {
                // Rejected, not skipped: whatever is under here went unchecked, and a verifier that
                // stops looking must not report a pass for the part it did not look at.
                result.Violations.Add(new Violation(
                    ViolationKind.VerifierLimit,
                    $"type signature nests deeper than {VerificationResult.MaxDepth}", where, file, line));
                return;
            }

            switch (typeRef)
            {
                case GenericInstanceType git:
                    CheckType(git.ElementType, result, where, policy, file, line, depth + 1);
                    foreach (TypeReference arg in git.GenericArguments)
                        CheckType(arg, result, where, policy, file, line, depth + 1);
                    return;
                case ArrayType at:
                    CheckType(at.ElementType, result, where, policy, file, line, depth + 1);
                    return;
                case ByReferenceType brt:
                    CheckType(brt.ElementType, result, where, policy, file, line, depth + 1);
                    return;
                case PointerType pt:
                    result.Violations.Add(new Violation(ViolationKind.UnsafePointer, pt.FullName, where, file, line));
                    CheckType(pt.ElementType, result, where, policy, file, line, depth + 1);
                    return;
                case GenericParameter:
                    return;
            }

            // A type this module DEFINES is the code under review, not an API it reaches for. The
            // whitelist constrains what a creator may call, not how they may name their own
            // classes: without this, a script inside `namespace MyGame` is rejected the moment it
            // instantiates or takes typeof() of itself. The only reason that has not bitten is
            // that creator scripts have so far lived in the global namespace, which the namespace
            // rule lets through for an unrelated reason. Their members and everything they touch
            // are still checked — this exempts the name, not the contents.
            if (typeRef is TypeDefinition)
                return;

            CheckTypeName(typeRef, result, where, policy, file, line);
        }

        /// <summary>
        /// Judges a type by its own name and by every name it is nested inside.
        ///
        /// Both halves are needed because of how ECMA-335 stores nesting. A nested type's metadata
        /// row carries an EMPTY namespace — the namespace belongs to the outermost enclosing type —
        /// so asking the whitelist about the row on its own asks it about a type with no namespace,
        /// which it used to answer by allowing. That waved through every nested type in every
        /// permitted assembly: measured across the 23 whitelisted assemblies, 480 of them, 130
        /// public, and 32 of those nested inside a name the policy denies in as many words —
        /// the System.Runtime.InteropServices marshalling machinery, System.Environment,
        /// System.IO.Enumeration.
        ///
        /// So the namespace comes from the outermost type, and every level of the chain has to pass
        /// on its own name too: a type nested inside a denied type is reached through the denied
        /// type, and its own name says nothing about that.
        /// </summary>
        private static void CheckTypeName(TypeReference typeRef, VerificationResult result, string where,
                                          HotUpdatePolicy policy, string file, int? line)
        {
            TypeReference outermost = typeRef;
            while (outermost.DeclaringType != null)
                outermost = outermost.DeclaringType;

            for (TypeReference current = typeRef; current != null; current = current.DeclaringType)
            {
                if (policy.IsTypeAllowed(outermost.Namespace, current.FullName))
                    continue;

                string detail = ReferenceEquals(current, typeRef)
                    ? typeRef.FullName
                    : $"{typeRef.FullName} (nested in the disallowed {current.FullName})";

                result.Violations.Add(new Violation(ViolationKind.DisallowedType, detail, where, file, line));
                return;
            }
        }

        private static (string file, int? line) LineOf(MethodDefinition method, Instruction ins)
        {
            MethodDebugInformation di = method?.DebugInformation;
            if (di == null || !di.HasSequencePoints)
                return (null, null);

            if (ins != null)
            {
                for (Instruction cur = ins; cur != null; cur = cur.Previous)
                {
                    SequencePoint sp = di.GetSequencePoint(cur);
                    if (sp != null && !sp.IsHidden)
                        return (sp.Document?.Url, sp.StartLine);
                }
            }

            SequencePoint first = di.SequencePoints.FirstOrDefault(s => !s.IsHidden);
            return first != null ? (first.Document?.Url, first.StartLine) : (null, null);
        }

        /// <summary>
        /// The parameter types of the overload this reference names — what tells
        /// GetComponent&lt;T&gt;() apart from GetComponent(string). Cecil has them without
        /// resolving the assembly the member lives in.
        /// </summary>
        private static IEnumerable<string> ParameterTypes(MethodReference method)
        {
            foreach (ParameterDefinition parameter in method.Parameters)
                yield return parameter.ParameterType?.FullName;
        }

        private static bool IsPointer(TypeReference t)
            => t != null && (t.IsPointer || t.IsFunctionPointer);

        /// <summary>
        /// Every type in the module, nested ones included. Bounded for the same reason
        /// <see cref="CheckType"/> is.
        /// </summary>
        private static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> types,
                                                            VerificationResult result, int depth = 0)
        {
            if (depth > VerificationResult.MaxDepth)
            {
                result.Violations.Add(new Violation(
                    ViolationKind.VerifierLimit,
                    $"nested types go deeper than {VerificationResult.MaxDepth}", "typedef"));
                yield break;
            }

            foreach (TypeDefinition t in types)
            {
                yield return t;
                foreach (TypeDefinition nested in AllTypes(t.NestedTypes, result, depth + 1))
                    yield return nested;
            }
        }
    }
}
