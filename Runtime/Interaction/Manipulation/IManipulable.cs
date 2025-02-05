using System;

using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IManipulable : IInteractableBehaviour
    {
        UnityEvent OnGrabManipulableStart { get; set; }
        UnityEvent OnGrabManipulableEnd { get; set; }
        UnityEvent OnRayGrabManipulableStart { get; set; }
        UnityEvent OnRayGrabManipulableEnd { get; set; }

        UnityEvent<EManipulableState> OnCurrentStateChange { get; }

        EManipulableState CurrentInteractionState { get; }
        EManipulationInput CurrentManipulationInput { get; }

        public enum EManipulableState
        {
            Idle,
            BeforeManipulating,
            Manipulating
        }

        [Flags]
        public enum EManipulationMode
        {
            Translate = 1,
            Rotate = 2,
            Scale = 4
        }

        [Flags]
        public enum EVRInteraction
        {
            RayInteraction = 1,
            Hands = 2
        }

        [Flags]
        public enum EManipulationInput
        {
            None = 0,
            LeftRayInteraction = 1,
            RightRayInteraction = 2,
            LeftHand = 4,
            RightHand = 8,
            Mouse = 16,
        }
    }
}
