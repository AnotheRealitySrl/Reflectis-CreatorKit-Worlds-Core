using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.Core;
using Virtuademy.Core.SceneManagement;
using Virtuademy.Placeholders;

public class TeleportPoint : MonoBehaviour, IRuntimeComponent
{
    private string sceneName;

    #region Interfaces implementation
    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        TeleportPointPlaceholder teleportPointPlaceholer = placeholder as TeleportPointPlaceholder;
        sceneName = teleportPointPlaceholer.SceneName;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        AppManager.Instance.LoadAddressableScene(sceneName);
    }
}
