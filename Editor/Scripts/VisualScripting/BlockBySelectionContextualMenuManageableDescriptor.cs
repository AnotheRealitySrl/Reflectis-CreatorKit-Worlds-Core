using Reflectis.CreatorKit.Core;
using Unity.VisualScripting;

namespace Reflectis.CreatorKit.CoreEditor
{
    [Descriptor(typeof(BlockBySelectionContextualMenuManageableUnit))]
    public class BlockBySelectionContextualMenuManageableDescriptor : UnitDescriptor<BlockBySelectionContextualMenuManageableUnit>
    {
        public BlockBySelectionContextualMenuManageableDescriptor(BlockBySelectionContextualMenuManageableUnit unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit will enable or disable the interaction with a given ContextualMenuManageable element.";
        }
    }
}
