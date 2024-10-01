using Reflectis.SDK.CreatorKit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(OnOtherPlayerEnteredEventNode))]
    public class OnOtherPlayerEnteredEventDescriptor : UnitDescriptor<OnOtherPlayerEnteredEventNode>
    {
        public OnOtherPlayerEnteredEventDescriptor(OnOtherPlayerEnteredEventNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This event will be triggered when a player enters the Reflectis event where the local " +
                "player currently is.\n" +
                "It won't be triggered by the local player upon entering a Reflectis event.\n\n" +
                "UserId is the unique identifier for the player's Reflectis profile.\n" +
                "ActorNumber is the Photon actor number value assigned to that player " +
                "for the current room.";
        }
    }
}
