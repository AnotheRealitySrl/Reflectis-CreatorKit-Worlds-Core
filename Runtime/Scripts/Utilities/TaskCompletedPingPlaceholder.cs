using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public enum TasksToPing { MacroTasksOnly, AllTasks }

    public class TaskCompletedPingPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private TasksToPing taskToPingSetting;

        public TasksToPing TaskToPingSetting { get => taskToPingSetting; }
    }
}
