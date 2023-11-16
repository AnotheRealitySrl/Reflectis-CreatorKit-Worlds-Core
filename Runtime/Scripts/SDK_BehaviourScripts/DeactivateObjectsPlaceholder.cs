using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class DeactivateObjectsPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private List<GameObject> disableInVR = new List<GameObject>();

        [SerializeField]
        private List<GameObject> disableInWebGL = new List<GameObject>();

        public List<GameObject> DisableInVR { get => disableInVR; set => disableInVR = value; }
        public List<GameObject> DisableInWebGL { get => disableInWebGL; set => disableInWebGL = value; }
    }
}
