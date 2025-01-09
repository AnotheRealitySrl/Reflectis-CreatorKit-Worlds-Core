using Reflectis.SDK.Core;
using Reflectis.SDK.Core.CharacterController;
using Reflectis.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Character: Pan")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Pan")]
    [UnitCategory("Reflectis\\Flow")]
    public class PanNode : AwaitableUnit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput TargetTransform { get; private set; }

        protected override void Definition()
        {
            TargetTransform = ValueInput<Transform>(nameof(TargetTransform));

            base.Definition();
        }

        protected override async Task AwaitableAction(Flow flow)
        {
            await SM.GetSystem<ICharacterControllerSystem>().GoToInteractState(flow.GetValue<Transform>(TargetTransform));
        }
    }
}
