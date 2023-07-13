using System;

using UnityEngine;

using Reflectis.SDK.Interaction;
using Reflectis.SDK.Avatars;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/AllCharactersActivationScriptableAction", fileName = "AllCharactersActivationScriptableAction")]
public class AllCharactersActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;

    public override void Action(Action completedCallback)
    {
        foreach (var avatarConfigVR in FindObjectsOfType<AvatarConfigControllerBase>())
        {
            avatarConfigVR.EnableAvatarMeshes(activate);
        }
    }
}
