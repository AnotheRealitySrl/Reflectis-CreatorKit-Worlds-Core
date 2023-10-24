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

        private PickState state = PickState.Dropped;

        public PickState State { get => state; set => state = value; }
    }
}
