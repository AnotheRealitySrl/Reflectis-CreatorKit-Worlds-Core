using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Placeholders;

public class SpawnAddressablePrefabPlaceholder : SceneComponentPlaceholderBase
{
    [SerializeField]
    private string prefabAddressableName;
    [SerializeField]
    private Transform parent;

    public string PrefabAddressableName => prefabAddressableName;
    public Transform Parent => parent;
}
