using Reflectis.SDK.Core.Interaction;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis: Expose Generic Interactable")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("Generic Interactable")]
    [UnitCategory("Reflectis\\Expose")]
    public class ExposeGenericInteractableUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Interactable { get; private set; }

        [DoNotSerialize]
        public ValueOutput GameObjectReference { get; private set; }

        [DoNotSerialize]
        public ValueOutput InteractionState { get; private set; }

        [DoNotSerialize]
        public ValueOutput InteractionColliders { get; private set; }

        protected override void Definition()
        {
            Interactable = ValueInput<GenericInteractable>(nameof(Interactable), null).NullMeansSelf();

            GameObjectReference = ValueOutput(nameof(GameObjectReference), (flow) => flow.GetValue<GenericInteractable>(Interactable).InteractableRef.GameObjectRef);

            InteractionState = ValueOutput(nameof(InteractionState), (flow) => flow.GetValue<GenericInteractable>(Interactable).InteractableRef.InteractionState);

            InteractionColliders = ValueOutput(nameof(InteractionColliders), (flow) => flow.GetValue<GenericInteractable>(Interactable).InteractableRef.InteractionColliders);
        }


    }
}
