using Reflectis.SDK.Utilities;

using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class POIPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Read only references. Do no edit them, unless you need to do some heavy customizations.")]
        [SerializeField] private RectTransform activator;
        [SerializeField] private RectTransform panel;

        [HelpBox("To edit the content of each block of the POI, please double-click on the item in the list." +
            "You will be redirected to the placeholder where you can customize the content.", HelpBoxMessageType.Info)]

        [SerializeField] private List<POIBlockPlaceholder> poiElements;

        public List<POIBlockPlaceholder> POIElements => poiElements;

        public Transform Activator => activator;
        public Transform Panel => panel;
    }
}
