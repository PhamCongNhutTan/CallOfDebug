
using UnityEngine;


public class CrossHairTarget : MonoBehaviour
{
	
	private void Awake()
	{
		transform.SetParent(mainCamera.transform);
	}

	
	private void FixedUpdate()
	{
		this.ray.origin = this.mainCamera.transform.position;
		this.ray.direction = this.mainCamera.transform.forward;
		if (Physics.Raycast(this.ray, out this.hitInfo))
		{
			base.transform.position = this.hitInfo.point;
			return;
		}
		base.transform.position = this.ray.GetPoint(500f);
	}

	
	public Camera mainCamera;

	
	private Ray ray;

	
	private RaycastHit hitInfo;
}
