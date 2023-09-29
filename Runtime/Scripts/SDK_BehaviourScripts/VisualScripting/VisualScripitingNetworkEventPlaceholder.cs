using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class VisualScripitingNetworkEventPlaceholder : SceneComponentPlaceholderNetwork
    {
        [HideInInspector]
        public UnityEvent<string> action;

        private void Start()
        {
            action.AddListener(VisualScriptingEvent);
        }

        private void VisualScriptingEvent(string eventName)
        {
            CustomEvent.Trigger(this.gameObject, eventName, null);
        }

        public void ActionInvoke(string eventName)
        {
            action?.Invoke(eventName);
        }
    }
}