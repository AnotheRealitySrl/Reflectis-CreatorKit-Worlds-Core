using Reflectis.SDK.Interaction;
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/ActivateDashboardBlock", fileName = "ActivateDashboardBlockScriptableAction")]
public class ActivateDashboardBlockScriptableAction : ActionScriptable
{
    [SerializeField] private bool activate;
    [SerializeField] private string objectName;
    [SerializeField] private GameObject block2D;

    public override void Action(Action completedCallback)
    {
        Transform[] allChildren = InteractableObjectReference.GetComponentsInChildren<Transform>(true);
        block2D = allChildren.FirstOrDefault(x => x.name == objectName)?.gameObject;

        block2D.SetActive(activate);
    }
}


