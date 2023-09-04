using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class ObjectActivatorPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private List<GameObject> vrComponents = new();
        [SerializeField] private List<GameObject> desktopComponents = new();

        public List<GameObject> VRComponents => vrComponents;
        public List<GameObject> DesktopComponents => desktopComponents;
    }

}

