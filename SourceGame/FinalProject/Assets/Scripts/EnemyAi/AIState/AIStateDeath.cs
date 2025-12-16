
using UnityEngine;


public class AIStateDeath : AIState
{
	
	public AIStateID GetID()
	{
		return AIStateID.Death;
	}

	
	public void Enter(AiAgent agent)
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			this.dieForce = BaseManager<DataManager>.Instance.GlobalConfig.dieForce;
		}
		agent.ragdoll.ActiveRagdoll();
		this.direction.y = 1f;
		agent.ragdoll.ApplyForce(this.direction * this.dieForce, this.rigidbody);
		agent.weapon.DropWeapon();
		agent.DisableAll();
	}

	
	public void Update(AiAgent agent)
	{
	}

	
	public void Exit(AiAgent agent)
	{
	}

	
	private float dieForce;

	
	public Vector3 direction;

	
	public Rigidbody rigidbody;
}
