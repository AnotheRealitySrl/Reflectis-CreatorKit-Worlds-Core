using Reflectis.SDK.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle(UNIT_TITLE)]
    [UnitSurtitle("Reflectis Diagnostic")]
    [UnitShortTitle("Get " + TYPE + " Displayable Data")]
    [UnitCategory("Reflectis\\Get")]
    public class DiagnosticGetDynamicDisplayableDataUnit : Unit
    {
        private const string TYPE = "Dynamic";

        public const string UNIT_TITLE = "Reflectis Diagnostic: Get " + TYPE + " Displayable Data";
        //public List<string> Headers { get => headers; set => headers = value; }
        //public List<string> Types { get => types; set => types = value; }
        //public List<List<object>> Records { get => records; set => records = value; }
        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput Headers { get; private set; }
        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput Types { get; private set; }
        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput Records { get; private set; }

        [DoNotSerialize]
        [PortLabel("Type")]
        public ValueOutput OutputType { get; private set; }

        [DoNotSerialize]
        public ValueOutput DisplayableData { get; private set; }


        private GameObject gameObject;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            gameObject = instance.gameObject;
        }

        protected override void Definition()
        {
            Headers = ValueInput<List<string>>(nameof(Headers));
            Types = ValueInput<List<string>>(nameof(Types));
            Records = ValueInput<List<object>>(nameof(Records));


            OutputType = ValueOutput(nameof(OutputType), (f) =>
            {
                return TYPE;
            });

            DisplayableData = ValueOutput(nameof(DisplayableData), (f) =>
            {
                DynamicDisplayableContent dynamicDisplayableContent = new DynamicDisplayableContent();

                dynamicDisplayableContent.Headers = f.GetConvertedValue(Headers) as List<string>;
                dynamicDisplayableContent.Types = f.GetConvertedValue(Types) as List<string>;
                List<object> recordsValue = f.GetConvertedValue(Records) as List<object>;
                dynamicDisplayableContent.Records = recordsValue.Select((x, i) =>
                {
                    if (x is AotList aotList)
                    {
                        List<object> records = new List<object>();
                        foreach (var obj in aotList)
                        {
                            records.Add(obj);
                        }
                        return records;
                    }
                    else
                    {
                        throw new Exception($"The record {i} is not a list");
                    }
                }).ToList();

                try
                {
                    dynamicDisplayableContent.CheckValidity();
                    Debug.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(dynamicDisplayableContent));
                    return Newtonsoft.Json.JsonConvert.SerializeObject(dynamicDisplayableContent); // SM.GetSystem<IDiagnosticsSystem>().SerializeData(displayableData);
                }
                catch (Exception exception)
                {
                    string message = $"Error during execution of \"{UNIT_TITLE}\" on gameObject {gameObject} validity check failed with message: {exception.Message} ";
                    Debug.LogError(message, gameObject);
                }
                return null;
            });

        }
    }
}
