using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    public class CMWorldConfig
    {
        [SerializeField] private string videoChatAppId;
        [SerializeField] private int maxShardCapacity = 20;

        public string VideoChatAppId { get => videoChatAppId; set => videoChatAppId = value; }
        public int MaxShardCapacity { get => maxShardCapacity; set => maxShardCapacity = value; }

    }
}
