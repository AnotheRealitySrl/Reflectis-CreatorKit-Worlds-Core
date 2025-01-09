using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.CreatorKit.Core
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