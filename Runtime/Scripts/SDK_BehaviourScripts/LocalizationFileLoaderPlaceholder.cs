using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class LocalizationFileLoaderPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private TextAsset localizationCSV;

        public TextAsset LocalizationCSV { get => localizationCSV; set => localizationCSV = value; }
    }
}
