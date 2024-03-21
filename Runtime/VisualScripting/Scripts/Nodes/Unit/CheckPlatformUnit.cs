using Reflectis.SDK.Core;
using Reflectis.SDK.Platform;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Platform: Switch")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Switch")]
    [UnitCategory("Reflectis\\Flow")]
    public class CheckPlatformUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabel("VR")]
        public ControlOutput OutputTriggerVR { get; private set; }
        [DoNotSerialize]
        [PortLabel("WebGL")]
        public ControlOutput OutputTriggerWebGL { get; private set; }


        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                switch (SM.GetSystem<IPlatformSystem>().RuntimePlatform)
                {
                    case UnityEngine.RuntimePlatform.Android:
                        return OutputTriggerVR;
                    case UnityEngine.RuntimePlatform.WebGLPlayer:
                        return OutputTriggerWebGL;
                }
#if UNITY_WEBGL
                return OutputTriggerWebGL;
#endif
#if UNITY_ANDROID
                return OutputTriggerVR;
#endif
                return OutputTriggerWebGL;
            });

            OutputTriggerVR = ControlOutput(nameof(OutputTriggerVR));
            OutputTriggerWebGL = ControlOutput(nameof(OutputTriggerWebGL));

            Succession(InputTrigger, OutputTriggerVR);
            Succession(InputTrigger, OutputTriggerWebGL);
        }
    }
}
