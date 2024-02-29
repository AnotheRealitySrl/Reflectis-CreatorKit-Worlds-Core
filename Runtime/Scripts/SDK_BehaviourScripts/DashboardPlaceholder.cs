using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(BoxCollider))]
    public class DashboardPlaceholder : SceneComponentPlaceholderNetwork
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

        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawCube(Vector3.zero, new Vector3(transform.localScale.x / transform.localScale.x, transform.localScale.y / transform.localScale.y, transform.localScale.z / transform.localScale.z));

        }
    }
}
