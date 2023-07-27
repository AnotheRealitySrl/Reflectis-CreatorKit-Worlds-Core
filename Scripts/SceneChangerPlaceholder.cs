using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SceneChangerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private string sceneAddressableName;
        /// <summary>
        /// <para>
        /// If true, the user enters the generic scene without random event creation. Good for lobby scenarios.
        /// </para>
        /// <para>
        /// If false, a new random event is created.
        /// </para>
        /// </summary>
        [SerializeField]
        private bool enterGenericInstance;

        public string SceneAddressableName => sceneAddressableName;
        /// <summary>
        /// <para>
        /// If true, the user enters the generic scene without random event creation. Good for lobby scenarios.
        /// </para>
        /// <para>
        /// If false, a new random event is created.
        /// </para>
        /// </summary>
        public bool EnterGenericInstance => enterGenericInstance;
    }
}
