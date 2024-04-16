using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class LocalizeTaskPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField, Tooltip("Enable description localization")]
        public bool localizeDescription = false;
        [SerializeField, Tooltip("Enable image localization")]
        public bool localizeImage = false;
        [SerializeField, Tooltip("Enable videoClip localization")]
        public bool localizeVideo = false;

        [Space]
        [SerializeField, Tooltip("String key relative to the task's description")]
        public string descriptionKey;
        [SerializeField, Tooltip("String key relative to the task's sprite")]
        public string imageKey;
        [SerializeField, Tooltip("String key relative to the task's video")]
        public string videoKey;
    }
}
