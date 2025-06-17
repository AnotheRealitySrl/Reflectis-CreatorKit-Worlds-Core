using System;
using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMSession
    {
        [SerializeField] private int id;
        [SerializeField] private CMExperience experience;
        [SerializeField] private string title;
        [SerializeField] private DateTime? startDateTime;
        [SerializeField] private DateTime? endDateTime;
        [SerializeField] private List<CMUser> participants;
        [SerializeField] private List<CMTag> tags;
        [SerializeField] private int maxParticipants;
        [SerializeField] private bool isPublic;
        [SerializeField] private bool isVisible;
        [SerializeField] private List<CMPermission> permissions;
        [SerializeField] private bool isOwner;
        [SerializeField] private string shortLink;
        [SerializeField] private bool multiplayer;
        [SerializeField] private object template;
        [SerializeField] private bool canVisualize;
        [SerializeField] private bool canWrite;
        [SerializeField] private bool isLimited;
        [SerializeField] private bool isStatic;
        [SerializeField] private bool isScheduled;

        /// <summary>
        /// This DateTime is in local time
        /// </summary>
        public DateTime? StartDateTime { get => startDateTime; set => startDateTime = value; }

        /// <summary>
        /// This DateTime is in local time
        /// </summary>
        public DateTime? EndDateTime { get => endDateTime; set => endDateTime = value; }
        public List<CMUser> Participants { get => participants; set => participants = value; }
        public int MaxParticipants { get => maxParticipants; set => maxParticipants = value; }
        public bool IsPublic { get => isPublic; set => isPublic = value; }
        public bool IsVisible { get => isVisible; set => isVisible = value; }
        public bool IsOwner { get => isOwner; set => isOwner = value; }
        public string ShortLink { get => shortLink; set => shortLink = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public object Template { get => template; set => template = value; }
        public bool CanWrite { get => IsOwner; }
        public List<CMPermission> Permissions { get => permissions; set => permissions = value; }
        public bool IsLimited { get => isLimited; set => isLimited = value; }
        public CMExperience Experience { get => experience; set => experience = value; }
        public int Id { get => id; set => id = value; }
        public List<CMTag> Tags { get => tags; set => tags = value; }
        public string Title { get => title; set => title = value; }
        public bool IsStatic { get => isStatic; set => isStatic = value; }
        public bool IsScheduled { get => isScheduled; set => isScheduled = value; }
    }

}
