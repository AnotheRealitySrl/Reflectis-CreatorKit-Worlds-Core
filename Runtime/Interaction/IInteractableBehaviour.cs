using System.Threading.Tasks;

using static Reflectis.CreatorKit.Worlds.Core.Interaction.InteractableBehaviourBase;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IInteractableBehaviour
    {
        IInteractable InteractableRef { get; }
        bool IsIdleState { get; }
        bool LockHoverDuringInteraction { get; }
        public EBlockedState CurrentBlockedState { get; set; }

        Task Setup();

        void OnHoverStateEntered();
        void OnHoverStateExited();

        Task EnterInteractionState();
        Task ExitInteractionState();
    }
}
