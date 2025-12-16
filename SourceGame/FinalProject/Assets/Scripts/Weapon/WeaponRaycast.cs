
using System.Collections.Generic;
using UnityEngine;


public class WeaponRaycast : MonoBehaviour
{
	public ParticleSystem[] bodyHit;
	public ParticleSystem[] metalHit;
	public ParticleSystem shell;
    ParticleSystem[] hitEffect;
    
    private void Start()
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			this.gunInfo = BaseManager<DataManager>.Instance.GetInfo(this.weaponName);
		}
		this.ammoCount = this.gunInfo[KeyInfo.maxAmmo];
		this.totalAmmo = this.gunInfo[KeyInfo.maxAmmo] * 4;
		this.fireRate = (float)this.gunInfo[KeyInfo.fireRate];
		this.initFireRate = this.fireRate;
	}

	
	public void SingleShot(Vector3 target)
	{
		this.FireBullet(target);
	}

	
	public void StartFiring()
	{
		this.fireRate = this.initFireRate;
		this.isFiring = true;
		if (this.accumulatedTime > 0f)
		{
			this.accumulatedTime = 0f;
		}
		WeaponRecoil weaponRecoil = this.weaponRecoil;
		if (weaponRecoil == null)
		{
			return;
		}
		weaponRecoil.Reset();
	}

	
	public void UpdateWeapon(float deltaTime, Vector3 target)
	{
		if (this.isFiring)
		{
			this.UpdateFiring(deltaTime, target);
		}
		else
		{
			this.accumulatedTime += deltaTime;
		}
		this.UpdateBullets(deltaTime);
	}

	
	private void UpdateFiring(float deltaTime, Vector3 target)
	{
		this.accumulatedTime += deltaTime;
		float num = 1f / this.fireRate;
		while (this.accumulatedTime >= 0f)
		{
			this.FireBullet(target);
			this.accumulatedTime -= num;
		}
	}

	
	public void StopFiring()
	{
		this.isFiring = false;
	}

	
	public void UpdateBullets(float deltaTime)
	{
		if (!ObjectPool.HasInstance()) return;
		if (this.equipBy == EquipBy.Player)
		{
			BaseManager<ObjectPool>.Instance.poolObjects.ForEach(delegate(Bullet bullet)
			{
				Vector3 position = this.GetPosition(bullet);
				bullet.time += deltaTime;
				Vector3 position2 = this.GetPosition(bullet);
				this.RaycastSegment(position, position2, bullet);
			});
		}
		else
		{
			BaseManager<ObjectPool>.Instance.poolAiObjects.ForEach(delegate(Bullet bullet)
			{
				Vector3 position = this.GetPosition(bullet);
				bullet.time += deltaTime;
				Vector3 position2 = this.GetPosition(bullet);
				this.RaycastSegment(position, position2, bullet);
			});
		}
		this.DestroyBullets();
	}

	
	private void DestroyBullets()
	{
		if (this.equipBy == EquipBy.Player)
		{
			using (List<Bullet>.Enumerator enumerator = BaseManager<ObjectPool>.Instance.poolObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Bullet bullet = enumerator.Current;
					if (bullet.time > this.maxLifetime)
					{
						bullet.Deactive();
					}
				}
				return;
			}
		}
		foreach (Bullet bullet2 in BaseManager<ObjectPool>.Instance.poolAiObjects)
		{
			if (bullet2.time > this.maxLifetime)
			{
				bullet2.Deactive();
			}
		}
	}

	
	private void FireBullet(Vector3 target)
	{
		if (this.ammoCount <= 0)
		{
			this.isFiring = false;
			return;
		}
		this.ammoCount--;
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_AMMO, this);
		}
		if (shell != null)
		{
            shell.Emit(1);
        }

		foreach (ParticleSystem particleSystem in this.muzzleFlash)
		{
			particleSystem.Emit(particleSystem.maxParticles);
		}
		Vector3 velocity = (target - this.raycastOrigin.position).normalized * (float)this.gunInfo[KeyInfo.bulletSpeed];
		if (ObjectPool.HasInstance())
		{
            if (this.equipBy == EquipBy.Player)
            {
                BaseManager<ObjectPool>.Instance.GetPooledObject().Active(this.raycastOrigin.position, velocity);
            }
            else
            {
                BaseManager<ObjectPool>.Instance.GetPooledAiObject().Active(this.raycastOrigin.position, velocity);
            }
        }
		
		WeaponRecoil weaponRecoil = this.weaponRecoil;
		if (weaponRecoil != null)
		{
			weaponRecoil.GenerateRecoil(this.weaponName);
		}
		if (this.equipBy == EquipBy.AI && BaseManager<AudioManager>.HasInstance())
		{
			this.audio.Stop();
			this.audio.clip = BaseManager<AudioManager>.Instance.GetAudioClip("TommyShoot");
			this.audio.PlayOneShot(this.audio.clip);
		}
	}

	
	private Vector3 GetPosition(Bullet bullet)
	{
		Vector3 a = Vector3.down * this.bulletDrop;
		return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * a * bullet.time * bullet.time;
	}

	
	private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
	{
		Vector3 direction = end - start;
		float magnitude = direction.magnitude;
		this.ray.origin = start;
		this.ray.direction = direction;
		if (Physics.Raycast(this.ray, out this.hitInfo, magnitude, this.hitLayer))
		{

			if (hitInfo.collider.gameObject.CompareTag("AI") || hitInfo.collider.gameObject.CompareTag("Head"))
			{
				hitEffect = bodyHit;
			}
			else if (hitInfo.collider.gameObject.CompareTag("Metal"))
			{
				hitEffect = metalHit;
			}
			else hitEffect = hitCon;
 			foreach (var effect in hitEffect)
			{
                effect.transform.position = this.hitInfo.point;
                effect.transform.forward = this.hitInfo.normal;
                effect.Emit(effect.maxParticles);
            }
			
			bullet.transform.position = this.hitInfo.point;
			bullet.time = this.maxLifetime;
			end = this.hitInfo.point;
			Rigidbody component = this.hitInfo.collider.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddForceAtPosition(this.ray.direction * 10f, this.hitInfo.point, ForceMode.Impulse);
			}
			HitBox component2 = this.hitInfo.collider.GetComponent<HitBox>();
			if (component2)
			{
				component2.OnHit(this, this.ray.direction);
			}
		}
		bullet.transform.position = end;
	}

	
	public LayerMask hitLayer;

	
	public string weaponName;

	
	public ActiveWeapon.WeaponSlot weaponSlot;

	
	public bool isFiring;

	
	public float bulletDrop;

	
	public ParticleSystem[] muzzleFlash;

	
	public ParticleSystem[] hitCon;

	
	public Transform raycastOrigin;

	
	[HideInInspector]
	public Transform raycastDestination;

	
	private Ray ray;

	
	private RaycastHit hitInfo;

	
	private float accumulatedTime;

	
	private float maxLifetime = 3f;

	
	public int ammoCount;

	
	public int totalAmmo;

	
	public GameObject magazine;

	
	public GameObject meshWeapon;

	
	public WeaponRecoil weaponRecoil;

	
	public Dictionary<KeyInfo, int> gunInfo;

	
	public EquipBy equipBy;

	
	public List<ShootingMode> mode;

	
	public string shooter;

	
	public float fireRate;

	
	private float initFireRate;

	
	public AudioSource audio;
}
