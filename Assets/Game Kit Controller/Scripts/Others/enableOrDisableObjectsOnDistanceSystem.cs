using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class enableOrDisableObjectsOnDistanceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkObjectsActive = true;

	public bool checkForNullObjects = true;

	public float maxDistanceObjectEnabledOnScreen;
	public float maxDistanceObjectEnableOutOfScreen;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showObjectsPaused;

	public List<objectInfo> objectInfoList = new List<objectInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Camera mainCamera;
	public playerCamera mainPlayerCamera;

	Transform mainCameraTransform;
	Vector3 targetPosition;
	Vector3 cameraPosition;
	Transform currentObject;
	Vector3 screenPoint;
	float distance;

	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	float screenWidth;
	float screenHeight;

	objectInfo currentObjectInfo;

	int objectListCount;

	bool screenResolutionAssigned;

	void Start ()
	{
		if (mainCamera == null) {
			mainPlayerCamera = GKC_Utils.findMainPlayerCameraOnScene ();

			if (mainPlayerCamera != null) {
				mainCamera = mainPlayerCamera.getMainCamera ();
			}
		}

		if (mainCamera != null) {
			mainCameraTransform = mainCamera.transform;

			usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();
		} else {
			checkObjectsActive = false;
		}
	}

	void FixedUpdate ()
	{
		if (!checkObjectsActive || showObjectsPaused) {
			return;
		}

		objectListCount = objectInfoList.Count;

		if (objectListCount == 0) {
			return;
		}

		if (!usingScreenSpaceCamera) {
			if (!screenResolutionAssigned) {
				updateScreenValues ();

				screenResolutionAssigned = true;
			}
		}

		cameraPosition = mainCameraTransform.position;

		for (int i = 0; i < objectListCount; i++) {
			currentObjectInfo = objectInfoList [i];

			if (checkForNullObjects) {
				if (currentObjectInfo.objectTransform == null) {
					objectInfoList.RemoveAt (i);

					return;
				}
			}

			//get the target position from global to local in the screen
			targetPosition = currentObjectInfo.objectTransform.position;

			if (usingScreenSpaceCamera) {
				screenPoint = mainCamera.WorldToViewportPoint (targetPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
			} else {
				screenPoint = mainCamera.WorldToScreenPoint (targetPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
			}

			distance = GKC_Utils.distance (targetPosition, cameraPosition);

			//if the target is visible in the screen, enable the icon
			if (targetOnScreen) {

				bool checkResult = false;

				if (currentObjectInfo.useCustomDistance) {
					checkResult = (distance <= currentObjectInfo.maxDistanceObjectEnabledOnScreen);
				} else {
					checkResult = (distance <= maxDistanceObjectEnabledOnScreen);
				}

				if (checkResult) {
					if (!currentObjectInfo.objectActive) {
						enableOrDisableObject (true, i);

						currentObjectInfo.objectActive = true;
					}
				} else {
					if (currentObjectInfo.objectActive) {
						enableOrDisableObject (false, i);

						currentObjectInfo.objectActive = false;
					}
				}
			} else {
				bool checkResult = false;

				if (currentObjectInfo.useCustomDistance) {
					checkResult = (distance <= currentObjectInfo.maxDistanceObjectEnableOutOfScreen);
				} else {
					checkResult = (distance <= maxDistanceObjectEnableOutOfScreen);
				}

				//else the icon is only disabled, when the player is not looking at its direction
				if (checkResult) {
					if (!currentObjectInfo.objectActive) {
						enableOrDisableObject (true, i);

						currentObjectInfo.objectActive = true;
					}
				} else {
					if (currentObjectInfo.objectActive) {
						enableOrDisableObject (false, i);

						currentObjectInfo.objectActive = false;
					}
				}
			}
		}
	}

	public void enableOrDisableObject (bool state, int index)
	{
		objectInfoList [index].objectTransform.gameObject.SetActive (state);

		objectInfoList [index].mainEnableOrDisableObjectOnDistanceManager.setActiveState (state);
	}

	//set what type of pick up is this object, and the object that the icon has to follow
	public void addObject (enableOrDisableObjectOnDistanceManager newEnableOrDisableObjectOnDistanceManager)
	{
		objectInfo newObjectInfo = new objectInfo ();

		newObjectInfo.objectTransform = newEnableOrDisableObjectOnDistanceManager.mainTransform;

		newObjectInfo.mainEnableOrDisableObjectOnDistanceManager = newEnableOrDisableObjectOnDistanceManager;

		newObjectInfo.objectActive = true;

		newObjectInfo.useCustomDistance = newEnableOrDisableObjectOnDistanceManager.useCustomDistance;
		newObjectInfo.maxDistanceObjectEnabledOnScreen = newEnableOrDisableObjectOnDistanceManager.maxDistanceObjectEnabledOnScreen;
		newObjectInfo.maxDistanceObjectEnableOutOfScreen = newEnableOrDisableObjectOnDistanceManager.maxDistanceObjectEnableOutOfScreen;

		objectInfoList.Add (newObjectInfo);

		if (!checkObjectsActive) {
			newObjectInfo.objectTransform.gameObject.SetActive (false);

			newObjectInfo.objectActive = false;
		}
	}

	public void removeObject (Transform newTransform)
	{
		if (newTransform == null) {
			return;
		}

		int objectIndex = objectInfoList.FindIndex (s => s.objectTransform == newTransform);

		if (objectIndex > -1) {
			objectInfoList.RemoveAt (objectIndex);
		}
	}

	public void updateScreenValues ()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}

	[System.Serializable]
	public class objectInfo
	{
		public int ID;
		public Transform objectTransform;

		public bool useCustomDistance;

		public float maxDistanceObjectEnabledOnScreen;
		public float maxDistanceObjectEnableOutOfScreen;

		public bool objectActive;

		public enableOrDisableObjectOnDistanceManager mainEnableOrDisableObjectOnDistanceManager;
	}
}