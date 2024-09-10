using Reflectis.SDK.CreatorKit;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(OnSceneUnloadEventNode))]
    public class OnSceneUnloadEventDescriptor : UnitDescriptor<OnSceneUnloadEventNode>
    {
        public OnSceneUnloadEventDescriptor(OnSceneUnloadEventNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This event unit will be triggered when the scene is unloaded, hence when we are exiting " +
                "the current room to move to another room. The execution of all the flows started " +
                "from this node trigger will be awaited before proceeding with the next scene load. " +
                "This unit will NOT be called if the user exits the game or closes the browser's tab.";
        }

    }
}
