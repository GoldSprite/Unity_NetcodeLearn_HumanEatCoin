using GoldSprite.AutoListUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoldSprite.LobbyRoomUI
{
    public class RoomAutoListItem : AutoListItem
    {
        public Toggle Ready_toggle;
        public bool Ready;

        public override void UpdateContent()
        {
            base.UpdateContent();
            Ready_toggle.isOn = Ready;
        }
    }
}
