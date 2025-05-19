using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMExperience
    {
        [SerializeField] private int id;
        [SerializeField] private int worldId;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private List<CMTag> tags;
        [SerializeField] private CMEnvironment environment;
        [SerializeField] private bool isFeatured;
        [SerializeField] private string featuredThumb;
        [SerializeField] private Texture thumbnailTexture;
        [SerializeField] private bool isPublic;
        [SerializeField] private bool isVisible;
        [SerializeField] private bool isOwner;
        [SerializeField] private bool isDraft;
        [SerializeField] private bool multiplayer;
        [SerializeField] private object config;
        [SerializeField] private bool canWrite;
        [SerializeField] private EExperienceType type;

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public CMEnvironment Environment { get => environment; set => environment = value; }
        public bool IsFeatured { get => isFeatured; set => isFeatured = value; }
        public string FeaturedThumb { get => featuredThumb; set => featuredThumb = value; }
        public bool IsPublic { get => isPublic; set => isPublic = value; }
        public bool IsVisible { get => isVisible; set => isVisible = value; }
        public bool IsOwner { get => isOwner; set => isOwner = value; }
        public bool IsDraft { get => isDraft; set => isDraft = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public object Config { get => config; set => config = value; }
        public bool CanWrite { get => canWrite; set => canWrite = value; }
        public List<CMTag> Tags { get => tags; set => tags = value; }
        public EExperienceType Type { get => type; set => type = value; }

        public Texture ThumbnailTexture
        {
            get => thumbnailTexture;
            set => thumbnailTexture = value;
        }

        public enum EExperienceType
        {
            Core,
            Authored,
        }
    }

}
