using Virtuademy.ScriptingApi;

using System;
using System.Collections.Generic;
using System.Reflection;

using Unity.VisualScripting;

using UnityEngine;


using SPACS.Utilities;

namespace Virtuademy.SDK.Environments.Analytics
{
    [UnitTitle(UNIT_TITLE)]
    [UnitSurtitle("Reflectis Analytic")]
    [UnitShortTitle("Create Displayable Data")]
    [UnitCategory("Reflectis\\Create")]
    public class AnalyticCreateDisplayableDataUnit : Unit
    {

        [SerializeAs(nameof(DisplayableType))]
        private EAnalyticsDisplayableType displayableType = EAnalyticsDisplayableType.Dynamic;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable(nameof(displayableType))]
        public EAnalyticsDisplayableType DisplayableType
        {
            get => displayableType;
            set => displayableType = value;
        }

        public const string UNIT_TITLE = "Reflectis Analytic: Create Displayable Data";


        [DoNotSerialize]
        public List<ValueInput> Arguments { get; private set; }

        [DoNotSerialize]
        public ValueOutput DisplayableData { get; private set; }



        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
        }

        protected override void Definition()
        {
            DisplayableData = ValueOutput(nameof(DisplayableData), (f) =>
            {
                //var customObjects = CustomObjects.Select((x) =>
                //{
                //    return f.GetConvertedValue(x) as CustomType;
                //});

                Type type = AnalyticDefinitions.DisplayableDataTypes[displayableType];

                if (type != null)
                {
                    var typeInstance = type.Instantiate();
                    List<Field> fields = new List<Field>();
                    foreach (var argument in Arguments)
                    {
                        if (argument.hasValidConnection || argument.hasDefaultValue)
                        {
                            var value = f.GetConvertedValue(argument);

                            //Set field value
                            fields.Add(new Field() { name = argument.key, value = value });

                        }
                    }
                    if (typeInstance is DisplayableContentBase displayableContent)
                    {
                        try
                        {
                            displayableContent.AssignValues(fields);
                            displayableContent.CheckValidity();

                            return new AnalyticsDisplayableData() { type = displayableType, content = displayableContent };
                        }
                        catch (Exception exception)
                        {
                            string message = $"Error during execution of \"{UNIT_TITLE}\" validity check failed with message: {exception.Message} ";
                            Debug.LogError(message);
                        }
                    }
                    else
                    {
                        Debug.LogError("Wrong type assigned to the value: " +
                        displayableType + "! Check the correct setup of the Analytic system. ");
                    }
                }
                else
                {
                    Debug.LogError("There are no Displayable content class rapresenting the given type: " +
                        displayableType + "! Check the correct setup of the Analytic system. ");
                }

                return null;
            });


            Arguments = new List<ValueInput>();

            Type type = AnalyticDefinitions.DisplayableDataTypes[displayableType];

            if (type != null)
            {
                foreach (var field in type.GetRuntimeFields())
                {
                    var attr = field.GetCustomAttribute<SettableFieldAttribute>();
                    if (attr != null)
                    {
                        ValueInput argument;
                        if (attr.entryType != null)
                        {
                            argument = ValueInput(attr.entryType, field.Name);
                        }
                        else
                        {
                            argument = ValueInput(field.FieldType, field.Name);
                        }
                        //if ((!field.FieldType.IsValueType && !field.FieldType.IsPrimitive && !field.FieldType.IsClass)
                        //|| (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        //|| field.FieldType.IsEnum)

                        if (field.FieldType.IsNullable() && !field.FieldType.IsClass)
                        {
                            argument.unit.defaultValues[field.Name] = null;
                        }
                        else
                        {
                            argument.SetDefaultValue(field.FieldType.Default());
                        }


                        Arguments.Add(argument);
                        if (attr.isRequired)
                        {
                            Requirement(argument, DisplayableData);
                        }
                    }
                }
            }


        }
    }
}
