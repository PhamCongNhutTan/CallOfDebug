using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class MultiplayerManager : BaseManager<MultiplayerManager>
{
    public bool isStarted = false;
    public bool startTiming = false;
    public GameObject cam;
    public bool curTeam;
    public int AScore = 0;
    public int BScore = 0;
    [SerializeField]
    public Dictionary<string, int[]> teamA = new Dictionary<string, int[]>();
    [SerializeField]
    public Dictionary<string, int[]> teamB = new Dictionary<string, int[]>();
    public GameObject[] posA;
    public GameObject[] posB;
    public int curPosA;
    public int curPosB;
    public CharacterLocomotionMultiplayer curCharacterLocomotion;
    private void Start()
    {
        posA = GameObject.FindGameObjectsWithTag("SpawnA");
        posB = GameObject.FindGameObjectsWithTag("SpawnB");
        if (UIManager.HasInstance())
        {
            UIManager.Instance.ShowOverlap<ChooseTeamBox>(null, true);
        }
        
    }
    private void Update()
    {

        if (isStarted)
        {
            curPosA = PhotonNetwork.LocalPlayer.ActorNumber;
            curPosB = PhotonNetwork.LocalPlayer.ActorNumber;
            StartGame();
            isStarted = false;
        }
    }
    public void updateScore(PlayerHealthMultiplayer health)
    {
        if (health.team)
        {
            teamA[health.gameObject.name][1]++;
            teamB[health.shooter][0]++;
            BScore++;
        }
        else
        {
            teamB[health.gameObject.name][1]++;
            teamA[health.shooter][0]++;
            AScore++;
        }

        if (ListenerManager.HasInstance())
        {
            ListenerManager.Instance.BroadCast(ListenType.ON_UPDATE_KDA, curTeam ? teamA[PhotonNetwork.NickName] : teamB[PhotonNetwork.NickName]);
        }
    }
    public void StartGame()
    {
        SpawnPlayer(curTeam);
        if(UIManager.HasInstance())
        {
            UIManager.Instance.HideAllPopups();
            UIManager.Instance.ShowScreen<GameUIMulti>(null, true);
        }
        if (AudioManager.HasInstance() && cam.active)
        {
            AudioManager.Instance.voiceSource.PlayOneShot(AudioManager.Instance.GetAudioClip("TeamDeath" + Random.Range(0, 4)));
        }
        PlayerHealthMultiplayer[] players = GameObject.FindObjectsOfType<PlayerHealthMultiplayer>();
        foreach (PlayerHealthMultiplayer p in players)
        {
            JoinTeam(p);
        }
        cam.SetActive(false);
    }
    public void EndGame()
    {
        if (curTeam && AScore > BScore)
        {
            int[] ins = { AScore, BScore };
            UIManager.Instance.ShowScreen<VictoryPanelMulti>(ins, true);
        }
        else if (curTeam)
        {
            int[] ins = { AScore, BScore };
            UIManager.Instance.ShowScreen<DefeatPanelMulti>(ins, true);
        }
        if (!curTeam&& AScore < BScore)
        {
            int[] ins = { BScore, AScore };
            UIManager.Instance.ShowScreen<VictoryPanelMulti>(ins, true);
        }
        else if (!curTeam)
        {
            int[] ins = { BScore, AScore };
            UIManager.Instance.ShowScreen<DefeatPanelMulti>(ins, true);
        }
    }

    public void ChooseTeam(bool team)
    { 
        curTeam = team;
        bool ready = false;
        if (PhotonNetwork.IsMasterClient)
        {
            ready = true;
        }
        Hashtable initProp = new Hashtable() { { "team", team } ,{"ready",ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(initProp);
    }
    public void SpawnPlayer(bool team)
    {
        if (team) //true = team A
        {
           
            GameObject player = PhotonNetwork.Instantiate("PlayerA", posA[curPosA].transform.position, Quaternion.identity);
            curPosA = (curPosA + 1) % posA.Length;
        }
        else
        {
            GameObject player = PhotonNetwork.Instantiate("PlayerB", posB[curPosB].transform.position, Quaternion.identity);
            curPosB = (curPosB + 1) % posB.Length;
        }
    }
    public void JoinTeam(PlayerHealthMultiplayer player)
    {
        if (player.team == MultiplayerManager.Instance.curTeam)
        {
            if (player.activeWeapon.photonView.IsMine && player.activeWeapon.photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                player.mark[(int)MARK.PLAYER].SetActive(true);
            }
            else
            {
                player.mark[(int)MARK.TEAMATE].SetActive(true);
            }
        }
        else
        {
            player.mark[(int)MARK.ENEMY].SetActive(true);
        }
        if (player.team)
        {
            if (teamA.ContainsKey(player.gameObject.name)) return;
            int[] zero = { 0, 0 };
            teamA.Add(player.gameObject.name, zero);
        }
        else
        {
            if (teamB.ContainsKey(player.gameObject.name)) return;
            int[] zero = { 0, 0 };
            teamB.Add(player.gameObject.name, zero);
        }
    }

    
}

