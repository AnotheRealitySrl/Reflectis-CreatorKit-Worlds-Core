using Reflectis.SDK.Core.ApplicationManagement;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMEnvironment
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private Texture thumbnailTexture;
        [SerializeField] private string addressableKey;
        [SerializeField] private CMCatalog catalogInfo;
        [SerializeField] private int worldId;
        [SerializeField] private bool isTenant;
        [SerializeField] private string localizationUri;
        [SerializeField] private string localizationCSV;
        [SerializeField] private List<ESupportedPlatform> platforms;
        [SerializeField] private bool multiplayer;
        [SerializeField] private List<CMTag> tags;

        public int ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public string ThumbnailUri { get => thumbnailUri; set => thumbnailUri = value; }
        public Texture ImageTexture { get => thumbnailTexture; set => thumbnailTexture = value; }
        public string AddressableKey { get => addressableKey; set => addressableKey = value; }
        public CMCatalog Catalog { get => catalogInfo; set => catalogInfo = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public bool IsTenant { get => isTenant; set => isTenant = value; }
        public string LocalizationUri { get => localizationUri; set => localizationUri = value; }
        public string LocalizationCSV { get => localizationCSV; set => localizationCSV = value; }
        public List<ESupportedPlatform> Platforms { get => platforms; set => platforms = value; }

        public string PlatformsListString
        {
            get
            {
                string platforms = "";
                foreach (var eventDataPlatforms in Platforms)
                {
                    platforms += eventDataPlatforms.ToString() + " / ";
                }
                if (platforms.Length > 0)
                {
                    platforms = platforms.Substring(0, platforms.Length - " / ".Length);
                }
                return platforms;
            }
        }

        public List<CMTag> Tags { get => tags; set => tags = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
    }
}
