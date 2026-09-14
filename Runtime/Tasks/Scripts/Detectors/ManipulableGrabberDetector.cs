
using Virtuademy.Environments.ScriptingApi.Interaction;
using Virtuademy.Environments.ScriptingApi.Placeholders;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Virtuademy.SDK.Environments.Tasks
{
    public class ManipulableGrabberDetector : MonoBehaviour
    {
        [HideInInspector]
        public InteractablePlaceholderObsolete interactablePlaceholder;
        public ManipulablePlaceholder manipulablePlaceholder;
        public UnityEvent OnGrabStart = default;
        public UnityEvent OnGrabEnd = default;
        public UnityEvent OnRayGrabStart = default;
        public UnityEvent OnRayGrabEnd = default;


        public async void Setup()
        {
            //gameObject.GetOrAddComponent<Grabbable>();
            if (manipulablePlaceholder == null)
            {
                return;
            }
            //add OnGrabEvent to the Manipulable. The ManipulableVR and ManipulableDesktop will have a callback on that event
            IManipulable manipulable = null;
            while (!manipulablePlaceholder.TryGetComponent(out manipulable))
            {
                await Task.Yield();
            }

            if (manipulable != null)
            {
                if (manipulable.OnGrabManipulableStart == null)
                {
                    manipulable.OnGrabManipulableStart = new UnityEvent();
                }
                manipulable.OnGrabManipulableStart.AddListener(OnGrabStartEvent);
                manipulable.OnGrabManipulableEnd.AddListener(OnGrabEndEvent);
                manipulable.OnRayGrabManipulableStart.AddListener(OnRayGrabStartEvent);
                manipulable.OnRayGrabManipulableEnd.AddListener(OnRayGrabEndEvent);
            }
        }

        public void OnEnable()
        {
            Setup();
        }

        public void OnDisable()
        {
            IManipulable manipulable = manipulablePlaceholder.gameObject.GetComponent<IManipulable>();

            if (manipulable != null)
            {
                if (manipulable.OnGrabManipulableStart == null)
                {
                    manipulable.OnGrabManipulableStart = new UnityEvent();
                }

                manipulable.OnGrabManipulableStart.RemoveListener(OnGrabStartEvent);
                manipulable.OnGrabManipulableEnd.RemoveListener(OnGrabEndEvent);
                manipulable.OnRayGrabManipulableStart.RemoveListener(OnRayGrabStartEvent);
                manipulable.OnRayGrabManipulableEnd.RemoveListener(OnRayGrabEndEvent);
            }
        }

        private void OnGrabStartEvent()
        {
            OnGrabStart.Invoke();
        }

        private void OnGrabEndEvent()
        {
            OnGrabEnd.Invoke();
        }

        private void OnRayGrabStartEvent()
        {
            OnRayGrabStart.Invoke();
        }

        private void OnRayGrabEndEvent()
        {
            OnRayGrabEnd.Invoke();
        }
    }
}

