using Reflectis.SDK.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class PanelPlaceholder : SceneComponentPlaceholderBase
    {
        [Serializable]
        public struct UISize
        {
            public float width;
            public float height;
        }

        [SerializeField]
        private Transform uiParent;
        [SerializeField]
        private UISize uiSize;

        public Transform UiParent { get => uiParent; }
        public UISize UiSize { get => uiSize; }

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
