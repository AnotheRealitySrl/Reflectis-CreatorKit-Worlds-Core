using SPACS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Virtuademy.Placeholders;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ParametricScriptableAction", fileName = "ParametricScriptableAction")]
public class ParametricScriptableAction : ActionScriptable
{
    [SerializeField] private string goName;
    [SerializeField] private string componentReference;
    [SerializeField] private string methodName;

    public override void Action(Action completedCallback)
    {
        GameObject.Find(goName).SendMessage(methodName);
    }
}
