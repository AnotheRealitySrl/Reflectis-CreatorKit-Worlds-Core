// I2Loc drawer for [LocalizationTerm]. Compiled only when I2Loc is present (I2LOC define, enforced by
// this assembly's defineConstraint). Draws the key as a dropdown of I2 terms, like I2's own [TermsPopup];
// without I2Loc no drawer exists and the key is edited as a plain string (UI Builder included).
#if I2LOC
using I2.Loc;

using UnityEditor;

using UnityEngine;

using Virtuademy.LocalizedComponents;

namespace Virtuademy.SDK.Environments.Utilities.Editor.I2Loc
{
    [CustomPropertyDrawer(typeof(LocalizationTermAttribute))]
    public class LocalizationTermDrawer : PropertyDrawer
    {
        private GUIContent[] termsCache;
        private int framesLeftBeforeUpdate;
        private string prevFilter;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            TermsPopup_Drawer.ShowGUICached(position, property, label, null, "",
                ref termsCache, ref framesLeftBeforeUpdate, ref prevFilter);
        }
    }
}
#endif
