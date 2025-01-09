using Reflectis.CreatorKit.Core;
using Unity.VisualScripting;

namespace Reflectis.CreatorKit.CoreEditor
{
    [Descriptor(typeof(CheckPlatformUnit))]
    public class CheckPlatformDescriptor : UnitDescriptor<CheckPlatformUnit>
    {
        public CheckPlatformDescriptor(CheckPlatformUnit unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit allows to define a flow based on which platform the experience is running.";
        }
    }
}
