using SPACS.SDK.Extensions;

using System;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    [CreateAssetMenu(menuName = "AnotheReality/Virtuademy/SceneComponentsMapper", fileName = "SceneComponentsMapper")]
    public class SceneComponentsMapper : ScriptableObject
    {
        [SerializeField] private TextAsset teleportationComponent;
        [SerializeField] private TextAsset playerListPanelComponent;

        public Type TeleportationComponent => GetType(teleportationComponent.name);
        public Type PlayerListPanelComponent => GetType(playerListPanelComponent.name);

        public Type GetType(string typeName)
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