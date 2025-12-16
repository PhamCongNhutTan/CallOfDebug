
using Photon.Pun;
using UnityEngine;


public class Ragdoll : MonoBehaviour
{
	
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.rigidbodies = base.GetComponentsInChildren<Rigidbody>();
		this.DeactiveRagdoll();
	}

    
    public void ActiveRagdoll()
	{
		this.animator.enabled = false;
		Rigidbody[] array = this.rigidbodies;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].isKinematic = false;
		}
	}

	
	public void DeactiveRagdoll()
	{
		this.animator.enabled = true;
		Rigidbody[] array = this.rigidbodies;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].isKinematic = true;
		}
	}

	
	public void ApplyForce(Vector3 force, Rigidbody rigidbody)
	{
		rigidbody.AddForce(force, ForceMode.VelocityChange);
	}

	
	private Animator animator;

	
	private Rigidbody[] rigidbodies;
}
