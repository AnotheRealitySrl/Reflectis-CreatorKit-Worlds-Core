using Reflectis.SDK.ApplicationManagement;
using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using Reflectis.SDK.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Platform: Load Default Event")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Load Default Event")]
    [UnitCategory("Reflectis\\Flow")]
    public class LoadDefaultEventNode : AwaitableUnit
    {
        protected override async Task AwaitableAction(Flow flow)
        {
            await IReflectisApplicationManager.Instance.LoadDefaultEvent();
        }
    }
}
