
using System.Collections;
using UnityEngine;


public class ActiveWeapon : MonoBehaviour
{

	private void Awake()
	{
		this.characterAiming = base.GetComponent<CharacterAiming>();
		this.reload = base.GetComponent<WeaponReload>();
		this.animator = base.GetComponent<Animator>();
		this.weaponEvent = base.GetComponent<WeaponEvent>();
		this.characterLocomotion = base.GetComponent<CharacterLocomotion>();
		this.isHolstered = true;
	}

	private bool IsMobileInput()
	{
		return Application.isMobilePlatform ||
			   (BaseManager<GameManager>.HasInstance() &&
				BaseManager<GameManager>.Instance.isAndroidDebugEditor);
	}


	private void Start()
	{
		if (mobileInput == null && IsMobileInput())
		{
			mobileInput = GameObject.FindFirstObjectByType<MobileInput>();
		}
		Debug.Log($"mobileInput in ActiveWeapon: {mobileInput}");
		this.existWeapon = base.GetComponentsInChildren<WeaponRaycast>();
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
		if (this.characterAiming.isEcs)
		{
			return;
		}
		WeaponRaycast weapon = this.GetWeapon(this.activeWeaponIndex);
		bool flag = false;
		if (BaseManager<CameraManager>.HasInstance())
		{
			flag = BaseManager<CameraManager>.Instance.isAiming;
		}
		if (!this.isChangingWeapon && !this.reload.isReloading && !this.IsFiring() && !this.characterLocomotion.IsSprinting() && !flag)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.SetActiveWeapon(ActiveWeapon.WeaponSlot.Primary);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.SetActiveWeapon(ActiveWeapon.WeaponSlot.Secondary);
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
				//Debug.Log("changeMode " + weapon.mode[(int)((this.curShootingMode + 1) % (ShootingMode)weapon.mode.Count)].ToString() + " num: " + ((int)((this.curShootingMode + 1) % (ShootingMode)weapon.mode.Count)).ToString());
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
				}
				else if (fireUp)
				{
					weapon.StopFiring();
				}
			}
			else if (fireDown && weapon.mode.Contains(ShootingMode.Single) && this.curShootingMode == ShootingMode.Single)
			{
				weapon.SingleShot(this.crossHairTarget.position);
			}
			weapon.UpdateWeapon(Time.deltaTime, this.crossHairTarget.position);
		}
		else if (weapon != null && weapon.weaponName.Equals("knife"))
		{
			this.animator.SetLayerWeight(2, 1f);
			bool fireDown = false;
			if (IsMobileInput() && mobileInput != null)
			{
				if (mobileInput.firePressed)
				{
					fireDown = true;
					mobileInput.firePressed = false;
				}
			}
			else
			{
				fireDown = Input.GetButtonDown("Fire1");
			}
			if (fireDown)
			{
				this.animator.Play("KnifeAttack");
			}
		}

		bool fire2Down = false;
		if (IsMobileInput() && mobileInput != null)
		{
			if (mobileInput.firePressed)
			{
				fire2Down = true;
				mobileInput.firePressed = false;
			}
		}
		else
		{
			fire2Down = Input.GetButtonDown("Fire1");
		}
		if (this.isHolstered && fire2Down)
		{
			this.animator.Play("2HandAttack");
		}
	}


	public bool IsFiring()
	{
		WeaponRaycast activeWeapon = this.GetActiveWeapon();
		return activeWeapon && activeWeapon.isFiring;
	}


	public void Equip(WeaponRaycast newWeapon)
	{
		int weaponSlot = (int)newWeapon.weaponSlot;
		WeaponRaycast weapon = this.GetWeapon(weaponSlot);
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
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, newWeapon);
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_WEAPONUI, newWeapon);
			this.curShootingMode = newWeapon.mode[0];
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_SHOOTING_MODE, this.curShootingMode);
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_TOTAL_AMMO, newWeapon);
			return;
		}
	}


	private WeaponRaycast GetWeapon(int index)
	{
		if (index < 0 || index >= this.equippedWeapons.Length)
		{
			return null;
		}
		return this.equippedWeapons[index];
	}


	public WeaponRaycast GetActiveWeapon()
	{
		return this.GetWeapon(this.activeWeaponIndex);
	}


	private void SetActiveWeapon(ActiveWeapon.WeaponSlot weaponSlot)
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
		this.activeWeaponIndex = activeIndex;
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
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, null);
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_WEAPONUI, null);
		}
		yield break;
	}


	private IEnumerator ActivateWeapon(int index)
	{
		this.isChangingWeapon = true;
		WeaponRaycast weapon = this.GetWeapon(index);
		if (weapon)
		{
			this.activeWeaponIndex = index;
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
				BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_WEAPONUI, weapon);
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
		WeaponRaycast activeWeapon = this.GetActiveWeapon();
		if (activeWeapon)
		{
			activeWeapon.transform.SetParent(null);
			activeWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
			activeWeapon.gameObject.AddComponent<Rigidbody>();
			this.equippedWeapons[this.activeWeaponIndex] = null;
		}
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, null);
		}
	}


	public WeaponRaycast[] equippedWeapons = new WeaponRaycast[4];


	private int activeWeaponIndex;


	public Animator rigController;


	public Transform[] weaponSlots;


	public Transform crossHairTarget;


	private Animator animator;


	public bool isChangingWeapon;


	public bool isHolstered;


	public CharacterAiming characterAiming;


	public WeaponReload reload;


	private WeaponEvent weaponEvent;


	public WeaponRaycast[] existWeapon;


	private CharacterLocomotion characterLocomotion;


	private ShootingMode curShootingMode;


	public MobileInput mobileInput;


	public enum WeaponSlot
	{

		Primary,

		Secondary,

		Knife
	}
}
