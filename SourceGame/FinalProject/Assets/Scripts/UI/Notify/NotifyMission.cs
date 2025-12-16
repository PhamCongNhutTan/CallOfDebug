
using UnityEngine.UI;


public class NotifyMission : BaseNotify
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
			this.notifyText.text = "Done " + text;
		}
		base.Invoke("Hide", 5f);
	}

	
	public Text notifyText;
}
