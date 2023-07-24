using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

using static Virtuademy.Placeholders.MediaPlayerControllerPlaceholder;

[CreateAssetMenu(menuName = "Reflectis/SDK-WorldEditor/MediaTypesAddressablesDictionary", fileName = "MediaTypesAddressablesDictionary")]
public class MediaTypesAddressablesDictionary : SerializedScriptableObject
{
    // This Dictionary will be serialized by Odin.
    [SerializeField] private Dictionary<MediaType, string> mediaTypesDictionary = new();

    public Dictionary<MediaType, string> MediaTypesDictionary => mediaTypesDictionary;
}