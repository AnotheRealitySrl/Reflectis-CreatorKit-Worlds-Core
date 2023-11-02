using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{

    public class RPMAvatarWebViewPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private GameObject loader;
        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI goToWebsiteText;

        public GameObject Loader { get => loader; set => loader = value; }
        public Button Button { get => button; set => button = value; }
        public TextMeshProUGUI GoToWebsiteText { get => goToWebsiteText; set => goToWebsiteText = value; }
    }
}
