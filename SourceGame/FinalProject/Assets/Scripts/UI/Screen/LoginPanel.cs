using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BaseScreen
{
    public InputField name;
    #region panel
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnLoginButton();
        }
    }
    public void OnLoginButton()
    {
        string playerName = name.text;
        
        if (playerName != "")
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            
            // Update GameManager PlayerName
            if (BaseManager<GameManager>.HasInstance())
            {
                BaseManager<GameManager>.Instance.SetPlayerName(playerName);
            }
            
            PlayFabClientAPI.UpdateUserTitleDisplayName(new PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest
            {
                DisplayName = playerName
            }, result =>
            {
                Debug.Log("Display name updated successfully");
                
                // Save initial player data to PlayFab for new user
                if (BaseManager<DataManager>.HasInstance())
                {
                    BaseManager<DataManager>.Instance.SavePlayerData();
                }
                UIManager.Instance.ShowScreen<MainMenu>(null, true);
                // Refresh MainMenu if it's showing
                if (BaseManager<UIManager>.HasInstance() && BaseManager<UIManager>.Instance.CurScreen is MainMenu)
                {
                    (BaseManager<UIManager>.Instance.CurScreen as MainMenu).RefreshPlayerName();
                }
            }, error =>
            {
                Debug.LogError("Failed to update display name: " + error.GenerateErrorReport());
            });
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
        name.Select();
    }

    public override void Show(object data)
    {
        base.Show(data);
    }
    #endregion

}

