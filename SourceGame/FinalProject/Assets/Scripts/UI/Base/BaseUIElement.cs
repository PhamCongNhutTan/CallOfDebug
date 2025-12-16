
using UnityEngine;


public class BaseUIElement : MonoBehaviour
{
	

	public bool IsInited
	{
		get
		{
			return this.isInited;
		}
	}

	

	public bool IsHide
	{
		get
		{
			return this.isHide;
		}
	}

	

	public CanvasGroup CanvasGroup
	{
		get
		{
			return this.canvasGroup;
		}
	}

	

	public UIType UIType
	{
		get
		{
			return this.uiType;
		}
	}

	
	public virtual void Init()
	{
		this.isInited = true;
		if (!base.gameObject.GetComponent<CanvasGroup>())
		{
			base.gameObject.AddComponent<CanvasGroup>();
		}
		this.canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		base.gameObject.SetActive(true);
		RectTransform rectTransform = (RectTransform)this.transform;
		if(rectTransform)
		{
			// Kiểm tra nếu RectTransform đang stretch full size
			if(rectTransform.anchorMin == Vector2.zero && rectTransform.anchorMax == Vector2.one)
			{
				// Reset offsetMin (left, bottom) và offsetMax (right, top) về 0
				rectTransform.offsetMin = Vector2.zero; // left = 0, bottom = 0
				rectTransform.offsetMax = Vector2.zero; // right = 0, top = 0
			}
		}
	}

	
	public virtual void Show(object data)
	{
		base.gameObject.SetActive(true);
		this.isHide = false;
		this.SetActiveCanvasGroup(true);
	}

	
	public virtual void Hide()
	{
		this.isHide = true;
		this.SetActiveCanvasGroup(false);
	}

	
	private void SetActiveCanvasGroup(bool isActive)
	{
		if (this.CanvasGroup != null)
		{
			this.CanvasGroup.blocksRaycasts = isActive;
			this.CanvasGroup.alpha = (float)(isActive ? 1 : 0);
		}
	}

	
	public static void OnPointerEnter()
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.PlaySE("OnPointerEnter", 0f);
		}
	}

	
	public static void OnPointerDown()
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.PlaySE("OnPointerDown", 0f);
		}
	}

	
	protected CanvasGroup canvasGroup;

	
	protected UIType uiType;

	
	protected bool isHide;

	
	protected bool isInited;
}
