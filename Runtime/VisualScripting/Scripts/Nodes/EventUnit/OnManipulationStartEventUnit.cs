using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Manipulable: On Manipulation Start")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation Start")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationStartEventUnit : OnManipulationEventUnit
    {
        protected override void OnManipulableStateChange(Manipulable.EManipulableState manipulableState)
        {
            if (manipulableState == InteractionNew.Manipulable.EManipulableState.Manipulating)
            {
                Trigger(graphReference, manipulableReference);
            }
        }

    }
}
