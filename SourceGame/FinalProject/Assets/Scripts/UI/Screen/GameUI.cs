using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class GameUI : BaseScreen
{
    public Image hitCrosshair;
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
            ListenerManager.Instance.Unregister(ListenType.ON_ENEMY_HIT, OnHitEnemy);

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
            ListenerManager.Instance.Register(ListenType.ON_ENEMY_HIT, OnHitEnemy);
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_START_MISSION, null);
        }
        if (BaseManager<DataManager>.HasInstance())
        {
            this.totalEnemyText.text = "/" + BaseManager<DataManager>.Instance.GlobalConfig.totalEnemy.ToString();
            this.timeRemaining = BaseManager<DataManager>.Instance.GlobalConfig.maxTimePlay * 60f;
        }
        for (int i = 0; i < this.rowKill.Length; i++)
        {
            this.rowKill[i].orderNum = i;
            this.rowKill[i].gameObject.SetActive(false);
        }
        StartCoroutine(SetUp());
    }
    public IEnumerator SetUp()
    {
        yield return new WaitForSeconds(1f);
        if (AudioManager.HasInstance())
        {
            AudioManager.Instance.PlaySE("GO");
        }
    }
    
    private void Update()
    {
        this.timeRemainingText.text = string.Format("{0}:{1}", math.floor(this.timeRemaining / 60f), math.floor(this.timeRemaining - math.floor(this.timeRemaining / 60f) * 60f));
        this.timeRemaining -= Time.deltaTime;
    }

    
    public override void Show(object data)
    {
        base.Show(data);
    }

    
    public void OnHitEnemy(object? data)
    {

        hitCrosshair.color = Color.white;
        if (data is true)
        {
            hitCrosshair.color = Color.red;
        }
        hitCrosshair.DOFade(1, 0);
        hitCrosshair.DOFade(0, 2);

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
        this.currentEnemyText.text = (int.Parse(this.currentEnemyText.text) + 1).ToString();
        if (int.Parse(this.currentEnemyText.text) == BaseManager<DataManager>.Instance.GlobalConfig.totalEnemy)
        {
            BaseManager<UIManager>.Instance.ShowScreen<VictoryPanel>(null, true);
        }
    }

    
    private IEnumerator UnactiveRow(int n)
    {
        yield return new WaitForSeconds(3f);
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
        PlayerHealth playerHealth = value as PlayerHealth;
        if (playerHealth != null)
        {
            this.healthBar.value = playerHealth.currentHealth * 1f / 100f;
            this.healthText.text = playerHealth.currentHealth.ToString() + "%";
        }
    }

    
    public void UpdateAmmo(object value)
    {
        WeaponRaycast weaponRaycast = value as WeaponRaycast;
        if (weaponRaycast != null && weaponRaycast.equipBy == EquipBy.Player)
        {
            this.ammoText.text = weaponRaycast.ammoCount.ToString();
        }
        if (value == null)
        {
            this.ammoText.text = "0";
        }
    }

    
    public void UpdateAmmoTotal(object value)
    {
        WeaponRaycast weaponRaycast = value as WeaponRaycast;
        if (weaponRaycast != null)
        {
            this.ammoTotalText.text = weaponRaycast.totalAmmo.ToString();
            return;
        }
        this.ammoTotalText.text = "0";

    }

    
    public void UpdateActiveWeapon(object? value)
    {
        GameObject[] activeWeaponOverlay = this.ActiveWeaponOverlay;
        for (int i = 0; i < activeWeaponOverlay.Length; i++)
        {
            activeWeaponOverlay[i].SetActive(false);
        }
        WeaponRaycast weaponRaycast = value as WeaponRaycast;
        if (weaponRaycast != null)
        {
            this.ActiveWeaponOverlay[(int)weaponRaycast.weaponSlot].SetActive(true);
            this.weaponName.text = weaponRaycast.weaponName.ToString().ToUpper();
            return;
        }
        this.weaponName.text = "FIST";
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

    
    public Text totalEnemyText;

    
    public Text currentEnemyText;

    
    public Text timeRemainingText;

    
    public float timeRemaining;

    
    public KillDisplay[] rowKill;

    
    public int curRow;
}
