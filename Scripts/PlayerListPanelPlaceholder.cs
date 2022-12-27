using System.Collections;
using System.Collections.Generic;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class PlayerListPanelPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private TextMeshProUGUI numberOfParticipants;
        [SerializeField] private PlayerListPanelTile playerEntryTemplate;
        [SerializeField] private GameObject muteButton;

        public override void Init(SceneComponentsMapper mapper)
        {
            gameObject.AddComponent(mapper.PlayerListPanelComponent);

            PlayerListPanel playerListPanel = gameObject.AddComponent<PlayerListPanel>();
            playerListPanel.NumberOfParticipants = numberOfParticipants;
            playerListPanel.PlayerEntryTemplate = playerEntryTemplate;
            playerListPanel.MuteButton = muteButton;
            playerListPanel.Init();
        }
    }
}

