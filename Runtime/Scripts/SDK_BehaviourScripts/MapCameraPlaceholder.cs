using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(Camera))]
    public class MapCameraPlaceholder : SceneComponentPlaceholderNetwork
    {
        private Camera cam;

        public Camera Cam => cam;

        private void Awake()
        {
            cam.enabled = false;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (cam == null)
            {
                cam = GetComponent<Camera>();
            }
        }
#endif
    }
}
