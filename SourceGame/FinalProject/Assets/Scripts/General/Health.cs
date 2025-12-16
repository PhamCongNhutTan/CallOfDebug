
using UnityEngine;


public class Health : MonoBehaviour
{
	
	private void Start()
	{
		this.OnStart();
	}

	
	public void TakeDamage(float amount, Vector3 direction, Rigidbody rigidbody)
	{
		if (this.isDead)
		{
			return;
		}
		this.currentHealth -= amount;
		if (this.currentHealth <= 0f)
		{
			this.currentHealth = 0f;
		}
		this.OnDamage(direction, rigidbody);
		if (this.currentHealth <= 0f)
		{
			this.isDead = true;
			this.Die(direction, rigidbody);
		}
	}

	
	public bool IsDead()
	{
		return this.currentHealth <= 0f;
	}

	
	private void Die(Vector3 direction, Rigidbody rigidbody)
	{
		this.OnDeath(direction, rigidbody);
	}

	
	protected virtual void OnStart()
	{
	}

	
	protected virtual void OnDeath(Vector3 direction, Rigidbody rigidbody)
	{
	}

	
	protected virtual void OnDamage(Vector3 direction, Rigidbody rigidbody)
	{
	}

	
	protected virtual void OnHealth(float amount)
	{
	}

	
	protected float maxHealth;

	
	public float currentHealth;

	
	public string shooter;

	
	public bool isDead;
}
