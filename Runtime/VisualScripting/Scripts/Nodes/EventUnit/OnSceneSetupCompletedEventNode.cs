using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Scene: On Setup Completed")]
    [UnitSurtitle("Scene")]
    [UnitShortTitle("On Setup Completed")]
    [UnitCategory("Events\\Reflectis")]
    public class OnSceneSetupCompletedEventNode : EventUnit<string>
    {
        public static string EventName => "OnSceneSetupCompletedEvent";

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventName);
        }

    }
}

