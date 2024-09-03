using Reflectis.SDK.Core;
using Reflectis.SDK.Diagnostics;
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
    public class SendExperienceDiagnosticUnit : Unit
    {
        [SerializeAs(nameof(Verb))]
        private EExperienceDiagnosticVerb verb = EExperienceDiagnosticVerb.ExpStart;

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

                var customProperties = this.CustomProperties.Select((x) => { return f.GetConvertedValue(x) as VisualScriptingCustomProperty; }).ToArray();

                SM.GetSystem<IDiagnosticsSystem>().SendExperienceDiagnostic(Verb, CreateJSON(Arguments, customProperties, f));
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Arguments = new List<ValueInput>();

            Type t = null;

            switch (Verb)
            {
                case EExperienceDiagnosticVerb.ExpStart:
                    t = typeof(ExperienceStartDTO);
                    break;
                case EExperienceDiagnosticVerb.ExpComplete:
                    t = typeof(ExperienceCompleteDTO);
                    break;
                case EExperienceDiagnosticVerb.StepStart:
                    t = typeof(ExperienceStepStartDTO);
                    break;
                case EExperienceDiagnosticVerb.StepComplete:
                    t = typeof(ExperienceStepCompleteDTO);
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
                        Arguments.Add(argument);
                        Requirement(argument, InputTrigger);
                    }
                }
            }

            CustomProperties = new List<ValueInput>();

            for (var i = 0; i < CustomEntriesCount; i++)
            {
                var customProperty = ValueInput<VisualScriptingCustomProperty>("Custom_property_" + i);
                CustomProperties.Add(customProperty);
                Requirement(customProperty, InputTrigger);
            }

            Succession(InputTrigger, OutputTrigger);
        }

        private string CreateJSON(List<ValueInput> arguments, VisualScriptingCustomProperty[] customProperties, Flow f)
        {
            string json = "{";
            foreach (var argument in arguments)
            {
                json += $"\"{argument.key}\":{Newtonsoft.Json.JsonConvert.SerializeObject(f.GetConvertedValue(argument))}, ";
            }
            foreach (var customProperty in customProperties)
            {
                json += $"\"{customProperty.propertyName}\":{Newtonsoft.Json.JsonConvert.SerializeObject(customProperty.value)}, ";
            }
            //remove last comma
            return json.Substring(0, json.Length - 2) + "}";
        }

    }
}
