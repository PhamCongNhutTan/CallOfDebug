
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeaponMultiplayer : MonoBehaviourPun
{
    public CamMultiplayer camManager;
    public ObjectPoolMulti pool;
    private MobileInput mobileInput;
    
    private bool IsMobileInput()
    {
        return Application.isMobilePlatform || 
               (BaseManager<GameManager>.HasInstance() && 
                BaseManager<GameManager>.Instance.isAndroidDebugEditor);
    }
    
    private void Awake()
    {
        this.characterAiming = base.GetComponent<CharacterAimingMultiplayer>();
        this.reload = base.GetComponent<WeaponReloadMultiplayer>();
        this.animator = base.GetComponent<Animator>();
        this.weaponEvent = base.GetComponent<WeaponEventMulti>();
        this.characterLocomotion = base.GetComponent<CharacterLocomotionMultiplayer>();
        camManager = GetComponent<CamMultiplayer>();
        this.mobileInput = GetComponent<MobileInput>();
        if (this.mobileInput == null)
        {
            this.mobileInput = GameObject.FindFirstObjectByType<MobileInput>();
        }
        this.isHolstered = true;
        //if (!photonView.IsMine) { return; }
        //if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        //{
        //    return;
        //}
        //crossHairTarget = FindObjectOfType<CrossHairTarget>().transform;
        
    }
    
    private void Start()
    {
        this.existWeapon = base.GetComponentsInChildren<WeaponRaycastMulti>();
        for (int i = 0; i < this.existWeapon.Length; i++)
        {
            if (this.existWeapon[i] != null)
            {
                this.Equip(this.existWeapon[i]);
            }
        }
        for (int j = 0; j < this.equippedWeapons.Length; j++)
        {
            if (this.equippedWeapons[j] != null)
            {
                this.SetActiveWeapon(this.equippedWeapons[j].weaponSlot);
                return;
            }
        }
    }

    
    private void Update()
    {
        //weapon.UpdateWeapon(Time.deltaTime, this.crossHairTarget.position);
        if (!photonView.IsMine) { return; }
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        if (this.characterAiming.isEcs)
        {
            return;
        }


        WeaponRaycastMulti weapon = GetActiveWeapon(); /*this.GetWeapon(this.activeWeaponIndex);*/
        bool flag = false;
        if (BaseManager<CameraManager>.HasInstance())
        {
            flag = BaseManager<CameraManager>.Instance.isAiming;
        }
        else
        {
            flag = camManager.isAiming;
        }
        if (!this.isChangingWeapon && !this.reload.isReloading && !this.IsFiring() && !this.characterLocomotion.IsSprinting() && !flag)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                this.SetActiveWeapon(ActiveWeaponMultiplayer.WeaponSlot.Primary);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                this.SetActiveWeapon(ActiveWeaponMultiplayer.WeaponSlot.Secondary);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                this.ToggleActiveWeapon();
            }
        }
        bool flag2 = !this.isHolstered && !this.isChangingWeapon && !this.reload.isReloading && !this.characterLocomotion.IsSprinting();
        if (weapon != null && (weapon.weaponName.Equals("scar") || weapon.weaponName.Equals("pistol")))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                this.curShootingMode = weapon.mode[(int)((int)(this.curShootingMode + 1) % (int)(ShootingMode)weapon.mode.Count)];
                if (BaseManager<ListenerManager>.HasInstance())
                {
                    BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_SHOOTING_MODE, this.curShootingMode);
                }
            }
            
            // Check fire input - keyboard or mobile button
            bool fireDown = false;
            bool fireUp = false;
            bool fireHeld = false;
            
            if (IsMobileInput() && mobileInput != null)
            {
                if (mobileInput.firePressed)
                {
                    fireDown = true;
                    mobileInput.firePressed = false;
                }
                fireHeld = mobileInput.fireHolding;
                if (!mobileInput.fireHolding)
                {
                    fireUp = true;
                }
            }
            else
            {
                fireDown = Input.GetButtonDown("Fire1");
                fireUp = Input.GetButtonUp("Fire1");
                fireHeld = Input.GetButton("Fire1");
            }
            
            if (flag2 && this.curShootingMode == ShootingMode.Auto)
            {
                if (fireDown)
                {
                    weapon.StartFiring();
                    //photonView.RPC("PlayerStartFiring", RpcTarget.All);
                }
                else if (fireUp)
                {
                    weapon.StopFiring();
                    //photonView.RPC("PlayerStopFiring", RpcTarget.All);
                }
            }
            else if (fireDown && weapon.mode.Contains(ShootingMode.Single) && this.curShootingMode == ShootingMode.Single && flag2)
            {
                weapon.SingleShot(this.crossHairTarget.position);
                //photonView.RPC("PlayerSingleShot", RpcTarget.All);
            }
            weapon.UpdateWeapon(Time.deltaTime, this.crossHairTarget.position);
            //photonView.RPC("PlayerUpdateWeapon", RpcTarget.All);
        }
        //else if (weapon != null && weapon.weaponName.Equals("knife"))
        //{
        //	this.animator.SetLayerWeight(2, 1f);
        //	if (Input.GetButtonDown("Fire1"))
        //	{
        //		this.animator.Play("KnifeAttack");
        //	}
        //}
        //if (this.isHolstered && Input.GetButtonDown("Fire1"))
        //{
        //	this.animator.Play("2HandAttack");
        //}
    }
    [PunRPC]
    public void RecoilWeapon(string name)
    {
        rigController.SetTrigger("recoil_" + name);
        if(GetActiveWeapon().weaponAnimator != null)
        {
            GetActiveWeapon().weaponAnimator.SetTrigger("Fire");

        }
    }
    [PunRPC]
    public void EmitMuzzle()
    {
        GetActiveWeapon().shell.Emit(1);
        foreach (ParticleSystem particleSystem in this.GetWeapon(this.activeWeaponIndex).muzzleFlash)
        {
            particleSystem.Emit(particleSystem.maxParticles);
        }
    }
    //[PunRPC]
    //public void PlayerSingleShot()
    //{
    //       if (!photonView.IsMine) { return; }
    //       if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
    //       {
    //           return;
    //       }
    //       GetWeapon(this.activeWeaponIndex).SingleShot(crossHairTarget.gameObject.transform.position);

    //   }
    //[PunRPC]
    //public void PlayerStopFiring()
    //{
    //       if (!photonView.IsMine) { return; }
    //       if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
    //       {
    //           return;
    //       }
    //       GetWeapon(this.activeWeaponIndex).StopFiring();

    //   }
    //[PunRPC]
    //public void PlayerStartFiring()
    //{
    //       if (!photonView.IsMine) { return; }
    //       if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
    //       {
    //           return;
    //       }
    //       GetWeapon(this.activeWeaponIndex).StartFiring();
    //   }
    //[PunRPC]
    //public void PlayerUpdateWeapon()
    //{
    //       if (!photonView.IsMine) { return; }
    //       if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
    //       {
    //           return;
    //       }
    //       this.GetWeapon(this.activeWeaponIndex).UpdateWeapon(Time.deltaTime, this.crossHairTarget.gameObject.transform.position);
    //   }
    
    public bool IsFiring()
    {
        WeaponRaycastMulti activeWeapon = this.GetActiveWeapon();
        return activeWeapon && activeWeapon.isFiring;
    }

    
    public void Equip(WeaponRaycastMulti newWeapon)
    {
        int weaponSlot = (int)newWeapon.weaponSlot;
        WeaponRaycastMulti weapon = this.GetWeapon(weaponSlot);
        if (weapon)
        {
            UnityEngine.Object.Destroy(weapon.gameObject);
        }
        newWeapon.raycastDestination = this.crossHairTarget;
        newWeapon.weaponRecoil.rigController = this.rigController;
        newWeapon.shooter = base.gameObject.name;
        newWeapon.transform.SetParent(this.weaponSlots[weaponSlot], false);
        this.equippedWeapons[weaponSlot] = newWeapon;
        newWeapon.weaponRecoil.characterAiming = this.characterAiming;
        newWeapon.pool = this.pool;
        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, newWeapon);
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_WEAPONUI, this);
            this.curShootingMode = newWeapon.mode[0];
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_SHOOTING_MODE, this.curShootingMode);
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_TOTAL_AMMO, newWeapon);
            return;
        }
    }

    
    private WeaponRaycastMulti GetWeapon(int index)
    {
        if (index < 0 || index >= this.equippedWeapons.Length)
        {
            return null;
        }
        return this.equippedWeapons[index];
    }

    
    public WeaponRaycastMulti GetActiveWeapon()
    {
        //activeWeaponIndex = rigController.GetInteger("weapon_index");
        return this.GetWeapon(this.activeWeaponIndex);
    }

    
    private void SetActiveWeapon(ActiveWeaponMultiplayer.WeaponSlot weaponSlot)
    {
        int num = this.activeWeaponIndex;
        if (num == (int)weaponSlot)
        {
            num = -1;
        }
        base.StartCoroutine(this.SwitchWeapon(num, (int)weaponSlot));
    }

    
    private IEnumerator SwitchWeapon(int holsterIndex, int activeIndex)
    {
        this.rigController.SetInteger("weapon_index", activeIndex);
        yield return base.StartCoroutine(this.HolsterWeapon(holsterIndex));
        yield return base.StartCoroutine(this.ActivateWeapon(activeIndex));
        photonView.RPC("ChangeIndex",RpcTarget.All, activeIndex);
        yield break;
    }

    
    private void ToggleActiveWeapon()
    {
        if (this.rigController.GetBool("holster_weapon"))
        {
            base.StartCoroutine(this.ActivateWeapon(this.activeWeaponIndex));
            return;
        }
        base.StartCoroutine(this.HolsterWeapon(this.activeWeaponIndex));
    }

    
    private IEnumerator HolsterWeapon(int index)
    {
        this.isChangingWeapon = true;
        if (this.GetWeapon(index))
        {
            this.rigController.SetBool("holster_weapon", true);
            yield return new WaitForSeconds(0.1f);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while ((double)this.rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0);
        }
        this.isHolstered = true;
        this.isChangingWeapon = false;
        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, this);
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_WEAPONUI, this);
        }
        yield break;
    }

    
    private IEnumerator ActivateWeapon(int index)
    {
        this.isChangingWeapon = true;
        WeaponRaycastMulti weapon = this.GetWeapon(index);
        if (weapon)
        {
            //this.activeWeaponIndex = index;
            photonView.RPC("ChangeIndex", RpcTarget.All, index);
            this.rigController.SetBool("holster_weapon", false);
            this.rigController.Play("equip_" + weapon.weaponName);
            yield return new WaitForSeconds(0.1f);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while ((double)this.rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0);
            this.isHolstered = false;
            if (BaseManager<ListenerManager>.HasInstance())
            {
                BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, weapon);
                BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_WEAPONUI, this);
                this.curShootingMode = weapon.mode[0];
                BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_SHOOTING_MODE, this.curShootingMode);
                BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_TOTAL_AMMO, weapon);
            }
        }
        this.curShootingMode = weapon.mode[0];
        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_SHOOTING_MODE, this.curShootingMode);
        }
        this.isHolstered = false;
        this.isChangingWeapon = false;
        yield break;
    }

    
    public void DropWeapon()
    {
        WeaponRaycastMulti activeWeapon = this.GetActiveWeapon();
        if (activeWeapon)
        {
            activeWeapon.transform.SetParent(null);
            activeWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            activeWeapon.gameObject.AddComponent<Rigidbody>();
            this.equippedWeapons[this.activeWeaponIndex] = null;
        }
        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, this);
        }
    }
    [PunRPC]
    public void ChangeIndex(int ind)
    {
        activeWeaponIndex = ind;
    }
    
    public WeaponRaycastMulti[] equippedWeapons = new WeaponRaycastMulti[4];

    
    public int activeWeaponIndex;

    
    public Animator rigController;

    
    public Transform[] weaponSlots;

    
    public Transform crossHairTarget;

    
    private Animator animator;

    
    public bool isChangingWeapon;

    
    public bool isHolstered;

    
    public CharacterAimingMultiplayer characterAiming;

    
    public WeaponReloadMultiplayer reload;

    
    private WeaponEventMulti weaponEvent;

    
    public WeaponRaycastMulti[] existWeapon;

    
    private CharacterLocomotionMultiplayer characterLocomotion;

    
    private ShootingMode curShootingMode;

    
    public enum WeaponSlot
    {
        
        Primary,
        
        Secondary,
        
        Knife
    }
}
