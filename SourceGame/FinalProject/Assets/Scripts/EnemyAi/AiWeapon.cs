
using Unity.VisualScripting;
using UnityEngine;


public class AiWeapon : MonoBehaviour
{
	
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.aiHealth = base.GetComponent<AiHealth>();
	}

	
	public void Attack(Vector3 targetTransform)
	{
		if (this.aiHealth.isHitting)
		{
			return;
		}
		float magnitude = (targetTransform - this.weapon.raycastOrigin.position).magnitude;
		this.inAccurancy = magnitude / 30f;
		Vector3 vector = targetTransform + Random.insideUnitSphere * this.inAccurancy;
		this.timer -= Time.deltaTime;
		//Debug.DrawLine(this.weapon.raycastOrigin.position, vector, Color.blue);
		if (this.weapon.ammoCount <= 0)
		{
			this.weapon.StopFiring();
			this.animator.Play("Recharge");
			this.timer = 2f;
			this.weapon.ammoCount = this.weapon.gunInfo[KeyInfo.maxAmmo];
			return;
		}
		if (!this.weapon.isFiring && this.timer < 0f)
		{
			this.weapon.StartFiring();
		}
		this.weapon.UpdateWeapon(Time.deltaTime, vector);
	}

	
	public void DropWeapon()
	{
		this.weapon.transform.SetParent(null);
		this.weapon.GetComponent<BoxCollider>().enabled = true;
		this.weapon.GetComponent<BoxCollider>().isTrigger = false;
		this.weapon.AddComponent<Rigidbody>();
		this.weapon.GetComponent<Rigidbody>().useGravity = true;
	}

	
	public WeaponRaycast weapon;

	
	public float inAccurancy;

	
	public float timer;

	
	private Animator animator;

	
	private AiHealth aiHealth;
}
