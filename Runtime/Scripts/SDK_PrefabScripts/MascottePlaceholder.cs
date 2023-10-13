using Reflectis.SDK.Interaction;
using System;
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
        private bool panOnInit;
        [SerializeField]
        private BaseInteractableGO pan;
        [SerializeField]
        private string[] faqCategory;

        public Animator Animator { get => animator; }
        public BaseInteractableGO Pan { get => pan; }
        public string[] FaqCategory { get => faqCategory;  }
        public bool PanOnInit { get => panOnInit; }
    }
}
