using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Collect environments data")]
    [UnitSurtitle("Environments data")]
    [UnitShortTitle("Data")]
    [UnitCategory("Events\\Reflectis\\Data")]
    [TypeIcon(typeof(Material))]
    public class CollectEnvironmentsDataNode : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueOutput envID { get; private set; }
        [DoNotSerialize]
        public ValueOutput envName { get; private set; }
        [DoNotSerialize]
        public ValueOutput envDescription { get; private set; }
        [DoNotSerialize]
        public ValueOutput envAddressableKey { get; private set; }
        [DoNotSerialize]
        public ValueOutput envCatalog { get; private set; }

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


            envID = ValueOutput(nameof(envID), (flow) =>
            {
                return system.CurrentEvent.Environment.ID;
            });

            envName = ValueOutput(nameof(envName), (flow) =>
            {
                return system.CurrentEvent.Environment.Name;
            });

            envDescription = ValueOutput(nameof(envDescription), (flow) =>
            {
                return system.CurrentEvent.Environment.Description;
            });

            envAddressableKey = ValueOutput(nameof(envAddressableKey), (flow) =>
            {
                return system.CurrentEvent.Environment.AddressableKey;
            });

            envCatalog = ValueOutput(nameof(envCatalog), (flow) =>
            {
                return system.CurrentEvent.Environment.Catalog;
            });
        }
    }
}