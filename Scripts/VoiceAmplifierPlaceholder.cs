using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.DTO;
using static BaseVoiceAmplifierController;

namespace Virtuademy.Placeholders
{
    public class VoiceAmplifierPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Animation references")]
        [SerializeField] private float volumeAmplified;
        [SerializeField] private float rangeAmplified;
        [SerializeField] private AmplifierStates  state = AmplifierStates.NotAmplified;
        [SerializeField] private List<GameObject> connectables;

        public Role OwnershipMask => ownershipMask;
        public float VolumeAmplified => volumeAmplified;
        public float RangeAmplified => rangeAmplified;
        public AmplifierStates State  => state;
        public List<GameObject> Connectables => connectables; 
    }
}
