using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class DashboardPlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum DashboardFilter
        {
            Environment,
            Tag,
            None
        }

        [Header("DashboardData")]
        [SerializeField] private int dashboardID;
        [SerializeField] private DashboardFilter filter;

        public int DashboardID => dashboardID;

        public DashboardFilter Filter => filter;
    }
}
