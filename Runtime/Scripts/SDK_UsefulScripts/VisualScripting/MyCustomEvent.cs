using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public static class EventNames
    {
        public static string MyCustomEvent = "MyCustomEvent";
    }

    [UnitTitle("On my Custom Event")]//The Custom Scripting Event node to receive the Event. Add "On" to the node title as an Event naming convention.
    [UnitCategory("Events\\MyEvents")]//Set the path to find the node in the fuzzy finder as Events > My Events.
    public class MyCustomEvent : EventUnit<int>
    {
        [DoNotSerialize]// No need to serialize ports.
        public ValueOutput result { get; private set; }// The Event output data to return when the Event is triggered.
        protected override bool register => true;

        // Add an EventHook with the name of the Event to the list of Visual Scripting Events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.MyCustomEvent);
        }
        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            result = ValueOutput<string>(nameof(result));
        }

        protected override void AssignArguments(Flow flow, int data)
        {
            flow.SetValue(result, data);
        }
    }
}
