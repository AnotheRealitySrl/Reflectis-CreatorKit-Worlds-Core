using Reflectis.SDK.Core.ApplicationManagement;

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
            [SerializeField] private ESupportedPlatform supportedPlatforms = ESupportedPlatform.VR | ESupportedPlatform.WebGL;

            public SceneAsset Scene { get => scene; set => scene = value; }
            public bool IncludeInBuild { get => includeInBuild; set => includeInBuild = value; }
            public ESupportedPlatform SupportedPlatforms { get => supportedPlatforms; set => supportedPlatforms = value; }

            public string SceneNameFiltered => Regex.Replace(scene.name.ToLower(), alphanumeric_lowercase_string_pattern_negated, string.Empty);

            public HashSet<BuildTarget> GetRequiredBuildTargets()
            {
                var targets = new HashSet<BuildTarget> { BuildTarget.StandaloneWindows64 };

                if (supportedPlatforms.HasFlag(ESupportedPlatform.VR) || supportedPlatforms.HasFlag(ESupportedPlatform.Mobile))
                    targets.Add(BuildTarget.Android);

                if (supportedPlatforms.HasFlag(ESupportedPlatform.WebGL))
                    targets.Add(BuildTarget.WebGL);

                if (supportedPlatforms.HasFlag(ESupportedPlatform.Mobile))
                    targets.Add(BuildTarget.iOS);

                return targets;
            }

            /// <summary>
            /// Returns the subset of required BuildTargets whose Unity build module is NOT installed.
            /// Uses BuildPipeline.IsBuildTargetSupported — always empty for StandaloneWindows64
            /// because that module is bundled with the editor.
            /// </summary>
            public List<BuildTarget> GetMissingBuildTargets()
            {
                var missing = new List<BuildTarget>();
                foreach (BuildTarget target in GetRequiredBuildTargets())
                {
                    BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
                    if (!BuildPipeline.IsBuildTargetSupported(group, target))
                        missing.Add(target);
                }
                return missing;
            }
        }

        [SerializeField] private List<SceneConfiguration> sceneConfigurations;

        public List<SceneConfiguration> SceneConfigurations => sceneConfigurations;

    }
}
