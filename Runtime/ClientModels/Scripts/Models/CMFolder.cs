using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMFolder
    {
        [SerializeField] private int id;
        [SerializeField] private int parentId;
        [SerializeField] private string name;
        [SerializeField] private string path;
        [SerializeField] private int assetCount;
        [SerializeField] private List<CMFolder> subFolders;
        [SerializeField] private List<CMResource> assets;

        public int Id { get => id; set => id = value; }
        public int ParentId { get => parentId; set => parentId = value; }
        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
        public int AssetCount { get => assetCount; set => assetCount = value; }
        public List<CMFolder> SubFolders { get => subFolders; set => subFolders = value; }
        public List<CMResource> Assets { get => assets; set => assets = value; }
    }
}
