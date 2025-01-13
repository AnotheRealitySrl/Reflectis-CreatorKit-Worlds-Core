using System;
using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    [Serializable]
    public class CMPermissionBlock
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private List<CMPermission> permissions;

        public int ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public List<CMPermission> Permissions { get => permissions; set => permissions = value; }
    }
}
