using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reflectis.SDK.CreatorKit;

public class PickableContainerInitializationPlaceholder : SceneComponentPlaceholderBase
{
    public List<PickableInitializationEnv> EnvList { get; private set; }
    public List<PickablePlaceholder> DownloadedList { get; private set; }

    private void Awake()
    {
        EnvList = new List<PickableInitializationEnv>();
        DownloadedList = new List<PickablePlaceholder>();

        var childPickables = GetComponentsInChildren<PickableInitializationEnv>();

        foreach (var pickable in childPickables)
        {
            EnvList.Add(pickable);
        }
    }
}
