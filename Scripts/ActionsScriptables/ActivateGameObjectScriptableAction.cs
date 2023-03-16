using SPACS.Core;
using SPACS.SDK.Avatars;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateGameObjectScriptableAction", fileName = "ActivateGameObjectScriptableAction")]
public class ActivateGameObjectScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;
    [SerializeField] private string objectName;

    public override void Action(Action completedCallback)
    {
        InteractableObjectReference.transform.Find(objectName).gameObject.SetActive(activate);
    }
}
