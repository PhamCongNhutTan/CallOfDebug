
using System.Collections;
using UnityEngine;


public class PlayerHealth : Health
{
	
	protected override void OnDamage(Vector3 direction, Rigidbody rigidbody)
	{
		
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_HEALTH, this);
		}
	}

	
	protected override void OnDeath(Vector3 direction, Rigidbody rigidbody)
	{
		this.ragdoll.ActiveRagdoll();
		
		this.characterController.enabled = false;
		this.characterLocomotion.enabled = false;
		this.ragdoll.ApplyForce(direction, rigidbody);
		if (BaseManager<CameraManager>.HasInstance())
		{
			BaseManager<CameraManager>.Instance.TurnOffScope();
		}
        this.characterAiming.enabled = false;

        this.activeWeapon.DropWeapon();
		if (BaseManager<ListenerManager>.HasInstance())
		{
			string[] value = new string[]
			{
				this.shooter,
				base.gameObject.name
			};
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_ENEMY_KILL, value);
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_PLAYER_DEATH, null);
		}
		base.StartCoroutine(this.OnPlayerDeath());
	}

	
	public IEnumerator OnPlayerDeath()
	{
		yield return new WaitForSeconds(2f);
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowScreen<DefeatPanel>(null, true);
		}
		yield break;
	}

	
	protected override void OnHealth(float amount)
	{
	}

	
	protected override void OnStart()
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			this.maxHealth = BaseManager<DataManager>.Instance.GlobalConfig.maxHealth;
			this.currentHealth = this.maxHealth;
		}
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_HEALTH, this);
		}
		this.SetUp();
	}

	
	private void SetUp()
	{
		foreach (Rigidbody rigidbody in base.GetComponentsInChildren<Rigidbody>())
		{
			rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			HitBox hitBox = rigidbody.gameObject.AddComponent<HitBox>();
			hitBox.health = this;
			hitBox.rb = rigidbody;
			if (hitBox.gameObject != base.gameObject)
			{
				hitBox.gameObject.layer = LayerMask.NameToLayer("HitboxPlayer");
			}
		}
	}

	
	public Ragdoll ragdoll;

	
	public ActiveWeapon activeWeapon;

	
	public CharacterAiming characterAiming;

	
	public CharacterLocomotion characterLocomotion;

	
	public CharacterController characterController;

	
	public Transform spine;
}
