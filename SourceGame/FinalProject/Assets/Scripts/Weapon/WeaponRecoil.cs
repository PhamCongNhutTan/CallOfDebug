
using Unity.Cinemachine;
using UnityEngine;


public class WeaponRecoil : MonoBehaviour
{
	public Animator weaponAnimator;
	
	private void Awake()
	{
		this.cameraShake = base.GetComponent<CinemachineImpulseSource>();
	}

	
	public void GenerateRecoil(string weaponName)
	{
		this.time = this.duration;
		if (this.cam.enabled)
		{
			this.cameraShake?.GenerateImpulse(characterAiming.mainCamera.transform.forward);
			this.rigController.Play("weapon_recoil_" + weaponName, 1, 0f);
			if (weaponAnimator!=null)
			{
                weaponAnimator?.SetTrigger("Fire");

            }
            this.duration = 0.5f;
		}
		else
		{
			this.duration = 0.25f;
		}
		this.horizontalRecoil = this.recoilPattern[this.index].x;
		this.verticleRecoil = this.recoilPattern[this.index].y;
		this.index = this.NextIndex(this.index);
	}

	
	public void Reset()
	{
		this.index = 0;
	}

	
	private int NextIndex(int index)
	{
		return (index + 1) % this.recoilPattern.Length;
	}

	
	private void Update()
	{
		if (this.time > 0f)
		{
			CharacterAiming characterAiming = this.characterAiming;
			characterAiming.yAxist.Value = characterAiming.yAxist.Value - this.verticleRecoil / 10f * Time.deltaTime / this.duration * this.recoilModifier;
			CharacterAiming characterAiming2 = this.characterAiming;
			characterAiming2.xAxist.Value = characterAiming2.xAxist.Value - this.horizontalRecoil / 10f * Time.deltaTime / this.duration * this.recoilModifier;
			this.time -= Time.deltaTime;
		}
	}

	
	public CharacterAiming characterAiming;

	
	[HideInInspector]
	public CinemachineImpulseSource cameraShake;

	
	[HideInInspector]
	public Animator rigController;

	
	private float verticleRecoil;

	
	private float horizontalRecoil;

	
	public float duration;

	
	public float time;

	
	private int index;

	
	public Vector2[] recoilPattern;

	
	public float recoilModifier;

	
	public Camera cam;
}
