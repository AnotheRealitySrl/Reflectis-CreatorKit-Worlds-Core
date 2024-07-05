using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Reflectis.SDK.InteractionNew;
using static Reflectis.SDK.InteractionNew.IInteractable;
using Reflectis.SDK.Core;

namespace Reflectis.SDK.CreatorKit
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

            SM.GetSystem<IGenericInteractionSystem>().OnSelectedInteractableChange.AddListener(OnSelectedChange);

            return new EventHook("GenericInteractable" + this.ToString().Split("EventUnit")[0]);
        }

        private void OnSelectedChange(GenericInteractable newSelection)
        {
            interactableReference = newSelection;
            Trigger(graphReference, interactableReference);
        }
    }
}
