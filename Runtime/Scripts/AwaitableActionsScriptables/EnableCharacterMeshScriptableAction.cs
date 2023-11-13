using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Reflectis.SDK.InteractionNew;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/EnableCharacterMeshScriptableAction", fileName = "EnableCharacterMeshScriptableAction")]
public class EnableCharacterMeshScriptableAction : AwaitableScriptableAction
{
    [SerializeField] private bool enable;

    public override Task Action(IInteractable interactable)
    {
        var count = SM.GetSystem<AvatarSystem>().ManageCounterAvatarMeshEnable(enable);

        if (count == 0 && enable)
        {
            SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(enable);
        }
        else if (count == 1 && !enable)
        {
            SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(enable);
        }

        return Task.CompletedTask;
    }
}
