
using UnityEngine;


public class HealthMultiplayer : MonoBehaviour
{
	
	private void Start()
	{
		this.OnStart();
	}

	
	public void TakeDamage(float amount, Vector3 direction, int idRb)
	{
		if (this.isDead)
		{
			return;
		}
		this.currentHealth -= amount;
		Debug.Log("cur " + currentHealth);
		if (this.currentHealth <= 0f)
		{
			this.currentHealth = 0f;
		}
		this.OnDamage(direction, idRb);
		if (this.currentHealth <= 0f)
		{
			this.isDead = true;
			this.Die(direction, idRb);
		}
	}

	
	public bool IsDead()
	{
		return this.currentHealth <= 0f;
	}

	
	private void Die(Vector3 direction, int idRb)
	{
		this.OnDeath(direction, idRb);
	}

	
	protected virtual void OnStart()
	{
	}

	
	protected virtual void OnDeath(Vector3 direction, int idRb)
	{
	}

	
	protected virtual void OnDamage(Vector3 direction, int idRb)
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
