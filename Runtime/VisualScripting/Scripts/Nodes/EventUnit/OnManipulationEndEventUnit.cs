using Unity.VisualScripting;

using static Virtuademy.Environments.ScriptingApi.Interaction.IManipulable;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Manipulable: On Manipulation End")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("On Manipulation End")]
    [UnitCategory("Events\\Reflectis")]
    public class OnManipulationEndEventUnit : OnManipulationEventUnit
    {
        protected override bool ShouldTriggerOnChange(EManipulableState manipulableState)
        {
            return manipulableState == EManipulableState.Idle;
        }

    }
}
