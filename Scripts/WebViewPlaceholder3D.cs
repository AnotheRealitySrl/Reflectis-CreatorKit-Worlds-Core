using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class WebViewPlaceholder3D : SceneComponentPlaceholderBase
    {
        [Header("Prefab instantiation")]
        [SerializeField] private string addressableKey;

        [Header("3D WebView settings")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private RectTransform canvasTransform;
        [SerializeField] private RectTransform uiTransform;

        [Header("General settings")]
        [SerializeField] private string urlSandbox;
        [SerializeField] private string urlProduction;
        //[SerializeField] private float resolution = 1.5f;
        //[SerializeField] private bool native2DMode = true;
        //[SerializeField] private float pixelDensity = 2f;

        //[Header("WebView controller settings")]
        //[SerializeField] private bool startWebViewVisible;

        //[Header("Canvas Group settings")]
        //[SerializeField] private bool ignoreParentGroups = true;

        [Header("Querystring formation")]
        [SerializeField] private WebViewQuerystringScriptable querystringMappings;


        public string AddressableKey => addressableKey;

        public Transform TargetTransform => targetTransform;
        public RectTransform CanvasTransform => canvasTransform;
        public RectTransform UiTransform => uiTransform;

        public string UrlSandbox => urlSandbox;
        public string UrlProduction => urlProduction;
        //public float Resolution => resolution;
        //public bool Native2DMode => native2DMode;
        //public float PixelDensity => pixelDensity;

        //public bool StartWebViewVisible => startWebViewVisible;

        //public bool IgnoreParentGroups => ignoreParentGroups;

        public WebViewQuerystringScriptable QuerystringMappings => querystringMappings;
    }
}


