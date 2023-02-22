using SPACS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Virtuademy.Placeholders;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ParametricScriptableAction", fileName = "ParametricScriptableAction")]
public class ParametricScriptableAction : ActionScriptable
{
    [SerializeField] private SceneComponentPlaceholderBase placeholderReference;
    [SerializeField] private string componentReference;
    [SerializeField] private string methodName;

    public override void Action(Action completedCallback)
    {
        Component component = placeholderReference.GetComponent(componentReference);
        component.gameObject.SendMessage(methodName);
    }
}
