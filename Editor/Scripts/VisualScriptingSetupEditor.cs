#if UNITY_EDITOR
using Reflectis.SDK.CreatorKitEditor;
using Reflectis.SDK.Utilities;
using Reflectis.SDK.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class VisualScriptingSetupEditor
{
    //[MenuItem("Reflectis/Reset Visual Scripting Nodes")]
    //public static void ResetNodes()
    //{
    //    var _typeOptionsMetadata = BoltCore.Configuration.GetMetadata(nameof(BoltCore.Configuration.typeOptions));
    //    _typeOptionsMetadata.Reset(true);
    //}

    [MenuItem("Reflectis/Setup Visual Scripting Nodes")]
    public static void Setup()
    {
        var boltTypeOptionsChanged = false;
        //var _typeOptionsMetadata = BoltCore.Configuration.GetMetadata(nameof(BoltCore.Configuration.typeOptions));
        //_typeOptionsMetadata.Reset(true);

        foreach (var typeTexts in GetCustomTypeTexts())
        {
            if (typeTexts == null)
            {
                Debug.Log("Found Null textAsset inside custom type list!");
                continue;
            }
            foreach (var typeName in typeTexts.GetTextTypes())
            {
                Type type = TypeExtensions.GetTypeFromString(typeName);
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
                    boltTypeOptionsChanged = true;
                }
            }
        }

        if (boltTypeOptionsChanged)
        {
            var typeOptionsMetadata = BoltCore.Configuration.GetMetadata(nameof(BoltCore.Configuration.typeOptions));
            typeOptionsMetadata.Save();
            BoltCore.Configuration.Save();
            Codebase.UpdateSettings();
            UnitBase.Rebuild();
        }
        else
        {
            Debug.Log("Reflectis visual scripting nodes already up to date.");
        }

    }

    private static IEnumerable<TextAsset> GetCustomTypeTexts()
    {
        var customTypeCollector = Resources.Load<VisualScriptingCustomTypeCollector>("VisualScriptingCustomTypeCollector");
        var reflectisCustomTypeCollector = Resources.Load<VisualScriptingCustomTypeCollector>("ReflectisVisualScriptingCustomTypeCollector");
        if (reflectisCustomTypeCollector != null)
        {
            return customTypeCollector.CustomTypeTexts.Union(reflectisCustomTypeCollector.CustomTypeTexts);
        }
        return customTypeCollector.CustomTypeTexts;
    }
}
#endif