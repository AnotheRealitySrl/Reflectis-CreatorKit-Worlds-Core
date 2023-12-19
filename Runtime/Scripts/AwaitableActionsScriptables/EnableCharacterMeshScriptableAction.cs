using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Reflectis.SDK.InteractionNew;

using System.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/EnableCharacterMeshScriptableAction", fileName = "EnableCharacterMeshScriptableAction")]
public class EnableCharacterMeshScriptableAction : AwaitableScriptableAction
{
    [SerializeField] private bool enable;

    public override Task Action(IInteractable interactable = null)
    {
        SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(enable);

        return Task.CompletedTask;
    }
}
