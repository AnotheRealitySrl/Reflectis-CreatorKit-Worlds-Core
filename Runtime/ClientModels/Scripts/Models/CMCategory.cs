using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMCategory
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private int parent;

        public int ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int Parent { get => parent; set => parent = value; }
    }
}
