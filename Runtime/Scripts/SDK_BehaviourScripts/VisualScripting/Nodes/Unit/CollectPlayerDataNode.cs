using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Player: Get Player")]
    [UnitSurtitle("Player")]
    [UnitShortTitle("Get Player")]
    [UnitCategory("ReflectisUnit")]
    public class CollectPlayerDataNode : Unit
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
        public ValueOutput EMail { get; private set; }
        [DoNotSerialize]
        public ValueOutput Roles { get; private set; }

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
                return system.UserData.ID;
            });

            Name = ValueOutput(nameof(Name), (flow) =>
            {
                return system.UserData.DisplayName;
            });

            EMail = ValueOutput(nameof(EMail), (flow) =>
            {
                return system.UserData.Email;
            });

            Roles = ValueOutput(nameof(Roles), (flow) =>
            {
                List<string> roles = new List<string>();
                foreach (var role in system.UserData.Tags)
                    roles.Add(role.Label);

                return roles;
            });
        }
    }
}
