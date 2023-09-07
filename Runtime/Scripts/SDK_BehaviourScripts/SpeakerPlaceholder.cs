using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class SpeakerPlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum SpeakerState
        {
            Playing = 1,
            Stopped = 2
        }

        [Header("Speaker audio references")]
        [SerializeField] private AudioClip audioToInstantiate;
        [SerializeField] private float audioListenRange;
        [SerializeField] private bool isLooping;
        [SerializeField] private SpeakerState state = SpeakerState.Stopped;
        [SerializeField] private bool isInteractable = true;

        public AudioClip AudioToInstantiate => audioToInstantiate;
        public float AudioListenRange => audioListenRange;
        public bool IsLooping => isLooping;
        public SpeakerState State => state;
        public bool IsInteractable => isInteractable;
    }
}
