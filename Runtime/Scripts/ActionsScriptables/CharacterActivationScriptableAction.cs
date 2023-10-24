using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Reflectis.SDK.Interaction;

using System;

using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateCharacterScriptableAction", fileName = "ActivateCharacterScriptableAction")]
public class CharacterActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;

    public override void Action(Action completedCallback)
    {
        var count = SM.GetSystem<AvatarSystem>().ManageCounterAvatarMeshEnable(activate);

        if (count == 0 && activate)
        {
            SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);
        }
        else if(count == 1 && !activate)
        {
            SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);
        }

        completedCallback?.Invoke();
    }
}
