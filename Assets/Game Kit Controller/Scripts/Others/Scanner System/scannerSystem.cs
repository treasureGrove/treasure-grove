using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class scannerSystem : MonoBehaviour
{
	[Header ("Main Setting")]
	[Space]

	public bool scannerSystemEnabled;

	public bool infiniteScanDistance;
	public float scanDistance;
	public bool scanDistanceAffectedByZoom;
	public float scanDistanceExtraWithZoom;

	public LayerMask layer;

	public bool ignoreChangeToFirstPersonEnabled;

	public bool removeLocatedObjectIfDisabledOnScene = true;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public float initialCameraFov;
	public float zoomCameraFov;
	public float fovChangeSpeed;
	public float scanSpeed;
	public float scaneIconMovementSpeed;


	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter;

	public bool useAutoScanner;

	[Space]
	[Header ("Scanner Messages Settings Settings")]
	[Space]

	public string scanCompleteString = "SCAN COMPLETED";
	public string scanVisorActiveString = "SCAN VISOR ACTIVE";
	public string scanningString = "SCANNING...";
	public string emptyScannerInfoString = "...";

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool useWeakSpotScanner;

	public string scannerAnimationName = "scannerTarget";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool holdScannerInputActive;
	public GameObject scannedObject;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnScannerEnabled;
	public UnityEvent eventOnScannerDisabled;

	[Space]
	[Header ("Scanner Components")]
	[Space]

	public GameObject scannerHUD;
	public GameObject scanIcon;
	public RectTransform scanIconRect;
	public Animation iconAnimation;
	public RectTransform iconRectTransform;

	public GameObject scannerCamera;
	public GameObject scannerCameraUI;
	public Slider slider;
	public Text objectName;
	public Text objectInfo;
	public Text scanStatus;

	public Transform healthInfoParent;
	public GameObject healthElement;
	public Text objectScannedHealthNameText;
	public Text objectScannedHealthAmountText;

	public Camera scannerMainCamera;
	public playerCamera mainPlayerCamera;
	public playerController playerControllerManager;

	public Transform mainCameraTransform;
	public Camera mainCamera;

	bool zoomActive;
	bool scannerModeIsActivated;
	bool isFirstPersonView;
	float originalScannerCameraFov;
	float targetScannerCameraFov;

	bool lookingObject;
	RaycastHit hit;

	scanElementInfo currentObjectToScan;

	List<healthElementInfo> healtWeakSpotList = new List<healthElementInfo> ();

	health currentScannedObjectHealth;
	vehicleHUDManager currentScannedVehicle;
	GameObject currentGameObjectInScanner;
	float distanceToScan;
	float originalScanDistance;
	Coroutine changeFovCoroutine;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;
	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;
	Vector3 screenPoint;

	float currentDistanceToTarget;
	Vector3 centerScreen;

	float screenWidth;
	float screenHeight;

	bool objectToScanLocated;

	void Start ()
	{
		//get the field of view of the camera
		originalScannerCameraFov = scannerMainCamera.fieldOfView;

		if (infiniteScanDistance) {
			distanceToScan = Mathf.Infinity;
		} else {
			distanceToScan = scanDistance;
		}

		originalScanDistance = distanceToScan;
		enableOrDisableScannerCameraComponents (false);
	
		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
	}

	void FixedUpdate ()
	{
		if (holdScannerInputActive) {
			executeScanner ();
		}

		if (scannerModeIsActivated) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}

		//if there is a scanned object, make the scan icon follow this object in the screen
		if (objectToScanLocated && scannedObject != null) {
			if (usingScreenSpaceCamera) {
				screenPoint = mainCamera.WorldToViewportPoint (scannedObject.transform.position);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
			} else {
				screenPoint = mainCamera.WorldToScreenPoint (scannedObject.transform.position);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
			}

			centerScreen = new Vector3 (screenWidth / 2, screenHeight / 2, 0);

			if (useMaxDistanceToCameraCenter) {
				currentDistanceToTarget = GKC_Utils.distance (screenPoint, centerScreen);

				if (currentDistanceToTarget > maxDistanceToCameraCenter) {
					targetOnScreen = false;
				}
			}

			//if the target is visible in the screnn, set the icon position
			if (targetOnScreen) {
				if (usingScreenSpaceCamera) {
					iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

					iconRectTransform.anchoredPosition = Vector2.MoveTowards (iconRectTransform.anchoredPosition, iconPosition2d, Time.deltaTime * scaneIconMovementSpeed);
				} else {
					scanIcon.transform.position = Vector3.MoveTowards (scanIcon.transform.position, screenPoint, Time.deltaTime * scaneIconMovementSpeed);
				}

				if (!scanIcon.activeSelf) {
					//play the scan icon animation to signal the scannable object
					scanIcon.SetActive (true);

					iconAnimation [scannerAnimationName].speed = -1; 
					iconAnimation [scannerAnimationName].time = iconAnimation [scannerAnimationName].length;
					iconAnimation.Play (scannerAnimationName);
				}

				if (useAutoScanner) {
					executeScanner ();
				}
			}
			//if the object is off screen, disable the scan icon in the screen
			else {
				scanIconRect.anchoredPosition = Vector2.zero;

				if (scanIcon.activeSelf) {
					scanIcon.SetActive (false);
				}

				reset ();
			}

			if (removeLocatedObjectIfDisabledOnScene) {
				if (!scannedObject.activeSelf || !scannedObject.activeInHierarchy) {
					holdScannerInputActive = false;

					activateScanner ();

					return;
				}
			}
		} else {
			objectToScanLocated = false;
		}

		if (useWeakSpotScanner) {
			if (currentScannedObjectHealth != null) {
				for (int i = 0; i < healtWeakSpotList.Count; i++) {
					if (healtWeakSpotList [i].used) {

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (healtWeakSpotList [i].target.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (healtWeakSpotList [i].target.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
						}
							
						//if the target is visible in the screnn, set the icon position
						if (targetOnScreen) {
							if (!healtWeakSpotList [i].healthElementGameObject.activeSelf) {
								healtWeakSpotList [i].healthElementGameObject.SetActive (true);
							}

							if (usingScreenSpaceCamera) {
								iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

								healtWeakSpotList [i].healthElementRectTransform.anchoredPosition = iconPosition2d;
							} else {
								healtWeakSpotList [i].healthElementGameObject.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
							}
						} else {
							if (healtWeakSpotList [i].healthElementGameObject.activeSelf) {
								healtWeakSpotList [i].healthElementGameObject.SetActive (false);
							}
						}
					}
				}
			}

			if (currentScannedVehicle != null) {
				for (int i = 0; i < healtWeakSpotList.Count; i++) {
					if (healtWeakSpotList [i].used) {

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (healtWeakSpotList [i].target.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (healtWeakSpotList [i].target.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
						}

						//if the target is visible in the screnn, set the icon position
						if (targetOnScreen) {
							if (!healtWeakSpotList [i].healthElementGameObject.activeSelf) {
								healtWeakSpotList [i].healthElementGameObject.SetActive (true);
							}

							if (usingScreenSpaceCamera) {
								iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

								healtWeakSpotList [i].healthElementRectTransform.anchoredPosition = iconPosition2d;
							} else {
								healtWeakSpotList [i].healthElementGameObject.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
							}
						} else {
							if (healtWeakSpotList [i].healthElementGameObject.activeSelf) {
								healtWeakSpotList [i].healthElementGameObject.SetActive (false);
							}
						}
					}
				}
			}
		}

		//if the scan mode is enabled, launch a ray from the center of the screen in forward direction, searching a scannable object
		if (scannerModeIsActivated) {
			Debug.DrawRay (mainCameraTransform.position, mainCameraTransform.forward * distanceToScan, Color.red);

			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, distanceToScan, layer)) {
				//scannble object detected

				if (hit.collider.GetComponent<scanElementInfo> ()) {
					//if it the first scannable object found, set as scannedObject
					if (scannedObject == null) {
						scannedObject = hit.collider.gameObject;

						objectToScanLocated = true;

						setScannedObjectInfo ();
					}
					//if there was already another scannable object different from the current found, change it
					else if (scannedObject != hit.collider.gameObject) {
						scannedObject = hit.collider.gameObject;

						objectToScanLocated = true;

						setScannedObjectInfo ();

						reset ();
					}

					lookingObject = true;
				}
				//nothing found
				else {
					lookingObject = false;
				}
			}
		}
	}

	public bool playerIsBusy ()
	{
		if (!playerControllerManager.isUsingDevice () && !playerControllerManager.isUsingSubMenu () && !playerControllerManager.isPlayerMenuActive ()) {
			return false;
		}

		return true;
	}

	public void activateScanner ()
	{
		//activate the scanner mode
		enableScanner ();

		//if the key button is released, reset the info of the scanner
		if (scannedObject != null) {
			if (slider.value == slider.maxValue || slider.value != slider.maxValue) {
				reset ();
			}
		}
	}

	public void executeScanner ()
	{
		//if there is a scannedObject detected
		if (scannedObject != null) {
			//check if the info of the object has been already scanned
			if (!currentObjectToScan.isScanned ()) {
				//in that case, scan the object
				scanStatus.text = scanningString;

				//while the key is held, increase the slider value
				slider.value += Time.deltaTime * scanSpeed;

				//when the slider reachs its max value
				if (slider.value == slider.maxValue) {
					//set the object to already scanned
					currentObjectToScan.scanObject ();

					//get the info of the object
					objectChecked ();
				}
			}
			//if the object has been already scanned
			else {
				//get the info of the object
				objectChecked ();
			}
		}
	}

	public void setScannedObjectInfo ()
	{
		currentObjectToScan = scannedObject.GetComponent<scanElementInfo> ();
		currentGameObjectInScanner = currentObjectToScan.dataGameObject;

		if (useWeakSpotScanner) {
			if (currentGameObjectInScanner != null) {
				currentScannedObjectHealth = currentGameObjectInScanner.GetComponent<health> ();

				if (currentScannedObjectHealth == null) {
					characterDamageReceiver currentCharacterDamageReceiver = GetComponent<characterDamageReceiver> ();

					if (currentCharacterDamageReceiver != null) {
						currentScannedObjectHealth = currentCharacterDamageReceiver.getHealthManager ();
					}
				}

				if (currentScannedObjectHealth != null) {
					if (currentScannedObjectHealth.advancedSettings.weakSpots.Count > 0) {
						addHealthWeakSpotList ();

						return;
					}
				} 

				currentScannedVehicle = currentGameObjectInScanner.GetComponent<vehicleHUDManager> ();

				if (currentScannedVehicle == null) {
					vehicleDamageReceiver currentVehicleDamageReceiver = currentGameObjectInScanner.GetComponent<vehicleDamageReceiver> ();

					currentScannedVehicle = currentVehicleDamageReceiver.getHUDManager ();
				}

				if (currentScannedVehicle != null) {
					if (currentScannedVehicle.advancedSettings.damageReceiverList.Count > 0) {
						addHealthWeakSpotList ();
					}
				}
			}
		}
	}

	//get the info from the scannable object, name and description
	void objectChecked ()
	{
		slider.value = slider.maxValue;
		objectInfo.text = currentObjectToScan.dataObject.info;
		objectName.text = currentObjectToScan.dataObject.name;
		scanStatus.text = scanCompleteString;
	}

	//reset the info of the scanner
	void reset ()
	{
		slider.value = 0;
		scanStatus.text = scanVisorActiveString;
		objectInfo.text = emptyScannerInfoString;
		objectName.text = "";

		scannedObject = null;
		currentObjectToScan = null;
		currentGameObjectInScanner = null;
		currentScannedObjectHealth = null;
		currentScannedVehicle = null;

		if (useWeakSpotScanner) {
			disableHealtWeakSpotList ();
		}

		objectToScanLocated = false;
	}

	//enable of disable the scanner according to the situation
	public void enableScanner ()
	{
		//if the player is not in aim mode, or the scanner mode is not already enabled, or using a device and the scanner mode is active in the feature manager
		if (!playerControllerManager.isPlayerAimingInThirdPerson () && !lookingObject && !playerControllerManager.isUsingDevice () && scannerSystemEnabled
		    && mainPlayerCamera.isCameraTypeFree ()) {

			//change its state
			scannerModeIsActivated = !scannerModeIsActivated;

			checkEventsOnEnableDisableScanner ();

			//if the scanner mode is enabled, check if the player is in first person mode
			if (scannerModeIsActivated) {
				isFirstPersonView = mainPlayerCamera.isFirstPersonActive ();
			}

			//if the player is not in first person mode, change it to that view
			if (!isFirstPersonView) {
				if (!ignoreChangeToFirstPersonEnabled) {
					mainPlayerCamera.changeCameraView ();
				}
			}

			reset ();

			if (scanIcon.activeSelf) {
				scanIcon.SetActive (false);
			}

			checkScannerCameraFov ();
		}
	}

	//disable the scanner from other script
	public void disableScanner ()
	{
		scannerModeIsActivated = false;

		reset ();

		if (scanIcon.activeSelf) {
			scanIcon.SetActive (false);
		}

		checkScannerCameraFov ();

		checkEventsOnEnableDisableScanner ();

		holdScannerInputActive = false;
	}

	public void checkEventsOnEnableDisableScanner ()
	{
		if (scannerModeIsActivated) {
			eventOnScannerEnabled.Invoke ();
		} else {
			eventOnScannerDisabled.Invoke ();
		}
	}

	public void checkScannerCameraFov ()
	{
		//check if the player set the zoom mode, so if the scanned mode is enabled or disabled the camera fov is correctly changed
		if (zoomActive) {
			//change the fov of the scanner camera
			if (scannerModeIsActivated) {
				enableOrDisableScannerZoom (true);
				//the scanner mode is enabled when the zoom was enabled
			} else {
				enableOrDisableScannerZoom (false);
				//the scanner mode is disabled when the zoom was enabled
			}
		} else {
			if (scannerModeIsActivated) {
				targetScannerCameraFov = initialCameraFov;
			} else {
				targetScannerCameraFov = originalScannerCameraFov;
			}

			checkFovCoroutine (targetScannerCameraFov);
		}
	}

	//change the zoom in the scanner camera if the player use the zoom
	public void enableOrDisableScannerZoom (bool value)
	{
		zoomActive = value;

		//decrease the fov 
		if (zoomActive) {
			//zoom enabled
			targetScannerCameraFov = zoomCameraFov;
		} else {
			//increase the fov
			if (scannerModeIsActivated) {
				targetScannerCameraFov = initialCameraFov;
				//zoom disabled when the scanner mode was enabled
			} else {
				targetScannerCameraFov = originalScannerCameraFov;
				//zoom disable when the scanner mode was disabled
			}
		}

		if (!zoomActive && mainPlayerCamera.isUsingZoom ()) {
			zoomActive = true;

			targetScannerCameraFov = mainCamera.fieldOfView;
		}

		checkScanDistance (value);

		checkFovCoroutine (targetScannerCameraFov);
	}

	public void checkScanDistance (bool value)
	{
		if (scanDistanceAffectedByZoom) {
			if (value) {
				distanceToScan += scanDistanceExtraWithZoom;
			} else {
				distanceToScan = originalScanDistance;
			}
		}
	}

	public bool isScannerActivated ()
	{
		return scannerModeIsActivated;
	}

	public void checkFovCoroutine (float targetValue)
	{
		if (changeFovCoroutine != null) {
			StopCoroutine (changeFovCoroutine);
		}

		changeFovCoroutine = StartCoroutine (changeFovValue (targetValue));
	}

	public IEnumerator changeFovValue (float targetValue)
	{
		if (scannerModeIsActivated) {
			enableOrDisableScannerCameraComponents (true);
		}

		//if the small screen in the center is enabled, change the fov to the scanner mode, checking also if the player use the zoom mode
		while (scannerMainCamera.fieldOfView != targetValue) {
			scannerMainCamera.fieldOfView = Mathf.MoveTowards (scannerMainCamera.fieldOfView, targetValue, Time.deltaTime * fovChangeSpeed);

			yield return null;
		}

		if (!scannerModeIsActivated) {
			enableOrDisableScannerCameraComponents (false);
		}
	}

	public void enableOrDisableScannerCameraComponents (bool state)
	{
		enableOrDisableScannerCamera (state);

		if (scannerCameraUI.activeSelf != state) {
			scannerCameraUI.SetActive (state);
		}

		if (scannerHUD.activeSelf != state) {
			scannerHUD.SetActive (state);
		}
	}

	public void enableOrDisableScannerCamera (bool state)
	{
		if (pauseEnableOrDisableScannerCameraElement) {
			return;
		}

		if (scannerMainCamera.enabled != state) {
			scannerMainCamera.enabled = state;
		}
	}

	public void toggleScannerCameraElement ()
	{
		enableOrDisableScannerCamera (!scannerMainCamera.enabled);
	}

	bool pauseEnableOrDisableScannerCameraElement;

	public void setPauseEnableOrDisableScannerCameraElementState (bool state)
	{
		pauseEnableOrDisableScannerCameraElement = state;
	}

	public void togglePauseEnableOrDisableScannerCameraElementState ()
	{
		setPauseEnableOrDisableScannerCameraElementState (!pauseEnableOrDisableScannerCameraElement);
	}

	public void addHealthWeakSpotList ()
	{
		if (!healthInfoParent.gameObject.activeSelf) {
			healthInfoParent.gameObject.SetActive (true);
		}

		if (currentScannedObjectHealth != null) {
			int weakSpotsAmount = currentScannedObjectHealth.advancedSettings.weakSpots.Count;

			for (int i = 0; i < weakSpotsAmount; i++) {
				if (i < healtWeakSpotList.Count) {
					healtWeakSpotList [i].target = currentScannedObjectHealth.advancedSettings.weakSpots [i].spotTransform;

					string spotText = "x" + currentScannedObjectHealth.advancedSettings.weakSpots [i].damageMultiplier;

					if (currentScannedObjectHealth.advancedSettings.weakSpots [i].killedWithOneShoot) {
						spotText = "Death";
					}

					healtWeakSpotList [i].healthSpot.text = spotText;
					healtWeakSpotList [i].used = true;
				} else {
					GameObject newWeakSpotMesh = (GameObject)Instantiate (healthElement, healthElement.transform.position, Quaternion.identity);
					newWeakSpotMesh.transform.SetParent (healthInfoParent);
					newWeakSpotMesh.transform.localScale = Vector3.one;
					newWeakSpotMesh.transform.localPosition = Vector3.zero;

					healthElementInfo newHealthElementInfo = newWeakSpotMesh.GetComponent<weakSpotInfo> ().elementInfo;
					newHealthElementInfo.target = currentScannedObjectHealth.advancedSettings.weakSpots [i].spotTransform;

					string spotText = "x" + currentScannedObjectHealth.advancedSettings.weakSpots [i].damageMultiplier;

					if (currentScannedObjectHealth.advancedSettings.weakSpots [i].killedWithOneShoot) {
						spotText = "Death";
					}

					newHealthElementInfo.healthSpot.text = spotText;
					newHealthElementInfo.used = true;
					healtWeakSpotList.Add (newHealthElementInfo);
				}
			}

			if (weakSpotsAmount < healtWeakSpotList.Count) {
				for (int i = weakSpotsAmount; i < healtWeakSpotList.Count; i++) {
					if (healtWeakSpotList [i].healthElementGameObject.activeSelf) {
						healtWeakSpotList [i].healthElementGameObject.SetActive (false);
					}

					healtWeakSpotList [i].used = false;
				}
			}

			objectScannedHealthNameText.text = currentScannedObjectHealth.getCharacterName ();

			objectScannedHealthAmountText.text = currentScannedObjectHealth.getCurrentHealthAmount ().ToString ();
		}

		if (currentScannedVehicle != null) {
			int weakSpotsAmount = currentScannedVehicle.advancedSettings.damageReceiverList.Count;

			for (int i = 0; i < weakSpotsAmount; i++) {
				if (i < healtWeakSpotList.Count) {
					healtWeakSpotList [i].target = currentScannedVehicle.advancedSettings.damageReceiverList [i].spotTransform;

					string spotText = "x" + currentScannedVehicle.advancedSettings.damageReceiverList [i].damageMultiplier;

					if (currentScannedVehicle.advancedSettings.damageReceiverList [i].killedWithOneShoot) {
						spotText = "Death";
					}

					healtWeakSpotList [i].healthSpot.text = spotText;
					healtWeakSpotList [i].used = true;
				} else {
					GameObject newWeakSpotMesh = (GameObject)Instantiate (healthElement, healthElement.transform.position, Quaternion.identity);
					newWeakSpotMesh.transform.SetParent (healthInfoParent);
					newWeakSpotMesh.transform.localScale = Vector3.one;
					newWeakSpotMesh.transform.localPosition = Vector3.zero;

					healthElementInfo newHealthElementInfo = newWeakSpotMesh.GetComponent<weakSpotInfo> ().elementInfo;
					newHealthElementInfo.target = currentScannedVehicle.advancedSettings.damageReceiverList [i].spotTransform;

					string spotText = "x" + currentScannedVehicle.advancedSettings.damageReceiverList [i].damageMultiplier;

					if (currentScannedVehicle.advancedSettings.damageReceiverList [i].killedWithOneShoot) {
						spotText = "Death";
					}

					newHealthElementInfo.healthSpot.text = spotText;
					newHealthElementInfo.used = true;
					healtWeakSpotList.Add (newHealthElementInfo);
				}
			}

			if (weakSpotsAmount < healtWeakSpotList.Count) {
				for (int i = weakSpotsAmount; i < healtWeakSpotList.Count; i++) {
					if (healtWeakSpotList [i].healthElementGameObject.activeSelf) {
						healtWeakSpotList [i].healthElementGameObject.SetActive (false);
					}

					healtWeakSpotList [i].used = false;
				}
			}

			objectScannedHealthNameText.text = currentScannedVehicle.getVehicleName ();

			objectScannedHealthAmountText.text = currentScannedVehicle.getCurrentHealthAmount ().ToString ();
		}
	}

	public void disableHealtWeakSpotList ()
	{
		for (int i = 0; i < healtWeakSpotList.Count; i++) {
			if (healtWeakSpotList [i].healthElementGameObject.activeSelf) {
				healtWeakSpotList [i].healthElementGameObject.SetActive (false);
			}

			healtWeakSpotList [i].used = false;
		}

		if (healthInfoParent.gameObject.activeSelf) {
			healthInfoParent.gameObject.SetActive (false);
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputHoldScanner ()
	{
		if (!scannerSystemEnabled) {
			return;
		}

		if (playerIsBusy ()) {
			return;
		}

		holdScannerInputActive = true;
	}

	public void inputReleaseScanner ()
	{
		if (!scannerSystemEnabled) {
			return;
		}

		if (playerIsBusy ()) {
			return;
		}

		if (!mainPlayerCamera.isFirstPersonActive () &&
		    !mainPlayerCamera.isChangeCameraViewEnabled () &&
		    !ignoreChangeToFirstPersonEnabled) {

			return;
		}

		holdScannerInputActive = false;

		activateScanner ();
	}

	public void setScannerSystemEnabledState (bool state)
	{
		scannerSystemEnabled = state;
	}

	public void setScannerSystemEnabledStateFromEditor (bool state)
	{
		setScannerSystemEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}