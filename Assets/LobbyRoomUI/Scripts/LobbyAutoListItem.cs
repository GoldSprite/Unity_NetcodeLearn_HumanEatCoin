using GoldSprite.AutoListUI;
using System.Collections;
using System.Collections.Generic;
using Unity.Sync.Relay.Lobby;
using Unity.Sync.Relay.Model;
using UnityEngine;
using UnityEngine.UI;

namespace GoldSprite.LobbyRoomUI
{
    public class LobbyAutoListItem : AutoListItem
    {
        public Text RoomPlayerCount_Txt;
        public uint MaxRoomPlayers = 4;
        public uint RoomPlayerCount;
        public LobbyRoom roomInfo;

        public override void UpdateContent()
        {
            base.UpdateContent();
            RoomPlayerCount_Txt.text = RoomPlayerCount + "/" + MaxRoomPlayers;
        }

        public void JoinRoom()
        {
            if (roomInfo != null)
            {
                LobbyRoomManager.Instance.JoinRoom(roomInfo);
            }
        }
    }
}
