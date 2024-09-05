using Reflectis.SDK.ClientModels;
using Reflectis.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis CMUser: Get CMUserByID")]
    [UnitSurtitle("CMUserByID")]
    [UnitShortTitle("Get CMUserByID")]
    [UnitCategory("Reflectis\\Get")]
    public class GetCMUserByIDNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput CMUser { get; private set; }

        public ValueInput UserID { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        private List<Flow> runningFlows = new List<Flow>();

        private CMUser cmUserData;

        protected override void Definition()
        {
            UserID = ValueInput<int>(nameof(UserID));
            CMUser = ValueOutput<CMUser>(nameof(CMUser), f => cmUserData);
            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), CMUserCoroutine);

            OutputTrigger = ControlOutput(nameof(OutputTrigger));
            Succession(InputTrigger, OutputTrigger);
        }

        private IEnumerator CMUserCoroutine(Flow flow)
        {
            runningFlows.Add(flow);

            CallAwaitableMethod(flow);

            yield return new WaitUntil(() => !runningFlows.Contains(flow));

            yield return OutputTrigger;
        }
        private async void CallAwaitableMethod(Flow flow)
        {         
            cmUserData = await SM.GetSystem<IClientModelSystem>().GetUserData(flow.GetValue<int>(UserID));
            runningFlows.Remove(flow);
        }

    }
}
