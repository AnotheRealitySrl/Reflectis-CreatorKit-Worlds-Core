using Virtuademy.SDK.Environments.Interaction;

using Unity.VisualScripting;

using UnityEngine;

using Virtuademy.Environments.ScriptingApi.Interaction;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis GameObject: Get Interactable")]
    [UnitSurtitle("GameObject")]
    [UnitShortTitle("Get Interactable")]
    [UnitCategory("Reflectis\\Get")]
    public class GetInteractableUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput GameObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput Interactable { get; private set; }

        protected override void Definition()
        {
            GameObject = ValueInput<GameObject>(nameof(GameObject), null).NullMeansSelf();

            Interactable = ValueOutput(nameof(Interactable), (flow) => flow.GetValue<GameObject>(GameObject).GetComponent<IInteractable>());
        }


    }
}
