
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class AiAgent : MonoBehaviour
{

	
	public AIStateID initState;



    
    [HideInInspector]
    public AiStateMachine stateMachine;

    
    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    
    public Transform spinePlayerTransform;

    
    public Transform playerTransform;

    
    [HideInInspector]
    public AiWeaponIK weaponIK;

    
    [HideInInspector]
    public Animator animator;

    
    [HideInInspector]
    public AiWeapon weapon;

    
    [HideInInspector]
    public Ragdoll ragdoll;

    
    [HideInInspector]
    public AiHealth health;

    
    public GameObject markUI;
    
    private void Awake()
	{
		this.weaponIK = base.GetComponent<AiWeaponIK>();
		this.animator = base.GetComponent<Animator>();
		this.weapon = base.GetComponent<AiWeapon>();
		this.ragdoll = base.GetComponent<Ragdoll>();
		this.health = base.GetComponent<AiHealth>();
		this.navMeshAgent = base.GetComponent<NavMeshAgent>();
	}

	
	private void Start()
	{
		if (this.spinePlayerTransform == null)
		{
			this.spinePlayerTransform = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>().spine;
			this.playerTransform = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>().transform;
		}
		this.stateMachine = new AiStateMachine(this);
		this.stateMachine.RegisterState(new AIStateIdle());
		this.stateMachine.RegisterState(new AIStateChasePlayer());
		this.stateMachine.RegisterState(new AIStateAttackPlayer());
		this.stateMachine.RegisterState(new AIStateDeath());
		this.stateMachine.ChangeState(this.initState);
	}

	
	private void Update()
	{
		this.stateMachine.Update();
	}

	
	public void DisableAll()
	{
		this.animator.enabled = false;
		this.weapon.enabled = false;
		this.weaponIK.enabled = false;
		this.ragdoll.enabled = false;
		this.navMeshAgent.enabled = false;
		this.markUI.gameObject.SetActive(false);
		base.StartCoroutine(this.health.OnDestroyWhenDeath());
	}

	
	public IEnumerator EnableAll()
	{
		yield return new WaitForSeconds(60f);
		this.animator.enabled = true;
		this.markUI.gameObject.SetActive(true);
		this.weapon.enabled = true;
		WeaponRaycast weaponRaycast = this.weapon.weapon;
		weaponRaycast.gameObject.transform.SetParent(base.transform, false);
		weaponRaycast.GetComponent<BoxCollider>().isTrigger = true;
		weaponRaycast.GetComponent<BoxCollider>().enabled = false;
		weaponRaycast.GetComponent<Rigidbody>().useGravity = false;
		this.weaponIK.enabled = true;
		this.ragdoll.enabled = true;
		this.ragdoll.DeactiveRagdoll();
		this.navMeshAgent.enabled = true;
		if (BaseManager<DataManager>.HasInstance())
		{
			this.health.currentHealth = BaseManager<DataManager>.Instance.GlobalConfig.maxHealth;
			this.health.isHitting = false;
			this.health.isDead = false;
		}
		this.stateMachine.ChangeState(this.initState);
		base.gameObject.SetActive(true);
		yield break;
	}

	
}
