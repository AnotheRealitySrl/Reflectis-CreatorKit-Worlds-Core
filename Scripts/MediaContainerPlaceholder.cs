using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class MediaContainerPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Networking")]
        [SerializeField] private int initializationId;

        [Header("Component id")]
        [SerializeField] private string addressableKey;

        [Header("Asset data")]
        [SerializeField] private string url;
        [SerializeField] private bool videoPlayOnAwake;

        [Header("Media player controller")]
        [SerializeField] private Sprite mediaSprite;
        [SerializeField] private List<Sprite> controllerSprites;
        [SerializeField] private List<string> controllerCallbacks;

        // Networking
        public int InitializationId => initializationId;

        // Component id
        public string AddressableKey => addressableKey;

        // Asset data
        public string Url => url;
        public bool VideoPlayOnAwake => videoPlayOnAwake;

        // Media player controller
        public Sprite MediaSprite => mediaSprite;
        public List<Sprite> ControllerSprites => controllerSprites;
        public List<string> ControllerCallbacks => controllerCallbacks;
    }
}
