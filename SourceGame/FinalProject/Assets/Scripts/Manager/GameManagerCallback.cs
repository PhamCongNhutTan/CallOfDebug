using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManagerCallback : MonoBehaviourPunCallbacks
{
    public override void OnConnected()
    {
        Debug.Log("Onconnected");
        base.OnConnected();
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("oncreateroom");
        base.OnCreatedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected");
        base.OnDisconnected(cause);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Onjoinlobby");
        base.OnJoinedLobby();
    }

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("Onjoinroom");
    //    base.OnJoinedRoom();
    //}

    public override void OnLeftLobby()
    {
        Debug.Log("joinlobby");
        base.OnLeftLobby();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("connecttomaster");
        PhotonNetwork.JoinLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("roomlistUpdate");
        base.OnRoomListUpdate(roomList);
    }
}
