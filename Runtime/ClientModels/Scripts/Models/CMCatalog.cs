using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    public class CMCatalog
    {
        [SerializeField] private string baseUrl;
        [SerializeField] private string catalogName;
        [SerializeField] private string catalogFullName;

        public string BaseUrl { get => baseUrl; set => baseUrl = value; }
        public string CatalogName { get => catalogName; set => catalogName = value; }
        public string CatalogFullName { get => catalogFullName; set => catalogFullName = value; }
    }
}
