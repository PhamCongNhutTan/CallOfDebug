
using UnityEngine;
using Photon.Pun;

public class WeaponAnimationEventMulti : MonoBehaviourPun
{
	public AudioSource playerAudio;
	public AudioSource weaponAudio;
	
	public void OnAnimationEvent(string eventName)
	{
		if (!photonView.IsMine || photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber) { return; }
		this.WeaponAnimEvent.Invoke(eventName);
	}

	
	public void OnHolsterEvent(string eventName)
	{
        //if (!photonView.IsMine || photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber) { return; }
        this.WeaponAnimEvent.Invoke(eventName);
	}

	
	public void OnAudioEvent(string audioName)
	{
		if (audioName.Equals("walk"))
		{
			audioName += Random.Range(0, 4);
            if (BaseManager<AudioManager>.HasInstance())
            {
                playerAudio.PlayOneShot(BaseManager<AudioManager>.Instance.GetAudioClip(audioName));
            }
        }
		else
		{
            weaponAudio.PlayOneShot(BaseManager<AudioManager>.Instance.GetAudioClip(audioName));
        }
	}

	
	public _AnimationEvent WeaponAnimEvent = new _AnimationEvent();
}
