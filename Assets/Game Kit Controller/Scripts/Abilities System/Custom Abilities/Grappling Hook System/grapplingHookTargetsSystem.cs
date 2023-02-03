using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class grapplingHookTargetsSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showGrapplingHookTargetsActive = true;

	public LayerMask layerForThirdPerson;
	public LayerMask layerForFirstPerson;

	public bool checkObstaclesToGrapplingHookTargets;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showGrapplingHookTargetsPaused;
	public Transform closestTarget;
	public List<grapplingHookTargetInfo> grapplingHookTargetInfoList = new List<grapplingHookTargetInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject grapplingHookTargetIconPrefab;

	public Camera mainCamera;
	public Transform mainCameraTransform;

	public GameObject playerGameObject;

	public playerCamera mainPlayerCamera;

	public Transform grapplingHookTargetIconsParent;

	public List<grapplingHookSystem> grapplingHookSystemList = new List<grapplingHookSystem> ();

	public playerTeleportSystem mainPlayerTeleportSystem;

	Vector3 screenPoint;
	Vector3 currentPosition;
	Vector3 mainCameraPosition;
	Vector3 direction;
	float distanceToMainCamera;

	bool layerForThirdPersonAssigned;
	bool layerForFirstPersonAssigned;
	LayerMask currentLayer;
	bool activeIcon;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;
	Vector2 iconPosition2d;
	Vector3 currentTargetPosition;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	float screenWidth;
	float screenHeight;

	grapplingHookTargetInfo currentGrapplingHookTargetInfo;

	Transform previousClosestTarget;

	Vector3 centerScreen;

	float currentDistanceToTarget;

	float minDistanceToTarget;

	[HideInInspector] public grapplingHookTargetInfo closestTargetInfo;
	grapplingHookTargetInfo previousClosestTargetInfo;

	bool hookTargetsDetected;

	int grapplingHookTargetInfoListCount;

	void Start ()
	{
		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
	}

	void LateUpdate ()
	{
		if (!showGrapplingHookTargetsActive || showGrapplingHookTargetsPaused || !hookTargetsDetected) {
			return;
		}

		if (mainPlayerCamera.isFirstPersonActive ()) {
			if (!layerForThirdPersonAssigned) {
				currentLayer = layerForFirstPerson;
				layerForThirdPersonAssigned = true;
				layerForFirstPersonAssigned = false;
			}
		} else {
			if (!layerForFirstPersonAssigned) {
				currentLayer = layerForThirdPerson;
				layerForFirstPersonAssigned = true;
				layerForThirdPersonAssigned = false;
			}
		}

		if (!usingScreenSpaceCamera) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}

		mainCameraPosition = mainCameraTransform.position;

		minDistanceToTarget = Mathf.Infinity;
		centerScreen = new Vector3 (screenWidth / 2, screenHeight / 2, 0);

		bool anyTargetVisible = false;

		grapplingHookTargetInfoListCount = grapplingHookTargetInfoList.Count;

		for (int i = 0; i < grapplingHookTargetInfoListCount; i++) {
			currentGrapplingHookTargetInfo = grapplingHookTargetInfoList [i];
				
			if (currentGrapplingHookTargetInfo.targetCanBeShown) {
				currentPosition = currentGrapplingHookTargetInfo.targetTransform.position;

				currentTargetPosition = currentPosition + currentGrapplingHookTargetInfo.positionOffset;

				if (usingScreenSpaceCamera) {
					screenPoint = mainCamera.WorldToViewportPoint (currentTargetPosition);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
				} else {
					screenPoint = mainCamera.WorldToScreenPoint (currentTargetPosition);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
				}

				if (targetOnScreen) {
					if (currentGrapplingHookTargetInfo.iconCurrentlyEnabled) {
						if (usingScreenSpaceCamera) {
							iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

							currentGrapplingHookTargetInfo.targetRectTransform.anchoredPosition = iconPosition2d;
						} else {
							currentGrapplingHookTargetInfo.targetRectTransform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
						}
					}

					if (checkObstaclesToGrapplingHookTargets) {
						//set the direction of the raycast
						direction = currentTargetPosition - mainCameraPosition;
						direction = direction / direction.magnitude;
						activeIcon = false;

						distanceToMainCamera = GKC_Utils.distance (mainCameraPosition, currentTargetPosition);

						//if the raycast find an obstacle between the enemy and the camera, disable the icon
						//if the distance from the camera to the enemy is higher than 100, disable the raycast and the icon
						if (Physics.Raycast (currentTargetPosition, -direction, distanceToMainCamera, currentLayer)) {
							activeIcon = false;
						} else {
							//else, the raycast reachs the camera, so enable the pick up icon
							activeIcon = true;
						}

						currentGrapplingHookTargetInfo.targetVisible = activeIcon;
					} else {
						currentGrapplingHookTargetInfo.targetVisible = true;
					}

					if (currentGrapplingHookTargetInfo.targetVisible) {
						screenPoint = mainCamera.WorldToScreenPoint (currentPosition);

						currentDistanceToTarget = GKC_Utils.distance (screenPoint, centerScreen);
					
						if (currentDistanceToTarget < minDistanceToTarget) {
							minDistanceToTarget = currentDistanceToTarget;

							closestTargetInfo = currentGrapplingHookTargetInfo;
						}

						anyTargetVisible = true;
					}
				} else {
					currentGrapplingHookTargetInfo.targetVisible = false;
				}
			} else {
				currentGrapplingHookTargetInfo.targetVisible = false;
			}
		}

		if (anyTargetVisible) {
			if (closestTargetInfo != null) {
				closestTarget = closestTargetInfo.targetTransform;

				if (closestTarget != previousClosestTarget) {

					previousClosestTarget = closestTarget;

					if (previousClosestTargetInfo != null) {
						if (previousClosestTargetInfo.iconCurrentlyEnabled) {
							previousClosestTargetInfo.targetRectTransform.gameObject.SetActive (false);
							previousClosestTargetInfo.iconCurrentlyEnabled = false;
						}
					}

					previousClosestTargetInfo = closestTargetInfo;

					closestTargetInfo.targetRectTransform.gameObject.SetActive (true);
					closestTargetInfo.iconCurrentlyEnabled = true;

					for (int i = 0; i < grapplingHookSystemList.Count; i++) {
						grapplingHookSystemList [i].setGrapplingHookTarget (closestTarget);
					}

					mainPlayerTeleportSystem.setTeleportHookTarget (closestTarget);
				}
			}
		} else {
			if (closestTargetInfo != null && closestTargetInfo.targetRectTransform != null) {
				closestTargetInfo.targetRectTransform.gameObject.SetActive (false);
				closestTargetInfo.iconCurrentlyEnabled = false;

				closestTargetInfo = null;

				closestTarget = null;
				previousClosestTarget = null;

				for (int i = 0; i < grapplingHookSystemList.Count; i++) {
					grapplingHookSystemList [i].setGrapplingHookTarget (null);
				}

				mainPlayerTeleportSystem.setTeleportHookTarget (null);
			}
		}
	}

	public void setCurrentGrapplingHookTargetState (bool state)
	{
		currentGrapplingHookTargetInfo.targetRectTransform.gameObject.SetActive (state);

		currentGrapplingHookTargetInfo.iconCurrentlyEnabled = state;
	}

	public void disableGrapplingHookTargets ()
	{
		enableOrDisableGrapplingHookTargets (false);
	}

	public void enableGrapplingHookTargets ()
	{
		enableOrDisableGrapplingHookTargets (true);
	}

	public void enableOrDisableGrapplingHookTargets (bool state)
	{
		for (int i = 0; i < grapplingHookTargetInfoList.Count; i++) {
			currentGrapplingHookTargetInfo = grapplingHookTargetInfoList [i];

			if (currentGrapplingHookTargetInfo.targetTransform != null) {
				currentGrapplingHookTargetInfo.targetTransform.gameObject.SetActive (state);
				currentGrapplingHookTargetInfo.iconCurrentlyEnabled = state;
			}
		}
	}

	public void addNewGrapplingHookTarget (Transform targetTransform)
	{
		grapplingHookTargetInfo newGrapplingHookTargetInfo = new grapplingHookTargetInfo ();

		GameObject grapplingHookTargetGameObject = Instantiate (grapplingHookTargetIconPrefab);

		grapplingHookTargetGameObject.name = "Grappling Hook Target " + grapplingHookTargetInfoList.Count;

		grapplingHookTargetGameObject.transform.SetParent (grapplingHookTargetIconsParent);
		grapplingHookTargetGameObject.transform.localScale = Vector3.one;
		grapplingHookTargetGameObject.transform.localPosition = Vector3.zero;
		grapplingHookTargetGameObject.transform.localRotation = Quaternion.identity;

		newGrapplingHookTargetInfo.targetTransform = targetTransform;

		newGrapplingHookTargetInfo.iconCurrentlyEnabled = false;

		newGrapplingHookTargetInfo.targetRectTransform = grapplingHookTargetGameObject.GetComponent<RectTransform> ();

		grapplingHookTargetInfoList.Add (newGrapplingHookTargetInfo);

		hookTargetsDetected = true;
	}

	public void removeNewGrapplingHookTarget (Transform targetTransform)
	{
		for (int i = 0; i < grapplingHookTargetInfoList.Count; i++) {
			if (grapplingHookTargetInfoList [i].targetTransform == targetTransform) {
				grapplingHookTargetInfoList [i].iconCurrentlyEnabled = false;

				Destroy (grapplingHookTargetInfoList [i].targetRectTransform.gameObject);

				grapplingHookTargetInfoList.RemoveAt (i);

				if (grapplingHookTargetInfoList.Count == 0) {
					for (int j = 0; j < grapplingHookSystemList.Count; j++) {
						grapplingHookSystemList [j].setGrapplingHookTarget (null);
					}

					mainPlayerTeleportSystem.setTeleportHookTarget (null);

					hookTargetsDetected = false;
				}

				return;
			}
		}
	}

	public void pauseOrResumeShowGrapplingHookTargets (bool state)
	{
		showGrapplingHookTargetsPaused = state;

		grapplingHookTargetIconsParent.gameObject.SetActive (!showGrapplingHookTargetsPaused);
	}

	[System.Serializable]
	public class grapplingHookTargetInfo
	{
		public Transform targetTransform;
		public RectTransform targetRectTransform;

		public bool targetCanBeShown = true;
		public Vector3 positionOffset;

		public bool targetVisible;

		public bool iconCurrentlyEnabled;
	}
}