using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(Camera))]
    public class MapCameraPlaceholder : SceneComponentPlaceholderBase
    {
        public Camera Cam => GetComponent<Camera>();

        private void Awake()
        {
            Cam.enabled = false;
        }
    }
}