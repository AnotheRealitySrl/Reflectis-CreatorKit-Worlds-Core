using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SittablePlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private int instantiationId;

        [SerializeField] private Transform sitTransform;
        [SerializeField] private Transform stepUpTransform;

        [SerializeField] private Collider interactableArea;
        [SerializeField] private bool isInteractable;


        public int InstantiationId => instantiationId;
        public Transform SitTransform => sitTransform;
        public Transform StepUpTransform => stepUpTransform;
        public Collider InteractableArea => interactableArea;
        public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    }
}
