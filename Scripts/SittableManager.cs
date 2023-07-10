using Lightbug.CharacterControllerPro.Implementation;
using Reflectis.SDK.CharacterController;
using Reflectis.SDK.CharacterControllerPro;
using Reflectis.SDK.Core;
using Reflectis.SDK.UIKit.ToastSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Virtuademy.Placeholders;
using static Reflectis.SDK.CharacterControllerPro.ReflectisNormalMovement;

public class SittableManager : MonoBehaviour, IRuntimeComponent, ISeatable
{
    [SerializeField] private Transform sitTransform;
    [SerializeField] private Transform stepUpTransform;

    [SerializeField] private Collider interactableArea;

    CharacterControllerProSystem characterControllerSystem;

    public bool isInteractable { get; set; }

    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        SittablePlaceholder sittablePlaceholder = placeholder as SittablePlaceholder;
        sitTransform = sittablePlaceholder.SitTransform;
        stepUpTransform = sittablePlaceholder.StepUpTransform;

        interactableArea = sittablePlaceholder.InteractableArea;
        isInteractable = true;
    }

    private void Update()
    {
        if (characterControllerSystem == null) return;

        if (characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract && Input.GetKeyDown(KeyCode.E))
        {
            SitAction(characterControllerSystem.CharacterControllerInstance);
        }
        else if (!characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract && Input.GetKeyDown(KeyCode.E))
        {
            StepUpAction(characterControllerSystem.CharacterControllerInstance);
        }
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
        
        //other.GetComponent<CharacterControllerBase>().CanvasInteraction.transform.Find("PressButtonText").gameObject.SetActive(false);

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
            //var text = characterController.CanvasInteraction.transform.Find("PressButtonText");
            //text.gameObject.SetActive(true);
            //text.GetComponent<TextMeshProUGUI>().text = "Press 'E' to interact ";

            characterController.IsInRangeToInteract = true;
        }
    }

    /// <summary>
    /// Function to trigger the sit action in case is possible to sit.
    /// </summary>
    public void SitAction(CharacterControllerBase characterController)
    {
        if (!isInteractable) return;

        isInteractable = false;
        //var text = characterController.CanvasInteraction.transform.Find("PressButtonText");
        //text.GetComponent<TextMeshProUGUI>().text = "Press 'E' to step up ";
        StartCoroutine(ToastStepUpPanel());
        characterController.IsInRangeToInteract = false;

        characterControllerSystem.MoveCharacter(new Pose(new Vector3(sitTransform.position.x, characterController.transform.position.y, sitTransform.position.z), sitTransform.rotation));
    }

    private IEnumerator ToastStepUpPanel()
    {
        yield return new WaitForSeconds(1);

        GetComponentInChildren<ToastActivatorController>().ToastActivator();
    }

    /// <summary>
    /// Function to trigger the step up action.
    /// </summary>
    public void StepUpAction(CharacterControllerBase characterController)
    {
        isInteractable = true;

        characterControllerSystem.MoveCharacter(new Pose(stepUpTransform.position, stepUpTransform.rotation));

        characterController.IsInRangeToInteract = true;

        GetComponentInChildren<ToastActivatorController>().ToastDeactivator();

        //var text = characterController.CanvasInteraction.transform.Find("PressButtonText");
        //text.GetComponent<TextMeshProUGUI>().text = "Press 'E' to interact ";
    }

    private void ToastAction()
    {
        //SM.GetSystem<ToastSystem>().CurrentToast.GetComponentInChildren<Toggle>().onValueChanged 
        if (characterControllerSystem == null) return;

        if (characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract)
        {
            SitAction(characterControllerSystem.CharacterControllerInstance);
        }
        else if (!characterControllerSystem.CharacterControllerInstance.IsInRangeToInteract)
        {
            StepUpAction(characterControllerSystem.CharacterControllerInstance);
        }
    }
}
