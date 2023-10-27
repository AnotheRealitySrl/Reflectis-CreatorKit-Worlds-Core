using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{

    public class RPMAvatarWebViewButtonPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private GameObject loader;
        [SerializeField]
        private Button button;

        public GameObject Loader { get => loader; set => loader = value; }
        public Button Button { get => button; set => button = value; }
    }
}
