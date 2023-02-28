using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Virtuademy.Placeholders
{
    public class WebViewPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Prefab instantiation")]
        [SerializeField] private string addressableKey;

        [Header("Canvas settings")]
        [SerializeField] private RenderMode renderMode = RenderMode.ScreenSpaceOverlay;

        [Header("Canvas Scaler settings")]
        [SerializeField] private int rectWidth;
        [SerializeField] private int rectHeight;
        [SerializeField] private CanvasScaler.ScaleMode uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        [SerializeField] private float referenceResolutionX = 1280f;
        [SerializeField] private float referenceResolutionY = 720f;
        [SerializeField] private CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        [SerializeField] private float match = 0.5f;
        [SerializeField] private CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points;
        [SerializeField] private float fallbackScreenDPI = 96f;
        [SerializeField] private float defaultSpriteDPI = 96f;
        [SerializeField] private float referencePixelPerUnit = 100f;

        [Header("WebView settings")]
        [SerializeField] private string initialUrl;
        [SerializeField] private float resolution = 1.5f;
        [SerializeField] private bool native2DMode = true;
        [SerializeField] private float pixelDensity = 2f;

        [Header("WebView controller settings")]
        [SerializeField] private bool startWebViewVisible;

        [Header("Canvas Group settings")]
        [SerializeField] private bool ignoreParentGroups = true;


        public string AddressableKey => addressableKey;

        public int RectWidth => rectWidth;
        public int RectHeight => rectHeight;
        public RenderMode RenderMode => renderMode;
        public CanvasScaler.ScaleMode UiScaleMode => uiScaleMode;
        public float ReferenceResolutionX => referenceResolutionX;
        public float ReferenceResolutionY => referenceResolutionY;
        public CanvasScaler.ScreenMatchMode ScreenMatchMode => screenMatchMode;
        public float Match => match;
        public CanvasScaler.Unit PhysicalUnit => physicalUnit;
        public float FallbackScreenDPI => fallbackScreenDPI;
        public float DefaultSpriteDPI => defaultSpriteDPI;
        public float ReferencePixelPerUnit => referencePixelPerUnit;

        public string InitialUrl => initialUrl;
        public float Resolution => resolution;
        public bool Native2DMode => native2DMode;
        public float PixelDensity => pixelDensity;

        public bool StartWebViewVisible => startWebViewVisible;

        public bool IgnoreParentGroups => ignoreParentGroups;
    }
}

