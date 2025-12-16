
using UnityEngine;
using UnityEngine.AI;


public class AILocomotion : MonoBehaviour
{
	
	private void Start()
	{
		this.navMeshAgent = base.GetComponent<NavMeshAgent>();
		this.animator = base.GetComponent<Animator>();
	}

	
	private void Update()
	{
		if (this.navMeshAgent.hasPath)
		{
			this.animator.SetFloat("Speed", this.navMeshAgent.velocity.magnitude);
			return;
		}
		this.animator.SetFloat("Speed", 0f);
	}

	
	private NavMeshAgent navMeshAgent;

	
	private Animator animator;
}
