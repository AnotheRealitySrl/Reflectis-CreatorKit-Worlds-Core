using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis CustomProperty: New")]
    [UnitSurtitle("Reflectis CustomProperty")]
    [UnitShortTitle("New")]
    [UnitCategory("Reflectis\\New")]
    public class NewVisualScriptingCustomPropertyUnit : Unit
    {
        [DoNotSerialize]
        public ValueInput Name { get; private set; }

        [DoNotSerialize]
        public ValueInput Value { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput CustomProperty { get; private set; }

        protected override void Definition()
        {
            Name = ValueInput<string>(nameof(Name), null);

            Value = ValueInput<object>(nameof(Value), null);

            CustomProperty = ValueOutput(nameof(CustomProperty),
                (flow) => new VisualScriptingCustomProperty()
                {
                    propertyName = (string)flow.GetConvertedValue(Name),
                    value = flow.GetConvertedValue(Value)
                });
        }


    }
}