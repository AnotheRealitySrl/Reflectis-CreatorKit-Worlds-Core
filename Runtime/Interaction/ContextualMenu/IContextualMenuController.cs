using System.Threading.Tasks;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IContextualMenuController
    {
        Task Hide();
        void Setup(IContextualMenuManageable manageable);
        Task Show();
        void Unsetup();
    }
}
