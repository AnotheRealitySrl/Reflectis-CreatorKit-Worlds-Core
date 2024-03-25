using Reflectis.SDK.InteractionNew;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Reflectis.SDK.InteractionNew.IInteractable;

namespace Reflectis.SDK.CreatorKit
{
    public abstract class OnManipulationEventUnit : EventUnit<Manipulable>
    {
        [DoNotSerialize]
        public ValueOutput Manipulable { get; private set; }
        protected override bool register => true;

        protected GraphReference graphReference;

        protected Manipulable manipulableReference;

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            Manipulable = ValueOutput<Manipulable>(nameof(Manipulable));
        }

        protected override void AssignArguments(Flow flow, Manipulable data)
        {
            flow.SetValue(Manipulable, manipulableReference);
        }

        public override EventHook GetHook(GraphReference reference)
        {
            graphReference = reference;
            RegisterToManipulableGrab(reference.gameObject);

            return new EventHook("Manipulable" + this.ToString().Split("EventUnit")[0]);
        }

        private async void RegisterToManipulableGrab(GameObject gameObject)
        {
            manipulableReference = gameObject.GetComponent<Manipulable>();
            if (manipulableReference == null)
            {
                if (gameObject.TryGetComponent<InteractablePlaceholder>(out var interactablePlaceholder) && ((interactablePlaceholder.InteractionModes & EInteractableType.Manipulable) == EInteractableType.Manipulable))
                {
                    manipulableReference = gameObject.GetComponent<Manipulable>();
                    while (manipulableReference == null)
                    {
                        await Task.Yield();
                        manipulableReference = gameObject.GetComponent<Manipulable>();
                    }
                }
            }
            if (manipulableReference == null)
            {
                Debug.LogError("The unit " + this.ToString().Split("EventUnit")[0] + "in the graph on the game object " + gameObject + " needs a proper interactable placeholder" +
                    " on the game object to function.");
            }
            else
            {
                manipulableReference.OnCurrentStateChange.AddListener(OnManipulableStateChange);
            }
        }

        protected abstract void OnManipulableStateChange(Manipulable.EManipulableState manipulableState);
    }
}
