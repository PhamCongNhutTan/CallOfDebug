
using UnityEngine;


public class BaseManager<T> : MonoBehaviour where T : BaseManager<T>
{
	

	public static T Instance
	{
		get
		{
			if (BaseManager<T>.instance == null)
			{
				BaseManager<T>.instance = UnityEngine.Object.FindObjectOfType<T>();
				if (BaseManager<T>.instance == null)
				{
					Debug.Log("No " + typeof(T).Name + " Singleton Instance");
				}
			}
			return BaseManager<T>.instance;
		}
	}

	
	public static bool HasInstance()
	{
		return BaseManager<T>.instance != null;
	}

	
	protected virtual void Awake()
	{
		this.CheckInstance();
	}

	
	protected bool CheckInstance()
	{
		if (BaseManager<T>.instance == null)
		{
			BaseManager<T>.instance = (T)((object)this);
			Object.DontDestroyOnLoad(this);
			return true;
		}
		if (BaseManager<T>.instance == this)
		{
			Object.DontDestroyOnLoad(this);
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	
	private static T instance;
}
