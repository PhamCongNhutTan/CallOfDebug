using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCallback : MonoBehaviourPunCallbacks
{
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (UIManager.HasInstance())
        {
            UIManager.Instance.ShowScreen<ListRoomPanel>(null,true);
        }
    }
}