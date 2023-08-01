using System.Threading.Tasks;

using Virtuademy.Placeholders;

public interface IRuntimeComponent
{
    Task Init(SceneComponentPlaceholderBase placeholder);
}
