using UnityEngine;

public class PickableInitializationEnv : MonoBehaviour
{
    [SerializeField] string addressableNameToInstantiate;

    public string AddressableNameToInstantiate => addressableNameToInstantiate;
}
