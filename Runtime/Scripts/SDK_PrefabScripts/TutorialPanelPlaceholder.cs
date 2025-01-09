using Reflectis.SDK.Core.Transitions;

using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class TutorialPanelPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] float loopInterval = 5f;
        [SerializeField] List<CanvasGroup> images;
        [SerializeField] List<AbstractTransitionProvider> instructions;

        public float LoopInterval => loopInterval;
        public List<CanvasGroup> Images => images;
        public List<AbstractTransitionProvider> Instructions => instructions;
    }
}

