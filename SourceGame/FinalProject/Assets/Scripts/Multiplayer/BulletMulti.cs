
using UnityEngine;


public class BulletMulti : MonoBehaviour
{
	//public GameObject sp;
	//public Vector3 preTF = new Vector3(99,99,99);
	

	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
	}
    //private void Update()
    //{
		
    //}
    //private void Update()
    //{
    //      if ( preTF.Equals (new Vector3(99, 99, 99)))
    //      {
    //	return;
    //      }
    //if (Vector3.Distance(preTF,transform.position)<=0.1f)
    //{
    //          this.tracer.emitting = false;
    //	sp.SetActive(false);
    //      }
    //else
    //{
    //	this.tracer.emitting = true;
    //	sp.SetActive(true);
    //}
    //preTF = transform.position;
    //}
    
    public void Deactive(Vector3 pos)
	{
		this.isActive = false;
		base.gameObject.SetActive(false);
		this.initialPosition = pos;
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

	
	public bool isActive= true;
}
