using Reflectis.CreatorKit.Core.SceneHierarchy;
using Reflectis.SDK.Core;

using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Scene: Enable Spawned Objects")]
    [UnitSurtitle("Scene")]
    [UnitShortTitle("Enable Spawned Objects")]
    [UnitCategory("Reflectis\\Flow")]
    public class EnableSpawnedObjectsNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Enable { get; private set; }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

        }

        protected override void Definition()
        {
            Enable = ValueInput<bool>(nameof(Enable));

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                SM.GetSystem<ISceneObjectsSystem>().EnableSpawnedObjects(f.GetValue<bool>(Enable), new List<GameObject>() { f.stack.AsReference().gameObject });

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
