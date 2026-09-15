using SPACS.Tasks;
using System.Collections;
using UnityEngine;
using static SPACS.Tasks.TaskNode;

namespace Virtuademy.SDK.Environments.Tasks
{
    public class TaskReflectis : Task
    {
        protected bool forceCompleted = false;
        [SerializeField]
        protected int taskID;

        protected ITasksRPCManager rpcManagerInterface;
        private TaskSystemReflectis taskSystem;

        public int TaskID { get => taskID; }

        protected void Start()
        {
            taskSystem = GetComponentInParent<TaskSystemReflectis>();
            if (taskSystem.isNetworked)
            {
                StartCoroutine(WaitForRPCManager());
            }
        }

        IEnumerator WaitForRPCManager()
        {
            if (taskSystem)
                StartCoroutine(taskSystem.WaitForRPCManager());
            while (rpcManagerInterface == null)
            {
                rpcManagerInterface = taskSystem.rpcManagerInterface;
                yield return null;
            }
            rpcManagerInterface.SetOnTaskCompleted(ForceTaskComplete);
            forceCompleted = false;
        }

        protected override void OnStatusChanged(TaskStatus oldStatus)
        {
            if ((Node.Status == TaskStatus.Completed && !forceCompleted) && taskSystem.isNetworked)
            {

                taskSystem.rpcManagerInterface.UpdateTasksID(taskID);
                taskSystem.rpcManagerInterface.SendRPCTaskStatusChange(taskID);
            }

            forceCompleted = false;

            base.OnStatusChanged(oldStatus);
        }

        public void ForceTaskComplete(int id)
        {
            if (taskID != id)
                return;

            forceCompleted = true;
            CompleteTask();
        }

        //Auto-assign random id when task is created from graph
        protected void Reset()
        {
            taskID = Mathf.Abs(gameObject.GetInstanceID());
        }

        public override void AddDetector()
        {
            base.AddDetector();
        }
    }
}
