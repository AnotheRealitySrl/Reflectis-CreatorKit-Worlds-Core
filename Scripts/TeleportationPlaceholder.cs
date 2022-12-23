using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Virtuademy.Core;

public class TeleportationPlaceholder : MonoBehaviour, IScenePlaceholder
{
    [Header("Common references")]
    [SerializeField] private Collider teleportCollider;
    [SerializeField] private Transform teleportDestination;

    [Header("VR references")]
    [SerializeField] private GameObject customReticleVR;

    public Collider TeleportCollider => teleportCollider;
    public Transform TeleportDestination => teleportDestination;
    public GameObject CustomReticleVR => customReticleVR;

    public void Init(SceneComponentsMapper mapper)
    {
        gameObject.AddComponent(mapper.TeleportationComponent);
    }
}
