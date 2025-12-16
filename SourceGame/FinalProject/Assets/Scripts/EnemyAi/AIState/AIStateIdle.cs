
using UnityEngine;


public class AIStateIdle : AIState
{
	
	public AIStateID GetID()
	{
		return AIStateID.Idle;
	}

	
	public void Enter(AiAgent agent)
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			this.maxSightDistance = BaseManager<DataManager>.Instance.GlobalConfig.maxSight;
		}
		agent.navMeshAgent.destination = agent.transform.position;
		this.patrolPos = agent.transform.position;
		agent.navMeshAgent.speed = 3f;
		agent.navMeshAgent.isStopped = false;
	}

	
	public void Exit(AiAgent agent)
	{
	}

	
	public void Update(AiAgent agent)
	{
		agent.animator.SetFloat("Rotation", 0f);
		Vector3 destination = new Vector3(Random.insideUnitCircle.x * 10f + this.patrolPos.x, this.patrolPos.y, Random.insideUnitCircle.y * 10f + this.patrolPos.z);
		if ((agent.transform.position - agent.navMeshAgent.destination).sqrMagnitude < 0.5f)
		{
			agent.navMeshAgent.destination = destination;
		}
		Vector3 position = agent.weapon.weapon.raycastOrigin.position;
		Vector3 position2 = agent.spinePlayerTransform.position;
		this.playerDirection = position2 - position;
		if (this.playerDirection.magnitude > this.maxSightDistance)
		{
			return;
		}
		Vector3 forward = agent.transform.forward;
		float magnitude = this.playerDirection.magnitude;
		Ray ray = new Ray(position, this.playerDirection);
		Debug.DrawRay(position, playerDirection, Color.red);
		this.playerDirection.Normalize();
		float num = Vector3.Dot(this.playerDirection, forward);
		RaycastHit raycastHit;
		
		if (Physics.Raycast(ray, out raycastHit, magnitude) && num >= 0f && (raycastHit.collider.gameObject.tag.Equals("Player")))
        {
			if (magnitude > BaseManager<DataManager>.Instance.GlobalConfig.shootingRange) return;
            agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
		}	
    }

	
	private Vector3 playerDirection;

	
	private float maxSightDistance;

	
	private Vector3 patrolPos;
}
