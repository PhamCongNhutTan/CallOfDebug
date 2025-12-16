
using UnityEngine;
using UnityEngine.Events;


public class WeaponEvent : MonoBehaviour
{
	
	private void Awake()
	{
		if (weaponEvent != null)
		{
            this.weaponEvent.WeaponAnimEvent.AddListener(new UnityAction<string>(this.OnHolsterEvent));
        }
		
	}

	
	private void OnHolsterEvent(string eventName)
	{
		if (eventName == "holster_weapon")
		{
			this.holster_weapon();
		}
	}

	
	private void holster_weapon()
	{
		bool @bool = this.activeWeapon.rigController.GetBool("holster_weapon");
		WeaponRaycast weaponRaycast = this.activeWeapon.GetActiveWeapon();
		MeshRenderer component = weaponRaycast.gameObject.GetComponent<MeshRenderer>();
		int weaponSlot = (int)weaponRaycast.weaponSlot;
		if (@bool)
		{
			if (component != null)
			{
				component.enabled = false;
			}
			weaponRaycast.meshWeapon.SetActive(false);
			this.magMesh[weaponSlot].SetActive(true);
			return;
		}
		if (component != null)
		{
			component.enabled = true;
		}
		weaponRaycast.meshWeapon.SetActive(true);
		this.magMesh[weaponSlot].SetActive(false);
	}

	
	public ActiveWeapon activeWeapon;

	
	public WeaponAnimationEvent weaponEvent;

	
	public GameObject[] magMesh;
}
