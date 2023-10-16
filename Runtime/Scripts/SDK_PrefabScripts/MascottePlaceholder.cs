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
        private string mascotteName;
        [Header("Animator")]
        [SerializeField]
        private Animator animator;
        [Header("Pan")]
        [SerializeField] 
        private bool panOnInit;
        [SerializeField]
        private BaseInteractableGO pan;


        public Animator Animator { get => animator; }
        public BaseInteractableGO Pan { get => pan; }
        public bool PanOnInit { get => panOnInit; }
        public string MascotteName { get => mascotteName; set => mascotteName = value; }
    }
}
