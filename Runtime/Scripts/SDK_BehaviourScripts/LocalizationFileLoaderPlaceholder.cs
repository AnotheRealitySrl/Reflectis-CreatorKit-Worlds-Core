using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class LocalizationFileLoaderPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private TextAsset localizationCSV;

        public TextAsset LocalizationCSV { get => localizationCSV; set => localizationCSV = value; }
    }
}
