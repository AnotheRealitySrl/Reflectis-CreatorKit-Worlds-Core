using SPACS.SDK.CharacterController;
using SPACS.SDK.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class DrawableBoardPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private Collider drawableArea;
        [SerializeField] private Transform drawingProjectionTransform;
        [SerializeField] private Button closeButton;
        [SerializeField] private Role ownershipMask;

        public Collider DrawableArea => drawableArea;
        public Transform DrawingProjectionTransform => drawingProjectionTransform;
        public Button CloseButton => closeButton;
        public Role OwnershipMask => ownershipMask;
    }
}

