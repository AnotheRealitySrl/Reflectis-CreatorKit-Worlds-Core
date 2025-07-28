using Reflectis.SDK.Core.SystemFramework;
using System.Collections.Generic;

namespace Reflectis.CreatorKit.Worlds.Core.Localization
{
    public interface ILocalizationSystem : ISystem
    {
        public string GetCurrentLocalization();
        public string GetCurrentLanguageCode();
        public List<string> GetLanguagesList();
    }
}
