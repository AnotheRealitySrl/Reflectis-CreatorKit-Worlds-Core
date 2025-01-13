using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CMWorldConfig
    {
        [SerializeField] private string videoChatAppId;
        [SerializeField] private string gptAppId;
        [SerializeField] private int maxShardCapacity = 20;

        public string VideoChatAppId { get => videoChatAppId; set => videoChatAppId = value; }
        public string GptAppId { get => gptAppId; set => gptAppId = value; }
        public int MaxShardCapacity { get => maxShardCapacity; set => maxShardCapacity = value; }

    }
}
