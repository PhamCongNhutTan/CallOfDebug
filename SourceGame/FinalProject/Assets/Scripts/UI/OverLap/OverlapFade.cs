
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class OverlapFade : BaseOverlap
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
		string text = data as string;
		if (text != null)
		{
			if (text == "Main")
			{
				this.FadeShowMenu(1f);
				return;
			}
			if (!(text == "CampaignOffline"))
			{
				return;
			}
			this.FadeShowGameUI(1f);
		}
	}

	
	public void FadeShowMenu(float fadeTime)
	{
		this.imgFade.color = Color.black;
		this.SetAlpha(0f);
		Sequence sequence = DOTween.Sequence();
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.AttachBGMSource.Play();
			BaseManager<AudioManager>.Instance.StopGT();
		}
		sequence.Append(this.imgFade.DOFade(1f, fadeTime));
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowScreen<MainMenu>(null, true);
		}
		sequence.AppendInterval(fadeTime / 2f);
		sequence.Append(this.imgFade.DOFade(0f, fadeTime));
		sequence.OnComplete(delegate
		{
			this.OnFinish();
		});
	}

	
	public void FadeShowGameUI(float fadeTime)
	{
		this.imgFade.color = Color.black;
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.AttachBGMSource.Stop();
			BaseManager<AudioManager>.Instance.PlayGT();
		}
		this.SetAlpha(0f);
		Sequence sequence = DOTween.Sequence();
		Debug.Log("fade start");
		sequence.Append(this.imgFade.DOFade(1f, fadeTime));
		Debug.Log("Gameui");
		if (BaseManager<UIManager>.HasInstance())
		{
			BaseManager<UIManager>.Instance.ShowScreen<GameUI>(null, true);
		}
		sequence.AppendInterval(fadeTime / 2f);
		sequence.Append(this.imgFade.DOFade(0f, fadeTime));
		sequence.OnComplete(delegate
		{
			this.OnFinish();
		});
	}

	
	private void SetAlpha(float alp)
	{
		Color color = this.imgFade.color;
		color.a = alp;
		this.imgFade.color = color;
	}

	
	private void OnFinish()
	{

		this.Hide();
    }

	
	[SerializeField]
	private Image imgFade;
}
