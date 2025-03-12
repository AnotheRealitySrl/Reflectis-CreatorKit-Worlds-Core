using System.Threading.Tasks;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IInteractableBehaviour
    {
        IInteractable InteractableRef { get; }

        Task Setup();
    }
}
