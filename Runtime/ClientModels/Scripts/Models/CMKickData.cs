using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CMKickData
    {
        [SerializeField] private int eventId;
        [SerializeField] private int shardNumber;

        public int EventId { get => eventId; set => eventId = value; }
        public int ShardNumber { get => shardNumber; set => shardNumber = value; }
    }
}
