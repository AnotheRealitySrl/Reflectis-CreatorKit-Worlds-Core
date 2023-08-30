using System;

using UnityEngine;

using Reflectis.SDK.Core;
using Reflectis.SDK.Avatars;
using Reflectis.SDK.Interaction;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateCharacterScriptableAction", fileName = "ActivateCharacterScriptableAction")]
public class CharacterActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;

    public override void Action(Action completedCallback)
    {
        SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(activate);
        completedCallback();
    }
}
