using Reflectis.SDK.CreatorKit;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(TeleportPlayerActionNode))]
    public class TeleportPlayerActionDescriptor : UnitDescriptor<TeleportPlayerActionNode>
    {
        public TeleportPlayerActionDescriptor(TeleportPlayerActionNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit requires a coroutine flow to run properly. This unit will teleport the player to a given transform.";
        }

    }
}
