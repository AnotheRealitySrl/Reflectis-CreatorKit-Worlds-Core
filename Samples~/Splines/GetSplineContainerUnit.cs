using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Splines;

namespace Virtuademy.SDK.Environments.VisualScripting.Splines
{
    [UnitTitle("Virtuademy Spline: Get SplineContainer")]
    [UnitSurtitle("Spline")]
    [UnitShortTitle("Get SplineContainer")]
    [UnitCategory("Virtuademy\\Get")]

    public class GetSplineContainerUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput ObjInput { get; private set; }

        [DoNotSerialize]
        public ValueOutput SplineContainer { get; private set; }

        protected override void Definition()
        {
            ObjInput = ValueInput<GameObject>(nameof(ObjInput), null).NullMeansSelf();

            SplineContainer = ValueOutput(nameof(SplineContainer), (flow) => flow.GetValue<GameObject>(ObjInput).GetComponent<SplineContainer>());
        }
    }
}