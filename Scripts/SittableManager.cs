using Lightbug.CharacterControllerPro.Implementation;
using Reflectis.SDK.CharacterController;
using Reflectis.SDK.CharacterControllerPro;
using Reflectis.SDK.Core;
using Reflectis.SDK.DesktopInteraction;
using Reflectis.SDK.UIKit.ToastSystem;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Virtuademy.Placeholders;
using static Reflectis.SDK.CharacterControllerPro.ReflectisNormalMovement;

public class SittableManager : MonoBehaviour, IRuntimeComponent, ISeatable
{
    [SerializeField] private Transform sitTransform;
    [SerializeField] private Transform stepUpTransform;

    [SerializeField] private Collider interactableArea;

    CharacterControllerProSystem characterControllerSystem;
    Toggle toggle;
    MultiCustomInputHandler inputHandler;

    public bool isInteractable { get; set; }

    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        SittablePlaceholder sittablePlaceholder = placeholder as SittablePlaceholder;
        sitTransform = sittablePlaceholder.SitTransform;
        stepUpTransform = sittablePlaceholder.StepUpTransform;

        interactableArea = sittablePlaceholder.InteractableArea;
        isInteractable = true;
        toggle = null;
        inputHandler = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<CharacterStateController>().CurrentState is ReflectisSittingState sitting) return;

        characterControllerSystem = SM.GetSystem<CharacterControllerProSystem>();
        if (other.GetComponentInChildren<CharacterControllerBase>() is CharacterControllerBase characterController &&
            characterControllerSystem.CharacterControllerInstance == characterController)
        {
            CheckSitAction(characterController);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInChildren<CharacterStateController>().CurrentState is ReflectisSittingState sitting) return;

        if (other.GetComponentInChildren<CharacterControllerBase>() is CharacterControllerBase characterController &&
            characterControllerSystem.CharacterControllerInstance == characterController)
        {
            characterControllerSystem = null;

            characterController.IsInRangeToInteract = false;
        }        
    }

    /// <summary>
    /// Function to check once you arrived on range to sit if you can interact with it.
    /// </summary>
    private void CheckSitAction(CharacterControllerBase characterController)
    {
        if (!isInteractable) return;

        if (characterController.GetComponentInChildren<CharacterStateController>() is CharacterStateController characterState && characterState.CurrentState is ReflectisNormalMovement normalState && normalState.CurrentMovingState == NormalState.Idlemoving)
        {
            characterController.IsInRangeToInteract = true;

            StartCoroutine(ToastActionEnable(characterController));
        }
    }

    /// <summary>
    /// Function to trigger the sit action in case is possible to sit.
    /// </summary>
    public void SitAction(CharacterControllerBase characterController, ref bool isToggled)
    {
        if (isToggled) return;

        if (!isInteractable) return;

        isToggled = true;
        isInteractable = false;

        StartCoroutine(ToastStepUpPanel(characterController));
        characterController.IsInRangeToInteract = false;

        characterControllerSystem.MoveCharacter(new Pose(new Vector3(sitTransform.position.x, characterController.transform.position.y, sitTransform.position.z), sitTransform.rotation));

        GetComponentInChildren<ToastActivatorController>().ToastDeactivator();
    }

    private IEnumerator ToastStepUpPanel(CharacterControllerBase characterController)
    {
        yield return new WaitForSeconds(1);

        StartCoroutine(DestroyToast(false));
        GetComponentInChildren<ToastActivatorController>().ToastActivator();
        StartCoroutine(ToastActionEnable(characterController));
    }

    /// <summary>
    /// Function to trigger the step up action.
    /// </summary>
    public void StepUpAction(ref bool isToggled)
    {
        if (isToggled) return;
        if (!characterControllerSystem) return;

        isToggled = true;
        isInteractable = true;

        characterControllerSystem.MoveCharacter(new Pose(stepUpTransform.position, stepUpTransform.rotation));

        characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract = true;

        StartCoroutine(DestroyToast(true));
    }

    private IEnumerator ToastActionEnable(CharacterControllerBase characterController)
    {
        if (characterControllerSystem == null) yield return null;

        inputHandler = FindObjectOfType<MultiCustomInputHandler>();
        foreach (var sittingInput in inputHandler.GetComponentsInChildren<MultiInputsController>())
        {
            if (!sittingInput.EnableSittingInteraction) continue;

            if (characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract)
            {
                if (!toggle)
                {
                    yield return new WaitForSeconds(.2f);

                    toggle = SM.GetSystem<ToastSystem>().CurrentToast.GetComponentInChildren<Toggle>();
                }
                var isToggledCheck = false;
                toggle.onValueChanged.AddListener((x) => sittingInput.CheckCanSit(ref isToggledCheck));
                var isToggledSit = false;
                toggle.onValueChanged.AddListener((x) => SitAction(characterControllerSystem.CharacterControllerInstance,ref isToggledSit));

            }
            else if (!characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract)
            {
                yield return new WaitForSeconds(.2f);

                toggle = SM.GetSystem<ToastSystem>().CurrentToast.GetComponentInChildren<Toggle>();

                var isToggledStep = false;
                toggle?.onValueChanged.AddListener((x) => StepUpAction(ref isToggledStep));
                var isToggledSit = false;
                toggle?.onValueChanged.AddListener((x) => sittingInput.CheckCanSit(ref isToggledSit));
                var isToggledBehavior = false;
                toggle?.onValueChanged.AddListener((x) => characterController.GetComponentInChildren<ReflectisSittingState>().TriggerBehaviourExit(ref isToggledBehavior));
                toggle?.onValueChanged.AddListener((x) => StartCoroutine(sittingInput.ResetKeyboardValue()));
            }
            break;
        }
    }

    private IEnumerator DestroyToast(bool hasToWait)
    {
        var go = SM.GetSystem<ToastSystem>().CurrentToast.gameObject;

        if (hasToWait)
        {
            yield return new WaitForSeconds(0.5f);

            SM.GetSystem<IDesktopInteractionSystem>()?.Deinit();
            SM.GetSystem<ToastSystem>().WipeToastCache();
            
            Destroy(go);
            toggle = null;


        }
        else
        {
            SM.GetSystem<IDesktopInteractionSystem>()?.Deinit();
            SM.GetSystem<ToastSystem>().WipeToastCache();
            
            Destroy(SM.GetSystem<ToastSystem>().CurrentToast.gameObject);
            toggle = null;
        }
    }
}
