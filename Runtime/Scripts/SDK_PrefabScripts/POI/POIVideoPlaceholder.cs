using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class POIVideoPlaceholder : POIBlockPlaceholder
    {
        public override string AddressableKey => "POIVideoBlock";

        [SerializeField] private string url;

        public string Url => url;
    }
}
