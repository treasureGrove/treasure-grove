using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class headTrack : OnAnimatorIKComponent
{
	public bool headTrackEnabled = true;

	public Animator animator;
	public playerController playerControllerManager;
	public playerCamera playerCameraManager;
	public IKSystem IKManager;

	public bool useHeadRangeRotation = true;
	public Vector2 rangeAngleX = new Vector2 (-90, 90);
	public Vector2 rangeAngleY = new Vector2 (-90, 90);

	public Transform head;

	[Range (0, 1)] public float headWeight = 1;
	[Range (0, 1)] public float bodyWeight = 0.4f;
	public float rotationSpeed = 3;
	public float weightChangeSpeed = 2;
	public bool useTimeToChangeTarget;
	public float timeToChangeTarget;
	public LayerMask obstacleLayer;

	public bool showGizmo;
	public float gizmoRadius = 0.2f;
	public float arcGizmoRadius;

	public bool lookInCameraDirection;
	public Transform cameraTargetToLook;
	public Vector2 cameraRangeAngleX = new Vector2 (-90, 90);
	public Vector2 cameraRangeAngleY = new Vector2 (-90, 90);
	[Range (0, 1)] public float cameraHeadWeight = 1;
	[Range (0, 1)] public float cameraBodyWeight = 0.4f;

	public bool lookInOppositeDirectionOutOfRange;
	public Transform oppositeCameraTargetToLook;
	public Transform oppositeCameraTargetToLookParent;

	public float oppositeCameraParentRotationSpeed;
	public bool lookBehindIfMoving;
	public float lookBehindRotationSpeed;
	float currentParentRotationSpeed;

	public bool useDeadZone;
	public float deadZoneLookBehind = 10;

	public bool playerCanLookState;

	public bool useHeadTrackTarget;
	public Transform headTrackTargeTransform;
	Transform headTrackTargetParent;
	Vector3 originalHeadTrackTargetTransformPosition;

	Vector2 currentRangeAngleX;
	Vector2 currentRangeAngleY;
	float currentHeadWeightValue;
	float currentBodyWeightValue;

	public List<headTrackTarget> lookTargetList = new List<headTrackTarget> ();

	public bool useTargetsToIgnoreList;
	public List<GameObject> targetsToIgnoreList = new List<GameObject> ();

	public float currentHeadWeight;
	public float currentbodyWeight;

	public bool useSmoothHeadTrackDisable;

	public float maxDistanceToHeadToLookAtCameraTarget = 1;

	public bool headTrackActiveWhileAiming;


	float distanceToHead;

	float headHeight;
	float lastTimeTargetChanged;

	headTrackTarget currentLookTarget;

	bool currentLookTargetLocated;

	Vector3 currentPositionToLook;
	Vector3 currentDirectionToLook;
	public Vector3 temporalDirectionToLook;

	Vector3 IKDirection;
	Vector3 IKTargetDirection;

	bool headTrackPaused;

	public bool positionToLookFound;

	Vector3 positionToLook;
	Vector3 headPosition;
	bool lookingInCameraDirection;
	bool currentTargetVisible;
	bool targetOnHeadRange;
	float lookTargetDistance;
	bool lookingAtRight;


	Vector3 currentOppositeCameraParentRotation;

	float originalCameraBodyWeight;
	float cameraBodyWeightTarget;

	float cameraBodyWeightSpeed = 2;

	bool externalHeadTrackPauseActive;

	Transform cameraPivotTransform;

	bool playerIsAiming;

	bool isCheckToKeepWeaponAfterAimingWeaponFromShooting;

	float currentHorizontalAngle;

	float currentHorizontalAngleABS;

	bool lookTargetLocated;

	float longDistance = 1000;

	Vector3 transformForward;
	Vector3 transformUp;
	Vector3 transformPosition;

	int lookTargetListCount;

	float temporalHorizontalAngle;
	float temporalVerticalAngle;

	bool originalLookInOppositeDirectionOutOfRangeValue;

	void Start ()
	{
		head = animator.GetBoneTransform (HumanBodyBones.Head);

		if (head != null) {
			headHeight = GKC_Utils.distance (transform.position, head.position);
		}

		originalCameraBodyWeight = cameraBodyWeight;
		cameraBodyWeightTarget = cameraBodyWeight;

		if (useHeadTrackTarget) {
			originalHeadTrackTargetTransformPosition = headTrackTargeTransform.localPosition;
			headTrackTargetParent = headTrackTargeTransform.parent;
		}

		cameraPivotTransform = playerCameraManager.getPivotCameraTransform ();

		originalLookInOppositeDirectionOutOfRangeValue = lookInOppositeDirectionOutOfRange;
	}

	public void setSmoothHeadTrackDisableState (bool state)
	{
		useSmoothHeadTrackDisable = state;
	}

	public void setExternalHeadTrackPauseActiveState (bool state)
	{
		externalHeadTrackPauseActive = state;
	}

	public void setHeadTrackSmoothPauseState (bool state)
	{
		externalHeadTrackPauseActive = state;
		useSmoothHeadTrackDisable = state;
	}

	public float lookInOppositeDirectionExtraRange = 20;

	bool checkToLookAtLeft;
	bool checkToLookAtRight = true;

	public override void updateOnAnimatorIKState ()
	{
		if (!headTrackEnabled) {
			return;
		}

		transformForward = transform.forward;

		transformUp = transform.up;

		transformPosition = transform.position;

		if (lookInOppositeDirectionOutOfRange) {
			currentHorizontalAngle = Vector3.SignedAngle (transformForward, playerCameraManager.transform.forward, transformUp);

			currentHorizontalAngleABS = Math.Abs (currentHorizontalAngle);

			if (checkToLookAtRight) {
				if (currentHorizontalAngle < 0) {
					if (currentHorizontalAngleABS < 180 - lookInOppositeDirectionExtraRange) {
						checkToLookAtLeft = true;

						checkToLookAtRight = false;
					}
				}
			} else {
				if (checkToLookAtLeft) {
					if (currentHorizontalAngle > 0) {
						if (currentHorizontalAngleABS < 180 - lookInOppositeDirectionExtraRange) {
							checkToLookAtLeft = false;

							checkToLookAtRight = true;
						}
					}
				}
			}
				
			if (checkToLookAtRight) {
				if (currentHorizontalAngle > 0) {
					currentHorizontalAngle = 180 - currentHorizontalAngle;
				} else {
					currentHorizontalAngle = -(180 + currentHorizontalAngle);
				}

				lookingAtRight = true;
			} 

			if (checkToLookAtLeft) {
				if (currentHorizontalAngle < 0) {
					currentHorizontalAngle = -(180 + currentHorizontalAngle);
				} else {
					currentHorizontalAngle = 180 - currentHorizontalAngle;
				}

				lookingAtRight = false;
			}

			currentHorizontalAngleABS = Math.Abs (currentHorizontalAngle);

			if (lookBehindIfMoving && (playerControllerManager.getVerticalInput () < -0.5f) && currentHorizontalAngleABS < 30) {

				if (!useDeadZone || currentHorizontalAngleABS > deadZoneLookBehind) {
					if (lookingAtRight) {
						currentHorizontalAngle = cameraRangeAngleY.y;
					} else {
						currentHorizontalAngle = cameraRangeAngleY.x;
					}
				}

				currentParentRotationSpeed = lookBehindRotationSpeed;
			} else {
				currentParentRotationSpeed = oppositeCameraParentRotationSpeed;
			}

			currentOppositeCameraParentRotation = new Vector3 (cameraPivotTransform.localEulerAngles.x, currentHorizontalAngle, 0);

			oppositeCameraTargetToLookParent.localRotation = 
				Quaternion.Lerp (oppositeCameraTargetToLookParent.localRotation, Quaternion.Euler (currentOppositeCameraParentRotation), Time.deltaTime * currentParentRotationSpeed);
		}

		playerCanLookState = playerCanLook ();

		if (!playerControllerManager.isPlayerOnFirstPerson () && playerCanLookState && !headTrackPaused && !externalHeadTrackPauseActive) {
			updateHeadTrack ();

			if (lookTargetList.Count == 0 && !lookInCameraDirection &&
			    (currentDirectionToLook == Vector3.zero || IKDirection == Vector3.zero)) {
				//print ("pause");
				headTrackPaused = true;
			}

		} else {
			if (currentHeadWeight != 0 && currentbodyWeight != 0) {
				lerpWeights (0, 0);
			}

			if (useSmoothHeadTrackDisable) {
				updateHeadTrack ();

				if (currentHeadWeight == 0 && currentbodyWeight == 0) {
					useSmoothHeadTrackDisable = false;
				}
			}
		}
	}

	public void updateHeadTrack ()
	{
		animator.SetLookAtWeight (currentHeadWeight, currentbodyWeight);

		currentPositionToLook = getLookPosition ();

		animator.SetLookAtPosition (currentPositionToLook);
	}

	public bool playerCanLook ()
	{ 
		if (playerControllerManager.canHeadTrackBeUsed () && !playerControllerManager.driving) {

			playerIsAiming = playerControllerManager.isPlayerAiming ();

			isCheckToKeepWeaponAfterAimingWeaponFromShooting = playerControllerManager.isCheckToKeepWeaponAfterAimingWeaponFromShooting ();

			if (!playerIsAiming && IKManager.getHeadWeight () == 0) {
				return true;
			}
				
			if (!playerControllerManager.hasToLookInCameraDirectionOnFreeFire () && !isCheckToKeepWeaponAfterAimingWeaponFromShooting) {
				return true;
			}
			
			if (playerIsAiming && IKManager.getHeadWeight () == 0 && isCheckToKeepWeaponAfterAimingWeaponFromShooting) {
				return true;
			}

			if (playerIsAiming && headTrackActiveWhileAiming) {
				return true;
			}

			return false;
		}

		return false;
	}

	public Vector3 getLookPosition ()
	{
		//get the head position
		headPosition = transformPosition + (transformUp * headHeight);

		positionToLookFound = false;

		currentTargetVisible = false;

		//if the player is inside a head track target trigger, check if he can look at it
		if (currentLookTargetLocated) {
			//check if the target is visible according to its configuration
			if (currentLookTarget.lookTargetVisible (headPosition, obstacleLayer)) {
				//get the look position
				positionToLook = currentLookTarget.getLookPositon ();

				//assign the range values
				currentRangeAngleX = rangeAngleX;
				currentRangeAngleY = rangeAngleY;
				currentHeadWeightValue = headWeight;
				currentBodyWeightValue = bodyWeight;

				//check if it the look direction is inside the range
				targetOnHeadRange = isTargetOnHeadRange (positionToLook - headPosition);

				//in that case, set the found position as the one to look
				if (targetOnHeadRange) {
					positionToLookFound = true;

					lookingInCameraDirection = false;
				}

				currentTargetVisible = true;
			} 
		}

		//if there is no target to look or it can be seen by the player or is out of the range of vision, check if the player can look in the camera direction
		if (!positionToLookFound) {
			if (lookInCameraDirection && !playerControllerManager.isLockedCameraStateActive () &&
			    playerCameraManager.isCurrentCameraStateHeadTrackActive ()) {

				distanceToHead = GKC_Utils.distance (head.position, cameraPivotTransform.position);

				if (distanceToHead < maxDistanceToHeadToLookAtCameraTarget) {

					//get the look position
					positionToLook = cameraTargetToLook.position;

					//assign the range values
					currentRangeAngleX = cameraRangeAngleX;
					currentRangeAngleY = cameraRangeAngleY;
					currentHeadWeightValue = cameraHeadWeight;
					currentBodyWeightValue = cameraBodyWeight;

					if (cameraBodyWeight != cameraBodyWeightTarget) {
						cameraBodyWeight = Mathf.Lerp (cameraBodyWeight, cameraBodyWeightTarget, cameraBodyWeightSpeed * Time.deltaTime);
					}

					//check if it the look direction is inside the range
					targetOnHeadRange = isTargetOnHeadRange (positionToLook - headPosition);

					//in that case, set the found position as the one to look
					if (targetOnHeadRange) {
						positionToLookFound = true;

						lookingInCameraDirection = true;
					} else {
						if (lookInOppositeDirectionOutOfRange) {
							positionToLook = oppositeCameraTargetToLook.position;

							targetOnHeadRange = true;
							positionToLookFound = true;

							lookingInCameraDirection = true;
						}
					}
				}
			} 
		}

		if (positionToLookFound) {
			temporalDirectionToLook = headPosition + transformForward - headPosition;

			if ((useHeadRangeRotation && targetOnHeadRange) || !useHeadRangeRotation) {
				if (currentTargetVisible || (lookInCameraDirection && lookingInCameraDirection)) {
					temporalDirectionToLook = positionToLook - headPosition;
				}
			}

			if (showGizmo) {
				lookTargetDistance = GKC_Utils.distance (headPosition, positionToLook);
				Debug.DrawRay (headPosition, temporalDirectionToLook.normalized * lookTargetDistance, Color.black);
			}

			if (useHeadRangeRotation) {
				if (targetOnHeadRange) {
					if (!useSmoothHeadTrackDisable) {
						lerpWeights (currentHeadWeightValue, currentBodyWeightValue);
					}
				} else
					lerpWeights (0, 0);
			} else {
				lerpWeights (currentHeadWeightValue, currentBodyWeightValue);
			}

			getClosestTarget ();

			IKTargetDirection = temporalDirectionToLook;
		} else {
			lerpWeights (0, 0);

			getClosestTarget ();

			temporalDirectionToLook = transformForward;

			if (lookInCameraDirection) {
				IKTargetDirection = transformForward;
			} else {
				IKTargetDirection = Vector3.zero;
			}
		}

		currentDirectionToLook = Vector3.Lerp (currentDirectionToLook, temporalDirectionToLook, Time.deltaTime * rotationSpeed);

		IKDirection = Vector3.Lerp (IKDirection, IKTargetDirection, Time.deltaTime * rotationSpeed);

		//Debug.DrawLine (headPosition, currentDirectionToLook, Color.white);
		return headPosition + currentDirectionToLook;
	}

	public bool isTargetOnHeadRange (Vector3 direction)
	{
		temporalHorizontalAngle = Vector3.SignedAngle (transformForward, direction, transformUp);
		temporalVerticalAngle = Vector3.SignedAngle (transformForward, direction, transform.right);

		if (Math.Abs (temporalVerticalAngle) <= currentRangeAngleX.y && temporalVerticalAngle >= currentRangeAngleX.x &&
		    Math.Abs (temporalHorizontalAngle) <= currentRangeAngleY.y && temporalHorizontalAngle >= currentRangeAngleY.x) {
			return true;
		}

		return false;
	}

	public void lerpWeights (float headWeightTarget, float bodyWeightTarget)
	{
		if (currentHeadWeight != headWeightTarget) {
			currentHeadWeight = Mathf.Lerp (currentHeadWeight, headWeightTarget, weightChangeSpeed * Time.deltaTime);
		}

		if (currentbodyWeight != bodyWeightTarget) {
			currentbodyWeight = Mathf.Lerp (currentbodyWeight, bodyWeightTarget, weightChangeSpeed * Time.deltaTime);
		}
	}

	public void getClosestTarget ()
	{
		if (lookTargetLocated) {
			lookTargetListCount = lookTargetList.Count;
			
			if (lookTargetListCount > 0) {
			
				if (!useTimeToChangeTarget || Time.time > lastTimeTargetChanged + timeToChangeTarget) {
					lastTimeTargetChanged = Time.time;

					for (int i = lookTargetList.Count - 1; i >= 0; i--) {
						if (lookTargetList [i] == null) {
							lookTargetList.RemoveAt (i);
						}
					}

					lookTargetListCount = lookTargetList.Count;

					if (lookTargetListCount == 0) {
						lookTargetLocated = false;

						return;
					}

					if (lookTargetListCount == 1) {
						currentLookTarget = lookTargetList [0];

						currentLookTargetLocated = true;

						return;
					}

					float maxDistance = longDistance;

					for (int i = 0; i < lookTargetListCount; i++) {
						float currentDistance = GKC_Utils.distance (transformPosition, lookTargetList [i].getLookPositon ());

						if (currentDistance < maxDistance) {
							maxDistance = currentDistance;

							currentLookTarget = lookTargetList [i];

							currentLookTargetLocated = true;
						}
					}
				} 
			}
		} else {
			currentLookTarget = null;

			currentLookTargetLocated = false;
		}
	}

	public void checkHeadTrackTarget (headTrackTarget target)
	{
		if (useTargetsToIgnoreList && target != null && targetsToIgnoreList.Contains (target.gameObject)) {
			return;
		}

		if (!lookTargetList.Contains (target)) {
			lookTargetList.Add (target);

			headTrackPaused = false;

			lookTargetLocated = true;
		}
	}

	public void removeHeadTrackTarget (headTrackTarget target)
	{
		if (lookTargetList.Contains (target)) {
			lookTargetList.Remove (target);

			if (lookTargetList.Count == 0) {
				lookTargetLocated = false;
			}
		}
	}

	public void setHeadTransform (Transform headTransform)
	{
		head = headTransform;
	}

	public void searchHead ()
	{
		head = animator.GetBoneTransform (HumanBodyBones.Head);

		updateComponent ();
	}

	public void setCameraBodyWeightValue (float newValue)
	{
		cameraBodyWeightTarget = newValue;
	}

	public void setOriginalCameraBodyWeightValue ()
	{
		cameraBodyWeightTarget = originalCameraBodyWeight;
	}

	public Transform getHeadTrackTargetTransform ()
	{
		return headTrackTargeTransform;
	}

	public Transform getHeadTrackTargetParent ()
	{
		return headTrackTargetParent;
	}

	public Vector3 getOriginalHeadTrackTargetPosition ()
	{
		return originalHeadTrackTargetTransformPosition;
	}

	public void setHeadTrackEnabledStateFromEditor (bool state)
	{
		setHeadTrackEnabledState (state);

		updateComponent ();
	}

	public void setHeadTrackEnabledState (bool state)
	{
		headTrackEnabled = state;
	}

	public bool isHeadTrackEnabled ()
	{
		return headTrackEnabled;
	}

	public void setHeadTrackActiveWhileAimingState (bool state)
	{
		headTrackActiveWhileAiming = state;
	}

	public void setHeadWeight (float newValue)
	{
		headWeight = newValue;
	}

	public void setBodyWeight (float newValue)
	{
		bodyWeight = newValue;
	}

	public void setLookInOppositeDirectionOutOfRangeValue (bool state)
	{
		lookInOppositeDirectionOutOfRange = state;
	}

	public void setOriginalLookInOppositeDirectionOutOfRangeValue ()
	{
		setLookInOppositeDirectionOutOfRangeValue (originalLookInOppositeDirectionOutOfRangeValue);
	}

	//Editor functions
	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}
			
		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo && Application.isPlaying) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (currentPositionToLook, gizmoRadius);
		}
	}
}