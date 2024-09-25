using Reflectis.SDK.Utilities;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Property: New")]
    [UnitSurtitle("Reflectis Property")]
    [UnitShortTitle("New")]
    [UnitCategory("Reflectis\\New")]
    public class NewPropertyUnit : Unit
    {
        [DoNotSerialize]
        public ValueInput Name { get; private set; }

        [DoNotSerialize]
        public ValueInput Value { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput Property { get; private set; }

        protected override void Definition()
        {
            Name = ValueInput<string>(nameof(Name), null);

            Value = ValueInput<object>(nameof(Value), null);

            Property = ValueOutput(nameof(Property),
                (flow) => new Property((string)flow.GetConvertedValue(Name), flow.GetConvertedValue(Value)));
        }


    }
}