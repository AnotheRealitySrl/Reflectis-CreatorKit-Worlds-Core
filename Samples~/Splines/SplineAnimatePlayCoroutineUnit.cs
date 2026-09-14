
using System.Threading.Tasks;

using Unity.Mathematics;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Splines;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting.Splines
{
    [UnitTitle("Reflectis SplineAnimate: Play Coroutine")]
    [UnitSurtitle("SplineAnimate")]
    [UnitShortTitle("Play Coroutine")]
    [UnitCategory("Reflectis\\Flow")]
    public class SplineAnimatePlayCoroutineUnit : AwaitableUnit
    {

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput SplineAnimateInput { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabel("Move Backwards")]
        public ValueInput MoveBackwardsInput { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabel("Freeze")]
        public ValueInput FreezeInput { get; private set; }

        protected override void Definition()
        {
            SplineAnimateInput = ValueInput<SplineAnimate>(nameof(SplineAnimateInput), null).NullMeansSelf();
            MoveBackwardsInput = ValueInput<bool>(nameof(MoveBackwardsInput));
            FreezeInput = ValueInput<bool>(nameof(FreezeInput));

            base.Definition();
        }

        protected override async Task AwaitableAction(Flow flow)
        {
            SplineAnimate sa = flow.GetValue<SplineAnimate>(SplineAnimateInput);
            bool mb = flow.GetValue<bool>(MoveBackwardsInput);

            float playTime = 0f;

            float3 pos, tan, up;
            while (playTime <= sa.Duration)
            {
                if (!flow.GetValue<bool>(FreezeInput))
                {
                    if (mb)
                    {
                        sa.Container.Evaluate((sa.Duration - playTime) / sa.Duration, out pos, out tan, out up);
                    }
                    else
                    {
                        sa.Container.Evaluate(playTime / sa.Duration, out pos, out tan, out up);
                    }

                    sa.transform.position = pos;
                    sa.transform.LookAt(pos + tan, up);

                    playTime += Time.deltaTime;
                }

                await Task.Yield();
            }
            if (mb)
            {
                sa.Container.Evaluate(0f, out pos, out tan, out up);
            }
            else
            {
                sa.Container.Evaluate(1f, out pos, out tan, out up);
            }

            sa.transform.position = pos;
            sa.transform.LookAt(pos + tan, up);
        }
    }
}