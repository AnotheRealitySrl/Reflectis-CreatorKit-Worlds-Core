using Reflectis.SDK.Utilities;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "AnotheReality/CreatorKit/POI/POI media element", fileName = "POIMediaElementConfig")]
    public class POIImagePlaceholder : POIBlockPlaceholder
    {
        public override string AddressableKey => "POIImageBlock";

        [SerializeField, Tooltip("Add an image that will be shown in the POI.")]
        [OnChangedCall(nameof(OnImageChanged))]
        private Sprite image;

        public Sprite Image => image;

        public void OnImageChanged()
        {
            SerializedObject so = new(GetComponentInChildren<Image>());
            so.FindProperty("m_Sprite").objectReferenceValue = image;
            so.ApplyModifiedProperties();
        }
    }
}
