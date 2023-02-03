using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using GKC.Localization;

using System.Linq;

public class usingDevicesSystem : MonoBehaviour
{
	public bool canUseDevices;

	public RawImage touchButtonRawImage;
	public Texture originalTouchButtonRawImage;

	public GameObject touchButton;
	public GameObject touchButtonIcon;
	public GameObject iconButton;
	public RectTransform iconButtonRectTransform;

	public Text actionText;
	public Text keyText;
	public Text objectNameText;

	public RawImage objectImage;
	public Text objectDescriptionText;
	public Text objectAmountText;

	public string useDeviceFunctionName = "activateDevice";

	public string extraTextStartActionKey = "[";
	public string extraTextEndActionKey = "]";

	public int defaultDeviceNameFontSize = 17;

	public string setCurrentUserOnDeviceFunctionName = "setCurrentUser";

	public string useDevicesActionName = "Activate Device";

	public List<string> tagsForDevices = new List<string> ();
	public Camera examinateDevicesCamera;
	public bool usePickUpAmountIfEqualToOne;
	public bool usedByAI;

	public bool driving;
	public bool iconButtonCanBeShown = true;
	public GameObject objectToUse;

	public GameObject interactionMessageGameObject;
	public Text interactionMessageText;

	public GameObject currentVehicle;
	public List<deviceInfo> deviceList = new List<deviceInfo> ();

	bool deviceListContainsElements;

	public List<GameObject> deviceGameObjectList = new List<GameObject> ();

	public deviceInfo currentDeviceInfo;

	deviceInfo deviceInfoToCheck;

	public LayerMask layer;
	public float raycastDistance;

	public bool searchingDevicesWithRaycast;
	public bool secondaryDeviceFound;

	public bool showUseDeviceIconEnabled = true;
	public bool useDeviceButtonEnabled = true;
	public bool useFixedDeviceIconPosition;
	public bool deviceOnScreenIfUseFixedIconPosition;

	public bool showInteractionPanelIfObjectNotOnScreen;

	public bool getClosestDeviceToCameraCenter;

	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter = 20;

	public bool currentDeviceIsPickup;

	public bool holdButtonToTakePickupsAround;
	public float holdButtonTime = 0.5f;

	public bool showCurrentDeviceAmount = true;
	public GameObject currentDeviceAmountTextPanel;
	public Text currentDeviceAmountText;

	public bool useDeviceFoundShader;
	public Shader deviceFoundShader;
	public float shaderOutlineWidth;
	public Color shaderOutlineColor;

	public bool examiningObject;

	public GameObject examineObjectRenderTexturePanel;
	public Transform examineObjectBlurPanelParent;

	public Camera examineObjectRenderCamera;

	public bool showDetectedDevicesIconOnScreen;
	public GameObject detectedDevicesIconPrefab;
	public Transform detectedDevicesIconParent;

	public bool useMinDistanceToUseDevices;
	public float minDistanceToUseDevices = 4;

	public bool useInteractionActions;
	public List<interactionActionInfo> interactionActionInfoList = new List<interactionActionInfo> ();

	public bool useIconButtonInfoList;
	public List<iconButtonInfo> iconButtonInfoList = new List<iconButtonInfo> ();
	public string defaultIconButtonName = "Default";
	iconButtonInfo currentIconButtonInfo;
	string currentIconButtonNameToSearch;

	public bool useOnlyDeviceIfVisibleOnCamera;

	public bool disableInteractionTouchButtonIfNotDevicesDetected;
	public bool keepInteractionTouchButtonAlwaysActive;

	public bool useGameObjectListToIgnore;
	public List<GameObject> gameObjectListToIgnore = new List<GameObject> ();

	public playerController playerControllerManager;
	public grabObjects grabObjectsManager;
	public playerInputManager playerInput;
	public Camera mainCamera;
	public playerCamera playerCameraManager;
	public Transform mainCameraTransform;


	public List<multipleInteractionInfo> multipleInteractionInfoList = new List<multipleInteractionInfo> ();

	float lastTimePressedButton;
	bool holdingButton;

	Touch currentTouch;

	deviceStringAction deviceStringManager;

	Vector3 currentIconPosition;

	Coroutine showObjectMessageCoroutine;

	RaycastHit hit;
	deviceStringAction secondaryDeviceStringManager;
	GameObject currentRaycastFoundObject;
	deviceInfo secondaryDeviceInfo;

	Vector3 currentPosition;

	string currentDeviceActionText;
	public string currenDeviceActionName;
	int currentDeviceNameTextFontSize;

	Texture currentDeviceIconTexture;
	string currentDeviceDescription;
	int currentDeviceDescriptionFontSize;

	Vector3 devicePosition;
	Vector3 screenPoint;
	Vector3 centerScreen;
	float currentDistanceToTarget;
	float minDistanceToTarget;
	int currentTargetIndex;
	bool deviceCloseEnoughToScreenCenter;

	float currentAngleWithTarget;

	GameObject objectToRemoveAferStopUse;

	GameObject currentDeviceToUseFound;

	Vector3 originalDeviceIconPosition;

	bool firstPersonActive;
	bool cameraViewChecked;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	Coroutine pauseUseDeviceButtonForDuration;

	bool useCustomMinDistanceToUseDevice;
	float customMinDistanceToUseDevice;

	bool usingDevicePreviously;

	float screenWidth;
	float screenHeight;

	GameObject currentTouchButtonIcon;
	Text currentKetText;
	string currentExtraTextStartActionKey;
	string currentExtraTextEndActionKey;

	bool useMultipleInteractionInfo;

	Text currentActionText;
	Text currentObjectNameText;

	List<Text> currentKeyTextList = new List<Text> ();
	List<Text> currentActionTextList = new List<Text> ();

	RawImage currentObjectImage;
	Text currentObjectDescriptionText;

	int deviceListCount;


	public Collider mainCollider;
	public bool ignoreCheckIfObstacleBetweenDeviceAndPlayer = true;

	bool touchButtonLocated;

	bool usingNewTouchButtonRawImageIcon;

	multipleInteractionSystem currentMultipleInteractionSystem;

	GameObject currentDeviceToUseHoldInteraction;

	public bool holdInteractionButtonEnabled;

	float currentHoldInteractionButtonDuration;

	bool holdingInteractionButtonActive;

	Image currentHoldInteractionSlider;

	Coroutine holdInteractionButtonCoroutine;
	string currentMultipleInteractionName;

	bool checkMultipleInteractionActive;

	bool showHoldInteractionTimer;
	Text holdInteractionTimerText;

	//Editor variables
	public bool showIntereactionSettings;
	public bool showOutlineShaderSettings;
	public bool showEventsSettings;
	public bool showOtherSettings;
	public bool showDebugSettings;
	public bool showAllSettings;
	public bool showComponents;
	public bool showUISettings;

	bool initialCheck;

	bool iconButtonActive;


	void Start ()
	{
		if (iconButton != null) {
			originalDeviceIconPosition = iconButton.transform.position;
		}

		mainCanvasSizeDelta = playerCameraManager.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

		touchButtonLocated = touchButton != null;

		if (mainCamera == null) {
			mainCamera = playerCameraManager.getMainCamera ();
		}
	}

