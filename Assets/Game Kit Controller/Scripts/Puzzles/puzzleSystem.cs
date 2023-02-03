using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class puzzleSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<GameObject> dragableElements = new List<GameObject> ();
	public bool respawnDragableElements;
	public bool respawnDragableElementsInOriginalPosition;
	public Transform respawnPosition;
	public float dragVelocity = 8;
	public LayerMask layer;
	public float maxRaycastDistance;

	public bool freezeRotationWhenGrabbed;

	public bool objectCanBeRotated;
	public float rotateSpeed;

	public bool horizontalRotationEnabled = true;
	public bool verticalRotationEnabled = true;

	public bool rotateToCameraInFixedPosition;
	public float rotationSpeed;

	public float changeHoldDistanceSpeed = 2;

	public bool useColliderMaterialWhileGrabbed;
	public PhysicMaterial grabbedColliderMaterial;

	[Space]
	[Header ("Puzzle Settings")]
	[Space]

	public bool resetObjectsWhenStopUse;
	public bool canResetObjectsWhileUsing;

	public int numberObjectsToPlace;

	public bool disableObjectActionAfterResolve;
	public float waitDelayToStopUsePuzzleAfterResolve;

	[Space]
	[Header ("Camera Rotation Settings")]
	[Space]

	public bool rotateAroundPointEnabled;
	public float rotateAroundPointSpeed;
	public Transform rotationPointPivot;
	public Transform rotationPointCamera;
	public Transform cameraPosition;

	public bool useVerticalRotationLimit;
	public bool useHorizontalRotationLimit;
	public Vector2 verticalRotationLimit = new Vector2 (-90, 90);
	public Vector2 horizontalRotationLimit = new Vector2 (-90, 90);

	[Space]
	[Header ("Action Screen Settings")]
	[Space]

	public bool activateActionScreen = true;
	public string actionScreenName = "Use Puzzle System";

	public bool useMeshTextForClues;
	public GameObject meshTextGameObject;

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public bool usePuzzleSolvedSound;
	public AudioClip puzzleSolvedSound;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool puzzleSolved;
	public bool usingPuzzle;
	public int currentNumberObjectsPlaced;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnResetPuzzle;
	public UnityEvent resetPuzzleEvent;
	public UnityEvent puzzleSolvedEvent;

	public bool useEventAfterResolveDelay;
	public UnityEvent eventAfterResolveDelay;

	[Space]
	[Header ("Components")]
	[Space]

	public electronicDevice electronicDeviceManager;
	public deviceStringAction deviceStringActionManager;
	public AudioSource mainAudioSource;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor;
	public float gizmoRadius = 0.1f;
	public float gizmoArrowLength = 1;
	public float gizmoArrowLineLength = 2.5f;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;

	GameObject currentDragableElement;

	playerInputManager playerInput;

	GameObject currentPlayer;

	bool touchPlatform;
	Touch currentTouch;
	bool touching;
	bool movingObject;
	GameObject currentGrabbedObject;
	Rigidbody currentGrabbedObjectRigidbody;
	Camera mainCamera;
	Transform mainCameraTransform;
	RaycastHit hit;
	float currentDistanceGrabbedObject;
	Vector3 touchPosition;
	Vector3 nextPosition;
	Vector3 currentPosition;
	bool rotatingObject;
	bool increaseHoldObjectDistance;
	bool decreaseHoldObjectDistance;
	Quaternion originalRotationPointPivotRotation;
	Quaternion oriinalRotationPointCameraRotation;

	Collider grabbedObjectCollider;
	PhysicMaterial originalGrabbedColliderMaterial;

	Vector2 lookAngle;

	float lastTimeMouseWheelUsed;
	bool mouseWheelUsedPreviously;

	List<grabableObjectsInfo> grabableObjectsInfoList = new List<grabableObjectsInfo> ();

	Coroutine resetObjectRotationCoroutine;

	puzzleSystemPlayerManagement puzzleSystemPlayerManager;

	Vector2 axisValues;


	void Start ()
	{
		if (electronicDeviceManager == null) {
			electronicDeviceManager = GetComponent<electronicDevice> ();
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (resetObjectsWhenStopUse) {
			for (int i = 0; i < dragableElements.Count; i++) {
				grabableObjectsInfo newGrabableObjectsInfo = new grabableObjectsInfo ();
				newGrabableObjectsInfo.grabableGameObject = dragableElements [i];
				newGrabableObjectsInfo.grabableTransform = dragableElements [i].transform;
				newGrabableObjectsInfo.originalPosition = dragableElements [i].transform.position;
				newGrabableObjectsInfo.originalRotation = dragableElements [i].transform.rotation;

				grabableObjectsInfoList.Add (newGrabableObjectsInfo);
			}
		}

		originalRotationPointPivotRotation = rotationPointPivot.localRotation;
		oriinalRotationPointCameraRotation = rotationPointCamera.localRotation;

		if (deviceStringActionManager == null) {
			deviceStringActionManager = GetComponent<deviceStringAction> ();
		}

		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}
	}

	void Update ()
	{
		if (usingPuzzle) {

			int touchCount = Input.touchCount;
			if (!touchPlatform) {
				touchCount++;
			}

			for (int i = 0; i < touchCount; i++) {
				if (!touchPlatform) {
					currentTouch = touchJoystick.convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (i);
				}

				if (currentTouch.phase == TouchPhase.Began && !touching) {
					Ray ray = mainCamera.ScreenPointToRay (currentTouch.position);
					if (Physics.Raycast (ray, out hit, maxRaycastDistance, layer)) {

						GameObject objectToCheck = canBeDragged (hit.collider.gameObject);

						if (objectToCheck != null) {
							touching = true;
							currentGrabbedObject = objectToCheck;
							currentGrabbedObjectRigidbody = currentGrabbedObject.GetComponent<Rigidbody> ();
							currentGrabbedObjectRigidbody.velocity = Vector3.zero;
							currentDistanceGrabbedObject = mainCameraTransform.InverseTransformDirection (currentGrabbedObject.transform.position - mainCameraTransform.position).z;
							grabbedObjectCollider = currentGrabbedObject.GetComponent<Collider> ();

							if (freezeRotationWhenGrabbed) {
								currentGrabbedObjectRigidbody.freezeRotation = true;
							}

							if (useColliderMaterialWhileGrabbed && grabbedObjectCollider != null) {
								originalGrabbedColliderMaterial = grabbedObjectCollider.material;
								grabbedObjectCollider.material = grabbedColliderMaterial;
							}
						}
					}
				}

				if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) && currentGrabbedObject != null) {
					movingObject = true;
					touchPosition = mainCamera.ScreenToWorldPoint (new Vector3 (currentTouch.position.x, currentTouch.position.y, currentDistanceGrabbedObject));
					nextPosition = touchPosition;
					currentPosition = currentGrabbedObject.transform.position;
					currentGrabbedObjectRigidbody.velocity = (nextPosition - currentPosition) * dragVelocity;


					if (rotatingObject) {
						if (horizontalRotationEnabled) {
							currentGrabbedObject.transform.Rotate (0, -rotateSpeed * playerInput.getPlayerMovementAxis ().x, 0);
						}

						if (verticalRotationEnabled) {
							currentGrabbedObject.transform.Rotate (rotateSpeed * playerInput.getPlayerMovementAxis ().y, 0, 0);
						}

					} else {
						if (rotateToCameraInFixedPosition) {
							Quaternion direction = Quaternion.LookRotation (mainCameraTransform.forward);
							currentGrabbedObject.transform.rotation = Quaternion.Slerp (currentGrabbedObject.transform.rotation, direction, Time.deltaTime * rotationSpeed);
						}
					}

					if (mouseWheelUsedPreviously && Time.time > lastTimeMouseWheelUsed + 0.1f) {
						increaseHoldObjectDistance = false;
						decreaseHoldObjectDistance = false;
						mouseWheelUsedPreviously = false;
					}
						
					if (increaseHoldObjectDistance) {
						currentDistanceGrabbedObject += Time.deltaTime * changeHoldDistanceSpeed;
					}

					if (decreaseHoldObjectDistance) {
						currentDistanceGrabbedObject -= Time.deltaTime * changeHoldDistanceSpeed;
					}
				}
					
				if (currentTouch.phase == TouchPhase.Ended && touching) {
					dropObject ();
				}
			}
				
			if (rotateAroundPointEnabled && !rotatingObject) {
				axisValues = playerInput.getPlayerMovementAxis ();
				lookAngle.x -= axisValues.x * rotateAroundPointSpeed;
				lookAngle.y += axisValues.y * rotateAroundPointSpeed;

				if (useVerticalRotationLimit) {
					lookAngle.y = Mathf.Clamp (lookAngle.y, verticalRotationLimit.x, verticalRotationLimit.y);
				} else {
					if (lookAngle.y > 360 || lookAngle.y < -360) {
						lookAngle.y = 0;
					}
				}

				if (useHorizontalRotationLimit) {
					lookAngle.x = Mathf.Clamp (lookAngle.x, horizontalRotationLimit.x, horizontalRotationLimit.y);
				} else {
					if (lookAngle.x > 360 || lookAngle.x < -360) {
						lookAngle.x = 0;
					}
				}

				rotationPointPivot.localRotation = Quaternion.Euler (0, lookAngle.x, 0);
				rotationPointCamera.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
			}
		}
	}

	public void resetObjects ()
	{
		for (int i = 0; i < grabableObjectsInfoList.Count; i++) {
			grabableObjectsInfoList [i].grabableTransform.position = grabableObjectsInfoList [i].originalPosition;
			grabableObjectsInfoList [i].grabableTransform.rotation = grabableObjectsInfoList [i].originalRotation;

			Rigidbody currentRigidbody = grabableObjectsInfoList [i].grabableGameObject.GetComponent<Rigidbody> ();

			if (currentRigidbody != null) {
				currentRigidbody.isKinematic = false;
			}
		}

		resetNumberObjectsPlaced ();

		if (useEventOnResetPuzzle) {
			if (resetPuzzleEvent.GetPersistentEventCount () > 0) {
				resetPuzzleEvent.Invoke ();
			}
		}
	}

	public void OnTriggerExit (Collider col)
	{
		if (respawnDragableElements && respawnPosition != null) {
			GameObject objectToCheck = canBeDragged (col.gameObject);

			if (objectToCheck != null) {
				currentDragableElement = objectToCheck;

				if (currentDragableElement == currentGrabbedObject) {
					dropObject ();
				}

				Rigidbody currentRigidbody = currentDragableElement.GetComponent<Rigidbody> ();

				if (currentRigidbody != null) {
					currentRigidbody.isKinematic = true;
				}

				if (respawnDragableElementsInOriginalPosition) {
					for (int i = 0; i < grabableObjectsInfoList.Count; i++) {
						if (grabableObjectsInfoList [i].grabableGameObject == currentDragableElement) {
							grabableObjectsInfoList [i].grabableTransform.position = grabableObjectsInfoList [i].originalPosition;
							grabableObjectsInfoList [i].grabableTransform.rotation = grabableObjectsInfoList [i].originalRotation;
						}
					}
				} else {
					currentDragableElement.transform.position = respawnPosition.position;
					currentDragableElement.transform.rotation = respawnPosition.rotation;
				}

				if (currentRigidbody != null) {
					currentRigidbody.isKinematic = false;
				}
			}
		}
	}

	public GameObject canBeDragged (GameObject objectToCheck)
	{
		if (dragableElements.Contains (objectToCheck)) {
			return objectToCheck;
		}

		for (int i = 0; i < dragableElements.Count; i++) {
			if (objectToCheck.transform.IsChildOf (dragableElements [i].transform)) {
				return dragableElements [i];
			}
		}

		return null;
	}

	public void dropObject ()
	{
		if (resetObjectRotationCoroutine != null) {
			StopCoroutine (resetObjectRotationCoroutine);
		}

		currentGrabbedObjectRigidbody.freezeRotation = false;

		if (useColliderMaterialWhileGrabbed && grabbedObjectCollider != null) {
			grabbedObjectCollider.material = originalGrabbedColliderMaterial;
		}

		touching = false; 
		movingObject = false;
		currentGrabbedObject = null;
		rotatingObject = false;
		increaseHoldObjectDistance = false;
		decreaseHoldObjectDistance = false;
		currentGrabbedObjectRigidbody = null;
	}

	public void checkIfObjectGrabbed (GameObject objectToCheck)
	{
		if (currentGrabbedObject == objectToCheck) {
			dropObject ();
		}
	}

	public void startOrStopPuzzle ()
	{
		usingPuzzle = !usingPuzzle;

		if (usingPuzzle) {
			setCurrentPlayer (electronicDeviceManager.getCurrentPlayer ());
		} else {
			if (resetObjectsWhenStopUse && !puzzleSolved) {
				resetObjects ();
			}

			rotationPointPivot.localRotation = originalRotationPointPivotRotation;
			rotationPointCamera.localRotation = oriinalRotationPointCameraRotation;
			lookAngle = Vector3.zero;
		}

		if (puzzleSystemPlayerManager != null) {
			puzzleSystemPlayerManager.setUsingPuzzleState (usingPuzzle);
		}

		if (activateActionScreen) {
			playerInput.enableOrDisableActionScreen (actionScreenName, usingPuzzle);
		}

		if (useMeshTextForClues) {
			if (meshTextGameObject != null) {
				meshTextGameObject.SetActive (usingPuzzle);
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			puzzleSystemPlayerManager = mainPlayerComponentsManager.getPuzzleSystemPlayerManagement ();

			mainCamera = mainPlayerComponentsManager.getPlayerCamera ().getMainCamera ();

			mainCameraTransform = mainCamera.transform;

			playerInput = mainPlayerComponentsManager.getPlayerInputManager ();

			puzzleSystemPlayerManager.setcurrentPuzzleSystem (this);
		}
	}

	public void increaseNumberObjectsPlaced ()
	{
		currentNumberObjectsPlaced++;

		if (currentNumberObjectsPlaced == numberObjectsToPlace) {
			solvePuzzle ();
		}
	}

	public void resetNumberObjectsPlaced ()
	{
		currentNumberObjectsPlaced = 0;
	}

	public void solvePuzzle ()
	{
		if (puzzleSolvedEvent.GetPersistentEventCount () > 0) {
			puzzleSolvedEvent.Invoke ();
		}

		puzzleSolved = true;

		if (disableObjectActionAfterResolve) {
			deviceStringActionManager.showIcon = false;
			removeDeviceFromList ();

			StartCoroutine (waitingAfterUnlock ());
		}

		if (usePuzzleSolvedSound) {
			playSound (puzzleSolvedSound);
		}
	}

	public void resetCurrentObjectRotation ()
	{
		if (resetObjectRotationCoroutine != null) {
			StopCoroutine (resetObjectRotationCoroutine);
		}

		resetObjectRotationCoroutine = StartCoroutine (resetCurrenObjectRotationCoroutine ());
	}

	IEnumerator resetCurrenObjectRotationCoroutine ()
	{
		GameObject objectToRotate = currentGrabbedObject;
		Quaternion targetRotation = Quaternion.identity;

		for (int i = 0; i < grabableObjectsInfoList.Count; i++) {
			if (grabableObjectsInfoList [i].grabableGameObject == objectToRotate) {
				targetRotation = grabableObjectsInfoList [i].originalRotation;
			}
		}

		float timePassed = 0;

		while (objectToRotate.transform.rotation != targetRotation || timePassed < 3) {
			objectToRotate.transform.rotation = Quaternion.Slerp (objectToRotate.transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

			timePassed += Time.deltaTime;

			yield return null;
		}
	}


	public void removeDeviceFromList ()
	{
		if (currentPlayer != null) {
			currentPlayer.GetComponent<usingDevicesSystem> ().removeDeviceFromListExternalCall (gameObject);
		}
	}

	public void playSound (AudioClip soundEffect)
	{
		if (mainAudioSource != null) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);

			mainAudioSource.PlayOneShot (soundEffect);
		}
	}

	IEnumerator waitingAfterUnlock ()
	{
		yield return new WaitForSeconds (waitDelayToStopUsePuzzleAfterResolve);

		if (useEventAfterResolveDelay) {
			eventAfterResolveDelay.Invoke ();
		}

		electronicDeviceManager.stopUsindDevice ();

		yield return null;
	}

	//CALL INPUT FUNCTIONS
	public void inputSetRotateObjectState (bool state)
	{
		if (usingPuzzle && objectCanBeRotated && currentGrabbedObjectRigidbody != null) {
			if (state) {
				rotatingObject = true;
				currentGrabbedObjectRigidbody.freezeRotation = true;
			} else {
				rotatingObject = false;
				currentGrabbedObjectRigidbody.freezeRotation = false;
			}
		}
	}

	public void inputIncreaseObjectHolDistanceByButton (bool state)
	{
		if (usingPuzzle && movingObject) {
			if (state) {
				increaseHoldObjectDistance = true;
			} else {
				increaseHoldObjectDistance = false;
			}
		}
	}

	public void inputDecreaseObjectHolDistanceByButton (bool state)
	{
		if (usingPuzzle && movingObject) {
			if (state) {
				decreaseHoldObjectDistance = true;
			} else {
				decreaseHoldObjectDistance = false;
			}
		}
	}

	public void inputSetObjectHolDistanceByMouseWheel (bool state)
	{
		if (usingPuzzle && movingObject) {
			if (state) {
				lastTimeMouseWheelUsed = Time.time;

				increaseHoldObjectDistance = true;

				mouseWheelUsedPreviously = true;
			} else {
				lastTimeMouseWheelUsed = Time.time;

				decreaseHoldObjectDistance = true;

				mouseWheelUsedPreviously = true;
			}
		}
	}

	public void inputResetPuzzle ()
	{
		if (usingPuzzle && canResetObjectsWhileUsing && !puzzleSolved) {
			if (currentGrabbedObject == null) {
				resetObjects ();
			} else {
				resetCurrentObjectRotation ();
			}
		}
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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawLine (transform.position, rotationPointPivot.position);
			Gizmos.DrawLine (rotationPointPivot.position, rotationPointCamera.position);
			Gizmos.DrawLine (rotationPointPivot.position, cameraPosition.position);

			Gizmos.DrawSphere (transform.position, gizmoRadius);
			Gizmos.DrawSphere (rotationPointPivot.position, gizmoRadius);
			Gizmos.DrawSphere (rotationPointCamera.position, gizmoRadius);
			Gizmos.DrawSphere (cameraPosition.position, gizmoRadius);

			Gizmos.color = Color.green;
			GKC_Utils.drawGizmoArrow (cameraPosition.position, cameraPosition.forward * gizmoArrowLineLength, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);
		}			
	}

	[System.Serializable]
	public class grabableObjectsInfo
	{
		public GameObject grabableGameObject;
		public Transform grabableTransform;
		public Vector3 originalPosition;
		public Quaternion originalRotation;
	}
}
