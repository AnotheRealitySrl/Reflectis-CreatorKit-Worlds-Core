using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class MediaContainerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private int initializationId;
        [SerializeField] private string addressableKey;
        [SerializeField] private Sprite mediaSprite;
        [SerializeField] private List<Sprite> controllerSprites;
        [SerializeField] private List<string> controllerCallbacks;

        public int InitializationId => initializationId;
        public string AddressableKey => addressableKey;
        public Sprite MediaSprite => mediaSprite;
        public List<Sprite> ControllerSprites => controllerSprites;
        public List<string> ControllerCallbacks => controllerCallbacks;
    }
}
