using Reflectis.SDK.Core;
using Reflectis.SDK.NetworkingSystem;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Networking: On Other Player Entered")]
    [UnitSurtitle("Networking")]
    [UnitShortTitle("On Other Player Entered")]
    [UnitCategory("Events\\Reflectis")]
    public class OnOtherPlayerEnteredEventNode : EventUnit<(int, int)>
    {
        public static string eventName = "NetworkingOnOtherPlayerEntered";

        [DoNotSerialize]
        public ValueOutput UserId { get; private set; }
        [DoNotSerialize]
        public ValueOutput PlayerId { get; private set; }
        protected override bool register => true;

        protected GraphReference graphReference;

        public override EventHook GetHook(GraphReference reference)
        {
            graphReference = reference;

            SM.GetSystem<INetworkingSystem>().OtherPlayerJoinedRoom.AddListener(OnPlayerEntered);

            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            UserId = ValueOutput<int>(nameof(UserId));
            PlayerId = ValueOutput<int>(nameof(PlayerId));
        }

        protected override void AssignArguments(Flow flow, (int, int) args)
        {
            flow.SetValue(UserId, args.Item1);
            flow.SetValue(PlayerId, args.Item2);
        }

        private void OnPlayerEntered(int userId, int playerId)
        {
            Trigger(graphReference, (userId, playerId));
        }
    }
}
