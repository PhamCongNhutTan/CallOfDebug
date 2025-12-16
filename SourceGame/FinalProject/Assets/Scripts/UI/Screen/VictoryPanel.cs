
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;


public class VictoryPanel : BaseScreen
{
	
	public override void Hide()
	{
		base.Hide();
	}

	
	public override void Init()
	{
		base.Init();
        
        base.StartCoroutine(this.ReturnMenu());
	}

	
	public override void Show(object data)
	{
		base.Show(data);

	}

	
	public IEnumerator ReturnMenu()
	{
        if (AudioManager.HasInstance())
        {
            AudioManager.Instance.PlaySE("Victory"+Random.Range(0,	3));
        }
        yield return new WaitForSeconds(5f);
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowNotify<LoadingGame>("Main", true);
			BaseManager<UIManager>.Instance.HideAllPopups();
		}
		if (BaseManager<ObjectPool>.HasInstance())
		{
			Object.Destroy(BaseManager<ObjectPool>.Instance.gameObject);
		}
		if (BaseManager<CameraManager>.HasInstance())
		{
			Object.Destroy(BaseManager<CameraManager>.Instance.gameObject);
		}
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		yield break;
	}
}
