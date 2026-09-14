
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Player Save Data: Get Data")]
    [UnitSurtitle("Player Save Data")]
    [UnitShortTitle("Get Player Save Data")]
    [UnitCategory("Reflectis\\Get")]
    public class GetPlayerSaveDataUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Key { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput Data { get; private set; }

        protected override void Definition()
        {
            Key = ValueInput(nameof(Key), string.Empty);

            Data = ValueOutput(nameof(Data),
                (f) =>
            {
                return VirtuademyFramework.Current.GetMySaveData(f.GetValue<string>(Key));
            });
        }
    }
}
