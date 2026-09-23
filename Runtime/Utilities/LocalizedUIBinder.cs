using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.UIElements;

namespace Virtuademy.SDK.Environments.Utilities
{
    /// <summary>
    /// Per-instance authoring surface for the localized UI Toolkit elements
    /// (<c>Virtuademy.LocalizedComponents.LocalizedLabel</c>, <c>LocalizedButton</c>, …).
    ///
    /// Its custom Inspector lists every localized element found in the bound <see cref="UIDocument"/>'s
    /// UXML and lets you assign each element's localization key(s) on THIS instance, without editing
    /// the shared <c>.uxml</c>. At runtime the keys are pushed onto the matching elements (looked up by
    /// their UXML <c>name</c>), so the same panel asset can be reused in several places with different
    /// keys set from the Inspector.
    ///
    /// The value field shows I2's term dropdown when I2Loc is present and a plain string field
    /// otherwise — the same convention the <c>LocalizedXxx</c> elements themselves use. When I2Loc is
    /// absent the key is only stored (nothing extra is displayed); it becomes visible once I2Loc is
    /// installed and the elements translate it.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LocalizedUIBinder : MonoBehaviour
    {
        [Serializable]
        public class KeySlot
        {
            [Tooltip("The localized attribute this value is written to (e.g. locKey, locKeyLabel).")]
            public string attribute;

            [Tooltip("The localization key (or, with I2, the term) assigned to that attribute.")]
            public string value;
        }

        [Serializable]
        public class LocalizedElementBinding
        {
            [Tooltip("The 'name' of the localized element in the UXML.")]
            public string elementName;

            [Tooltip("The element's type — display only.")]
            public string elementType;

            public List<KeySlot> slots = new();
        }

        [SerializeField, Tooltip("UIDocument that renders the UXML. If empty, the first UIDocument on " +
            "this object or its children is used.")]
        private UIDocument document;

        [SerializeField, Tooltip("One entry per localized element found in the UXML. Populated " +
            "automatically in the Inspector.")]
        private List<LocalizedElementBinding> bindings = new();

        // Maximum number of frames to wait for the UIDocument to build its visual tree.
        private const int MaxApplyFrames = 120;

        private Coroutine applyRoutine;
        private bool applied;

        public IReadOnlyList<LocalizedElementBinding> Bindings => bindings;

        private void OnEnable()
        {
            if (TryApply())
            {
                return;
            }
            // rootVisualElement is not always built during OnEnable on the first frame; keep trying.
            applyRoutine = StartCoroutine(ApplyWhenReady());
        }

        private void OnDisable()
        {
            if (applyRoutine != null)
            {
                StopCoroutine(applyRoutine);
                applyRoutine = null;
            }
            applied = false;
        }

        private IEnumerator ApplyWhenReady()
        {
            for (int frame = 0; frame < MaxApplyFrames && !applied; frame++)
            {
                yield return null;
                if (TryApply())
                {
                    break;
                }
            }
            applyRoutine = null;
        }

        private bool TryApply()
        {
            if (applied)
            {
                return true;
            }
            if (document == null)
            {
                document = GetComponentInChildren<UIDocument>(true);
            }
            if (document == null)
            {
                Debug.LogWarning($"[{nameof(LocalizedUIBinder)}] No UIDocument found on '{name}'.", this);
                return false;
            }

            VisualElement root = document.rootVisualElement;
            if (root == null)
            {
                // The document has not built its tree yet; the caller will retry.
                return false;
            }

            foreach (LocalizedElementBinding binding in bindings)
            {
                if (binding == null || string.IsNullOrEmpty(binding.elementName))
                {
                    continue;
                }
                VisualElement element = root.Q(binding.elementName);
                if (element == null)
                {
                    Debug.LogWarning($"[{nameof(LocalizedUIBinder)}] Element '{binding.elementName}' " +
                        $"not found in the document on '{name}'.", this);
                    continue;
                }
                ApplySlots(element, binding);
            }

            applied = true;
            return true;
        }

        // Writes each slot's value onto the matching localized attribute via reflection. Setting the
        // attribute triggers the element's own logic (it translates through I2 when present).
        private static void ApplySlots(VisualElement element, LocalizedElementBinding binding)
        {
            Type type = element.GetType();
            foreach (KeySlot slot in binding.slots)
            {
                if (slot == null || string.IsNullOrEmpty(slot.attribute) || string.IsNullOrEmpty(slot.value))
                {
                    // Empty value → keep whatever the UXML authored for this attribute.
                    continue;
                }
                PropertyInfo prop = type.GetProperty(slot.attribute,
                    BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
                {
                    prop.SetValue(element, slot.value);
                }
            }
        }
    }
}
