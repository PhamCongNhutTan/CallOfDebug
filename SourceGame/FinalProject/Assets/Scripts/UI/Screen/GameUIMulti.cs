using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class GameUIMulti : BaseScreen
{
    public TextMeshProUGUI allyScore;
    public TextMeshProUGUI enemyScore;
    public Image crossHit;
    public float countdownTime;
    public TextMeshProUGUI countdownText;
    public Image[] effectScore;
    public Text roomName;
    public GameObject ScoreBoard;
    public Transform[] scoreBoard;
    public List<GameObject> ScoreRows = new List<GameObject>();
    public GameObject scoreRowPf;
    public ChatMulti chatMulti;
    public MobileInput mobileInput;
    public GameObject mobileInputUI;
    
    private bool IsMobileInput()
    {
        return Application.isMobilePlatform || 
               (BaseManager<GameManager>.HasInstance() && 
                BaseManager<GameManager>.Instance.isAndroidDebugEditor);
    }
    
    public override void Hide()
    {
        base.Hide();
    }

    
    private void OnDestroy()
    {
        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.UPDATE_AMMO, new Action<object>(this.UpdateAmmo));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.UPDATE_HEALTH, new Action<object>(this.UpdateHealth));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.CHANGE_SCOPE, new Action<object>(this.changeScope));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.UPDATE_WEAPONUI, new Action<object>(this.UpdateActiveWeapon));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.UPDATE_TOTAL_AMMO, new Action<object>(this.UpdateAmmoTotal));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.UPDATE_SHOOTING_MODE, new Action<object>(this.changeShootingMode));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.ON_ENEMY_KILL, new Action<object>(this.UpdateEnemyKill));
            BaseManager<ListenerManager>.Instance.Unregister(ListenType.ON_ALLY_KILL, new Action<object>(this.UpdateAllyKill));
            ListenerManager.Instance.Unregister(ListenType.ON_UPDATE_KDA, UpdateKDA);
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_END_MISSION, null);
        }
    }

    
    public override void Init()
    {
        base.Init();
        this.crossHairUI.SetActive(true);
        this.scopeUI.SetActive(false);
        
        if (IsMobileInput())
        {
            if (mobileInputUI != null)
                mobileInputUI.SetActive(true);
        }
        else
        {
            if (mobileInputUI != null)
                mobileInputUI.SetActive(false);
        }

        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.Register(ListenType.UPDATE_AMMO, new Action<object>(this.UpdateAmmo));
            BaseManager<ListenerManager>.Instance.Register(ListenType.UPDATE_HEALTH, new Action<object>(this.UpdateHealth));
            BaseManager<ListenerManager>.Instance.Register(ListenType.CHANGE_SCOPE, new Action<object>(this.changeScope));
            BaseManager<ListenerManager>.Instance.Register(ListenType.UPDATE_ENERGY, new Action<object>(this.UpdateEnergyBar));
            BaseManager<ListenerManager>.Instance.Register(ListenType.UPDATE_WEAPONUI, new Action<object>(this.UpdateActiveWeapon));
            BaseManager<ListenerManager>.Instance.Register(ListenType.UPDATE_TOTAL_AMMO, new Action<object>(this.UpdateAmmoTotal));
            BaseManager<ListenerManager>.Instance.Register(ListenType.UPDATE_SHOOTING_MODE, new Action<object>(this.changeShootingMode));
            BaseManager<ListenerManager>.Instance.Register(ListenType.ON_ENEMY_KILL, new Action<object>(this.UpdateEnemyKill));
            BaseManager<ListenerManager>.Instance.Register(ListenType.ON_ALLY_KILL, new Action<object>(this.UpdateAllyKill));
            ListenerManager.Instance.Register(ListenType.ON_UPDATE_KDA, UpdateKDA);
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_START_MISSION, null);
        }
        if (BaseManager<DataManager>.HasInstance())
        {
            this.DeathPoint.text = "/0";
            this.timeRemaining = BaseManager<DataManager>.Instance.GlobalConfig.maxTimeDeathMath * 60f;
            countdownTime = DataManager.Instance.GlobalConfig.countdown * 60f;
        }
        if (MultiplayerManager.HasInstance())
        {
            if (MultiplayerManager.Instance.curTeam)
            {
                chatMulti.defaultChannels[1] = "TeamA";
            }
            else
            {
                chatMulti.defaultChannels[1] = "TeamB";
            }
        }
        for (int i = 0; i < this.rowKill.Length; i++)
        {
            this.rowKill[i].orderNum = i;
            this.rowKill[i].gameObject.SetActive(false);
        }
        string[] str = PhotonNetwork.CurrentRoom.Name.Split('_');
        roomName.text = $"{str[0]} : {str[1]}";
    }

    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ScoreBoard.SetActive(true);
            ShowScoreBoard();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ScoreBoard.SetActive(false);
        }
        if (countdownTime > 0)
        {
            countdownText.SetText($"{math.floor(countdownTime)}");
            countdownTime -= Time.deltaTime;
        }

        else if (countdownText.gameObject.active)
        {
            countdownText.gameObject.SetActive(false);
            PhotonNetwork.RaiseEvent((byte)EVENT_CODE.START_FIRE, null, new RaiseEventOptions { Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
            if(AudioManager.HasInstance())
            {
                AudioManager.Instance.PlaySE("GO");
            }
        }
        this.timeRemainingText.text = string.Format("{0}:{1}", math.floor(this.timeRemaining / 60f), math.floor(this.timeRemaining - math.floor(this.timeRemaining / 60f) * 60f));
        if (MultiplayerManager.Instance.startTiming)
        this.timeRemaining -= Time.deltaTime;
        if (int.Parse(allyScore.text)== int.Parse(enemyScore.text)&& timeRemaining<=0)
        {
            timeRemaining += 60;
        }
        if (timeRemaining<=0)
        {
            MultiplayerManager.Instance.EndGame();
        }
    }
    
    public override void Show(object data)
    {
        base.Show(data);
    }
    public void ShowScoreBoard()
    {
        foreach(GameObject r in ScoreRows)
        {
            Destroy(r);
        }
        ScoreRows.Clear();
        if (MultiplayerManager.HasInstance())
        {
            foreach(var row in MultiplayerManager.Instance.teamA)
            {
                GameObject r = Instantiate(scoreRowPf, scoreBoard[MultiplayerManager.Instance.curTeam ? 0 : 1]);
                r.GetComponent<ScoreBoardRow>().InitRow(row.Key, row.Value);
                ScoreRows.Add(r);
            }
            foreach (var row in MultiplayerManager.Instance.teamB)
            {
                GameObject r = Instantiate(scoreRowPf, scoreBoard[MultiplayerManager.Instance.curTeam ? 1 : 0]);
                r.GetComponent<ScoreBoardRow>().InitRow(row.Key, row.Value);
                ScoreRows.Add(r);
            }
        }
    }
    public void UpdateKDA(object data)
    {
        if (data is int[] values)
        {
            KillPoint.text = values[0].ToString();
            DeathPoint.text = "/" + values[1].ToString();
        }
    }
    
    public void UpdateEnemyKill(object data)
    {
        string[] array = data as string[];
        if (array != null)
        {
            this.rowKill[this.curRow].OnEnemyKill(array[0], array[1]);
            this.rowKill[this.curRow].gameObject.transform.SetAsFirstSibling();
            if (!this.rowKill[this.curRow].gameObject.active)
            {
                this.rowKill[this.curRow].gameObject.SetActive(true);
                base.StartCoroutine(this.UnactiveRow(this.curRow));
            }
            this.curRow = (this.curRow + 1) % this.rowKill.Length;
        }
        enemyScore.SetText((int.Parse(this.enemyScore.text) + 1).ToString());
        effectScore[1].DOFade(1, 0);
        effectScore[1].DOFade(0, 2);
    }

    
    public void UpdateAllyKill(object data)
    {
        string[] array = data as string[];
        if (array != null)
        {
            this.rowKill[this.curRow].OnAllyKill(array[0], array[1]);
            this.rowKill[this.curRow].gameObject.transform.SetAsFirstSibling();
            if (!this.rowKill[this.curRow].gameObject.active)
            {
                this.rowKill[this.curRow].gameObject.SetActive(true);
                base.StartCoroutine(this.UnactiveRow(this.curRow));
            }
            this.curRow = (this.curRow + 1) % this.rowKill.Length;
        }
        allyScore.SetText((int.Parse(this.allyScore.text) + 1).ToString());
        effectScore[0].DOFade(1, 0);
        effectScore[0].DOFade(0, 2);
    }

    
    private IEnumerator UnactiveRow(int n)
    {
        yield return new WaitForSeconds(60f);
        this.rowKill[n].gameObject.SetActive(false);
        yield break;
    }

    
    public void UpdateEnergyBar(object data)
    {
        if (data is float)
        {
            float value = (float)data;
            this.energyBar.value = value;
        }
    }

    
    public void changeShootingMode(object data)
    {
        if (data is ShootingMode)
        {
            ShootingMode shootingMode = (ShootingMode)data;
            GameObject[] array = this.modeIcon;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].SetActive(false);
            }
            this.modeIcon[(int)shootingMode].SetActive(true);
            this.modeText.text = shootingMode.ToString();
        }
    }

    
    public void changeScope(object data = null)
    {
        if (data is bool && (bool)data)
        {
            this.scopeUI.SetActive(false);
            this.crossHairUI.SetActive(true);
            return;
        }
        if (!this.scopeUI.active)
        {
            this.scopeUI.SetActive(true);
            this.crossHairUI.SetActive(false);
            return;
        }
        this.scopeUI.SetActive(false);
        this.crossHairUI.SetActive(true);
    }

    
    public void UpdateHealth(object value)
    {
        PlayerHealthMultiplayer playerHealth = value as PlayerHealthMultiplayer;
        if (playerHealth.shooter.Equals(PhotonNetwork.NickName))
        {
            if(playerHealth.currentHealth <= 0)
            {
                crossHit.color = Color.red;
            }
            else
            {
                crossHit.color = Color.white;
            }
            crossHit.DOFade(1, 0);
            crossHit.DOFade(0, 2);
        }
        if (!playerHealth.activeWeapon.photonView.IsMine) { return; }
        if (playerHealth.activeWeapon.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        if (playerHealth != null)
        {
            this.healthBar.value = playerHealth.currentHealth * 1f / 100f;
            this.healthText.text = playerHealth.currentHealth.ToString() + "%";
        }
    }

    
    public void UpdateAmmo(object value)
    {
        if (value is WeaponRaycastMulti weaponRaycastMulti)
        {
            if (!weaponRaycastMulti.photonView.IsMine) { return; }
            if (weaponRaycastMulti.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }
            if (weaponRaycastMulti != null && weaponRaycastMulti.equipBy == EquipBy.Player)
            {
                this.ammoText.text = weaponRaycastMulti.ammoCount.ToString();
            }
        }
        if (value is ActiveWeaponMultiplayer activeWeaponMultiplayer)
        {
            if (!activeWeaponMultiplayer.photonView.IsMine) return;
            if (activeWeaponMultiplayer.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber) return;

            this.ammoText.text = "0";

        }

    }

    
    public void UpdateAmmoTotal(object value)
    {
        WeaponRaycastMulti weaponRaycastMulti = value as WeaponRaycastMulti;
        if (!weaponRaycastMulti.photonView.IsMine) { return; }
        if (weaponRaycastMulti.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        if (weaponRaycastMulti != null && weaponRaycastMulti.equipBy == EquipBy.Player)
        {
            this.ammoTotalText.text = weaponRaycastMulti.totalAmmo.ToString();
        }
        if (value == null)
        {
            this.ammoText.text = "0";
        }

    }

    
    public void UpdateActiveWeapon(object? value)
    {
        if (value is ActiveWeaponMultiplayer active)
        {
            if (!active.photonView.IsMine) { return; }
            if (active.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }
            GameObject[] activeWeaponOverlay = this.ActiveWeaponOverlay;
            for (int i = 0; i < activeWeaponOverlay.Length; i++)
            {
                activeWeaponOverlay[i].SetActive(false);
            }
            if (active != null && !active.isHolstered)
            {
                this.ActiveWeaponOverlay[(int)active.GetActiveWeapon().weaponSlot].SetActive(true);
                this.weaponName.text = active.GetActiveWeapon().weaponName.ToString().ToUpper();
                return;
            }
            else
            {
                this.weaponName.text = "FIST";
            }
        }
        
    }

    
    public Text ammoText;

    
    public Text ammoTotalText;

    
    public Slider healthBar;

    
    public Text healthText;

    
    public Text weaponName;

    
    public GameObject scopeUI;

    
    public GameObject crossHairUI;

    
    public Slider energyBar;

    
    public GameObject[] ActiveWeaponOverlay;

    
    public GameObject[] modeIcon;

    
    public Text modeText;

    
    public Text DeathPoint;

    
    public Text KillPoint;

    
    public Text timeRemainingText;

    
    public float timeRemaining;

    
    public KillDisplay[] rowKill;

    
    public int curRow;
}
