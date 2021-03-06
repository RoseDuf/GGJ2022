using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using System.Collections;
using Game;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class ThirdPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;
		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;
		[Tooltip("Acceleration and deceleration")]
		public float Acceleration = 10.0f;
		public float Deceleration = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.50f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.28f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

        [Header("Attack variables")]
        private CameraTargetScope _targetScope;
        private Villager _targetVillager;
        [SerializeField]
        private float _dashSpeed;
        private bool _isDashing;
        private bool _inTargetRange;

        // Added stuff
        private bool _canDoAtion;
        private UIManager _uiManager;
        private Player _player;

        // cinemachine
        private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

        // player
        private float _speed;
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		// animation IDs
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;

		private Animator _animator;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool _hasAnimator;

		private void Awake()
		{
            // get a reference to our main camera
            _mainCamera = Camera.main.gameObject;

            _targetScope =  _mainCamera.GetComponentInChildren<CameraTargetScope>();

            _uiManager = _uiManager = UIManager.Instance;
        }

		private void Start()
		{
            _player = GetComponent<Player>();
            _animator = _player.DayAnimator;
            _hasAnimator = TryGetComponent(out _animator);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();

			AssignAnimationIDs();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
            _isDashing = false;

            Cursor.lockState = CursorLockMode.Locked;

            DaytimeManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;

            walkingSound = StartCoroutine(WalkingSound());
		}

		private Coroutine walkingSound = null;
		
		private IEnumerator WalkingSound()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.5f);
			
				float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
				if (_input.move == Vector2.zero) targetSpeed = 0.0f;

				if (targetSpeed != 0.0f)
				{
					if (GameManager.HasInstance)
					{
						if (GameManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
						{
							SoundSystem.Instance.PlayDogStepSound(gameObject);
						}
						else
						{
							SoundSystem.Instance.PlayWolfStepSound(gameObject);
						}
					}
				}
			}
		}

        protected void OnDestroy()
        {
            if (DaytimeManager.HasInstance)
            {
                DaytimeManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
            }

            if (walkingSound != null)
            {
	            StopCoroutine(walkingSound);
            }
        }

        private void Update()
		{
            if (_uiManager != null && _uiManager.GameIsPaused) return;

			_hasAnimator = (_animator != null);
			if (!_hasAnimator)
			{
                if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
                {
                    _animator = _player.DayAnimator;
                }
                if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
                {
                    _animator = _player.NightAnimator;
                }
			}

            //AllTime behaviour
            GroundedCheck();
            JumpAndGravity();
			Move();

            //NightTime behaviour
            if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
            {
                DetectTarget();
            }

        }

		private void LateUpdate()
		{
			CameraRotation();
		}

        #region Added Stuff

        private void OnTimeOfDayChanged(DaytimeManager.TimeOfDay timeOfDay)
        {
            if (timeOfDay == DaytimeManager.TimeOfDay.Day)
            {
                _animator = _player.DayAnimator;
            }
            if (timeOfDay == DaytimeManager.TimeOfDay.Night)
            {
                _animator = _player.NightAnimator;
            }
        }

        private void DetectTarget()
        {
            if (_targetScope.TargetList.Count > 0)
            {
                float closestDistance = Mathf.Infinity;
                Transform closestCollider = null;

                foreach (Transform collider in _targetScope.TargetList)
                {
                    if (collider == null)
                    {
                        closestCollider = collider;
                        break;
                    }

                    float distanceFromTarget = (collider.transform.position - transform.position).magnitude;
                    if (distanceFromTarget < closestDistance)
                    {
                        closestDistance = distanceFromTarget;
                        closestCollider = collider;
                    }
                    else
                    {
                        Villager villager = collider.GetComponent<Villager>();

                        if (villager != null)
                        {
                            villager.UIArrow.ShowArrow(false);
                        }
                    }
                }

                if (closestCollider != null)
                {
                    _targetVillager = closestCollider.GetComponent<Villager>();

                    if (_targetVillager != null)
                    {
                        float distanceToTarget = (closestCollider.transform.position - transform.position).magnitude;
                        if (_inTargetRange || distanceToTarget < _targetScope.StopDistance)
                        {
                            _targetVillager.UIArrow.ShowArrow(false);
                            _targetScope.TargetList.Remove(closestCollider);
                            return;
                        }

                        _targetVillager.UIArrow.ShowArrow(true);
                    }
                }
                else
                {
                    _targetScope.TargetList.Remove(closestCollider);
                }
            }
        }

        #endregion

        private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDGrounded, Grounded);
			}
		}

		private void CameraRotation()
		{
			// if there is an input and camera position is not fixed
			if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
			{
				_cinemachineTargetYaw += _input.look.x * Time.deltaTime;
				_cinemachineTargetPitch += _input.look.y * Time.deltaTime;
			}

			// clamp our rotations so our values are limited 360 degrees
			_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Cinemachine will follow this target
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			float acceleration = Acceleration;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				if (_input.move == Vector2.zero)
				{
					acceleration = Deceleration;
				}

				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * acceleration);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * acceleration);
            
            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

				// rotate to face input direction relative to camera position
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}
            
			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            if (_targetVillager != null && _input.dash && !_targetVillager.IsDead)
            {
                _animator.SetBool("IsDashing", true);
                float distanceToTarget = (_targetVillager.transform.position - transform.position).magnitude;
                float rotationToTarget = Vector3.Angle(_targetVillager.transform.position, transform.forward);
                Vector3 direction = (_targetVillager.transform.position - transform.position).normalized;

                if (distanceToTarget > _targetScope.StopDistance)
                {
					_speed = _dashSpeed;
                    targetDirection = direction;
                    transform.LookAt(_targetVillager.transform.position);
                }
                else
                {
                    _animator.SetBool("IsDashing", false);
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
					_speed = 0;
                    _targetVillager = null;
                    _input.dash = false;
                }
            }

            else if (_input.dash)
            {
                _animator.SetBool("IsDashing", false);
                _input.dash = false;
            }

            // move the player
            _controller.Move((targetDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetFloat(_animIDSpeed, _animationBlend);
				_animator.SetBool("IsWalking", (targetSpeed != 0.0f));
				_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
			}
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDJump, false);
					_animator.SetBool(_animIDFreeFall, false);
				}

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDJump, true);
					}
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else
				{
					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDFreeFall, true);
					}
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
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
			
			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}