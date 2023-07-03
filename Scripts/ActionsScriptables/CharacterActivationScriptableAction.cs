using System;

using UnityEngine;

using SPACS.SDK.Core;
using SPACS.SDK.Avatars;
using SPACS.SDK.Interaction;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateCharacterScriptableAction", fileName = "ActivateCharacterScriptableAction")]
public class CharacterActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;

    public override void Action(Action completedCallback)
    {
        SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);
    }
}
