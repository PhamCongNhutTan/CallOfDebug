
using UnityEngine;


public class HitBox : MonoBehaviour
{
	
	public void OnHit(WeaponRaycast weapon, Vector3 direction)
	{
		float num = (float)weapon.gunInfo[KeyInfo.damage];
		if (base.tag.Equals("Head"))
		{
			num *= 2f;
		}
		this.health.shooter = weapon.shooter;
		this.health.TakeDamage(num, direction, this.rb);
	}

	
	public Health health;

	
	public Rigidbody rb;
}
