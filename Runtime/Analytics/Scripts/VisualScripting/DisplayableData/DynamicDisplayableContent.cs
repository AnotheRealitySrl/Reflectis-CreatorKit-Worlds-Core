using Virtuademy.ScriptingApi;
using Virtuademy.SDK.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Virtuademy.SDK.Environments.Analytics
{
    public class DynamicDisplayableContent : DisplayableContentBase
    {
        [SettableField(isRequired = true)]
        public List<string> headers;
        [SettableField(isRequired = true)]
        public List<string> types;
        /// <summary>
        /// Visual scripting does not handle correctly list of list of objects
        /// in type entry definitions, so we have to pass a list of object as 
        /// type definition
        /// </summary>
        [SettableField(isRequired = true, entryType = typeof(List<object>))]
        public List<List<object>> records;


        public override void CheckValidity()
        {
            int columnCount = headers.Count;

            if (!(types.Count == columnCount && (records).All(x => ((List<object>)x).Count == columnCount)))
            {
                throw new System.Exception($"Record types count are not matching.\n" +
                    $"Headers count: {headers.Count} \n" +
                    $"Types count: {types.Count} \n" +
                    string.Join("", records.Select((x, i) => $"Record {i}: {((List<object>)x).Count} \n")));
            }
        }

        public override void AssignValues(List<Field> args)
        {
            var headerField = args.FirstOrDefault(x => x.name == "headers");
            if (headerField != null)
            {
                headers = headerField.value as List<string>;
            }
            var typesField = args.FirstOrDefault(x => x.name == "types");
            if (typesField != null)
            {
                types = typesField.value as List<string>;
            }
            var recordField = args.FirstOrDefault(x => x.name == "records");
            if (recordField != null)
            {
                List<object> recordsValue = recordField.value as List<object>;

                records = recordsValue.Select((x, i) =>
                {
                    if (x is ArrayList aotList)
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

            }
        }
    }
}
