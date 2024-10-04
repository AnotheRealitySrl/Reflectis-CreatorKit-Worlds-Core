using Reflectis.SDK.Utilities;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "AnotheReality/CreatorKit/POI/POI media element", fileName = "POIMediaElementConfig")]
    public class POIImagePlaceholder : POIBlockPlaceholder
    {
        [Space]
        [Header("Configurable stuff")]

        [SerializeField, Tooltip("An image can be referenced with a sprite.")]
        [OnChangedCall(nameof(OnImageChanged))]
        private Sprite image;

        public Sprite Image => image;

        public void OnImageChanged()
        {
#if UNITY_EDITOR
            SerializedObject so = new(GetComponentInChildren<Image>());
            so.FindProperty("m_Sprite").objectReferenceValue = image;
            so.ApplyModifiedProperties();
#endif
        }
    }
}
