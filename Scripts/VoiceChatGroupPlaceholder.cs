using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class VoiceChatGroupPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string voiceRoomName = "Global";
        [SerializeField] private bool isMainChannel = false;

        public string VoiceRoomName => voiceRoomName;
        public bool IsMainChannel => isMainChannel;
    }
}
