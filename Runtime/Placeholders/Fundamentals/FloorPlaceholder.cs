using UnityEngine;

namespace Virtuademy.CreatorKit.Worlds.Core.Placeholders
{
    public class FloorPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private GameObject customReticleVR;

        public GameObject CustomReticleVR => customReticleVR;
    }
}

