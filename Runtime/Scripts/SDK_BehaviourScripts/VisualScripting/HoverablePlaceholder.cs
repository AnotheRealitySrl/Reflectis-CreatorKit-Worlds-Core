using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class HoverablePlaceholder : SceneComponentPlaceholderBase
    {

        [SerializeField] private string hoverActionName;
        [SerializeField] private string unhoverActionName;

        [HideInInspector]
        public UnityEvent HoverAction = new UnityEvent();    
        
        [HideInInspector]
        public UnityEvent UnhoverAction = new UnityEvent();

        public string HoverActionName => hoverActionName;
        public string UnhoverActionName => unhoverActionName;
    }
}
