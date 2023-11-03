using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Reflectis.SDK.CreatorKit
{
    public class ButtonVideoPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string mediaUrl;
        [SerializeField] VideoClip clip;
        [SerializeField] MediaContainerPlaceholder videoPlayerRef;

        public string MediaUrl => mediaUrl;
        public VideoClip Clip => clip;
        public MediaContainerPlaceholder VideoPlayerRef => videoPlayerRef;
    }
}
