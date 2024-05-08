using TMPro;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class LocalizationPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField, Tooltip("String key relative to the text description")]
        public string key;

        [SerializeField, Tooltip("the text of the textMeshPro where you want to display the description")]
        public TextMeshProUGUI textField;
    }
}
