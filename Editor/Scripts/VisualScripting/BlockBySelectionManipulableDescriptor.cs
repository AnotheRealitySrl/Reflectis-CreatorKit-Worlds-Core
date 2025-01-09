using Reflectis.CreatorKit.Core;
using Unity.VisualScripting;

namespace Reflectis.CreatorKit.CoreEditor
{
    [Descriptor(typeof(BlockBySelectionManipulableUnit))]
    public class BlockBySelectionManipulableDescriptor : UnitDescriptor<BlockBySelectionManipulableUnit>
    {
        public BlockBySelectionManipulableDescriptor(BlockBySelectionManipulableUnit unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit will enable or disable the interaction with a given Manipulable element.";
        }
    }
}
