using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis GameObject: Get Contextual Menu Interactable")]
    [UnitSurtitle("GameObject")]
    [UnitShortTitle("Get Contextual Menu Interactable")]
    [UnitCategory("Reflectis\\Get")]
    public class GetContextualMenuInteractableUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput GameObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput ContextualMenuManageable { get; private set; }

        protected override void Definition()
        {
            GameObject = ValueInput<GameObject>(nameof(GameObject), null).NullMeansSelf();

            ContextualMenuManageable = ValueOutput(nameof(ContextualMenuManageable), (flow) => flow.GetValue<GameObject>(GameObject).GetComponent<ContextualMenuManageable>());
        }


    }
}
