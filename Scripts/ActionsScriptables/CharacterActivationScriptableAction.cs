using SPACS.Core;
using SPACS.SDK.Avatars;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;
    [SerializeField] private float activationTime;

    public override void Action(Action completedCallback)
    {
        SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate, activationTime);
    }
}
