using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ClientModels
{
    public enum HandPreference
    {
        left,
        right
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CMUserPreference
    {
        [Serializable]
        [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
        public class AvatarConfigDTO
        {
            [JsonProperty("avatarId")]
            [SerializeField] private string avatarId;
            [JsonProperty("avatarPng")]
            [SerializeField] private string avatarPng;

            public string AvatarId { get => avatarId; set => avatarId = value; }
            public string AvatarPng { get => avatarPng; set => avatarPng = value; }
        }

        [SerializeField] private string language;
        [SerializeField] private string nickname;
        [SerializeField] private string bio;
        [SerializeField] private float height;
        [SerializeField] private string social1;
        [SerializeField] private string social2;
        [SerializeField] private string social3;
        [SerializeField] private AvatarConfigDTO avatarConfig;
        [SerializeField][CanBeNull] private string dateOfBirth;
        [SerializeField][CanBeNull] private string city;
        [SerializeField] private string hand;
        [SerializeField] private Dictionary<string, string> mascotteNames;

        [JsonProperty("language")]
        public string Language { get => language; set => language = value; }

        [JsonProperty("nickname")]
        public string Nickname { get => nickname; set => nickname = value; }

        [JsonProperty("bio")]
        public string Bio { get => bio; set => bio = value; }

        [JsonProperty("height")]
        public float Height { get => height; set => height = value; }

        [JsonProperty("social1")]
        public string Social1 { get => social1; set => social1 = value; }

        [JsonProperty("social2")]
        public string Social2 { get => social2; set => social2 = value; }

        [JsonProperty("social3")]
        public string Social3 { get => social3; set => social3 = value; }

        [JsonProperty("avatarConfig")]
        public AvatarConfigDTO AvatarConfig { get => avatarConfig; set => avatarConfig = value; }

        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get => dateOfBirth; set => dateOfBirth = value; }

        [JsonProperty("city")]
        [CanBeNull] public string City { get => city; set => city = value; }


        [JsonProperty("hand")]
        public HandPreference Hand
        {
            get => Enum.TryParse(hand, out HandPreference _) ? (HandPreference)Enum.Parse(typeof(HandPreference), hand) : HandPreference.right;
            set => hand = value.ToString();
        }

        public Dictionary<string, string> MascotteNames { get => mascotteNames; set => mascotteNames = value; }
    }

}
