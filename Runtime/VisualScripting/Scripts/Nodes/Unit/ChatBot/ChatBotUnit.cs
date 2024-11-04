using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis ChatBot: Start Conversation")]
    [UnitSurtitle("ChatBot")]
    [UnitShortTitle("Start Conversation")]
    [UnitCategory("Reflectis\\Flow")]
    public class ChatBotUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [DoNotSerialize]
        [NullMeansSelf]
        [Serialize]
        [AllowsNull]
        [PortLabel("Target")]
        public ValueInput Target { get; private set; }


        protected override void Definition()
        {
            Target = ValueInput<GameObject>(nameof(Target), null).NullMeansSelf();

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                f.GetValue<GameObject>(Target).GetComponent<ChatBotPlaceholder>().OnConversationStart?.Invoke();

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }

    }
}
