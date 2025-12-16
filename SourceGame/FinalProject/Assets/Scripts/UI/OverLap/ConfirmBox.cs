
using UnityEngine;


public class ConfirmBox : BaseOverlap
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
	}

	
	public void OnCancelButton()
	{
		this.Hide();
	}

	
	public void OnConfirmButton()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}
}
