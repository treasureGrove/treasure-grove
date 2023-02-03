using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerLadderSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool ladderFound;
	public float ladderMoovementSpeed = 5;
	public float ladderVerticalMovementAmount = 0.3f;
	public float ladderHorizontalMovementAmount = 0.1f;

	public float minAngleToInverseDirection = 100;

	public bool useAlwaysHorizontalMovementOnLadder;

	public bool useAlwaysLocalMovementDirection;

	public float minAngleVerticalDirection = 60;
	public float maxAngleVerticalDirection = 120;

	public LayerMask layerToCheckLadderEnd;

	public string climbLadderFootStepStateName = "Climb Ladders";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool ladderEndDetected;
	public bool ladderStartDetected;

	public int movementDirection;
	public float ladderVerticalInput;
	public float ladderHorizontalInput;
	public float ladderAngle;
	public float ladderSignedAngle;

	public float currentVerticalInput;
	public float currentHorizontalInput;

	public Vector3 ladderMovementDirection;

	public bool movingOnLadder;
	public bool movingOnLadderPreviously;

	public Transform ladderDirectionTransform;
	public Transform ladderRaycastDirectionTransform;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnThirdPerson;
	public UnityEvent eventOnLocatedLadderOnThirdPerson;
	public UnityEvent eventOnRemovedLadderOnThirdPerson;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController playerControllerManager;
	public playerCamera playerCameraManager;
	public Transform playerCameraTransform;
	public Transform playerPivotTransform;
	public Rigidbody mainRigidbody;

	bool useLadderHorizontalMovement;
	bool moveInLadderCenter;
	bool useLocalMovementDirection;

	ladderSystem currentLadderSystem;
	ladderSystem previousLadderSystem;

	float verticalInput;
	float horizontalInput;

	Vector3 currentPlayerPosition;

	Vector3 currentPositionOffset;
	Vector3 lastPosition;

	void FixedUpdate ()
	{
		if (ladderFound) {

			verticalInput = playerControllerManager.getVerticalInput ();
			horizontalInput = playerControllerManager.getHorizontalInput ();

			movementDirection = 1;

			ladderAngle = Vector3.Angle (playerCameraTransform.forward, ladderDirectionTransform.forward);

			if (ladderAngle > minAngleToInverseDirection) {
				movementDirection = (-1);
			}

			if (useLocalMovementDirection || useAlwaysLocalMovementDirection) {

				ladderSignedAngle = Vector3.SignedAngle (playerCameraTransform.forward, ladderDirectionTransform.forward, playerCameraTransform.up);

				if (ladderAngle < minAngleVerticalDirection || ladderAngle > maxAngleVerticalDirection) {
					currentVerticalInput = verticalInput;
					currentHorizontalInput = horizontalInput;
				} else {
					if (ladderSignedAngle < 0) {
						movementDirection = (-1);
					} else {
						movementDirection = 1;
					}

					currentVerticalInput = horizontalInput;
					currentHorizontalInput = -verticalInput;
				}
			} else {
				currentVerticalInput = verticalInput;
				currentHorizontalInput = horizontalInput;
			}
				
			ladderVerticalInput = currentVerticalInput * movementDirection;

			ladderMovementDirection = Vector3.zero;

			currentPlayerPosition = mainRigidbody.position;

			if (moveInLadderCenter) {
				if (useLadderHorizontalMovement || useAlwaysHorizontalMovementOnLadder) {
					if (Mathf.Abs (currentHorizontalInput) < 0.01f) {
						currentPlayerPosition = new Vector3 (ladderDirectionTransform.position.x, mainRigidbody.position.y, ladderDirectionTransform.position.z);
					}
				}
			}

			ladderMovementDirection += currentPlayerPosition + ladderDirectionTransform.up * (ladderVerticalMovementAmount * ladderVerticalInput);

			ladderEndDetected = !Physics.Raycast (mainRigidbody.position, ladderRaycastDirectionTransform.forward, 2, layerToCheckLadderEnd);

			ladderStartDetected = Physics.Raycast (mainRigidbody.position + playerCameraTransform.up * 0.1f, 
				-playerCameraTransform.up, 0.13f, layerToCheckLadderEnd);

			if (ladderEndDetected || (ladderStartDetected && ladderVerticalInput < 0)) {
				ladderMovementDirection = currentPlayerPosition + ladderRaycastDirectionTransform.forward * ladderVerticalInput;
			}

			if (useLadderHorizontalMovement || useAlwaysHorizontalMovementOnLadder) {

				ladderHorizontalInput = currentHorizontalInput * movementDirection;

				ladderMovementDirection += ladderDirectionTransform.right * (ladderHorizontalInput * ladderHorizontalMovementAmount);
			}

			mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, ladderMovementDirection, Time.deltaTime * ladderMoovementSpeed);

			currentPositionOffset = mainRigidbody.position - lastPosition;
			if (currentPositionOffset.sqrMagnitude > 0.0001f) {
				lastPosition = mainRigidbody.position;
				movingOnLadder = true;
			} else {
				movingOnLadder = false;
			}

			if (movingOnLadder != movingOnLadderPreviously) {
				movingOnLadderPreviously = movingOnLadder;

				if (movingOnLadder) {
					playerControllerManager.stepManager.setFootStepState (climbLadderFootStepStateName);
				} else {
					playerControllerManager.stepManager.setDefaultGroundFootStepState ();
				}
			}
		}
	}

	public void setLadderFoundState (bool state, ladderSystem newLadderSystem)
	{
		if (playerControllerManager.isIgnoreExternalActionsActiveState () && state) {
			return;
		}

		if (!playerControllerManager.isPlayerOnFirstPerson ()) {
			if (state) {
				checkLadderEventsOnThirdPerson (true);

				return;
			} else {
				checkLadderEventsOnThirdPerson (false);

			}
		}

		if (previousLadderSystem != currentLadderSystem) {

			previousLadderSystem = currentLadderSystem;
		}

		currentLadderSystem = newLadderSystem;

		ladderFound = state;

		playerControllerManager.setGravityForcePuase (ladderFound);

		playerControllerManager.setPhysicMaterialAssigmentPausedState (ladderFound);

		playerCameraManager.setCameraPositionMouseWheelEnabledState (!ladderFound);

		if (ladderFound) {
			playerControllerManager.setRigidbodyVelocityToZero ();

			playerControllerManager.setOnGroundState (false);

			playerControllerManager.setZeroFrictionMaterial ();

			playerControllerManager.headBobManager.stopAllHeadbobMovements ();
			playerControllerManager.headBobManager.playOrPauseHeadBob (false);

			playerControllerManager.stepManager.setFootStepState (climbLadderFootStepStateName);

			playerControllerManager.setPauseAllPlayerDownForces (true);

			playerCameraManager.enableOrDisableChangeCameraView (false);

			playerControllerManager.setLadderFoundState (true);

			playerControllerManager.setIgnoreExternalActionsActiveState (true);
		} else {
			playerControllerManager.headBobManager.pauseHeadBodWithDelay (0.5f);
			playerControllerManager.headBobManager.playOrPauseHeadBob (true);

			playerControllerManager.setPauseAllPlayerDownForces (false);

			playerCameraManager.setOriginalchangeCameraViewEnabledValue ();

			playerControllerManager.setLadderFoundState (false);

			playerControllerManager.stepManager.setDefaultGroundFootStepState ();

			playerControllerManager.setIgnoreExternalActionsActiveState (false);
		}
	}

	public void setLadderDirectionTransform (Transform newLadderDirectionTransform, Transform newLadderRaycastDirectionTransform)
	{
		ladderDirectionTransform = newLadderDirectionTransform;
		ladderRaycastDirectionTransform = newLadderRaycastDirectionTransform;
	}

	public void setLadderHorizontalMovementState (bool state)
	{
		useLadderHorizontalMovement = state;
	}

	public void setMoveInLadderCenterState (bool state)
	{
		moveInLadderCenter = state;
	}

	public bool isLadderFound ()
	{
		return ladderFound;
	}

	public void setUseLocalMovementDirectionState (bool state)
	{
		useLocalMovementDirection = state;
	}

	public void checkLadderEventsOnThirdPerson (bool state)
	{
		if (useEventsOnThirdPerson) {
			if (state) {
				eventOnLocatedLadderOnThirdPerson.Invoke ();
			} else {
				eventOnRemovedLadderOnThirdPerson.Invoke ();
			}
		}
	}
}
