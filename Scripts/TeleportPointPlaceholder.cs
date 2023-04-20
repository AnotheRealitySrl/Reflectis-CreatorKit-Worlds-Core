using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class TeleportPointPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private string sceneAddressableName;

        public string SceneAddressableName => sceneAddressableName;
    }
}
