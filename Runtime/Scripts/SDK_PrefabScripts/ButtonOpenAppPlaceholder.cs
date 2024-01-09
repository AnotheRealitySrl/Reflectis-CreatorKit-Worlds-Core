using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class ButtonOpenAppPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string appLink;

        public string AppLink => appLink;
    }
}
