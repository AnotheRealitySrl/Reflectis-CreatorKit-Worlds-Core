using SPACS.Graphs;
using Virtuademy.SDK.Tasks;
using UnityEngine;


namespace Virtuademy.SDK.Environments.Tasks
{
    public class GrabTaskReflectis : TaskReflectis, ITaskNode<GrabTaskNode>
    {
        GrabTaskNode IContainer<GrabTaskNode>.Value { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        ///////////////////////////////////////////////////////////////////////////
        /*private void Awake()
        {
            OnStatusChanged(oldStatus: TaskStatus.Locked);
            Node.onStatusChanged.AddListener(OnStatusChanged);
        }*/

        public override void AddDetector()
        {
            base.AddDetector();
            GameObject go = new GameObject("ManipulableDetector");
            go.transform.SetParent(gameObject.transform);
            go.AddComponent<TaskReactor>();
            go.AddComponent<ManipulableGrabberDetector>().enabled = false;
        }
    }
}
