using Virtuademy.SDK.Core.VisualScripting;
using Unity.VisualScripting;

using Virtuademy.ScriptingApi;

using System;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Localization: On Language Changed")]
    [UnitSurtitle("Localization")]
    [UnitShortTitle("On Language Changed")]
    [UnitCategory("Events\\Reflectis")]
    public class OnLanguageChangedEventUnit : ActionEventUnit<string, string>
    {

        public static string eventName = "OnLanguageChanged";
        //public static Dictionary<GraphReference, List<OnLanguageChangedEventUnit>> instances = new Dictionary<GraphReference, List<OnLanguageChangedEventUnit>>();

        protected override bool register => true;

        public ValueOutput CurrentLanguage { get; private set; }
        public ValueOutput CurrentLanguageCode { get; private set; }
        public ValueOutput PreviousLanguage { get; private set; }
        public ValueOutput PreviousLanguageCode { get; private set; }


        protected override void Definition()
        {
            base.Definition();

            CurrentLanguage = ValueOutput<string>(nameof(CurrentLanguage), (flow) => IVirtuademyFramework.Current.Localization.CurrentLanguage);
            CurrentLanguageCode = ValueOutput<string>(nameof(CurrentLanguageCode), (flow) => IVirtuademyFramework.Current.Localization.CurrentLanguageCode);
            PreviousLanguage = ValueOutput<string>(nameof(PreviousLanguage), (flow) => IVirtuademyFramework.Current.Localization.PreviousLanguage);
            PreviousLanguageCode = ValueOutput<string>(nameof(PreviousLanguageCode), (flow) => IVirtuademyFramework.Current.Localization.PreviousLanguageCode);
        }

        public override EventHook GetHook(GraphReference reference)
        {
            /*if (instances.TryGetValue(reference, out var value))
            {
                if (!value.Contains(this))
                {
                    value.Add(this);
                }
            }
            else
            {
                List<OnLanguageChangedEventUnit> variableList = new List<OnLanguageChangedEventUnit>
                {
                    this
                };

                instances.Add(reference, variableList);
            }*/

            //return new EventHook(eventName);
            return new EventHook("lANGUAGEChange" + this.ToString().Split("EventUnit")[0]);
        }

        protected override void Subscribe(Action<string> handler)
        {
            if (!IVirtuademyFramework.Current.Localization.IsAvailable)
            {
                return;
            }

            IVirtuademyFramework.Current.Localization.LanguageChanged += handler;
        }

        protected override void Unsubscribe(Action<string> handler)
        {
            IVirtuademyFramework.Current.Localization.LanguageChanged -= handler;
        }

        protected override string GetArguments(GraphReference reference, string data)
        {
            return "";
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            //instances.Remove(instance);
        }
    }
}
