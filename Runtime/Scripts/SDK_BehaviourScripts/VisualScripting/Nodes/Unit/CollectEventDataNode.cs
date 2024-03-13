using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Event: Get Current Event")]
    [UnitSurtitle("Event")]
    [UnitShortTitle("Get Current Event")]
    [UnitCategory("ReflectisUnit")]
    public class CollectEventDataNode : Unit
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
        public ValueOutput Title { get; private set; }
        [DoNotSerialize]
        public ValueOutput Description { get; private set; }
        [DoNotSerialize]
        public ValueOutput StartDateTime { get; private set; }
        [DoNotSerialize]
        public ValueOutput EndDateTime { get; private set; }
        [DoNotSerialize]
        public ValueOutput CategoryID { get; private set; }
        [DoNotSerialize]
        public ValueOutput CategoryName { get; private set; }
        [DoNotSerialize]
        public ValueOutput SubcategoryID { get; private set; }
        [DoNotSerialize]
        public ValueOutput SubcategoryName { get; private set; }
        [DoNotSerialize]
        public ValueOutput IsEventPublic { get; private set; }
        [DoNotSerialize]
        public ValueOutput IsEventStatic { get; private set; }

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
                return system.CurrentEvent.Id;
            });

            Title = ValueOutput(nameof(Title), (flow) =>
            {
                return system.CurrentEvent.Title;
            });

            Description = ValueOutput(nameof(Description), (flow) =>
            {
                return system.CurrentEvent.Description;
            });

            StartDateTime = ValueOutput(nameof(StartDateTime), (flow) =>
            {
                return system.CurrentEvent.StartDateTime;
            });

            EndDateTime = ValueOutput(nameof(EndDateTime), (flow) =>
            {
                return system.CurrentEvent.EndDateTime;
            });

            CategoryID = ValueOutput(nameof(CategoryID), (flow) =>
            {
                return system.CurrentEvent.Category.ID;
            });

            CategoryName = ValueOutput(nameof(CategoryName), (flow) =>
            {
                return system.CurrentEvent.Category.Name;
            });

            SubcategoryID = ValueOutput(nameof(SubcategoryID), (flow) =>
            {
                return system.CurrentEvent.SubCategory.ID;
            });

            SubcategoryID = ValueOutput(nameof(SubcategoryName), (flow) =>
            {
                return system.CurrentEvent.SubCategory.Name;
            });

            IsEventPublic = ValueOutput(nameof(IsEventPublic), (flow) =>
            {
                return system.CurrentEvent.IsPublic;
            });

            IsEventStatic = ValueOutput(nameof(IsEventStatic), (flow) =>
            {
                return system.CurrentEvent.StaticEvent;
            });
        }
    }
}