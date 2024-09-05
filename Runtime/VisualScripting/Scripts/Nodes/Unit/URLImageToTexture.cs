using Reflectis.SDK.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis image: Set sprite from URL")]
    [UnitSurtitle("Image")]
    [UnitShortTitle("Set sprite from URL")]
    [UnitCategory("Reflectis\\Flow")]
    public class URLImageToTexture : AwaitableUnit
    {

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput ImageURL { get; private set; }

        [NullMeansSelf]

        public ValueInput ImageValue { get; private set; }

        private List<Flow> runningFlows = new List<Flow>();

        private Sprite sprite;

        //Convert to coroutine
        protected override void Definition()
        {
            ImageURL = ValueInput<string>(nameof(ImageURL), null);
            ImageValue = ValueInput<Image>(nameof(ImageValue), null).NullMeansSelf();

            base.Definition();
        }

        protected async override Task AwaitableAction(Flow flow)
        {
            ImageDownloader.DownloadImage(flow.GetValue<string>(ImageURL), (tex) =>
            {
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                flow.GetValue<Image>(ImageValue).sprite = sprite;
                runningFlows.Remove(flow);
            });
        }
    }
}
