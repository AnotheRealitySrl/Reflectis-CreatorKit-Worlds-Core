using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class DashboardPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("DashboardData")]
        [SerializeField] private int dashboardID;

        public int DashboardID => dashboardID;
    }
}
