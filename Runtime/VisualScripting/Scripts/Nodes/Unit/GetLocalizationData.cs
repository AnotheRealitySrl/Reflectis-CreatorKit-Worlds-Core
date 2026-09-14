using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.VisualScripting;

using Virtuademy.ScriptingApi;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Localization: Get Localization Data")]
    [UnitSurtitle("LocalizationData")]
    [UnitShortTitle("Get LocalizationData")]
    [UnitCategory("Reflectis\\Get")]
    public class GetLocalizationData : Unit
    {
        public ValueOutput CurrentLanguage { get; private set; }
        public ValueOutput CurrentLanguageCode { get; private set; }
        public ValueOutput LanguageList { get; private set; }

        protected override void Definition()
        {
            CurrentLanguage = ValueOutput<string>(nameof(CurrentLanguage), (flow) => IVirtuademyFramework.Current.Localization.CurrentLanguage);
            CurrentLanguageCode = ValueOutput<string>(nameof(CurrentLanguageCode), (flow) => IVirtuademyFramework.Current.Localization.CurrentLanguageCode);
            LanguageList = ValueOutput<List<string>>(nameof(LanguageList), f => IVirtuademyFramework.Current.Localization.AvailableLanguages);
        }

    }
}
