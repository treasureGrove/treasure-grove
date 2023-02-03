using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealthBarManagementSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showSlidersActive = true;

	public bool disableHealthBarsOnLockedCamera;

	public bool showSlidersPaused;

	public LayerMask layer;
	public LayerMask layerForFirstPerson;

	public float distanceToShowSlider = 200;

	public checkBarTypes checkBarType;
	public showSliderModeFirstTime showSliderFirstTimeMode;

	public string mainManagerName = "Health Bar Manager";

	[Space]
	[Header ("Health Slider Info On Screen Settings")]
	[Space]

	public bool useHealthSlideInfoOnScreen;

	public GameObject mainHealtSliderInfoOnScreenGameObject;
	public AIHealtSliderInfo mainHealtSliderInfoOnScreen;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<healthSliderInfo> healthSliderInfoList = new List<healthSliderInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Camera mainCamera;
	public Transform mainCameraTransform;

	public GameObject playerGameObject;
	public playerController playerControllerManager;

	public playerCamera mainPlayerCamera;

	public Transform healtSlidersParent;

	public healthBarManagementSystem healthBarManager;

	public enum checkBarTypes
	{
		raycast,
		distance,
		always_visible
	}

	public enum showSliderModeFirstTime
	{
		raycast,
		distance,
		inScreen
	}

	Vector3 screenPoint;
	Vector3 currentPosition;
	Vector3 mainCameraPosition;
	Vector3 mainCameraForward;
	Vector3 upDirection = Vector3.up;
	Vector3 positionUpDirection;
	Vector3 direction;
	float distanceToMainCamera;

	RaycastHit hit;

	healthSliderInfo currentHealthSliderInfo;

	bool layerForThirdPersonAssigned;
	bool layerForFirstPersonAssigned;
	LayerMask currentLayer;
	bool activeIcon;

	bool checkDisableHealthBars;

	bool isLockedCameraActive;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;
	Vector2 iconPosition2d;
	Vector3 currentSliderPosition;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	float screenWidth;
	float screenHeight;

	Vector3 centerScreen;

	int healthSliderInfoListCount;

	float currentDistanceToTarget;
	float minDistanceToTarget;

	int currentTargetIndex;
	int previousTargetIndex = -1;
	int currentTargetID;

	GameObject objectToCheck;

	float currentSliderOffset;

	void Awake ()
	{
		if (healthBarManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(healthBarManagementSystem));

			healthBarManager = FindObjectOfType<healthBarManagementSystem> ();
		} 

		if (healthBarManager != null) {
			healthBarManager.addNewPlayer (this);
		} else {
			showSlidersActive = false;
		}
	}

	void Start ()
	{
		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
	}

	void FixedUpdate ()
	{
		if (!showSlidersActive || showSlidersPaused) {
			return;
		}

		healthSliderInfoListCount = healthSliderInfoList.Count;

		if (healthSliderInfoListCount == 0) {
			return;
		}

		isLockedCameraActive = !mainPlayerCamera.isCameraTypeFree ();

		if (disableHealthBarsOnLockedCamera) {
			if (isLockedCameraActive) {
				if (!checkDisableHealthBars) {
					disableHealhtBars ();
					checkDisableHealthBars = true;
				}
			} else {
				checkDisableHealthBars = false;
			}

			if (isLockedCameraActive) {
				return;
			}
		} else {
			checkDisableHealthBars = false;
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
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			if (useHealthSlideInfoOnScreen) {
				centerScreen = new Vector3 (screenWidth / 2, screenHeight / 2, 0);
			}
		}

		mainCameraPosition = mainCameraTransform.position;
		mainCameraForward = mainCameraTransform.forward;

		if (isLockedCameraActive) {
			mainCameraPosition = mainPlayerCamera.getCurrentLockedCameraTransform ().position;
		}

		//if the health slider has been created, set its position in the screen, so the slider follows the object position
		//to make the slider visible, the player has to see directly the object
		//also, the slider is disabled, when the object is not visible in the screen

		if (useHealthSlideInfoOnScreen) {
			currentDistanceToTarget = 0;
			minDistanceToTarget = Mathf.Infinity;
			currentTargetIndex = -1;
			currentTargetID = -1;
		}

		for (int i = 0; i < healthSliderInfoListCount; i++) {
			currentHealthSliderInfo = healthSliderInfoList [i];

//			if (currentHealthSliderInfo.sliderGameObject != null) {
			if (currentHealthSliderInfo.sliderOwner != null) {
				if (currentHealthSliderInfo.sliderCanBeShown) {
					currentPosition = currentHealthSliderInfo.sliderOwner.position;
			
					positionUpDirection = currentPosition + upDirection;
			
					if (currentHealthSliderInfo.useSliderOffset) {
						currentSliderPosition = currentPosition + currentHealthSliderInfo.sliderOffset;
					}
			
					if (usingScreenSpaceCamera) {
						screenPoint = mainCamera.WorldToViewportPoint (currentSliderPosition);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
					} else {
						screenPoint = mainCamera.WorldToScreenPoint (currentSliderPosition);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
					}

					if (targetOnScreen && currentHealthSliderInfo.sliderOwnerLocated) {
						if (usingScreenSpaceCamera) {
							iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

							currentHealthSliderInfo.sliderRectTransform.anchoredPosition = iconPosition2d;
						} else {
							currentHealthSliderInfo.sliderGameObject.transform.position = screenPoint;
						}

						if (checkBarType == checkBarTypes.raycast) {

							//set the direction of the raycast
							direction = positionUpDirection - mainCameraPosition;
							direction = direction / direction.magnitude;
							distanceToMainCamera = GKC_Utils.distance (currentPosition, mainCameraPosition);
							activeIcon = false;

							//if the raycast find an obstacle between the enemy and the camera, disable the icon
							//if the distance from the camera to the enemy is higher than 100, disable the raycast and the icon
							if (distanceToMainCamera < distanceToShowSlider) {
								if (Physics.Raycast (positionUpDirection, -direction, out hit, distanceToMainCamera, currentLayer)) {

									if (showGizmo) {
										Debug.DrawRay (positionUpDirection, -direction * hit.distance, Color.red);
									}

									if (hit.collider.gameObject == currentHealthSliderInfo.sliderOwner) {
										activeIcon = true;
									}
								} else {
									//else, the raycast reachs the camera, so enable the pick up icon
									if (showGizmo) {
										Debug.DrawRay (positionUpDirection, -direction * distanceToMainCamera, Color.green);
									}

									activeIcon = true;
								}
							}

							if (currentHealthSliderInfo.iconCurrentlyEnabled != activeIcon) {
								setCurrentHealthSliderState (activeIcon);
							}
						} else if (checkBarType == checkBarTypes.distance) {
							//if the icon uses the distance, then check it
							distanceToMainCamera = GKC_Utils.distance (currentPosition, mainCameraPosition);

							if (distanceToMainCamera < distanceToShowSlider) {
								if (!currentHealthSliderInfo.iconCurrentlyEnabled) {
									setCurrentHealthSliderState (true);
								}
							} else {
								if (currentHealthSliderInfo.iconCurrentlyEnabled) {
									setCurrentHealthSliderState (false);
								}
							}
						} else {
							if (!currentHealthSliderInfo.iconCurrentlyEnabled) {
								setCurrentHealthSliderState (true);
							}
						}


						if (useHealthSlideInfoOnScreen) {
							if (currentHealthSliderInfo.useHealthSlideInfoOnScreen && currentHealthSliderInfo.iconCurrentlyEnabled) {

								currentDistanceToTarget = GKC_Utils.distance (screenPoint, centerScreen);


								if (currentDistanceToTarget < minDistanceToTarget) {
									minDistanceToTarget = currentDistanceToTarget;

									currentTargetID = currentHealthSliderInfo.ID;

									currentTargetIndex = i;
								} 
							}
						}
					} else {
						if (currentHealthSliderInfo.iconCurrentlyEnabled) {
							setCurrentHealthSliderState (false);
						}
					}

					//if the slider is not visible yet, check the camera position
					if (!currentHealthSliderInfo.sliderOwnerLocated) {
						if (isLockedCameraActive) {
							if (targetOnScreen) {
								currentHealthSliderInfo.sliderOwnerLocated = true;
							}
						} else {
							if (showSliderFirstTimeMode == showSliderModeFirstTime.raycast) {
								//when the player looks at the enemy position, enable his slider health bar
								if (Physics.Raycast (mainCameraPosition, mainCameraForward, out hit, distanceToShowSlider, currentLayer)) {
									objectToCheck = hit.collider.gameObject;

									if (objectToCheck == currentHealthSliderInfo.sliderOwner.gameObject ||
									   objectToCheck.transform.IsChildOf (currentHealthSliderInfo.sliderOwner)) {

										currentHealthSliderInfo.sliderOwnerLocated = true;
									}
								}
							} else if (showSliderFirstTimeMode == showSliderModeFirstTime.inScreen) {
								if (targetOnScreen) {
									currentHealthSliderInfo.sliderOwnerLocated = true;
								}
							} else if (showSliderFirstTimeMode == showSliderModeFirstTime.distance) {
								float distance = GKC_Utils.distance (currentPosition, mainCameraPosition);

								if (distance < distanceToShowSlider) {
									currentHealthSliderInfo.sliderOwnerLocated = true;
								}
							}
						}
					}
				} else {
					if (currentHealthSliderInfo.iconCurrentlyEnabled) {
						setCurrentHealthSliderState (false);
					}
				}
			} else {
				removeElementFromListByPlayer (currentHealthSliderInfo.ID);

				healthSliderInfoListCount = healthSliderInfoList.Count;

				return;
			}
		}

		if (useHealthSlideInfoOnScreen) {
			if (currentTargetIndex > -1 && healthSliderInfoListCount > currentTargetIndex) {
				if (currentTargetIndex != previousTargetIndex) {
					if (!mainHealtSliderInfoOnScreenGameObject.activeSelf) {
						mainHealtSliderInfoOnScreenGameObject.SetActive (true);
					}

					healthSliderInfo mainHealthSliderInfo = healthSliderInfoList [currentTargetIndex];

					mainHealtSliderInfoOnScreen.healthSlider.maxValue = mainHealthSliderInfo.healthSlider.maxValue;
					mainHealtSliderInfoOnScreen.healthSlider.value = mainHealthSliderInfo.healthSlider.value;

					if (mainHealtSliderInfoOnScreen.nameText != null) {
						mainHealtSliderInfoOnScreen.nameText.text = mainHealthSliderInfo.Name;
					}

					if (mainHealthSliderInfo.shieldSlider != null) {
						if (mainHealtSliderInfoOnScreen.shieldSlider != null) {
							if (!mainHealtSliderInfoOnScreen.shieldSlider.gameObject.activeSelf) {
								mainHealtSliderInfoOnScreen.shieldSlider.gameObject.SetActive (true);
							}

							mainHealtSliderInfoOnScreen.shieldSlider.maxValue = mainHealthSliderInfo.shieldSlider.maxValue;
							mainHealtSliderInfoOnScreen.shieldSlider.value = mainHealthSliderInfo.shieldSlider.value;
						}
					} else {
						if (mainHealtSliderInfoOnScreen.shieldSlider != null) {
							if (mainHealtSliderInfoOnScreen.shieldSlider.gameObject.activeSelf) {
								mainHealtSliderInfoOnScreen.shieldSlider.gameObject.SetActive (false);
							}
						}
					}

//					mainHealtSliderInfoOnScreen.nameText.color = mainHealthSliderInfo.sliderInfo.nameText.color;

					if (mainHealthSliderInfo.sliderInfo.sliderBackground != null) {
						mainHealtSliderInfoOnScreen.sliderBackground.color = mainHealthSliderInfo.sliderInfo.sliderBackground.color;
					}

					previousTargetIndex = currentTargetIndex;
				}
			} else {
				if (previousTargetIndex != -1) {
					if (mainHealtSliderInfoOnScreenGameObject.activeSelf) {
						mainHealtSliderInfoOnScreenGameObject.SetActive (false);
					}

					previousTargetIndex = -1;

					currentTargetIndex = -1;

					currentTargetID = -1;
				}
			}
		}
	}

	public void setCurrentHealthSliderState (bool state)
	{
		if (currentHealthSliderInfo.useHealthSlideInfoOnScreen) {
			currentHealthSliderInfo.sliderGameObject.SetActive (false);
		} else {
			currentHealthSliderInfo.sliderGameObject.SetActive (state);
		}

		currentHealthSliderInfo.iconCurrentlyEnabled = state;
	}

	public void disableHealhtBars ()
	{
		enableOrDisableHealhtBars (false);
	}

	public void enableHealhtBars ()
	{
		enableOrDisableHealhtBars (true);
	}

	public void enableOrDisableHealhtBars (bool state)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			currentHealthSliderInfo = healthSliderInfoList [i];

			if (currentHealthSliderInfo.sliderGameObject != null && currentHealthSliderInfo.sliderOwner != null) {
				currentHealthSliderInfo.sliderGameObject.SetActive (state);
				currentHealthSliderInfo.iconCurrentlyEnabled = state;
			}
		}
	}

	public void addNewTargetSlider (GameObject sliderOwner, GameObject sliderPrefab, Vector3 sliderOffset, float healthAmount, float shieldAmount, 
	                                string ownerName, Color textColor, Color sliderColor, int objectID, bool healthBarSliderActiveOnStart, bool useHealthSlideInfoOnScreenValue)
	{
		healthSliderInfo newHealthSliderInfo = new healthSliderInfo ();
		newHealthSliderInfo.Name = ownerName;
		newHealthSliderInfo.sliderOwner = sliderOwner.transform;
		newHealthSliderInfo.ID = objectID;

		newHealthSliderInfo.useHealthSlideInfoOnScreen = useHealthSlideInfoOnScreenValue;

		GameObject sliderGameObject = Instantiate (sliderPrefab, healtSlidersParent);

		sliderGameObject.name = "Health Bar " + ownerName;

		sliderGameObject.transform.localScale = Vector3.one;
		sliderGameObject.transform.localPosition = Vector3.zero;
		sliderGameObject.transform.localRotation = Quaternion.identity;

		newHealthSliderInfo.sliderGameObject = sliderGameObject;
		newHealthSliderInfo.sliderInfo = sliderGameObject.GetComponent<AIHealtSliderInfo> ();

		newHealthSliderInfo.healthSlider = newHealthSliderInfo.sliderInfo.healthSlider;

		newHealthSliderInfo.useSliderOffset = sliderOffset != Vector3.zero;

		newHealthSliderInfo.sliderOffset = sliderOffset;

		newHealthSliderInfo.healthSlider.maxValue = healthAmount;
		newHealthSliderInfo.healthSlider.value = healthAmount;

		newHealthSliderInfo.shieldSlider = newHealthSliderInfo.sliderInfo.shieldSlider;

		if (newHealthSliderInfo.shieldSlider != null) {
			newHealthSliderInfo.shieldSlider.maxValue = shieldAmount;
			newHealthSliderInfo.shieldSlider.value = shieldAmount;
		}

		newHealthSliderInfo.iconCurrentlyEnabled = true;

		newHealthSliderInfo.sliderOwnerLocated = healthBarSliderActiveOnStart;

		newHealthSliderInfo.sliderRectTransform = sliderGameObject.AddComponent<RectTransform> ();

		healthSliderInfoList.Add (newHealthSliderInfo);

		udpateSliderInfo (objectID, ownerName, textColor, sliderColor);
	}

	public void removeTargetSlider (int objectID)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				Destroy (temporalSliderInfo.sliderGameObject);

				healthSliderInfoList.RemoveAt (i);

				return;
			}
		}
	}

	public void removeElementFromListByPlayer (int objectID)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				Destroy (temporalSliderInfo.sliderGameObject);

				healthBarManager.removeElementFromObjectiveListCalledByPlayer (objectID, playerGameObject);

				healthSliderInfoList.RemoveAt (i);

				return;
			}
		}
	}

	public void udpateSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				if (temporalSliderInfo.sliderInfo.nameText != null) {
					temporalSliderInfo.sliderInfo.nameText.text = newName;
					temporalSliderInfo.sliderInfo.nameText.color = textColor;
				}

				if (temporalSliderInfo.sliderInfo.sliderBackground != null) {
					temporalSliderInfo.sliderInfo.sliderBackground.color = backgroundColor;
				}

				return;
			}
		}
	}

	public void updateSliderAmount (int objectID, float value)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				if (temporalSliderInfo.sliderInfo.healthSlider != null) {
					temporalSliderInfo.sliderInfo.healthSlider.value = value;

					if (useHealthSlideInfoOnScreen) {
						if (currentTargetID == objectID) {
							mainHealtSliderInfoOnScreen.healthSlider.value = value;
						}
					}

					return;
				}
			}
		}
	}


	public void setSliderAmount (int objectID, float sliderValue)
	{
		updateSliderAmount (objectID, sliderValue);
	}

	public void updateShieldSliderAmount (int objectID, float value)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				if (temporalSliderInfo.sliderInfo.shieldSlider != null) {
					temporalSliderInfo.sliderInfo.shieldSlider.value = value;

					if (useHealthSlideInfoOnScreen) {
						if (currentTargetID == objectID) {
							mainHealtSliderInfoOnScreen.shieldSlider.value = value;
						}
					}

					return;
				}
			}
		}
	}

	public void setShieldSliderAmount (int objectID, float sliderValue)
	{
		updateShieldSliderAmount (objectID, sliderValue);
	}

	public void updateSliderMaxValue (int objectID, float newMaxValue)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				if (temporalSliderInfo.sliderInfo.healthSlider != null) {
					temporalSliderInfo.sliderInfo.healthSlider.maxValue = newMaxValue;

					if (useHealthSlideInfoOnScreen) {
						if (currentTargetID == objectID) {
							mainHealtSliderInfoOnScreen.healthSlider.maxValue = newMaxValue;
						}
					}

					return;
				}
			}
		}
	}

	public void updateShieldSliderMaxValue (int objectID, float newMaxValue)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				if (temporalSliderInfo.sliderInfo.shieldSlider != null) {
					temporalSliderInfo.sliderInfo.shieldSlider.maxValue = newMaxValue;

					if (useHealthSlideInfoOnScreen) {
						if (currentTargetID == objectID) {
							mainHealtSliderInfoOnScreen.shieldSlider.maxValue = newMaxValue;
						}
					}

					return;
				}
			}
		}
	}

	public void updateSliderOffset (int objectID, float value)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				temporalSliderInfo.sliderOffset.y = value;
					
				return;
			}
		}
	}

	public void setSliderInfo (int objectID, string newName, Color textColor, Color backgroundColor)
	{
		udpateSliderInfo (objectID, newName, textColor, backgroundColor);
	}

	public void setSliderVisibleState (int objectID, bool state)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				temporalSliderInfo.sliderCanBeShown = state;

				return;
			}
		}
	}

	public void setSliderLocatedState (int objectID, bool state)
	{
		int temporalHealthSliderInfoListCount = healthSliderInfoList.Count;

		for (int i = 0; i < temporalHealthSliderInfoListCount; i++) {
			healthSliderInfo temporalSliderInfo = healthSliderInfoList [i];

			if (temporalSliderInfo.ID == objectID) {
				temporalSliderInfo.sliderOwnerLocated = state;

				return;
			}
		}
	}

	public void pauseOrResumeShowHealthSliders (bool state)
	{
		showSlidersPaused = state;

		healtSlidersParent.gameObject.SetActive (!showSlidersPaused);
	}

	public GameObject getPlayerGameObject ()
	{
		return playerGameObject;
	}

	public void setShowSlidersActiveState (bool state)
	{
		showSlidersActive = state;
	}
}
