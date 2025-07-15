using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMShard
    {
        public static int maxShardCapacity;

        [SerializeField] private int shardNumber;
        [SerializeField] private int sessionId;
        [SerializeField] private int currentParticipants;
        [SerializeField] private int maxParticipants;
        [SerializeField] private bool isClosed;

        public int ShardNumber => shardNumber;
        public int SessionId => sessionId;
        public int CurrentParticipants { get => currentParticipants; set => currentParticipants = value; }
        public int MaxParticipants
        {
            get
            {
                return //Math.Clamp(maxParticipants, 0, maxShardCapacity); 
                    maxShardCapacity;
            }
            set => maxParticipants = value;
        }
        public bool IsClosed => isClosed;

        public CMShard(int shardNumber, int sessionId, int currentParticipants, int maxParticipants, bool isClosed)
        {
            this.shardNumber = shardNumber;
            this.sessionId = sessionId;
            this.currentParticipants = currentParticipants;
            this.maxParticipants = maxParticipants;
            this.isClosed = isClosed;
        }
    }
}
