using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    [Serializable]
    public class CMWorld
    {
        [SerializeField] private int id;
        [SerializeField] private string label;
        [SerializeField] private string description;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private bool enabled;
        [SerializeField] private bool multiplayer;
        [SerializeField] private int maxCCU;
        [SerializeField] private CMWorldConfig config;

        public int Id { get => id; set => id = value; }
        public string Label { get => label; set => label = value; }
        public string Description { get => description; set => description = value; }
        public string ThumbnailUri { get => thumbnailUri; set => thumbnailUri = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public CMWorldConfig Config { get => config; set => config = value; }
        public int MaxCCU { get => maxCCU; set => maxCCU = value; }
    }
}
