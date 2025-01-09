using Reflectis.SDK.Core.Interaction;

using Unity.VisualScripting;

using static Reflectis.SDK.Core.Interaction.Manipulable;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Manipulable: On Manipulation End")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation End")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationEndEventUnit : OnManipulationEventUnit
    {
        protected override void OnManipulableStateChange(Manipulable.EManipulableState manipulableState)
        {
            if (manipulableState == EManipulableState.Idle)
            {
                Trigger(graphReference, manipulableReference);
            }
        }

    }
}
