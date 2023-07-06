using Reflectis.SDK.CharacterController;
using Reflectis.SDK.CharacterControllerPro;
using Reflectis.SDK.Core;
using Reflectis.SDK.Fade;
using Reflectis.SDK.UIKit.ToastSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Placeholders;

public class SittableManager : MonoBehaviour, IRuntimeComponent
{
    [SerializeField] private Transform sitTransform;
    [SerializeField] private Transform stepUpTransform;

    [SerializeField] private Collider interactableArea;
    [SerializeField] private bool isInteractable;
    [SerializeField] private bool isInRangeToInteract = false;

    [SerializeField] Canvas canvasButton;


    CharacterControllerProSystem characterControllerSystem;

    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        SittablePlaceholder sittablePlaceholder = placeholder as SittablePlaceholder;
        sitTransform = sittablePlaceholder.SitPosition;
        stepUpTransform = sittablePlaceholder.StepUpPosition;

        interactableArea = sittablePlaceholder.InteractableArea;
        isInteractable = sittablePlaceholder.IsInteractable;

        canvasButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        //TODO change the update and input with the new input system delegate action.
        if (isInRangeToInteract && Input.GetKeyDown(KeyCode.E))
        {
            SitAction(characterControllerSystem.CharacterControllerInstance);
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
        isInRangeToInteract = false;
        canvasButton.gameObject.SetActive(false);

        if(other.GetComponentInChildren<CharacterControllerBase>() is CharacterControllerBase characterController &&
            characterControllerSystem.CharacterControllerInstance == characterController)
        {
            characterControllerSystem = null;
        }
    }

    /// <summary>
    /// Function to check once you arrived on range to sit if you can interact with it.
    /// </summary>
    private void CheckSitAction(CharacterControllerBase characterController)
    {
        if (!isInteractable) return;
        //if(characterController) //Checkare lo stato del character per controllare se è in un possibile stato per potersi sedere. 

        canvasButton.gameObject.SetActive(true);
        isInRangeToInteract = true;
    }

    /// <summary>
    /// Function to trigger the sit action in case is possible to sit.
    /// </summary>
    private void SitAction(CharacterControllerBase characterController)
    {
        if (!isInteractable) return;

        //Cambiare lo stato del character che si sta sedendo.
        isInteractable = false;
        canvasButton.gameObject.SetActive(false);
        isInRangeToInteract = false;

        characterController.transform.position = sitTransform.position;
        characterController.transform.rotation = sitTransform.rotation;

        //TODO triggerare l'animazione di seduta 
    }

    /// <summary>
    /// Function to trigger the step up action.
    /// </summary>
    private void StepUpAction(CharacterControllerBase characterController)
    {
        //Cambiare lo stato del character che si sta alzando.
        isInteractable = true;

        characterController.transform.position = stepUpTransform.position;
        characterController.transform.rotation = stepUpTransform.rotation;

        //TODO triggerare l'animazione di alzata
    }
}
