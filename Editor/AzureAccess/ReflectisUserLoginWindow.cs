using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reflectis.CreatorKit.Worlds.CoreEditor
{
    // Assumi che TenantDataSO sia definito altrove e sia un ScriptableObject

    public class ReflectisUserLoginWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        // VARIABILE PERSISTENTE: Unity salverà e caricherà automaticamente il riferimento 
        // all'asset ScriptableObject tra le sessioni grazie a [SerializeField].
        [SerializeField] private TenantDataSO _selectedTenantDataSO;

        private RadioToggleGroup<TenantDataSO> _tenantToggleGroup;

        // Proprietà per accedere allo stato corrente (opzionale)
        public TenantDataSO SelectedTenantDataSO => _selectedTenantDataSO;

        [MenuItem("Reflectis Worlds/Creator Kit/Core/Login")]
        public static void ShowExample()
        {
            ReflectisUserLoginWindow wnd = GetWindow<ReflectisUserLoginWindow>();
            wnd.titleContent = new GUIContent("ReflectisLoginWindow");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            // 1. Inizializzazione della logica di gruppo
            _tenantToggleGroup = new RadioToggleGroup<TenantDataSO>();
            _tenantToggleGroup.OnSelectionChanged += OnTenantSelectionChanged;

            // Recupera tutti gli asset TenantDataSO
            // (La logica LINQ per il recupero è già efficiente)
            TenantDataSO[] tenantDataSOs = AssetDatabase.FindAssets("t:" + typeof(TenantDataSO).Name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TenantDataSO>)
                .Where(t => t != null)
                .ToArray();

            VisualElement selectedAppConfigSection = root.Q<VisualElement>("SelectedTenant");

            // 2. Popolamento della UI e del gruppo
            foreach (var tenantData in tenantDataSOs)
            {
                Toggle t = new Toggle
                {
                    text = tenantData.name
                };
                t.AddToClassList("toggle-item");

                // UTILIZZA LA VARIABILE SERIALIZZATA PER IMPOSTARE LO STATO INIZIALE PERSISTENTE
                bool isSelected = tenantData == _selectedTenantDataSO;

                _tenantToggleGroup.Add(t, tenantData, isSelected);
                selectedAppConfigSection.Add(t);
            }

            if (_selectedTenantDataSO != null)
            {
                Debug.Log($"Caricato stato persistente: {_selectedTenantDataSO.name}");
            }
        }

        private void OnTenantSelectionChanged(TenantDataSO newTenant)
        {
            // AGGIORNA LA VARIABILE SERIALIZZATA
            _selectedTenantDataSO = newTenant;

            if (newTenant != null)
            {
                Debug.Log($"Tenant Selezionato e Salvato: {newTenant.name}");
            }
            else
            {
                Debug.Log("Deselezionato.");
            }
        }

        private void OnDestroy()
        {
            if (_tenantToggleGroup != null)
            {
                // Rimuove l'iscrizione per prevenire memory leaks
                _tenantToggleGroup.OnSelectionChanged -= OnTenantSelectionChanged;
            }
        }
    }
}