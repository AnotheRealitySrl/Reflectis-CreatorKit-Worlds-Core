using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class PickablePlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum PickState
        {
            Picked,
            Used,
            Dropped
        }

        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Pickable references")]
        [SerializeField] private PickState state = PickState.Dropped;

        public Role OwnershipMask => ownershipMask;
        public PickState State => state;
    }
}
