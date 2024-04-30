using Reflectis.SDK.CreatorKit;
using Reflectis.SDK.RadialMenuUtils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Reflectis.SDK.CreatorKit
{
    public class RadialMenuPlaceholder : SceneComponentPlaceholderBase
    {
        [Tooltip("The list of the items that are going to be seen in the radialMenu.")]
        public List<RadialMenuItemData> itemListData;

        [Tooltip("the offset of the radialMenu position")]
        public Vector3 positionOffset;

        [Tooltip("The distance offset of the radialMenu from the camera")]
        public float distanceOffset;

        [Tooltip("The radius that the items are going to use when opening the radialMenu")]
        public float radius;

        [Tooltip("The speed with which the radialMenu will be opened")]
        public float openSpeed = 1f;

        [Tooltip("The speed with which the radialMenu will be closed")]
        public float closeSpeed;

        [Tooltip("The number of items after which the radialMenuItem scale is decreased.")]
        public int numberOfItemThreshold = 10;

        [Tooltip("the input pressed in order to open and close the radialMenu")]
        public InputAction action;

        public AudioClip itemClickAudio;

        public bool isNetworked; //used to know if we need to add the RadialRPCManager too and also to know if we need to instantiate the RadialMenuNetworked


        [HideInInspector] public ReflectisRadialItemSpawnerPlaceholder reflectisRadialItemSpawnerPlaceholder;

    }
}

[CustomEditor(typeof(RadialMenuPlaceholder))]
public class RadialMenuPlaceholderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RadialMenuPlaceholder script = (RadialMenuPlaceholder)target;

        if (script.isNetworked)
        {
            EditorGUILayout.BeginHorizontal();

            script.reflectisRadialItemSpawnerPlaceholder = EditorGUILayout.ObjectField("ReflectisRadialItemSpawnerPlaceholder", script.reflectisRadialItemSpawnerPlaceholder, typeof(ReflectisRadialItemSpawnerPlaceholder), true) as ReflectisRadialItemSpawnerPlaceholder;

            EditorGUILayout.EndHorizontal();
        }
    }

}
