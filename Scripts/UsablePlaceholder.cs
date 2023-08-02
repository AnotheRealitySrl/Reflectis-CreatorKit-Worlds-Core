using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class UsablePlaceholder : SceneComponentPlaceholderBase
    {
        public enum UseInteractionState
        {
            SingleUse,
            MultipleUse
        }

        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Pickable references")]
        [SerializeField] private UseInteractionState state = UseInteractionState.SingleUse;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private bool canBeUsed;



        public Role OwnershipMask => ownershipMask;
        public UseInteractionState State => state;
        public List<GameObject> Connectables => connectables;
        public bool CanBeUsed => canBeUsed;
    }
}
