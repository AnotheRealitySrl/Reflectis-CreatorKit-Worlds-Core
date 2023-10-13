using Reflectis.SDK.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(MascottePlaceholder))]
    public class MascottePanelPlaceholder : PanelPlaceholder
    {
        [SerializeField]
        private string[] faqCategory;

        public string[] FaqCategory { get => faqCategory; }

    }
}