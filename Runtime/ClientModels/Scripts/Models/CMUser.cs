using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    [Serializable]
    public class CMUser
    {
        [SerializeField] private int id;
        [SerializeField] private string nickname;
        [SerializeField] private int code;
        [SerializeField] private string email;
        [SerializeField] private List<CMTag> tags;
        [SerializeField] private CMUserPreference preferences;

        public int ID { get => id; set => id = value; }
        public string Nickname
        {
            get => nickname; set
            {
                nickname = value;
                if (string.IsNullOrEmpty(DisplayName))
                {
                    DisplayName = value;
                }
            }
        }
        public int Code { get => code; set => code = value; }
        public string Email { get => email; set => email = value; }
        public List<CMTag> Tags { get => tags; set => tags = value; }
        public CMUserPreference Preferences { get => preferences; set => preferences = value; }
        public string DisplayName { get; set; } // Use this property for user interfaces!
        public Color UserColor
        {
            get
            {
                Color color = new Color(1, 1, 1, 0);
                var visibleTags = Tags?.Where(x => x.Visible).ToList();
                if (visibleTags != null && visibleTags.Count > 0)
                {
                    color = visibleTags[0].Color;
                }
                return color;
            }
        }
    }

}
