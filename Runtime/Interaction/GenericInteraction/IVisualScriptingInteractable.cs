using System;
using System.Threading.Tasks;

using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IVisualScriptingInteractable : IInteractableBehaviour
    {
        UnityEvent OnHoverGrabEnter { get; }
        UnityEvent OnHoverGrabExit { get; }
        UnityEvent OnHoverRayEnter { get; }
        UnityEvent OnHoverRayExit { get; }
        UnityEvent OnHoverMouseEnter { get; }
        UnityEvent OnHoverMouseExit { get; }
        EGenericInteractableState CurrentInteractionState { get; }
        bool SkipSelectState { get; }

        Task Interact();

        [Flags]
        public enum EGenericInteractableState
        {
            Idle = 0,
            SelectEntering = 1,
            Selected = 2,
            Interacting = 3,
            SelectExiting = 4,
        }

        [Flags]
        public enum EAllowedGenericInteractableState
        {
            Selected = 1,
            Interacting = 2,
            Hovered = 4,
        }

        [Flags]
        public enum EVRGenericInteraction
        {
            RayInteraction = 1,
            Hands = 2
        }
    }
}
