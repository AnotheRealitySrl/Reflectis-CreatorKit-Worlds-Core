using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Placeholders;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(PickablePlaceholder)), RequireComponent(typeof(UsablePlaceholder))]
public class PickableInitializationEnv : MonoBehaviour
{
    private Dictionary<UsablePlaceholder.UseType, string> usableMapper;
    private string addressableNameToInstantiate;
    private InstantiatedCMTemplateObj instantiatedCMTemplateObj;

    public string AddressableNameToInstantiate => addressableNameToInstantiate;

    public InstantiatedCMTemplateObj InstantiatedCMTemplateObj { get => instantiatedCMTemplateObj; set => instantiatedCMTemplateObj = value; }

    private void Awake()
    {
        InitMapper();

        if (!usableMapper.TryGetValue(GetComponent<UsablePlaceholder>().UsableType, out addressableNameToInstantiate))
        {
            Debug.LogError("Missing the usable type on mapper");
        }
    }

    private void InitMapper()
    {
        usableMapper = new Dictionary<UsablePlaceholder.UseType, string>
        {
            { UsablePlaceholder.UseType.ChangeColor, "Download3DPickable" },
        };
    }
}
