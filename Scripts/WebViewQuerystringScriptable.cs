using Sirenix.OdinInspector;

using SPACS.SDK.CharacterController;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Virtuademy.DTO;
using Virtuademy.Placeholders;

using static Virtuademy.Core.AppManager;

[CreateAssetMenu(menuName = "AnotheReality/Utils/WebViewQuerystringScriptable", fileName = "WebViewQuerystringScriptable")]
public class WebViewQuerystringScriptable : SerializedScriptableObject
{
    // These Dictionaries will be serialized by Odin.
    [SerializeField] private Dictionary<QuerystringParameter, string> querystringKeyMappings = new();
    [SerializeField] private Dictionary<string, string> constantQuerystrings = new();
    [SerializeField] private Dictionary<Role, string> rolesMapping = new();

    public Dictionary<QuerystringParameter, string> QuerystringKeyMappings => querystringKeyMappings;
    public Dictionary<string, string> ConstantQuerystrings => constantQuerystrings;
    public Dictionary<Role, string> RolesMapping => rolesMapping;
}