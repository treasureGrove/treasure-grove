using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class matchPlayerToTargetSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool matchSystemEnabled = true;

	public float matchSpeed = 10;

	public float positionOffset;

	public bool ignoreCustomPositionOffset;

	public bool useDistancePercentageAsOffset;
	[Range (0, 1)] public float positionOffsetPercentage;


	public float minDistanceToMatchPosition;
	public float maxDistanceToMatchPosition;

	public float minAngleToMatchRotation;

	public float maxAngleToMatchRotation;

	public bool adjustRotationEvenIfNotMoved;

	public float minDistanceToMatchRotation;
	public float maxDistanceToMatchRotation;

	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter;

	public bool useMoveInputAsDirectionToClosestTarget;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool movementActive;

	public List<Transform> charactersAround = new List<Transform> ();

	[Space]
	[Header ("Events")]
	[Space]

	public bool useEventsOnTargetsChange;
	public UnityEvent eventOnTargetsDetected;
	public UnityEvent eventOnTargetsRemoved;

	public bool useEventOnFirstTargetDetected;
	public UnityEvent eventOnFirstTargetDetected;

	public bool useEventOnLastTargetRemoved;
	public UnityEvent eventOnLastTargetRemoved;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform playerTransform;

	public Transform debugTargetToMatchTransform;

	public Camera mainCamera;

	public playerCamera mainPlayerCamera;

	public playerController mainPlayerController;

	public playerInputManager mainPlayerInputManager;

	Coroutine movementCoroutine;

	Vector3 screenPoint;

	Vector3 characterPosition;

	Vector3 centerScreen;

	Vector3 currentPlayerPosition;

	bool matchPositionPaused;


	public void addCharacterAround (Transform newCharacter)
	{
		if (!charactersAround.Contains (newCharacter)) {
			charactersAround.Add (newCharacter);

			checkRemoveEmptyObjects ();

			if (charactersAround.Count == 1) {
				checkEventOnFirstTargetDetected ();
			}

			checkEventsOnTargetsChange (true);

			if (showDebugPrint) {
				print ("Adding target " + newCharacter.name);
			}
		}
	}

	public void removeCharacterAround (Transform newCharacter)
	{
		if (charactersAround.Contains (newCharacter)) {
			charactersAround.Remove (newCharacter);

			checkRemoveEmptyObjects ();

			if (showDebugPrint) {
				print ("Removing target " + newCharacter.name);
			}
		}

		if (charactersAround.Count == 0) {
			checkEventsOnTargetsChange (false);

			checkEventOnLastTargetRemoved ();
		}
	}

	public void setMatchPositionPausedState (bool state)
	{
		matchPositionPaused = state;
	}

	public void setMatchSystemEnabledstate (bool state)
	{
		matchSystemEnabled = state;
	}

	public void activateMatchPosition (float customOffset)
	{
		if (!matchSystemEnabled) {
			return;
		}

		if (matchPositionPaused) {
			return;
		}

		if (debugTargetToMatchTransform != null) {
			activateMatchPosition (debugTargetToMatchTransform, customOffset);

			return;
		}

		float maxDistanceToTarget = Mathf.Infinity;

		Transform transformToMatchPosition = null;

		float currentDistance = 0;

		float currentDistanceToScreenCenter = 0;

		centerScreen = mainPlayerCamera.getScreenCenter ();

		bool ignoreRestOfChecks = false;

		float maxAngleWithTarget = Mathf.Infinity;

		currentPlayerPosition = playerTransform.position;

		Transform currentLockedCameraTransform = mainPlayerCamera.getCurrentLockedCameraTransform ();

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}

		for (int i = charactersAround.Count - 1; i >= 0; i--) {	
			if (charactersAround [i] != null) {
				if (!applyDamage.checkIfDead (charactersAround [i].gameObject)) {

					characterPosition = charactersAround [i].position;

					screenPoint = mainCamera.WorldToScreenPoint (characterPosition);
					currentDistanceToScreenCenter = GKC_Utils.distance (screenPoint, centerScreen);

					bool canBeChecked = false;

					if (useMoveInputAsDirectionToClosestTarget) {
						Vector2 rawAxisvalues = mainPlayerInputManager.rawMovementAxis;

						if (rawAxisvalues != Vector2.zero) {
							Vector3 direction = new Vector3 (characterPosition.x, currentPlayerPosition.y, characterPosition.z) - currentPlayerPosition;

							direction = direction / direction.magnitude;

							Vector3 inputDirection = Vector3.zero;

							if (mainPlayerCamera.isCameraTypeFree ()) {
								inputDirection = (rawAxisvalues.y * mainPlayerCamera.transform.forward + rawAxisvalues.x * mainPlayerCamera.transform.right);	
							} else {
								inputDirection = (rawAxisvalues.y * currentLockedCameraTransform.forward + rawAxisvalues.x * currentLockedCameraTransform.right);	
							}

							inputDirection.Normalize ();

							float angle = Vector3.SignedAngle (inputDirection, direction, playerTransform.up);

							ignoreRestOfChecks = true;

							if (Mathf.Abs (angle) < maxAngleWithTarget) {
								maxAngleWithTarget = Mathf.Abs (angle);

								transformToMatchPosition = charactersAround [i];
							}
						}
					}

					if (!ignoreRestOfChecks) {
						if (useMaxDistanceToCameraCenter && mainPlayerCamera.isCameraTypeFree ()) {
							if (currentDistanceToScreenCenter < maxDistanceToCameraCenter) {
								canBeChecked = true;
							}
						} else {
							canBeChecked = true;
						}

						if (canBeChecked) {
							currentDistance = GKC_Utils.distance (characterPosition, currentPlayerPosition);

							if (currentDistance < maxDistanceToTarget) {
								maxDistanceToTarget = currentDistance;

								transformToMatchPosition = charactersAround [i];
							}
						}
					}
				} else {
					charactersAround.RemoveAt (i);
				}
			} else {
				charactersAround.RemoveAt (i);
			}
		}

		if (transformToMatchPosition != null) {
			activateMatchPosition (transformToMatchPosition, customOffset);
		}

		if (charactersAround.Count == 0) {
			checkEventsOnTargetsChange (false);

			checkEventOnLastTargetRemoved ();
		}
	}

	public void activateMatchPosition (Transform targetToUse, float customOffset)
	{
		stopMovement ();

		if (targetToUse == null) {
			return;
		}

		movementCoroutine = StartCoroutine (activateMatchPositionCoroutine (targetToUse, customOffset));
	}

	public void stopMovement ()
	{
		if (movementCoroutine != null) {
			StopCoroutine (movementCoroutine);
		}

		movementActive = false;
	}

	IEnumerator activateMatchPositionCoroutine (Transform targetToUse, float customOffset)
	{
		movementActive = true;

		bool targetReached = false;

		float angleDifference = 0;

		float positionDifference = 0;

		Vector3 currentPosition = playerTransform.position;

		Vector3 direction = new Vector3 (targetToUse.position.x, currentPosition.y, targetToUse.position.z) - currentPosition;

		direction = direction / direction.magnitude;

		float angle = Vector3.SignedAngle (playerTransform.forward, direction, playerTransform.up);

		float distanceToTarget = GKC_Utils.distance (targetToUse.position, currentPosition);

		float t = 0;

		float movementTimer = 0;

		float currentPositionOffset = positionOffset;

		if (customOffset != 0 && !ignoreCustomPositionOffset) {
			currentPositionOffset = customOffset;
		} else {
			if (useDistancePercentageAsOffset) {
				currentPositionOffset = distanceToTarget * positionOffsetPercentage;
			}
		}


		float targetToUseRadius = GKC_Utils.getCharacterRadius (targetToUse);

		currentPositionOffset += mainPlayerController.getCharacterRadius () + targetToUseRadius;

		Vector3 targetPosition = currentPosition + direction * (distanceToTarget - currentPositionOffset);

		Quaternion targetRotation = Quaternion.Euler (playerTransform.eulerAngles + playerTransform.up * angle);

		float dist = GKC_Utils.distance (targetPosition, currentPosition);

		float duration = dist / matchSpeed;

		bool positionChangeActive = false;

		bool rotationChangeActive = false;

		if (distanceToTarget > minDistanceToMatchPosition && distanceToTarget < maxDistanceToMatchPosition) {
			positionChangeActive = true;
		}

		if (positionChangeActive ||
		    (adjustRotationEvenIfNotMoved && distanceToTarget > minDistanceToMatchRotation && distanceToTarget < maxDistanceToMatchRotation)) {

			if (Mathf.Abs (angle) > minAngleToMatchRotation && Mathf.Abs (angle) < maxAngleToMatchRotation) {
				rotationChangeActive = true;
			}
		}

		while (!targetReached) {
			t += Time.deltaTime / duration;
			if (positionChangeActive) {
				playerTransform.position = Vector3.Lerp (playerTransform.position, targetPosition, t);
			}

			if (rotationChangeActive) {
				playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, targetRotation, t);
			}

			angleDifference = Quaternion.Angle (playerTransform.rotation, targetRotation);

			positionDifference = GKC_Utils.distance (playerTransform.position, targetPosition);

			movementTimer += Time.deltaTime;

			if (positionDifference < 0.01f) {
				targetReached = true;
			}

			if (angleDifference < 0.2f) {
				targetReached = true;
			}

			if (movementTimer > (duration + 1)) {
				targetReached = true;
			}

			if (!positionChangeActive && !rotationChangeActive) {
				targetReached = true;
			}

			yield return null;
		}

		movementActive = false;
	}

	void checkEventsOnTargetsChange (bool state)
	{
		if (useEventsOnTargetsChange) {
			if (state) {
				eventOnTargetsDetected.Invoke ();
			} else {
				eventOnTargetsRemoved.Invoke ();
			}
		}
	}

	void checkEventOnFirstTargetDetected ()
	{
		if (useEventOnFirstTargetDetected) {
			eventOnFirstTargetDetected.Invoke ();
		}
	}

	void checkEventOnLastTargetRemoved ()
	{
		if (useEventOnLastTargetRemoved) {
			eventOnLastTargetRemoved.Invoke ();
		}
	}

	void checkRemoveEmptyObjects ()
	{
		for (int i = charactersAround.Count - 1; i >= 0; i--) {	
			if (charactersAround [i] == null) {
				charactersAround.RemoveAt (i);
			}
		}
	}
}