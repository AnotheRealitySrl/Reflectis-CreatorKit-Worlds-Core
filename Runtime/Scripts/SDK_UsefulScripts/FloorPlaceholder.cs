using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class FloorPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private GameObject customReticleVR;

        public GameObject CustomReticleVR => customReticleVR;
    }
}

