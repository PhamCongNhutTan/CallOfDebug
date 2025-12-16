
using Photon.Pun;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : BaseManager<GameManager>
{

	public bool login = false;
	public string PlayerName { get; private set; } = "";
	public bool isAndroidDebugEditor = false;

	public void SetPlayerName(string name)
	{
		PlayerName = name;
		Debug.Log($"Player name set to: {name}");
	}

	private void Start()
	{
		Application.targetFrameRate = 60;
		PhotonNetwork.ConnectUsingSettings();
		if (BaseManager<UIManager>.HasInstance())
		{
			// UIManager.Instance.ShowScreen<MainMenu>(null, true);
			BaseManager<UIManager>.Instance.ShowScreen<LoginPanel>(null, true);
			PlayFabClientAPI.LoginWithCustomID(new PlayFab.ClientModels.LoginWithCustomIDRequest
			{
				CustomId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true,
				InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams
				{
					GetUserData = true,
					GetPlayerProfile = true
				}
			}, result =>
			{
				Debug.Log("Login successful");
				login = true;

				// Load player data from PlayFab
				if (BaseManager<DataManager>.HasInstance())
				{
					BaseManager<DataManager>.Instance.LoadPlayerDataFromPlayFab(
						result.InfoResultPayload.UserData,
						result.NewlyCreated
					);
				}

				if (result.NewlyCreated || string.IsNullOrEmpty(result.InfoResultPayload.PlayerProfile.DisplayName))
				{
					
				}
				else
				{
					if (result.InfoResultPayload.PlayerProfile != null && !string.IsNullOrEmpty(result.InfoResultPayload.PlayerProfile.DisplayName))
					{
						PlayerName = result.InfoResultPayload.PlayerProfile.DisplayName;
						PhotonNetwork.LocalPlayer.NickName = PlayerName;

						// Refresh MainMenu to show player name
						if (BaseManager<UIManager>.HasInstance() && BaseManager<UIManager>.Instance.CurScreen is MainMenu)
						{
							(BaseManager<UIManager>.Instance.CurScreen as MainMenu).RefreshPlayerName();
						}
					}
				}
			}, error =>
			{
				Debug.LogError("Login failed: " + error.GenerateErrorReport());
			});
		}
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
}
