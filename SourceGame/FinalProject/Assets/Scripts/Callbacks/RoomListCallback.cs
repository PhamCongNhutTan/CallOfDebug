using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public enum BUTTON
{
    CREATE,
    LEAVE,
    JOIN,
    CANCEL,
    START,
    READY
}
public enum EVENT_CODE
{
    START_GAME,
    START_FIRE
}
public class RoomListCallback : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public ListRoomPanel listRoomPanel;
    public List <GameObject> roomObject;
    public GameObject room;
    public GameObject roomList;
    public GameObject[] buttons;
    public Text roomName;
    public Text mapName;
    public GameObject[] map;
    private void Update()
    {
        if(PhotonNetwork.NetworkClientState== ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("roomlistupdate");
        ClearRoom();
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                continue;
            }
            GameObject row = Instantiate(listRoomPanel.prefabRoom,listRoomPanel.All.board.transform);
            RoomRow roomRow = row.GetComponent<RoomRow>();
            roomRow.toggle.group = listRoomPanel.All.board.GetComponent<ToggleGroup>();
            string[] info = roomInfo.Name.Split('_');
            roomRow.InitRow((byte)roomObject.Count, info[0], info[1], (byte)roomInfo.PlayerCount, (byte)roomInfo.MaxPlayers);
            roomObject.Add(row);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LoadLevel("Main");
        room.SetActive(false);
        roomList.SetActive(true);
        foreach(GameObject button in buttons)
        {
            button.SetActive(false);
        }
        buttons[(int)BUTTON.CREATE].SetActive(true);
        buttons[(int)BUTTON.JOIN].SetActive(true);
        
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Joinroom");
        base.OnJoinedRoom();
        room.SetActive(true);
        roomList.SetActive(false );
        roomName.text = PhotonNetwork.CurrentRoom.Name.Split('_')[0];
        string mapString = PhotonNetwork.CurrentRoom.Name.Split('_')[1];
        mapName.text = mapString;
        switch (mapString)
        {
            case "OldCity":
                {
                    map[0].SetActive(true);
                    break;
                }
            case "Industry1":
                {
                    map[1].SetActive(true);
                    break;
                }
            case "Industry2":
                {
                    map[2].SetActive(true);
                    break;
                }
        }
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            buttons[(int)BUTTON.START].SetActive(true);
        }
        else
        {
            buttons[(int)BUTTON.READY].SetActive(true);
        }
        buttons[(int)BUTTON.LEAVE].SetActive(true);
        listRoomPanel.playerListUpdate();
        //Debug.Log("joined room "+PhotonNetwork.CurrentRoom);
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.LoadLevel(mapString);
        
    }
    
    private void ClearRoom()
    {
        foreach (GameObject room in roomObject)
        {
            Destroy(room);
            
        }
        roomObject.Clear();
    }
    //private IEnumerator waitConnectMaster()
    //{
    //    while (!isConnected)
    //    {
    //        yield return null;
    //    }
    //    PhotonNetwork.JoinLobby();
    //}
    

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("playerenter");   
        base.OnPlayerEnteredRoom(newPlayer);
        listRoomPanel.playerListUpdate();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        listRoomPanel.playerListUpdate();

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (targetPlayer.Equals(PhotonNetwork.LocalPlayer))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                buttons[(int)BUTTON.START].SetActive(true);
            }
            else
            {
                buttons[(int)BUTTON.READY].SetActive(!(bool)changedProps["ready"]);
                buttons[(int)BUTTON.CANCEL].SetActive((bool)changedProps["ready"]);
            }
            
        }
        listRoomPanel.playerListUpdate();
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (int)EVENT_CODE.START_GAME)
        {
            if (MultiplayerManager.HasInstance())
            {
                MultiplayerManager.Instance.isStarted = true;
            }
        }
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("connecttomaster");
        PhotonNetwork.JoinLobby();
    }
}
