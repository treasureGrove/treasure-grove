using UnityEngine;
using System.Collections;

public class simpleMoveCameraToPosition : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cameraMovementActive = true;
	public bool smoothCameraMovement = true;

	public bool useFixedLerpMovement = true;
	public float fixedLerpMovementSpeed = 2;

	public float cameraMovementSpeedThirdPerson = 1;
	public float cameraMovementSpeedFirstPerson = 0.2f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool movingCamera;
	public bool moveCameraActive;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject cameraPosition;

	public Camera mainCamera;

	public GameObject currentPlayer;

	public playerCamera playerCameraManager;


	Transform cameraParentTransform;
	Vector3 mainCameraTargetPosition;
	Quaternion mainCameraTargetRotation;
	Coroutine cameraState;

	playerComponentsManager mainPlayerComponentsManager;


	public void moveCamera (bool state)
	{
		moveCameraActive = state;

		if (cameraPosition == null) {
			cameraPosition = gameObject;
		}

		mainCameraTargetRotation = Quaternion.identity;
		mainCameraTargetPosition = Vector3.zero;

		if (moveCameraActive) {		
			if (cameraMovementActive) {
				if (cameraParentTransform == null) {
					cameraParentTransform = mainCamera.transform.parent;
					mainCamera.transform.SetParent (cameraPosition.transform);
				}
			}
		} else {
			
			if (cameraMovementActive) {
				if (cameraParentTransform != null) {
					mainCamera.transform.SetParent (cameraParentTransform);
					cameraParentTransform = null;
				}
			}
		}

		if (cameraMovementActive) {
			if (smoothCameraMovement) {
				stopMovement ();

				cameraState = StartCoroutine (adjustCamera ());
			} else {
				mainCamera.transform.localRotation = mainCameraTargetRotation;
				mainCamera.transform.localPosition = mainCameraTargetPosition;
			}
		}
	}

	IEnumerator adjustCamera ()
	{
		movingCamera = true;

		Transform mainCameraTransform = mainCamera.transform;

		if (useFixedLerpMovement) {
			float i = 0;
			//store the current rotation of the camera
			Quaternion currentQ = mainCameraTransform.localRotation;
			//store the current position of the camera
			Vector3 currentPos = mainCameraTransform.localPosition;

			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * fixedLerpMovementSpeed;

				mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, mainCameraTargetRotation, i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPos, mainCameraTargetPosition, i);

				yield return null;
			}

		} else {
			bool isFirstPersonActive = playerCameraManager.isFirstPersonActive ();

			float currentCameraMovementSpeed = cameraMovementSpeedThirdPerson;

			if (isFirstPersonActive) {
				currentCameraMovementSpeed = cameraMovementSpeedFirstPerson;
			}

			float dist = GKC_Utils.distance (mainCameraTransform.localPosition, mainCameraTargetPosition);

			float duration = dist / currentCameraMovementSpeed;

			float t = 0;

			float movementTimer = 0;

			bool targetReached = false;

			float angleDifference = 0;

			float positionDifference = 0;

			while (!targetReached) {
				t += Time.deltaTime / duration; 

				mainCameraTransform.localPosition = Vector3.Lerp (mainCameraTransform.localPosition, mainCameraTargetPosition, t);
				mainCameraTransform.localRotation = Quaternion.Lerp (mainCameraTransform.localRotation, mainCameraTargetRotation, t);

				angleDifference = Quaternion.Angle (mainCameraTransform.localRotation, mainCameraTargetRotation);

				positionDifference = GKC_Utils.distance (mainCameraTransform.localPosition, mainCameraTargetPosition);

				movementTimer += Time.deltaTime;

				if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
					targetReached = true;
				}

				yield return null;
			}
		}

		movingCamera = false;
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

			mainCamera = playerCameraManager.getMainCamera ();
		}
	}

	public void stopMovement ()
	{
		if (cameraState != null) {
			StopCoroutine (cameraState);
		}
	}
}
