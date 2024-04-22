using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class RadialRPCManagerPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Tooltip("The models and only the models of the items that we want the other player to see spawned when we press on the icon if the item in the radial Menu. The list must have the items in the same order as the radial Menu")]
        public List<GameObject> radialMenuItemModels;
    }
}
