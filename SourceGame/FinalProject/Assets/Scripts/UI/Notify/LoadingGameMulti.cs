
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingGameMulti : BaseNotify
{
	
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
		base.StartCoroutine(LoadScene());
	}

	
	private IEnumerator LoadScene()
	{
		while ((PhotonNetwork.LevelLoadingProgress<1f))
		{
			Debug.Log(PhotonNetwork.LevelLoadingProgress);
			this.loadingSlider.value = PhotonNetwork.LevelLoadingProgress;

            this.loadingPercentText.SetText(string.Format("LOADING: {0}%", PhotonNetwork.LevelLoadingProgress * 100f), true);
			if (PhotonNetwork.LevelLoadingProgress >= 0.9f)
			{
				this.loadingSlider.value = 1f;
				this.loadingPercentText.SetText(string.Format("LOADING: {0}%", this.loadingSlider.value * 100f), true);
				this.Hide();
			}
			yield return null;
		}
		yield break;
	}

	
	public TextMeshProUGUI loadingPercentText;

	
	public Slider loadingSlider;
}
