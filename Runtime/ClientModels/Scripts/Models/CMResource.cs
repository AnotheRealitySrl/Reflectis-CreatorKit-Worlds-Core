using System;
using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMResource
    {
        [SerializeField] private int id;
        [SerializeField] private int ownerUserId;
        [SerializeField] private bool isPublic;
        [SerializeField] private string path; // folder
        [SerializeField] private string name; // label
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string url; // contentUri
        [SerializeField] private int type; // contentType
        [SerializeField] private string thumbnailPath; // thumbnailUri
        [SerializeField] private int sizeBytes;
        [SerializeField] private object metadata;

        public int Id { get => id; set => id = value; }
        public int OwnerUserId { get => ownerUserId; set => ownerUserId = value; }
        public bool IsPublic { get => isPublic; set => isPublic = value; }
        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }
        public DateTime LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        public string Url { get => url; set => url = value; }
        public FileTypeExt Type { get => (FileTypeExt)type; set => type = (int)value; }
        public string ThumbnailPath { get => thumbnailPath; set => thumbnailPath = value?.Replace(" ", "%20"); }
        public int SizeBytes { get => sizeBytes; set => sizeBytes = value; }
        public string SizeBytesString
        {
            get
            {
                const int gigabyte = 1024 * 1024 * 1024;
                const int megabyte = 1024 * 1024;
                const int kilobyte = 1024;

                if (SizeBytes >= gigabyte)
                {
                    int value = SizeBytes / gigabyte;
                    return $"{value:F2} GB";
                }

                if (sizeBytes >= megabyte)
                {
                    double value = SizeBytes / megabyte;
                    return $"{value:F2} MB";
                }

                if (SizeBytes >= kilobyte)
                {
                    double value = SizeBytes / kilobyte;
                    return $"{value:F2} KB";
                }

                return $"{SizeBytes} bytes";
            }
        }

        public object Metadata { get => metadata; set => metadata = value; }

        public CMResource() { }

        public CMResource(int id, string name, int sizeBytes, string path, string thumbnailPath, int type, DateTime creationDate)
        {
            Id = id;
            Name = name;
            this.sizeBytes = sizeBytes;
            Path = path;
            this.thumbnailPath = thumbnailPath;
            this.type = type;
            CreationDate = creationDate;
        }

        public override string ToString()
        {
            return $"ID: {id}\n" +
               $"Name: {name}\n" +
               $"Size (Bytes): {sizeBytes}\n" +
               $"Path: {path}\n" +
               $"Thumbnail Path: {thumbnailPath}\n" +
               $"Type: {type}\n" +
               $"Creation Date: {creationDate}";
        }
    }

    public class CMResourceEqualityComparer : IEqualityComparer<CMResource>
    {
        public bool Equals(CMResource x, CMResource y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.Id == y.Id;
            }
        }

        public int GetHashCode(CMResource obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
