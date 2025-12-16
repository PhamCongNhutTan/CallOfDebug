using System;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations;


public class CharacterAiming : MonoBehaviour
{
	public float turnSpeed = 15f;
	public Camera mainCamera;
	public AxisState xAxist;
	public AxisState yAxist;
	public Transform[] cameraLookAt;
	public ActiveWeapon activeWeapon;
	public bool isEcs;
	public int curLookAt;
	private float touchSensitivity = 1f;
	private MobileInput mobileInput;

	private void Start()
	{
		if (!IsMobileInput())
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		this.activeWeapon = base.GetComponent<ActiveWeapon>();
		this.mobileInput = GameObject.FindFirstObjectByType<MobileInput>();
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.Register(ListenType.CHANGE_SCOPE, new Action<object>(this.ChangeCamInfo));
		}
	}

	private bool IsMobileInput()
	{
		return Application.isMobilePlatform ||
			   (BaseManager<GameManager>.HasInstance() &&
				BaseManager<GameManager>.Instance.isAndroidDebugEditor);
	}

	private Vector2 GetInputDelta()
	{
		if (IsMobileInput())
		{
			// Touch input - swipe detection - chỉ cho phép nửa phải màn hình
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);

				// Chỉ xử lý touch ở nửa bên phải màn hình (x > Screen.width / 2)
				if (touch.position.x > Screen.width / 2f && touch.phase == TouchPhase.Moved)
				{
					return touch.deltaPosition * touchSensitivity;
				}
			}
			return Vector2.zero;
		}
		else
		{
			// Mouse input
			float mouseX = Input.GetAxis("Mouse X") * 100f;
			float mouseY = Input.GetAxis("Mouse Y") * 100f;
			return new Vector2(mouseX, mouseY);
		}
	}

	private void Update()
	{
		if (activeWeapon.GetActiveWeapon() != null)
		{
			bool aimInput = false;
			if (IsMobileInput() && mobileInput != null)
			{
				if (mobileInput.aimPressed)
				{
					aimInput = true;
					mobileInput.aimPressed = false;
				}
			}
			else
			{
				aimInput = Input.GetKeyDown(KeyCode.Mouse1);
			}
			
			if (this.activeWeapon.GetActiveWeapon().weaponSlot == ActiveWeapon.WeaponSlot.Primary && aimInput)
			{
				if (CameraManager.HasInstance())
				{
					BaseManager<CameraManager>.Instance.ChangeScope();
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape) && !this.isEcs)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			if (BaseManager<UIManager>.HasInstance())
			{
				BaseManager<UIManager>.Instance.ShowPopup<GameMenu>(null, false);
				this.isEcs = true;
			}
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (!Cursor.visible)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				return;
			}
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}


	private void FixedUpdate()
	{
		if (!this.isEcs)
		{
			this.moveAxist();
		}
	}


	private void moveAxist()
	{
		Vector2 inputDelta = GetInputDelta();

		// AxisState expects mouse input, apply delta manually
		if (IsMobileInput() && Input.touchCount > 0)
		{
			// For touch: directly apply swipe delta
			this.xAxist.Value += inputDelta.x * Time.fixedDeltaTime;
			this.yAxist.Value -= inputDelta.y * Time.fixedDeltaTime;
		}
		else
		{
			// For mouse: use standard input handling
			this.xAxist.Update(Time.fixedDeltaTime);
			this.yAxist.Update(Time.fixedDeltaTime);
		}

		this.cameraLookAt[this.curLookAt].eulerAngles = new Vector3(this.yAxist.Value, this.xAxist.Value, 0f);
		float y = this.mainCamera.transform.rotation.eulerAngles.y;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, y, 0f), this.turnSpeed * Time.fixedDeltaTime);
	}


	public void ChangeCamInfo(object data)
	{
		if (BaseManager<AudioManager>.HasInstance())
		{
			BaseManager<AudioManager>.Instance.PlaySE("ChangeScope", 0f);
		}
		if (data is bool && (bool)data)
		{
			this.curLookAt = 0;
			this.xAxist.m_MaxSpeed = 300f;
			this.yAxist.m_MaxSpeed = 300f;
			this.xAxist.m_AccelTime = 0.02f;
			this.yAxist.m_AccelTime = 0.02f;
			return;
		}
		bool isAiming = false;
		if (BaseManager<CameraManager>.HasInstance())
		{
			isAiming = BaseManager<CameraManager>.Instance.isAiming;

		}
		if (isAiming)
		{
			this.curLookAt = 0;
			this.xAxist.m_MaxSpeed = 300f;
			this.yAxist.m_MaxSpeed = 300f;
			this.xAxist.m_AccelTime = 0.02f;
			this.yAxist.m_AccelTime = 0.02f;
			return;
		}
		this.curLookAt = 1;
		this.xAxist.m_MaxSpeed = 50f;
		this.yAxist.m_MaxSpeed = 50f;
		this.xAxist.m_AccelTime = 0.1f;
		this.yAxist.m_AccelTime = 0.1f;
	}
}
