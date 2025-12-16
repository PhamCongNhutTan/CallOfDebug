
using UnityEngine;


[CreateAssetMenu]
public class GlobalConfig : ScriptableObject
{
	
	[Header("AI")]
	public int totalEnemy = 20;

	
	public float maxTime = 1f;

	
	public float maxDistance = 1f;

	
	public float maxHealth = 100f;

	
	public float blinkDuration = 0.1f;

	
	public float dieForce = 10f;

	
	public float maxSight = 5f;

	
	public float timeDestroyAI = 3f;

	
	public float shootingRange = 15f;

	
	[Header("Player")]
	public float jumpHeight = 3f;

	
	public float gravity = 20f;

	
	public float stepDown = 0.4f;

	
	public float airControl = 2.5f;

	
	public float jumpDamp = 0.5f;

	
	public float groundSpeed = 1.2f;

	
	public float pushPower = 2f;

	
	public float turnSpeed = 15f;

	
	public float defaultRecoil = 1f;

	
	public float aimRecoil = 0.3f;

	
	public float timeDestroyDroppedMagazine = 5f;

	
	public float maxCroissHairTargetDistance = 100f;

	
	public int maxBulletPoolSize = 30;

	
	public float maxEnergy = 10f;

	
	public float maxTimePlay = 15f;
	public float maxTimeDeathMath = 3f;
	public float countdown = 0.3f;

	
	[Header("UI")]
	public float loadingOverLapTime = 1f;
}
