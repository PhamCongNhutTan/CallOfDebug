using System;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class CharacterAimingMultiplayer : MonoBehaviour
{
    public CamMultiplayer camManager;
    public PhotonView photonView;
    public GameObject camGroup;
    private MobileInput mobileInput;
    
    
    private void Awake()
    {
        if (!photonView.IsMine) { return; }
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        mainCamera.enabled = true;
        this.mainCamera.transform.SetParent(null, false);       
        camGroup.SetActive(true);
    }

    
    private void Start()
    {
        if (!photonView.IsMine) { return; }
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //this.activeWeapon = base.GetComponent<ActiveWeapon>();
        camManager = GetComponent<CamMultiplayer>();
        if (BaseManager<ListenerManager>.HasInstance())
        {
            BaseManager<ListenerManager>.Instance.Register(ListenType.CHANGE_SCOPE, new Action<object>(this.ChangeCamInfo));
        }
        this.mobileInput = GameObject.FindFirstObjectByType<MobileInput>();
    }
    
    private bool IsMobileInput()
    {
        return Application.isMobilePlatform ||
               (BaseManager<GameManager>.HasInstance() &&
                BaseManager<GameManager>.Instance.isAndroidDebugEditor);
    }

    
    private void Update()
    {
        if (!photonView.IsMine) { return; }
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        
        if (IsMobileInput())
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.position.x > Screen.width / 2f && touch.phase == TouchPhase.Moved)
                    {
                        this.xAxist.Value += touch.deltaPosition.x * 0.5f;
                        this.yAxist.Value -= touch.deltaPosition.y * 0.5f;
                    }
                }
            }
        }
        
        if (activeWeapon.GetActiveWeapon() != null && !isDeath)
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
            
            if (this.activeWeapon.GetActiveWeapon().weaponSlot == ActiveWeaponMultiplayer.WeaponSlot.Primary && aimInput)
            {
                camManager.ChangeScope();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !this.isEcs)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (BaseManager<UIManager>.HasInstance())
            {
                BaseManager<UIManager>.Instance.ShowPopup<GameMenuMulti>(null, true);
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
        if (!photonView.IsMine) { return; }
        if (photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        if (!this.isEcs && !isDeath)
        {
            this.moveAxist();
        }
    }

    
    private void moveAxist()
    {
        this.xAxist.Update(Time.fixedDeltaTime);
        this.yAxist.Update(Time.fixedDeltaTime);
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

        if (camManager.isAiming)
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

    
    public float turnSpeed = 15f;

    
    public Camera mainCamera;

    
    public AxisState xAxist;

    
    public AxisState yAxist;

    
    public Transform[] cameraLookAt;

    
    public ActiveWeaponMultiplayer activeWeapon;

    
    public bool isEcs;
    public bool isDeath = false;
    
    public int curLookAt;
}
