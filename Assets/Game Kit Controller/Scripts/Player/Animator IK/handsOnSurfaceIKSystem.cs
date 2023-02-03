using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class handsOnSurfaceIKSystem : OnAnimatorIKComponent
{
	[Header ("Main Settings")]
	[Space]

	public bool adjustHandsToSurfacesEnabled = true;

	public LayerMask layerToAdjustHandsToSurfaces;

	public float movementSpeedLerpSpeed = 2;

	public float waitTimeAfterGroundToCheckSurface = 1.5f;
	public float waitTimeAfterCrouchToCheckSurface = 1.5f;

	public float weightLerpSpeed = 6;

	public float maxHandsDistance = 3;

	public float minMovingInputToUseWalkSpeed = 0.5f;
	public float minIKWeightToUseWalkSpeed = 0.4f;

	[Space]
	[Header ("Hands Info List Settings")]
	[Space]

	public List<handsToSurfaceInfo> handsToSurfaceInfoList = new List<handsToSurfaceInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingHands;
	public bool adjustHandsPaused;

	public bool smoothBusyDisableActive;

	public bool handsCheckPausedWithDelayActive;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.white;
	public float gizmoRadius;

	[Space]
	[Header ("Component Elements")]
	[Space]

	public Animator animator;
	public IKSystem IKManager;
	public playerController playerControllerManager;
	public Transform playerTransform;

	handsToSurfaceInfo currentHandInfo;
	Vector3 rayDirection;

	RaycastHit hit;
	float movementSpeed;
	float currentMovementSpeed;

	bool isThirdPersonView;
	bool isBusy;

	bool onGround;

	bool movingOnPlatformActive;

	bool dead;

	bool groundChecked;
	float lastTimeOnGround;

	bool crouchingChecked;
	float lastTimeCrouch;

	Vector3 currentPositionCenter;

	float currentRotationSpeed;

	float originalWeightLerpSpeed;

	bool crouching;

	Transform rightHandTransform;
	Transform leftHandTransform;

	bool isRightHand;

	multipleRayInfo currentMultipleRayInfo;

	Vector3 currentTransformUp;

	Vector3 currentTransformForward;

	Vector3 vector3Zero = Vector3.zero;

	Quaternion targetRotation;

	Vector3 currentHandPosition;

	float currentWeightToApply;

	AvatarIKGoal currentIKGoal;

	int handsToSurfaceInfoListCount;

	float minIKValue = 0.001f;

	float currentDeltaTime;

	Coroutine delayToResumeStateCoroutine;


	void Start ()
	{
		originalWeightLerpSpeed = weightLerpSpeed;

		rightHandTransform = animator.GetBoneTransform (HumanBodyBones.RightHand);

		leftHandTransform = animator.GetBoneTransform (HumanBodyBones.LeftHand);

		handsToSurfaceInfoListCount = handsToSurfaceInfoList.Count;
	}

	void FixedUpdate ()
	{
		if (!adjustHandsToSurfacesEnabled) {
			return;
		}

		isThirdPersonView = !playerControllerManager.isPlayerOnFirstPerson ();

		usingHands = IKManager.getUsingHands ();

		onGround = playerControllerManager.isPlayerOnGround ();

		movingOnPlatformActive = playerControllerManager.isMovingOnPlatformActive ();

		isBusy = playerIsBusy ();

		crouching = playerControllerManager.isCrouching ();

		dead = playerControllerManager.isPlayerDead ();

		if (groundChecked != onGround) {
			groundChecked = onGround;

			if (onGround) {
				lastTimeOnGround = Time.time;
			}
		}

		if (crouchingChecked != crouching) {
			crouchingChecked = crouching;

			if (!crouching) {
				lastTimeCrouch = Time.time;
			}
		}

		if (adjustHandsToSurfacesEnabled) {
			if (!isBusy || smoothBusyDisableActive) {

				currentTransformUp = playerTransform.up;
				currentTransformForward = playerTransform.forward;

				for (int j = 0; j < handsToSurfaceInfoListCount; j++) {

					currentHandInfo = handsToSurfaceInfoList [j];

					currentHandInfo.surfaceFound = false;
					currentHandInfo.multipleRaycastHitNormal = vector3Zero;
					currentHandInfo.multipleRaycastHitPoint = vector3Zero;
					currentHandInfo.numberOfSurfacesFound = 0;
					currentHandInfo.multipleRaycastDirection = vector3Zero;

					for (int k = 0; k < currentHandInfo.raycastTransformList.Count; k++) {
						currentMultipleRayInfo = currentHandInfo.raycastTransformList [k];

						rayDirection = currentMultipleRayInfo.raycastTransform.forward;

						if (currentMultipleRayInfo.raycastEnabled &&
						    Physics.Raycast (currentMultipleRayInfo.raycastTransform.position, rayDirection, out hit, 
							    currentMultipleRayInfo.rayCastDistance, layerToAdjustHandsToSurfaces)) {

							currentMultipleRayInfo.hitPoint = hit.point;
							currentMultipleRayInfo.hitNormal = hit.normal;
							currentHandInfo.surfaceFound = true;
							currentHandInfo.multipleRaycastHitNormal += hit.normal;
							currentHandInfo.multipleRaycastHitPoint += hit.point;

							currentHandInfo.numberOfSurfacesFound++;
							currentHandInfo.multipleRaycastDirection += rayDirection;

							if (showGizmo) {
								Debug.DrawRay (currentMultipleRayInfo.raycastTransform.position, rayDirection * hit.distance, Color.green);
							}
						} else {
							if (showGizmo) {
								Debug.DrawRay (currentMultipleRayInfo.raycastTransform.position, rayDirection * currentMultipleRayInfo.rayCastDistance, Color.red);
							}
						}
					}

					if (smoothBusyDisableActive) {
						currentHandInfo.surfaceFound = false;

						if (currentWeightInBothHandsEqualTo (0)) {
							smoothBusyDisableActive = false;
						}
					}

					if (currentHandInfo.surfaceFound) {
						currentHandInfo.handPosition = 
							(currentHandInfo.multipleRaycastHitPoint / currentHandInfo.numberOfSurfacesFound) +
						(currentHandInfo.handSurfaceOffset * currentHandInfo.multipleRaycastDirection / currentHandInfo.numberOfSurfacesFound);      

						targetRotation = Quaternion.LookRotation (currentHandInfo.multipleRaycastDirection / currentHandInfo.numberOfSurfacesFound);           

						currentHandInfo.handRotation = 
							Quaternion.FromToRotation (currentTransformUp, currentHandInfo.multipleRaycastHitNormal / currentHandInfo.numberOfSurfacesFound) * targetRotation;   

						currentHandInfo.elbowPosition =
							((currentHandInfo.multipleRaycastHitPoint / currentHandInfo.numberOfSurfacesFound) + currentHandInfo.raycastTransform.position) / 2 - currentTransformUp * 0.1f;

						currentHandInfo.handWeight = 1;

						if (currentHandInfo.useMinDistance) {
							currentHandInfo.currentDistance = GKC_Utils.distance (currentHandInfo.raycastTransform.position, currentHandInfo.handPosition);

							if (currentHandInfo.currentDistance < currentHandInfo.minDistance) {
								currentHandInfo.surfaceFound = false;
							}
						}
					} 

					if (handsCheckPausedWithDelayActive ||
					    !currentHandInfo.surfaceFound ||
					    movingOnPlatformActive ||
					    !onGround ||
					    Time.time < waitTimeAfterGroundToCheckSurface + lastTimeOnGround ||
					    crouching ||
					    Time.time < waitTimeAfterCrouchToCheckSurface + lastTimeCrouch ||
					    dead) {

						currentHandInfo.handPosition = currentHandInfo.noSurfaceHandPosition.position;
						currentHandInfo.handRotation = Quaternion.LookRotation (currentTransformForward);

						currentHandInfo.elbowPosition = currentHandInfo.noSurfaceElbowPosition.position;

						currentHandInfo.handWeight = 0;
					}

					if (playerControllerManager.isPlayerMoving (minMovingInputToUseWalkSpeed) &&
					    currentHandInfo.surfaceFound && currentHandInfo.currentHandWeight > minIKWeightToUseWalkSpeed) {
						movementSpeed = currentHandInfo.walkingMovementSpeed;
					} else {
						movementSpeed = currentHandInfo.movementSpeed;
					}

					currentDeltaTime = Time.deltaTime;

					currentMovementSpeed = Mathf.Lerp (currentMovementSpeed, movementSpeed, currentDeltaTime * movementSpeedLerpSpeed);

					currentHandInfo.currentHandPosition = Vector3.Lerp (currentHandInfo.currentHandPosition, currentHandInfo.handPosition, currentDeltaTime * currentMovementSpeed);            

					currentRotationSpeed = currentDeltaTime * currentHandInfo.rotationSpeed;

					if (currentRotationSpeed > 0) {
						currentHandInfo.currentHandRotation = Quaternion.Lerp (currentHandInfo.currentHandRotation, currentHandInfo.handRotation, currentRotationSpeed);
					}

					currentHandInfo.currentElbowPosition = Vector3.Lerp (currentHandInfo.currentElbowPosition, currentHandInfo.elbowPosition, currentDeltaTime * currentMovementSpeed);            

					currentHandInfo.currentHandWeight = Mathf.Lerp (currentHandInfo.currentHandWeight, currentHandInfo.handWeight, currentDeltaTime * weightLerpSpeed);

					if (currentHandInfo.currentHandWeight < 0.01f) {
						currentHandInfo.handPosition = currentHandInfo.noSurfaceHandPosition.position;
						currentHandInfo.handRotation = Quaternion.LookRotation (currentTransformForward);

						currentHandInfo.elbowPosition = currentHandInfo.noSurfaceElbowPosition.position;

						currentHandInfo.currentHandPosition = currentHandInfo.handPosition;            

						currentHandInfo.currentHandRotation = currentHandInfo.handRotation;

						currentHandInfo.currentElbowPosition = currentHandInfo.elbowPosition;
					}
				}
			} else {

				currentTransformForward = playerTransform.forward;

				for (int j = 0; j < handsToSurfaceInfoListCount; j++) {

					currentHandInfo = handsToSurfaceInfoList [j];

					currentHandInfo.surfaceFound = false;

					currentHandInfo.handWeight = 0;

					currentHandInfo.currentHandWeight = 0;
					currentHandInfo.handPosition = currentHandInfo.noSurfaceHandPosition.position;
					currentHandInfo.handRotation = Quaternion.LookRotation (currentTransformForward);

					currentHandInfo.elbowPosition = currentHandInfo.noSurfaceElbowPosition.position;

					currentHandInfo.currentHandPosition = currentHandInfo.handPosition;            

					currentHandInfo.currentHandRotation = currentHandInfo.handRotation;

					currentHandInfo.currentElbowPosition = currentHandInfo.elbowPosition;
				}

			}
		}
	}

	public bool currentWeightInBothHandsEqualTo (float weightValue)
	{
		int numberOfHands = 0;

		for (int j = 0; j < handsToSurfaceInfoListCount; j++) {
			if (handsToSurfaceInfoList [j].currentHandWeight == weightValue) {
				numberOfHands++;
			}
		}

		if (numberOfHands == 2) {
			return true;
		}

		return false;
	}

	public override void updateOnAnimatorIKState ()
	{
		if (adjustHandsToSurfacesEnabled) {

			if (!isBusy || smoothBusyDisableActive) {

				currentPositionCenter = playerTransform.position + playerTransform.up;

				for (int j = 0; j < handsToSurfaceInfoListCount; j++) {

					currentHandInfo = handsToSurfaceInfoList [j];

					isRightHand = currentHandInfo.isRightHand;

					currentHandPosition = currentHandInfo.currentHandPosition;

					if (float.IsNaN (currentHandPosition.x)) {
						if (isRightHand) {
							currentHandPosition.x = rightHandTransform.position.x;
						} else {
							currentHandPosition.x = leftHandTransform.position.x;
						}
					}

					if (float.IsNaN (currentHandPosition.y)) {
						if (isRightHand) {
							currentHandPosition.y = rightHandTransform.position.y;
						} else {
							currentHandPosition.y = leftHandTransform.position.y;
						}
					}

					if (float.IsNaN (currentHandPosition.z)) {
						if (isRightHand) {
							currentHandPosition.z = rightHandTransform.position.z;
						} else {
							currentHandPosition.z = leftHandTransform.position.z;
						}
					}

					currentHandPosition.x = 
						Mathf.Clamp (currentHandPosition.x, currentPositionCenter.x - maxHandsDistance, currentPositionCenter.x + maxHandsDistance);
					currentHandPosition.y =
						Mathf.Clamp (currentHandPosition.y, currentPositionCenter.y - maxHandsDistance, currentPositionCenter.y + maxHandsDistance);
					currentHandPosition.z = 
						Mathf.Clamp (currentHandPosition.z, currentPositionCenter.z - maxHandsDistance, currentPositionCenter.z + maxHandsDistance);

					currentHandInfo.currentHandPosition = currentHandPosition;


					currentWeightToApply = currentHandInfo.currentHandWeight;

					if (currentWeightToApply > minIKValue) {

						currentIKGoal = currentHandInfo.IKGoal;

						animator.SetIKRotationWeight (currentIKGoal, currentWeightToApply);
						animator.SetIKPositionWeight (currentIKGoal, currentWeightToApply);

						animator.SetIKPosition (currentIKGoal, currentHandInfo.currentHandPosition);
						animator.SetIKRotation (currentIKGoal, currentHandInfo.currentHandRotation);

						animator.SetIKHintPositionWeight (currentHandInfo.IKHint, currentWeightToApply);
						animator.SetIKHintPosition (currentHandInfo.IKHint, currentHandInfo.currentElbowPosition);
					}
				}
			} 
		}
	}

	public bool playerIsBusy ()
	{
		return 

			usingHands ||
		!isThirdPersonView ||
		playerControllerManager.isPlayerOnFFOrZeroGravityModeOn () ||
		playerControllerManager.isUsingCloseCombatActive () ||
		adjustHandsPaused ||
		playerControllerManager.isUsingJetpack () ||
		playerControllerManager.isFlyingActive () ||
		playerControllerManager.isSwimModeActive ();
	}

	public void setWeightLerpSpeedValue (float newValue)
	{
		weightLerpSpeed = newValue;
	}

	public void setOriginalWeightLerpSpeed ()
	{
		setWeightLerpSpeedValue (originalWeightLerpSpeed);
	}

	public void setAdjustHandsPausedState (bool state)
	{
		adjustHandsPaused = state;
	}

	public void setSmoothBusyDisableActiveState (bool state)
	{
		smoothBusyDisableActive = state;
	}

	public void setAdjustHandsToSurfacesEnabledState (bool state)
	{
		adjustHandsToSurfacesEnabled = state;
	}


	public void enableHandsCheckPausedWithDelayActive (float duration)
	{
		stopUpdateDelayToResumeStateCoroutine ();

		delayToResumeStateCoroutine = StartCoroutine (updateDelayToResumeStateCoroutine (duration));
	}

	public void stopUpdateDelayToResumeStateCoroutine ()
	{
		if (delayToResumeStateCoroutine != null) {
			StopCoroutine (delayToResumeStateCoroutine);
		}

		handsCheckPausedWithDelayActive = false;
	}

	IEnumerator updateDelayToResumeStateCoroutine (float duration)
	{
		handsCheckPausedWithDelayActive = true;

		yield return new WaitForSeconds (duration);

		handsCheckPausedWithDelayActive = false;
	}


	//EDITOR FUNCTIONS
	public void setAdjustHandsToSurfacesEnabledStateFromEditor (bool state)
	{
		setAdjustHandsToSurfacesEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
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
		if (showGizmo) {
			for (int j = 0; j < handsToSurfaceInfoList.Count; j++) {
				Gizmos.color = Color.green;
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].handPosition, gizmoRadius);

				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].elbowPosition, gizmoRadius);

				Gizmos.color = gizmoColor;
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].currentHandPosition, gizmoRadius);
				Gizmos.DrawSphere (handsToSurfaceInfoList [j].currentElbowPosition, gizmoRadius);
			}
		}
	}

	[System.Serializable]
	public class handsToSurfaceInfo
	{
		public string Name;
		public Transform raycastTransform;

		public bool useMultipleRaycast;
		public List<multipleRayInfo> raycastTransformList = new List<multipleRayInfo> ();

		public AvatarIKHint IKHint;
		[HideInInspector] public Vector3 elbowPosition;
		[HideInInspector] public Vector3 currentElbowPosition;

		public bool isRightHand;
		public AvatarIKGoal IKGoal;
		public float handSurfaceOffset;
		public float rayCastDistance;

		public float currentHandWeight;

		public float movementSpeed;
		public float rotationSpeed;

		public float walkingMovementSpeed;

		[HideInInspector] public bool surfaceFound;

		public bool useMinDistance;
		public float minDistance;

		[HideInInspector]public float currentDistance;

		[HideInInspector] public Vector3 multipleRaycastDirection;

		[HideInInspector] public int numberOfSurfacesFound;
		[HideInInspector] public Vector3 multipleRaycastHitNormal;
		[HideInInspector] public Vector3 multipleRaycastHitPoint;

		[HideInInspector] public float handWeight;
		[HideInInspector] public Vector3 handPosition;
		[HideInInspector] public Quaternion handRotation;

		[HideInInspector] public Vector3 currentHandPosition;
		[HideInInspector] public Quaternion currentHandRotation;

		public Transform noSurfaceHandPosition;
		public Transform noSurfaceElbowPosition;
	}

	[System.Serializable]
	public class multipleRayInfo
	{
		public Transform raycastTransform;
		public bool surfaceFound;
		[HideInInspector] public Vector3 hitPoint;
		[HideInInspector] public Vector3 hitNormal;
		public float rayCastDistance;
		public bool raycastEnabled = true;
	}
}