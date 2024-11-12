using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        [SerializeField] private bool automaticSetup = true;

        public bool AutomaticSetup => automaticSetup;

    }
}