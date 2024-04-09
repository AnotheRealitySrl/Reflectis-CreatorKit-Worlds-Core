using Reflectis.SDK.Utilities;
using UnityEngine;

using static Reflectis.SDK.InteractionNew.ContextualMenuManageable;

namespace Reflectis.SDK.CreatorKit
{
    public class ContextualMenuManageablePlaceholder : SceneComponentPlaceholderNetwork
    {
        #region Contextual menu

        [Header("Contextual menu")]
        [HelpBox("Please note that \"LockTransform\", \"ResetTransform\" and \"Duplicate\" options are not currently supported.", HelpBoxMessageType.Warning)]

        [SerializeField, Tooltip("Select how many options will be available in this menu")]
        private EContextualMenuOption contextualMenuOptions =
            EContextualMenuOption.Delete;

        public EContextualMenuOption ContextualMenuOptions { get => contextualMenuOptions; set => contextualMenuOptions = value; }

        #endregion
    }
}
