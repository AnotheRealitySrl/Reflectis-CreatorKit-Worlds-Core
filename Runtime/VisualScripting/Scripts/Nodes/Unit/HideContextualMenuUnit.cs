using Reflectis.SDK.Core;
using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Hide Contextual Menu")]
    [UnitSurtitle("ContextualMenu")]
    [UnitShortTitle("Hide")]
    [UnitCategory("Reflectis\\ContextualMenu")]

    public class HideContextualMenuUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        protected override void Definition()
        {
            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                SM.GetSystem<ContextualMenuSystem>().HideContextualMenu();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }


    }
}
