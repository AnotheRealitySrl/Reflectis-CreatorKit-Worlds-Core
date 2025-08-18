using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMLeaderboardRecord
    {
        [SerializeField] private string leaderboardKey;
        [SerializeField] private float data;

        public string LeaderboardKey { get => leaderboardKey; set => leaderboardKey = value; }
        public float Data { get => data; set => data = value; }
    }
}
