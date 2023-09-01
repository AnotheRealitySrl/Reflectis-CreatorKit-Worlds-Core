using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.DTO;

namespace Reflectis.SDK.CreatorKit
{
    public class PickablePlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum PickState
        {
            Dropped = 1,
            Picked = 2,
            Used = 3
        }

        //[Header("Network settings")]
        //[SerializeField] private Role ownershipMask;

        private PickState state = PickState.Dropped;

        //public Role OwnershipMask { get => ownershipMask; set => ownershipMask = value; }
        public PickState State { get => state; set => state = value; }
    }
}
