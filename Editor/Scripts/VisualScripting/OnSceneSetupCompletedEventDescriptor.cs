using Reflectis.SDK.CreatorKit;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(OnSceneSetupCompletedEventNode))]
    public class OnSceneSetupCompletedEventDescriptor : UnitDescriptor<OnSceneSetupCompletedEventNode>
    {
        public OnSceneSetupCompletedEventDescriptor(OnSceneSetupCompletedEventNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This event unit will be triggered when the scene setup is completed, after the loading panel " +
                "fade out.";
        }
    }
}
