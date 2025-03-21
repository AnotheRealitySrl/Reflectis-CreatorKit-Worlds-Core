using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.CoreEditor
{
    public class CreatorKitUpgradeWindow : EditorWindow
    {
        private readonly List<KeyValuePair<string, string>> stringsToReplace = new()
        {
            new KeyValuePair<string, string>("Reflectis.PLG.Graphs", "Reflectis.SDK.Graphs"),
            new KeyValuePair<string, string>("Reflectis.PLG.TasksReflectis", "Reflectis.CreatorKit.Worlds.Tasks"),
            new KeyValuePair<string, string>("Reflectis.PLG.Tasks", "Reflectis.SDK.Tasks"),
            new KeyValuePair<string, string>("Reflectis.SDK.CreatorKit", "Reflectis.CreatorKit.Worlds.VisualScripting"),
            new KeyValuePair<string, string>("Reflectis.SDK.InteractionNew", "Reflectis.CreatorKit.Worlds.VisualScripting"),
            new KeyValuePair<string, string>("GenericInteract", "VisualScriptingInteract"),
            new KeyValuePair<string, string>("VisualScripting.Diagnostic", "Analytics.Analytic"),
            new KeyValuePair<string, string>("Reflectis.SDK.Diagnostic", "Reflectis.CreatorKit.Worlds.Analytics"),

            // Splines
            new KeyValuePair<string, string>("ExposeSplineAnimateUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.ExposeSplineAnimateUnit"),
            new KeyValuePair<string, string>("ExposeSplineContainerUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.ExposeSplineContainerUnit"),
            new KeyValuePair<string, string>("GetSplineAnimateUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.GetSplineAnimateUnit"),
            new KeyValuePair<string, string>("GetSplineContainerUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.GetSplineContainerUnit"),
            new KeyValuePair<string, string>("SetSplineAnimateContainerUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.SetSplineAnimateContainerUnit"),
            new KeyValuePair<string, string>("SetSplineAnimateMaxSpeedUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.SetSplineAnimateMaxSpeedUnit"),
            new KeyValuePair<string, string>("SplineAnimatePlayCoroutineUnit", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines.SplineAnimatePlayCoroutineUnit"),
            // Multiple runs of the update window
            new KeyValuePair<string, string>("Reflectis.CreatorKit.Worlds.VisualScripting.Splines.Reflectis.CreatorKit.Worlds.VisualScripting.Splines.", "Reflectis.CreatorKit.Worlds.VisualScripting.Splines."),
        };

        private readonly List<string> extensions = new()
        {
            ".unity", ".prefab", ".asset"
        };

        [MenuItem("Reflectis/CK upgrade window")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(CreatorKitUpgradeWindow));
        }

        private void OnGUI()
        {
            GUIStyle style = new(EditorStyles.label)
            {
                richText = true,
            };

            if (GUILayout.Button("Migrate Reflectis version 2024.9.x -> 2025.1.0"))
            {
                try
                {
                    List<string> filePaths = new();
                    foreach (var extension in extensions)
                    {
                        filePaths.AddRange(Directory.GetFiles("Assets", $"*{extension}*", SearchOption.AllDirectories));
                    }

                    foreach (string file in filePaths)
                    {
                        // Leggi tutto il contenuto del file
                        string fileContent = File.ReadAllText(file);

                        foreach (KeyValuePair<string, string> stringToFind in stringsToReplace)
                        {
                            fileContent = fileContent.Replace(stringToFind.Key, stringToFind.Value);
                            Debug.Log($"Replaced {stringToFind.Key} -> {stringToFind.Value} in: {file}");
                        }
                        File.WriteAllText(file, fileContent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

            }
        }

    }
}
