namespace Reflectis.SDK.CreatorKit
{
    public interface INetworkPlaceholder
    {
        bool IsNetworked { get; set; }
        int InitializationId { get; set; }
    }
}
