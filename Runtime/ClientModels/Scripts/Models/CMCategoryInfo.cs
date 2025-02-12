using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    public class CMCategoryInfo
    {
        [SerializeField] private CMCategory category;
        [SerializeField] private CMCategory[] subcategories;


        public CMCategory Category { get => category; set => category = value; }
        public CMCategory[] Subcategories { get => subcategories; set => subcategories = value; }

    }
}
