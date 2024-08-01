using Reflectis.SDK.ApplicationManagement;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
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
            PlaceholdersInChildren = ValueInput<bool>(nameof(PlaceholdersInChildren), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                SceneComponentPlaceholderBase[] placeHolders;

                // If the PlaceholdersInChildren is checked, gets all placeholder components 
                // on the target gameobject and its children. Else, it only gets the placeholder
                // on the target gameobject.
                if (f.GetValue<bool>(PlaceholdersInChildren))
                {
                    placeHolders = f.GetValue<GameObject>(Target).GetComponentsInChildren<SceneComponentPlaceholderBase>();
                }
                else
                {
                    placeHolders = new SceneComponentPlaceholderBase[1];
                    placeHolders[0] = f.GetValue<GameObject>(Target).GetComponent<SceneComponentPlaceholderBase>();
                }
                
                // Initializes all placeholder components found.
                foreach (var placeHolder in placeHolders)
                {
                    IReflectisApplicationManager.Instance.InitializeObject(placeHolder);
                }

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
