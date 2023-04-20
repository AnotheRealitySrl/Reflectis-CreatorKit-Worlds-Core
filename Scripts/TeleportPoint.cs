using Newtonsoft.Json;
using Photon.Realtime;
using SPACS.Core;
using SPACS.SDK.Utilities.API;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Virtuademy.Core;
using Virtuademy.Core.SceneManagement;
using Virtuademy.DTO;
using Virtuademy.Placeholders;

public class TeleportPoint : MonoBehaviour, IRuntimeComponent
{
    private string sceneAddressableName;
    private bool isLoading = false;

    #region Interfaces implementation
    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        TeleportPointPlaceholder teleportPointPlaceholer = placeholder as TeleportPointPlaceholder;
        sceneAddressableName = teleportPointPlaceholer.SceneAddressableName;
        isLoading = false;
        Debug.LogError($"inited {sceneAddressableName}");
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!isLoading)
        {
            isLoading = true;
            //prima di questo va creata experience (c'è api) da joinare. Checkare magari prima se ce ne sono già create
            //In caso mettere nel placeholder i preset sul come crearla
            //AppManager.Instance.LoadAddressableScene(sceneName);
            Load();
        }
    }

    private async void Load()
    {
        Debug.LogError(sceneAddressableName);
        var template = /*templatesDictionary[0];*/sceneAddressableName;
        var thumbnailPath = /*thumbnailPathsDictionary[0];*/string.Empty;
        Debug.Log(template);
        System.Random rnd = new();
        Experience newExperience = new()
        {
            InstituteId = AppManager.Instance.CurrentAssignment.Id,
            Label = $"#public#{new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5).Select(s => s[rnd.Next(s.Length)]).ToArray())}",
            Environment = new EnvironmentData { Template = template, ThumbnailKey = thumbnailPath },
        };
        Debug.Log(newExperience.Environment.Template);

        ApiResponse<Experience> newPublicExperienceReq = await SM.GetSystem<VirtuademyApiSystem>().PostPublicExperience(newExperience, 15, AppManager.Instance.CurrentAssignment.Id);
        //AppManager.Instance.LoadAddressableScene(sceneAddressableName);
        if (newPublicExperienceReq.IsSuccess)
        {
            Debug.Log($"New public experience created: {JsonConvert.SerializeObject(newPublicExperienceReq.Content)}");

            await AppManager.Instance.JoinExperience(newExperience);
        }
        else
        {
            Debug.LogError($"{newPublicExperienceReq.StatusCode} {newPublicExperienceReq.ReasonPhrase}");
        }
    }
}