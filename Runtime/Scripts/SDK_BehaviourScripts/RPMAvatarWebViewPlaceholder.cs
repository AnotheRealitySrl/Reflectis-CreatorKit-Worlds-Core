using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{

    public class RPMAvatarWebViewPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private GameObject loader;
        [SerializeField]
        private TextMeshPro goToWebsiteText;

        private UnityEvent onClickEvent = new UnityEvent();

        public GameObject Loader { get => loader; set => loader = value; }
        public TextMeshPro GoToWebsiteText { get => goToWebsiteText; set => goToWebsiteText = value; }
        public UnityEvent OnClickEvent => onClickEvent;

        public void OnClickCustomizeInvoke()
        {
            OnClickEvent?.Invoke();
        }
    }
}
