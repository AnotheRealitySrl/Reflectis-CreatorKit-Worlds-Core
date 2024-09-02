using Reflectis.SDK.Diagnostics;
using Reflectis.SDK.InteractionNew;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{

    [UnitTitle("Reflectis ExperienceDiagnostic: Send Data")]
    [UnitSurtitle("Reflectis ExperienceDiagnostic")]
    [UnitShortTitle("Send Data")]
    [UnitCategory("Reflectis\\Flow")]
    public class SendExperienceDiagnosticUnitBase : Unit
    {
        [SerializeAs(nameof(Verb))]
        private EExperienceDiagnosticVerb verb;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable(nameof(verb))]
        public EExperienceDiagnosticVerb Verb
        {
            get => verb;
            set => verb = value;
        }

        [SerializeAs(nameof(CustomEntriesCount))]
        private int customEntriesCount;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Custom entries")]
        public int CustomEntriesCount
        {
            get => customEntriesCount;
            set => customEntriesCount = value;
        }

        [DoNotSerialize]
        public List<ValueInput> arguments { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput ContextualMenuManageable { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput BlockValue { get; private set; }

        protected override void Definition()
        {
            ContextualMenuManageable = ValueInput<ContextualMenuManageable>(nameof(ContextualMenuManageable));

            BlockValue = ValueInput<bool>(nameof(BlockValue), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {

                if (f.GetValue<bool>(BlockValue))
                {
                    f.GetValue<ContextualMenuManageable>(ContextualMenuManageable).CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                else
                {
                    f.GetValue<ContextualMenuManageable>(ContextualMenuManageable).CurrentBlockedState = f.GetValue<ContextualMenuManageable>(ContextualMenuManageable).CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            arguments = new List<ValueInput>();

            Type t = null;

            switch (Verb)
            {
                case EExperienceDiagnosticVerb.ExpStart:
                    t = typeof(ExperienceStartDTO);
                    break;
                default:
                    break;
            }

            if (t != null)
            {
                BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                //Since the variable "OnVariableChange" is marked as internal we are using reflection to get the variable and set its value
                foreach (var field in t.GetFields(bf))
                {
                    var attr = field.GetCustomAttribute<SettableFieldExperienceAttribute>();
                    if (attr != null)
                    {
                        var argument = ValueInput(field.FieldType, field.Name);
                        arguments.Add(argument);
                        Requirement(argument, InputTrigger);
                    }
                }
            }

            for (var i = 0; i < CustomEntriesCount; i++)
            {
                var argument = ValueInput<object>("Entry_" + i);
                arguments.Add(argument);
                Requirement(argument, InputTrigger);
            }

            Requirement(ContextualMenuManageable, InputTrigger);
            Requirement(BlockValue, InputTrigger);

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
