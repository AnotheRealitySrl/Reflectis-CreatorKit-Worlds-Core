using SPACS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Virtuademy.Placeholders;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ParametricScriptableAction", fileName = "ParametricScriptableAction")]
public class ParametricScriptableAction : ActionScriptable
{
    [SerializeField] private bool isChildObject;
    [SerializeField] private string goName;
    [SerializeField] private string methodName;

    public override void Action(Action completedCallback)
    {
        if (isChildObject)
        {
            InteractableObjectReference.transform.Find(goName).gameObject.SendMessage(methodName);
        }
        else
        {
            GameObject.Find(goName).SendMessage(methodName);
        }
    }
}
