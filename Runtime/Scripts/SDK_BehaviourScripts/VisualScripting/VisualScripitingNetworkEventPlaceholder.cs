using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class VisualScripitingNetworkEventPlaceholder : SceneComponentPlaceholderNetwork
    {
        [HideInInspector]
        public UnityEvent<string> action;

        public void ActionInvoke(string eventName)
        {
            action?.Invoke(eventName);
        }
    }
}