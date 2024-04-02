#if UNITY_EDITOR
using Reflectis.SDK.CreatorKitEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class VisualScriptingSetupEditor
{
    //[MenuItem("Reflectis/Reset Visual Scripting Nodes")]
    public static void ResetNodes()
    {
        while (BoltCore.Configuration.typeOptions.Count > 0)
        {
            BoltCore.Configuration.typeOptions.RemoveAt(0);
        }

        var _typeOptionsMetadata = BoltCore.Configuration.GetMetadata(nameof(BoltCore.Configuration.typeOptions));
        //_typeOptionsMetadata.Reset(true);
        if (_typeOptionsMetadata.defaultValue is List<Type> defaultTypes)
        {
            foreach (Type type in defaultTypes)
            {
                BoltCore.Configuration.typeOptions.Add(type);
            }
        }
    }

    [MenuItem("Reflectis/Setup Visual Scripting Nodes")]
    public static void Setup()
    {

        ResetNodes();

        foreach (var type in GetCustomTypes())
        {
            if (type == null)
            {
                Debug.Log("Found Null type!");
                continue;
            }
            if (type == null)
            {
                continue;
            }
            if (!BoltCore.Configuration.typeOptions.Contains(type))
            {
                if (type.Namespace != null && !BoltCore.Configuration.assemblyOptions.Exists(x => x.name == type.Namespace))
                {
                    LooseAssemblyName looseAssemblyName = new LooseAssemblyName(type.Namespace);
                    BoltCore.Configuration.assemblyOptions.Add(looseAssemblyName);
                }

                BoltCore.Configuration.typeOptions.Add(type);
                Debug.Log($"Add {type.FullName} to Visual Scripting type options.");
            }

        }

        var typeOptionsMetadata = BoltCore.Configuration.GetMetadata(nameof(BoltCore.Configuration.typeOptions));
        typeOptionsMetadata.Save();
        BoltCore.Configuration.Save();
        Codebase.UpdateSettings();
        UnitBase.Rebuild();
    }

    private static IEnumerable<Type> GetCustomTypes()
    {
        var customTypeCollector = Resources.Load<VisualScriptingCustomTypeCollector>("VisualScriptingCustomTypeCollector");
        var reflectisCustomTypeCollector = Resources.Load<VisualScriptingCustomTypeCollector>("ReflectisVisualScriptingCustomTypeCollector");
        if (reflectisCustomTypeCollector != null)
        {
            return customTypeCollector.GetCustomTypes().Union(reflectisCustomTypeCollector.GetCustomTypes());
        }
        return customTypeCollector.GetCustomTypes();
    }
}
#endif