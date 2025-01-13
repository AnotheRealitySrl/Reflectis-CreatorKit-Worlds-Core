using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class SceneChangerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField, Tooltip("Name of an environment that has a static event associated from the backoffice")]

        private string sceneAddressableName;



        public string SceneAddressableName => sceneAddressableName;
    }
}
