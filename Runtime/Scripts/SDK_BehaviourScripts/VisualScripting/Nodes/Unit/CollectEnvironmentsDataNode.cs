using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Environments: Get Current Environment")]
    [UnitSurtitle("Environment")]
    [UnitShortTitle("Get Current Environment")]
    [UnitCategory("ReflectisUnit")]
    public class CollectEnvironmentsDataNode : Unit
    {
        [PortLabelHidden]
        [DoNotSerialize]
        public ControlInput InputTrigger { get; private set; }
        [PortLabelHidden]
        [DoNotSerialize]
        public ControlOutput OutputTrigger { get; private set; }

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
            IClientModelSystem system = null;

            //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
            InputTrigger = ControlInput(nameof(InputTrigger), (flow) =>
            {
                system = SM.GetSystem<IClientModelSystem>();
                return OutputTrigger;
            });
            //Making the ControlOutput port visible and setting its key.
            OutputTrigger = ControlOutput(nameof(OutputTrigger));


            ID = ValueOutput(nameof(ID), (flow) =>
            {
                return system.CurrentEvent.Environment.ID;
            });

            Name = ValueOutput(nameof(Name), (flow) =>
            {
                return system.CurrentEvent.Environment.Name;
            });

            Description = ValueOutput(nameof(Description), (flow) =>
            {
                return system.CurrentEvent.Environment.Description;
            });

            AddressableKey = ValueOutput(nameof(AddressableKey), (flow) =>
            {
                return system.CurrentEvent.Environment.AddressableKey;
            });

            Catalog = ValueOutput(nameof(Catalog), (flow) =>
            {
                return system.CurrentEvent.Environment.Catalog;
            });
        }
    }
}