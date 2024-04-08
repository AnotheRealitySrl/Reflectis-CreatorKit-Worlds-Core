using Reflectis.SDK.Utilities;

using UnityEngine;

using static Reflectis.SDK.InteractionNew.ContextualMenuManageable;

namespace Reflectis.SDK.CreatorKit
{
    public class ContextualMenuManageablePlaceholder : SceneComponentPlaceholderNetwork
    {
        #region Contextual menu

        [Header("Contextual menu")]
        [HelpBox("Please note that only \"Delete\" option is currently supported. Other options will not be considered", HelpBoxMessageType.Warning)]

        [SerializeField, Tooltip("Select how many options will be available in this menu")]
        private EContextualMenuOption contextualMenuOptions =
            EContextualMenuOption.Delete;

        [SerializeField, Tooltip("Select the type of contextual menu. Use \"Default\" one unless you are working with a media player")]
        private EContextualMenuType contextualMenuType = EContextualMenuType.Default;


        public EContextualMenuOption ContextualMenuOptions { get => contextualMenuOptions; set => contextualMenuOptions = value; }
        public EContextualMenuType ContextualMenuType { get => contextualMenuType; set => contextualMenuType = value; }

        #endregion
    }
}
