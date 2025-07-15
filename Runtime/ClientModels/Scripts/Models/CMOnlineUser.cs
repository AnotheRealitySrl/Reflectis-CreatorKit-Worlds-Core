using System;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMOnlineUser
    {
        public enum Platform
        {
            None,
            WebGL,
            VR
        }

        [SerializeField] private string connectionId;
        [SerializeField] private Platform currentPlatform;
        [SerializeField] private int shard;
        [SerializeField] private int sessionId;
        [SerializeField] private int worldId;
        [SerializeField] private CMUser user;

        public string ConnectionId { get => connectionId; set => connectionId = value; }
        public Platform CurrentPlatform { get => currentPlatform; set => currentPlatform = value; }
        public int Shard { get => shard; set => shard = value; }
        public int SessionId { get => sessionId; set => sessionId = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public CMUser User { get => user; set => user = value; }
    }
}
