using System;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;


public class CameraManager : BaseManager<CameraManager>
{
	
	private void Start()
	{
		//this.ChangeCam(virCam.Main);
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.Register(ListenType.ON_PLAYER_DEATH, new Action<object>(this.DeathCamera));
		}
	}

	
	public void OnDestroy()
	{
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.Unregister(ListenType.ON_PLAYER_DEATH, new Action<object>(this.DeathCamera));
		}
	}

	
	public void DeathCamera(object data)
	{
		this.brain.DefaultBlend.Time = 2f;
		this.ChangeCam(virCam.Death);
	}

	
	public void TurnOffScope()
	{
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.CHANGE_SCOPE, true);
		}
		this.ChangeCam(virCam.Main);
		this.isAiming = false;
	}

	
	public void ChangeScope()
	{
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.CHANGE_SCOPE, null);
		}
		if (this.isAiming)
		{
			this.ChangeCam(virCam.Main);
			this.isAiming = false;
			return;
		}
		this.ChangeCam(virCam.Scope);
		this.isAiming = true;
	}

	
	private void ChangeCam(virCam cam)
	{
		CinemachineVirtualCamera[] array = this.virtualCameras;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Priority = 0;
		}
		this.virtualCameras[(int)cam].Priority = 20;
	}

	
	public CrossHairTarget crossHairTarget;

	
	public CinemachineVirtualCamera[] virtualCameras;

	
	public CinemachineBrain brain;

	
	public bool isAiming;
}
