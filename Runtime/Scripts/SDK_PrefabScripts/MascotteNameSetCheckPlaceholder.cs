using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class MascotteNameSetCheckPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string mascotteName;

        public string MascotteName { get => mascotteName; set => mascotteName = value; }
    }
}
