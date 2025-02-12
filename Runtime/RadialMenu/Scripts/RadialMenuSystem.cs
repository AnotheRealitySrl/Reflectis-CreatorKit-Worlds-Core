using Reflectis.SDK.Core.SystemFramework;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.RadialMenu
{
    [CreateAssetMenu(fileName = "New Radial Menu", menuName = "RadialMenu/Create New Radial Menu")]
    public class RadialMenuSystem : BaseSystem
    {
        public GameObject radialMenuPrefab;
        public GameObject radialMenuNetworkPrefab;

        private GameObject instantiatedRadialMenu;

        public GameObject InstantiateRadialMenu(bool network)
        {
            if (!network)
            {
                radialMenuPrefab.SetActive(false);
                instantiatedRadialMenu = Instantiate(radialMenuPrefab);
            }
            else
            {
                radialMenuNetworkPrefab.SetActive(false);
                instantiatedRadialMenu = Instantiate(radialMenuNetworkPrefab);
            }

            return instantiatedRadialMenu;
        }
    }
}
