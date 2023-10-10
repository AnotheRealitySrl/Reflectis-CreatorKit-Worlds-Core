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
        [SerializeField] private string dashboardNameFilter;
        [SerializeField] private DashboardFilter filter;

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
