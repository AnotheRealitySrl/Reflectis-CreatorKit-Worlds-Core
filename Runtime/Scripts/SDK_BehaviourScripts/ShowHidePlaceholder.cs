using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class ShowHidePlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum HideShowState
        {
            Hiding,
            Showing
        }

        [Header("Show Hide references")]
        [SerializeField] private bool haveToSwitchOnlyOnce = false;
        [SerializeField] private bool canBeInteractable = true;
        [SerializeField] private bool haveToWait;
        [SerializeField] private float timeToTriggerAction;
        [SerializeField] private HideShowState state = HideShowState.Showing;
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Collider colliderArea;

        public bool HaveToSwitchOnlyOnce => haveToSwitchOnlyOnce;
        public bool CanBeInteractable => canBeInteractable;
        public HideShowState State => state;
        public float TimeToTriggerAction => timeToTriggerAction;
        public bool HaveToWait => haveToWait;
        public MeshRenderer Mesh => mesh;
        public Collider ColliderArea => colliderArea;
    }
}
