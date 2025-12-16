
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingGame : BaseNotify
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
		string a = data.ToString();
		if (a == "Main")
		{
			base.StartCoroutine(this.LoadScene("Main"));
			return;
		}
		if (!(a == "Campaign"))
		{
			return;
		}
		base.StartCoroutine(this.LoadScene("CampaignOffline"));
	}

	
	private IEnumerator LoadScene(string scene)
	{
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
		asyncOperation.allowSceneActivation = false;
		while (!asyncOperation.isDone)
		{
			this.loadingSlider.value = asyncOperation.progress;
			this.loadingPercentText.SetText(string.Format("LOADING: {0}%", asyncOperation.progress * 100f), true);
			if (asyncOperation.progress >= 0.9f)
			{
				this.loadingSlider.value = 1f;
				this.loadingPercentText.SetText(string.Format("LOADING: {0}%", this.loadingSlider.value * 100f), true);
				if (BaseManager<UIManager>.HasInstance())
				{
					BaseManager<UIManager>.Instance.ShowOverlap<OverlapFade>(scene, true);
				}
				
				yield return new WaitForSeconds(1f);
                asyncOperation.allowSceneActivation = true;
				this.Hide();
			}
			yield return null;
		}
		yield break;
	}

	
	public TextMeshProUGUI loadingPercentText;

	
	public Slider loadingSlider;
}
