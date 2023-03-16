using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Virtuademy.Placeholders
{
    public class MediaContainerPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Instantiation")]
        [SerializeField] private string addressableKey;
        [SerializeField] private bool instantiateOnInit;
        [SerializeField] private bool activateOnInit = true;

        [Header("Networking")]
        [SerializeField] private int initializationId;

        [Header("Asset data")]
        [SerializeField] private string url;
        [SerializeField] private bool videoPlayOnAwake;

        [Header("Media player controller")]
        [SerializeField] private Sprite mediaSprite;
        [SerializeField] private List<Sprite> controllerSprites;
        [SerializeField] private List<string> controllerCallbacks;
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

        // Networking
        public int InitializationId => initializationId;

        // Asset data
        public string Url => url;
        public bool VideoPlayOnAwake => videoPlayOnAwake;

        // Media player controller
        public Sprite MediaSprite => mediaSprite;
        public List<Sprite> ControllerSprites => controllerSprites;
        public List<string> ControllerCallbacks => controllerCallbacks;
        public GameObject ExternalController => externalController;
        public GameObject ButtonTemplate => buttonTemplate;
        public GameObject SeparatorTemplate => separatorTemplate;

        // Media player controller
        public List<GameObject> VrComponents => vrComponents;
        public List<GameObject> DesktopComponents => desktopComponents;
    }
}
