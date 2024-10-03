using System;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [Serializable]
    public abstract class POIBlockPlaceholder : MonoBehaviour
    {
        public abstract string AddressableKey { get; }
    }
}
