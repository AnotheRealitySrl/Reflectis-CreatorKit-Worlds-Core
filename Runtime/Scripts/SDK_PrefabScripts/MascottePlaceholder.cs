using Reflectis.SDK.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class MascottePlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform nameInputParent;
        [SerializeField]
        private Transform uiParent;
        [SerializeField]
        private BaseInteractableGO pan;

        public Animator Animator { get => animator; set => animator = value; }
        public Transform NameInputParent { get => nameInputParent; set => nameInputParent = value; }
        public Transform UiParent { get => uiParent; set => uiParent = value; }
        public BaseInteractableGO Pan { get => pan; set => pan = value; }
    }
}
