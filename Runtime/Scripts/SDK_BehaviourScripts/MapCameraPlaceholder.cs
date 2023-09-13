using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(Camera))]
    public class MapCameraPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("MapValues")]
        [SerializeField] private Camera cam;

        public Camera Cam  => cam;
    }
}
