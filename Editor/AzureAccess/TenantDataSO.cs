using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.CoreEditor
{
    [CreateAssetMenu(fileName = "TenantDataSO", menuName = "Reflectis/TenantDataSO")]
    public class TenantDataSO : ScriptableObject
    {
        [SerializeField]
        private string clientId = "2b066105-bc17-4242-ad1b-f6175ca71d6f"; // Unity Application ID
        [SerializeField]
        private string tenant = "spacsidentity";        // tenant name
        [SerializeField]
        private TenantAuthenticationPolicy policy;          // es: B2C_1_signin
        [SerializeField]
        private string profileApi = "https://profile.spacs.it/";
        [SerializeField]
        private string profileApiId = "F92D927F-2084-4477-8FCF-77447C96D17A";
        [SerializeField]
        private string apiLabel = "Reflectis2023-PREP";

        public string ClientId => clientId;
        public string Tenant => tenant;
        public TenantAuthenticationPolicy Policy => policy;
        public string ProfileApi => profileApi;
        public string ProfileApiId => profileApiId;
        public string ApiLabel => apiLabel;
    }

    public enum TenantAuthenticationPolicy
    {
        B2C_1_local,
        EntraId
    }
}
