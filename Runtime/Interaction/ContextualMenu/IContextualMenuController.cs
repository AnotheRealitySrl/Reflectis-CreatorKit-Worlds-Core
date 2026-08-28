using System.Threading.Tasks;

namespace Virtuademy.CreatorKit.Worlds.Core.Interaction
{
    public interface IContextualMenuController
    {
        Task Hide();
        void Setup(IContextualMenuManageable manageable);
        Task Show(IContextualMenuManageable manageable);
        void Unsetup();
    }
}
