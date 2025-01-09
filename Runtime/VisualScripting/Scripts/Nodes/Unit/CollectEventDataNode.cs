using Reflectis.CreatorKit.Core.ClientModels;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Expose: CMEvent")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("CMEvent")]
    [UnitCategory("Reflectis\\Expose")]
    public class CollectEventDataNode : Unit
    {
        [DoNotSerialize]
        public ValueInput CMEvent { get; private set; }

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
            CMEvent = ValueInput<CMEvent>(nameof(CMEvent), null).NullMeansSelf();

            ID = ValueOutput(nameof(ID), (flow) => flow.GetValue<CMEvent>(CMEvent).Id);

            Title = ValueOutput(nameof(Title), (flow) => flow.GetValue<CMEvent>(CMEvent).Title);

            Description = ValueOutput(nameof(Description), (flow) => flow.GetValue<CMEvent>(CMEvent).Description);

            StartDateTime = ValueOutput(nameof(StartDateTime), (flow) => flow.GetValue<CMEvent>(CMEvent).StartDateTime);

            EndDateTime = ValueOutput(nameof(EndDateTime), (flow) => flow.GetValue<CMEvent>(CMEvent).EndDateTime);

            CategoryID = ValueOutput(nameof(CategoryID), (flow) => flow.GetValue<CMEvent>(CMEvent).Category.ID);

            CategoryName = ValueOutput(nameof(CategoryName), (flow) => flow.GetValue<CMEvent>(CMEvent).Category.Name);

            SubcategoryID = ValueOutput(nameof(SubcategoryID), (flow) => flow.GetValue<CMEvent>(CMEvent).SubCategory.ID);

            SubcategoryName = ValueOutput(nameof(SubcategoryName), (flow) => flow.GetValue<CMEvent>(CMEvent).SubCategory.Name);

            IsEventPublic = ValueOutput(nameof(IsEventPublic), (flow) => flow.GetValue<CMEvent>(CMEvent).IsPublic);

            IsEventStatic = ValueOutput(nameof(IsEventStatic), (flow) => flow.GetValue<CMEvent>(CMEvent).StaticEvent);
        }
    }
}