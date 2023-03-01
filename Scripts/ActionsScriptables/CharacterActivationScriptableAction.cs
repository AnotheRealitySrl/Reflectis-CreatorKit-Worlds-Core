using SPACS.Core;
using SPACS.SDK.Avatars;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateCharacterScriptableAction", fileName = "ActivateCharacterScriptableAction")]
public class CharacterActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;

    public override void Action(Action completedCallback)
    {
        SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);
    }
}
