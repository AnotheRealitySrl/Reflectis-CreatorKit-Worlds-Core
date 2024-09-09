using Reflectis.SDK.Core;
using Reflectis.SDK.Diagnostics;
using Reflectis.SDK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{

    [UnitTitle("Reflectis ExperienceDiagnostic: Send Data")]
    [UnitSurtitle("Reflectis ExperienceDiagnostic")]
    [UnitShortTitle("Send Data")]
    [UnitCategory("Reflectis\\Flow")]
    public class SendDiagnosticUnit : Unit
    {
        [SerializeAs(nameof(Verb))]
        private EDiagnosticVerb verb = EDiagnosticVerb.ExpStart;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable(nameof(verb))]
        public EDiagnosticVerb Verb
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
        public List<ValueInput> Arguments { get; private set; }

        [DoNotSerialize]
        public List<ValueInput> CustomProperties { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }


        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                var customProperties = this.Arguments.Select((x) =>
                {
                    return new Property(x.key, f.GetConvertedValue(x));
                }).Concat(CustomProperties.Select((x) =>
                {
                    return new Property(x.key, f.GetConvertedValue(x));
                })).ToList();
                SM.GetSystem<IDiagnosticsSystem>().SendDiagnostic(Verb, customProperties);
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Arguments = new List<ValueInput>();

            Type t = IDiagnosticsSystem.VerbsDTOs[Verb];

            if (t != null)
            {
                if (IDiagnosticsSystem.VerbsTypes[EDiagnosticType.Experience].Contains(Verb))
                {
                    var keyArgument = ValueInput<string>(ExperienceDiagnosticDTO.KEY_FIELD_NAME);
                    Arguments.Add(keyArgument);
                    Requirement(keyArgument, InputTrigger);
                }

                BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                //Since the variable "OnVariableChange" is marked as internal we are using reflection to get the variable and set its value
                foreach (var field in t.GetFields(bf))
                {
                    var attr = field.GetCustomAttribute<SettableFieldExperienceAttribute>();
                    if (attr != null)
                    {
                        var argument = ValueInput(field.FieldType, field.Name);
                        Arguments.Add(argument);
                    }
                }
            }

            CustomProperties = new List<ValueInput>();

            for (var i = 0; i < CustomEntriesCount; i++)
            {
                var customProperty = ValueInput<Property>("Custom_property_" + i);
                CustomProperties.Add(customProperty);
                Requirement(customProperty, InputTrigger);
            }

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