	void FixedUpdate ()
	{
		if (usedByAI) {
			return;
		}

		if (!initialCheck) {
			if (keepInteractionTouchButtonAlwaysActive) {
				if (touchButton.activeSelf != true) {
					touchButton.SetActive (true);
				}
			}

			initialCheck = true;
		}
			
		firstPersonActive = playerCameraManager.isFirstPersonActive ();

		//set the icon button above the device to use, just to indicate to the player that he can activate a device by pressing T
		if (deviceListContainsElements && deviceList.Count > 0 && !examiningObject && !driving) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			
			int index = getclosestDevice (true, true);

			if (iconButtonCanBeShown && index != -1) {
				currentDeviceInfo = deviceList [index];

				//show a secondary device string action inside the current object to use
				if (searchingDevicesWithRaycast) {
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, raycastDistance, layer)) {
						if (currentRaycastFoundObject != hit.collider.gameObject) {
							currentRaycastFoundObject = hit.collider.gameObject;

							secondaryDeviceStringManager = currentRaycastFoundObject.GetComponent<deviceStringAction> ();

							if (secondaryDeviceStringManager != null) {
								secondaryDeviceInfo = new deviceInfo ();
								secondaryDeviceInfo.deviceGameObject = currentRaycastFoundObject;

								if (secondaryDeviceStringManager.useSeparatedTransformForEveryViewEnabled ()) {
									if (firstPersonActive) {
										secondaryDeviceInfo.positionToIcon = secondaryDeviceStringManager.getTransformForIconFirstPerson ();
									} else {
										secondaryDeviceInfo.positionToIcon = secondaryDeviceStringManager.getTransformForIconThirdPerson ();
									}
								} else {
									secondaryDeviceInfo.positionToIcon = secondaryDeviceStringManager.getRegularTransformForIcon ();
								}

								secondaryDeviceInfo.useTransformForStringAction = secondaryDeviceStringManager.useTransformForStringAction;
								secondaryDeviceInfo.actionOffset = secondaryDeviceStringManager.actionOffset;
								secondaryDeviceInfo.useLocalOffset = secondaryDeviceStringManager.useLocalOffset;

								secondaryDeviceInfo.useCustomMinDistanceToUseDevice = secondaryDeviceStringManager.useCustomMinDistanceToUseDevice;
								secondaryDeviceInfo.customMinDistanceToUseDevice = secondaryDeviceStringManager.customMinDistanceToUseDevice;

								secondaryDeviceInfo.useCustomMinAngleToUse = secondaryDeviceStringManager.useCustomMinAngleToUse;
								secondaryDeviceInfo.customMinAngleToUseDevice = secondaryDeviceStringManager.customMinAngleToUseDevice;
								secondaryDeviceInfo.useRelativeDirectionBetweenPlayerAndObject = secondaryDeviceStringManager.useRelativeDirectionBetweenPlayerAndObject;

								secondaryDeviceInfo.ignoreUseOnlyDeviceIfVisibleOnCamera = secondaryDeviceStringManager.ignoreUseOnlyDeviceIfVisibleOnCamera;

								secondaryDeviceInfo.useCustomDeviceTransformPosition = secondaryDeviceStringManager.useCustomDeviceTransformPosition;
								secondaryDeviceInfo.customDeviceTransformPosition = secondaryDeviceStringManager.customDeviceTransformPosition;

								secondaryDeviceInfo.useFixedDeviceIconPosition = secondaryDeviceStringManager.useFixedDeviceIconPosition;

								secondaryDeviceInfo.checkIfObstacleBetweenDeviceAndPlayer = secondaryDeviceStringManager.checkIfObstacleBetweenDeviceAndPlayer;

								string deviceAction = secondaryDeviceStringManager.getDeviceAction ();

								setKeyText (true);

								currentDeviceActionText = deviceAction;

								setActionAndNameText (deviceAction, secondaryDeviceStringManager.getDeviceName ());

								currentDeviceIconTexture = deviceStringManager.iconTexture;
								currentDeviceDescription = deviceStringManager.objectDescription;
								currentDeviceDescriptionFontSize = deviceStringManager.descriptionFontSize;

								setExtraActionInfo (currentDeviceIconTexture, currentDeviceDescription);

								currentDeviceNameTextFontSize = secondaryDeviceStringManager.getTextFontSize ();

								if (currentDeviceNameTextFontSize <= 0 && defaultDeviceNameFontSize > 0) {
									currentDeviceNameTextFontSize = defaultDeviceNameFontSize;
								}

								if (currentDeviceNameTextFontSize > 0) {
									objectNameText.fontSize = currentDeviceNameTextFontSize;
								}

								secondaryDeviceFound = true;
							} else {
								secondaryDeviceFound = false;

								string deviceAction = deviceStringManager.getDeviceAction ();

								currentDeviceActionText = deviceAction;

								setActionAndNameText (deviceAction, deviceStringManager.getDeviceName ());

								currentDeviceIconTexture = deviceStringManager.iconTexture;
								currentDeviceDescription = deviceStringManager.objectDescription;
								currentDeviceDescriptionFontSize = deviceStringManager.descriptionFontSize;

								setExtraActionInfo (currentDeviceIconTexture, currentDeviceDescription);
					
								currentDeviceNameTextFontSize = deviceStringManager.getTextFontSize ();

								if (currentDeviceNameTextFontSize <= 0 && defaultDeviceNameFontSize > 0) {
									currentDeviceNameTextFontSize = defaultDeviceNameFontSize;
								}

								if (currentDeviceNameTextFontSize > 0) {
									objectNameText.fontSize = currentDeviceNameTextFontSize;
								}
							}
						} 
					} else {
						removeSecondaryDeviceInfo ();
					}

					if (secondaryDeviceFound) {
						setIconButtonCanBeShownState (true);

						currentDeviceInfo = secondaryDeviceInfo;
					} else {
						if (!deviceStringManager.showIcon) {
							currentDeviceInfo = null;
						}
						removeSecondaryDeviceInfo ();
					}
				}

