using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class playerPickupIconManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showIconsActive = true;
	public bool showIconsPaused;

	public LayerMask layer;
	public LayerMask layerForFirstPerson;

	public checkIconType checkIcontype;
	public float maxDistanceIconEnabled;

	public string mainManagerName = "Pickup Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public List<pickUpIconInfo> pickUpIconList = new List<pickUpIconInfo> ();
	public bool layerForThirdPersonAssigned;
	public bool layerForFirstPersonAssigned;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject pickUpIconObject;
	public Transform pickupObjectIconParent;

	public Camera mainCamera;
	public pickUpManager mainPickupManager;
	public playerController playerControllerManager;
	public playerCamera mainPlayerCamera;

	Transform mainCameraTransform;
	Vector3 targetPosition;
	Vector3 cameraPosition;
	GameObject currentIconObject;
	Vector3 screenPoint;
	Vector3 direction;
	float distance;

	LayerMask currentLayer;

	//how to check if the icon is visible,
	//		-using a raycast from the object to the camera
	//		-using distance from the object to the player position
	//		-visible always that the player is looking at the object position

	public enum checkIconType
	{
		raycast,
		distance,
		always_visible,
		nothing
	}

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	float screenWidth;
	float screenHeight;

	pickUpIconInfo currentPickUpIconInfo;

	int pickUpIconListCount;

	bool screenResolutionAssigned;

	void Awake ()
	{
		if (mainPickupManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(pickUpManager));

			mainPickupManager = FindObjectOfType<pickUpManager> ();
		} 

		if (mainPickupManager != null) {
			mainPickupManager.addNewPlayer (this);
		} else {
			showIconsActive = false;
		}
	}

	void Start ()
	{
		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}

		mainCameraTransform = mainCamera.transform;

		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();
	}

	void FixedUpdate ()
	{
		if (!showIconsActive || showIconsPaused) {
			return;
		}

		pickUpIconListCount = pickUpIconList.Count;

		if (pickUpIconListCount == 0) {
			return;
		}

		if (playerControllerManager.isPlayerOnFirstPerson ()) {
			if (!layerForThirdPersonAssigned) {
				currentLayer = layerForFirstPerson;
				layerForThirdPersonAssigned = true;
				layerForFirstPersonAssigned = false;
			}
		} else {
			if (!layerForFirstPersonAssigned) {
				currentLayer = layer;
				layerForFirstPersonAssigned = true;
				layerForThirdPersonAssigned = false;
			}
		}

		if (!usingScreenSpaceCamera) {
			if (!screenResolutionAssigned) {
				updateScreenValues ();

				screenResolutionAssigned = true;
			}
		}

		cameraPosition = mainCameraTransform.position;
			
		for (int i = 0; i < pickUpIconListCount; i++) {
			currentPickUpIconInfo = pickUpIconList [i];
				
			currentIconObject = currentPickUpIconInfo.iconObject;

			if (currentPickUpIconInfo.paused) {
				if (currentPickUpIconInfo.iconActive) {
					enableOrDisableIcon (false, i);

					currentPickUpIconInfo.iconActive = false;
				}
			} else {

				//get the target position from global to local in the screen
				targetPosition = currentPickUpIconInfo.targetTransform.position;

				if (usingScreenSpaceCamera) {
					screenPoint = mainCamera.WorldToViewportPoint (targetPosition);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
				} else {
					screenPoint = mainCamera.WorldToScreenPoint (targetPosition);
					targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
				}

				//if the target is visible in the screen, enable the icon
				if (targetOnScreen) {
					if (usingScreenSpaceCamera) {
						iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

						currentPickUpIconInfo.iconRectTransform.anchoredPosition = iconPosition2d;
					} else {
						currentIconObject.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
					}
						
					//use a raycast to check if the icon is visible
					if (checkIcontype == checkIconType.raycast) {
						distance = GKC_Utils.distance (targetPosition, cameraPosition);

						if (distance <= maxDistanceIconEnabled) {
							//set the direction of the raycast
							direction = targetPosition - cameraPosition;
							direction = direction / direction.magnitude;

							//Debug.DrawRay(target.transform.position,-direction*distance,Color.red);
							//if the raycast find an obstacle between the pick up and the camera, disable the icon
							if (Physics.Raycast (targetPosition, -direction, distance, currentLayer)) {
								if (currentPickUpIconInfo.iconActive) {
									enableOrDisableIcon (false, i);

									currentPickUpIconInfo.iconActive = false;
								}
							} else {
								//else, the raycast reachs the camera, so enable the pick up icon
								if (!currentPickUpIconInfo.iconActive) {
									enableOrDisableIcon (true, i);

									currentPickUpIconInfo.iconActive = true;
								}
							}
						} else {
							if (currentPickUpIconInfo.iconActive) {
								enableOrDisableIcon (false, i);

								currentPickUpIconInfo.iconActive = false;
							}
						}
					} else if (checkIcontype == checkIconType.distance) {
						//if the icon uses the distance, then check it
						distance = GKC_Utils.distance (targetPosition, cameraPosition);

						if (distance <= maxDistanceIconEnabled) {
							if (!currentPickUpIconInfo.iconActive) {
								enableOrDisableIcon (true, i);

								currentPickUpIconInfo.iconActive = true;
							}
						} else {
							if (currentPickUpIconInfo.iconActive) {
								enableOrDisableIcon (false, i);

								currentPickUpIconInfo.iconActive = false;
							}
						}
					} else {
						//else, always visible when the player is looking at its direction
						if (!currentPickUpIconInfo.iconActive) {
							enableOrDisableIcon (true, i);

							currentPickUpIconInfo.iconActive = true;
						}
					}
				} else {
					//else the icon is only disabled, when the player is not looking at its direction
					if (currentPickUpIconInfo.iconActive) {
						enableOrDisableIcon (false, i);

						currentPickUpIconInfo.iconActive = false;
					}
				}
			}
		}
	}

	public void enableOrDisableIcon (bool state, int index)
	{
		pickUpIconList [index].iconObject.SetActive (state);
	}

	//set what type of pick up is this object, and the object that the icon has to follow
	public void setPickUpIcon (GameObject target, Texture targetTexture, int objectID, GameObject iconPrefab)
	{
		if (checkIcontype == checkIconType.nothing) {
			return;
		}

		GameObject currentIconPrefab = pickUpIconObject;

		if (iconPrefab != null) {
			currentIconPrefab = iconPrefab;
		}

		GameObject newIconElement = (GameObject)Instantiate (currentIconPrefab, Vector3.zero, Quaternion.identity, pickupObjectIconParent);

		pickUpIconInfo newIcon = newIconElement.GetComponent<pickUpIcon> ().pickUpElementInfo;

		newIconElement.transform.localScale = Vector3.one;
		newIconElement.transform.localPosition = Vector3.zero;

		newIcon.target = target;
		newIcon.targetTransform = target.transform;

		newIconElement.gameObject.SetActive (true);
		newIcon.ID = objectID;

		if (targetTexture != null) {
			newIcon.pickupIconImage.texture = targetTexture;
		}

		newIcon.iconActive = true;

		pickUpIconList.Add (newIcon);

		if (!showIconsActive) {
			newIcon.iconObject.SetActive (false);

			newIcon.iconActive = false;
		}
	}

	//destroy the icon
	public void removeAtTarget (int index)
	{
		if (index < pickUpIconList.Count) {
			if (pickUpIconList [index].iconObject) {
				Destroy (pickUpIconList [index].iconObject);
			}

			mainPickupManager.removeElementFromPickupListCalledByPlayer (pickUpIconList [index].ID);
			pickUpIconList.RemoveAt (index);

		} else {
			print ("WARNING: the index to remove in player pickup icon manager is not correct, check the object picked to see if the icon is configured correctly");
		}
	}

	public void removeAtTargetByID (int objectID)
	{
		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].ID == objectID) {
				if (pickUpIconList [i].iconObject) {
					Destroy (pickUpIconList [i].iconObject);
				}

				pickUpIconList.RemoveAt (i);

				return;
			}
		}
	}

	public void setPauseState (bool state, int index)
	{
		pickUpIconList [index].paused = state;
	}

	public void pauseOrResumeShowIcons (bool state)
	{
		showIconsPaused = state;

		pickupObjectIconParent.gameObject.SetActive (!showIconsPaused);
	}

	public void setShowIconsActiveState (bool state)
	{
		showIconsActive = state;
	}

	public void updateScreenValues ()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
}