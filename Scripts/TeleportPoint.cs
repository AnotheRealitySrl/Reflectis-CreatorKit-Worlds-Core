using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

using SPACS.Core;
using SPACS.SDK.Utilities.API;

using Virtuademy.Core;
using Virtuademy.DTO;
using Virtuademy.Placeholders;
using System.Collections;

public class TeleportPoint : MonoBehaviour, IRuntimeComponent
{
    private string sceneAddressableName;
    private bool enterGenericInstance = false;
    private bool isLoading = false;

    #region Interfaces implementation
    public void Init(SceneComponentPlaceholderBase placeholder)
    {
        TeleportPointPlaceholder teleportPointPlaceholer = placeholder as TeleportPointPlaceholder;
        sceneAddressableName = teleportPointPlaceholer.SceneAddressableName;
        enterGenericInstance = teleportPointPlaceholer.EnterGenericInstance;
        isLoading = false;
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

    private void Load()
    {
        // ToDo: All this code must be refactored when Shards and other similar stuff is
        // designed and defined!

        if (enterGenericInstance)
            LoadGenericInstance();
        else
            LoadNewRandomInstance();
    }

    private void LoadGenericInstance()
    {
        // We want to enter into a generic instance in order to see other people, such as this instance
        // is a sort of "lobby" space.

        Experience newExperience = JsonConvert.DeserializeObject<Experience>(
        @"{
            ""id"":-1,
            ""templateId"":-1,
            ""label"":""" + sceneAddressableName.ToLower() + @"#public#123456-inst" + AppManager.Instance.CurrentAssignment.Id + @""",
            ""description"":"""",
            ""details"":"""",
            ""scope"":[
                ""VR""
            ],
            ""status"":[
                ""Draft""
            ],
            ""teacherId"":-1,
            ""instituteId"":" + AppManager.Instance.CurrentAssignment.Id + @",
            ""instituteLabel"":""" + AppManager.Instance.CurrentAssignment.Label + @""",
            ""trackId"":-1,
            ""trackLabel"":""" + sceneAddressableName + @""",
            ""timeSlot"":""1900-01-01T00:00:00.000"",
            ""duration"":-1,
            ""capacity"":30,
            ""environment"":{
                ""template"":""" + sceneAddressableName.ToLower() + @"""
            },
            ""sceneObjs"":[],
            ""features"":[]
        }");

        // Attempt to connect to network if the connection is never started.
        if (AppManager.Instance.NetworkingManager.ConnectionState == NetworkingManager.EConnectionState.PeerCreated)
        {
            AppManager.Instance.NetworkingManager.ConnectUsingSettings();

            StartCoroutine(Co_WaitAndJoinExperience());
        }
        else
        {
            DoJoinExperience();
        }

        IEnumerator Co_WaitAndJoinExperience()
        {
            while (AppManager.Instance.NetworkingManager.ConnectionState != NetworkingManager.EConnectionState.JoinedLobby)
            {
                yield return null;
            }
            DoJoinExperience();
        }

        async void DoJoinExperience()
        {
            await AppManager.Instance.JoinExperience(newExperience);
        }
    }

    private async void LoadNewRandomInstance()
    {
        // We don't want to enter into a generic instance, so a new random Experience is created.

        var template = /*templatesDictionary[0];*/sceneAddressableName;
        var thumbnailPath = /*thumbnailPathsDictionary[0];*/string.Empty;
        System.Random rnd = new();
        Experience newExperience = new()
        {
            InstituteId = AppManager.Instance.CurrentAssignment.Id,
            Label = $"#public#{new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5).Select(s => s[rnd.Next(s.Length)]).ToArray())}",
            Environment = new EnvironmentData { Template = template, ThumbnailKey = thumbnailPath },
        };

        ApiResponse<Experience> newPublicExperienceReq = await SM.GetSystem<VirtuademyApiSystem>().PostPublicExperience(newExperience, 15, AppManager.Instance.CurrentAssignment.Id);
        //AppManager.Instance.LoadAddressableScene(sceneAddressableName);
        if (newPublicExperienceReq.IsSuccess)
        {
            Debug.Log($"New public experience created: {JsonConvert.SerializeObject(newPublicExperienceReq.Content)}");

            await AppManager.Instance.JoinExperience(newPublicExperienceReq.Content);
        }
        else
        {
            Debug.LogError($"{newPublicExperienceReq.StatusCode} {newPublicExperienceReq.ReasonPhrase}");
        }
    }
}