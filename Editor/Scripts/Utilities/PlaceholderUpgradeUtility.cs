using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    public abstract class PlaceholderUpgradeUtility : ScriptableObject
    {
        [SerializeField] private TextAsset legacyPlaceholder;

        public TextAsset LegacyPlaceholder { get => legacyPlaceholder; set => legacyPlaceholder = value; }

        public abstract void Upgrade();
    }
}
