
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MARK
{
    PLAYER,
    TEAMATE,
    ENEMY
}

public class PlayerHealthMultiplayer : HealthMultiplayer
{
	public Rigidbody[] m_Rigidbodies;
    public string layerHitbox;
	public bool team;
	public GameObject[] mark;
	
    
    protected override void OnDamage(Vector3 direction, int idRb)
	{
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_HEALTH, this);
		}
	}

	
	protected override void OnDeath(Vector3 direction, int idRb)
	{
		this.ragdoll.ActiveRagdoll();
        this.ragdoll.ApplyForce(direction, m_Rigidbodies[idRb]);
		if (activeWeapon.photonView.IsMine || activeWeapon.photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
		{
            this.characterController.enabled = false;
            this.characterLocomotion.enabled = false;

            if (BaseManager<CameraManager>.HasInstance())
            {
                BaseManager<CameraManager>.Instance.TurnOffScope();
            }
            else
            {
                characterAiming.camManager.TurnOffScope();
            }
			if (AudioManager.HasInstance())
			{
				if (m_Rigidbodies[idRb].gameObject.CompareTag("Head"))
				{
                    AudioManager.Instance.PlaySE("HeadshotHit");
                }
				else
				AudioManager.Instance.PlaySE("Death");
			}
			this.characterAiming.isDeath = true;
			if (UIManager.HasInstance())
			{
				UIManager.Instance.HideAllPopups();
			}
            BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_PLAYER_DEATH, null);
        }
		

        this.activeWeapon.DropWeapon();
		if (BaseManager<ListenerManager>.HasInstance())
		{
			string[] value = new string[]
			{
				this.shooter,
				base.gameObject.name
			};
			if (MultiplayerManager.HasInstance())
			{
				if(team != MultiplayerManager.Instance.curTeam)
				{
					ListenerManager.Instance.BroadCast(ListenType.ON_ALLY_KILL, value);
				}
				else
				{
                    BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_ENEMY_KILL, value);
                }
			}
			
			
		}
		base.StartCoroutine(this.OnPlayerDeath());
	}
	public IEnumerator OnPlayerDeath()
	{
		if (shooter == PhotonNetwork.NickName)
		{
			if (AudioManager.HasInstance())
			{
				AudioManager.Instance.PlaySE("targetdown");
			}
		}
		if (MultiplayerManager.HasInstance())
		{
			MultiplayerManager.Instance.updateScore(this);
		}
		yield return new WaitForSeconds(5f);
		isDead = false;
		if (characterAiming.photonView.IsMine&&characterAiming.photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
		{
            if (MultiplayerManager.HasInstance())
            {
				MultiplayerManager.Instance.SpawnPlayer(team);
            }
        }
		Destroy(characterAiming.mainCamera.gameObject);
		Destroy(this.gameObject);
	}
	protected override void OnHealth(float amount)
	{
	}
	protected override void OnStart()
	{
		
		if (BaseManager<DataManager>.HasInstance())
		{
			this.maxHealth = BaseManager<DataManager>.Instance.GlobalConfig.maxHealth;
			this.currentHealth = this.maxHealth;
		}

		if (MultiplayerManager.HasInstance())
		{
			MultiplayerManager.Instance.JoinTeam(this);
			
		}
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_HEALTH, this);
		}
		this.SetUp();
	}

	
	private void SetUp()
	{
		m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
		PhotonView photonView = GetComponent<PhotonView>();
		for (int i = 0; i < m_Rigidbodies.Length; i++)
		{
			m_Rigidbodies[i].collisionDetectionMode = CollisionDetectionMode.Continuous;
			HitBoxMulti hitBox = m_Rigidbodies[i].gameObject.AddComponent<HitBoxMulti>();
			hitBox.idRb = i;
			hitBox.photonView = photonView;
			hitBox.health = this;
			hitBox.rb = m_Rigidbodies[i];
			//if (view.IsMine && view.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber) layerHitbox = "HitboxPlayer";
            if (hitBox.gameObject != base.gameObject)
			{
				hitBox.gameObject.layer = LayerMask.NameToLayer(layerHitbox);
			}
		}
	}
	
	
	public RagdollMulti ragdoll;

	
	public ActiveWeaponMultiplayer activeWeapon;

	
	public CharacterAimingMultiplayer characterAiming;

	
	public CharacterLocomotionMultiplayer characterLocomotion;

	
	public CharacterController characterController;

	
	public Transform spine;
}
