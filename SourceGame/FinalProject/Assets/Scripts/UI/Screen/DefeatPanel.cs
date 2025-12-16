
using System.Collections;
using UnityEngine;


public class DefeatPanel : BaseScreen
{
	
	public override void Hide()
	{
		base.Hide();
	}

	
	public override void Init()
	{
		base.Init();
        if (AudioManager.HasInstance())
        {
            AudioManager.Instance.PlaySE("Defeat" + Random.Range(0, 2));
        }
        base.StartCoroutine(this.ReturnMenu());
	}

	
	public IEnumerator ReturnMenu()
	{
		yield return new WaitForSeconds(5f);
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowNotify<LoadingGame>("Main", false);
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

	
	public override void Show(object data)
	{
		base.Show(data);
	}
}
