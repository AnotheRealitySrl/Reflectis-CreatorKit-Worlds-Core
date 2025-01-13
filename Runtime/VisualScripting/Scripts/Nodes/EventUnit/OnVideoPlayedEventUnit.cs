using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Video: On Video Played")]
    [UnitSurtitle("BigScreen")]
    [UnitShortTitle("On Video Played")]
    [UnitCategory("Events\\Reflectis")]

    public class OnVideoPlayedEventUnit : EventUnit<BigScreenPlaceholder>
    {
        private GraphReference graphReference;
        private BigScreenPlaceholder bigScreenReference;

        protected override bool register => true;

        protected override void Definition()
        {
            base.Definition();
            //VideoPlayerReference = ValueInput<BigScreenPlaceholder>(nameof(videoPlayerReference), null).NullMeansSelf(); //this should always be self
        }

        public override EventHook GetHook(GraphReference reference)
        {
            graphReference = reference;
            RegisterToBigScreenPlay(reference.gameObject);

            return new EventHook("BigScreen" + this.ToString().Split("EventUnit")[0]);
        }

        private void RegisterToBigScreenPlay(GameObject gameObject)
        {
            bigScreenReference = gameObject.GetComponent<BigScreenPlaceholder>();
            bigScreenReference.onVideoPlayed.AddListener(OnVideoPlayed);
        }

        private void OnVideoPlayed()
        {
            Trigger(graphReference, bigScreenReference);
        }
    }
}
