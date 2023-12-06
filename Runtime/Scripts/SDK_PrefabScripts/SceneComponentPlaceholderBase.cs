using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

using Virtuademy.DTO;

namespace Reflectis.SDK.CreatorKit
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        [SerializeField] private bool automaticSetup = true;

        public bool AutomaticSetup => automaticSetup;

        public virtual async Task Init(SceneComponentsMapper mapper)
        {
            foreach (Type type in mapper.GetComponentsTypes(GetType().ToString().Split('.')[^1]))
            {
                await ((IRuntimeComponent)gameObject.AddComponent(type)).Init(this);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Set All Placeholder New ID")]
        private void SetAllPlaceholderNewID()
        {
            var placeholders = FindObjectsOfType<SceneComponentPlaceholderNetwork>(true);
            var addressablePlaceholders = FindObjectsOfType<SpawnNetworkedAddressablePlaceholder>(true);

            if (placeholders.Length != 0)
            {
                for (var i = 0; i < placeholders.Length; i++)
                {
                    if (placeholders[i].IsNetworked)
                    {
                        placeholders[i].InitializationId = i + 1;

                        EditorUtility.SetDirty(placeholders[i]);
                    }


                    if (i == placeholders.Length - 1)
                    {
                        for (var j = 0; j < addressablePlaceholders.Length; j++)
                        {
                            if (addressablePlaceholders[j].IsNetworked)
                            {
                                addressablePlaceholders[j].InitializationId = j + i + 1;
                                EditorUtility.SetDirty(addressablePlaceholders[j]);
                            }
                        }
                    }
                }
            }
            else
            {
                for (var j = 0; j < addressablePlaceholders.Length; j++)
                {
                    if (addressablePlaceholders[j].IsNetworked)
                    {
                        addressablePlaceholders[j].InitializationId = j + 1;
                        EditorUtility.SetDirty(addressablePlaceholders[j]);
                    }
                }
            }
        }

#endif
    }
}