using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Reflectis.SDK.CreatorKit
{
    public class ButtonVideoPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] VideoClip clip;
        [SerializeField] Button buttonRef;
        [SerializeField] MediaContainerPlaceholder videoPlayerRef;

        public VideoClip Clip => clip;
        public Button ButtonRef => buttonRef;
        public MediaContainerPlaceholder VideoPlayerRef => videoPlayerRef;
    }
}
