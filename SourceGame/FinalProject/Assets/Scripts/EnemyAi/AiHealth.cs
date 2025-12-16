
using System.Collections;
using UnityEngine;


public class AiHealth : Health
{
    
    public void OnEndHitAnim()
    {
        this.aiAgent.navMeshAgent.enabled = true;
        this.isHitting = false;
    }

    
    protected override void OnDamage(Vector3 direction, Rigidbody rigidbody)
    {
        if (currentHealth <= 0)
        {
            ListenerManager.Instance.BroadCast(ListenType.ON_ENEMY_HIT, true);
        }
        else
        {
            ListenerManager.Instance.BroadCast(ListenType.ON_ENEMY_HIT);
        }
        this.aiAgent.navMeshAgent.enabled = false;
        this.isHitting = true;
        this.aiAgent.animator.Play("Gethit");
    }

    
    protected override void OnDeath(Vector3 direction, Rigidbody rigidbody)
    {
        AIStateDeath aistateDeath = this.aiAgent.stateMachine.GetState(AIStateID.Death) as AIStateDeath;
        aistateDeath.direction = direction;
        aistateDeath.rigidbody = rigidbody;
        if (BaseManager<ListenerManager>.HasInstance())
        {
            string[] value = new string[]
            {
                this.shooter,
                base.gameObject.name
            };
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_ALLY_KILL, value);
        }
        this.aiAgent.stateMachine.ChangeState(AIStateID.Death);
    }

    
    public IEnumerator OnDestroyWhenDeath()
    {
        yield return new WaitForSeconds(this.timeDestroyAI);
        base.gameObject.SetActive(false);
        yield break;
    }

    
    protected override void OnStart()
    {
        this.aiAgent = base.GetComponent<AiAgent>();
        if (BaseManager<DataManager>.HasInstance())
        {
            this.maxHealth = BaseManager<DataManager>.Instance.GlobalConfig.maxHealth;
            this.timeDestroyAI = BaseManager<DataManager>.Instance.GlobalConfig.timeDestroyAI;
        }
        this.currentHealth = this.maxHealth;
        this.SetUp();
    }

    
    private void SetUp()
    {
        foreach (Rigidbody rigidbody in base.GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            HitBox hitBox = rigidbody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
            hitBox.rb = rigidbody;
            if (hitBox.gameObject != base.gameObject)
            {
                hitBox.gameObject.layer = LayerMask.NameToLayer("HitboxEnemy");
            }
        }
    }

    
    private float timeDestroyAI;

    
    private AiAgent aiAgent;

    
    public bool isHitting;
}
