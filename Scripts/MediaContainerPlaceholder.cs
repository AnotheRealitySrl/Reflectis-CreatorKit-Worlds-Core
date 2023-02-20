using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class MediaContainerPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Instantiation")]
        [SerializeField] private string addressableKey;
        [SerializeField] private bool instantiateOnInit;

        [Header("Networking")]
        [SerializeField] private int initializationId;

        [Header("Asset data")]
        [SerializeField] private string url;
        [SerializeField] private bool videoPlayOnAwake;

        [Header("Media player controller")]
        [SerializeField] private Sprite mediaSprite;
        [SerializeField] private List<Sprite> controllerSprites;
        [SerializeField] private List<string> controllerCallbacks;

        // Instantiation
        public string AddressableKey => addressableKey;
        public bool InstantiateOnInit => instantiateOnInit;

        // Networking
        public int InitializationId => initializationId;

        // Asset data
        public string Url => url;
        public bool VideoPlayOnAwake => videoPlayOnAwake;

        // Media player controller
        public Sprite MediaSprite => mediaSprite;
        public List<Sprite> ControllerSprites => controllerSprites;
        public List<string> ControllerCallbacks => controllerCallbacks;
    }
}
