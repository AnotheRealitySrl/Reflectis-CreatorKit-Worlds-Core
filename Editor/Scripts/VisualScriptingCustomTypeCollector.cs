#if UNITY_EDITOR
using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    [CreateAssetMenu(menuName = "AnotheReality/Editor/VisualScriptingCustomTypeCollector", fileName = "VisualScriptingCustomTypeCollector")]
    public class VisualScriptingCustomTypeCollector : ScriptableObject
    {
        [SerializeField]
        private TextAsset[] customTypeTexts;

        public TextAsset[] CustomTypeTexts { get => customTypeTexts; set => customTypeTexts = value; }
    }
}
#endif