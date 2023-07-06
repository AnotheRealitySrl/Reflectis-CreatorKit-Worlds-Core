using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SittablePlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private int instantiationId;

        [SerializeField] private Transform sitPosition;
        [SerializeField] private Transform stepUpPosition;

        [SerializeField] private Collider interactableArea;
        [SerializeField] private bool isInteractable;


        public int InstantiationId => instantiationId;
        public Transform SitPosition => sitPosition;
        public Transform StepUpPosition => stepUpPosition;
        public Collider InteractableArea => interactableArea;
        public bool IsInteractable => isInteractable;
    }
}
