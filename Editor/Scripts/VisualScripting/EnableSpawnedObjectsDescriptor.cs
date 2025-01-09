using Reflectis.CreatorKit.Core;
using Unity.VisualScripting;

namespace Reflectis.CreatorKit.CoreEditor
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
