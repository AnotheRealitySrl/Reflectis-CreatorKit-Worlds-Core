using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Manipulable: On Manipulation End")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation End")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationEndEventUnit : OnManipulationEventUnit
    {
        protected override void OnManipulableStateChange(Manipulable.EManipulableState manipulableState)
        {
            if (manipulableState == InteractionNew.Manipulable.EManipulableState.Idle)
            {
                Trigger(graphReference, manipulableReference);
            }
        }

    }
}
