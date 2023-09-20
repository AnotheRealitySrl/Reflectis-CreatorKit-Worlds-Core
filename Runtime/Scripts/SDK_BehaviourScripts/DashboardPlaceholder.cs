using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
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
        [SerializeField] private int dashboardID;
        [SerializeField] private DashboardFilter filter;

        public int DashboardID => dashboardID;

        public DashboardFilter Filter => filter;

        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawCube(Vector3.zero, transform.localScale);

        }
    }
}
