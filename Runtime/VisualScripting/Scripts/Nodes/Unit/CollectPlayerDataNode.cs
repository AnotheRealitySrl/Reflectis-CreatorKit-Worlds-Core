using Reflectis.SDK.ClientModels;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Expose: CMUser")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("CMUser")]
    [UnitCategory("Reflectis\\Expose")]
    public class CollectPlayerDataNode : Unit
    {
        [DoNotSerialize]
        public ValueInput CMUser { get; private set; }
        [DoNotSerialize]
        public ValueOutput ID { get; private set; }
        [DoNotSerialize]
        public ValueOutput Name { get; private set; }
        [DoNotSerialize]
        public ValueOutput EMail { get; private set; }
        [DoNotSerialize]
        public ValueOutput Roles { get; private set; }

        [DoNotSerialize]
        public ValueOutput ProfileImageURL { get; private set; }

        protected override void Definition()
        {
            CMUser = ValueInput<CMUser>(nameof(CMUser), null).NullMeansSelf();

            ID = ValueOutput(nameof(ID), (flow) => flow.GetValue<CMUser>(CMUser).ID);

            Name = ValueOutput(nameof(Name), (flow) => flow.GetValue<CMUser>(CMUser).DisplayName);

            EMail = ValueOutput(nameof(EMail), (flow) => flow.GetValue<CMUser>(CMUser).Email);

            ProfileImageURL = ValueOutput(nameof(ProfileImageURL), (flow) => flow.GetValue<CMUser>(CMUser).Preferences.AvatarConfig.AvatarPng);

            Roles = ValueOutput(nameof(Roles), (flow) =>
            {
                List<string> roles = new List<string>();
                foreach (var role in flow.GetValue<CMUser>(CMUser).Tags)
                    roles.Add(role.Label);

                return roles;
            });
        }
    }
}
