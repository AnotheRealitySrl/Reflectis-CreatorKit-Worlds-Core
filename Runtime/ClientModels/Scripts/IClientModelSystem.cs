using Reflectis.SDK.Core.SystemFramework;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine.Events;

using static Reflectis.CreatorKit.Worlds.Core.ClientModels.CMPermission;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    public enum FileTypeExt
    {
        None = -1,
        Video = 1,
        Documents = 2,
        Images = 3,
        Asset3D = 4,
    }

    public enum EPingStatus
    {
        Connected,
        Disconnected,
        Replaced
    }

    public interface IClientModelSystem : ISystem
    {

        #region Current Client Data
        #region Events
        public CMSession CurrentSession { get; }
        #endregion

        #region Shards
        public CMShard CurrentShard { get; }
        public List<CMShard> CurrentSessionShards { get; }
        public Action onShardsChange { get; set; }
        #endregion

        #region Worlds
        List<CMWorld> Worlds { get; }
        CMWorld CurrentWorld { get; }

        #endregion

        #region Session
        public string SessionId { get; }
        #endregion

        #region Users
        CMUser UserData { get; }
        #endregion

        #endregion

        #region Session
        Task StartConnection();

        Task EndConnection();

        Task<int> JoinWorld(int worldId, int eventId);
        #endregion

        #region Worlds
        /// <summary>
        /// Action called on changes on online users.
        /// The first int represent the worldId the second one the Users count
        /// </summary>
        public Action<int /*worldID*/, int /*usersCount*/> onOnlineUsersPerWorld { get; set; }

        /// <summary>
        /// Returns all the available worlds
        /// </summary>
        Task<List<CMWorld>> GetAllWorlds();

        void ConnectToOnlineUsersPerWorld();

        Task<CMWorld> GetWorld(int worldId);

        Task<CMWorldConfig> GetWorldConfig(int id);

        Task<List<CMCatalog>> GetWorldCatalogs(int worldId);

        public Task KickPlayer(string kickedUserSession);

        public Task LoadMyWorldData();
        #endregion

        #region Experiences
        public Task<List<CMExperience>> GetExperiences();

        public Task<List<CMExperience>> GetHighlightedExperiences();
        public Task DeleteExperience(int id);
        public Task DuplicateExperience(CMExperience currentData);
        public Task UpdateExperienceData(CMExperience cMExperience);
        public Task ToggleExperienceStatus(int id);
        #endregion

        #region Events
        /// <summary>
        /// Try to join event with given id at shard shardId
        /// Id shardId is null the system will try to join the event in any shard
        /// Response is the shard id where the player has been joined
        /// if the response is null the join has failed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shardId"></param>
        /// <returns></returns>
        Task<int?> JoinSession(int id, int? shardId);

        /// <summary>
        /// Setup current event entering data and starts downloading data for the given event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="shard"></param>
        /// <returns></returns>
        public Task LoadSessionShardData(int eventId, int shard);

        void LeaveEvent();

        void InvalidateEventCache();
        /// <summary>
        /// Force refresh on cached event data
        /// Refreshes also cached data that usually should not be refreshed (categories and environments)
        /// </summary>
        /// <returns></returns>
        Task RefreshAllCachedEventsData();

        /// <summary>
        /// Force refresh on cached event data.
        /// Refreshes only data that has a refresh expiring time
        /// </summary>
        /// <returns></returns>
        Task RefreshEventsData();

        /// <summary>
        /// Returns the static event for the given experienceId
        /// Such an event should always be present in the list of events
        /// otherwise null is returned
        /// </summary>
        Task<CMSession> GetStaticSession(int experienceId);

        /// <summary>
        /// Returns the static event for the given experienceId
        /// Such an event should always be present in the list of events
        /// otherwise null is returned
        /// </summary>
        Task<CMSession> GetStaticSessionByAddressableName(string addressableName);

        /// <summary>
        /// Returns an event given its id
        /// </summary>
        Task<CMSession> GetSessionById(int id, bool useCache = true);

        /// <summary>
        /// Returns the list of all events visible by user
        /// </summary>
        Task<List<CMSession>> GetActiveSessions();

        /// <summary>
        /// Return the list events in which the player is also the owner
        /// </summary>
        /// <returns></returns>
        Task<List<CMSession>> GetMyActiveEvents();

        /// <summary>
        /// Returns the list of all events visible by user filtered by category
        /// </summary>
        Task<List<CMSession>> GetActiveSessionsByTagID(int categoryId);

        /// <summary>
        /// Returns the list of all events visible by user filtered by environment
        /// </summary>
        Task<List<CMSession>> GetActiveEventsByEnvironmentID(int environmentId);

        /// <summary>
        /// Returns the list of all events visible by user filtered by environment
        /// </summary>
        Task<List<CMSession>> GetActiveEventsByEnvironmentName(string environmentName);
        /// <summary>
        /// Get sessions by date, month and year.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<List<CMSession>> GetSessionByDate(int month, int year);

        /// <summary>
        /// Create an event with given e data.
        /// If successfull return the eventId, -1 otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task<int> CreateSession(CMSession e);

        /// <summary>
        /// Delete an event with given id.
        /// If successfull return empty string, response reason phrase otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task<long> DeleteSessionById(int eventId);

        /// <summary>
        /// Update an event with given e data.
        /// If successfull return true, false otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task<bool> UpdateSession(CMSession e);

        /// <summary>
        /// Ask to API to replace all the users in the specified event with the users listed in <see cref="CMSession.Participants">
        /// </summary>
        /// <param name="cMEvent"></param>
        /// <returns></returns>
        Task<bool> InviteUsersToEvent(CMSession cMEvent, string eventInvitationMessage);

        /// <summary>
        /// Create all event permission for the given event
        /// </summary>
        /// <param name="_event"></param>
        /// <returns></returns>
        Task<bool> CreateSessionPermissions(CMSession _event);

        /// <summary>
        /// replace the asset list in the given event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="assets"></param>
        /// <returns></returns>
        Task<bool> UpdateAssetsInSession(int eventId, List<CMResource> assets);

        /// <summary>
        /// load the asset list saved previously.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="assets"></param>
        /// <returns></returns>
        Task<bool> UpdateCurrentEventExperienceSaveData(object assets);

        /// <summary>
        /// Create new authored experience
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        Task<CMExperience> CreateNewAuthoredExperience(object template);
        #endregion

        #region Environments

        Task<List<CMEnvironment>> GetEnvironments();

        #endregion

        #region Users
        /// <summary>
        /// Get all registered users in this world
        /// </summary>
        /// <returns></returns>
        Task<List<CMUser>> GetAllUsers();

        /// <summary>
        /// Return all users that match search criteria
        /// </summary>
        Task<List<CMUser>> SearchUsersByNickname(string nicknameSubstring);

        /// <summary>
        /// Return data of the player with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CMUser> GetUserData(int id);

        /// <summary>
        /// Update my user preferences outside of world context
        /// </summary>
        /// <param name="newPreferences"></param>
        /// <returns></returns>
        Task UpdateMyUserPreference(CMUserPreference newPreferences);

        /// <summary>
        /// Get all the users with given tag id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<CMUser>> GetUsersWithTag(int id);
        #endregion

        #region Permissions

        List<EFacetIdentifier> CurrentEventPermissions { get; }
        List<EFacetIdentifier> WorldPermissions { get; }

        bool IsPermissionGranted(EFacetIdentifier identifier);

        /// <summary>
        /// Get the permission available to the player for the given event
        /// </summary>
        /// <returns></returns>
        Task<List<EFacetIdentifier>> GetMyEventPermissions(int eventId);

        /// <summary>
        /// Get the permission available in the current event for a given tag
        /// </summary>
        /// <returns></returns>
        Task<List<CMPermission>> GetEventPermissionsByTag(int eventId, int tagId);

        /// <summary>
        /// Get the permission available for a given tag
        /// </summary>
        /// <returns></returns>
        Task<List<CMPermission>> GetAllPermissionsByTag(int tagId);

        #endregion

        #region Keys

        Task<bool> CheckMyKeys();

        #endregion

        #region Schedule

        Task<bool> CheckScheduleAccessibilityForToday();

        #endregion

        #region Assets

        Task<CMSearch<CMFolder>> GetWorldFolders(int startItem, int page = 1, IEnumerable<FileTypeExt> fileTypes = null);

        Task<CMSearch<CMResource>> GetWorldAssets(int startItem, bool buildSasThumbnailUrl, string path, int page = 1, IEnumerable<FileTypeExt> fileTypes = null);

        Task<CMSearch<CMResource>> SearchWorldAssets(string label, int startItem, bool buildSasThumbnailUrl, string path, int page = 1, IEnumerable<FileTypeExt> fileTypes = null);

        Task<CMResource> GetSessionAssetById(int assetId);

        Task<CMSearch<CMFolder>> GetEventAssetsFolders(int eventId, int pageSize, int page = 1, IEnumerable<FileTypeExt> fileTypes = null);

        Task<CMSearch<CMResource>> GetEventAssetsInFolder(int eventId, string path, int pageSize, int page = 1, IEnumerable<FileTypeExt> fileTypes = null);

        Task CreateEventAssetsAssociation(int eventId, List<CMResource> resources);

        #endregion

        #region Tags
        /// <summary>
        /// Get all tags
        /// </summary>
        Task<List<CMTag>> GetAllContentTags();

        /// <summary>
        /// Get all tags
        /// </summary>
        Task<List<CMTag>> GetAllUsersTags();

        /// <summary>
        /// Search user tag
        /// </summary>
        /// <param name="labelSubstring"></param>
        /// <returns></returns>
        Task<List<CMTag>> SearchUserTags(string labelSubstring);

        Task<List<CMTag>> GetAllExperiencesTags();

        Task<List<CMTag>> GetAllSessionsTags();
        #endregion

        #region Online presence
        UnityEvent OnlineUsersUpdated { get; }
        List<CMOnlineUser> GetOnlineUsers();
        CMOnlineUser GetOnlineUser(int userId);
        bool IsOnlineUser(int userId);
        List<CMOnlineUser> GetUsersInEvent(int eventId);

        #endregion

        #region Shard
        Task EnableShard(bool enable);
        #endregion
    }
}