using Reflectis.SDK.Core;
using Reflectis.SDK.Diagnostics;
using Reflectis.SDK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{


    [UnitTitle(UNIT_TITLE)]
    [UnitSurtitle("Reflectis Diagnostic")]
    [UnitShortTitle("Send Data")]
    [UnitCategory("Reflectis\\Flow")]
    public class DiagnosticSendDataUnit : Unit
    {

        public const string UNIT_TITLE = "Reflectis Diagnostic: Send Data";

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

        private GameObject gameObject;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            gameObject = instance.gameObject;
        }

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                var customProperties = this.Arguments
                .Where(x => f.GetValue(x) != null)
                .Select((x) =>
                {
                    return new Property(x.key, f.GetConvertedValue(x));
                }).Concat(CustomProperties.Select((x) =>
                {

                    return f.GetConvertedValue(x) as Property;
                })).ToList();
                try
                {
                    SM.GetSystem<IDiagnosticsSystem>().SendDiagnostic(Verb, customProperties);
                }
                catch (Exception exception)
                {
                    string message = $"Error during execution of \"{UNIT_TITLE}\" on gameObject {gameObject}: {exception.Message} ";
                    if (IDiagnosticsSystem.VerbsTypes[EDiagnosticType.Experience].Contains(Verb))
                    {
                        message = message +
                        $"Remember to call the node {DiagnosticGenerateExperienceIDUnit.UNIT_TITLE} to generate the ExperienceID before trying to send diagnostics data!";
                    }
                    Debug.LogError(message, gameObject);
                }
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
