using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    [Serializable]
    public class CMWorldCCU
    {
        [SerializeField] private int worldId;
        [SerializeField] private int onlineUsersCount;

        public int WorldId { get => worldId; set => worldId = value; }
        public int OnlineUsersCount { get => onlineUsersCount; set => onlineUsersCount = value; }
    }
}
