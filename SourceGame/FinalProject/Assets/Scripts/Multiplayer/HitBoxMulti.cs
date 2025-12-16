
using UnityEngine;
using Photon.Pun;

public class HitBoxMulti : MonoBehaviourPun
{
	
	public void OnHit(WeaponRaycastMulti weapon, Vector3 direction)
	{
		Debug.Log("Hitbox OnHit");
		float num = (float)weapon.gunInfo[KeyInfo.damage];
		if (base.tag.Equals("Head"))
		{
			num *= 2f;
		}
		photonView.RPC("RPCTakeDame", RpcTarget.All, weapon.shooter, num, direction, this.idRb, photonView.ViewID);
	}
	
	public PhotonView photonView;
	public int idRb;
	
	public HealthMultiplayer health;

	
	public Rigidbody rb;
}
