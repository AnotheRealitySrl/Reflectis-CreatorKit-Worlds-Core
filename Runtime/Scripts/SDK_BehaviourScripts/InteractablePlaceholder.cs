using Reflectis.SDK.InteractionNew;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System;
using System.Collections.Generic;

using UnityEngine;

using static Reflectis.SDK.InteractionNew.ContextualMenuManageable;
using static Reflectis.SDK.InteractionNew.GenericInteractable;
using static Reflectis.SDK.InteractionNew.Manipulable;

namespace Reflectis.SDK.CreatorKit
{
    public class InteractablePlaceholder : SceneComponentPlaceholderNetwork
    {
        [Flags]
        public enum EInteractableType
        {
            GenericInteractable = 1,
            Manipulable = 2,
            ContextualMenuInteractable = 4
        }

        [Header("Interaction settings")]
        [SerializeField] private List<Collider> interactionColliders = new();

        [SerializeField, Tooltip("Which kind of interaction should this object have?")]
        private EInteractableType interactionModes;

        public List<Collider> InteractionColliders => interactionColliders;
        public EInteractableType InteractionModes => interactionModes;

        #region Manipulation

        private bool showManipulationSettings => interactionModes.HasFlag(EInteractableType.Manipulable);
#if ODIN_INSPECTOR
        [ShowIfGroup(nameof(showManipulationSettings))]
#endif
#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings")]
#endif
        [SerializeField] private EManipulationMode manipulationMode = (EManipulationMode)~0;

        private bool hasScaleSelected => manipulationMode.HasFlag(EManipulationMode.Scale);
#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings"), ShowIf(nameof(hasScaleSelected))]
#endif
        [SerializeField] private bool nonProportionalScale;

        [Header("VR manipulation settings")]

#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings")]
#endif
        [SerializeField] private EVRInteraction vrInteraction = (EVRInteraction)~0;

#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings")]
#endif
        [SerializeField] private bool dynamicAttach;

#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings")]
#endif
        [SerializeField] private bool adjustRotationOnRelease;


#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings/Adjust rotation on release"), ShowIf(nameof(adjustRotationOnRelease))]
#endif
        [SerializeField] private bool realignAxisX = true;

#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings/Adjust rotation on release"), ShowIf(nameof(adjustRotationOnRelease))]
#endif
        [SerializeField] private bool realignAxisY = false;

#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings/Adjust rotation on release"), ShowIf(nameof(adjustRotationOnRelease))]
#endif
        [SerializeField] private bool realignAxisZ = true;

#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings/Adjust rotation on release"), ShowIf(nameof(adjustRotationOnRelease))]
#endif
        [SerializeField] private float realignDurationTimeInSeconds = 1f;


        private bool hasTranslationSelected => manipulationMode.HasFlag(EManipulationMode.Translate);
        [Header("WebGL manipulation settings")]
#if ODIN_INSPECTOR
        [BoxGroup("showManipulationSettings/Manipulation settings"), ShowIf(nameof(hasTranslationSelected))]
#endif
        [SerializeField] private bool mouseLookAtCamera;
        [SerializeField] private GameObject scalablePointCornerPrefab;
        [SerializeField] private GameObject scalablePointFacePrefab;

        public EManipulationMode ManipulationMode => manipulationMode;
        public EVRInteraction VRInteraction => vrInteraction;
        public bool DynamicAttach => dynamicAttach;
        public bool MouseLookAtCamera => mouseLookAtCamera;
        public bool NonProportionalScale => nonProportionalScale;
        public bool AdjustRotationOnRelease => adjustRotationOnRelease;
        public bool RealignAxisX => realignAxisX;
        public bool RealignAxisY => realignAxisY;
        public bool RealignAxisZ => realignAxisZ;
        public float RealignDurationTimeInSeconds => realignDurationTimeInSeconds; 
        public GameObject ScalablePointCornerPrefab  => scalablePointCornerPrefab;
        public GameObject ScalablePointFacePrefab => scalablePointFacePrefab;

        #endregion

        #region Generic interaction

        private bool showGenericInteractionSettings => interactionModes.HasFlag(EInteractableType.GenericInteractable);
#if ODIN_INSPECTOR
        [ShowIfGroup(nameof(showGenericInteractionSettings))]
#endif

        [Header("Generic interaction scriptable actions")]
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onHoverEnterActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onHoverExitActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onSelectingActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onSelectedActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onDeselectingActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onDeselectedActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onInteractActions = new();
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private List<AwaitableScriptableAction> onInteractFinishActions = new();

        [Header("Allowed states")]
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private EAllowedGenericInteractableState desktopAllowedStates = (EAllowedGenericInteractableState)~0;
#if ODIN_INSPECTOR
        [BoxGroup("showGenericInteractionSettings/Generic interaction settings")]
#endif
        [SerializeField] private EAllowedGenericInteractableState vrAllowedStates = (EAllowedGenericInteractableState)~0;


        public List<AwaitableScriptableAction> OnHoverEnterActions => onHoverEnterActions;
        public List<AwaitableScriptableAction> OnHoverExitActions => onHoverExitActions;
        public List<AwaitableScriptableAction> OnSelectingActions => onSelectingActions;
        public List<AwaitableScriptableAction> OnSelectedActions => onSelectedActions;
        public List<AwaitableScriptableAction> OnDeselectingActions => onDeselectingActions;
        public List<AwaitableScriptableAction> OnDeselectedActions => onDeselectedActions;
        public List<AwaitableScriptableAction> OnInteractActions => onInteractActions;
        public List<AwaitableScriptableAction> OnInteractFinishActions => onInteractFinishActions;

        public EAllowedGenericInteractableState DesktopAllowedStates => desktopAllowedStates;
        public EAllowedGenericInteractableState VRAllowedStates => vrAllowedStates;

        #endregion

        #region Contextual menu

        private bool showContextualMenuSettings => interactionModes.HasFlag(EInteractableType.ContextualMenuInteractable);
#if ODIN_INSPECTOR
        [ShowIfGroup(nameof(showContextualMenuSettings))]
#endif
#if ODIN_INSPECTOR
        [BoxGroup("showContextualMenuSettings/Contextual menu settings")]
#endif
        [SerializeField]
        private EContextualMenuOption contextualMenuOptions =
            EContextualMenuOption.LockTransform |
            EContextualMenuOption.ResetTransform |
            EContextualMenuOption.Duplicate |
            EContextualMenuOption.Delete;

        public EContextualMenuOption ContextualMenuOptions { get => contextualMenuOptions; set => contextualMenuOptions = value; }

        #endregion
    }
}
