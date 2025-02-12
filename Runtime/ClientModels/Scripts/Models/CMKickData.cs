using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CMKickData
    {
        [SerializeField] private int eventId;
        [SerializeField] private int shardId;
        [SerializeField] private string kickerId;

        public int EventId { get => eventId; set => eventId = value; }
        public int ShardId { get => shardId; set => shardId = value; }
        public string KickerId { get => kickerId; set => kickerId = value; }
    }
}
