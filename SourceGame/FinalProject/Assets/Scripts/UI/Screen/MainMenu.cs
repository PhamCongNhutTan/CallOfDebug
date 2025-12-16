using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : BaseScreen
{
	public Text name;
	public GameObject content;
	public GameObject tutorial;

	public override void Hide()
	{
		base.Hide();
	}


	public override void Init()
	{
		base.Init();
	}


	public override void Show(object data)
	{
		base.Show(data);
		RefreshPlayerName();
	}

	public void RefreshPlayerName()
	{
		// Try to get name from GameManager first, then PhotonNetwork
		if (BaseManager<GameManager>.HasInstance() && !string.IsNullOrEmpty(BaseManager<GameManager>.Instance.PlayerName))
		{
			name.text = BaseManager<GameManager>.Instance.PlayerName;
		}
		else if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
		{
			name.text = PhotonNetwork.NickName;
		}
		else
		{
			name.text = "Player"; // Fallback
		}
	}

	public void OnTutorialButton(bool check)
	{
		tutorial.SetActive(check);
		content.SetActive(!check);
	}

	public void OnCampaignButton()
	{
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowNotify<LoadingGame>("Campaign".ToString(), true);
		}
		this.Hide();
	}


	public void OnMissionButton()
	{
		if (BaseManager<UIManager>.HasInstance())
		{
			if (BaseManager<UIManager>.Instance.CurPopup is MissionPanel && !BaseManager<UIManager>.Instance.CurPopup.IsHide)
			{
				return;
			}
			BaseManager<UIManager>.Instance.ShowPopup<MissionPanel>(null, true);
		}
	}
	public void OnMultiplayerButton()
	{
		PhotonNetwork.JoinLobby();
	}

	public void OnExitButton()
	{
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowOverlap<ConfirmBox>(null, false);
		}
	}
}
