
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;


public class VictoryPanelMulti : BaseScreen
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
			AudioManager.Instance.PlaySE("Victory" + Random.Range(0, 3));
		}
		base.StartCoroutine(this.ReturnMenu());
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

	
	public IEnumerator ReturnMenu()
	{
		yield return new WaitForSeconds(5f);
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowNotify<LoadingGame>("Main", true);
			BaseManager<UIManager>.Instance.HideAllPopups();
		}
		PhotonNetwork.LocalPlayer.CustomProperties.Clear();
		PhotonNetwork.LeaveRoom();
		if (MultiplayerManager.HasInstance())
		{
			Destroy(MultiplayerManager.Instance.gameObject);
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
