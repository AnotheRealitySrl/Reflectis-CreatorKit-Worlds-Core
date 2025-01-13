using System;

using UnityEngine;

namespace Reflectis.CreatorKit.Core.ClientModels
{
    [Serializable]
    public class CMFolder
    {
        [SerializeField] private string name;
        [SerializeField] private int fileCount;
        [SerializeField] private int totalKBytes;

        public string TotalKBytesString
        {
            get
            {
                const int gigabyte = 1024 * 1024 * 1024;
                const int megabyte = 1024 * 1024;
                const int kilobyte = 1024;

                if (totalKBytes >= gigabyte)
                {
                    int value = totalKBytes / gigabyte;
                    return $"{value:F2} GB";
                }

                if (totalKBytes >= megabyte)
                {
                    double value = totalKBytes / megabyte;
                    return $"{value:F2} MB";
                }

                if (totalKBytes >= kilobyte)
                {
                    double value = totalKBytes / kilobyte;
                    return $"{value:F2} KB";
                }

                return $"{totalKBytes} bytes";
            }
        }

        public string Name { get => name; set => name = value; }
        public int FileCount { get => fileCount; set => fileCount = value; }
        public int TotalKBytes { get => totalKBytes; set => totalKBytes = value; }
    }
}
