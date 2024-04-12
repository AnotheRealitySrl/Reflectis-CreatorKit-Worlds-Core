using Reflectis.SDK.InteractionNew;
using Reflectis.SDK.Utilities;

using UnityEngine;
using static Reflectis.SDK.InteractionNew.Manipulable;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(InteractableColliderContainer))]
    public class ManipulablePlaceholder : SceneComponentPlaceholderNetwork
    {
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
    }
}
