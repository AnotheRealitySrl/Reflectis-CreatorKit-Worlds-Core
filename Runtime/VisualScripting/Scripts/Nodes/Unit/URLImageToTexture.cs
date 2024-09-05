using Reflectis.SDK.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Apply Image URL To Sprite")]
    [UnitSurtitle("Image")]
    [UnitShortTitle("URL To Image")]
    [UnitCategory("Reflectis\\Flow")]
    public class URLImageToTexture : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput ImageURL { get; private set; }

        [NullMeansSelf]

        public ValueInput ImageValue { get; private set; }

        protected override void Definition()
        {
            ImageURL = ValueInput<string>(nameof(ImageURL), null);
            ImageValue = ValueInput<Image>(nameof(ImageValue), null).NullMeansSelf();

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                ImageDownloader.DownloadImage(f.GetValue<string>(ImageURL), (tex) =>
                {
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                    f.GetValue<Image>(ImageValue).sprite = sprite;
                });

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));
            Succession(InputTrigger, OutputTrigger);
        }
    }
}