				if (showUseDeviceIconEnabled) {
					if (currentDeviceInfo != null) {
						if (useFixedDeviceIconPosition && !deviceOnScreenIfUseFixedIconPosition) {
							enableOrDisableIconButton (true);

							iconButton.transform.position = originalDeviceIconPosition;
						} else {
							//check if the current device to use has two separated positions to show the device icon on third person and firs person
							if (!useFixedDeviceIconPosition && currentDeviceInfo.useSeparatedTransformForEveryView) {
								if (firstPersonActive) {
									if (!cameraViewChecked) {
										setCurrentDeviceStringManagerInfo (currentDeviceInfo);

										cameraViewChecked = true;
									}
								} else {
									if (cameraViewChecked) {
										setCurrentDeviceStringManagerInfo (currentDeviceInfo);

										cameraViewChecked = false;
									}
								}
							}

							if (currentDeviceInfo.positionToIcon != null) {
								currentIconPosition = currentDeviceInfo.positionToIcon.position;

								if (!currentDeviceInfo.useTransformForStringAction) {
									if (currentDeviceInfo.useLocalOffset) {
										currentIconPosition += currentDeviceInfo.positionToIcon.up * currentDeviceInfo.actionOffset;
									} else {
										currentIconPosition += Vector3.up * currentDeviceInfo.actionOffset;
									}
								}

								if (usingScreenSpaceCamera) {
									screenPoint = mainCamera.WorldToViewportPoint (currentIconPosition);
									targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
								} else {
									screenPoint = mainCamera.WorldToScreenPoint (currentIconPosition);
									targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
								}

								if (targetOnScreen) {
									if (useFixedDeviceIconPosition || currentDeviceInfo.useFixedDeviceIconPosition) {

										if (useIconButtonInfoList) {
											if (currentIconButtonInfo != null && currentIconButtonInfo.useFixedPosition) {
												iconButton.transform.position = currentIconButtonInfo.fixedPositionTransform.position;
											} else {
												iconButton.transform.position = originalDeviceIconPosition;
											}
										} else {
											iconButton.transform.position = originalDeviceIconPosition;
										}

									} else {
										bool useDynamicIconPositionResult = true;

										if (useIconButtonInfoList) {
											if (currentIconButtonInfo != null) {
												if (currentIconButtonInfo.useFixedPosition) {
													if (currentIconButtonInfo.fixedPositionTransform != null) {
														iconButton.transform.position = currentIconButtonInfo.fixedPositionTransform.position;
													} else {
														iconButton.transform.position = originalDeviceIconPosition;
													}

													useDynamicIconPositionResult = false;
												}
											}
										}

										if (useDynamicIconPositionResult) {
											if (usingScreenSpaceCamera) {
												iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, 
													(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
										
												iconButtonRectTransform.anchoredPosition = iconPosition2d;
											} else {
												iconButton.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
											}
										}
									}

									enableOrDisableIconButton (true);
								} else {
									if (showInteractionPanelIfObjectNotOnScreen) {
										enableOrDisableIconButton (true);

										iconButton.transform.position = originalDeviceIconPosition;
									} else {
										enableOrDisableIconButton (false);
									}
								}
							} else {
								enableOrDisableIconButton (false);
							}
						}
					} else {
						enableOrDisableIconButton (false);
					}
				}
			} else {
				enableOrDisableIconButton (false);
			}
		} else {
			enableOrDisableIconButton (false);
		}
	}

	public void removeSecondaryDeviceInfo ()
	{
		currentRaycastFoundObject = null;
		secondaryDeviceStringManager = null;
		secondaryDeviceInfo = null;
	}

	public void enableOrDisableIconButton (bool state)
	{
		if (iconButtonActive != state) {
			setIconButtonState (state);
		}
	}

	public void setUseMinDistanceToUseDevicesState (bool state)
	{
		useMinDistanceToUseDevices = state;
	}

	public int getclosestDevice (bool activeDevicesIcons, bool checkForCameraDistance)
	{
		currentTargetIndex = -1;
		minDistanceToTarget = Mathf.Infinity;
		currentPosition = transform.position;

		centerScreen = new Vector3 (screenWidth / 2, screenHeight / 2, 0);

		deviceListCount = deviceList.Count;

		for (int i = 0; i < deviceListCount; i++) {

			deviceInfoToCheck = deviceList [i];

			if (deviceInfoToCheck.deviceGameObject != null) {
				
				devicePosition = deviceInfoToCheck.deviceTransform.position;

				if (getClosestDeviceToCameraCenter && !playerControllerManager.isLockedCameraStateActive ()) {
					screenPoint = mainCamera.WorldToScreenPoint (devicePosition);

					currentDistanceToTarget = GKC_Utils.distance (screenPoint, centerScreen);

					deviceCloseEnoughToScreenCenter = false;

					if (useMaxDistanceToCameraCenter) {
						if (currentDistanceToTarget < maxDistanceToCameraCenter) {
							deviceCloseEnoughToScreenCenter = true;
						}
					} else {
						deviceCloseEnoughToScreenCenter = true;
					}

					if (deviceCloseEnoughToScreenCenter) {
						if (currentDistanceToTarget < minDistanceToTarget) {
							minDistanceToTarget = currentDistanceToTarget;
							currentTargetIndex = i;
						}
					}
				} else {
					currentDistanceToTarget = GKC_Utils.distance (devicePosition, currentPosition);

					if (currentDistanceToTarget < minDistanceToTarget) {
						minDistanceToTarget = currentDistanceToTarget;
						currentTargetIndex = i;
					}
				}
					
			} else {
				if (showDetectedDevicesIconOnScreen) {
					if (deviceInfoToCheck.deviceIcon != null) {
						Destroy (deviceInfoToCheck.deviceIcon);
					}
				}

				deviceList.RemoveAt (i);

				deviceGameObjectList.RemoveAt (i);

				i = 0;

				deviceListCount = deviceList.Count;

				if (deviceListCount == 0) {
					deviceListContainsElements = false;
				}
			}
		}

		bool removeCurrentTargetIndex = false;

		if (!ignoreCheckIfObstacleBetweenDeviceAndPlayer) {
			if (currentTargetIndex > -1 && deviceListCount > currentTargetIndex) {
				deviceInfo temporalDeviceInfo = deviceList [currentTargetIndex];

				if (temporalDeviceInfo.checkIfObstacleBetweenDeviceAndPlayer) {

					Vector3 targetPosition = currentPosition + transform.up;

					Vector3 devicePosition = temporalDeviceInfo.deviceTransform.position;

					//for every enemy in front of the camera, use a raycast, if it finds an obstacle between the enemy and the camera, the enemy is removed from the list
					Vector3 direction = targetPosition - devicePosition;

					direction = direction / direction.magnitude;

					if (Physics.Raycast (devicePosition, direction, out hit, raycastDistance, layer)) {

						if (hit.collider != mainCollider) {
							removeCurrentTargetIndex = true;

							currentTargetIndex = -1;
						}
					}
				}
			}
		}

		if (currentTargetIndex > -1 && deviceListCount > currentTargetIndex) {

			if (checkForCameraDistance) {
				if (useMinDistanceToUseDevices) {
					deviceInfo temporalDeviceInfo = deviceList [currentTargetIndex];

					currentDistanceToTarget = GKC_Utils.distance (temporalDeviceInfo.deviceTransform.position, currentPosition);

					useCustomMinDistanceToUseDevice = temporalDeviceInfo.useCustomMinDistanceToUseDevice;
					customMinDistanceToUseDevice = temporalDeviceInfo.customMinDistanceToUseDevice;

					if ((!useCustomMinDistanceToUseDevice && currentDistanceToTarget > minDistanceToUseDevices) || (useCustomMinDistanceToUseDevice && currentDistanceToTarget > customMinDistanceToUseDevice)) {
						removeCurrentTargetIndex = true;

						currentTargetIndex = -1;
					}
				}

				if (useOnlyDeviceIfVisibleOnCamera && currentTargetIndex > -1 && !playerControllerManager.isUsingDevice ()) {
					deviceInfo temporalDeviceInfo = deviceList [currentTargetIndex];

					if (!temporalDeviceInfo.ignoreUseOnlyDeviceIfVisibleOnCamera) {

						if (temporalDeviceInfo.useCustomDeviceTransformPosition) {
							devicePosition = temporalDeviceInfo.customDeviceTransformPosition.position;
						} else {
							devicePosition = temporalDeviceInfo.deviceTransform.position;
						}
	
						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (devicePosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (devicePosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
						}

						if (!targetOnScreen) {
							removeCurrentTargetIndex = true;

							currentTargetIndex = -1;
						}
					}
				}
			}

			if (currentTargetIndex > -1) {
				deviceInfo temporalDeviceInfo = deviceList [currentTargetIndex];

				if (temporalDeviceInfo.useCustomMinAngleToUse) {
					Vector3 directionToCheck = temporalDeviceInfo.deviceTransform.forward;

					if (temporalDeviceInfo.useRelativeDirectionBetweenPlayerAndObject) {
						directionToCheck = temporalDeviceInfo.deviceTransform.position - transform.position;

						directionToCheck = directionToCheck / directionToCheck.magnitude;
					}

					currentAngleWithTarget = Vector3.SignedAngle (transform.forward, directionToCheck, transform.up);
				
					if (Math.Abs (currentAngleWithTarget) > temporalDeviceInfo.customMinAngleToUseDevice) {
						removeCurrentTargetIndex = true;

						currentTargetIndex = -1;
					}
				}
			}
		}

		if (showDetectedDevicesIconOnScreen && activeDevicesIcons) {
			for (int i = 0; i < deviceListCount; i++) {
				deviceInfoToCheck = deviceList [i];

				if (deviceInfoToCheck.deviceGameObject != null) {
					if (i != currentTargetIndex) {
						if (deviceInfoToCheck.positionToIcon != null) {
							devicePosition = deviceInfoToCheck.positionToIcon.position;
						} else {
							devicePosition = deviceInfoToCheck.deviceTransform.position;
						}
					 
						//Show icons on the screen for every device found
						targetOnScreen = false;

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (devicePosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (devicePosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
						}

						if (targetOnScreen) {
							if (!deviceInfoToCheck.deviceIcon.activeSelf) {
								deviceInfoToCheck.deviceIcon.SetActive (true);
							}

							if (usingScreenSpaceCamera) {
								iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

								deviceInfoToCheck.deviceIconRectTransform.anchoredPosition = iconPosition2d;
							} else {
								deviceInfoToCheck.deviceIconRectTransform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
							}
						} else {
							if (deviceInfoToCheck.deviceIcon.activeSelf) {
								deviceInfoToCheck.deviceIcon.SetActive (false);
							}
						}
					} else {
						if (deviceInfoToCheck.deviceIcon.activeSelf) {
							deviceInfoToCheck.deviceIcon.SetActive (false);
						}
					}
				}
			}
		}
	
		if (removeCurrentTargetIndex) {
			if (objectToUse != null) {
				checkIfSetOriginalShaderToPreviousDeviceFound (objectToUse);
			}

			objectToUse = null;

			return -1;
		}

		if (getClosestDeviceToCameraCenter && useMaxDistanceToCameraCenter) {
			if (currentTargetIndex == -1 && objectToUse != null) {

				checkIfSetOriginalShaderToPreviousDeviceFound (objectToUse);

				objectToUse = null;

				return -1;
			}
		}

		if (currentTargetIndex != -1) {
			deviceInfo temporalDeviceInfo = deviceList [currentTargetIndex];

			if (objectToUse != temporalDeviceInfo.deviceGameObject) {

				if (objectToUse != null) {
					checkIfSetOriginalShaderToPreviousDeviceFound (objectToUse);
				}
					
				objectToUse = temporalDeviceInfo.deviceGameObject;

				setCurrentDeviceStringManagerInfo (temporalDeviceInfo);

				removeSecondaryDeviceInfo ();

				checkIfSetNewShaderToDeviceFound (objectToUse);
			}
		}
			
		return currentTargetIndex;
	}

	public void setCurrentDeviceStringManagerInfo (deviceInfo deviceToUseInfo)
	{
		//get the action made by the current device
		string deviceAction = "";

		deviceStringManager = objectToUse.GetComponent<deviceStringAction> ();

		if (deviceStringManager != null) {
			deviceToUseInfo.useSeparatedTransformForEveryView = deviceStringManager.useSeparatedTransformForEveryViewEnabled ();

			if (deviceToUseInfo.useSeparatedTransformForEveryView) {
				if (firstPersonActive) {
					deviceToUseInfo.positionToIcon = deviceStringManager.getTransformForIconFirstPerson ();
				} else {
					deviceToUseInfo.positionToIcon = deviceStringManager.getTransformForIconThirdPerson ();
				}
			} else {
				deviceToUseInfo.positionToIcon = deviceStringManager.getRegularTransformForIcon ();
			}

			deviceToUseInfo.useTransformForStringAction = deviceStringManager.useTransformForStringAction;
			deviceToUseInfo.actionOffset = deviceStringManager.actionOffset;
			deviceToUseInfo.useLocalOffset = deviceStringManager.useLocalOffset;

			deviceToUseInfo.useCustomMinDistanceToUseDevice = deviceStringManager.useCustomMinDistanceToUseDevice;
			deviceToUseInfo.customMinDistanceToUseDevice = deviceStringManager.customMinDistanceToUseDevice;

			deviceToUseInfo.useCustomMinAngleToUse = deviceStringManager.useCustomMinAngleToUse;
			deviceToUseInfo.customMinAngleToUseDevice = deviceStringManager.customMinAngleToUseDevice;
			deviceToUseInfo.useRelativeDirectionBetweenPlayerAndObject = deviceStringManager.useRelativeDirectionBetweenPlayerAndObject;

			deviceToUseInfo.ignoreUseOnlyDeviceIfVisibleOnCamera = deviceStringManager.ignoreUseOnlyDeviceIfVisibleOnCamera;

			deviceToUseInfo.useCustomDeviceTransformPosition = deviceStringManager.useCustomDeviceTransformPosition;
			deviceToUseInfo.customDeviceTransformPosition = deviceStringManager.customDeviceTransformPosition;

			deviceToUseInfo.useFixedDeviceIconPosition = deviceStringManager.useFixedDeviceIconPosition;

			deviceAction = deviceStringManager.getDeviceAction ();

			deviceToUseInfo.checkIfObstacleBetweenDeviceAndPlayer = deviceStringManager.checkIfObstacleBetweenDeviceAndPlayer;
		}

		if (deviceAction == null) {
			deviceAction = "";
		}

		if (deviceStringManager != null) {
			//show the icon in the hud of the screen according to the deviceStringAction component
			if ((deviceStringManager.showIcon && deviceAction.Length > 0 && deviceStringManager.iconEnabled) || searchingDevicesWithRaycast) {
				setIconButtonCanBeShownState (true);
			} else {
				setIconButtonCanBeShownState (false);
			}

			//enable the interection button in the touch screen
			if (deviceStringManager.showTouchIconButton) {
				enableOrDisableTouchButton (true);
			} else {
				enableOrDisableTouchButton (false);
			}
		} else {
			setIconButtonCanBeShownState (false);

			enableOrDisableTouchButton (false);
		}

		if (useIconButtonInfoList) {
			bool customIconButtonInfoFound = false;

			if (deviceStringManager != null) {
				if (deviceStringManager.useCustomIconButtonInfo) {
					currentIconButtonNameToSearch = deviceStringManager.customIconButtonInfoName;
				} else {
					currentIconButtonNameToSearch = defaultIconButtonName;
				}
			} else {
				currentIconButtonNameToSearch = defaultIconButtonName;
			}

			for (int i = 0; i < iconButtonInfoList.Count; i++) {
				if (iconButtonInfoList [i].Name.Equals (currentIconButtonNameToSearch)) {
					currentIconButtonInfo = iconButtonInfoList [i];

					customIconButtonInfoFound = true;
					
				} else {
					if (iconButtonInfoList [i].iconButtonPanel.activeSelf) {
						iconButtonInfoList [i].iconButtonPanel.SetActive (false);
					}
				}
			}

			if (!customIconButtonInfoFound) {
				currentIconButtonInfo = null;
			}
		}

		setKeyText (true);

		if (actionText != null) {
			currentDeviceActionText = deviceAction;

			if (deviceStringManager != null) {
				currenDeviceActionName = deviceStringManager.getDeviceName ();

				currentDeviceNameTextFontSize = deviceStringManager.getTextFontSize ();

				currentDeviceIconTexture = deviceStringManager.iconTexture;
				currentDeviceDescription = deviceStringManager.objectDescription;
				currentDeviceDescriptionFontSize = deviceStringManager.descriptionFontSize;
			}

			setInteractionButtonName ();

			updateCurrentIconButtonInfo (true);
		}
	}

	public void checkIfSetNewShaderToDeviceFound (GameObject objectToCheck)
	{
		currentDeviceToUseFound = objectToCheck;

		if (useDeviceFoundShader) {
			//print ("new on device" + currenDeviceActionName);

			outlineObjectSystem currentOutlineObjectSystem = objectToCheck.GetComponent<outlineObjectSystem> ();

			if (currentOutlineObjectSystem != null) {
				currentOutlineObjectSystem.setOutlineState (true, deviceFoundShader, shaderOutlineWidth, shaderOutlineColor, playerControllerManager);
			}
		}
	}

	public void checkIfSetOriginalShaderToPreviousDeviceFound (GameObject objectToCheck)
	{
		if (useDeviceFoundShader) {
//			print ("original on device" + objectToCheck.name);

			outlineObjectSystem currentOutlineObjectSystem = objectToCheck.GetComponent<outlineObjectSystem> ();

			if (currentOutlineObjectSystem != null) {
				if (!grabObjectsManager.useObjectToGrabFoundShader || !grabObjectsManager.isCurrentObjectToGrabFound (currentOutlineObjectSystem.getMeshParent ())) {
					currentOutlineObjectSystem.setOutlineState (false, null, 0, Color.white, playerControllerManager);
				}
			}
		}

		currentDeviceToUseFound = null;
	}

	public bool isCurrentDeviceToUseFound (GameObject objectToCheck)
	{
		if (currentDeviceToUseFound == objectToCheck) {
			return true;
		}

		return false;
	}

	public void setInteractionButtonName ()
	{
		if (objectToUse == null) {
			return;
		}

		if (objectNameText != null) {
			setActionAndNameText (currentDeviceActionText, currenDeviceActionName);

			setExtraActionInfo (currentDeviceIconTexture, currentDeviceDescription);

			int currentAmount = 0;

			pickUpObject currentPickUpObject = objectToUse.GetComponentInParent<pickUpObject> ();

			if (currentPickUpObject != null) {
				currentAmount = currentPickUpObject.amount;

				currentDeviceIsPickup = true;
			} else {
				currentDeviceIsPickup = false;
			}

			if (currentDeviceIsPickup && showCurrentDeviceAmount) {
				if (currentAmount > 1 || (usePickUpAmountIfEqualToOne && currentAmount == 1)) {
					if (!currentDeviceAmountTextPanel.activeSelf) {
						currentDeviceAmountTextPanel.SetActive (true);
					}

					if (currentDeviceIsPickup) {
						currentDeviceAmountText.text = "x " + currentAmount;
					}
				} else {
					if (currentDeviceAmountTextPanel.activeSelf) {
						currentDeviceAmountTextPanel.SetActive (false);
					}
				}
			} else {
				if (currentDeviceAmountTextPanel.activeSelf) {
					currentDeviceAmountTextPanel.SetActive (false);
				}
			}

			if (currentDeviceNameTextFontSize <= 0 && defaultDeviceNameFontSize > 0) {
				currentDeviceNameTextFontSize = defaultDeviceNameFontSize;
			}

			if (currentDeviceNameTextFontSize > 0) {
				objectNameText.fontSize = currentDeviceNameTextFontSize;
			}
		}
	}

	//check if the player enters or exits the trigger of a device

	void OnTriggerEnter (Collider col)
	{
		if (playerControllerManager.isUsingDevice ()) {
			return;
		}

		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (!isInTagsForDevicesList (col.gameObject)) {
			return;
		}

		if (useGameObjectListToIgnore) {
			if (gameObjectListToIgnore.Contains (col.gameObject)) {
				return;
			}
		}

		if (driving) {
			return;
		}
			
		if (isEnter) {
			//if the player is driving, he can't use any other device
			if (!canUseDevices || driving) {
				return;
			}

			if (usedByAI) {
				return;
			}

			GameObject usableObjectFound = col.gameObject;

			if (!existInDeviceList (usableObjectFound)) {
				addDeviceToList (usableObjectFound);

				deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();

				if (deviceStringManager != null) {
					if (deviceStringManager.useRaycastToDetectDeviceParts) {
						searchingDevicesWithRaycast = true;
					}
				} else {
					print ("WARNING: Device String Action component hasn't been configured on " + usableObjectFound.name);
				}
			}
		} else {
			//when the player exits from the trigger of a device, if he is not driving, set the device to null
			//else the player is driving, so the current device is that vehicle, so the device can't be changed
			GameObject usableObjectFound = col.gameObject;

			if (existInDeviceList (usableObjectFound)) {

				deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();

				if (deviceStringManager != null) {
					if (deviceStringManager.useRaycastToDetectDeviceParts) {
						searchingDevicesWithRaycast = false;
					} 
				}

				if (driving) {
					if (usableObjectFound != currentVehicle) {
						removeDeviceFromList (usableObjectFound);
					}
				} else {
					removeDeviceFromList (usableObjectFound);
				}

				checkIfSetOriginalShaderToPreviousDeviceFound (usableObjectFound);
			}

			if (!driving) {
				enableOrDisableTouchButton (false);
			}

			if (deviceList.Count == 0) {
				objectToUse = null;

				setIconButtonCanBeShownState (false);

				currentDeviceInfo = null;

				deviceListContainsElements = false;
			}
		}
	}

	public void addDeviceToList (GameObject deviceToAdd)
	{
		if (useGameObjectListToIgnore) {
			if (gameObjectListToIgnore.Contains (deviceToAdd)) {
				return;
			}
		}

		deviceInfo newDeviceInfo = new deviceInfo ();
		newDeviceInfo.deviceGameObject = deviceToAdd;

		if (newDeviceInfo.deviceGameObject != null) {
			newDeviceInfo.deviceTransform = newDeviceInfo.deviceGameObject.transform;
		}

		if (showDetectedDevicesIconOnScreen) {
			GameObject newScreenIcon = (GameObject)Instantiate (detectedDevicesIconPrefab, Vector3.zero, Quaternion.identity, detectedDevicesIconParent);

			newScreenIcon.transform.localScale = Vector3.one;
			newScreenIcon.transform.localPosition = Vector3.zero;

			newDeviceInfo.deviceIcon = newScreenIcon;
			newDeviceInfo.deviceIconRectTransform = newScreenIcon.GetComponent<RectTransform> ();
		}

		deviceStringAction newDeviceStringAction = deviceToAdd.GetComponent<deviceStringAction> ();

		if (newDeviceStringAction != null) {
			newDeviceInfo.useSeparatedTransformForEveryView = newDeviceStringAction.useSeparatedTransformForEveryViewEnabled ();

			if (newDeviceInfo.useSeparatedTransformForEveryView) {
				if (firstPersonActive) {
					newDeviceInfo.positionToIcon = newDeviceStringAction.getTransformForIconFirstPerson ();
				} else {
					newDeviceInfo.positionToIcon = newDeviceStringAction.getTransformForIconThirdPerson ();
				}
			} else {
				newDeviceInfo.positionToIcon = newDeviceStringAction.getRegularTransformForIcon ();
			}

			newDeviceInfo.useTransformForStringAction = newDeviceStringAction.useTransformForStringAction;
			newDeviceInfo.actionOffset = newDeviceStringAction.actionOffset;
			newDeviceInfo.useLocalOffset = newDeviceStringAction.useLocalOffset;

			newDeviceInfo.useCustomMinDistanceToUseDevice = newDeviceStringAction.useCustomMinDistanceToUseDevice;
			newDeviceInfo.customMinDistanceToUseDevice = newDeviceStringAction.customMinDistanceToUseDevice;

			newDeviceInfo.useCustomMinAngleToUse = newDeviceStringAction.useCustomMinAngleToUse;
			newDeviceInfo.customMinAngleToUseDevice = newDeviceStringAction.customMinAngleToUseDevice;
			newDeviceInfo.useRelativeDirectionBetweenPlayerAndObject = newDeviceStringAction.useRelativeDirectionBetweenPlayerAndObject;

			newDeviceInfo.ignoreUseOnlyDeviceIfVisibleOnCamera = newDeviceStringAction.ignoreUseOnlyDeviceIfVisibleOnCamera;

			newDeviceInfo.useCustomDeviceTransformPosition = newDeviceStringAction.useCustomDeviceTransformPosition;
			newDeviceInfo.customDeviceTransformPosition = newDeviceStringAction.customDeviceTransformPosition;

			newDeviceInfo.useFixedDeviceIconPosition = newDeviceStringAction.useFixedDeviceIconPosition;

			newDeviceInfo.checkIfObstacleBetweenDeviceAndPlayer = newDeviceStringAction.checkIfObstacleBetweenDeviceAndPlayer;
		}

		deviceList.Add (newDeviceInfo);

		deviceGameObjectList.Add (deviceToAdd);

		deviceListContainsElements = true;
	}

	//call the device action
	public void useDevice ()
	{
		if (!playerControllerManager.canPlayerRagdollMove ()) {
			return;
		}
			
		if (playerControllerManager.isPlayerMenuActive () && !playerControllerManager.isUsingDevice ()) {
			return;
		}
			
		GameObject usableObjectFound = objectToUse;

		if (secondaryDeviceInfo != null) {
			usableObjectFound = secondaryDeviceInfo.deviceGameObject;
		}
			
		if (usableObjectFound != null && canUseDevices) {

			if (holdInteractionButtonEnabled) {
				if (!holdingInteractionButtonActive) {
					if (deviceStringManager != null) {
						if (deviceStringManager.useHoldInteractionButton) {
							holdingInteractionButtonActive = true;

							currentHoldInteractionButtonDuration = deviceStringManager.holdInteractionButtonDuration;

							currentDeviceToUseHoldInteraction = usableObjectFound;

							if (currentIconButtonInfo != null) {
								currentHoldInteractionSlider = currentIconButtonInfo.holdInteractionSlider;

								showHoldInteractionTimer = currentIconButtonInfo.showHoldInteractionTimer;
								holdInteractionTimerText = currentIconButtonInfo.holdInteractionTimerText;
							}

							stopHoldInteractionButtonCoroutine ();

							holdInteractionButtonCoroutine = StartCoroutine (updateHoldInteractionButtonCoroutine ());

							return;
						}
					}
				}
			}

			if (checkMultipleInteractionActive) {
				currentMultipleInteractionSystem = usableObjectFound.GetComponent<multipleInteractionSystem> ();

				bool checkMultipleInteractionResult = true;

				if (currentMultipleInteractionSystem == null) {
					checkMultipleInteractionResult = false;
				} 
					
				if (deviceStringManager == null) {
					checkMultipleInteractionResult = false;
				} else {
					if (!deviceStringManager.useMultipleInteraction) {
						checkMultipleInteractionResult = false;	
					}
				}

				if (checkMultipleInteractionResult) {
					for (int i = 0; i < multipleInteractionInfoList.Count; i++) {
						if (multipleInteractionInfoList [i].inputName.Equals (currentMultipleInteractionName)) {
							bool interactionTypeLocated = 
								multipleInteractionInfoList [i].multipleInteractionNameList.Any (x => deviceStringManager.multipleInteractionNameList.Contains (x));

							if (interactionTypeLocated) {
								currentMultipleInteractionSystem.activateInteraction (multipleInteractionInfoList [i].multipleInteractionNameList, gameObject);
							}
						}
					}
				} else {
					return;
				}
			}

			vehicleHUDManager currentVehicleHUDManager = usableObjectFound.GetComponent<vehicleHUDManager> ();

			if (currentVehicleHUDManager != null) {
				setCurrentVehicleGameObject (usableObjectFound);

				currentVehicleHUDManager.setCurrentPassenger (gameObject);
			}

			usableObjectFound.SendMessage (setCurrentUserOnDeviceFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);

			usableObjectFound.SendMessage (useDeviceFunctionName, SendMessageOptions.DontRequireReceiver);

			if (playerControllerManager.isUsingDevice ()) {
				checkIfSetOriginalShaderToPreviousDeviceFound (usableObjectFound);

				usingDevicePreviously = true;
			} else {
				if (existInDeviceList (usableObjectFound)) {
					//print (usableObjectFound.name + " in device list");
					checkIfSetNewShaderToDeviceFound (usableObjectFound);
				} else {
					checkIfSetOriginalShaderToPreviousDeviceFound (usableObjectFound);
					//print (usableObjectFound.name + " not in device list");
				}

				if (usingDevicePreviously) {
//					print ("previoulsy using device, change player collider state to search close devices to use");

					bool isMainColliderEnabled = playerControllerManager.isMainColliderEnabled ();

					playerControllerManager.setMainColliderState (false);
					playerControllerManager.setMainColliderState (true);

					playerControllerManager.setMainColliderState (isMainColliderEnabled);
				}

				usingDevicePreviously = false;
			}

			if (deviceStringManager != null) {
				if (deviceStringManager.setUsingDeviceState) {
					deviceStringManager.checkSetUsingDeviceState ();
				}

				if (deviceStringManager.usingDevice) {
					if (deviceStringManager.hideIconOnUseDevice) {
						setIconButtonCanBeShownState (false);
					}
				} else {
					if (deviceStringManager.showIconOnStopUseDevice) {
						setIconButtonCanBeShownState (true);
					}
				}
			}

			checkIfRemoveDeviceFromList ();

			if (!usableObjectFound) {
				return;
			}
		
			if (usableObjectFound.GetComponent<useInventoryObject> ()) {
				return;
			}

			if (secondaryDeviceInfo == null) {
				deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();
			}

			//if the device is a turret or a chest, disable the icon
			if (deviceStringManager != null) {
				if (deviceStringManager.disableIconOnPress) {
					OnTriggerExit (usableObjectFound.GetComponent<Collider> ());
					return;
				}

				if (deviceStringManager.hideIconOnPress) {
					setIconButtonCanBeShownState (false);
				}

				if (deviceStringManager.reloadDeviceActionOnPress) {
					checkDeviceName ();
				}
			}

			setInteractionButtonName ();
		}
	}

	public void useCurrentDevice (GameObject currentObjectToUse)
	{
		//print ("Device To Use " + currentObjectToUse.name);
		objectToUse = currentObjectToUse;

		useDevice ();
	}

	public void setObjectToRemoveAferStopUse (GameObject objectToRemove)
	{
		objectToRemoveAferStopUse = objectToRemove;
	}

	public void checkIfRemoveDeviceFromList ()
	{
		if (!playerControllerManager.isUsingDevice ()) {
			if (objectToUse != null && objectToRemoveAferStopUse != null && objectToUse == objectToRemoveAferStopUse) {
				
				removeDeviceFromList (objectToRemoveAferStopUse);

				objectToRemoveAferStopUse = null;

				getclosestDevice (false, false);
			}
		}
	}

	public void takePickupsAround ()
	{
		if (!playerControllerManager.canPlayerRagdollMove () || !canUseDevices) {
			return;
		}

		List<GameObject> currentDeviceGameObjectList = new List<GameObject> (deviceGameObjectList);

		for (int i = 0; i < currentDeviceGameObjectList.Count; i++) {
			GameObject usableObjectFound = currentDeviceGameObjectList [i];

			if (usableObjectFound != null) {
				
				pickUpObject currentPickUpObject = usableObjectFound.GetComponentInParent<pickUpObject> ();

				if (currentPickUpObject != null) {
					bool pickObjectResult = true;

					if (holdingInteractionButtonActive) {
						
						deviceStringAction currentDeviceStringAction = currentPickUpObject.GetComponentInChildren<deviceStringAction> ();

						if (currentDeviceStringAction != null) {
							if (currentDeviceStringAction.useHoldInteractionButton) {
								pickObjectResult = false;
							}
						}
					}

					if (pickObjectResult) {
						bool originalIgnoreExamineObjectBeforeStoreEnabled = currentPickUpObject.ignoreExamineObjectBeforeStoreEnabled;
						
						currentPickUpObject.ignoreExamineObjectBeforeStoreEnabled = true;

						usableObjectFound.SendMessage (useDeviceFunctionName, SendMessageOptions.DontRequireReceiver);

						currentPickUpObject.ignoreExamineObjectBeforeStoreEnabled = originalIgnoreExamineObjectBeforeStoreEnabled;
					}
				}
			}
		}

		getclosestDevice (false, false);
	}

	public void updateClosestDeviceList ()
	{
		getclosestDevice (false, false);
	}

	public void enableOrDisableTouchButton (bool state)
	{
		if (keepInteractionTouchButtonAlwaysActive) {
			return;
		}

		if (touchButtonLocated) {
			if (touchButton.activeSelf != state) {
				touchButton.SetActive (state);
			}
		}
	}

	//disable the icon showed when the player is inside a device's trigger
	public void disableIcon ()
	{
		enableOrDisableTouchButton (false);

		setIconButtonCanBeShownState (true);

		driving = false;

		objectToUse = null;
	}

	public void setIconButtonCanBeShownState (bool state)
	{
		iconButtonCanBeShown = state;
	}

	void updateCurrentIconButtonInfo (bool state)
	{
		if (useIconButtonInfoList && currentIconButtonInfo != null) {

			if (currentIconButtonInfo.iconButtonPanel.activeSelf != state) {
				currentIconButtonInfo.iconButtonPanel.SetActive (state);
			}
		}
	}

	public void setIconButtonState (bool state)
	{
		updateCurrentIconButtonInfo (state);

		if (iconButton.activeSelf != state) {
			iconButton.SetActive (state);
		}

		if (disableInteractionTouchButtonIfNotDevicesDetected) {
			enableOrDisableTouchButton (state);
		}

		iconButtonActive = state;

		if (!state) {
			if (showDetectedDevicesIconOnScreen) {
				for (int i = 0; i < deviceList.Count; i++) {
					if (deviceList [i] != null) {
						if (deviceList [i].deviceIcon.activeSelf) {
							deviceList [i].deviceIcon.SetActive (false);
						}
					}
				}
			}
		}
	}

	public bool getCurrentIconButtonState ()
	{
		return iconButtonActive;
	}

	public void removeVehicleFromList ()
	{
		if (currentVehicle != null) {
			removeDeviceFromList (currentVehicle);
		}
	}

	public void setDrivingState (bool state)
	{
		//if the player is driving, and he is inside the trigger of other vehicle, disable the icon to use the other vehicle
		driving = state;

		setIconButtonCanBeShownState (!state);
	}

	public void setCurrentVehicle (GameObject vehicle)
	{
		setCurrentVehicleGameObject (vehicle);
	}

	public void removeCurrentVehicle (GameObject vehicle)
	{
		if (driving) {
			return;
		}

		if (vehicle == currentVehicle) {
			setCurrentVehicleGameObject (null);
		}
	}

	void setCurrentVehicleGameObject (GameObject newVehicle)
	{
		currentVehicle = newVehicle;
	}

	public void clearDeviceList ()
	{
		if (showDetectedDevicesIconOnScreen) {
			for (int i = 0; i < deviceList.Count; i++) {
				if (deviceList [i].deviceIcon != null) {
					Destroy (deviceList [i].deviceIcon);
				}
			}
		}

		deviceList.Clear ();

		deviceGameObjectList.Clear ();

		deviceListContainsElements = false;
	}

	public void clearDeviceListButOne (GameObject objectToKeep)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject != objectToKeep) {

				if (showDetectedDevicesIconOnScreen) {
					if (deviceList [i].deviceIcon != null) {
						Destroy (deviceList [i].deviceIcon);
					}
				}

				if (holdingInteractionButtonActive) {
					if (deviceList [i].deviceGameObject == currentDeviceToUseHoldInteraction) {
						stopHoldInteractionButtonCoroutine ();

						holdingInteractionButtonActive = false;
					}
				}

				deviceList.RemoveAt (i);

				deviceGameObjectList.RemoveAt (i);

				i = 0;
			}
		}

		if (deviceList.Count == 0) {
			deviceListContainsElements = false;
		}
	}

	public void checkDeviceName ()
	{
		GameObject usableObjectFound = objectToUse;

		if (secondaryDeviceInfo != null) {
			usableObjectFound = secondaryDeviceInfo.deviceGameObject;
		}

		if (usableObjectFound != null) {
			deviceStringManager = usableObjectFound.GetComponent<deviceStringAction> ();

			if (deviceStringManager != null) {
				string deviceAction = "";

				deviceAction = deviceStringManager.getDeviceAction ();

				setKeyText (true);

				currentDeviceActionText = deviceAction;

				setActionAndNameText (deviceAction, deviceStringManager.getDeviceName ());

				currentDeviceIconTexture = deviceStringManager.iconTexture;
				currentDeviceDescription = deviceStringManager.objectDescription;
				currentDeviceDescriptionFontSize = deviceStringManager.descriptionFontSize;

				setExtraActionInfo (currentDeviceIconTexture, currentDeviceDescription);

				currentDeviceNameTextFontSize = deviceStringManager.getTextFontSize ();

				if (currentDeviceNameTextFontSize <= 0 && defaultDeviceNameFontSize > 0) {
					currentDeviceNameTextFontSize = defaultDeviceNameFontSize;
				}

				if (currentDeviceNameTextFontSize > 0) {
					objectNameText.fontSize = currentDeviceNameTextFontSize;
				}
			}
		}
	}

	public void reloadDeviceStringActionOnPlayer (GameObject objectToCheck)
	{
		if (objectToUse != null && objectToUse == objectToCheck) {
			checkDeviceName ();
		}
	}

	public bool existInDeviceList (GameObject objecToCheck)
	{
//		for (int i = 0; i < deviceList.Count; i++) {
//			if (deviceList [i].deviceGameObject == objecToCheck) {
//				return true;
//			}
//		}
		if (objecToCheck != null) {
			if (deviceGameObjectList.Contains (objecToCheck)) {
				return true;
			}
		}

		return false;
	}

	public void removeDeviceFromList (GameObject objectToRemove)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceGameObject == objectToRemove) {

				if (showDetectedDevicesIconOnScreen) {
					if (deviceList [i].deviceIcon != null) {
						Destroy (deviceList [i].deviceIcon);
					}
				}

				deviceList.RemoveAt (i);

				deviceGameObjectList.RemoveAt (i);

				if (holdingInteractionButtonActive) {
					if (objectToRemove == currentDeviceToUseHoldInteraction) {
						stopHoldInteractionButtonCoroutine ();

						holdingInteractionButtonActive = false;
					}
				}
			}
		}
	
		if (deviceList.Count == 0) {
			objectToUse = null;
			currentDeviceInfo = null;

			deviceListContainsElements = false;
		}
	}

	public void removeDeviceFromListExternalCall (GameObject objectToRemove)
	{
		StartCoroutine (removeDeviceFromListExternalCallCoroutine (objectToRemove));
	}

	IEnumerator removeDeviceFromListExternalCallCoroutine (GameObject objectToRemove)
	{
		yield return new WaitForSeconds (0.01f);

		removeDeviceFromList (objectToRemove);

		checkIfSetOriginalShaderToPreviousDeviceFound (objectToRemove);
	}

	public void removeDeviceFromListUsingParent (GameObject objectToRemove)
	{
		for (int i = 0; i < deviceList.Count; i++) {
			if (deviceList [i].deviceTransform.IsChildOf (objectToRemove.transform)) {
				removeDeviceFromList (deviceList [i].deviceGameObject);
			}
		}
	}

	public bool isInTagsForDevicesList (GameObject objectToCheck)
	{
		if (tagsForDevices.Contains (objectToCheck.tag)) {
			return true;
		}

		return false;
	}

	public void setExamineteDevicesCameraState (bool state, bool useBlurUIPanel)
	{
		examinateDevicesCamera.enabled = state;

		if (useBlurUIPanel) {
			examineObjectRenderCamera.enabled = state;
		}

		examiningObject = state;
	}

	public Camera getExaminateDevicesCamera ()
	{
		return examinateDevicesCamera;
	}

	public bool isExaminingObject ()
	{
		return examiningObject;
	}

	public GameObject getExamineObjectRenderTexturePanel ()
	{
		return examineObjectRenderTexturePanel;
	}

	public Transform getExamineObjectBlurPanelParent ()
	{
		return examineObjectBlurPanelParent;
	}

	public void setKeyText (bool state)
	{
		//set the key text in the icon with the current action
		if (keyText != null) {

			currentTouchButtonIcon = touchButtonIcon;
			currentKetText = keyText;
			currentExtraTextStartActionKey = extraTextStartActionKey;
			currentExtraTextEndActionKey = extraTextEndActionKey;

			useMultipleInteractionInfo = false;

			if (useIconButtonInfoList) {
				if (currentIconButtonInfo != null) {
					currentTouchButtonIcon = currentIconButtonInfo.touchButtonIcon;
					currentKetText = currentIconButtonInfo.keyText;
					currentExtraTextStartActionKey = currentIconButtonInfo.extraTextStartActionKey;
					currentExtraTextEndActionKey = currentIconButtonInfo.extraTextEndActionKey;

					useMultipleInteractionInfo = currentIconButtonInfo.useMultipleInteractionInfo;

					if (useMultipleInteractionInfo) {
						currentKeyTextList = currentIconButtonInfo.keyTextList;
					}
				}
			}

			bool touchControlsActive = playerInput.isUsingTouchControls ();

			if (currentTouchButtonIcon.activeSelf != touchControlsActive) {
				currentTouchButtonIcon.SetActive (touchControlsActive);
			}

			if (useMultipleInteractionInfo) {
				for (int i = 0; i < currentKeyTextList.Count; i++) {
					if (currentKeyTextList [i].gameObject.activeSelf != !touchControlsActive) {
						currentKeyTextList [i].gameObject.SetActive (!touchControlsActive);
					}
				}
			} else {
				if (currentKetText != null) {
					if (currentKetText.gameObject.activeSelf != !touchControlsActive) {
						currentKetText.gameObject.SetActive (!touchControlsActive);
					}
				}
			}

			if (!touchControlsActive) {
				if (useMultipleInteractionInfo) {
					for (int i = 0; i < currentKeyTextList.Count; i++) {
						string textValue = "";

						if (state) {
							if (deviceStringManager != null && deviceStringManager.multipleInteractionNameList.Count > i) {
								string buttonKeyValue = getInteractionActionName (deviceStringManager.multipleInteractionNameList [i]);

								if (buttonKeyValue != "") {

									textValue = currentExtraTextStartActionKey +
									playerInput.getButtonKey (buttonKeyValue) +
									currentExtraTextEndActionKey;
								}
							} 
						}

						currentKeyTextList [i].text = textValue;
					}
				} else {
					if (currentKetText != null) {
						if (state) {
							currentKetText.text = currentExtraTextStartActionKey + playerInput.getButtonKey (useDevicesActionName) + currentExtraTextEndActionKey;
						} else {
							currentKetText.text = "";
						}
					}
				}
			}
		}
	}

	string getInteractionActionName (string interactionName)
	{
		for (int i = 0; i < multipleInteractionInfoList.Count; i++) {
			if (multipleInteractionInfoList [i].multipleInteractionNameList.Contains (interactionName)) {
				return multipleInteractionInfoList [i].inputName;
			}
		} 

		return "";
	}

	public void setActionAndNameText (string newActionText, string newObjectNameText)
	{
		if (actionText != null) {
			currentActionText = actionText;
			currentObjectNameText = objectNameText;

			useMultipleInteractionInfo = false;

			if (useIconButtonInfoList) {
				if (currentIconButtonInfo != null) {
					if (currentIconButtonInfo.actionText != null) {
						currentActionText = currentIconButtonInfo.actionText;
					}

					if (currentIconButtonInfo.objectNameText != null) {
						currentObjectNameText = currentIconButtonInfo.objectNameText;
					}

					useMultipleInteractionInfo = currentIconButtonInfo.useMultipleInteractionInfo;

					if (useMultipleInteractionInfo) {
						currentActionTextList = currentIconButtonInfo.actionTextList;
					}
				}
			}

			bool isCheckLanguageActive = gameLanguageSelector.isCheckLanguageActive ();
				
			if (isCheckLanguageActive) {
				if (newObjectNameText != null && newObjectNameText != "") {
					newObjectNameText = interactionObjectsLocalizationManager.GetLocalizedValue (newObjectNameText);
				}

				currentObjectNameText.text = newObjectNameText;
			}

			if (useMultipleInteractionInfo) {
				for (int i = 0; i < currentActionTextList.Count; i++) {
					if (deviceStringManager.multipleInteractionNameList.Count > i) {
						string keyTextValue = deviceStringManager.multipleInteractionNameList [i];

						if (isCheckLanguageActive) {
							keyTextValue = interactionObjectsLocalizationManager.GetLocalizedValue (keyTextValue);
						}

						currentActionTextList [i].text = keyTextValue;
					}
				}
			} else {
				if (isCheckLanguageActive) {
					if (newActionText != null && newActionText != "") {
						newActionText = interactionObjectsLocalizationManager.GetLocalizedValue (newActionText);
					}
				}

				currentActionText.text = newActionText;
			}
		}
	}

	public void setExtraActionInfo (Texture newIcon, string newDescription)
	{
		currentObjectImage = objectImage;
		currentObjectDescriptionText = objectDescriptionText;

		if (useIconButtonInfoList) {
			if (currentIconButtonInfo != null) {
				if (currentIconButtonInfo.objectImage != null) {
					currentObjectImage = currentIconButtonInfo.objectImage;
				}

				if (currentIconButtonInfo.objectDescriptionText != null) {
					currentObjectDescriptionText = currentIconButtonInfo.objectDescriptionText;
				}
			}
		}

		if (currentObjectImage != null) {
			if (newIcon != null) {
				if (currentObjectImage.gameObject.activeSelf != true) {
					currentObjectImage.gameObject.SetActive (true);
				}

				currentObjectImage.texture = newIcon;
			} else {
				if (currentObjectImage.gameObject.activeSelf != false) {
					currentObjectImage.gameObject.SetActive (false);
				}
			}
		}

		if (currentObjectDescriptionText != null) {
			if (newDescription != "") {
				if (currentObjectDescriptionText.gameObject.activeSelf != true) {
					currentObjectDescriptionText.gameObject.SetActive (true);
				}

				if (gameLanguageSelector.isCheckLanguageActive ()) {
					newDescription = interactionObjectsLocalizationManager.GetLocalizedValue (newDescription);
				}

				currentObjectDescriptionText.text = newDescription;

				currentObjectDescriptionText.fontSize = currentDeviceDescriptionFontSize;
			} else {
				if (currentObjectDescriptionText.gameObject.activeSelf != false) {
					currentObjectDescriptionText.gameObject.SetActive (false);
				}
			}
		}

		checkNewTouchButtonRawImageIcon ();
	}

	void checkNewTouchButtonRawImageIcon ()
	{
		bool usingCustomTouchButtonRawImage = false;

		if (useIconButtonInfoList) {
			if (currentIconButtonInfo != null && currentIconButtonInfo.setNewTouchButtonRawImageIcon && touchButtonRawImage != null) {
				touchButtonRawImage.texture = currentIconButtonInfo.newTouchButtonRawImageIcon;

				usingNewTouchButtonRawImageIcon = true;

				usingCustomTouchButtonRawImage = true;
			} 
		}

		if (!usingCustomTouchButtonRawImage) {
			if (usingNewTouchButtonRawImageIcon && touchButtonRawImage != null) {
				touchButtonRawImage.texture = originalTouchButtonRawImage;

				usingNewTouchButtonRawImageIcon = false;
			}
		}
	}

	public void updateUIInfo ()
	{
		checkDeviceName ();
	}

	public void checkShowObjectMessage (string message, float waitTime)
	{
		if (showObjectMessageCoroutine != null) {
			StopCoroutine (showObjectMessageCoroutine);
		}

		showObjectMessageCoroutine = StartCoroutine (showObjectMessage (message, waitTime));
	}

	IEnumerator showObjectMessage (string message, float waitTime)
	{
		if (gameLanguageSelector.isCheckLanguageActive ()) {
			message = interactionObjectsLocalizationManager.GetLocalizedValue (message);
		}

		interactionMessageText.text = message;

		if (!interactionMessageGameObject.activeSelf) {
			interactionMessageGameObject.SetActive (true);
		}

		yield return new WaitForSeconds (waitTime);

		if (waitTime > 0) {
			interactionMessageGameObject.SetActive (false);
		}
	}

	public void stopShowObjectMessage ()
	{
		if (showObjectMessageCoroutine != null) {
			StopCoroutine (showObjectMessageCoroutine);
		}

		if (interactionMessageGameObject.activeSelf) {
			interactionMessageGameObject.SetActive (false);
		}
	}

	public string getCurrentDeviceActionText ()
	{
		return currentDeviceActionText;
	}

	public bool hasDeviceToUse ()
	{
		return objectToUse != null;
	}

	public void setUseDeviceButtonEnabledState (bool state)
	{
		useDeviceButtonEnabled = state;
		holdingButton = false;
	}

	public void setPauseUseDeviceButtonForDurationState (float pauseDuration)
	{
		stopSetPauseUseDeviceButtonForDurationState ();

		pauseUseDeviceButtonForDuration = StartCoroutine (setPauseUseDeviceButtonForDurationStateCoroutine (pauseDuration));
	}

	public void stopSetPauseUseDeviceButtonForDurationState ()
	{
		if (pauseUseDeviceButtonForDuration != null) {
			StopCoroutine (pauseUseDeviceButtonForDuration);
		}
	}

	IEnumerator setPauseUseDeviceButtonForDurationStateCoroutine (float pauseDuration)
	{
		useDeviceButtonEnabled = false;

		yield return new WaitForSeconds (pauseDuration);

		useDeviceButtonEnabled = true;
	}

	public void checkIfPickObjectsAround ()
	{
		if (holdingButton) {
			if (Time.time > lastTimePressedButton + holdButtonTime) {
				//print ("take all");
				takePickupsAround ();

				holdingButton = false;
			} else {

				if (!holdingInteractionButtonActive) {
					useDevice ();
				}
			}
		}

		holdingButton = false;
	}

	public void checkIfStopUseDevice ()
	{
		if (playerControllerManager.isUsingDevice ()) {
			useDevice ();
		}
	}

	void checkInteractionActions ()
	{
		if (useInteractionActions) {
			bool playerCanUseActionsResult = (!playerControllerManager.isGamePaused () && !playerControllerManager.isPlayerMenuActive ());

			for (int i = 0; i < interactionActionInfoList.Count; i++) {

				if (!interactionActionInfoList [i].canBeUsedOnGamePaused || playerCanUseActionsResult) {

					interactionActionInfoList [i].eventOnInteraction.Invoke ();
				}
			}
		}
	}

	IEnumerator updateHoldInteractionButtonCoroutine ()
	{
//		print ("start hold coroutine");

		float timer = 0;

		bool targetReached = false;

		bool currentHoldInteractionSliderLocated = currentHoldInteractionSlider != null;

		bool holdInteractionTimerTextLocated = holdInteractionTimerText != null;

		if (currentHoldInteractionSliderLocated) {
			currentHoldInteractionSlider.fillAmount = 0;
		}

		if (holdInteractionTimerText != null) {
			holdInteractionTimerText.gameObject.SetActive (true);
		}

		while (!targetReached) {
			timer += Time.deltaTime;

			if (timer >= currentHoldInteractionButtonDuration) {
				targetReached = true;
			} else {
				if (currentHoldInteractionSliderLocated) {
					currentHoldInteractionSlider.fillAmount = timer / currentHoldInteractionButtonDuration;

					if (showHoldInteractionTimer) {
						if (holdInteractionTimerTextLocated) {
							holdInteractionTimerText.text = (currentHoldInteractionButtonDuration - timer).ToString ("F1");
						}
					}
				}
			}

			yield return null;
		}

		if (currentDeviceToUseHoldInteraction != null && deviceGameObjectList.Contains (currentDeviceToUseHoldInteraction)) {
			useCurrentDevice (currentDeviceToUseHoldInteraction);

			checkInteractionActions ();
		}

		if (currentHoldInteractionSliderLocated) {
			currentHoldInteractionSlider.fillAmount = 0;
		}

		if (holdInteractionTimerText != null) {
			holdInteractionTimerText.gameObject.SetActive (false);
		}

		holdingInteractionButtonActive = false;
	}

	public void stopHoldInteractionButtonCoroutine ()
	{
//		print ("stop coroutine hold");

		if (holdInteractionButtonCoroutine != null) {
			StopCoroutine (holdInteractionButtonCoroutine);
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputActivateDeviceByName (string interactionName)
	{
		currentMultipleInteractionName = interactionName;

		checkMultipleInteractionActive = true;

		inputActivateDevice ();

		checkMultipleInteractionActive = false;

		currentMultipleInteractionName = "";
	}

	public void inputActivateDevice ()
	{
		if (useDeviceButtonEnabled) {
			bool useDeviceResult = false;

			if (!currentDeviceIsPickup || !holdButtonToTakePickupsAround) {
				useDeviceResult = true;
			}

			if (currentDeviceIsPickup) {
				if (deviceStringManager != null) {
					if (deviceStringManager.useHoldInteractionButton) {
						useDeviceResult = true;
					}
				}
			}

			if (useDeviceResult) {
				useDevice ();
			}
		}

//		print ("press down interaction button");

		if (holdingInteractionButtonActive) {
			return;
		}

		checkInteractionActions ();
	}

	public void inputHoldToPickObjectsAround ()
	{
		if (useDeviceButtonEnabled) {
			if (currentDeviceIsPickup && holdButtonToTakePickupsAround) {
				holdingButton = true;

				lastTimePressedButton = Time.time;
			}
		}
	}

	public void inputReleaseToPickObjectsAround ()
	{
		if (useDeviceButtonEnabled) {
			if (currentDeviceIsPickup && holdButtonToTakePickupsAround) {
				checkIfPickObjectsAround ();
			}

			if (holdInteractionButtonEnabled) {
				if (holdingInteractionButtonActive) {
					stopHoldInteractionButtonCoroutine ();

					holdingInteractionButtonActive = false;

					if (currentHoldInteractionSlider != null) {
						currentHoldInteractionSlider.fillAmount = 0;
					}

					if (holdInteractionTimerText != null) {
						holdInteractionTimerText.gameObject.SetActive (false);
					}

//					print ("release hold button");
				}
			}
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void setCanUseDevicesState (bool state)
	{
		canUseDevices = state;
	}

	[System.Serializable]
	public class deviceInfo
	{
		public GameObject deviceGameObject;
		public Transform deviceTransform;

		public Transform positionToIcon;
		public bool useTransformForStringAction;
		public bool useSeparatedTransformForEveryView;
		public float actionOffset;
		public bool useLocalOffset;

		public GameObject deviceIcon;
		public RectTransform deviceIconRectTransform;

		public bool useCustomMinDistanceToUseDevice;
		public float customMinDistanceToUseDevice;

		public bool useCustomMinAngleToUse;
		public float customMinAngleToUseDevice;
		public bool useRelativeDirectionBetweenPlayerAndObject;

		public bool ignoreUseOnlyDeviceIfVisibleOnCamera;

		public bool useCustomDeviceTransformPosition;
		public Transform customDeviceTransformPosition;

		public bool useFixedDeviceIconPosition;

		public bool checkIfObstacleBetweenDeviceAndPlayer;
	}

	[System.Serializable]
	public class interactionActionInfo
	{
		public string Name;
		public UnityEvent eventOnInteraction;
		public bool canBeUsedOnGamePaused;
	}

	[System.Serializable]
	public class iconButtonInfo
	{
		public string Name;
		public GameObject iconButtonPanel;
		public Text keyText;
		public Text actionText;
		public Text objectNameText;
		public GameObject touchButtonIcon;
		public string extraTextStartActionKey = "[";
		public string extraTextEndActionKey = "]";

		public RawImage objectImage;
		public Text objectDescriptionText;
	
		public bool useFixedPosition;
		public Transform fixedPositionTransform;

		public bool setNewTouchButtonRawImageIcon;
		public Texture newTouchButtonRawImageIcon;

		public bool useMultipleInteractionInfo;

		public Image holdInteractionSlider;

		public bool showHoldInteractionTimer;
		public Text holdInteractionTimerText;

		public List<Text> keyTextList = new List<Text> ();
		public List<Text> actionTextList = new List<Text> ();
	}

	[System.Serializable]
	public class multipleInteractionInfo
	{
		public string inputName;

		public List<string> multipleInteractionNameList = new List<string> ();
	}
}