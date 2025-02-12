using Reflectis.SDK.Core.SystemFramework;

using System;
using System.Threading.Tasks;

using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Help
{
    public enum HelpState
    {
        Close,
        Open,
        InTransition
    }

    public interface IHelpSystem : ISystem
    {
        Task CallGetHelp();

        Task CallCloseGetHelp();

        void AddHelpAction(Func<Task> helpAction);

        void AddCloseHelpAction(Func<Task> closeHelpAction);

        public UnityEvent OnStartOpening { get; }

        public UnityEvent OnFinishedOpening { get; }

        public UnityEvent OnStartClosing { get; }

        public UnityEvent OnFinishedClosing { get; }

        void ResetHelpEvents();

        public bool FirstHelpCompleted { get; set; }

        public HelpState HelpState { get; }

        void CheckForFirstActivation();
    }
}