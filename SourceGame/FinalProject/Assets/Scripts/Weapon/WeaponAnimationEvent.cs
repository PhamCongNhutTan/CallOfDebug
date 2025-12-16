
using UnityEngine;


public class WeaponAnimationEvent : MonoBehaviour
{
	
	public void OnAnimationEvent(string eventName)
	{
		this.WeaponAnimEvent.Invoke(eventName);
	}

	
	public void OnHolsterEvent(string eventName)
	{
		this.WeaponAnimEvent.Invoke(eventName);
	}

	
	public void OnAudioEvent(string audioName)
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			if (audioName.Equals("walk"))
			{
				audioName += Random.Range(0, 4);
			}
			BaseManager<AudioManager>.Instance.PlaySE(audioName, 0f);
		}
	}

	
	public _AnimationEvent WeaponAnimEvent = new _AnimationEvent();
}
