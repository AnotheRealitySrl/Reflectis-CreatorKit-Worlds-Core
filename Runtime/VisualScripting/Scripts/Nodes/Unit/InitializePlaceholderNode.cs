using Reflectis.CreatorKit.Core;

using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Placeholder: Initialize Placeholder")]
    [UnitSurtitle("Placeholder")]
    [UnitShortTitle("Initialize Placeholder")]
    [UnitCategory("Reflectis\\Flow")]
    public class InitializePlaceholderNode : Unit
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
        [DoNotSerialize]
        [PortLabel("Placeholders in children")]
        public ValueInput PlaceholdersInChildren { get; private set; }

        protected override void Definition()
        {
            Target = ValueInput<GameObject>(nameof(Target), null).NullMeansSelf();
            PlaceholdersInChildren = ValueInput(nameof(PlaceholdersInChildren), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                IReflectisApplicationManager.Instance.InitializeObject(f.GetValue<GameObject>(Target), f.GetValue<bool>(PlaceholdersInChildren));

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
