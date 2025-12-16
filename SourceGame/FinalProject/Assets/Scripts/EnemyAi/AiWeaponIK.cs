
using UnityEngine;


public class AiWeaponIK : MonoBehaviour
{
	
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
	}

	
	public void AimTarget(Vector3 targetTransform)
	{
		Vector3 vector = targetTransform - (base.transform.position + new Vector3(0f, 2.2f, 0f));
		//Debug.DrawLine((base.transform.position + new Vector3(0f, 1.3f, 0f)), targetTransform,Color.red);
		float num = Vector3.Angle(base.transform.forward, vector);
		if (vector.y < 0f)
		{
			num = -num;
		}
		this.animator.SetFloat("Rotation", num / 10f);
		vector.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(vector);
		base.transform.rotation = rotation;
	}







	
	private Animator animator;


	
	public float rotationSpeed = 10f;

}
