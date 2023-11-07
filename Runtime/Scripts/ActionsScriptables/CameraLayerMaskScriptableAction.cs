using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Reflectis.SDK.Interaction;

using System;

using UnityEngine;

[CreateAssetMenu(menuName = "AnotheReality/Utilities/CameraLayerMaskScriptableAction", fileName = "CameraLayerMaskScriptableAction")]
public class CameraLayerMaskScriptableAction : ActionScriptable
{
    [SerializeField]
    private LayerMask layerMask;

    public override void Action(Action completedCallback)
    {
        Camera.main.cullingMask = layerMask;

        completedCallback?.Invoke();
    }
}
