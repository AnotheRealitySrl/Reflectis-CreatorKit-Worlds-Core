using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class ShowHidePlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum HideShowState
        {
            Hiding,
            Showing
        }

        [Header("Show Hide references")]
        [SerializeField] private bool haveToToggle;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private bool canBeInteractable;
        [SerializeField] private bool haveToWait;
        [SerializeField] private bool haveToDisableAtStart = false;
        [SerializeField] private HideShowState state = HideShowState.Hiding;
        [SerializeField] private float timeToTriggerAction;
        [SerializeField] private GameObject fatherConnecter;
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Collider colliderArea;

        public bool HaveToToggle => haveToToggle;
        public List<GameObject> Connectables => connectables;
        public bool CanBeInteractable => canBeInteractable;
        public HideShowState State => state;
        public float TimeToTriggerAction => timeToTriggerAction;
        public bool HaveToWait => haveToWait;
        public bool HaveToDisableAtStart => haveToDisableAtStart;
        public GameObject FatherConnecter => fatherConnecter;
        public MeshRenderer Mesh => mesh;
        public Collider ColliderArea => colliderArea;
    }
}
