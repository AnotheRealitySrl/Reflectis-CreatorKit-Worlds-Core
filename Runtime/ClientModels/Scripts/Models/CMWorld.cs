using System;

using Unity.Properties;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CMWorld
    {
        [SerializeField] private int id;
        [SerializeField] private string label;
        [SerializeField] private string description;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private bool enabled;
        [SerializeField] private bool multiplayer;
        [SerializeField] private int multiplayerUsersCount;
        [SerializeField] private int? maxOnlineUsers;
        [SerializeField] private CMWorldConfig config;
        [SerializeField] private int defaultEventId;

        [CreateProperty] public int Id { get => id; set => id = value; }
        [CreateProperty] public string Label { get => label; set => label = value; }
        public string Description { get => description; set => description = value; }
        public string ThumbnailUri { get => thumbnailUri; set => thumbnailUri = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public CMWorldConfig Config { get => config; set => config = value; }

        /// <summary>
        /// If maxCCU is null, the world has no limit on the number of users that can be in it at the same time.
        /// </summary>
        public int? MaxOnlineUsers { get => maxOnlineUsers; set => maxOnlineUsers = value; }
        public int MultiplayerUsersCount { get => multiplayerUsersCount; set => multiplayerUsersCount = value; }
        public int DefaultEventId { get => defaultEventId; set => defaultEventId = value; }
    }
}
