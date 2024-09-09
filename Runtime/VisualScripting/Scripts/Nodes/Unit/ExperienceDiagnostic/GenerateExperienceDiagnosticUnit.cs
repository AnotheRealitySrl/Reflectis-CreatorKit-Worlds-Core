using Reflectis.SDK.Core;
using Reflectis.SDK.Diagnostics;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Diagnostic Experience: Generate GUID")]
    [UnitSurtitle("Reflectis Diagnostic Experience")]
    [UnitShortTitle("Generate GUID")]
    [UnitCategory("Reflectis\\Get")]
    public class GenerateExperienceDiagnosticUnit : AwaitableUnit
    {

        private const int MAX_KEY_LENGTH = 10;

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Key { get; private set; }

        protected override async Task AwaitableAction(Flow flow)
        {
            string desiredKey = flow.GetConvertedValue(Key) as string;

            if (string.IsNullOrEmpty(desiredKey) || desiredKey.Length > MAX_KEY_LENGTH)
            {
                throw new InvalidStringException("The key string must not be null or empty and it has to contain less than " + MAX_KEY_LENGTH + " characters!");
            }
            await SM.GetSystem<IDiagnosticsSystem>().GenerateExperienceGUID(desiredKey);
        }

        protected override void Definition()
        {
            base.Definition();

            Key = ValueInput(typeof(string), nameof(Key));
            Requirement(Key, InputTrigger);

        }


    }


    public class InvalidStringException : Exception
    {
        public InvalidStringException() { }
        public InvalidStringException(string message) : base(message)
        {

        }

        public InvalidStringException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
