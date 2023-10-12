using Reflectis.SDK.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class MascottePlaceholder : SceneComponentPlaceholderBase
    {
        [Serializable]
        public struct UISize
        {
            public float width;
            public float height;
        }

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform uiParent;
        [SerializeField]
        private UISize uiSize;
        [SerializeField]
        private BaseInteractableGO pan;
        [SerializeField]
        private string[] faqCategory;

        public Animator Animator { get => animator; }
        public Transform UiParent { get => uiParent; }
        public BaseInteractableGO Pan { get => pan; }
        public UISize UiSize { get => uiSize;  }
        public string[] FaqCategory { get => faqCategory;  }

        private void OnDrawGizmos()
        {
            if (uiParent != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(uiParent.position, new Vector3(UiSize.width, UiSize.height, 0.01f));
            }
        }
    }
}
