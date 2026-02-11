using Microsoft.Identity.Client;
using Reflectis.CreatorKit.Worlds.Core.Editor;
using Reflectis.CreatorKit.Worlds.CoreEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class AzureAuthService
{
    private static IPublicClientApplication _pca;

    public static void Init(string clientId, string tenant, string policy)
    {
        if (_pca != null) return;

        string authority = $"https://{tenant}.b2clogin.com/tfp/{tenant}.onmicrosoft.com/{policy}";

        _pca = PublicClientApplicationBuilder
            .Create(clientId)
            .WithB2CAuthority(authority)
            .WithRedirectUri("http://localhost:10717/")
            .Build();
    }

    public static async Task<string> LoginInteractive(string[] scopes)
    {
        AuthenticationResult result = null;
        try
        {
            try
            {
                // Prova a usare un token esistente (cache + refresh token)
                var accounts = await _pca.GetAccountsAsync();
                result = await _pca.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
                Debug.Log("Token ottenuto in modo silenzioso (refresh riuscito)");
            }
            catch (MsalUiRequiredException)
            {
                // 🔸 Nessun token valido → login interattivo
                result = await _pca.AcquireTokenInteractive(scopes)
                .WithUseEmbeddedWebView(false)
                .WithExtraQueryParameters(new Dictionary<string, string>{
                        {"nonce", Guid.NewGuid().ToString()}
                    })
                .ExecuteAsync();
                Debug.Log("Login interattivo completato");
            }
        }
        catch (MsalException ex)
        {
            Debug.LogError($"MSAL Error: {ex.Message}");
            throw;
        }

        // Controlla se il token sta per scadere
        if (result.ExpiresOn <= DateTimeOffset.Now.AddMinutes(5))
        {
            Console.WriteLine("Token quasi scaduto, rinnovo in corso...");
            result = await _pca.AcquireTokenSilent(scopes, result.Account).ExecuteAsync();
            Console.WriteLine("Token rinnovato automaticamente!");
        }

        Debug.Log($"Access token: {result.AccessToken}");
        return result.AccessToken;
    }

    [MenuItem("Tools/Test Azure Login")]
    private static async void TestAzureLogin()
    {
        string[] tenantDataSOsPaths = AssetDatabase.FindAssets("t:" + typeof(TenantDataSO).Name).ToArray();

        TenantDataSO[] tenantDataSOs = new TenantDataSO[tenantDataSOsPaths.Length];

        foreach (var (path, index) in tenantDataSOsPaths.Select((value, i) => (value, i)))
        {
            tenantDataSOs[index] = AssetDatabase.LoadAssetAtPath<TenantDataSO>(AssetDatabase.GUIDToAssetPath(path));
        }

        foreach (var t in tenantDataSOs)
        {
            Debug.Log($"Trovato TenantDataSO: {t.name} - ClientId: {t.ClientId}, Tenant: {t.Tenant}, Policy: {t.Policy}, ProfileApi: {t.ProfileApi}, ProfileApiId: {t.ProfileApiId}");
        }

        TenantDataSO tenantData = tenantDataSOs[0];
        string clientId = tenantData.ClientId; // Unity Application ID
        string tenant = tenantData.Tenant;        // tenant name
        TenantAuthenticationPolicy policy = tenantData.Policy;          // es: B2C_1_signin
        string[] scopes = {
          "openid",
          "offline_access",
          "https://" + tenant + ".onmicrosoft.com/" + tenantData.ProfileApiId + "/access",
         };
        Debug.Log($"scopes: {Newtonsoft.Json.JsonConvert.SerializeObject(scopes)}");
        Init(clientId, tenant, policy.ToString());
        string token = await LoginInteractive(scopes);

        Debug.Log($"Token ricevuto {token}");

        Debug.Log("==> Chiamata API protetta...");
        string userData = await GetUserDataAsync(tenantData.ProfileApi, token);
        Debug.Log($"Dati utente ricevuti: {userData}");
        JwtToken[] tokens = Newtonsoft.Json.JsonConvert.DeserializeObject<JwtToken[]>(userData);
        AddressablesManagementWindow.token = tokens.FirstOrDefault((x => x.ApiLabel == tenantData.ApiLabel)).Bearer;
    }

    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<string> GetUserDataAsync(string apiBaseUrl, string accessToken)
    {
        // URL della tua API protetta
        string apiUrl = $"{apiBaseUrl}my/tokens";

        // Imposta l’header di autorizzazione
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        Debug.Log($"API Response: {json}");
        return json;
    }

    [Serializable, Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class JwtToken
    {
        [SerializeField] private string bearer;
        [SerializeField] private DateTime expiry;
        [SerializeField] private string apiLabel;
        [SerializeField] private List<string> apiUrls;

        public string Bearer { get => bearer; set => bearer = value; }
        public DateTime Expiry { get => expiry.ToUniversalTime(); }
        public string ApiLabel { get => apiLabel; set => apiLabel = value; }
        public IEnumerable<Uri> ApiUrls { get => apiUrls.Select(x => new Uri(x)); set => apiUrls = value.Select(x => x.ToString()).ToList(); }

        public bool IsExpired() => DateTime.UtcNow > expiry;
        public bool IsExpired(TimeSpan timeOffset) => DateTime.UtcNow - timeOffset > expiry;
    }
}
