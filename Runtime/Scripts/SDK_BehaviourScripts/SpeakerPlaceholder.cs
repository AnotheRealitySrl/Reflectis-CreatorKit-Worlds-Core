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

        private SpeakerState state = SpeakerState.Stopped;
        [SerializeField] private bool isInteractable = true;

        public SpeakerState State => state;
        public bool IsInteractable => isInteractable;
    }
}
