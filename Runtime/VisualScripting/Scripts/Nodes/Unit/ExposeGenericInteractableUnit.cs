using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Expose: Generic Interactable")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("Generic Interactable")]
    [UnitCategory("Expose\\Reflectis\\Interactable")]
    public class ExposeGenericInteractableUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput interactable { get; private set; }

        [DoNotSerialize]
        public ValueOutput gameObjectReference { get; private set; }

        [DoNotSerialize]
        public ValueOutput interactionState { get; private set; }

        [DoNotSerialize]
        public ValueOutput interactionColliders { get; private set; }

        protected override void Definition()
        {
            interactable = ValueInput<GenericInteractable>(nameof(interactable), null).NullMeansSelf();

            gameObjectReference = ValueOutput(nameof(gameObjectReference), (flow) => flow.GetValue<GenericInteractable>(interactable).InteractableRef.GameObjectRef);

            interactionState = ValueOutput(nameof(interactionState), (flow) => flow.GetValue<GenericInteractable>(interactable).InteractableRef.InteractionState);

            interactionColliders = ValueOutput(nameof(interactionColliders), (flow) => flow.GetValue<GenericInteractable>(interactable).InteractableRef.InteractionColliders);
        }


    }
}
