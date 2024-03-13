using Reflectis.SDK.InteractionNew;
using Reflectis.SDK.Utilities;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Reflectis.SDK.InteractionNew.ContextualMenuManageable;
using static Reflectis.SDK.InteractionNew.GenericInteractable;
using static Reflectis.SDK.InteractionNew.IInteractable;
using static Reflectis.SDK.InteractionNew.Manipulable;

namespace Reflectis.SDK.CreatorKit
{
    public class InteractablePlaceholder : SceneComponentPlaceholderNetwork
    {
        #region Shared settings

        [Header("Interaction settings")]

        [SerializeField, Tooltip("The colliders that will be recognized by the interactable behaviours")]
        private List<Collider> interactionColliders = new();

        [SerializeField, Tooltip("Which kind of interaction should this object have?")]
        private EInteractableType interactionModes;

        public List<Collider> InteractionColliders => interactionColliders;
        public EInteractableType InteractionModes { get => interactionModes; set => interactionModes = value; }

        #endregion

        #region Manipulation

        [Header("Manipulation section")]

        [SerializeField, Tooltip("Translate, rotate and scale.")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable)]
        private EManipulationMode manipulationMode = (EManipulationMode)~0;

        [SerializeField, Tooltip("Enables non-proportional scale on this object.")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(manipulationMode), EManipulationMode.Scale)]
        private bool nonProportionalScale;


        [Header("VR manipulation settings")]

        [SerializeField, Tooltip("Enables hand and ray interaction on this object")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable)]
        private EVRInteraction vrInteraction = (EVRInteraction)~0;

        [SerializeField, Tooltip("A dynamic attach means that the object won't snap to the center of gravity")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(vrInteraction), EVRInteraction.Hands | EVRInteraction.RayInteraction)]
        private bool dynamicAttach;

        [SerializeField, Tooltip("Resets the rotation on one or more axes when the interaction ends (VR-only)")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(vrInteraction), EVRInteraction.Hands | EVRInteraction.RayInteraction)]
        private bool adjustRotationOnRelease;

        [SerializeField, Tooltip("Resets the rotation on the X axis")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(vrInteraction), EVRInteraction.Hands | EVRInteraction.RayInteraction), DrawIf(nameof(adjustRotationOnRelease), true)]
        private bool realignAxisX = true;

        [SerializeField, Tooltip("Resets the rotation on the Y axis")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(vrInteraction), EVRInteraction.Hands | EVRInteraction.RayInteraction), DrawIf(nameof(adjustRotationOnRelease), true)]
        private bool realignAxisY = false;

        [SerializeField, Tooltip("Resets the rotation on the Z axis")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(vrInteraction), EVRInteraction.Hands | EVRInteraction.RayInteraction), DrawIf(nameof(adjustRotationOnRelease), true)]
        private bool realignAxisZ = true;

        [SerializeField, Tooltip("How much time is needed to reset the rotation")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(vrInteraction), EVRInteraction.Hands | EVRInteraction.RayInteraction), DrawIf(nameof(adjustRotationOnRelease), true)]
        private float realignDurationTimeInSeconds = 1f;


        [Header("WebGL manipulation settings")]

        [SerializeField, Tooltip("If selected, the object will face the camera on mouse interaction")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(manipulationMode), EManipulationMode.Translate)]
        private bool mouseLookAtCamera;

        [SerializeField, Tooltip("If specified, a dynamic attach will be generated on interaction")]
        [DrawIf(nameof(interactionModes), EInteractableType.Manipulable), DrawIf(nameof(manipulationMode), EManipulationMode.Translate)]
        private Transform attachTransform;

        public EManipulationMode ManipulationMode { get => manipulationMode; set => manipulationMode = value; }
        public EVRInteraction VRInteraction { get => vrInteraction; set => vrInteraction = value; }
        public bool DynamicAttach => dynamicAttach;
        public bool MouseLookAtCamera => mouseLookAtCamera;
        public Transform AttachTransform => attachTransform;
        public bool NonProportionalScale { get => nonProportionalScale; set => nonProportionalScale = value; }
        public bool AdjustRotationOnRelease => adjustRotationOnRelease;
        public bool RealignAxisX => realignAxisX;
        public bool RealignAxisY => realignAxisY;
        public bool RealignAxisZ => realignAxisZ;
        public float RealignDurationTimeInSeconds => realignDurationTimeInSeconds;

        #endregion

        #region Generic interaction

        [Header("Generic interaction scriptable actions")]

        [SerializeField, Tooltip("Actions that are triggered when hovering the object.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onHoverEnterActions = new();

        [SerializeField, Tooltip("Actions that are triggered when finishing the hover on the object.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onHoverExitActions = new();

        [SerializeField, Tooltip("Actions that are triggered when selecting the object. " +
            "During this state the interaction with the object is disabled.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onSelectingActions = new();

        [SerializeField, Tooltip("Actions that are triggered automatically when the \"Selecting\" states terminates. " +
            "In this state it's possible to interact with the object")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onSelectedActions = new();

        [SerializeField, Tooltip("Actions that are triggered when deselecting the object. " +
            "During this state the interaction with the object is disabled.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onDeselectingActions = new();

        [SerializeField, Tooltip("Actions that are triggered automatically when the \"Deselecting\" states terminates. " +
            "In this state it's possible to interact with the object")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onDeselectedActions = new();

        [SerializeField, Tooltip("Actions that are triggered automatically when entering the \"Interact\" state.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onInteractActions = new();

        [SerializeField, Tooltip("Actions that are triggered automatically when exiting the \"Interact\" state.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private List<AwaitableScriptableAction> onInteractFinishActions = new();

        [SerializeField]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private ScriptMachine interactionsScriptMachine;

        [Header("Allowed states")]

        [SerializeField, Tooltip("Choose which state are enabled on this object in desktop platforms.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private EAllowedGenericInteractableState desktopAllowedStates = (EAllowedGenericInteractableState)~0;

        [SerializeField, Tooltip("Choose which state are enabled on this object in VR platforms.")]
        [DrawIf(nameof(interactionModes), EInteractableType.GenericInteractable)]
        private EAllowedGenericInteractableState vrAllowedStates = (EAllowedGenericInteractableState)~0;

        [HideInInspector]
        public Action<GameObject> OnSelectedActionVisualScripting;

        public List<AwaitableScriptableAction> OnHoverEnterActions => onHoverEnterActions;
        public List<AwaitableScriptableAction> OnHoverExitActions => onHoverExitActions;
        public List<AwaitableScriptableAction> OnSelectingActions => onSelectingActions;
        public List<AwaitableScriptableAction> OnSelectedActions => onSelectedActions;
        public List<AwaitableScriptableAction> OnDeselectingActions => onDeselectingActions;
        public List<AwaitableScriptableAction> OnDeselectedActions => onDeselectedActions;
        public List<AwaitableScriptableAction> OnInteractActions => onInteractActions;
        public List<AwaitableScriptableAction> OnInteractFinishActions => onInteractFinishActions;
        public ScriptMachine InteractionsScriptMachine => interactionsScriptMachine;

        public EAllowedGenericInteractableState DesktopAllowedStates => desktopAllowedStates;
        public EAllowedGenericInteractableState VRAllowedStates => vrAllowedStates;

        #endregion

        #region Contextual menu

        [Header("Contextual menu")]

        [SerializeField, Tooltip("Select how many options will be available in this menu")]
        [DrawIf(nameof(interactionModes), EInteractableType.ContextualMenuInteractable)]
        private EContextualMenuOption contextualMenuOptions =
            EContextualMenuOption.LockTransform |
            EContextualMenuOption.ResetTransform |
            EContextualMenuOption.Duplicate |
            EContextualMenuOption.Delete;

        [SerializeField, Tooltip("Select the type of contextual menu. Use \"Default\" one unless you are working with a media player")]
        [DrawIf(nameof(interactionModes), EInteractableType.ContextualMenuInteractable)]
        private EContextualMenuType contextualMenuType = EContextualMenuType.Default;


        public EContextualMenuOption ContextualMenuOptions { get => contextualMenuOptions; set => contextualMenuOptions = value; }
        public EContextualMenuType ContextualMenuType { get => contextualMenuType; set => contextualMenuType = value; }

        #endregion

        private void Start()
        {
            OnSelectedActionVisualScripting += OnSelectedEnterEventUnit.TriggerEventCallback;
        }

    }
}
