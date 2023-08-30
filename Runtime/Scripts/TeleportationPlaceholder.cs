using System;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class TeleportationPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Common references")]
        [SerializeField] private Collider teleportCollider;
        [SerializeField] private Transform teleportDestination;

        [Header("VR references")]
        [SerializeField] private GameObject customReticleVR;

        public Collider TeleportCollider => teleportCollider;
        public Transform TeleportDestination => teleportDestination;
        public GameObject CustomReticleVR => customReticleVR;
    }
}