using System;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IContextualMenuManageable : IInteractableBehaviour
    {
        EContextualMenuType ContextualMenuType { get; }

        public enum EContextualMenuInteractableState
        {
            Idle,
            Showing
        }

        public enum EContextualMenuType
        {
            Default,
            VideoPlayerConttroller,
            PresentationPlayerController
        }

        [Flags]
        public enum EContextualMenuOption
        {
            None = 0,
            LockTransform = 1,
            ResetTransform = 2,
            Duplicate = 4,
            Delete = 8,
            ColorPicker = 16,
            Explodable = 32,
            NonProportionalScale = 64,
        }

        void OnContextualMenuButtonClicked(EContextualMenuOption option);
    }
}
