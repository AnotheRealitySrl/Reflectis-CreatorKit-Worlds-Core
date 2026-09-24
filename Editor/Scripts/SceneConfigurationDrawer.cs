using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Virtuademy.SDK.Environments.Editor
{
    /// <summary>
    /// Draws one scene of the Addressables scene list with, under its settings, where that scene is
    /// already published ("Published in Showroom, Academy" / "Published at tenant level" / "Not
    /// published yet"), read from <see cref="PublishedEnvironmentsIndex"/>. The fields themselves stay
    /// the default ones — scene, include in build, platforms — so the list looks as before.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneListScriptableObject.SceneConfiguration))]
    internal class SceneConfigurationDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.style.marginBottom = 4;

            SerializedProperty scene = property.FindPropertyRelative("scene");
            root.Add(new PropertyField(scene));
            root.Add(new PropertyField(property.FindPropertyRelative("includeInBuild")));
            root.Add(new PropertyField(property.FindPropertyRelative("supportedPlatforms")));

            Label published = new();
            published.style.marginLeft = 3;
            published.style.marginTop = 1;
            published.style.fontSize = 11;
            published.style.unityFontStyleAndWeight = FontStyle.Italic;
            published.style.opacity = 0.85f;
            published.style.whiteSpace = WhiteSpace.Normal;
            root.Add(published);

            void Refresh()
            {
                Object asset = scene.objectReferenceValue;
                if (asset == null) { published.text = string.Empty; published.tooltip = string.Empty; return; }
                string key = PublishedEnvironmentsIndex.Key(asset.name);
                published.text = PublishedEnvironmentsIndex.Summary(key);
                published.tooltip = PublishedEnvironmentsIndex.HasData ? PublishedEnvironmentsIndex.Details(key) : string.Empty;
            }

            Refresh();
            root.TrackPropertyValue(scene, _ => Refresh());
            root.RegisterCallback<AttachToPanelEvent>(_ => PublishedEnvironmentsIndex.Changed += Refresh);
            root.RegisterCallback<DetachFromPanelEvent>(_ => PublishedEnvironmentsIndex.Changed -= Refresh);
            return root;
        }
    }
}
