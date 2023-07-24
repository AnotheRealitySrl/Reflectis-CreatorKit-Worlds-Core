using Reflectis.SDK.Interaction;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class MediaContainerPlaceholder : SceneComponentPlaceholderBase
    {
        [Serializable]
        public class ActionScriptablesContainer
        {
            public List<ActionScriptable> scriptableActionsList = new();
        }

        [Header("Instantiation")]
        [SerializeField] private string addressableKey;
        [SerializeField] private bool instantiateOnInit;
        [SerializeField] private bool activateOnInit = true;

        [Header("Asset data")]
        [SerializeField] private string url;
        [SerializeField] private bool videoPlayOnAwake;

        [Header("Media player controller")]
        [SerializeField] private Sprite mediaSprite;
        [SerializeField] private List<Sprite> controllerSprites;
        [SerializeField] private List<string> controllerCallbacks;
        [SerializeField] private List<ActionScriptablesContainer> controllerScriptableActions;
        [SerializeField] private GameObject externalController;
        [SerializeField] private GameObject buttonTemplate;
        [SerializeField] private GameObject separatorTemplate;

        [Header("Platform-specific configuration")]
        [SerializeField] private List<GameObject> vrComponents;
        [SerializeField] private List<GameObject> desktopComponents;

        // Instantiation
        public string AddressableKey => addressableKey;
        public bool InstantiateOnInit => instantiateOnInit;
        public bool ActivateOnInit => activateOnInit;

        // Asset data
        public string Url => url;
        public bool VideoPlayOnAwake => videoPlayOnAwake;

        // Media player controller
        public Sprite MediaSprite => mediaSprite;
        public List<Sprite> ControllerSprites => controllerSprites;
        public List<string> ControllerCallbacks => controllerCallbacks;
        public List<ActionScriptablesContainer> ControllerScriptableActions => controllerScriptableActions;
        public GameObject ExternalController => externalController;
        public GameObject ButtonTemplate => buttonTemplate;
        public GameObject SeparatorTemplate => separatorTemplate;

        // Media player controller
        public List<GameObject> VrComponents => vrComponents;
        public List<GameObject> DesktopComponents => desktopComponents;
    }
}
