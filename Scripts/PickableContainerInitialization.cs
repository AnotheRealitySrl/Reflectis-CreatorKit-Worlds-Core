using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableContainerInitialization : MonoBehaviour
{
    public List<PickableInitializationEnv> EnvList { get; private set; }
    public List<PickableDownloaded> DownloadedList { get; private set; }

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
