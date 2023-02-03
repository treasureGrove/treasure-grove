using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class spriteAnimatorSystem : MonoBehaviour
{
	[Header ("Ability Settings")]
	[Space]

	public bool spriteAnimatorEnabled = true;

	public bool spriteAnimatorActive;

	public bool adjustSpriteToRotationToCameraDirection;

	public string groundState = "Grounded";
	public string movingState = "Moving";
	public string airSpeedState = "Air Speed Y";
	public string jumpState = "Jump";

	public string runState = "Run";

	public string cameraAngleState = "Camera Angle";

	public string idleAnimatorName = "Idle";
	public string walkAnimatorName = "Walk";
	public string jumpAnimatorName = "Jump";
	public string onAirUpAnimatorName = "On Air Up";

	public string onAirDownAnimatorName = "On Air Down";

	public string landAnimatorName = "Land";

	public float jumpAnimationDuration = 1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool idleActive;
	public bool walkActive;
	public bool jumpActive;

	public bool runActive;

	public bool onAirUpActive;
	public bool onAirDownActive;
	public bool landActive;

	public bool playerIsMoving;
	public bool idlePreviouslyActive;

	public bool walkPreviouslyActive;

	public bool jumpPreviouslyActive;

	bool animationStateChanged;

	public bool playerOnGround;

	public bool playerOnGroundPreviously;

	public bool playerHasJumped;

	public float airSpeed;

	public float horizontalInput;
	public float verticalInput;

	Vector2 axisValues;

	public float currentCameraAngle;

	public float currentCameraAngleValue;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnSpriteEnabled;
	public UnityEvent eventOnSpriteDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public playerWeaponsManager mainPlayerWeaponsManager;

	public playerCamera mainPlayerCamera;

	public Transform mainCameraTransform;

	public Transform playerControllerTransform;

	public Animator mainAnimator;

	public Animator mainSpriteAnimator;

	public GameObject playerSpriteGameObject;

	public Transform playerSpriteTransform;

	public Transform movementLastDirection;

	Vector3 originalSpriteScale;

	bool movingToRightActive;
	bool movingToLeftActive;

	float lastTimeOnAir;

	float lastTimeMoving;

	int groundID;
	int movingID;
	int airSpeedID;
	int jumpID;
	int runID;
	int cameraAngleID;


	void Start ()
	{
		originalSpriteScale = playerSpriteTransform.localScale;

		groundID = Animator.StringToHash (groundState);

		movingID = Animator.StringToHash (movingState);

		airSpeedID = Animator.StringToHash (airSpeedState);

		jumpID = Animator.StringToHash (jumpState);

		runID = Animator.StringToHash (runState);

		cameraAngleID = Animator.StringToHash (cameraAngleState);
	}

	void Update ()
	{
		if (spriteAnimatorActive) {
			playerOnGround = mainPlayerController.isPlayerOnGround ();

			if (mainPlayerController.isPlayerUsingInput ()) {
				lastTimeMoving = Time.time;
			}

			if (Time.time < lastTimeMoving + 0.4f) {
				playerIsMoving = true;
			} else {
				playerIsMoving = false;
			}

			playerHasJumped = mainPlayerController.jumpInput;

			runActive = mainPlayerController.isPlayerRunning ();

			airSpeed = mainPlayerController.currentVelocity.y;

			if (playerHasJumped != jumpPreviouslyActive) {
				if (playerHasJumped) {
					jumpPreviouslyActive = true;
				
					jumpActive = true;

					lastTimeOnAir = Time.time;
				}
			}

			if (jumpPreviouslyActive) {
				if (((playerOnGround || airSpeed < 0) && Time.time > lastTimeOnAir + 0.5f) || (!playerOnGround && Time.time > lastTimeOnAir + jumpAnimationDuration)) {
					jumpPreviouslyActive = false;

					jumpActive = false;
				}
			}

			axisValues = mainPlayerController.getRawAxisValues ();

			Vector3 direction = playerSpriteTransform.position - mainCameraTransform.position;

			//			float angle = Vector3.SignedAngle (direction, playerSpriteTransform.forward, playerSpriteTransform.up);

			Quaternion lookDirection = Quaternion.LookRotation (direction);

			float upAxisRotation = lookDirection.eulerAngles.y;

			playerSpriteTransform.eulerAngles = playerSpriteTransform.up * upAxisRotation;



			if (adjustSpriteToRotationToCameraDirection) {
				if (axisValues != Vector2.zero) {
					Vector3 moveInput = mainPlayerController.getMoveInputDirection ();

					Quaternion movevementDirectionRotation = Quaternion.LookRotation (new Vector3 (moveInput.x, 0, moveInput.z));

					movementLastDirection.rotation = Quaternion.Euler (new Vector3 (0, movevementDirectionRotation.eulerAngles.y, 0));
				}

				currentCameraAngle = Vector3.SignedAngle (mainPlayerCamera.transform.forward, movementLastDirection.forward, mainPlayerCamera.transform.up);
			
			
				float ABSAngle = Mathf.Abs (currentCameraAngle);

				if (ABSAngle < 30) {
					currentCameraAngleValue = 0;
				} else if (ABSAngle > 30 && ABSAngle < 60) {
					currentCameraAngleValue = 1;
				} else if (ABSAngle > 60 && ABSAngle < 120) {
					currentCameraAngleValue = 2;
				} else if (ABSAngle > 120 && ABSAngle < 150) {
					currentCameraAngleValue = 2;
				} else {
					currentCameraAngleValue = 3;
				}

//				if (currentCameraAngleValue == 1 || currentCameraAngleValue == 2) {
//					if (axisValues.x == 0) {
//						if (currentCameraAngle > 0) {
//							axisValues.x = -1;
//						} else if (currentCameraAngle < 0) {
//							axisValues.x = 1;
//						}
//					}
//				}
			}


			if (axisValues.x > 0) {
				if (!movingToRightActive) {
					playerSpriteTransform.localScale = new Vector3 (axisValues.x * originalSpriteScale.x, originalSpriteScale.y, originalSpriteScale.z);

					movingToRightActive = true;

					movingToLeftActive = false;
				}
			} else if (axisValues.x < 0) {
				if (!movingToLeftActive) {
					playerSpriteTransform.localScale = new Vector3 (axisValues.x * originalSpriteScale.x, originalSpriteScale.y, originalSpriteScale.z);

					movingToLeftActive = true;

					movingToRightActive = false;
				}
			}


			updateAnimator ();
		}
	}

	void updateAnimator ()
	{
		mainSpriteAnimator.SetBool (groundID, playerOnGround);
		mainSpriteAnimator.SetBool (movingID, playerIsMoving);
		mainSpriteAnimator.SetFloat (airSpeedID, airSpeed, 0.1f, Time.fixedDeltaTime);
		mainSpriteAnimator.SetBool (jumpID, jumpActive);

		mainSpriteAnimator.SetBool (runID, runActive);

		mainSpriteAnimator.SetFloat (cameraAngleID, currentCameraAngleValue);
	}

	public void setSpriteAnimatorActiveState (bool state)
	{
		if (!spriteAnimatorEnabled) {
			return;
		}

		if (spriteAnimatorActive == state) {
			return;
		}

		spriteAnimatorActive = state;

		if (spriteAnimatorActive) {

		} else {

		}

		mainAnimator.enabled = !state;

		mainSpriteAnimator.enabled = state;

		mainPlayerController.setUseFirstPersonPhysicsInThirdPersonActiveState (state);

		mainPlayerController.setCharacterMeshGameObjectState (!state);

		mainPlayerController.setCharacterMeshesListToDisableOnEventState (!state);

		mainPlayerController.setUseRootMotionActiveState (!state);

		mainPlayerWeaponsManager.enableOrDisableEnabledWeaponsMesh (!state);

		if (playerSpriteGameObject.activeSelf != state) {
			playerSpriteGameObject.SetActive (state);
		}

		checkEventState (state);
	}

	public void checkEventState (bool state)
	{
		if (state) {
			eventOnSpriteEnabled.Invoke ();
		} else {
			eventOnSpriteDisabled.Invoke ();
		}
	}

	public void setSpriteAnimatorActiveStateFromEditor (bool state)
	{
		setSpriteAnimatorActiveState (state);
	}
}
