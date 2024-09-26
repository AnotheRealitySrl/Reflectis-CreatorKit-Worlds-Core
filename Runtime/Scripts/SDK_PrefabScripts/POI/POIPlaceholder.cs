using Reflectis.SDK.Utilities;

using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class POIPlaceholder : SceneComponentPlaceholderBase
    {
        [HelpBox("To edit the content of each block of the POI, please double-click on the item in the list." +
            "You will be redirected to the placeholder where you can customize the content.", HelpBoxMessageType.Info)]

        [SerializeField] private List<POIBlockPlaceholder> poiElements;

        public List<POIBlockPlaceholder> POIElements => poiElements;
    }
}
