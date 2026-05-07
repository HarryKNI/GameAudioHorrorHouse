using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealisticHorrorGameSystem
{
	[RequireComponent(typeof(CharacterController))]
	public class FirstPersonController : MonoBehaviour
	{
		public float MoveSpeed = 4.0f;
		public float SprintSpeed = 6.0f;
		public float RotationSpeed = 1.0f;
		public float SpeedChangeRate = 10.0f;
		public float JumpHeight = 1.2f;
		public float Gravity = -15.0f;
		public float JumpTimeout = 0.1f;
		public float FallTimeout = 0.15f;
		public bool Grounded = true;
		public float GroundedOffset = -0.14f;
		public float GroundedRadius = 0.5f;
		public LayerMask GroundLayers;
		public float TopClamp = 90.0f;
		public float BottomClamp = -90.0f;
		private float _speed;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;
		private CharacterController _controller;
		public GameObject Camera;
		private bool canJump = true;
		public Animation FPSHandParent;
		[HideInInspector]
		public bool isCrouching = false;
        [HideInInspector]
        public bool isInVent = false;

        private float originalHeight;
		private bool increasingHeight = true;
		private float heightChangeSpeed = 6f;
		private float heightDelta = 0.05f;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction crouchAction;
        private InputAction sprintAction;


        private void Start()
		{
			_controller = GetComponent<CharacterController>();
			originalHeight = _controller.height;

            moveAction = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            moveAction.Enable();

            jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
            jumpAction.Enable();

            crouchAction = new InputAction(type: InputActionType.Button);
            crouchAction.AddBinding("<Keyboard>/leftCtrl");
            crouchAction.AddBinding("<Keyboard>/rightCtrl");
            crouchAction.Enable();

            sprintAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/leftShift");
            sprintAction.Enable();
        }

		private void Update()
		{
			JumpCrouchAndGravity();
			GroundedCheck();
			Move();
		}

		public void Jump()
        {
			if(canJump && AdvancedGameManager.Instance.canJump)
			{
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				Grounded = false;
				AudioManager.Instance.Play_Jump();
				StartCoroutine(ResetJump());
			}
		}

		public void Crouch()
        {
			if(AdvancedGameManager.Instance.canCrouch)
            {
				if(isInVent)
				{
					return;
				}
				if(isCrouching)
                {
					_controller.height = 2;
					originalHeight = 2;
					isCrouching = false;
				}
				else
                {
					_controller.height = 0.75f;
					originalHeight = 0.75f;
					isCrouching = true;
				}
            }
        }

		IEnumerator ResetJump()
		{
			canJump = false;
			yield return new WaitForSeconds(1);
			canJump = true;
        }


		private void LateUpdate()
        {
			RotationUpdate();
        }

		private void RotationUpdate()
        {
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.transform.eulerAngles.y, transform.eulerAngles.z);
        }


        private void GroundedCheck()
		{
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		public Vector2 _input;
		public Vector2 _input_look;
		[HideInInspector]
		public bool isSprinting = false;

		public void Sprint()
        {
			if (AdvancedGameManager.Instance.canSprint)
			{
				if (isSprinting)
				{
					isSprinting = false;
				}
				else
				{
					isSprinting = true;
				}
			}
        }

		private float Stamina = 100;

		private void Move()
		{
			float targetSpeed = MoveSpeed;
			if(AdvancedGameManager.Instance.canSprint)
            {
                if (sprintAction.IsPressed())
                {
                    isSprinting = true;
					AudioManager.Instance.footstepEvent.SetParameter("MoveState", 2, false);
                }
                else if (sprintAction.WasReleasedThisFrame())
                {
                    isSprinting = false;
                    AudioManager.Instance.footstepEvent.SetParameter("MoveState", 1, false);
                }
                if (isSprinting && Stamina > 0)
                {
					targetSpeed = SprintSpeed;
					Stamina = Stamina - Time.deltaTime * 25;
				}
				else if(!isSprinting && Stamina < 100)
                {
					Stamina = Stamina + Time.deltaTime * 10;
					if (Stamina > 100) Stamina = 100;
				}
                if (Stamina <= 0)
                {
                    AudioManager.Instance.Play_Audio_StaminaBreathing();
                    Stamina = 0;
                    AudioManager.Instance.footstepEvent.SetParameter("MoveState", 1, false);
                }
                GameCanvas.Instance.Slider_Stamina.fillAmount = (Stamina / 100f);
			}


			if(isCrouching)
            {
				targetSpeed = (MoveSpeed / 1.5f);
                AudioManager.Instance.footstepEvent.SetParameter("MoveState", 0, false);
            }
			else if (!isCrouching && !sprintAction.IsPressed())
			{
                AudioManager.Instance.footstepEvent.SetParameter("MoveState", 1, false);
            }


            _input = moveAction.ReadValue<Vector2>();
            if (_input == Vector2.zero) targetSpeed = 0.0f;
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
			float speedOffset = 0.1f;
			float inputMagnitude = 1f;
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			Vector3 inputDirection = new Vector3(_input.x, 0.0f, _input.y).normalized;
			if (_input != Vector2.zero)
			{
				inputDirection = transform.right * _input.x + transform.forward * _input.y;
				AudioManager.Instance.Play_Player_Walk();
				if (!FPSHandParent.isPlaying) FPSHandParent.Play("WalkingHandAnimation");

				if (increasingHeight)
				{
					_controller.height += heightDelta * Time.deltaTime * heightChangeSpeed;
					if (_controller.height >= originalHeight + heightDelta)
					{
						increasingHeight = false;
					}
				}
				else
				{
					_controller.height -= heightDelta * Time.deltaTime * heightChangeSpeed;
					if (_controller.height <= originalHeight - heightDelta)
					{
						increasingHeight = true;
					}
				}



			}
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpCrouchAndGravity()
		{
            if (jumpAction.WasReleasedThisFrame() && AdvancedGameManager.Instance.canJump)
            {
				Jump();
			}
            if (crouchAction.WasReleasedThisFrame() && AdvancedGameManager.Instance.canCrouch)
            {
                Crouch();
            }
            if (Grounded)
			{
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}
			}

			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}