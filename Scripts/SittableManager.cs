using Lightbug.CharacterControllerPro.Demo;
using Lightbug.CharacterControllerPro.Implementation;
using Reflectis.SDK.CharacterController;
using Reflectis.SDK.CharacterControllerPro;
using Reflectis.SDK.Core;
using Reflectis.SDK.Fade;
using Reflectis.SDK.UIKit.ToastSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Placeholders;
using static Lightbug.CharacterControllerPro.Demo.NormalMovement;

public class SittableManager : MonoBehaviour, IRuntimeComponent,ISeatable
{
    [SerializeField] private Transform sitTransform;
    [SerializeField] private Transform stepUpTransform;

    [SerializeField] private Collider interactableArea;

    [SerializeField] Canvas canvasButton;

    CharacterControllerProSystem characterControllerSystem;

    public bool isInteractable { get; set; }

    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        SittablePlaceholder sittablePlaceholder = placeholder as SittablePlaceholder;
        sitTransform = sittablePlaceholder.SitTransform;
        stepUpTransform = sittablePlaceholder.StepUpTransform;

        interactableArea = sittablePlaceholder.InteractableArea;
        isInteractable = true;

        canvasButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        //TODO change the update and input with the new input system delegate action.
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
        //UI che deve apparire solo se non c'è gia un altra persona seduta (Rimandare un check dopo aver premuto il tasto di ricontrollare se non c'è
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
        
        canvasButton.gameObject.SetActive(false);

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
            canvasButton.gameObject.SetActive(true);
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
        canvasButton.gameObject.SetActive(false);
        characterController.IsInRangeToInteract = false;

        characterController.transform.position = sitTransform.position;
        characterController.transform.rotation = sitTransform.rotation;
    }

    /// <summary>
    /// Function to trigger the step up action.
    /// </summary>
    public void StepUpAction(CharacterControllerBase characterController)
    {
        //Cambiare lo stato del character che si sta alzando.
        isInteractable = true;

        characterController.transform.position = stepUpTransform.position;
        characterController.transform.rotation = stepUpTransform.rotation;

        characterController.IsInRangeToInteract = false;
    }
}
