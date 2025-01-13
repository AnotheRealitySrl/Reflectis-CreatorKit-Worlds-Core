using Reflectis.CreatorKit.Core;
using Unity.VisualScripting;

namespace Reflectis.CreatorKit.CoreEditor
{
    [Descriptor(typeof(InitializePlaceholderNode))]
    public class InitializePlaceholderDescriptor : UnitDescriptor<InitializePlaceholderNode>
    {
        public InitializePlaceholderDescriptor(InitializePlaceholderNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit will manually activate initialization on the placeholder component attached " +
                "to the target gameobject. Check the flag \"Placeholders in children\" to initialize all the " +
                "placeholders attached to the target and to all its children.";
        }
    }
}
