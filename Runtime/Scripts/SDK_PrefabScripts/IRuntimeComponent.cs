using System.Threading.Tasks;

using Reflectis.SDK.CreatorKit;

public interface IRuntimeComponent
{
    Task Init(SceneComponentPlaceholderBase placeholder);
}
