using SPACS.Core;
using SPACS.SDK.Avatars;
using SPACS.SDK.Transitions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateGameObjectScriptableAction", fileName = "ActivateGameObjectScriptableAction")]
public class ActivateGameObjectScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;
    [SerializeField] private string objectName;

    public override void Action(Action completedCallback)
    {
        Transform[] allChildren = InteractableObjectReference.GetComponentsInChildren<Transform>(true);
        GameObject targetObject = allChildren.FirstOrDefault(x => x.name == objectName)?.gameObject;
        if (targetObject && targetObject.GetComponent<AbstractTransitionProvider>() is AbstractTransitionProvider transitionProvider)
        {
            transitionProvider.DoTransition(activate);
        }
        else
        {
            targetObject.SetActive(activate);
        }
    }
}
