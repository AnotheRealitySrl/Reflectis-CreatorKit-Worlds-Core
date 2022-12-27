using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public abstract class SceneComponentPlaceholderBase : MonoBehaviour
    {
        public abstract void Init(SceneComponentsMapper mapper);
    }
}