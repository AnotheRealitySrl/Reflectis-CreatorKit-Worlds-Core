using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class VideoPlayerPalceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private int initializationId;

        public int InitializationId => initializationId;
    }
}
