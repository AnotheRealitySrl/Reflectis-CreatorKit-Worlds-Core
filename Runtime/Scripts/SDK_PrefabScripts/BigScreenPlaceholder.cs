using Reflectis.SDK.ClientModels;
using Reflectis.SDK.CreatorKit;
using Reflectis.SDK.Utilities;

using UnityEngine;

public class BigScreenPlaceholder : SceneComponentPlaceholderNetwork
{
    [SerializeField, Tooltip("The transform that contains the body of the media player. Do not change this reference unless a custom prefab is needed.")]
    private Transform contentTransform;
    [SerializeField, Tooltip("The transform that contains the media being reproduced. Do not change this reference unless a custom prefab is needed.")]
    private Transform screenTransform;
    [SerializeField, Tooltip("If this flag is set, a default media is loaded in the big screen.")]
    private bool defaultMedia;
    [SerializeField, Tooltip("The type of media being loaded by default. Valid only if defaultMedia is set to true")]
    [DrawIf(nameof(defaultMedia), true)]
    private FileTypeExt mediaType;
    [SerializeField, Tooltip("The url of the media being loaded by default. Valid only if defaultMedia is set to true")]
    [DrawIf(nameof(defaultMedia), true)]
    private string defaultUrl;
    [SerializeField, Tooltip("If set to true, it won't be possible to change the media on this screen")]
    [DrawIf(nameof(defaultMedia), true)]
    private bool isLocked;

    public Transform ScreenTransform => screenTransform;
    public Transform ContentTransform => contentTransform;
    public bool DefaultMedia => defaultMedia;
    public FileTypeExt MediaType => mediaType;
    public string DefaultUrl => defaultUrl;
    public bool IsLocked => isLocked;
}