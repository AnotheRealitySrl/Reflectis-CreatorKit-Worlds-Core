using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class DisableBehaviourPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private List<MonoBehaviour> disableInVR = new List<MonoBehaviour>();

        [SerializeField]
        private List<MonoBehaviour> disableInWebGL = new List<MonoBehaviour>();

        public List<MonoBehaviour> DisableInVR { get => disableInVR; set => disableInVR = value; }
        public List<MonoBehaviour> DisableInWebGL { get => disableInWebGL; set => disableInWebGL = value; }
    }
}
