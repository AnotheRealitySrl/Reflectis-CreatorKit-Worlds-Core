using Reflectis.SDK.CreatorKit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Descriptor(typeof(ChangeSceneNode))]
    public class ChangeSceneNodeDescriptor : UnitDescriptor<ChangeSceneNode>
    {
        public ChangeSceneNodeDescriptor(ChangeSceneNode unit) : base(unit) { }

        protected override string DefinedSummary()
        {
            return "This unit requires a coroutine flow to run properly. This unit will " +
                "load a static Reflectis event.";
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            switch (port.key)
            {
                case "SceneAddressableName":
                    description.summary = "The name of the environment scene used by the target static event.";
                    break;
            }
        }
    }
}
