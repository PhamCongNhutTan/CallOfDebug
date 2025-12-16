

using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WeaponRaycastMulti : MonoBehaviourPun
{
    public ObjectPoolMulti pool;
    public BulletMulti curBullet;
    public Animator weaponAnimator;
    public ParticleSystem shell;
    
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
        WeaponRecoilMulti weaponRecoil = this.weaponRecoil;
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
            //photonView.RPC("FireBullet", RpcTarget.All, target);
            this.accumulatedTime -= num;
        }
    }

    
    public void StopFiring()
    {
        this.isFiring = false;
    }

    
    public void UpdateBullets(float deltaTime)
    {

        if (this.equipBy == EquipBy.Player)
        {
            pool.poolObjects.ForEach(delegate (BulletMulti bullet)
            {
                Vector3 position = this.GetPosition(bullet);
                bullet.time += deltaTime;
                Vector3 position2 = this.GetPosition(bullet);
                //photonView.RPC("RaycastSegment", RpcTarget.All, position, position2, bullet);
                this.RaycastSegment(position, position2, bullet);
            });
        }
        //photonView.RPC("DestroyBullets", RpcTarget.All);
        this.DestroyBullets();
    }

    

    private void DestroyBullets()
    {
        if (this.equipBy == EquipBy.Player)
        {
            //using (List<BulletMulti>.Enumerator enumerator = pool.poolObjects.GetEnumerator())
            //{
            //	while (enumerator.MoveNext())
            //	{
            //		BulletMulti bullet = enumerator.Current;
            //		if (bullet.time > this.maxLifetime)
            //		{
            //			//bullet.Deactive(raycastOrigin.position);

            //			photonView.RPC("Deactive",RpcTarget.All, pool.poolObjects.FindInstanceID(bullet), raycastOrigin.position);
            //		}
            //	}
            //	return;
            //}
            int ind = pool.GetIndexPooledObjectDeactive();
            if (ind >= 0)
            {
                photonView.RPC("Deactive", RpcTarget.All, ind, raycastOrigin.position);
            }
        }
    }

    
    //[PunRPC]
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
        //foreach (ParticleSystem particleSystem in this.muzzleFlash)
        //{
        //	particleSystem.Emit(particleSystem.maxParticles);
        //}
        photonView.RPC("EmitMuzzle", RpcTarget.All);
        Vector3 velocity = (target - this.raycastOrigin.position).normalized * (float)this.gunInfo[KeyInfo.bulletSpeed];
        {
            if (this.equipBy == EquipBy.Player)
            {
                //pool.GetPooledObject().Active(this.raycastOrigin.position, velocity);

                int ind = pool.GetIndexPooledObject();
                if(ind >= 0)
                {
                    photonView.RPC("Active", RpcTarget.All,ind, this.raycastOrigin.position, velocity);
                }
                
            }
        }

        WeaponRecoilMulti weaponRecoil = this.weaponRecoil;
        if (weaponRecoil != null)
        {
            weaponRecoil.GenerateRecoil(this.weaponName);
        }
    }
    
    private Vector3 GetPosition(BulletMulti bullet)
    {
        Vector3 a = Vector3.down * this.bulletDrop;
        return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * a * bullet.time * bullet.time;
    }

    

    private void RaycastSegment(Vector3 start, Vector3 end, BulletMulti bullet)
    {
        Vector3 direction = end - start;
        float magnitude = direction.magnitude;
        this.ray.origin = start;
        this.ray.direction = direction;
        if (Physics.Raycast(this.ray, out this.hitInfo, magnitude, this.hitLayer))
        {
            
            if (hitInfo.collider.gameObject.CompareTag("Player")|| hitInfo.collider.gameObject.CompareTag("Head"))
            {
                CurHitEffect = hitBody;
            }
            else if (hitInfo.collider.gameObject.CompareTag("Metal"))
            {
                CurHitEffect = hitMetal;
            }
            else
            {
                CurHitEffect = hitEffectConcrete;
            }
            foreach (var effect in CurHitEffect)
            {
                effect.transform.position = this.hitInfo.point;
                effect.transform.forward = this.hitInfo.normal;
                effect.Emit(effect.maxParticles);
            }

            bullet.transform.position = this.hitInfo.point;
            bullet.time = this.maxLifetime-1f;
            end = this.hitInfo.point;
            Rigidbody component = this.hitInfo.collider.GetComponent<Rigidbody>();
            if (component)
            {
                component.AddForceAtPosition(this.ray.direction * 10f, this.hitInfo.point, ForceMode.Impulse);
            }
            HitBoxMulti component2 = this.hitInfo.collider.GetComponent<HitBoxMulti>();
            if (component2)
            {
                component2.OnHit(this, this.ray.direction);
            }
        }
        bullet.transform.position = end;
    }
    public ParticleSystem[] CurHitEffect;
    
    public LayerMask hitLayer;

    
    public string weaponName;

    
    public ActiveWeaponMultiplayer.WeaponSlot weaponSlot;

    
    public bool isFiring;

    
    public float bulletDrop;

    
    public ParticleSystem[] muzzleFlash;

    
    public ParticleSystem[] hitEffectConcrete;
    public ParticleSystem[] hitBody;
    public ParticleSystem[] hitMetal;

    
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

    
    public WeaponRecoilMulti weaponRecoil;

    
    public Dictionary<KeyInfo, int> gunInfo;

    
    public EquipBy equipBy;

    
    public List<ShootingMode> mode;

    
    public string shooter;

    
    public float fireRate;

    
    private float initFireRate;

    
    public AudioSource audio;
}
