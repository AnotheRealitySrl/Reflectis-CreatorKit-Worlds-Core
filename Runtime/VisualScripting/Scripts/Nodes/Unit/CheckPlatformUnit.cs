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
                    case ESupportedPlatform.VR:
                        return OutputTriggerVR;
                    case ESupportedPlatform.WebGL:
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

    //Better way to implement this, we should deprecate the old node and find a way to fix existing graphs before updateing
    //[UnitTitle("Reflectis Platform: Switch")]
    //[UnitSurtitle("Platform")]
    //[UnitShortTitle("Switch")]
    //[UnitCategory("Reflectis\\Flow")]
    //public class CheckPlatformUnit : Unit
    //{
    //    [DoNotSerialize]
    //    [PortLabelHidden]
    //    public ControlInput InputTrigger { get; private set; }

    //    [DoNotSerialize]
    //    public List<ControlOutput> Outputs { get; private set; }

    //    private Dictionary<ESupportedPlatform, ControlOutput> supportedPlatformsOutputs = new Dictionary<ESupportedPlatform, ControlOutput>();

    //    protected override void Definition()
    //    {

    //        InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
    //        {
    //            return supportedPlatformsOutputs[SM.GetSystem<IPlatformSystem>().RuntimePlatform];
    //        });

    //        Outputs = new List<ControlOutput>();

    //        foreach (ESupportedPlatform supportedPlatform in Enum.GetValues(typeof(ESupportedPlatform)))
    //        {
    //            ControlOutput output = ControlOutput(supportedPlatform.ToString());
    //            supportedPlatformsOutputs[supportedPlatform] = output;
    //            Succession(InputTrigger, output);
    //            Outputs.Add(output);
    //        }
    //    }
    //}
}
