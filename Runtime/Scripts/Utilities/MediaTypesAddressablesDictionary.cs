#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System.Collections.Generic;

using UnityEngine;

using static Reflectis.SDK.CreatorKit.MediaPlayerControllerPlaceholder;

[CreateAssetMenu(menuName = "Reflectis/SDK-CreatorsKit/MediaTypesAddressablesDictionary", fileName = "MediaTypesAddressablesDictionary")]
public class MediaTypesAddressablesDictionary : SerializedScriptableObject
{
    // This Dictionary will be serialized by Odin.
    [SerializeField] private Dictionary<MediaType, string> mediaTypesDictionary = new();

    public Dictionary<MediaType, string> MediaTypesDictionary => mediaTypesDictionary;
}