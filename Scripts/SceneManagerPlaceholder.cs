using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SceneManagerPlaceholder : MonoBehaviour
    {
        #region Enums

        public enum EAppState
        {
            Login = 0,
            Welcome2D = 1,
            Learning = 2,
            Welcome3D = 3
        }

        #endregion

        #region Private/protected variables

        [SerializeField] protected EAppState appState;

        #endregion

        #region Properties 

        public EAppState AppState => appState;

        #endregion
    }
}
