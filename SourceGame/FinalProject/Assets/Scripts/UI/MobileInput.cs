using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInput : MonoBehaviour
{
    public FloatingJoystick joystick; //using joystick.Direction for movement
    public Button jumpButton;
    public Button crouchButton;
    public Button fireButton;
    public Button aimButton;
    public Button reloadButton;

    public bool jumpPressed = false;
    public bool crouchPressed = false;
    public bool firePressed = false;
    public bool fireHolding = false;
    public bool aimPressed = false;
    public bool reloadPressed = false;

    private bool IsMobileInput()
    {
        return Application.isMobilePlatform ||
               (BaseManager<GameManager>.HasInstance() &&
                BaseManager<GameManager>.Instance.isAndroidDebugEditor);
    }

    private void Start()
    {
        CharacterLocomotion characterLocomotion = FindObjectOfType<CharacterLocomotion>();
        if (characterLocomotion != null)
            characterLocomotion.mobileInput = this;
        ActiveWeapon activeWeapon = FindObjectOfType<ActiveWeapon>();
        if (activeWeapon != null)
            activeWeapon.mobileInput = this;
        Debug.Log(characterLocomotion);
        Debug.Log(activeWeapon);
        
        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpPressed);
        if (crouchButton != null)
            crouchButton.onClick.AddListener(OnCrouchPressed);
        
        // Fire button cần PointerDown/PointerUp thay vì onClick
        if (fireButton != null)
        {
            EventTrigger trigger = fireButton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = fireButton.gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnFireDown(); });
            trigger.triggers.Add(pointerDown);
            
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnFireUp(); });
            trigger.triggers.Add(pointerUp);
        }
        
        if (aimButton != null)
            aimButton.onClick.AddListener(OnAimPressed);
        if (reloadButton != null)
            reloadButton.onClick.AddListener(OnReloadPressed);
    }

    private void OnJumpPressed()
    {
        jumpPressed = true;
    }

    private void OnCrouchPressed()
    {
        crouchPressed = true;
    }

    private void OnFireDown()
    {
        firePressed = true;
        fireHolding = true;
    }

    private void OnFireUp()
    {
        fireHolding = false;
    }

    private void OnAimPressed()
    {
        aimPressed = true;
    }

    private void OnReloadPressed()
    {
        reloadPressed = true;
    }
}
