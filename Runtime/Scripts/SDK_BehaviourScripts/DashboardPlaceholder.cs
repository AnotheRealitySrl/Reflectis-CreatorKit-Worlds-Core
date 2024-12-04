using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(BoxCollider))]
    public class DashboardPlaceholder : SceneComponentPlaceholderBase
    {
        public enum DashboardFilter
        {
            Environment,
            Category,
            Tag,
            None
        }

        [Header("DashboardData")]
        [SerializeField, Tooltip("Choose the type on information you want to show on the dashboard")] private DashboardFilter filter;
        [SerializeField, Tooltip("Write the name of the category,environment or tag present in the backoffice that you want to show on the dashboard ")] private string dashboardNameFilter;

        public string DashboardNameFilter => dashboardNameFilter;

        public DashboardFilter Filter => filter;


        private BoxCollider previewCollider;
        public BoxCollider PreviewCollider
        {
            get
            {
                if (previewCollider == null)
                {
                    previewCollider = GetComponent<BoxCollider>();
                }
                return previewCollider;
            }
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.black;
            Gizmos.matrix = transform.localToWorldMatrix;
            if (PreviewCollider == null)
            {
                Debug.LogError("The dashboard placeholder requires a box collider inside its gameobject to work properly.");
            }
            Gizmos.DrawCube(PreviewCollider.center,
                new Vector3(PreviewCollider.size.x,
                PreviewCollider.size.y,
                PreviewCollider.size.z));

        }
    }
}
