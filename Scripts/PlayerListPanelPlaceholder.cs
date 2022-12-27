using System.Collections;
using System.Collections.Generic;

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

        public override void Init(SceneComponentsMapper mapper)
        {
            PlayerListPanelPlaceholder placeholder = (PlayerListPanelPlaceholder)gameObject.AddComponent(mapper.PlayerListPanelComponent);

            PlayerListPanel playerListPanel = gameObject.AddComponent<PlayerListPanel>();
            playerListPanel.NumberOfParticipants = placeholder.NumberOfParticipants;
            playerListPanel.PlayerEntryTemplate = placeholder.PlayerEntryTemplate;
            playerListPanel.MuteButton = placeholder.MuteButton;
            playerListPanel.Init();
        }
    }
}

