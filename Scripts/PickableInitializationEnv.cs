using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.AuthoringTool;
using Virtuademy.Core;

public class PickableInitializationEnv : MonoBehaviour
{
    [SerializeField] string addressableNameToInstantiate;


    private void Awake()
    {
        InstantiateObject();
    }

    private async void InstantiateObject()
    {
        var sceneObj = SceneObjUtility.MakeSceneObj(addressableNameToInstantiate, addressableNameToInstantiate, ~Role.Guest);

        var instantiatedObj = await sceneObj.Instantiate(new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
    }
}
