using Reflectis.SDK.CreatorKit;
using Reflectis.SDK.InteractionNew;
using Reflectis.SDK.Utilities.Extensions;

using System.Reflection;

using UnityEditor.SceneManagement;

using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    [CreateAssetMenu(menuName = "Reflectis/SDK-CreatorKit/InteractablePlaceholderUpgradeUtility", fileName = "InteractablePlaceholderUpgradeUtility")]
    public class InteractablePlaceholderUpgradeUtility : PlaceholderUpgradeUtility
    {
        public override void Upgrade()
        {
            InteractablePlaceholder[] interactablePlaceholders = FindObjectsOfType<InteractablePlaceholder>(false);

            foreach (var interactablePlaceholder in interactablePlaceholders)
            {
                InteractableColliderContainer interactableColliderContainer = interactablePlaceholder.gameObject.GetOrAddComponent<InteractableColliderContainer>();

                if (interactablePlaceholder.InteractionModes.HasFlag(IInteractable.EInteractableType.Manipulable))
                {
                    ManipulablePlaceholder manipulablePlaceholer = interactablePlaceholder.gameObject.GetOrAddComponent<ManipulablePlaceholder>();

                    manipulablePlaceholer.IsNetworked = interactablePlaceholder.IsNetworked;

                    manipulablePlaceholer.NonProportionalScale = interactablePlaceholder.NonProportionalScale;
                    manipulablePlaceholer.VRInteraction = interactablePlaceholder.VRInteraction;
                    manipulablePlaceholer.GetType().
                            GetField("dynamicAttach",
                            BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                            SetValue(manipulablePlaceholer, interactablePlaceholder.DynamicAttach);
                    manipulablePlaceholer.GetType().
                           GetField("adjustRotationOnRelease",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.AdjustRotationOnRelease);
                    manipulablePlaceholer.GetType().
                           GetField("realignAxisX",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.RealignAxisX);
                    manipulablePlaceholer.GetType().
                           GetField("realignAxisY",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.RealignAxisY);
                    manipulablePlaceholer.GetType().
                           GetField("realignAxisZ",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.RealignAxisZ);
                    manipulablePlaceholer.GetType().
                           GetField("realignDurationTimeInSeconds",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.RealignDurationTimeInSeconds);
                    manipulablePlaceholer.GetType().
                           GetField("mouseLookAtCamera",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.MouseLookAtCamera);
                    manipulablePlaceholer.GetType().
                           GetField("attachTransform",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(manipulablePlaceholer, interactablePlaceholder.AttachTransform);
                }

                if (interactablePlaceholder.InteractionModes.HasFlag(IInteractable.EInteractableType.GenericInteractable))
                {
                    GenericInteractablePlaceholder genericInteractablePlaceholder = interactablePlaceholder.gameObject.GetOrAddComponent<GenericInteractablePlaceholder>();

                    genericInteractablePlaceholder.GetType().
                           GetField("lockHoverDuringInteraction",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(genericInteractablePlaceholder, interactablePlaceholder.LockHoverDuringInteraction);
                    genericInteractablePlaceholder.GetType().
                           GetField("interactionsScriptMachine",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(genericInteractablePlaceholder, interactablePlaceholder.InteractionsScriptMachine);
                    genericInteractablePlaceholder.GetType().
                           GetField("needUnselectOnDestroyScriptMachine",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(genericInteractablePlaceholder, interactablePlaceholder.NeedUnselectOnDestroyScriptMachine);
                    genericInteractablePlaceholder.GetType().
                           GetField("unselectOnDestroyScriptMachine",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(genericInteractablePlaceholder, interactablePlaceholder.UnselectOnDestroyScriptMachine);
                    genericInteractablePlaceholder.GetType().
                           GetField("desktopAllowedStates",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(genericInteractablePlaceholder, interactablePlaceholder.DesktopAllowedStates);
                    genericInteractablePlaceholder.GetType().
                           GetField("vrAllowedStates",
                           BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).
                           SetValue(genericInteractablePlaceholder, interactablePlaceholder.VRAllowedStates);
                }

                if (interactablePlaceholder.InteractionModes.HasFlag(IInteractable.EInteractableType.ContextualMenuInteractable))
                {
                    ContextualMenuManageablePlaceholder contextualMenuManageablePlaceholder = interactablePlaceholder.gameObject.GetOrAddComponent<ContextualMenuManageablePlaceholder>();

                    contextualMenuManageablePlaceholder.IsNetworked = interactablePlaceholder.IsNetworked;

                    contextualMenuManageablePlaceholder.ContextualMenuOptions = interactablePlaceholder.ContextualMenuOptions;
                }

                DestroyImmediate(interactablePlaceholder);

                EditorSceneManager.SaveOpenScenes();
            }
        }
    }
}
