
using UnityEngine;


public class Bullet : MonoBehaviour
{
	

	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
	}

	
	public void Deactive()
	{
		this.isActive = false;
		base.gameObject.SetActive(false);
		this.initialPosition = Vector3.zero;
		this.initialVelocity = Vector3.zero;
		this.tracer.emitting = false;
		this.tracer.Clear();
	}

	
	public void Active(Vector3 position, Vector3 velocity)
	{
		this.isActive = true;
		base.gameObject.SetActive(true);
		this.time = 0f;
		this.initialPosition = position;
		this.initialVelocity = velocity;
		this.tracer.emitting = true;
		this.tracer.AddPosition(position);
	}

	
	public float time;

	
	public Vector3 initialPosition;

	
	public Vector3 initialVelocity;

	
	public TrailRenderer tracer;

	
	private bool isActive;
}
