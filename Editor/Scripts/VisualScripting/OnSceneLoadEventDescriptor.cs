using Reflectis.CreatorKit.Core;
using Unity.VisualScripting;

namespace Reflectis.CreatorKit.CoreEditor
{
    [Descriptor(typeof(OnSceneLoadEventNode))]
    public class OnSceneLoadEventDescriptor : UnitDescriptor<OnSceneLoadEventNode>
    {
        public OnSceneLoadEventDescriptor(OnSceneLoadEventNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This event unit will be triggered when the scene is loaded. The execution of all the flows started " +
                "from this node trigger will be awaited before proceeding with the scene setup. You can use this node" +
                " to instantiate " +
                "scene objects with placeholders attached to them.";
        }
    }
}
