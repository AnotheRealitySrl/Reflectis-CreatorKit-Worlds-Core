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
        if(SM.GetSystem<AvatarSystem>().ManageCounterAvatarMeshEnable(activate) == 0 && activate)
        {
            SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);

            completedCallback?.Invoke();
        }
        else if(SM.GetSystem<AvatarSystem>().ManageCounterAvatarMeshEnable(activate) == 1 && !activate)
        {
            SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);

            completedCallback?.Invoke();
        }
    }
}
