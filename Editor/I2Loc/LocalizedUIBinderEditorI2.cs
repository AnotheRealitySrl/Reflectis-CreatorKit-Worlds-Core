// I2Loc inspector for LocalizedUIBinder. Compiled only when I2Loc is present (I2LOC define, enforced by
// this assembly's defineConstraint). Each localized key is picked from a dropdown of I2 terms, mirroring
// the [TermsPopup] behaviour the LocalizedXxx elements use in the UI Builder.
#if I2LOC
using System.Collections.Generic;

using I2.Loc;

using Reflectis.CreatorKit.Worlds.Placeholders;
using Reflectis.CreatorKit.Worlds.Placeholders.EditorTools;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Reflectis.CreatorKit.Worlds.Placeholders.EditorTools.I2Loc
{
    [CustomEditor(typeof(LocalizedUIBinder))]
    public class LocalizedUIBinderEditorI2 : Editor
    {
        private const string NoneOption = "(nessuna)";

        private SerializedProperty documentProp;
        private SerializedProperty bindingsProp;
        private int cachedAssetId = -1;
        private int unnamedCount;
        private string[] terms = new string[0];

        private void OnEnable()
        {
            documentProp = serializedObject.FindProperty("document");
            bindingsProp = serializedObject.FindProperty("bindings");
            RefreshTerms();
            Rescan();
            cachedAssetId = CurrentAssetId();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int assetId = CurrentAssetId();
            if (assetId != cachedAssetId)
            {
                Rescan();
                cachedAssetId = assetId;
            }

            EditorGUILayout.PropertyField(documentProp);
            EditorGUILayout.Space();

            VisualTreeAsset vta = LocalizedUIBinderEditorCore.GetVisualTreeAsset(documentProp, target);
            if (vta == null)
            {
                EditorGUILayout.HelpBox("Assegna un UIDocument con un Source Asset per elencare gli " +
                    "elementi localized.", MessageType.Warning);
            }

            if (unnamedCount > 0)
            {
                EditorGUILayout.HelpBox($"{unnamedCount} elemento/i localized nella UXML non hanno un " +
                    "'name' e sono stati ignorati. Assegna un name per poterli gestire qui.",
                    MessageType.Warning);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Elementi localized (I2)", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("Ricarica termini", GUILayout.Width(120)))
                {
                    RefreshTerms();
                }
                if (GUILayout.Button("Ricarica dalla UXML", GUILayout.Width(150)))
                {
                    Rescan();
                    cachedAssetId = assetId;
                }
            }
            EditorGUILayout.Space();

            LocalizedUIBinderEditorCore.DrawBindings(bindingsProp, DrawTermDropdown);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTermDropdown(GUIContent label, SerializedProperty valueProp)
        {
            string current = valueProp.stringValue;

            List<string> options = new() { NoneOption };
            options.AddRange(terms);

            int index = string.IsNullOrEmpty(current) ? 0 : options.IndexOf(current);
            bool custom = index < 0;
            if (custom)
            {
                options.Add($"{current}  (assente)");
                index = options.Count - 1;
            }

            using (EditorGUI.ChangeCheckScope check = new())
            {
                int newIndex = EditorGUILayout.Popup(label, index, options.ToArray());
                if (check.changed)
                {
                    if (newIndex == 0)
                    {
                        valueProp.stringValue = string.Empty;
                    }
                    else if (newIndex - 1 < terms.Length)
                    {
                        valueProp.stringValue = terms[newIndex - 1];
                    }
                    // The trailing "(assente)" sentinel leaves the current value untouched.
                }
            }
        }

        private void RefreshTerms()
        {
            List<string> list = LocalizationManager.GetTermsList();
            if (list != null)
            {
                list.Sort();
                terms = list.ToArray();
            }
            else
            {
                terms = new string[0];
            }
        }

        private int CurrentAssetId()
        {
            VisualTreeAsset vta = LocalizedUIBinderEditorCore.GetVisualTreeAsset(documentProp, target);
            return vta != null ? vta.GetInstanceID() : 0;
        }

        private void Rescan()
        {
            serializedObject.Update();
            unnamedCount = 0;
            VisualTreeAsset vta = LocalizedUIBinderEditorCore.GetVisualTreeAsset(documentProp, target);
            if (vta == null)
            {
                return;
            }
            var scanned = LocalizedUIBinderEditorCore.Scan(vta, out unnamedCount);
            if (LocalizedUIBinderEditorCore.NeedsSync(bindingsProp, scanned))
            {
                LocalizedUIBinderEditorCore.Sync(bindingsProp, scanned);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
