using Reflectis.SDK.Core;
using Reflectis.SDK.Core.Interaction;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Generic Interactable: On Selected Change")]
    [UnitSurtitle("GenericInteractable")]
    [UnitShortTitle("On Selected Change")]
    [UnitCategory("Events\\Reflectis")]
    public class OnSelectedGenericInteractableChange : EventUnit<GenericInteractable>
    {
        [DoNotSerialize]
        public ValueOutput GenericInteractable { get; private set; }
        protected override bool register => true;

        protected GraphReference graphReference;

        protected GenericInteractable interactableReference;

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            GenericInteractable = ValueOutput<GenericInteractable>(nameof(GenericInteractable));
        }

        protected override void AssignArguments(Flow flow, GenericInteractable data)
        {
            flow.SetValue(GenericInteractable, interactableReference);
        }

        public override EventHook GetHook(GraphReference reference)
        {
            graphReference = reference;

            return new EventHook("GenericInteractable" + this.ToString().Split("EventUnit")[0]);
        }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            SM.GetSystem<IGenericInteractionSystem>().OnSelectedInteractableChange.AddListener(OnSelectedChange);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);

            SM.GetSystem<IGenericInteractionSystem>().OnSelectedInteractableChange.RemoveListener(OnSelectedChange);
        }

        private void OnSelectedChange(GenericInteractable newSelection)
        {
            interactableReference = newSelection;
            Trigger(graphReference, interactableReference);
        }
    }
}
