using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Collect event data")]
    [UnitSurtitle("Event data")]
    [UnitShortTitle("Data")]
    [UnitCategory("Events\\Reflectis\\Data")]
    [TypeIcon(typeof(Material))]
    public class CollectEventDataNode : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueOutput eventID { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventTitle { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventDescription { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventStartDateTime { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventEndDateTime { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventCategoryID { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventCategoryName { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventSubcategoryID { get; private set; }
        [DoNotSerialize]
        public ValueOutput eventSubcategoryName { get; private set; }
        [DoNotSerialize]
        public ValueOutput isEventPublic { get; private set; }
        [DoNotSerialize]
        public ValueOutput isEventStatic { get; private set; }

        protected override void Definition()
        {
            IClientModelSystem system = null;

            //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
            inputTrigger = ControlInput(nameof(inputTrigger), (flow) =>
            {
                system = SM.GetSystem<IClientModelSystem>();
                return outputTrigger;
            });
            //Making the ControlOutput port visible and setting its key.
            outputTrigger = ControlOutput(nameof(outputTrigger));


            eventID = ValueOutput(nameof(eventID), (flow) =>
            {
                return system.CurrentEvent.Id;
            });

            eventTitle = ValueOutput(nameof(eventTitle), (flow) =>
            {
                return system.CurrentEvent.Title;
            });

            eventDescription = ValueOutput(nameof(eventDescription), (flow) =>
            {
                return system.CurrentEvent.Description;
            });

            eventStartDateTime = ValueOutput(nameof(eventStartDateTime), (flow) =>
            {
                return system.CurrentEvent.StartDateTime;
            });

            eventEndDateTime = ValueOutput(nameof(eventEndDateTime), (flow) =>
            {
                return system.CurrentEvent.EndDateTime;
            });

            eventCategoryID = ValueOutput(nameof(eventCategoryID), (flow) =>
            {
                return system.CurrentEvent.Category.ID;
            });

            eventCategoryName = ValueOutput(nameof(eventCategoryName), (flow) =>
            {
                return system.CurrentEvent.Category.Name;
            });

            eventSubcategoryID = ValueOutput(nameof(eventSubcategoryID), (flow) =>
            {
                return system.CurrentEvent.SubCategory.ID;
            });

            eventSubcategoryID = ValueOutput(nameof(eventSubcategoryName), (flow) =>
            {
                return system.CurrentEvent.SubCategory.Name;
            });

            isEventPublic = ValueOutput(nameof(isEventPublic), (flow) =>
            {
                return system.CurrentEvent.IsPublic;
            });

            isEventStatic = ValueOutput(nameof(isEventStatic), (flow) =>
            {
                return system.CurrentEvent.StaticEvent;
            });
        }
    }
}