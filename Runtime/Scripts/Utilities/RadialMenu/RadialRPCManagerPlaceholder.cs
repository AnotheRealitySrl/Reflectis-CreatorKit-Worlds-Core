using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class RadialRPCManagerPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Tooltip("The radial rpc manager needs a reference to the radial menu in order to work correctly")]
        public RadialMenuPlaceholder radialMenuPlaceholder; //reference to the radial menu
    }
}
