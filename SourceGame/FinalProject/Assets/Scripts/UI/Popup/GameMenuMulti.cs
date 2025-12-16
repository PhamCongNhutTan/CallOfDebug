
using Photon.Pun;
using UnityEngine;


public class GameMenuMulti : BasePopup
{
	
	public override void Hide()
	{
        //if (ListenerManager.HasInstance())
        //{
        //    ListenerManager.Instance.Unregister(ListenType.ON_PLAYER_DEATH,ResumeGame);
        //}
        base.Hide();
        
    }

	
	public override void Init()
	{
		base.Init();
		
		//this.Hide();
	}

	
	public override void Show(object data)
	{
		base.Show(data);
        CharacterAimingMultiplayer[] characterAimingMultiplayers = Object.FindObjectsOfType<CharacterAimingMultiplayer>();
        foreach (CharacterAimingMultiplayer aim in characterAimingMultiplayers)
        {
            if (aim.photonView.IsMine && aim.photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                characterAimingMultiplayer = aim;
                photon = characterAimingMultiplayer.photonView; break;
            }


        }
		//if (ListenerManager.HasInstance())
		//{
		//	ListenerManager.Instance.Register(ListenType.ON_PLAYER_DEATH, ResumeGame);
		//}
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
		if (MultiplayerManager.HasInstance())
		{
			Destroy(MultiplayerManager.Instance.gameObject);
		}
		if (characterAimingMultiplayer!= null)
		{
            if (!photon.IsMine) { return; }
            if (photon.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }
			PhotonNetwork.LocalPlayer.CustomProperties.Clear();
			PhotonNetwork.LeaveRoom();
        }
	}

	
	public void ResumeGame(object? data)
	{
		OnResumeButton();
	}
	public void OnResumeButton()
	{
        if (!photon.IsMine) { return; }
        if (photon.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;
		if (characterAimingMultiplayer != null)
		{
			characterAimingMultiplayer.isEcs = false;
		}
		this.Hide();
	}

	
	public void OnExitButton()
	{
        if (!photon.IsMine) { return; }
        if (photon.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowOverlap<ConfirmBox>(null, true);
		}
	}

	
	private PhotonView photon;
	public CharacterAimingMultiplayer characterAimingMultiplayer;
}
