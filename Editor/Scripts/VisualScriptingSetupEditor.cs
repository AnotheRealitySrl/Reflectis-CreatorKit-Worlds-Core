#if UNITY_EDITOR
using Reflectis.SDK.CreatorKitEditor;
using Reflectis.SDK.Utilities;
using Reflectis.SDK.Utilities.Extensions;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class VisualScriptingSetupEditor
{
    [MenuItem("Reflectis/Setup Visual Scripting Nodes")]
    public static void Setup()
    {
        var boltTypeOptionsChanged = false;

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
        return customTypeCollector.CustomTypeTexts;
    }
}
#endif