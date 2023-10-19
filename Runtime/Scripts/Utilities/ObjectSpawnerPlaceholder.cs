
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Reflectis.SDK.CreatorKit
{
    public class ObjectSpawnerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private SpawnableData spawnableData;
        [SerializeField]
        private InputActionReference vrInput;
        [SerializeField]
        private InputActionReference desktopInput;

        public InputActionReference VrInput { get => vrInput; }
        public InputActionReference DesktopInput { get => desktopInput; }
        public SpawnableData Data { get => spawnableData; }
    }

    [Serializable]
    public class SpawnableData
    {
        [SerializeField]
        private GameObject objPrefab;
        [SerializeField]
        private int fovAngle = 90;
        [SerializeField]
        private int rayCount = 2;
        [SerializeField]
        private float viewDistance = 50f;
        [SerializeField]
        private float startingAngle = 0;
        [SerializeField]
        private Vector3 originOffset = Vector3.zero;
        [SerializeField]
        private LayerMask layerMask;


        public GameObject ObjPrefab { get => objPrefab; }
        public int FovAngle { get => fovAngle; }
        public int RayCount { get => rayCount; }
        public float ViewDistance { get => viewDistance; }
        public float StartingAngle { get => startingAngle; }
        public Vector3 OriginOffset { get => originOffset; }
        public LayerMask LayerMask { get => layerMask; }
    }
}
