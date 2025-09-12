using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMPermission
    {
        public enum EFacetIdentifier
        {
            Unknown = 0,
            MuteOthers = 1,
            KickOthers = 2,
            UseTools = 3,
            SpawnFiles = 7,
            SendGlobalAndShardMessages = 12,
            SendAnnouncementMessages = 14,
            ManageMyExperiencesAndSessions = 15,
            EnableSpeaker = 16,
            UseAuthoringTool = 17,
            InteractWithAllObjects = 18,
            UseVoiceChat = 19,
            ManageMyEvents = 20,
            ManageMySessions = 21,
            ManageMyAuthoredExperiences = 22,
            ShowCatalogTab = 23,
            ShowLeaderboard = 24,
            UseEmotes = 25,
            ShowLogo = 26,
            ShowTutorialButton = 27,
            EnableDoubleUserConnection = 28,
            ShowCompleteName = 29,
        }


        [SerializeField] private int tagId;
        [SerializeField] private int facetId;
        [SerializeField] private string facetLabel;
        [SerializeField] private EFacetIdentifier facetIdentifier;
        [SerializeField] private string facetGroup;
        [SerializeField] private bool overridableInEvent;
        [SerializeField] private bool isEnabled;

        public int TagId { get => tagId; set => tagId = value; }
        public int FacetId { get => facetId; set => facetId = value; }
        public string FacetLabel { get => facetLabel; set => facetLabel = value; }
        public EFacetIdentifier FacetIdentifier { get => facetIdentifier; set => facetIdentifier = value; }
        public string FacetGroup { get => facetGroup; set => facetGroup = value; }
        public bool OverridableInEvent { get => overridableInEvent; set => overridableInEvent = value; }
        public bool IsEnabled { get => isEnabled; set => isEnabled = value; }
    }
}
