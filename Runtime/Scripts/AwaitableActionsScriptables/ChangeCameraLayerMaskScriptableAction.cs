using Reflectis.SDK.InteractionNew;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/ChangeCameraLayerMaskScriptableAction", fileName = "ChangeCameraLayerMaskScriptableAction")]
public class ChangeCameraLayerMaskScriptableAction : AwaitableScriptableAction
{
    [SerializeField]
    private LayerMask layerMask;

    public override Task Action(IInteractable interactable)
    {
        Camera.main.cullingMask = layerMask;

        return Task.CompletedTask;
    }
}
