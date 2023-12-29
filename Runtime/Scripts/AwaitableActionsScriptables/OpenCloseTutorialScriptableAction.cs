using Reflectis.SDK.Core;
using Reflectis.SDK.Help;
using Reflectis.SDK.InteractionNew;

using System.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/CloseTutorialScriptableAction", fileName = "CloseTutorialScriptableAction")]
public class OpenCloseTutorialScriptableAction : AwaitableScriptableAction
{
    [SerializeField] private bool open;

    public override Task Action(IInteractable interactable = null)
    {
        if (open)
        {
            SM.GetSystem<IHelpSystem>().CallGetHelp();
        }
        else
        {
            SM.GetSystem<IHelpSystem>().CallCloseGetHelp();
        }

        return Task.CompletedTask;
    }
}
