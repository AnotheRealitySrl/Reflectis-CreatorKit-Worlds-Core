using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class TeleportationDestinationPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string teleportAreaName;

        public string TeleportAreaName => teleportAreaName;
    }
}
