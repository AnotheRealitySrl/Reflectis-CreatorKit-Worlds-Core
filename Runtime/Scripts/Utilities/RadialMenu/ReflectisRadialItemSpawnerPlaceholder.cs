using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class ReflectisRadialItemSpawnerPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Tooltip("This spawner needs a reference to the radial menu in order to work correctly")]
        public RadialMenuPlaceholder radialMenuPlaceholder; //reference to the radial menu
    }
}
