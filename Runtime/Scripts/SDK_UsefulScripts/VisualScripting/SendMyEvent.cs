using CodiceApp.EventTracking.Plastic;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Send My Custom Event")]
    [UnitCategory("Events\\MyEvents")]//Setting the path to find the node in the fuzzy finder as Events > My Events.
    public class SendMyEvent : Unit
    {
        [DoNotSerialize]// Mandatory attribute, to make sure we don’t serialize data that should never be serialized.
        [PortLabelHidden]// Hide the port label, as we normally hide the label for default Input and Output triggers.
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        public ValueInput myValue;
        [DoNotSerialize]
        [PortLabelHidden]// Hide the port label, as we normally hide the label for default Input and Output triggers.
        public ControlOutput outputTrigger { get; private set; }

        protected override void Definition()
        {

            inputTrigger = ControlInput(nameof(inputTrigger), Trigger);
            myValue = ValueInput<string>(nameof(myValue), "");
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        //Send the Event MyCustomEvent with the integer value from the ValueInput port myValueA.
        private ControlOutput Trigger(Flow flow)
        {
            EventBus.Trigger(EventNames.MyCustomEvent, flow.GetValue<string>(myValue));
            return outputTrigger;
        }
    }
}
