using System;

using UnityEngine;

using Reflectis.SDK.Interaction;
using Reflectis.SDK.RPMAvatars;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/AllCharactersActivationScriptableAction", fileName = "AllCharactersActivationScriptableAction")]
public class AllCharactersActivationScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;

    public override void Action(Action completedCallback)
    {
        foreach (var avatarConfigVR in FindObjectsOfType<RPMAvatarConfigManagerVR>())
        {
            avatarConfigVR.EnableAvatarMeshes(activate);
        }

        foreach (var avatarConfigDesktop in FindObjectsOfType<RPMAvatarConfigManagerDesktop>())
        {
            avatarConfigDesktop.EnableAvatarMeshes(activate);
        }
    }
}
