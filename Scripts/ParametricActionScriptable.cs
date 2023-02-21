using SPACS.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Virtuademy.Placeholders;

public class ParametricActionScriptable : ActionScriptable
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
