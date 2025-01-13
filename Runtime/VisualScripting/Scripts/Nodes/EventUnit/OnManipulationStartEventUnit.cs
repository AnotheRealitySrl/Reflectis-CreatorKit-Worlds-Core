using Unity.VisualScripting;

using static Reflectis.SDK.Core.Interaction.Manipulable;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Manipulable: On Manipulation Start")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation Start")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationStartEventUnit : OnManipulationEventUnit
    {
        protected override void OnManipulableStateChange(EManipulableState manipulableState)
        {
            if (manipulableState == EManipulableState.Manipulating)
            {
                Trigger(graphReference, manipulableReference);
            }
        }

    }
}
