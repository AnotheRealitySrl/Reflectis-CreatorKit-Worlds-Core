using Reflectis.SDK.CreatorKit;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(OnOtherPlayerLeftEventNode))]
    public class OnOtherPlayerLeftEventDescriptor : UnitDescriptor<OnOtherPlayerLeftEventNode>
    {
        public OnOtherPlayerLeftEventDescriptor(OnOtherPlayerLeftEventNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This event will be triggered when a player leaves the Reflectis event where the local " +
                "player currently is.\n" +
                "It won't be triggered by the local player upon leaving a Reflectis event.\n\n" +
                "UserId is the unique identifier for the player's Reflectis profile.\n" +
                "ActorNumber is the Photon actor number value assigned to that player " +
                "for the current room.";
        }
    }
}
