using SPACS.Core;

using System;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ParametricScriptableAction", fileName = "ParametricScriptableAction")]
public class ParametricScriptableAction : ActionScriptable
{
    [SerializeField] private string goName;
    [SerializeField] private bool isChildObject;
    [SerializeField] private string methodName;

    public override void Action(Action completedCallback)
    {
        if (isChildObject)
        {
            Transform[] allChildren = InteractableObjectReference.GetComponentsInChildren<Transform>(true);
            allChildren.FirstOrDefault(x => x.name == goName)?.gameObject.SendMessage(methodName);
        }
        else
        {
            GameObject.Find(goName).SendMessage(methodName);
        }
    }
}
