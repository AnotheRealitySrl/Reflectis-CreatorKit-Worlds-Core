using Reflectis.SDK.Core.SystemFramework;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Localization
{
    public interface ILocalizationSystem : ISystem
    {
        public UnityEvent<string> OnLanguageChanged { get; set; }
        public string GetCurrentLocalization();
        public string GetCurrentLanguageCode();
        public string GetPreviousLanguage();
        public string GetPreviousLanguageCode();
        public List<string> GetLanguagesList();
        public void LanguageChangedEvent();
        public void SetPreviousLanguage();
    }
}
