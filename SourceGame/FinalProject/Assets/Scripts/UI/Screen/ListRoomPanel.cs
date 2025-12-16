using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
public class ListRoomPanel : BaseScreen
{
    public TabMission All;
    public GameObject prefabRoom;
    public GameObject curSelectedRoomObject;
    public Text curSelectedRoomName;
    public Text curSelectedRoomMode;
    public Text curSelectedRoomMap;
    public GameObject ChooseMapPanel;
    public GameObject[] mapImage;
    public Transform[] playersTable;
    public PlayerRow playerRowPf;
    public List<PlayerRow> curPlayers;
    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackButton();
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance())
        {
            ListenerManager.Instance.Unregister(ListenType.ON_SELECTED_ROOM, OnSelectRoom);
            ListenerManager.Instance.Unregister(ListenType.ON_DESELECTED_ROOM, OnDeselectRoom);
        }
    }
    public override void Show(object data)
    {
        base.Show(data);
        All.TurnOn();
        OnDeselectRoom(null);
        if (ListenerManager.HasInstance())
        {
            ListenerManager.Instance.Register(ListenType.ON_SELECTED_ROOM, OnSelectRoom);
            ListenerManager.Instance.Register(ListenType.ON_DESELECTED_ROOM, OnDeselectRoom);
        }
    }
    public void OnDeselectRoom(object? data)
    {
        if (curSelectedRoomObject != null)
        {
            if (curSelectedRoomObject.active)
            {
                curSelectedRoomObject.SetActive(false);
            }
        }

    }
    public void OnSelectRoom(object data)
    {
        if (data is RoomRow roomRow)
        {
            curSelectedRoomObject.SetActive(true);
            curSelectedRoomName.text = roomRow.nameText.text;
            curSelectedRoomMode.text = roomRow.modeText.text;
            curSelectedRoomMap.text = roomRow.mapText.text;
            foreach (GameObject img in mapImage)
            {
                img.SetActive(false);
            }
            switch (roomRow.mapText.text)
            {
                case "OldCity":
                    {
                        mapImage[0].SetActive(true);
                        break;
                    }
                case "Industry1":
                    {
                        mapImage[1].SetActive(true);
                        break;
                    }
                case "Industry2":
                    {
                        mapImage[2].SetActive(true);
                        break;
                    }
            }
        }

    }
    public void OnJoinRoomButton()
    {
        PhotonNetwork.JoinRoom($"{curSelectedRoomName.text}_{curSelectedRoomMap.text}");
    }
    public void OnCreateRoomButton()
    {
        ChooseMapPanel.SetActive(true);

        //if (PhotonNetwork.CurrentRoom != null) { return; }
        //RoomOptions roomOptions = new RoomOptions
        //{
        //    MaxPlayers = 8
        //};
        //string roomName = PhotonNetwork.NickName + "'s room";
        //PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        //Debug.Log(PhotonNetwork.NetworkClientState);

    }
    public void CloseChooseMap()
    {
        ChooseMapPanel.SetActive(false);
    }
    public void ChooseMap(string name)
    {
        if (PhotonNetwork.CurrentRoom != null ) { PhotonNetwork.RejoinRoom(PhotonNetwork.CurrentRoom.Name); return; }
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 8
        };
        string roomName = $"{PhotonNetwork.NickName}'s room_{name}";
        
        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        ChooseMapPanel.SetActive(false);
    }
    public void OnLeaveRoomButton()
    {
        if (PhotonNetwork.CurrentRoom == null) { return; }
        if (MultiplayerManager.HasInstance())
        {
            Destroy(MultiplayerManager.Instance.gameObject);
        }
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
    }
    public void OnBackButton()
    {
        if (UIManager.HasInstance())
        {
            UIManager.Instance.ShowScreen<MainMenu>(null, true);
        }
        if (MultiplayerManager.HasInstance())
        {
            Destroy(MultiplayerManager.Instance.gameObject);
        }
        PhotonNetwork.LoadLevel("Main");
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.LeaveLobby();
        }
        
    }

    public void playerListUpdate()
    {
        foreach (PlayerRow row in curPlayers)
        {
            Destroy(row.gameObject);
        }
        curPlayers.Clear();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("team"))
            {
                PlayerRow row = Instantiate(playerRowPf, playersTable[(bool)player.CustomProperties["team"] ? 0 : 1]);
                row.Init(player.NickName, (bool)player.CustomProperties["ready"]);
                curPlayers.Add(row);
            }

        }
    }
    public void OnStartButton()
    {
        if (CheckPlayerReady())
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.RaiseEvent((byte)EVENT_CODE.START_GAME, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log("raise event");
        }
    }
    private bool CheckPlayerReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["ready"] == false)
            {
                return false;
            }
        }
        return true;
    }
    public void OnReadyButton()
    {
        Hashtable prop = new Hashtable() { { "ready", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
    }
    public void OncancelButton()
    {
        Hashtable prop = new Hashtable() { { "ready", false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
    }
}
