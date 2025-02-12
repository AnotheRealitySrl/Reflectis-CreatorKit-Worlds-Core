using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

using static Reflectis.CreatorKit.Worlds.Core.Interaction.IInteractable;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public class BaseInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool doAutomaticSetup = false;
        [SerializeField] private List<InteractableBehaviourBase> interactableBehaviours = new();
        [SerializeField] private List<Collider> interactionColliders = new();

        private bool isHovered;

        private EInteractionState interactionState = EInteractionState.Idle;

        public EInteractionState InteractionState
        {
            get => interactionState;
            set
            {
                if (value == EInteractionState.Idle)
                {
                    value = isHovered ? EInteractionState.Hovered : EInteractionState.Idle;
                }
                var oldValue = interactionState;
                interactionState = value;
                if (value == EInteractionState.Idle && oldValue != EInteractionState.Idle)
                {
                    PropagateHoverExit();
                }
                if (value == EInteractionState.Hovered && oldValue != EInteractionState.Hovered)
                {
                    PropagateHoverEnter();
                }

            }
        }
        public List<IInteractableBehaviour> InteractableBehaviours { get; private set; } = new();

        public GameObject GameObjectRef => gameObject;
        public List<Collider> InteractionColliders { get => interactionColliders; set => interactionColliders = value; }

        public UnityEvent OnInteractableSetupComplete { get; } = new();

        [HideInInspector]
        public bool SetupCompleted { get; private set; } = false;

        private void Awake()
        {
            if (doAutomaticSetup)
            {
                Setup();
            }
        }

        public async Task Setup()
        {
            if (interactableBehaviours.Count == 0)
            {
                interactableBehaviours.AddRange(GetComponentsInChildren<InteractableBehaviourBase>());
            }

            if (interactionColliders == null || interactionColliders.Count == 0)
            {
                interactionColliders = GetComponentsInChildren<Collider>().ToList();
            }
            foreach (var behaviour in interactableBehaviours)
            {
                if (!InteractableBehaviours.Contains(behaviour))
                {
                    InteractableBehaviours.Add(behaviour);
                }
            }

            foreach (var interactable in InteractableBehaviours)
            {
                await interactable.Setup();
            }
            SetupCompleted = true;
        }

        public void OnHoverEnter()
        {
            isHovered = true;
            InteractionState = EInteractionState.Hovered;
        }

        public void OnHoverExit()
        {
            isHovered = false;

            if (InteractableBehaviours.TrueForAll(x => (!x.LockHoverDuringInteraction && x is IVisualScriptingInteractable) || x.IsIdleState))
            {
                InteractionState = EInteractionState.Idle;
            }
        }

        private void PropagateHoverEnter()
        {
            InteractableBehaviours.ForEach(x => x.OnHoverStateEntered());
        }

        private void PropagateHoverExit()
        {
            InteractableBehaviours
                .ForEach(x =>
                {
                    if (x != null && ((!x.LockHoverDuringInteraction && x is IVisualScriptingInteractable) || x.IsIdleState))
                    {
                        x.OnHoverStateExited();
                    }
                }
            );
        }

        public static async Task SetupSubmesh(MeshRenderer mesh, bool needColliders)
        {
            var submesh = mesh.gameObject.AddComponent<InteractableSubmesh>();
            submesh.MeshRenderer = mesh;
            if (needColliders)
            {
                await submesh.SetupSubmeshColliders();
            }
        }

        //public List<InteractableSubmesh> SetupSubmesh()
        //{
        //    List<InteractableSubmesh> submeshes = new List<InteractableSubmesh>();

        //    MeshRenderer[] rendererList = GetComponentsInChildren<MeshRenderer>(true);

        //    List<GameObject> objectsToExclude = new List<GameObject>();
        //    // Excludes bounding box and scaling gizmos
        //    Manipulable modelManipulable = this.GetComponentInactive<Manipulable>();
        //    if (modelManipulable != null)
        //    {
        //        objectsToExclude.AddRange(modelManipulable.BoundingBox.gameObject.transform.GetComponentsInChildren<Transform>(true).Select(x => x.gameObject));

        //        foreach (GameObject gObject in modelManipulable.ScalingCorners)
        //        {
        //            objectsToExclude.Add(gObject);
        //        }
        //        foreach (GameObject gObject in modelManipulable.ScalingFaces)
        //        {
        //            objectsToExclude.Add(gObject);
        //        }
        //    }

        //    // Excludes placeholder object (which is used during asset download)
        //    GameObject placeholder = GetComponentsInChildren<GenericHookComponent>().FirstOrDefault(x => x.Id == "Placeholder")?.gameObject;
        //    if (placeholder != null)
        //    {
        //        objectsToExclude.Add(placeholder);
        //    }

        //    // Cycle to find objects that should be excluded
        //    foreach (var objectToExclude in objectsToExclude)
        //    {
        //        rendererList = rendererList.Where(x => x.gameObject != objectToExclude).ToArray();
        //    }
        //    foreach (MeshRenderer renderer in rendererList)
        //    {
        //        //check if there is already an interactable submesh
        //        if (!renderer.TryGetComponent<InteractableSubmesh>(out var interactableSubmesh))
        //        {
        //            interactableSubmesh = renderer.AddComponent<InteractableSubmesh>();
        //            interactableSubmesh.Renderer = renderer;
        //        }
        //        submeshes.Add(interactableSubmesh);
        //    }
        //    return submeshes;
        //}


    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BaseInteractable))]
    public class BaseInteractableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseInteractable interactable = (BaseInteractable)target;

            GUIStyle style = new(EditorStyles.label)
            {
                richText = true
            };

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField($"<b>Current state:</b> {interactable.InteractionState}", style);
            }
        }
    }

#endif

}
