using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpeakerPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Speaker audio references")]
        [SerializeField] private AudioClip audioToInstantiate;
        [SerializeField] private float audioListenRange;
        [SerializeField] private bool isSpatialized;
        [SerializeField] private bool isLooping;
        [SerializeField] private BaseSpeakerController.SpeakerState state = BaseSpeakerController.SpeakerState.Stopped;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private GameObject fatherConnecter;


        public Role OwnershipMask => ownershipMask;
        public AudioClip AudioToInstantiate => audioToInstantiate;
        public float AudioListenRange => audioListenRange;
        public bool IsSpatialized => isSpatialized;
        public bool IsLooping => isLooping;
        public BaseSpeakerController.SpeakerState State => state;
        public List<GameObject> Connectables => connectables;
        public GameObject FatherConnecter => fatherConnecter;
    }
}
