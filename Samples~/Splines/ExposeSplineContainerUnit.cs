using Unity.VisualScripting;

using UnityEngine.Splines;

namespace Virtuademy.SDK.Environments.VisualScripting.Splines
{
    [UnitTitle("Virtuademy: Expose SplineContainer")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("SplineContainer")]
    [UnitCategory("Virtuademy\\Expose")]
    [RenamedFrom("ExposeSplineContainerUnit")]
    public class ExposeSplineContainerUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput ObjInput { get; private set; }

        [DoNotSerialize] public ValueOutput Splines { get; private set; }

        protected override void Definition()
        {
            ObjInput = ValueInput<SplineContainer>(nameof(ObjInput), null).NullMeansSelf();

            Splines = ValueOutput(nameof(Splines), (flow) => flow.GetValue<SplineContainer>(ObjInput).Splines);
        }
    }
}