using Reflectis.SDK.CreatorKit;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(ManageTutorialActionNode))]
    public class ManageTutorialActionDescriptor : UnitDescriptor<ManageTutorialActionNode>
    {
        public ManageTutorialActionDescriptor(ManageTutorialActionNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit requires a coroutine flow to run properly. This unit will activate the tutorial or deactivate it.";
        }

    }
}
