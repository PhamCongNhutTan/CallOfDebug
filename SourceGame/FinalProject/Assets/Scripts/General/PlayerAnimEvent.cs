
using UnityEngine;


public class PlayerAnimEvent : MonoBehaviour
{
	
	public void SEAudio(string name)
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.PlaySE(name, 0f);
		}
	}
}
