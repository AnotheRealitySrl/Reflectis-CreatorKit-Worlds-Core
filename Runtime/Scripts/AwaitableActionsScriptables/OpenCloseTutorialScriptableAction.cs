using Reflectis.SDK.Core;
using Reflectis.SDK.Help;
using Reflectis.SDK.InteractionNew;
using System.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/CloseTutorialScriptableAction", fileName = "CloseTutorialScriptableAction")]
public class OpenCloseTutorialScriptableAction : AwaitableScriptableAction
{
    [SerializeField] private bool open;

    public override async Task Action(IInteractable interactable = null)
    {
        var helpSystem = SM.GetSystem<IHelpSystem>();
        if (open)
        {
            await helpSystem.CallGetHelp();
        }
        else
        {
            await helpSystem.CallCloseGetHelp();
        }

    }
}
