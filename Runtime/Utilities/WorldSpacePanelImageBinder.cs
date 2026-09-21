using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace Reflectis.CreatorKit.Worlds.Placeholders
{
    /// <summary>
    /// Assigns a list of images to the thumbnail elements of a world-space UI Toolkit panel, in
    /// document order, without touching the shared UXML/USS. This is the UI Toolkit equivalent of
    /// "swap the sprite reference on the instance" in uGUI: the images are serialized per-instance on
    /// this component and pushed into the visual tree at runtime.
    ///
    /// It targets every <see cref="VisualElement"/> that carries <see cref="ThumbClassName"/> (default
    /// <c>bcp-thumb</c>) and sets its <c>background-image</c>. The i-th element gets the i-th image;
    /// extra images or extra elements are simply ignored. Missing (null) entries are skipped, so a
    /// card keeps its USS placeholder.
    ///
    /// Companion to <see cref="WorldSpaceButtonBinder"/> (which binds clicks by name). Kept independent
    /// so you can use either, both, or neither. It re-applies automatically if the panel is rebuilt at
    /// runtime (e.g. by <see cref="WorldSpaceUIDocumentRebuilder"/>), because the rebuild replaces the
    /// visual tree and any image set on the old elements would otherwise be lost.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class WorldSpacePanelImageBinder : MonoBehaviour
    {
        [SerializeField, Tooltip("UIDocument that renders the panel. If empty, the first UIDocument on " +
            "this object or its children is used.")]
        private UIDocument document;

        [SerializeField, Tooltip("USS class of the elements that receive an image. Cards created from " +
            "ButtonChoicePanel use 'bcp-thumb'.")]
        private string thumbClassName = "bcp-thumb";

        [SerializeField, Tooltip("One image per thumbnail, in the same order the cards appear in the " +
            "UXML (button-0, button-1, ...). Import the textures as Sprite (2D and UI).")]
        private List<Sprite> images = new();

        // Maximum number of frames to wait for the UIDocument to build its visual tree.
        private const int MaxApplyFrames = 120;

        private Coroutine applyRoutine;
        private VisualElement watched;
        private bool applied;

        /// <summary>The USS class the binder looks for (read-only, for callers/tests).</summary>
        public string ThumbClassName => thumbClassName;

        /// <summary>The images assigned to the thumbnails, in order.</summary>
        public IReadOnlyList<Sprite> Images => images;

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
            Unwatch();
            applied = false;
        }

        /// <summary>
        /// Replaces the images at runtime and pushes them to the panel immediately. Handy from a
        /// UnityEvent or a Visual Scripting graph when the content changes.
        /// </summary>
        public void SetImages(IEnumerable<Sprite> newImages)
        {
            images = new List<Sprite>(newImages);
            Apply();
        }

        /// <summary>Forces a re-apply against the current visual tree.</summary>
        public void Apply()
        {
            applied = false;
            if (!TryApply() && isActiveAndEnabled)
            {
                if (applyRoutine != null)
                {
                    StopCoroutine(applyRoutine);
                }
                applyRoutine = StartCoroutine(ApplyWhenReady());
            }
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
                Debug.LogWarning($"[{nameof(WorldSpacePanelImageBinder)}] No UIDocument found on '{name}'.", this);
                return false;
            }

            VisualElement root = document.rootVisualElement;
            if (root == null)
            {
                // The document has not built its tree yet; the caller will retry.
                return false;
            }

            List<VisualElement> thumbs = root.Query<VisualElement>(className: thumbClassName).ToList();
            if (thumbs.Count == 0)
            {
                // Tree exists but the cards are not in yet (or the class name is wrong); retry.
                return false;
            }

            int count = Mathf.Min(thumbs.Count, images.Count);
            for (int i = 0; i < count; i++)
            {
                if (images[i] == null)
                {
                    continue;
                }
                thumbs[i].style.backgroundImage = new StyleBackground(images[i]);
            }

            // Re-apply automatically if the tree gets rebuilt (the elements detach from the panel).
            Watch(thumbs[0]);

            applied = true;
            return true;
        }

        private void Watch(VisualElement element)
        {
            if (watched == element)
            {
                return;
            }
            Unwatch();
            watched = element;
            watched.RegisterCallback<DetachFromPanelEvent>(OnThumbDetached);
        }

        private void Unwatch()
        {
            if (watched != null)
            {
                watched.UnregisterCallback<DetachFromPanelEvent>(OnThumbDetached);
                watched = null;
            }
        }

        private void OnThumbDetached(DetachFromPanelEvent _)
        {
            // The visual tree was torn down (e.g. a rebuild). Re-bind against the fresh tree.
            Unwatch();
            applied = false;
            if (isActiveAndEnabled)
            {
                if (applyRoutine != null)
                {
                    StopCoroutine(applyRoutine);
                }
                applyRoutine = StartCoroutine(ApplyWhenReady());
            }
        }
    }
}
