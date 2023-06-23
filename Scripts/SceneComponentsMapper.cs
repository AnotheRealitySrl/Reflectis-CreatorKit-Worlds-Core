using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using SPACS.SDK.Utilities.Extensions;

namespace Virtuademy.Placeholders
{
    [CreateAssetMenu(menuName = "AnotheReality/Virtuademy/SceneComponentsMapper", fileName = "SceneComponentsMapper")]
    public class SceneComponentsMapper : SerializedScriptableObject
    {
        public enum ERuntimeComponentId
        {
            Teleportation,
            PlayerListPanel,
            DrawableBoard,
            MediaPlayer,
            VoiceChannel,
            TutorialPanel,
            VideoPlayer,
            PresentationPlayer,
            Floor,
            WebView,
            ObjectActivator,
            TeleportPoint,
            SpawnAddressablePrefab,
            WebView3D
        }

        // This Dictionary will be serialized by Odin.
        [SerializeField] private Dictionary<ERuntimeComponentId, List<TextAsset>> componentsMap = new();

        public List<Type> GetComponentsTypes(ERuntimeComponentId id) => componentsMap[id].Select(x => GetType(x.name)).ToList();

        private Type GetType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.LastPartOfTypeName() == typeName)
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}