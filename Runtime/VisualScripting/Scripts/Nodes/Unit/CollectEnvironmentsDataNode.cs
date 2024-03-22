using Reflectis.SDK.ClientModels;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Expose: CMEnvironment")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("CMEnvironment")]
    [UnitCategory("Reflectis\\Expose")]
    public class CollectEnvironmentsDataNode : Unit
    {
        [DoNotSerialize]
        public ValueInput CMEnvironment{ get; private set; }
        [DoNotSerialize]
        public ValueOutput ID { get; private set; }
        [DoNotSerialize]
        public ValueOutput Name { get; private set; }
        [DoNotSerialize]
        public ValueOutput Description { get; private set; }
        [DoNotSerialize]
        public ValueOutput AddressableKey { get; private set; }
        [DoNotSerialize]
        public ValueOutput Catalog { get; private set; }

        protected override void Definition()
        {
            CMEnvironment = ValueInput<CMEnvironment>(nameof(CMEnvironment), null).NullMeansSelf();

            ID = ValueOutput(nameof(ID), (flow) => flow.GetValue<CMEnvironment>(CMEnvironment).ID);

            Name = ValueOutput(nameof(Name), (flow) => flow.GetValue<CMEnvironment>(CMEnvironment).Name);

            Description = ValueOutput(nameof(Description), (flow) => flow.GetValue<CMEnvironment>(CMEnvironment).Description);

            AddressableKey = ValueOutput(nameof(AddressableKey), (flow) => flow.GetValue<CMEnvironment>(CMEnvironment).AddressableKey);

            Catalog = ValueOutput(nameof(Catalog), (flow) => flow.GetValue<CMEnvironment>(CMEnvironment).Catalog);
        }
    }
}