using Reflectis.SDK.Core;
using Reflectis.SDK.Core.NetworkingSystem;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Network: IsMaster")]
    [UnitSurtitle("Reflectis Network")]
    [UnitShortTitle("Is Master")]
    [UnitCategory("Reflectis\\Flow")]
    public class SwitchNetworkMasterUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }

        [DoNotSerialize]
        public ControlOutput True { get; private set; }
        [DoNotSerialize]
        public ControlOutput False { get; private set; }


        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                var networkSystem = SM.GetSystem<INetworkingSystem>();
                if (networkSystem != null && networkSystem.IsMasterClient)
                {
                    return True;
                }
                else
                {
                    return False;
                }
            });

            True = ControlOutput(nameof(True));
            False = ControlOutput(nameof(False));

            Succession(InputTrigger, True);
            Succession(InputTrigger, False);
        }
    }
}
