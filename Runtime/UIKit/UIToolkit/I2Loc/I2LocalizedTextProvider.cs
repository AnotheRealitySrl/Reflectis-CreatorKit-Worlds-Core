// ============================================================
// I2LocalizedTextProvider.cs
//
// Belongs to Virtuademy.SDK.Environments.I2Loc — compiled only when
// I2LOC is defined (I2Loc package present in the project).
//
// Bridges I2Loc to the LocalizedXxx elements defined in
// Virtuademy.SDK.Environments: it registers itself as their
// ILocalizedTextProvider and forwards I2's language-change event.
// It defines no element types, so UXML stays identical with and
// without I2Loc.
// ============================================================
using I2.Loc;

using UnityEngine;

namespace Virtuademy.LocalizedComponents
{
    public sealed class I2LocalizedTextProvider : ILocalizedTextProvider
    {
        private static I2LocalizedTextProvider instance;

        /// <summary>Registers I2Loc as the translation backend of the LocalizedXxx elements. Idempotent.</summary>
        public static void Register()
        {
            if (instance != null && LocalizationHelper.Provider == instance)
            {
                return;
            }
            if (instance == null)
            {
                instance = new I2LocalizedTextProvider();
                LocalizationManager.OnLocalizeEvent += LocalizationHelper.NotifyLanguageChanged;
            }
            LocalizationHelper.SetProvider(instance);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterAtRuntime() => Register();

#if UNITY_EDITOR
        // Edit mode too, so the UI Builder preview and the inspectors show the translations.
        [UnityEditor.InitializeOnLoadMethod]
        private static void RegisterInEditor() => Register();
#endif

        public string Translate(string key) => LocalizationManager.GetTranslation(key);
    }
}
