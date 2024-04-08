using Reflectis.SDK.Utilities;

using System;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

using static Reflectis.SDK.InteractionNew.GenericInteractable;

namespace Reflectis.SDK.CreatorKit
{
    public class GenericInteractablePlaceholder : SceneComponentPlaceholderBase
    {
        #region Shared settings

        [Header("Interaction settings")]

        [SerializeField, Tooltip("The colliders that will be recognized by the interactable behaviours")]
        private List<Collider> interactionColliders = new();

        public List<Collider> InteractionColliders => interactionColliders;

        #endregion

        #region Generic interaction

        [Header("Generic interaction definition")]

        [SerializeField, Tooltip("If set to true the hover callback won't be called if the generic interactable is in a " +
            "state that is different from idle. As an example if the interactable is selected, it will keep the hover state " +
            "until the selection is over and the user is not hovering the object.")]
        private bool lockHoverDuringInteraction;

        [SerializeField, Tooltip("Reference to the script machine that describes what happens during interaction events." +
            "Utilize \"GenericInteractableHoverEnter\",\"GenericInteractableHoverExit\",\"GenericInteractableSelectEnter\",\"GenericInteractableSelectExit\"" +
            " and \"GenericInteractableInteract\" nodes to custumize your interactions")]
        private ScriptMachine interactionsScriptMachine;

        [SerializeField, Tooltip("Check if you need special operations to do after the object is destroyed while selected.")]
        private bool needUnselectOnDestroyScriptMachine;

        [SerializeField, Tooltip("Reference to the script machine that describes what happens if the object is destroyed while selected." +
            "The script machine has to be assigned to a different empty gameobject! " +
            "Utilize \"GenericInteractableUnselectOnDestroy\" node to custumize the interaction")]
        [DrawIf(nameof(needUnselectOnDestroyScriptMachine), true)]
        private ScriptMachine unselectOnDestroyScriptMachine;

        [Header("Allowed states")]

        [SerializeField, Tooltip("Choose which state are enabled on this object in desktop platforms.")]
        private EAllowedGenericInteractableState desktopAllowedStates = (EAllowedGenericInteractableState)~0;

        [SerializeField, Tooltip("Choose which state are enabled on this object in VR platforms.")]
        private EAllowedGenericInteractableState vrAllowedStates = (EAllowedGenericInteractableState)~0;

        [HideInInspector]
        public Action<GameObject> OnSelectedActionVisualScripting;

        public ScriptMachine InteractionsScriptMachine => interactionsScriptMachine;
        public bool NeedUnselectOnDestroyScriptMachine => needUnselectOnDestroyScriptMachine;
        public ScriptMachine UnselectOnDestroyScriptMachine => unselectOnDestroyScriptMachine;

        public EAllowedGenericInteractableState DesktopAllowedStates => desktopAllowedStates;
        public EAllowedGenericInteractableState VRAllowedStates => vrAllowedStates;

        public bool LockHoverDuringInteraction => lockHoverDuringInteraction;

        #endregion

    }
}
