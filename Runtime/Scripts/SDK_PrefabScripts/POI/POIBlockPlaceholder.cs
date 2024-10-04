using Reflectis.SDK.Utilities;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public abstract class POIBlockPlaceholder : MonoBehaviour
    {
        [HelpBox("To edit the position/dimension of a block, you can simply modify the values of its RectTransform. " +
            "To edit the content of a block, do not modify the values of its children components " +
            "(since any change will be ignored), but edit the fields available in the \"Configurable stuff\" " +
            "section of this inspector. You will see a preview of how the block will be displayed at runtime. ",
            HelpBoxMessageType.Info)]

        [Tooltip("Do not edit the addressable key.")]
        [SerializeField] private string addressableKey;

        public string AddressableKey => addressableKey;
    }
}
