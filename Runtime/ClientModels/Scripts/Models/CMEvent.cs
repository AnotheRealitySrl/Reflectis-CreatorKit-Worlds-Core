using System;
using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMEvent
    {
        [SerializeField] private int id;
        [SerializeField] private string title;
        [SerializeField] private string? description;
        [SerializeField] private int worldId;
        [SerializeField] private CMCategory? category;
        [SerializeField] private CMCategory? subCategory;
        [SerializeField] private DateTime startDateTime;
        [SerializeField] private DateTime endDateTime;
        [SerializeField] private List<CMUser> participants;
        [SerializeField] private int maxParticipants;
        [SerializeField] private CMEnvironment? environment;
        [SerializeField] private bool isFeatured;
        [SerializeField] private string featuredThumb;
        [SerializeField] private bool isPublic;
        [SerializeField] private bool isVisible;
        [SerializeField] private List<CMPermission> permissions;
        [SerializeField] private bool isOwner;
        [SerializeField] private bool isDraft;
        [SerializeField] private List<CMResource> resources;
        [SerializeField] private string shortLink;
        [SerializeField] private bool multiplayer;
        [SerializeField] private object template;
        [SerializeField] private bool isFavorite;
        [SerializeField] private bool canJoin;
        [SerializeField] private bool canVisualize;
        [SerializeField] private bool canWrite;
        [SerializeField] private bool startingPoint;
        [SerializeField] private bool staticEvent;
        [SerializeField] private bool isLimited;

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string? Description { get => description; set => description = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public CMCategory? Category { get => category; set => category = value; }
        public CMCategory? SubCategory { get => subCategory; set => subCategory = value; }

        /// <summary>
        /// This DateTime is in local time
        /// </summary>
        public DateTime StartDateTime { get => startDateTime; set => startDateTime = value; }

        /// <summary>
        /// This DateTime is in local time
        /// </summary>
        public DateTime EndDateTime { get => endDateTime; set => endDateTime = value; }
        public List<CMUser> Participants { get => participants; set => participants = value; }
        public int MaxParticipants { get => maxParticipants; set => maxParticipants = value; }
        public CMEnvironment? Environment { get => environment; set => environment = value; }
        public bool IsFeatured { get => isFeatured; set => isFeatured = value; }
        public string FeaturedThumb { get => featuredThumb; set => featuredThumb = value; }
        public bool IsPublic { get => isPublic; set => isPublic = value; }
        public bool IsVisible { get => isVisible; set => isVisible = value; }
        public bool IsOwner { get => isOwner; set => isOwner = value; }
        public bool IsDraft { get => isDraft; set => isDraft = value; }
        public List<CMResource> Resources { get => resources; set => resources = value; }
        public string ShortLink { get => shortLink; set => shortLink = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public object Template { get => template; set => template = value; }
        public bool IsFavorite { get => isFavorite; set => isFavorite = value; }
        public bool CanJoin
        {
            get
            {
                bool onTime = isOwner || (DateTime.Now > StartDateTime && DateTime.Now < EndDateTime);
                return staticEvent || (canJoin && onTime);
            }

            set => canJoin = value;
        }

        public bool CanVisualize { get => canVisualize; set => canVisualize = value; }
        public bool CanWrite { get => canWrite; set => canWrite = value; }
        public bool StartingPoint { get => startingPoint; set => startingPoint = value; }
        public bool StaticEvent { get => staticEvent; set => staticEvent = value; }
        public List<CMPermission> Permissions { get => permissions; set => permissions = value; }
        public bool IsLimited { get => isLimited; set => isLimited = value; }
    }

}
