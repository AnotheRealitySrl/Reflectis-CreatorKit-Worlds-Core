using Lightbug.CharacterControllerPro.Implementation;
using Reflectis.SDK.CharacterController;
using Reflectis.SDK.CharacterControllerPro;
using Reflectis.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Virtuademy.Placeholders;

public class SpeakerController : MonoBehaviour, IRuntimeComponent, ISpeaker, IConnectable
{
    [SerializeField] private AudioClip audioToInstantiate;
    [SerializeField] private float audioListenRange;
    [SerializeField] private bool isSpatialized;
    [SerializeField] private bool isLooping;
    [SerializeField] private bool isOn;
    [SerializeField] private List<IConnectable> connectables;

    private InteractableMouseCast interactableMouse;
    private AudioSource audio;

    #region Interface Implementation
    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        SpeakerPlaceholder speakerPlaceholder = placeholder as SpeakerPlaceholder;
        audioToInstantiate = speakerPlaceholder.AudioToInstantiate;
        audioListenRange = speakerPlaceholder.AudioListenRange;
        isSpatialized = speakerPlaceholder.IsSpatialized;
        isLooping = speakerPlaceholder.IsLooping;
        isOn = speakerPlaceholder.IsOn;
        connectables = speakerPlaceholder.Connectables;

        interactableMouse = FindObjectOfType<InteractableMouseCast>();
        audio.GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (!isOn)
        {
            isOn = true;
            
            if(connectables != null)
            {
                foreach (var connectable in connectables)
                {
                    if (connectable != null)
                    {
                        connectable.TriggerAction();
                    }
                }
            }
            else
            {
                TriggerAction();
            }
        }
        else 
        {
            isOn = false;

            if (connectables != null)
            {
                foreach (var connectable in connectables)
                {
                    if (connectable != null)
                    {
                        connectable.TriggerResetAction();
                    }
                }
            }
            else
            {
                TriggerResetAction();
            }
        }
    }

    public void TriggerAction()
    {
        audio.clip = audioToInstantiate;
        audio.spatialize = isSpatialized;
        audio.loop = isLooping;
        audio.maxDistance = audioListenRange;

        audio.Play();
    }

    public void TriggerResetAction()
    {
        audio.Stop();
        audio.clip = null;
    }
    #endregion

    private void Update()
    {
        CheckRayInteraction();
    }

    /// <summary>
    /// Function to check the ray interaction.
    /// </summary>
    private void CheckRayInteraction()
    {
        if (interactableMouse.RaySpeakerHit() == this)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Interact();
            }
        }
    }
}
    
