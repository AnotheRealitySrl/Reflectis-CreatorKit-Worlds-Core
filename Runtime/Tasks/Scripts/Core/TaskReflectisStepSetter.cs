using SPACS.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.SDK.Environments.Tasks
{
    public class TaskReflectisStepSetter : TaskStepSetter
    {
        protected ITasksRPCManager rpcManagerInterface;
        private TaskSystemReflectis systemReflectis;

        private void Start()
        {
            systemReflectis = GetComponent<TaskSystemReflectis>();
            if (systemReflectis.isNetworked)
            {
                StartCoroutine(WaitForRPCManager());
            }

        }

        private IEnumerator WaitForRPCManager()
        {
            while (rpcManagerInterface == null)
            {
                rpcManagerInterface = systemReflectis.rpcManagerInterface;
                yield return null;
            }
            rpcManagerInterface = systemReflectis.rpcManagerInterface;
            rpcManagerInterface.AddJoinRoomEvent(Init);
        }



        private void Init(int id)
        {
            if(id != -1){
                systemReflectis.Prepare();
            }
            //calculate last node
            var tasks = FindObjectsOfType<TaskReflectis>();
            TaskNode targetNode = null;
            foreach (var task in tasks)
            {
                if (task.TaskID == id)
                {
                    targetNode = task.Node;
                    break;
                }
            }

            TaskNode newNode = targetNode;
            if (targetNode.Dependencies.Count != 0 && targetNode.Next != null)
            {
                newNode = targetNode.Next;
            }

            // Ordered list of tasks. I assign the state "Complete" until I find the node I want.
            IReadOnlyCollection<TaskNode> allNodes = systemReflectis.Tasks;
            foreach (TaskNode node in allNodes)
            {
                if (CompleteTaskRecursive(node, newNode))
                {
                    // Target reached. Nothing else to do in this loop.
                    if (targetNode.Status == TaskNode.TaskStatus.Todo)
                        targetNode.Status = TaskNode.TaskStatus.Completed;
                    break;
                }
            }
        }
    }
}
