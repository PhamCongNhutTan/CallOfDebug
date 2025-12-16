
using UnityEngine;


public class AIStateChasePlayer : AIState
{
	
	public AIStateID GetID()
	{
		return AIStateID.ChasePlayer;
	}

	
	public void Enter(AiAgent agent)
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			this.maxDistance = BaseManager<DataManager>.Instance.GlobalConfig.maxDistance;
			this.maxTime = BaseManager<DataManager>.Instance.GlobalConfig.maxTime;
			this.shootingRange = BaseManager<DataManager>.Instance.GlobalConfig.shootingRange;
		}
	}

	
	public void Update(AiAgent agent)
	{
		Vector3 vector = agent.transform.position + new Vector3(0f, 2f, 0f);
		Vector3 dir = agent.spinePlayerTransform.position - vector;
		Ray ray = new Ray(vector, dir.normalized);
		Debug.DrawRay(vector, dir, Color.red);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, dir.magnitude))
		{
			if (raycastHit.collider.gameObject.tag.Equals("Player"))
			{
				agent.navMeshAgent.destination = agent.playerTransform.position;
				agent.navMeshAgent.stoppingDistance = 10f;
				this.timer = 0f;
			}
			else
			{
				agent.navMeshAgent.stoppingDistance = 0f;
				if ((agent.navMeshAgent.destination - agent.transform.position).sqrMagnitude < 0.5f)
				{
					this.timer += Time.deltaTime;
				}
			}
		}
		if (dir.sqrMagnitude > this.maxDistance * this.maxDistance || this.timer > this.maxTime)
		{
			agent.stateMachine.ChangeState(AIStateID.Idle);
			return;
		}
		if (dir.sqrMagnitude < this.shootingRange * this.shootingRange)
		{
			agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
			return;
		}
	}

	
	public void Exit(AiAgent agent)
	{
	}

	
	private float timer;

	
	private float maxDistance;

	
	private float maxTime;

	
	private float shootingRange;
}
