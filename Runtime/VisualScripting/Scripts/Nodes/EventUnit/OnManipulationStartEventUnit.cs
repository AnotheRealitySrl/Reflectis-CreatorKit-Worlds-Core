using Unity.VisualScripting;

using static Virtuademy.Environments.ScriptingApi.Interaction.IManipulable;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Manipulable: On Manipulation Start")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation Start")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationStartEventUnit : OnManipulationEventUnit
    {
        protected override bool ShouldTriggerOnChange(EManipulableState manipulableState)
        {
            return manipulableState == EManipulableState.Manipulating;
        }
    }
}
