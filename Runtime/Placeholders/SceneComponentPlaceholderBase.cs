
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.Placeholders
{
    [RequireComponent(typeof(SceneObjectId))]
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        protected SceneObjectId sceneObjectId;

        [SerializeField] private bool automaticSetup = true;

        public bool AutomaticSetup => automaticSetup;

        public int UniqueID
        {
            get
            {
                if (sceneObjectId == null)
                {
                    sceneObjectId = GetComponent<SceneObjectId>();
                }
                return GetComponent<SceneObjectId>().UniqueID;
            }
        }
    }
}