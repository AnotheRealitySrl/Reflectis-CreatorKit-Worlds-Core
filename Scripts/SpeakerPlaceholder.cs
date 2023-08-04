using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpeakerPlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum SpeakerState
        {
            Playing = 1,
            Paused = 2,
            Stopped = 3
        }

        [Header("Speaker audio references")]
        [SerializeField] private AudioClip audioToInstantiate;
        [SerializeField] private float audioListenRange;
        [SerializeField] private bool isSpatialized;
        [SerializeField] private bool isLooping;
        [SerializeField] private SpeakerState state = SpeakerState.Stopped;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private GameObject fatherConnecter;
        [SerializeField] private bool isInteractable = true;

        public AudioClip AudioToInstantiate => audioToInstantiate;
        public float AudioListenRange => audioListenRange;
        public bool IsSpatialized => isSpatialized;
        public bool IsLooping => isLooping;
        public SpeakerState State => state;
        public List<GameObject> Connectables => connectables;
        public GameObject FatherConnecter => fatherConnecter;
        public bool IsInteractable => isInteractable;
    }
}
