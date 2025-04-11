using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            private const string alphanumeric_lowercase_string_pattern_negated = @"[^a-z0-9]";

            [SerializeField] private SceneAsset scene;
            [SerializeField] private bool includeInBuild = true;

            public SceneAsset Scene { get => scene; set => scene = value; }
            public bool IncludeInBuild { get => includeInBuild; set => includeInBuild = value; }

            public string SceneNameFiltered => Regex.Replace(scene.name.ToLower(), alphanumeric_lowercase_string_pattern_negated, string.Empty);
        }

        [SerializeField] private List<SceneConfiguration> sceneConfigurations;

        public List<SceneConfiguration> SceneConfigurations => sceneConfigurations;

    }
}
