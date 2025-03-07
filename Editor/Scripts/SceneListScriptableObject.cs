using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.Editor
{
    [CreateAssetMenu(fileName = "AddressablesSceneList", menuName = "Reflectis/CreatorKit/Worlds/Core/AddressablesSceneList")]
    public class SceneListScriptableObject : ScriptableObject
    {
        [Serializable]
        public class SceneConfiguration
        {
            [SerializeField] private SceneAsset scene;
            [SerializeField] private bool includeInBuild = true;

            public SceneAsset Scene { get => scene; set => scene = value; }
            public bool IncludeInBuild { get => includeInBuild; set => includeInBuild = value; }
        }

        [SerializeField] private List<SceneConfiguration> sceneConfigurations;

        public List<SceneConfiguration> SceneConfigurations => sceneConfigurations;

    }
}
