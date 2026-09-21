using Unity.VisualScripting;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Splines;

namespace Virtuademy.SDK.Environments.VisualScripting.Splines
{
    [UnitTitle("Virtuademy Spline: Get SplineAnimate")]
    [UnitSurtitle("Spline")]
    [UnitShortTitle("Get SplineAnimate")]
    [UnitCategory("Virtuademy\\Get")]

    [RenamedFrom("GetSplineAnimateUnit")]

    public class GetSplineAnimateUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput ObjInput { get; private set; }

        [DoNotSerialize]
        public ValueOutput SplineAnimate { get; private set; }

        protected override void Definition()
        {
            ObjInput = ValueInput<GameObject>(nameof(ObjInput), null).NullMeansSelf();

            SplineAnimate = ValueOutput(nameof(SplineAnimate), (flow) => flow.GetValue<GameObject>(ObjInput).GetComponent<SplineAnimate>());
        }
    }
}