using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class AddressablesConfigurationWindowNew : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Reflectis/AddressablesConfigurationWindowNew")]
    public static void ShowExample()
    {
        AddressablesConfigurationWindowNew wnd = GetWindow<AddressablesConfigurationWindowNew>();
        wnd.titleContent = new GUIContent("AddressablesConfigurationWindowNew");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);



    }
}
