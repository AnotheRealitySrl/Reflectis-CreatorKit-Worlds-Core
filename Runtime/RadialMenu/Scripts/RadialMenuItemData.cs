using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.RadialMenu
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Radial Item")]
    public class RadialMenuItemData : ScriptableObject
    {
        public Sprite icon; //the icon used in the radial menu
        public Sprite background; //background of the image
        public string itemName; //name of the item used also to instantiate the prefab
        public GameObject itemPrefab; //the prefab of the object to be spawned
        public bool emptyObject = false;  //if true, rappresent the empty object
    }
}
