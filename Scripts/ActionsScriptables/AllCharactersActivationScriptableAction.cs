using SPACS.Core;
using SPACS.SDK.Avatars;
using SPACS.SDK.RPMAvatars;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
