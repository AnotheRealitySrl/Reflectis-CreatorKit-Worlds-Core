using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    /// <summary>
    /// Common interaface for any interactable entity.
    /// </summary>
    public interface IInteractable
    {
        [Flags]
        public enum EInteractableType
        {
            VisualScriptingInteractable = 1,
            Manipulable = 2,
            ContextualMenuInteractable = 4
        }

        public enum EInteractionState
        {
            Idle,
            Hovered,
            Interaction
        }

        List<IInteractableBehaviour> InteractableBehaviours { get; }
        EInteractionState InteractionState { get; set; }

        bool LockHoverDuringInteraction { get; }

        GameObject GameObjectRef { get; }
        List<Collider> InteractionColliders { get; }

        Task Setup();
        void OnHoverEnter();
        void OnHoverExit();

    }
}