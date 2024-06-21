using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Video: On Video Paused")]
    [UnitSurtitle("BigScreen")]
    [UnitShortTitle("On Video Paused")]
    [UnitCategory("Events\\Reflectis")]
    public class OnVideoPausedEventUnit : EventUnit<BigScreenPlaceholder>
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
            bigScreenReference.onVideoPaused.AddListener(OnVideoPaused);
        }

        private void OnVideoPaused()
        {
            Trigger(graphReference, bigScreenReference);
        }
    }
}
