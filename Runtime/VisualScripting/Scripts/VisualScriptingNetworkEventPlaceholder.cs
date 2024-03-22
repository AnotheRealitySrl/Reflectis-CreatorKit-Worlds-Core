using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class VisualScriptingNetworkEventPlaceholder : SceneComponentPlaceholderNetwork
    {
        [HideInInspector]
        public UnityEvent<string> action = new UnityEvent<string>();

        public void ActionInvoke(string eventName)
        {
            action?.Invoke(eventName);
        }
    }
}