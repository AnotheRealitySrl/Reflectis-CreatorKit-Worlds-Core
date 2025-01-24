using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMWorldCCU
    {
        [SerializeField] private int id;
        [SerializeField] private int ccu;

        public int Id { get => id; set => id = value; }
        public int CCU { get => ccu; set => ccu = value; }
    }
}
