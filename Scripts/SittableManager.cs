using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using Reflectis.SDK.CharacterController;
using Reflectis.SDK.CharacterControllerPro;
using Reflectis.SDK.Core;
using Reflectis.SDK.Fade;
using Reflectis.SDK.UIKit.ToastSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Virtuademy.Placeholders;
using static Lightbug.CharacterControllerPro.Demo.NormalMovement;

public class SittableManager : MonoBehaviour, IRuntimeComponent,ISeatable
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
        else if (characterControllerSystem.CharacterControllerInstance.GetComponentInChildren<CharacterStateController>().CurrentState is Sitting sitting && sitting.SittingState == Sitting.SittingAnimationStates.Idling && Input.GetKeyDown(KeyCode.E))
        {
            StepUpAction(characterControllerSystem.CharacterControllerInstance);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Al posto dell on trigger probabilmente un raycast o spherecast per controllare se stai colpendo la sedia
        //Se stai colpendo con lo spherecast triggerare un canvas ui dove devi premere un tasto per poterti sedere
        //UI che deve apparire solo se non c'� gia un altra persona seduta (Rimandare un check dopo aver premuto il tasto di ricontrollare se non c'�
        //qualcuno che ha triggerato l'animazione di seduta eludendo il gioco)
        //Delegare al tasto di seduta la funzione SitAction per potersi sedere (e gestire la variabile del check di seduta non solo sull'init ma ogni qualc volta uno hitta con il raycast e quando si preme l'azione di sit).

        characterControllerSystem = SM.GetSystem<CharacterControllerProSystem>();
        if (other.GetComponentInChildren<CharacterControllerBase>() is CharacterControllerBase characterController &&
            characterControllerSystem.CharacterControllerInstance == characterController)
        {

            CheckSitAction(characterController);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<CharacterControllerBase>().CanvasInteraction.transform.Find("PressButtonText").gameObject.SetActive(false);

        if(other.GetComponentInChildren<CharacterControllerBase>() is CharacterControllerBase characterController &&
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

        if(characterController.GetComponentInChildren<CharacterStateController>() is CharacterStateController characterState && characterState.CurrentState is NormalMovement normalState && normalState.CurrentMovingState == NormalState.Idlemoving)
        {
            var text = characterController.CanvasInteraction.transform.Find("PressButtonText");
            text.gameObject.SetActive(true);
            text.GetComponent<TextMeshProUGUI>().text = "Press 'E' to interact ";

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
        var text = characterController.CanvasInteraction.transform.Find("PressButtonText");
        text.GetComponent<TextMeshProUGUI>().text = "Press 'E' to step up ";

        characterController.IsInRangeToInteract = false;

        characterController.transform.position = new Vector3(sitTransform.position.x, characterController.transform.position.y, sitTransform.position.z);
        characterController.transform.rotation = sitTransform.rotation;
    }

    /// <summary>
    /// Function to trigger the step up action.
    /// </summary>
    public void StepUpAction(CharacterControllerBase characterController)
    {
        isInteractable = true;

        characterController.transform.position = stepUpTransform.position;
        characterController.transform.rotation = stepUpTransform.rotation;

        characterController.IsInRangeToInteract = true;
        var text = characterController.CanvasInteraction.transform.Find("PressButtonText");
        text.GetComponent<TextMeshProUGUI>().text = "Press 'E' to interact ";
    }
}
