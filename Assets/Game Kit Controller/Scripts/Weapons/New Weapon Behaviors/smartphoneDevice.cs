using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class smartphoneDevice : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool canMakePhotos;
	public bool usePhotoSound;
	public AudioClip photoSound;
	public bool canUseFlash;
	public float flashDuration = 0.1f;

	public bool storeCapturesEnabled = true;

	[Space]
	[Header ("Zoom Settings")]
	[Space]

	public bool canUseZoom;
	public float maxZoomAmout;
	public float minZoomAmount;
	public float zoomSpeed;

	[Space]
	[Header ("Check Objects On Capture Settings")]
	[Space]

	public bool checkObjectFoundOnCapture;
	public LayerMask layerToCheckObjectFound;
	public float rayDistanceToCheckObjectFound;
	public Transform raycastTransform;

	public bool useOverrideSystemOnCapture;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnCapture;
	public UnityEvent eventOnCapture;

	public bool useEventsOnCameraViewChange;
	public UnityEvent eventOnCameraViewChangeToThirdPerson;
	public UnityEvent eventOnCameraViewChangeToFirstPerson;

	public bool useEventsOnSmartphoneTurnOnOff;
	public UnityEvent eventOnSmartphoneTurnedOn;
	public UnityEvent eventOnSmartphoneTurnedOff;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool firstPersonActive;
	public bool isActivated;

	public bool perspectiveSystemLocated;

	[Space]
	[Header ("Smartphone Components")]
	[Space]

	public GameObject smartphoneCamera;
	public GameObject smartphoneScreenCanvas;
	public GameObject smartphoneScreenCenter;

	public GameObject cameraFlash;

	public Transform mainScreenCenter;
	public Transform screenCenter;

	public AudioSource mainAudioSource;

	public Camera smartphoneMainCamera;

	[Space]
	[Header ("Player Components")]
	[Space]

	public Transform playerCameraTransform;
	public playerWeaponSystem weaponManager;
	public playerWeaponsManager mainPlayerWeaponsManager;
	public cameraCaptureSystem cameraCaptureManager;

	public overrideElementControlSystem mainOverrideElementControlSystem;

	public eventObjectFoundOnRaycastSystem mainEventObjectFoundOnRaycastSystem;

	RaycastHit hit;

	bool changingZoom;
	float currentFov;
	float zoomDirection = -1;
	cameraPerspectiveSystem currentPerspectiveSystem;

	Coroutine flashCoroutine;

	bool cameraCaptureManagerLocated;


	void Update ()
	{
		if (isActivated) {
			if (canUseZoom && changingZoom) {
				currentFov = smartphoneMainCamera.fieldOfView + Time.deltaTime * zoomSpeed * zoomDirection;

				if (zoomDirection == -1) {
					if (currentFov < minZoomAmount) {
						zoomDirection = 1;
					}
				} else {
					if (currentFov > maxZoomAmout) {
						zoomDirection = -1;
					}
				}

				smartphoneMainCamera.fieldOfView = currentFov;
			}
		}
	}

	public void takePhoto ()
	{
		if (canMakePhotos) {
			playSound ();

			checkFlash ();

			if (storeCapturesEnabled) {
				if (cameraCaptureManagerLocated) {
					cameraCaptureManager.takeCapture (smartphoneMainCamera);
				}
			}

			if (perspectiveSystemLocated) {
				currentPerspectiveSystem.checkCurrentPlayerPosition (playerCameraTransform, weaponManager.getMainCameraTransform (), smartphoneMainCamera);
			}

			if (checkObjectFoundOnCapture) {
				if (Physics.Raycast (raycastTransform.position, raycastTransform.forward, out hit, rayDistanceToCheckObjectFound, layerToCheckObjectFound)) {
					eventObjectFoundOnCaptureSystem currentEventObjectFoundOnCaptureSystem = hit.collider.gameObject.GetComponent<eventObjectFoundOnCaptureSystem> ();

					if (currentEventObjectFoundOnCaptureSystem != null) {
						currentEventObjectFoundOnCaptureSystem.callEventOnCapture ();
					}
				}
			}

			changingZoom = false;

			if (useEventOnCapture) {
				eventOnCapture.Invoke ();
			}

			if (useOverrideSystemOnCapture) {
				if (mainOverrideElementControlSystem != null) {
					mainOverrideElementControlSystem.checkElementToControl (raycastTransform);
				}
			}
		}
	}

	public void checkFlash ()
	{
		if (!canUseFlash) {
			return;
		}

		if (flashCoroutine != null) {
			StopCoroutine (flashCoroutine);
		}

		flashCoroutine = StartCoroutine (flashCoroutineCoroutine ());
	}

	IEnumerator flashCoroutineCoroutine ()
	{
		cameraFlash.SetActive (true);

		yield return new WaitForSeconds (flashDuration);

		cameraFlash.SetActive (false);

		yield return null;
	}


	public void changeZoom ()
	{
		changingZoom = !changingZoom;
	}

	public void turnOn ()
	{
		isActivated = true;

		setSmartphoneState (isActivated);
	}

	public void turnOff ()
	{
		isActivated = false;

		setSmartphoneState (isActivated);
	}

	public void changeSmartphoneState ()
	{
		setSmartphoneState (!isActivated);
	}

	public void setSmartphoneState (bool state)
	{
		isActivated = state;

		smartphoneScreenCanvas.SetActive (isActivated);

		smartphoneScreenCenter.SetActive (isActivated);

		smartphoneCamera.SetActive (isActivated);

		changingZoom = false;

		initializeComponents ();

		if (isActivated) {
			if (mainPlayerWeaponsManager != null) {
				mainPlayerWeaponsManager.setWeaponPartLayer (smartphoneScreenCanvas);

				mainPlayerWeaponsManager.setWeaponPartLayer (smartphoneScreenCenter);
			}
		}

		if (useEventsOnSmartphoneTurnOnOff) {
			if (isActivated) {
				eventOnSmartphoneTurnedOn.Invoke ();
			} else {
				eventOnSmartphoneTurnedOff.Invoke ();
			}
		}
	}

	public void playSound ()
	{
		if (usePhotoSound && mainAudioSource != null) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);

			mainAudioSource.PlayOneShot (photoSound);
		}
	}

	public void setCurrentPerspectiveSystem (cameraPerspectiveSystem perspective)
	{
		currentPerspectiveSystem = perspective;

		perspectiveSystemLocated = currentPerspectiveSystem != null;
	}

	public void removeCurrentPerspectiveSystem ()
	{
		currentPerspectiveSystem = null;

		perspectiveSystemLocated = false;
	}

	public Camera getSmarthphoneMainCamera ()
	{
		return smartphoneMainCamera;
	}

	public void setUseEventOnCaptureState (bool state)
	{
		useEventOnCapture = state;
	}

	public void setStoreCapturesEnabledState (bool state)
	{
		storeCapturesEnabled = state;
	}

	public void setUseOverrideSystemOnCaptureState (bool state)
	{
		useOverrideSystemOnCapture = state;
	}

	public void rotateScreenToRight ()
	{
		mainScreenCenter.localEulerAngles = Vector3.zero;
		screenCenter.localEulerAngles = Vector3.zero;
		smartphoneCamera.transform.localEulerAngles = Vector3.zero;
	}

	public void rotateScreenToLeft ()
	{
		mainScreenCenter.localEulerAngles = new Vector3 (0, 0, 180);
		screenCenter.localEulerAngles = new Vector3 (0, 0, 180);
		smartphoneCamera.transform.localEulerAngles = new Vector3 (0, 0, 180);
	}

	public void setFirstOrThirdPersonViewState (bool state)
	{
		firstPersonActive = state;

		if (useEventsOnCameraViewChange) {
			if (firstPersonActive) {
				eventOnCameraViewChangeToFirstPerson.Invoke ();
			} else {
				eventOnCameraViewChangeToThirdPerson.Invoke ();
			}
		}
	}

	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (mainPlayerWeaponsManager == null) {
			mainPlayerWeaponsManager = weaponManager.getPlayerWeaponsManger ();
		}

		GameObject playerControllerGameObject = mainPlayerWeaponsManager.getPlayerGameObject ();

		playerComponentsManager mainPlayerComponentsManager = playerControllerGameObject.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			cameraCaptureManager = mainPlayerComponentsManager.getCameraCaptureSystem ();

			if (cameraCaptureManager != null) {
				cameraCaptureManagerLocated = true;
			}

			playerCameraTransform = mainPlayerComponentsManager.getPlayerCamera ().transform;

			raycastTransform = mainPlayerComponentsManager.getPlayerCamera ().getMainCamera ().transform;

			mainOverrideElementControlSystem = mainPlayerComponentsManager.getOverrideElementControlSystem ();

			if (mainEventObjectFoundOnRaycastSystem != null) {
				mainEventObjectFoundOnRaycastSystem.setRaycastTransform (raycastTransform);
			}
		}
	
		componentsInitialized = true;
	}
}