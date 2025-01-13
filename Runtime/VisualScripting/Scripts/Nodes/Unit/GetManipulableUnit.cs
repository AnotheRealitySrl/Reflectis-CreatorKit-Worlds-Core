using Reflectis.SDK.Core.Interaction;

using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis GameObject: Get Manipulable")]
    [UnitSurtitle("GameObject")]
    [UnitShortTitle("Get Manipulable")]
    [UnitCategory("Reflectis\\Get")]
    public class GetManipulableUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput GameObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput Manipulable { get; private set; }

        protected override void Definition()
        {
            GameObject = ValueInput<GameObject>(nameof(GameObject), null).NullMeansSelf();

            Manipulable = ValueOutput(nameof(Manipulable), (flow) => flow.GetValue<GameObject>(GameObject).GetComponent<Manipulable>());
        }


    }
}
