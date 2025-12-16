
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class WeaponReloadMultiplayer : MonoBehaviourPun
{
	private MobileInput mobileInput;
	
	private void Start()
	{
		if (animationEvent != null)
		{
            this.animationEvent.WeaponAnimEvent.AddListener(new UnityAction<string>(this.OnAnimationEvent));
        }
		this.mobileInput = GameObject.FindFirstObjectByType<MobileInput>();
	}

	private bool IsMobileInput()
	{
		return Application.isMobilePlatform ||
			   (BaseManager<GameManager>.HasInstance() &&
				BaseManager<GameManager>.Instance.isAndroidDebugEditor);
	}
	
	private void Update()
	{
        if (!photonView.IsMine) { return; }
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        if (this.activeWeapon.characterAiming.isEcs)
		{
			return;
		}
		
		bool reloadInput = false;
		if (IsMobileInput() && mobileInput != null)
		{
			if (mobileInput.reloadPressed)
			{
				reloadInput = true;
				mobileInput.reloadPressed = false;
			}
		}
		else
		{
			reloadInput = Input.GetKeyUp(KeyCode.R);
		}
		
		WeaponRaycastMulti weaponRaycast = this.activeWeapon.GetActiveWeapon();
		if (weaponRaycast && (reloadInput || weaponRaycast.ammoCount <= 0) && weaponRaycast.totalAmmo > 0 && weaponRaycast.ammoCount < weaponRaycast.gunInfo[KeyInfo.maxAmmo] - 5 &&!isReloading)
		{
			this.isReloading = true;
			if (AudioManager.HasInstance())
			{
				AudioManager.Instance.PlaySE("WeLoading");
			}
			if (rigController != null)
			{
                this.rigController.SetTrigger("reload_weapon");
            }
			
		}
	}

	
	private void OnAnimationEvent(string eventName)
	{
		if (eventName == "detach_magazine")
		{
			this.DetachMagazine();
			return;
		}
		if (eventName == "drop_magazine")
		{
			this.DropMagazine();
			return;
		}
		if (eventName == "refill_magazine")
		{
			this.RefillMagazine();
			return;
		}
		if (eventName == "attach_magazine")
		{
			this.AttachMagazine();
			return;
		}
		if (eventName == "load_bullet")
		{
			this.LoadBullet();
			return;
		}
		if (!(eventName == "done_reload"))
		{
			return;
		}
		this.DoneReload();
	}

	
	private void DetachMagazine()
	{
		if (BaseManager<CameraManager>.HasInstance())
		{
			BaseManager<CameraManager>.Instance.TurnOffScope();
		}
		else
		{
			activeWeapon.camManager.TurnOffScope();
		}
		if (BaseManager<AudioManager>.HasInstance())
		{
			//BaseManager<AudioManager>.Instance.PlaySE("PistolDetachMag", 0f);
            animationEvent.playerAudio.PlayOneShot(AudioManager.Instance.GetAudioClip("PistolDetachMag"));
        }
		WeaponRaycastMulti weaponRaycast = this.activeWeapon.GetActiveWeapon();
		Transform component;
		if (!weaponRaycast.weaponName.Equals("pistol"))
		{
			component = this.leftHand;
		}
		else
		{
			component = weaponRaycast.magazine.GetComponent<Transform>();
		}
		this.magazineHand = Object.Instantiate<GameObject>(weaponRaycast.magazine, component, true);
		weaponRaycast.magazine.SetActive(false);
	}

	
	private void DropMagazine()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.magazineHand, this.magazineHand.transform.position, this.magazineHand.transform.rotation);
		gameObject.transform.localScale = Vector3.one;
		gameObject.AddComponent<Rigidbody>();
		gameObject.AddComponent<BoxCollider>();
		Object.Destroy(gameObject, 3f);
		this.magazineHand.SetActive(false);
	}

	
	private void RefillMagazine()
	{
		this.magazineHand.SetActive(true);
	}

	
	private void AttachMagazine()
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			//BaseManager<AudioManager>.Instance.PlaySE("PistolAttachMag", 0f);
			animationEvent.playerAudio.PlayOneShot(AudioManager.Instance.GetAudioClip("PistolAttachMag"));
		}
		this.activeWeapon.GetActiveWeapon().magazine.SetActive(true);
		Object.Destroy(this.magazineHand);
	}

	
	private void LoadBullet()
	{
		WeaponRaycastMulti weaponRaycast = this.activeWeapon.GetActiveWeapon();
		weaponRaycast.ammoCount = weaponRaycast.gunInfo[KeyInfo.maxAmmo];
		weaponRaycast.totalAmmo -= weaponRaycast.gunInfo[KeyInfo.maxAmmo];
		this.rigController.ResetTrigger("reload_weapon");
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, weaponRaycast);
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_TOTAL_AMMO, weaponRaycast);
		}
	}

	
	private void DoneReload()
	{
		this.isReloading = false;
	}

	
	public Animator rigController;

	
	public WeaponAnimationEventMulti animationEvent;

	
	public Transform leftHand;

	
	public ActiveWeaponMultiplayer activeWeapon;

	
	private GameObject magazineHand;

	
	public bool isReloading;
}
