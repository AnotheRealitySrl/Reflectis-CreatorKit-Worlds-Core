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

        [HelpBox("Pleaase note that \"Scale\" option of Manipulation Mode and \"Non Proportional Scale\" are currently not supported.", HelpBoxMessageType.Warning)]

        [SerializeField, Tooltip("Translate, rotate and scale.")]
        private EManipulationMode manipulationMode = (EManipulationMode)~0;

        [SerializeField, Tooltip("Enables non-proportional scale on this object.")]
        private bool nonProportionalScale;


        [Header("VR manipulation settings")]

        [SerializeField, Tooltip("Enables hand and ray interaction on this object")]
        private EVRInteraction vrInteraction = (EVRInteraction)~0;

        [SerializeField, Tooltip("A dynamic attach means that the object won't snap to the center of gravity")]
        private bool dynamicAttach;

        [SerializeField, Tooltip("Resets the rotation on one or more axes when the interaction ends (VR-only)")]
        private bool adjustRotationOnRelease;

        [SerializeField, Tooltip("Resets the rotation on the X axis")]
        private bool realignAxisX = true;

        [SerializeField, Tooltip("Resets the rotation on the Y axis")]
        private bool realignAxisY = false;

        [SerializeField, Tooltip("Resets the rotation on the Z axis")]
        private bool realignAxisZ = true;

        [SerializeField, Tooltip("How much time is needed to reset the rotation")]
        private float realignDurationTimeInSeconds = 1f;


        [Header("WebGL manipulation settings")]

        [SerializeField, Tooltip("If selected, the object will face the camera on mouse interaction")]
        private bool mouseLookAtCamera;

        [SerializeField, Tooltip("If specified, a dynamic attach will be generated on interaction")]
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

        [SerializeField, Tooltip("Reference to the script machine that describes what happens during interaction events." +
            "Utilize \"GenericInteractableHoverEnter\",\"GenericInteractableHoverExit\",\"GenericInteractableSelectEnter\",\"GenericInteractableSelectExit\"" +
            " and \"GenericInteractableInteract\" nodes to custumize your interactions")]
        private ScriptMachine interactionsScriptMachine;

        [SerializeField, Tooltip("Check if you need special operations to do after the object is destroyed while selected.")]
        private bool needUnselectOnDestroyScriptMachine;

        [SerializeField, Tooltip("Reference to the script machine that describes what happens if the object is destroyed while selected." +
            "The script machine has to be assigned to a different empty gameobject! " +
            "Utilize \"GenericInteractableUnselectOnDestroy\" node to custumize the interaction")]
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

        #endregion

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
