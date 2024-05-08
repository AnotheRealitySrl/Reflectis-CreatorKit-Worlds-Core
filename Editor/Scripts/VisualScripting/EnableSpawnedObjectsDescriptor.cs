using Reflectis.SDK.CreatorKit;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(EnableSpawnedObjectsNode))]
    public class EnableSpawnedObjectsDescriptor : UnitDescriptor<EnableSpawnedObjectsNode>
    {
        public EnableSpawnedObjectsDescriptor(EnableSpawnedObjectsNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "NOT IMPLEMENTED YET!";
        }
    }
}
