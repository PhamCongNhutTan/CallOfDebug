
using UnityEngine;


public class AIStateAttackPlayer : AIState
{
	
	public void Enter(AiAgent agent)
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			this.maxDistance = BaseManager<DataManager>.Instance.GlobalConfig.maxDistance;
			this.maxTime = BaseManager<DataManager>.Instance.GlobalConfig.maxTime;
			this.shootingRange = BaseManager<DataManager>.Instance.GlobalConfig.shootingRange;
			this.playerHealth = UnityEngine.Object.FindObjectOfType<PlayerHealth>();
		}
		if (agent.navMeshAgent!= null)
		{
            agent.navMeshAgent.speed = 6f;
            agent.navMeshAgent.destination = agent.transform.position;
        }
		
	}

	
	public void Exit(AiAgent agent)
	{
	}

	
	public AIStateID GetID()
	{
		return AIStateID.AttackPlayer;
	}

	
	public void Update(AiAgent agent)
	{
		if (!agent.navMeshAgent.enabled)
		{
			return;
		}
		Vector3 position = agent.weapon.weapon.raycastOrigin.position;
		Vector3 direction = agent.spinePlayerTransform.position - position;
		Ray ray = new Ray(position, direction);
		float sqrMagnitude = direction.sqrMagnitude;
		if (sqrMagnitude < this.shootingRange * this.shootingRange)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, sqrMagnitude))
			{
				if (raycastHit.collider.gameObject.tag.Equals("Player"))
				{
					agent.weaponIK.AimTarget(agent.spinePlayerTransform.position);
					agent.weapon.Attack(agent.spinePlayerTransform.position);
					agent.navMeshAgent.destination = agent.playerTransform.position;
					agent.navMeshAgent.stoppingDistance = 20f;
					this.timer = 0f;
				}
				else
				{
					agent.navMeshAgent.isStopped = false;
					agent.navMeshAgent.stoppingDistance = 0f;
					if ((agent.navMeshAgent.destination - agent.transform.position).sqrMagnitude < 0.5f)
					{
						this.timer += Time.deltaTime;
					}
				}
				if (this.playerHealth != null && this.playerHealth.IsDead())
				{
					agent.stateMachine.ChangeState(AIStateID.Idle);
				}
			}
		}
		else
		{
			agent.navMeshAgent.stoppingDistance = 0f;
			if ((agent.navMeshAgent.destination - agent.transform.position).sqrMagnitude < 0.5f)
			{
				this.timer += Time.deltaTime;
			}
		}
		if (sqrMagnitude > this.maxDistance * this.maxDistance || this.timer > this.maxTime)
		{
			agent.stateMachine.ChangeState(AIStateID.Idle);
		}
		if (!agent.navMeshAgent.enabled)
		{
			return;
		}
		if (agent.weapon.timer > 0f)
		{
			agent.navMeshAgent.isStopped = true;
			return;
		}
		agent.navMeshAgent.isStopped = false;
	}

	
	private float timer;

	
	private float maxDistance;

	
	private float maxTime;

	
	private float shootingRange;

	
	private PlayerHealth playerHealth;
}
