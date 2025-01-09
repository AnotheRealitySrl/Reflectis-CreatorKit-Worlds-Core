using Reflectis.SDK.Core.Interaction;

using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis GameObject: Get Generic Interactable")]
    [UnitSurtitle("GameObject")]
    [UnitShortTitle("Get Generic Interactable")]
    [UnitCategory("Reflectis\\Get")]
    public class GetGenericInteractableUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput GameObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput GenericInteractable { get; private set; }

        protected override void Definition()
        {
            GameObject = ValueInput<GameObject>(nameof(GameObject), null).NullMeansSelf();

            GenericInteractable = ValueOutput(nameof(GenericInteractable), (flow) => flow.GetValue<GameObject>(GameObject).GetComponent<GenericInteractable>());
        }


    }
}
