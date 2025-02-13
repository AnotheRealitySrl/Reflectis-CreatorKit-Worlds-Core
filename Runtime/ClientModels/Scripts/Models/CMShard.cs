using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMShard
    {
        public static int maxShardCapacity;

        [SerializeField] private int shardNumber;
        [SerializeField] private int eventId;
        [SerializeField] private int currentParticipants;
        [SerializeField] private int maxParticipants;
        [SerializeField] private bool isClosed;

        public int ShardNumber => shardNumber;
        public int EventId => eventId;
        public int CurrentParticipants { get => currentParticipants; set => currentParticipants = value; }
        public int MaxParticipants
        {
            get
            {
                return Math.Clamp(maxParticipants, 0, maxShardCapacity);
            }
            set => maxParticipants = value;
        }
        public bool IsClosed => isClosed;

        public CMShard(int shardNumber, int eventId, int currentParticipants, int maxParticipants, bool isClosed)
        {
            this.shardNumber = shardNumber;
            this.eventId = eventId;
            this.currentParticipants = currentParticipants;
            this.maxParticipants = maxParticipants;
        }
    }
}
