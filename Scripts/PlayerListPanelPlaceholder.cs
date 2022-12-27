using System;

using TMPro;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class PlayerListPanelPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private TextMeshProUGUI numberOfParticipants;
        [SerializeField] private PlayerListPanelTile playerEntryTemplate;
        [SerializeField] private GameObject muteButton;

        public TextMeshProUGUI NumberOfParticipants => numberOfParticipants;
        public PlayerListPanelTile PlayerEntryTemplate => playerEntryTemplate;
        public GameObject MuteButton => muteButton;
    }
}

