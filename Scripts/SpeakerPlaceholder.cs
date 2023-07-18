using System.Collections.Generic;
using UnityEngine;
using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class SpeakerPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Speaker audio references")]
        [SerializeField] private AudioClip audioToInstantiate;
        [SerializeField] private float audioListenRange;
        [SerializeField] private bool isSpatialized;
        [SerializeField] private bool isLooping;
        [SerializeField] private bool isOn = false;
        [SerializeField] private List<GameObject> connectables; 


        public Role OwnershipMask => ownershipMask;
        public AudioClip AudioToInstantiate => audioToInstantiate;
        public float AudioListenRange => audioListenRange;
        public bool IsSpatialized => isSpatialized;
        public bool IsLooping => isLooping;
        public bool IsOn => isOn;
        public List<GameObject> Connectables  => connectables;
    }
}
