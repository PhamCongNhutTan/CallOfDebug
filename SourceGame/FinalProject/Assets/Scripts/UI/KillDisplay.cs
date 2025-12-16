
using UnityEngine;
using UnityEngine.UI;


public class KillDisplay : MonoBehaviour
{
	
	public void OnEnemyKill(string killer, string dead)
	{
		this.allyName.text = dead;
		this.enemyName.text = killer;
		this.allyName.gameObject.transform.SetAsLastSibling();
		this.enemyName.gameObject.transform.SetAsFirstSibling();
	}

	
	public void OnAllyKill(string killer, string dead)
	{
		this.allyName.text = killer;
		this.enemyName.text = dead;
		this.allyName.gameObject.transform.SetAsFirstSibling();
		this.enemyName.gameObject.transform.SetAsLastSibling();
	}

	
	public Text allyName;

	
	public Text enemyName;

	
	public int orderNum;
}
