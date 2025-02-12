using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CMWorldConfig
    {
        [SerializeField] private string videoChatAppId;
        [SerializeField] private string gptAppId;
        [SerializeField] private bool allowDoubleUserPresence;
        [SerializeField] private bool showFullNickname;
        [SerializeField] private int maxShardCapacity = 20;

        public string VideoChatAppId { get => videoChatAppId; set => videoChatAppId = value; }
        public string GptAppId { get => gptAppId; set => gptAppId = value; }
        public bool AllowDoubleUserPresence { get => allowDoubleUserPresence; set => allowDoubleUserPresence = value; }
        public bool ShowFullNickname { get => showFullNickname; set => showFullNickname = value; }
        public int MaxShardCapacity { get => maxShardCapacity; set => maxShardCapacity = value; }

    }
}
