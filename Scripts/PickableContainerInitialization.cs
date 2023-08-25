using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Core;
using Virtuademy.Placeholders;

public class PickableContainerInitialization : MonoBehaviour
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

    private void Start()
    {
        MapCallBack();   
    }

    private void MapCallBack()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            var pickableContInit = GetComponent<PickableContainerInitialization>();

            var list = FindObjectsOfType<PickableDownloaded>();

            for (var i = 0; i < EnvList.Count; i++)
            {
                var pickableEnvInit = EnvList[i];
                AppManager.Instance.MapOtherPickablesClient(list[i], pickableEnvInit);
            }
        }
    }
}
