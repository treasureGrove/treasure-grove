using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class photoModeSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool photoModeEnabled = true;

	public float rotationSpeed;

	public float smoothCameraRotationSpeedVertical;
	public float smoothCameraRotationSpeedHorizontal;

	public Vector2 clampTiltX;

	public float movementSpeed;

	public float sidesRotationSpeed = 1;

	public bool clampCameraDistance;
	public float maxCameraRadius;

	public bool setCustomTimeScale;
	public float customTimeScale;

	public bool freezeAllCharactersOnScene;

	public bool canUseTurboEnabled = true;
	public float turboSpeedMultiplier = 2;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnPhotoModeActive;
	public UnityEvent eventOnPhotoModeDeactive;

	public UnityEvent eventOnTakePhoto;

	[Space]
	[Header ("Debug")]
	[Space]

	public Vector2 movementAxisValues;
	public Vector2 mouseAxisValues;

	public Vector2 currentLookAngle;

	public bool photoModeInputActive;

	public bool photoModeActive;

	public bool turboActive;

	[Space]
	[Header ("Component Elements")]
	[Space]

	public Transform mainTransform;
	public Transform pivotTransform;

	public Transform mainCameraTransform;

	public playerInputManager playerInput;
	public playerCamera mainPlayerCamera;

	public timeBullet mainTimeBullet;
	public menuPause pauseManager;

	Transform previousMainCameraParent;

	Quaternion currentPivotRotation;
	float currentCameraUpRotation;

	Vector3 moveInput;

	bool movingCameraUp;
	bool movingCameraDown;
	bool rotatingCameraToRight;
	bool rotatingCameraToLeft;

	Vector3 lastCameraPosition;

	bool cameraRotationActive = true;

	bool resetingCamera;

	float previosScaleTime;
	Coroutine resetCameraPositionCoroutine;

	Coroutine mainUpdateCoroutine;


	public void stopUpdateCoroutine ()
	{
		if (mainUpdateCoroutine != null) {
			StopCoroutine (mainUpdateCoroutine);
		}
	}

	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForSeconds (0.00001f);

		while (true) {
			
//	void Update ()
//	{
			if (photoModeActive && photoModeInputActive) {
				movementAxisValues = playerInput.getPlayerRawMovementAxisWithoutCheckingEnabled ();

				mouseAxisValues = playerInput.getPlayerMouseAxis ();

				//get the look angle value
				if (cameraRotationActive) {
					currentLookAngle.x = mouseAxisValues.x * rotationSpeed;
					currentLookAngle.y -= mouseAxisValues.y * rotationSpeed;
				}

				//clamp these values to limit the camera rotation
				currentLookAngle.y = Mathf.Clamp (currentLookAngle.y, -clampTiltX.x, clampTiltX.y);

				//set every angle in the camera and the pivot
				currentPivotRotation = Quaternion.Euler (currentLookAngle.y, 0, 0);

				pivotTransform.localRotation = Quaternion.Slerp (pivotTransform.localRotation, currentPivotRotation, smoothCameraRotationSpeedVertical * Time.unscaledDeltaTime);

				currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, currentLookAngle.x, smoothCameraRotationSpeedHorizontal * Time.unscaledDeltaTime);

				mainTransform.Rotate (0, currentCameraUpRotation, 0);					

				if (rotatingCameraToRight) {
					mainTransform.Rotate (0, 0, -sidesRotationSpeed);	

					rotatingCameraToRight = false;
				}

				if (rotatingCameraToLeft) {
					mainTransform.Rotate (0, 0, sidesRotationSpeed);		

					rotatingCameraToLeft = false;
				}

				float currentSpeed = movementSpeed;

				if (turboActive) {
					currentSpeed *= turboSpeedMultiplier;
				}

				moveInput = (movementAxisValues.y * pivotTransform.forward + movementAxisValues.x * pivotTransform.right) * currentSpeed;

				if (movingCameraUp) {
					moveInput += Vector3.up * currentSpeed;

					movingCameraUp = false;
				}

				if (movingCameraDown) {
					moveInput -= Vector3.up * currentSpeed;

					movingCameraDown = false;
				}

				mainTransform.position += (moveInput * Time.unscaledDeltaTime);

				if (clampCameraDistance) {

					Vector3 newLocation = mainTransform.position;
					
					Vector3 centerPosition = lastCameraPosition;
					float distance = GKC_Utils.distance (newLocation, centerPosition);

					if (distance > maxCameraRadius) {
						Vector3 fromOriginToObject = newLocation - centerPosition;
						fromOriginToObject *= maxCameraRadius / distance;
						newLocation = centerPosition + fromOriginToObject;
					}

					mainTransform.position = newLocation;
				}
			}

			yield return waitTime;
		}
	}

	public void setPhotoModeActiveState (bool state)
	{
		if (state == photoModeActive) {
			return;
		}

		if (resetingCamera) {
			return;
		}

		photoModeActive = state;

		stopUpdateCoroutine ();

		if (photoModeActive) {
			mainUpdateCoroutine = StartCoroutine (updateCoroutine ());
		}

		if (setCustomTimeScale) {
			if (photoModeActive) {
				previosScaleTime = Time.timeScale;

				mainTimeBullet.setTimeValues (true, customTimeScale);

				pauseManager.setTimeScale (customTimeScale);

			} else {
				if (previosScaleTime < 1) {
					mainTimeBullet.setTimeValues (true, previosScaleTime);

					pauseManager.setTimeScale (previosScaleTime);
				} else {
					mainTimeBullet.disableTimeBullet ();

					pauseManager.setTimeScale (1);
				}
			}
		}

		if (freezeAllCharactersOnScene) {
			GKC_Utils.pauseOrResumeAllCharactersScene (state);
		}

		currentLookAngle = Vector2.zero;

		if (photoModeActive) {
			eventOnPhotoModeActive.Invoke ();

			movementAxisValues = Vector2.zero;
			mouseAxisValues = Vector2.zero;

			lastCameraPosition = mainCameraTransform.position;

			previousMainCameraParent = mainCameraTransform.parent;

			mainTransform.position = mainCameraTransform.position;

			mainTransform.rotation = Quaternion.identity;

			mainTransform.eulerAngles = new Vector3 (mainTransform.eulerAngles.x, mainCameraTransform.eulerAngles.y, mainTransform.eulerAngles.z);

			pivotTransform.rotation = mainCameraTransform.rotation;
			pivotTransform.localEulerAngles = new Vector3 (pivotTransform.localEulerAngles.x, 0, 0);

			currentLookAngle.y = pivotTransform.localEulerAngles.x;

			if (currentLookAngle.y > 180) {
				currentLookAngle.y -= 360;
			}

			mainCameraTransform.SetParent (pivotTransform);

			photoModeInputActive = true;
		} else {
			photoModeInputActive = false;

			resetCameraPosition ();
		}

		turboActive = false;
	}

	public void enableOrDisablePhotoMode ()
	{
		setPhotoModeActiveState (!photoModeActive);
	}

	public void takePhoto ()
	{
		eventOnTakePhoto.Invoke ();
	}

	public void resetCameraPosition ()
	{
		if (resetCameraPositionCoroutine != null) {
			StopCoroutine (resetCameraPositionCoroutine);
		}

		resetCameraPositionCoroutine = StartCoroutine (resetCameraCoroutine ());
	}

	IEnumerator resetCameraCoroutine ()
	{
		setCameraDirection ();

		if (previousMainCameraParent) {
			resetingCamera = true;

			mainCameraTransform.SetParent (previousMainCameraParent);

			Vector3	worldTargetPosition = previousMainCameraParent.position;

			float dist = GKC_Utils.distance (mainCameraTransform.position, worldTargetPosition);

			float duration = dist / movementSpeed;

			float t = 0;

			while ((t < 1 && (mainCameraTransform.localPosition != Vector3.zero || mainCameraTransform.localRotation != Quaternion.identity))) {
				t += Time.unscaledDeltaTime / duration;

				mainCameraTransform.localPosition = Vector3.Lerp (mainCameraTransform.localPosition, Vector3.zero, t);
				mainCameraTransform.localRotation = Quaternion.Lerp (mainCameraTransform.localRotation, Quaternion.identity, t);

				yield return null;
			}

			resetingCamera = false;
		}

		eventOnPhotoModeDeactive.Invoke ();
	}

	public void setCameraDirection ()
	{
		mainPlayerCamera.transform.eulerAngles = (mainPlayerCamera.transform.up * mainTransform.eulerAngles.y);

		Quaternion newCameraRotation = pivotTransform.localRotation;

		mainPlayerCamera.getPivotCameraTransform ().localRotation = newCameraRotation;

		float newLookAngleValue = newCameraRotation.eulerAngles.x;

		if (newLookAngleValue > 180) {
			newLookAngleValue -= 360;
		}

		mainPlayerCamera.setLookAngleValue (new Vector2 (0, newLookAngleValue));
	}


	public void inputTakePhoto ()
	{
		if (photoModeActive) {
			takePhoto ();
		}
	}

	public void inputToogleCameraRotation ()
	{
		if (photoModeActive) {
			cameraRotationActive = !cameraRotationActive;
		}
	}

	public void inputEnableOrDisablePhotoMode ()
	{
		enableOrDisablePhotoMode ();
	}

	public void inputSetTurboMode (bool state)
	{
		if (photoModeActive) {
			if (canUseTurboEnabled) {
				turboActive = state;
			}
		}
	}

	public void inputMoveCameraUp ()
	{
		movingCameraUp = true;
	}

	public void inputMoveCameraDown ()
	{
		movingCameraDown = true;
	}

	public void inputRotategCameraToRight ()
	{
		rotatingCameraToRight = true;
	}

	public void inputRotateCameraToLeft ()
	{
		rotatingCameraToLeft = true;
	}
}
