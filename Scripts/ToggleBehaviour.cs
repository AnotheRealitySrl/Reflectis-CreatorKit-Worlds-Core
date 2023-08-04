using Reflectis.SDK.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBehaviour : MonoBehaviour,IInteractable
{
    [SerializeField] List<GameObject> behaviourComponents;

    List<GameObject> behaviourComponentsReference;

    int currentBehaviourCycle;

    public List<GameObject> BehaviourComponents => behaviourComponents;

    public GameObject InteractionTarget => throw new NotImplementedException();


    private void Start()
    {
        behaviourComponentsReference = BehaviourComponents;

        foreach (var behaviour in behaviourComponentsReference)
        {
            try
            {
                behaviour.GetComponent<IInteractable>();
            }
            catch
            {
                BehaviourComponents.Remove(behaviour);
            }
        }

        behaviourComponentsReference = BehaviourComponents;
    }

    public void Interact(Action completedCallback = null)
    {
        behaviourComponents[currentBehaviourCycle].GetComponent<IInteractable>().Interact();

        try
        {
            behaviourComponents[currentBehaviourCycle - 1].GetComponent<IInteractable>().Interact();
        }
        catch (ArgumentOutOfRangeException e)
        {
            behaviourComponents[behaviourComponents.Count].GetComponent<IInteractable>().Interact();
        }
    }

    public void StopInteract(Action completedCallback = null)
    {

    }
}
