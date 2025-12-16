using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
	

	public Dictionary<string, BaseScreen> Screens
	{
		get
		{
			return this.screens;
		}
	}

	

	public Dictionary<string, BasePopup> Popups
	{
		get
		{
			return this.popups;
		}
	}

	

	public Dictionary<string, BaseOverlap> Overlaps
	{
		get
		{
			return this.overlaps;
		}
	}

	

	public Dictionary<string, BaseNotify> Notifies
	{
		get
		{
			return this.notifies;
		}
	}

	

	public BaseScreen CurScreen
	{
		get
		{
			return this.curScreen;
		}
	}

	

	public BasePopup CurPopup
	{
		get
		{
			return this.curPopup;
		}
	}

	

	public BaseNotify CurNotify
	{
		get
		{
			return this.curNotify;
		}
	}

	

	public BaseOverlap CurOverlap
	{
		get
		{
			return this.curOverlap;
		}
	}

	
	private BaseScreen GetNewScreen<T>() where T : BaseScreen
	{
		string name = typeof(T).Name;
		GameObject uiprefabs = this.GetUIPrefabs(UIType.Screen, name);
		if (uiprefabs == null || !uiprefabs.GetComponent<BaseScreen>())
		{
			throw new MissingReferenceException("Can not found" + name + "scenn. !!!");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(uiprefabs);
		gameObject.transform.SetParent(this.cScreen.transform);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		BaseScreen component = gameObject.GetComponent<BaseScreen>();
		component.Init();
		return component;
	}

	
	public void HideAllScreens()
	{
		foreach (KeyValuePair<string, BaseScreen> keyValuePair in this.screens)
		{
			BaseScreen value = keyValuePair.Value;
			if (!(value == null) && !value.IsHide)
			{
				value.Hide();
				if (this.screens.Count <= 0)
				{
					break;
				}
			}
		}
	}

	
	public T GetExistScreen<T>() where T : BaseScreen
	{
		string name = typeof(T).Name;
		if (this.screens.ContainsKey(name))
		{
			return this.screens[name] as T;
		}
		return default(T);
	}

	
	private void RemoveScreen(string v)
	{
		for (int i = 0; i < this.rmScreens.Count; i++)
		{
			if (this.rmScreens[i].Equals(v) && this.screens.ContainsKey(v))
			{
				UnityEngine.Object.Destroy(this.screens[v].gameObject);
				this.screens.Remove(v);
				Resources.UnloadUnusedAssets();
				GC.Collect();
			}
		}
	}

	
	public void ShowScreen<T>(object data = null, bool forceShowData = false) where T : BaseScreen
	{
		string name = typeof(T).Name;
		BaseScreen baseScreen = null;
		if (this.curScreen != null)
		{
			string name2 = this.curScreen.GetType().Name;
			if (name2.Equals(name))
			{
				baseScreen = this.curScreen;
			}
			else
			{
				this.rmScreens.Add(name2);
				this.RemoveScreen(name2);
			}
		}
		if (baseScreen == null)
		{
			if (!this.screens.ContainsKey(name))
			{
				BaseScreen newScreen = this.GetNewScreen<T>();
				if (newScreen != null)
				{
					this.screens.Add(name, newScreen);
				}
			}
			if (this.screens.ContainsKey(name))
			{
				baseScreen = this.screens[name];
			}
		}
		bool flag = false;
		if (baseScreen != null)
		{
			if (forceShowData)
			{
				flag = true;
			}
			else if (baseScreen.IsHide)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.curScreen = baseScreen;
			baseScreen.transform.SetAsFirstSibling();
			baseScreen.Show(data);
		}
	}

	
	private BasePopup GetNewPopup<T>() where T : BasePopup
	{
		string name = typeof(T).Name;
		GameObject uiprefabs = this.GetUIPrefabs(UIType.Popup, name);
		if (uiprefabs == null || !uiprefabs.GetComponent<BasePopup>())
		{
			throw new MissingReferenceException("Can not found" + name + "scenn. !!!");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(uiprefabs);
		gameObject.transform.SetParent(this.cPopup.transform);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		BasePopup component = gameObject.GetComponent<BasePopup>();
		component.Init();
		return component;
	}

	
	private void RemovePopup(string v)
	{
		for (int i = 0; i < this.rmPopups.Count; i++)
		{
			if (this.rmPopups[i].Equals(v) && this.popups.ContainsKey(v))
			{
				UnityEngine.Object.Destroy(this.popups[v].gameObject);
				this.popups.Remove(v);
				rmPopups.RemoveAt(i);
				Resources.UnloadUnusedAssets();
				GC.Collect();
			}
		}
	}

	
	public T GetExistPopup<T>() where T : BasePopup
	{
		string name = typeof(T).Name;
		if (this.popups.ContainsKey(name))
		{
			return this.popups[name] as T;
		}
		return default(T);
	}

	
	public void ShowPopup<T>(object data = null, bool forceShowData = false) where T : BasePopup
	{
		string name = typeof(T).Name;
		BasePopup basePopup = null;
		if (this.curPopup != null)
		{
			string name2 = this.curPopup.GetType().Name;
			if (name2.Equals(name))
			{
				basePopup = this.curPopup;
			}
			else
			{
				//rmPopups.Add(name2);
				this.RemovePopup(name2);
			}
		}
		if (basePopup == null)
		{
			if (!this.popups.ContainsKey(name))
			{
				BasePopup newPopup = this.GetNewPopup<T>();
				if (newPopup != null)
				{
					this.popups.Add(name, newPopup);
				}
			}
			if (this.popups.ContainsKey(name))
			{
				basePopup = this.popups[name];
			}
		}
		bool flag = false;
		if (basePopup != null)
		{
			if (forceShowData)
			{
				flag = true;
			}
			else if (basePopup.IsHide)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.curPopup = basePopup;
			basePopup.transform.SetAsFirstSibling();
			basePopup.Show(data);
		}
	}

	
	public void HideAllPopups()
	{
		foreach (KeyValuePair<string, BasePopup> keyValuePair in this.popups)
		{
			BasePopup value = keyValuePair.Value;
			if (!(value == null) && !value.IsHide)
			{
				value.Hide();
				if (this.popups.Count <= 0)
				{
					break;
				}
			}
		}
	}

	
	private BaseNotify GetNewNotify<T>()
	{
		string name = typeof(T).Name;
		GameObject uiprefabs = this.GetUIPrefabs(UIType.Notify, name);
		if (uiprefabs == null || !uiprefabs.GetComponent<BaseNotify>())
		{
			throw new MissingReferenceException("Can not found" + name + "scenn. !!!");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(uiprefabs);
		gameObject.transform.SetParent(this.cNotify.transform);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		BaseNotify component = gameObject.GetComponent<BaseNotify>();
		component.Init();
		return component;
	}

	
	public void HideAllNotifies()
	{
		foreach (KeyValuePair<string, BaseNotify> keyValuePair in this.notifies)
		{
			BaseNotify value = keyValuePair.Value;
			if (!(value == null) && !value.IsHide)
			{
				value.Hide();
				if (this.notifies.Count <= 0)
				{
					break;
				}
			}
		}
	}

	
	private void RemoveNotify(string v)
	{
		for (int i = 0; i < this.rmNotifies.Count; i++)
		{
			if (this.rmNotifies[i].Equals(v) && this.notifies.ContainsKey(v))
			{
				UnityEngine.Object.Destroy(this.notifies[v].gameObject);
				this.notifies.Remove(v);
				rmNotifies.RemoveAt(i);
				Resources.UnloadUnusedAssets();
				GC.Collect();
			}
		}
	}

	
	public T GetExistNotify<T>() where T : BaseNotify
	{
		string name = typeof(T).Name;
		if (this.notifies.ContainsKey(name))
		{
			return this.notifies[name] as T;
		}
		return default(T);
	}

	
	public void ShowNotify<T>(object data = null, bool forceShowData = false) where T : BaseNotify
	{
		string name = typeof(T).Name;
		BaseNotify baseNotify = null;
		if (this.curNotify != null)
		{
			string name2 = this.curNotify.GetType().Name;
			if (name2.Equals(name))
			{
				baseNotify = this.curNotify;
			}
			else
			{
				rmNotifies.Add(name2);
				this.RemoveNotify(name2);
			}
		}
		if (baseNotify == null)
		{
			if (!this.notifies.ContainsKey(name))
			{
				BaseNotify newNotify = this.GetNewNotify<T>();
				if (newNotify != null)
				{
					this.notifies.Add(name, newNotify);
				}
			}
			if (this.notifies.ContainsKey(name))
			{
				baseNotify = this.notifies[name];
			}
		}
		bool flag = false;
		if (baseNotify != null)
		{
			if (forceShowData)
			{
				flag = true;
			}
			else if (baseNotify.IsHide)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.curNotify = baseNotify;
			baseNotify.transform.SetAsFirstSibling();
			baseNotify.Show(data);
		}
	}

	
	private BaseOverlap GetNewOverLap<T>() where T : BaseOverlap
	{
		string name = typeof(T).Name;
		GameObject uiprefabs = this.GetUIPrefabs(UIType.Overlap, name);
		if (uiprefabs == null || !uiprefabs.GetComponent<BaseOverlap>())
		{
			throw new MissingReferenceException("Can not found" + name + "scenn. !!!");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(uiprefabs);
		gameObject.transform.SetParent(this.cOverlap.transform);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		BaseOverlap component = gameObject.GetComponent<BaseOverlap>();
		component.Init();
		return component;
	}

	
	private void RemoveOverLap(string v)
	{
		for (int i = 0; i < this.rmOverlaps.Count; i++)
		{
			if (this.rmOverlaps[i].Equals(v) && this.overlaps.ContainsKey(v))
			{
				UnityEngine.Object.Destroy(this.overlaps[v]);
				this.overlaps.Remove(v);
				rmOverlaps.RemoveAt(i);
				Resources.UnloadUnusedAssets();
				GC.Collect();
			}
		}
	}

	
	public T GetExistOverlap<T>() where T : BaseOverlap
	{
		string name = typeof(T).Name;
		if (this.overlaps.ContainsKey(name))
		{
			return this.overlaps[name] as T;
		}
		return default(T);
	}

	
	public void ShowOverlap<T>(object data = null, bool forceShowData = false) where T : BaseOverlap
	{
		string name = typeof(T).Name;
		BaseOverlap baseOverlap = null;
		if (this.curOverlap != null)
		{
			string name2 = this.curOverlap.GetType().Name;
			if (name2.Equals(name))
			{
				baseOverlap = this.curOverlap;
			}
			else
			{
				//rmOverlaps.Add(name2);
				this.RemoveOverLap(name2);
			}
		}
		if (baseOverlap == null)
		{
			if (!this.overlaps.ContainsKey(name))
			{
				BaseOverlap newOverLap = this.GetNewOverLap<T>();
				if (newOverLap != null)
				{
					this.overlaps.Add(name, newOverLap);
				}
			}
			if (this.overlaps.ContainsKey(name))
			{
				baseOverlap = this.overlaps[name];
			}
		}
		bool flag = false;
		if (baseOverlap != null)
		{
			if (forceShowData)
			{
				flag = true;
			}
			else if (baseOverlap.IsHide)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.curOverlap = baseOverlap;
			baseOverlap.transform.SetAsFirstSibling();
			baseOverlap.Show(data);
		}
	}

	
	public void HideAllOverlaps()
	{
		foreach (KeyValuePair<string, BaseOverlap> keyValuePair in this.overlaps)
		{
			BaseOverlap value = keyValuePair.Value;
			if (!(value == null) && !value.IsHide)
			{
				value.Hide();
				if (this.overlaps.Count <= 0)
				{
					break;
				}
			}
		}
	}

	
	private GameObject GetUIPrefabs(UIType t, string uiName)
	{
		GameObject gameObject = null;
		string path = "";
		if (gameObject == null)
		{
			switch (t)
			{
			case UIType.Screen:
				path = "Prefabs/UI/Screen/" + uiName;
				break;
			case UIType.Popup:
				path = "Prefabs/UI/Popup/" + uiName;
				break;
			case UIType.Notify:
				path = "Prefabs/UI/Notify/" + uiName;
				break;
			case UIType.Overlap:
				path = "Prefabs/UI/Overlap/" + uiName;
				break;
			}
			gameObject = (Resources.Load(path) as GameObject);
		}
		return gameObject;
	}

	
	public GameObject cScreen;

	
	public GameObject cPopup;

	
	public GameObject cNotify;

	
	public GameObject cOverlap;

	
	private Dictionary<string, BaseScreen> screens = new Dictionary<string, BaseScreen>();

	
	private Dictionary<string, BasePopup> popups = new Dictionary<string, BasePopup>();

	
	private Dictionary<string, BaseOverlap> overlaps = new Dictionary<string, BaseOverlap>();

	
	private Dictionary<string, BaseNotify> notifies = new Dictionary<string, BaseNotify>();

	
	private BaseScreen curScreen;

	
	private BasePopup curPopup;

	
	private BaseNotify curNotify;

	
	private BaseOverlap curOverlap;

	
	private const string SCREEN_RESOURCES_PATH = "Prefabs/UI/Screen/";

	
	private const string POPUP_RESOURCES_PATH = "Prefabs/UI/Popup/";

	
	private const string NOTIFY_RESOURCES_PATH = "Prefabs/UI/Notify/";

	
	private const string OVERLAP_RESOURCES_PATH = "Prefabs/UI/Overlap/";

	
	private List<string> rmScreens = new List<string>();

	
	private List<string> rmPopups = new List<string>();

	
	private List<string> rmNotifies = new List<string>();

	
	private List<string> rmOverlaps = new List<string>();
}
