
using Photon.Pun;
using UnityEngine;


public class GameMenu : BasePopup
{
	
	public override void Hide()
	{
		base.Hide();
	}

	
	public override void Init()
	{
		base.Init();
        this.aiming = UnityEngine.Object.FindObjectOfType<CharacterAiming>();

        //this.Hide();
    }

	
	public override void Show(object data)
	{
		base.Show(data);
        
    }

	
	public void OnLeaveGameButton()
	{
		this.Hide();
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
	}

	
	public void OnResumeButton()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;
		if (this.aiming != null)
		{
			this.aiming.isEcs = false;
		}
		this.Hide();
	}

	
	public void OnExitButton()
	{
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowOverlap<ConfirmBox>(null, true);
		}
	}

	
	public CharacterAiming aiming;


}
