using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Manipulable: On Manipulation End")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation End")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationEnd : EventUnit<Manipulable>
    {
        [DoNotSerialize]
        public ValueOutput Manipulable { get; private set; }
        protected override bool register => true;

        private GraphReference graphReference;

        private Manipulable manipulableReference;

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
            manipulableReference = reference.gameObject.GetComponent<Manipulable>();
            manipulableReference.OnCurrentStateChange.AddListener(OnManipulableStateChange);
            return new EventHook(typeof(OnManipulationStart).ToString());
        }

        private void OnManipulableStateChange(Manipulable.EManipulableState manipulableState)
        {
            if (manipulableState == InteractionNew.Manipulable.EManipulableState.Idle)
            {
                Trigger(graphReference, manipulableReference);
            }
        }

    }
}
