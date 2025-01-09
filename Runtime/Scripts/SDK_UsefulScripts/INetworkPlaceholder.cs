namespace Reflectis.CreatorKit.Core
{
    public interface INetworkPlaceholder
    {
        bool IsNetworked { get; set; }
        int InitializationId { get; set; }
    }
}
