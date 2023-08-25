using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Placeholders;

public class PickableContainerInitializationPlaceholder : SceneComponentPlaceholderNetwork

{
    public List<PickableInitializationEnv> EnvList { get; private set; }

    private void Awake()
    {
        EnvList = new List<PickableInitializationEnv>();

        var childPickables = GetComponentsInChildren<PickableInitializationEnv>();

        foreach(var pickable in childPickables)
        {
            EnvList.Add(pickable);
        }
    }
}
