
using UnityEngine;


public class AudioButton : MonoBehaviour
{
	
	public static void OnPointerEnter()
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.PlaySE("OnPointerEnter", 0f);
		}
	}

	
	public static void OnPointerDown()
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.PlaySE("OnPointerDown", 0f);
		}
	}
}
