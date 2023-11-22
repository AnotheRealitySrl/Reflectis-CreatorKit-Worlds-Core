using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class DisableColliderPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private List<Collider> disableInVR = new List<Collider>();

        [SerializeField]
        private List<Collider> disableInWebGL = new List<Collider>();

        public List<Collider> DisableInVR { get => disableInVR; set => disableInVR = value; }
        public List<Collider> DisableInWebGL { get => disableInWebGL; set => disableInWebGL = value; }
    }
}
