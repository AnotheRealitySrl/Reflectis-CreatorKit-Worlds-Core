using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class SceneChangerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private string sceneAddressableName;

        public string SceneAddressableName => sceneAddressableName;
    }
}
