
using Photon.Pun;
using UnityEngine;


public class CharacterLocomotion : MonoBehaviour
{
	public MobileInput mobileInput;

	private bool IsMobileInput()
	{
		return Application.isMobilePlatform ||
			   (BaseManager<GameManager>.HasInstance() &&
				BaseManager<GameManager>.Instance.isAndroidDebugEditor);
	}

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.characterController = base.GetComponent<CharacterController>();
		this.activeWeapon = base.GetComponent<ActiveWeapon>();
		this.reloadWeapon = base.GetComponent<WeaponReload>();
		this.jumpHeight = BaseManager<DataManager>.Instance.GlobalConfig.jumpHeight;
		this.gravity = BaseManager<DataManager>.Instance.GlobalConfig.gravity;
		this.stepDown = BaseManager<DataManager>.Instance.GlobalConfig.stepDown;
		this.airControl = BaseManager<DataManager>.Instance.GlobalConfig.airControl;
		this.jumpDamp = BaseManager<DataManager>.Instance.GlobalConfig.jumpDamp;
		this.groundSpeed = BaseManager<DataManager>.Instance.GlobalConfig.groundSpeed;
		this.pushPower = BaseManager<DataManager>.Instance.GlobalConfig.pushPower;
		this.maxEnergy = BaseManager<DataManager>.Instance.GlobalConfig.maxEnergy;
		this.energy = this.maxEnergy;
		this.isCrouching = false;
		this.checkTheChange = false;
		Debug.Log($"IsMobileInput: {IsMobileInput()}");
		if (IsMobileInput() && mobileInput == null)
		{
			mobileInput = GameObject.FindFirstObjectByType<MobileInput>();
		}
	}


	private void Update()
	{
		if (IsMobileInput())
		{
			if (mobileInput != null)
			{
				this.userInput.x = mobileInput.joystick.Direction.x;
				this.userInput.y = mobileInput.joystick.Direction.y;
			}
			else
				Debug.Log("MobileInput loco is null");
		}
		else
		{
			this.userInput.x = Input.GetAxis("Horizontal");
			this.userInput.y = Input.GetAxis("Vertical");
		}
		this.animator.SetFloat("InputX", this.userInput.x);
		this.animator.SetFloat("InputY", this.userInput.y);
		this.UpdateIsSprinting();

		if (IsMobileInput())
		{
			// Mobile input handling untuk jump
			if (mobileInput != null && mobileInput.jumpButton != null && !this.isCrouching)
			{
				if (mobileInput.jumpPressed)
				{
					this.Jump();
					mobileInput.jumpPressed = false;
				}
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Space) && !this.isCrouching)
			{
				this.Jump();
			}
		}

		if (IsMobileInput())
		{
			// Mobile input handling untuk crouch
			if (mobileInput != null && mobileInput.crouchButton != null && !this.isJumping && !this.IsSprinting())
			{
				if (mobileInput.crouchPressed)
				{
					this.isCrouching = !this.isCrouching;
					mobileInput.crouchPressed = false;
				}
			}
		}
		else
		{
			if (Input.GetButtonDown("Crouch") && !this.isJumping && !this.IsSprinting())
			{
				this.isCrouching = !this.isCrouching;
			}
		}

		if (this.checkTheChange != this.isCrouching)
		{
			this.Crouch();
		}
	}
	private void FixedUpdate()
	{
		if (this.isJumping)
		{
			this.UpdateInAir();
			return;
		}
		this.UpdateOnGround();
	}


	private void Crouch()
	{
		this.checkTheChange = this.isCrouching;
		this.animator.SetBool(this.isCrouchingParam, this.isCrouching);
		if (AudioManager.HasInstance())
		{
			AudioManager.Instance.PlaySE("Crouch");
		}
		if (this.isCrouching)
		{
			this.activeWeapon.GetActiveWeapon().weaponRecoil.recoilModifier = 0.5f;
			return;
		}
		this.activeWeapon.GetActiveWeapon().weaponRecoil.recoilModifier = 1f;
	}


	public bool IsSprinting()
	{
		bool key = Input.GetKey(KeyCode.LeftShift);
		bool flag = this.activeWeapon.IsFiring();
		bool isReloading = this.reloadWeapon.isReloading;
		bool isChangingWeapon = this.activeWeapon.isChangingWeapon;
		bool flag2 = false;
		if (BaseManager<CameraManager>.HasInstance())
		{
			flag2 = BaseManager<CameraManager>.Instance.isAiming;
		}
		return key && !flag && !isReloading && !isChangingWeapon && this.userInput.y > 0.9f && !flag2 && this.energy > 0f && !this.recover;
	}


	private void UpdateIsSprinting()
	{
		bool flag = this.IsSprinting();
		if (this.energy <= 0f)
		{
			this.recover = true;
		}
		if (this.energy > 5f)
		{
			this.recover = false;
		}
		if (flag)
		{
			this.energy -= Time.deltaTime;
		}
		else if (this.userInput.x == 0f && this.userInput.y == 0f)
		{
			this.energy += Time.deltaTime * 2f;
		}
		else
		{
			this.energy += Time.deltaTime;
		}
		if (this.energy >= this.maxEnergy)
		{
			this.energy = this.maxEnergy;
		}
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.UPDATE_ENERGY, this.energy / 10f);
		}
		if (flag && this.isCrouching)
		{
			this.isCrouching = false;
		}
		if (!activeWeapon.isHolstered)
		{
			this.rigController.SetBool(this.isSprintingParam, flag);
			this.animator.SetBool(this.isSprintingParam, flag);
		}
		else this.animator.SetBool(isSprintingUnarmParam, flag);

	}


	private void UpdateOnGround()
	{
		Vector3 a = this.rootMotion * this.groundSpeed;
		Vector3 b = Vector3.down * this.stepDown;
		this.characterController.Move(a + b);
		this.rootMotion = Vector3.zero;
		if (rigController != null)
		{
			this.rigController.SetBool(this.isJumpingParam, false);
		}

		if (!this.characterController.isGrounded)
		{
			this.SetInAir(0f);
		}
	}


	private void UpdateInAir()
	{
		this.velocity.y = this.velocity.y - this.gravity * Time.fixedDeltaTime;
		Vector3 vector = this.velocity * Time.fixedDeltaTime;
		vector += this.CalculateAircontrol();
		this.characterController.Move(vector);
		if (this.characterController.isGrounded)
		{
			if (AudioManager.HasInstance())
			{
				AudioManager.Instance.PlaySE("Land");
			}
		}
		this.isJumping = !this.characterController.isGrounded;
		this.rootMotion = Vector3.zero;
		this.animator.SetBool("IsJumping", this.isJumping);
	}


	private void OnAnimatorMove()
	{
		this.rootMotion += this.animator.deltaPosition;
	}


	private void Jump()
	{
		if (!this.isJumping)
		{
			if (AudioManager.HasInstance())
			{
				AudioManager.Instance.PlaySE("Jump");
			}
			float inAir = Mathf.Sqrt(2f * this.gravity * this.jumpHeight);
			this.SetInAir(inAir);
		}
	}


	private void SetInAir(float jumpVelocity)
	{
		this.isJumping = true;
		this.velocity = this.animator.velocity * this.jumpDamp * this.groundSpeed;
		this.velocity.y = jumpVelocity;
		this.animator.SetBool(this.isJumpingParam, true);
		if (rigController != null)
		{
			this.rigController.SetBool(this.isJumpingParam, true);
		}

	}


	private Vector3 CalculateAircontrol()
	{
		return (base.transform.forward * this.userInput.y + base.transform.right * this.userInput.x) * (this.airControl / 100f);
	}


	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody == null || attachedRigidbody.isKinematic)
		{
			return;
		}
		if (hit.moveDirection.y < -0.3f)
		{
			return;
		}
		Vector3 a = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
		attachedRigidbody.linearVelocity = a * this.pushPower;
	}


	public Animator rigController;


	private float jumpHeight;


	private float gravity;


	private float stepDown;


	private float airControl;


	private float jumpDamp;


	private float groundSpeed;


	private float pushPower;


	private Animator animator;


	private CharacterController characterController;


	private ActiveWeapon activeWeapon;


	private WeaponReload reloadWeapon;



	private Vector2 userInput;


	private Vector3 rootMotion;


	private Vector3 velocity;


	private bool isJumping;


	private bool isCrouching;


	public bool checkTheChange;


	private int isSprintingParam = Animator.StringToHash("IsSprinting");
	private int isSprintingUnarmParam = Animator.StringToHash("IsSprintingUnarm");


	private int isCrouchingParam = Animator.StringToHash("IsCrouching");


	private int isJumpingParam = Animator.StringToHash("IsJumping");



	public float maxEnergy;


	public float energy;


	private bool recover;
}
