using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Collect player data")]
    [UnitSurtitle("Player data")]
    [UnitShortTitle("Data")]
    [UnitCategory("Events\\Reflectis\\Data")]
    [TypeIcon(typeof(Material))]
    public class CollectPlayerDataNode : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueOutput playerID { get; private set; }
        [DoNotSerialize]
        public ValueOutput playerName { get; private set; }
        [DoNotSerialize]
        public ValueOutput playerMail { get; private set; }
        [DoNotSerialize]
        public ValueOutput playerRoles { get; private set; }

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


            playerID = ValueOutput(nameof(playerID), (flow) =>
            {
                return system.UserData.ID;
            });

            playerName = ValueOutput(nameof(playerName), (flow) =>
            {
                return system.UserData.DisplayName;
            });

            playerMail = ValueOutput(nameof(playerMail), (flow) =>
            {
                return system.UserData.Email;
            });

            playerRoles = ValueOutput(nameof(playerRoles), (flow) =>
            {
                List<string> roles = new List<string>();
                foreach (var role in system.UserData.Tags)
                    roles.Add(role.Label);

                return roles;
            });
        }
    }
}
