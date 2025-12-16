
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;


public class DefeatPanelMulti : BaseScreen
{
    public TextMeshProUGUI allyScore;
    public TextMeshProUGUI enemyScore;
    
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
			BaseManager<UIManager>.Instance.ShowNotify<LoadingGame>("Main", true);
			BaseManager<UIManager>.Instance.HideAllPopups();
		}
        if (MultiplayerManager.HasInstance())
        {
            Destroy(MultiplayerManager.Instance.gameObject);
        }
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
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
        if (data is int[] values)
        {
            allyScore.SetText(values[0].ToString());
            enemyScore.SetText(values[1].ToString());
        }
    }
}
