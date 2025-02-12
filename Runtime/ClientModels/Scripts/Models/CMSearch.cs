using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    public class CMSearch<T>
    {
        [SerializeField] private List<T> items = new List<T>();
        [SerializeField] private int totalCount;

        public List<T> Items { get => items; set => items = value; }
        public int TotalCount { get => totalCount; set => totalCount = value; }
    }
}
