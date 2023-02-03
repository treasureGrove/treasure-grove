using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mapSystem : MonoBehaviour
{
	public bool mapEnabled;
	//[Header ("Map Components")]
	public GameObject mapContent;
	public GameObject mapCamera;
	public Transform mapSystemPivotTransform;
	public Transform mapSystemCameraTransform;
	public GameObject player;
	public GameObject mapMenu;
	public RectTransform mapWindowTargetPosition;
	public RectTransform mapRender;
	public RectTransform mapWindow;

	public bool playerUseMapObjectInformation;
	public RectTransform playerMapIcon;
	public RectTransform playerIconChild;

	public Image removeMarkButtonImage;
	public Image quickTravelButtonImage;

	public Text mapObjectNameField;
	public Text mapObjectInfoField;

	public Text currentFloorNumberText;
	public bool useMapIndexWindow;
	public GameObject mapIndexWindow;
	public GameObject mapIndexWindowContent;
	public Scrollbar mapIndexWindowScroller;

	public string mainScreenObjectivesManagerName = "Screen Objectives Manager";

	public string ingameMenuName = "Map System";
	public string beaconIconTypeName = "Beacon";
	public string markIconTypeName = "Mark";


	public playerInputManager playerInput;
	public menuPause pauseManager;
	public screenObjectivesSystem screenObjectivesManager;
	public playerController playerControllerManager;
	public mapCreator mapCreatorManager;
	public mapObjectInformation playerMapObjectInformation;
	public Camera mainMapCamera;

	//[Header ("Map Options")]
	public float playerIconMovementSpeed;
	public float openMapSpeed;
	public float mouseDragMapSpeed = 4;
	public float keysDragMapSpeed = 1;

	public bool rotateMap;
	public bool smoothRotationMap;
	public float rotationSpeed;
	public bool showOffScreenIcons;
	public float borderOffScreen;
	public float iconSize;
	public float maxIconSize;
	public float offScreenIconSize;
	public float openMapIconSizeMultiplier;
	public float changeIconSizeSpeed;

	public float recenterCameraSpeed = 2;

	public Scrollbar zoomScrollbar;
	public float zoomWhenOpen;
	public float zoomWhenClose;
	public float openCloseZoomSpeed;
	public float zoomSpeed;
	[Range (0, 200)] public float maxZoom;
	[Range (0, 200)] public float minZoom;
	public float zoomToActivateIcons;
	public float zoomToActivateTextIcons = 100;

	public float zoomWhenOpen3d;
	public float zoomWhenClose3d;
	public float openCloseZoomSpeed3d;
	public float zoomSpeed3d;
	[Range (0, 200)] public float maxZoom3d;
	[Range (0, 200)] public float minZoom3d;
	public float zoomToActivateIcons3d;
	public float zoomToActivateTextIcons3d = 100;

	public bool usingCircleMap;
	public float circleMapRadius;
	public Transform mapCircleTransform;

	Matrix4x4 ortho;
	Matrix4x4 perspective;
	public float near = .3f;
	public float far = 1000f;
	float aspect;
	Coroutine cameraProjectionChangeCoroutine;

	bool viewTo3dHasBeenActive;
	float currentFieldOfView;
	float currentOrthographicSize;

	bool usingMouseWheelZoomIn;
	bool usingMouseWheelZoomOut;
	bool usingMouseWheelZoomPreviouosly;

	float zoomToActivateTextIconsValue;
	float zoomToActivateIconsValue;

	bool canEnableCurrentIcon;

	public Color disabledRemoveMarkColor;
	public Color disabledQuickTravelColor;
	public bool showIconsByFloor;

	//[Header ("Mark Options")]
	public GameObject markPrefab;
	public bool setMarkOnCurrenBuilding;
	public bool setMarkOnCurrentFloor;

	//[Header ("Compass Components")]
	public RectTransform compassWindow;
	public RectTransform compassElementsParent;
	public RectTransform north;
	public RectTransform south;
	public RectTransform east;
	public RectTransform west;
	public RectTransform northEast;
	public RectTransform southWest;
	public RectTransform southEast;
	public RectTransform northWest;
	public Transform compassDirections;

	public GameObject northGameObject;
	public GameObject southGameObject;
	public GameObject eastGameObject;
	public GameObject westGameObject;

	public GameObject northEastGameObject;
	public GameObject southWestGameObject;
	public GameObject southEastGameObject;
	public GameObject northwestGameObject;

	public float compassScale = 1;
	public float compassOffset;

	public float maximumLeftDistance;
	public float maximumRightDistance;

	//[Header ("Compass Options")]
	public bool compassEnabled;
	public bool showIntermediateDirections;

	bool compassInitialized;

	public bool usePlayerTransformOrientationOnCompassOnThirdPerson;
	public bool usePlayerTransformOrientationOnCompassOnFirstPerson = true;
	public bool usePlayerTransformOrientationOnCompassOnLockedCamera = true;

	public List<compassIconInfo> compassIconList = new List<compassIconInfo> ();

	compassIconInfo currentCompassIconInfo;

	Transform currentIconTargetTransform;

	float minDifferenceToUpdateDistanceToObject = 0.01f;

	Vector3 compassDirection;

	Vector3 playerCompassPosition;
	Vector3 compassTargetPosition;

	float currentCompassOrientation;
	float compassTargetRotation;
	float reversedCompassTargetRotation;
	float currentCompassTargetRotation;


	//[Header ("Map Floors")]
	buildingInfo currentBuilding;
	public List<floorInfo> floors = new List<floorInfo> ();
	public List<buildingInfo> buildingList = new List<buildingInfo> ();

	public bool updateMapIconTypesOnStart = true;

	//[Header ("Map Icons")]
	public List<mapIconType> mapIconTypes = new List<mapIconType> ();
	public List<markInfo> markList = new List<markInfo> ();
	public List<glossaryElementInfo> glossaryList = new List<glossaryElementInfo> ();

	public string currentBuildingName;
	public int currentBuildingIndex;
	public int currentMapPartIndex;

	public int previousBuildingIndex;

	public int currentFloorIndex;
	int previousCurrentFloorIndex = -1;
	public int currentFloorNumber;

	public bool mapOpened;
	public bool useTextInIcons;
	public GameObject mapObjectTextIcon;
	public Color mapObjectTextIconColor = Color.white;
	public int mapObjectTextSize = 20;
	public Vector2 textIconsOffset;
	public List<mapObjectInfo> mapObjects = new List<mapObjectInfo> ();
	public bool getClosestFloorToPlayerByDistance;

	public bool miniMapWindowEnabledInGame = true;

	public bool miniMapWindowSmoothOpening = true;
	public bool miniMapWindowWithMask;
	public RectTransform mapWindowMask;

	public mapCameraMovement mapCameraMovementType;

	public enum mapCameraMovement
	{
		XY,
		XZ,
		YZ
	}

	public bool useBlurUIPanel = true;

	public bool useMapCursor;
	public GameObject mapCursor;
	public bool showInfoIconInsideCursor;
	public int maxDistanceToMapIcon;
	public RectTransform mapCursorRectTransform;

	RectTransform currentIconOnMapCursor;
	RectTransform previousIconOnMapCursor;
	int currentMapObjectIndexOnMapCursor;
	float currentDistanceToMapCursor;

	public bool useCurrentMapIconPressed;
	public GameObject currenMapIconPressed;

	//custom inspector variables
	public bool showMapComponents;
	public bool showMapSettings;
	public bool showCompassComponents;
	public bool showCompassSettings;
	public bool showMapFloorAndIcons;
	public bool showMarkSettings;

	public Vector3 playerIconOffset;
	int reverseCurrentCompassRotation;
	int currentCompassRotation;
	Vector3 cameraOffset;
	float currenIconSize;
	float currentIconSizeMultiplier = 1;
	Vector2 originalPlayerMapIconSize;
	Vector3 originalMapPosition;
	Vector3 targetMapPosition;
	Vector2 originalMapScale;
	Vector2 targetScale;
	Vector3 beginTouchPosition;

	mapObjectInfo currentMark;
	Color originalRemoveMarkColor;
	Color originalQuickTravelColor;

	Coroutine moveMapCoroutine;
	Coroutine cameraSizeCoroutine;
	Touch currentTouch;
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	Transform mainMapCameraTransform;

	bool touchPlatform;
	bool zoomingIn;
	bool zoomingOut;
	bool movingMap;

	GameObject currentQuickTravelStation;

	float currentCameraSize;
	bool changingCameraSize;
	float zoomAmountMultiplier;

	bool mapIndexEnabled;

	float horizontalInput;
	float verticalInput;

	public bool map3dEnabled;
	public float map3dPositionSpeed = 30;
	public float map3dRotationSpeed = 10;
	public Vector2 rangeAngleX;
	public Vector2 rangeAngleY;
	public float transtionTo3dSpeed = 0.5f;
	public float maxTimeBetweenTransition = 0.5f;
	public float reset3dCameraSpeed = 20;
	public Vector2 inital3dCameraRotation;
	public bool hideOffscreenIconsOn3dView;
	public bool showIconsOn3dView;

	public bool use3dMeshForPlayer;
	public GameObject player3dMesh;

	public bool setColorOnCurrent3dMapPart;
	public Color colorOnCurrent3dMapPart = Color.blue;

	public Text currentMapZoneText;

	public GameObject mapCreatorPrefab;

	public bool useEventIfMapDisabled;
	public UnityEvent eventIfMapDisabled;

	public bool useEventIfMapEnabled;
	public UnityEvent eventIfMapEnabled;

	public bool showWarningMessages;

	Vector2 axisValues;
	Vector2 mouseValues;
	Vector2 currentLookAngle;
	Quaternion currentCameraRotation;
	Quaternion currentPivotRotation;

	bool mapViewIn2dActive = true;

	Vector3 mapObjectPosition;
	Vector2 mapObjectViewPoint;
	Vector2 mapObjectPosition2d;
	Vector3 mapIconRotation;

	RectTransform mapWindowBorder;
	Vector2 mapWindowSizeDelta;
	Vector2 mapWindowBorderSizeDelta;
	float mapWindowBorderSizeDeltaX;
	float mapWindowBorderSizeDeltaY;

	RectTransform currentMapIcon;
	RectTransform currentMapTextName;
	GameObject currentMapGameObject;

	bool currentIconOffscreen;

	mapObjectInfo currentMapObjectInfo;

	Vector3 playerPosition;
	Vector2 playerIconPosition;
	float playerIconPositionX;
	float playerIconPositionY;
	Vector3 mapCameraRotation;

	Vector2 fromOriginToObject;
	float currentMapObjectDistance;

	bool currentMouseWheelPositiveValue;
	bool currentMouseWheelNegativeValue;

	Vector3 newCameraPosition;

	public bool changeFloorWithTriggers = true;
	bool floorsConfiguredByTriggers;

	Coroutine resetCameraPositionCoroutine;

	Coroutine cameraPivotResetRotationCoroutine;

	bool resetingCameraPosition;

	int mapObjectsCount;

	public string[] buildingListString;
	public string[] floorListString;
	public string currentFloorName;
	public string[] mapPartListString;
	public string currentMapPartName;



	public void setMapEnabledState (bool state)
	{
		mapEnabled = state;
	}

	public void setCompassEnabledState (bool state)
	{
		compassEnabled = state;
	}

	public void setCompassEnabledStateAndUpdateState (bool state)
	{
		setCompassEnabledState (state);

		updateCompassCurrentEnabledState ();
	}


	void disableMapSystemWhenNotMapFound ()
	{
		if (showWarningMessages) {
			print ("WARNING: Map Creator hasn't been found, The map manager is disabled, along with the map window. " +
			"Drag and drop the Map System (in the Map Prefabs folder) to create the level map");
		}

		mapEnabled = false;

		if (mapCamera.activeSelf) {
			mapCamera.SetActive (false);
		}

		if (mapContent.activeSelf) {
			mapContent.SetActive (false);
		}

		setCompassInitialState ();

		if (useEventIfMapDisabled) {
			eventIfMapDisabled.Invoke ();
		}
	}

	void Awake ()
	{
		if (updateMapIconTypesOnStart) {
			if (mapCreatorManager != null) {
				mapIconTypes = mapCreatorManager.getMapIconTypes ();
			}
		}
	}

	void Start ()
	{
		if (mapEnabled) {

			checkGetMapCreatorManager ();

			if (mapCreatorManager == null) {
				disableMapSystemWhenNotMapFound ();

				return;
			}

			int floorsCount = floors.Count;

			if (floorsCount > 0) {
				for (int i = 0; i < floorsCount; i++) {
					if (floors [i].floor == null) {
						disableMapSystemWhenNotMapFound ();

						return;
					}
				}
			} else {
				disableMapSystemWhenNotMapFound ();

				return;
			}

			if (useEventIfMapEnabled) {
				eventIfMapEnabled.Invoke ();
			}

			originalMapPosition = mapWindow.position;
			originalMapScale = mapWindow.sizeDelta;

			cameraOffset = mapCamera.transform.localPosition;

			if (miniMapWindowEnabledInGame) {
				enableOrDisableMapWindow (true);
			} else {
				enableOrDisableMapWindow (false);
			}

			touchPlatform = touchJoystick.checkTouchPlatform ();

			mainMapCameraTransform = mainMapCamera.transform;

			originalPlayerMapIconSize = playerIconChild.sizeDelta;
			originalRemoveMarkColor = removeMarkButtonImage.color;
			removeMarkButtonImage.color = disabledRemoveMarkColor;
			originalQuickTravelColor = quickTravelButtonImage.color;
			quickTravelButtonImage.color = disabledQuickTravelColor;

			removeMapObjectInfo ();

			setCompassInitialState ();

			mapMenu.SetActive (false);

			if (useMapIndexWindow) {
				mapMenu.SetActive (true);

				showOrHideMapIndexWindow (true);

				GameObject iconInfoElement = mapIndexWindowContent.transform.GetChild (0).gameObject;

				//every key field in the edit input button has an editButtonInput component, so create every of them
				int mapIconTypesCount = mapIconTypes.Count;

				for (int i = 0; i < mapIconTypesCount; i++) {
					mapIconType currentMapIconType = mapIconTypes [i];
						
					GameObject iconInfoElementClone = (GameObject)Instantiate (iconInfoElement, iconInfoElement.transform.position, 
						                                  Quaternion.identity, iconInfoElement.transform.parent);

					iconInfoElementClone.transform.localScale = Vector3.one;
					iconInfoElementClone.name = currentMapIconType.typeName;

					Text currentIconText = iconInfoElementClone.GetComponentInChildren<Text> ();

					RawImage currentIconInfoRawImage = iconInfoElementClone.GetComponentInChildren<RawImage> ();
					RawImage currentMapIconRawImage = currentMapIconType.icon.GetComponent<RawImage> ();

					currentIconText.text = currentMapIconType.typeName;
					currentIconInfoRawImage.texture = currentMapIconRawImage.texture;
					currentIconInfoRawImage.color = currentMapIconRawImage.color;

					glossaryElementInfo newGlossaryElement = iconInfoElementClone.GetComponent<glossaryElement> ().glossaryInfo;

					newGlossaryElement.textName = currentIconText;
					newGlossaryElement.slider = iconInfoElementClone.GetComponentInChildren<Slider> ();
					newGlossaryElement.typeName = currentMapIconType.typeName;

					glossaryList.Add (newGlossaryElement);
				}

				iconInfoElement.SetActive (false);

				//set the scroller in the top position
				mapIndexWindowScroller.value = 1;

				showOrHideMapIndexWindow (false);

				mapMenu.SetActive (false);

				if (screenObjectivesManager == null) {
					GKC_Utils.instantiateMainManagerOnSceneWithType (mainScreenObjectivesManagerName, typeof(screenObjectivesSystem));

					screenObjectivesManager = FindObjectOfType<screenObjectivesSystem> ();
				}
			}

			mainMapCamera.orthographicSize = zoomWhenClose;

			aspect = (float)Screen.width / (float)Screen.height;
			aspect = mainMapCamera.aspect;

			assignMapWindowBorder ();

			zoomScrollbar.value = zoomWhenOpen / maxZoom;

			currentFieldOfView = mainMapCamera.fieldOfView;
			currentOrthographicSize = mainMapCamera.orthographicSize;

			if (mapCursor != null) {
				if (mapCursor.activeSelf) {
					mapCursor.SetActive (false);
				}
			}

			if (playerUseMapObjectInformation) {
				if (playerMapIcon.gameObject.activeSelf) {
					playerMapIcon.gameObject.SetActive (false);
				}
			}

		} else {
			if (useEventIfMapDisabled) {
				eventIfMapDisabled.Invoke ();
			}

			if (mapContent.activeSelf) {
				mapContent.SetActive (false);
			}

			setCompassInitialState ();
		}
	}

	void initializeMainCompassElements ()
	{
		if (!compassInitialized) {

			if (!showIntermediateDirections) {
				if (northEast.gameObject.activeSelf) {
					northEast.gameObject.SetActive (false);
				}

				if (southWest.gameObject.activeSelf) {
					southWest.gameObject.SetActive (false);
				}

				if (southEast.gameObject.activeSelf) {
					southEast.gameObject.SetActive (false);
				}

				if (northWest.gameObject.activeSelf) {
					northWest.gameObject.SetActive (false);
				}
			}

			addCompassIcon (northGameObject, north.gameObject, compassOffset, false);
			addCompassIcon (southGameObject, south.gameObject, compassOffset, false);
			addCompassIcon (eastGameObject, east.gameObject, compassOffset, false);
			addCompassIcon (westGameObject, west.gameObject, compassOffset, false);

			if (showIntermediateDirections) {
				addCompassIcon (northEastGameObject, northEast.gameObject, compassOffset, false);
				addCompassIcon (southWestGameObject, southWest.gameObject, compassOffset, false);
				addCompassIcon (southEastGameObject, southEast.gameObject, compassOffset, false);
				addCompassIcon (northwestGameObject, northWest.gameObject, compassOffset, false);
			}

			compassInitialized = true;
		}
	}

	public void updateCompassCurrentEnabledState ()
	{
		enableOrDisableCompass (compassEnabled);

		if (compassEnabled) {
			initializeMainCompassElements ();
		}
	}

	public void setCompassInitialState ()
	{
		updateCompassCurrentEnabledState ();

		if (!mapEnabled) {
			if (mapCreatorManager != null) {
				mapIconTypes = mapCreatorManager.getMapIconTypes ();
			}
		}
	}

	public Vector3 getCameraPosition ()
	{
		Vector3 newPosition = player.transform.position;

		if (mapCameraMovementType == mapCameraMovement.XZ) {
			newPosition = new Vector3 (newPosition.x, newPosition.y + cameraOffset.y, newPosition.z);
		} else if (mapCameraMovementType == mapCameraMovement.XY) {
			newPosition = new Vector3 (newPosition.x, newPosition.y, cameraOffset.z);
		} else if (mapCameraMovementType == mapCameraMovement.YZ) {
			newPosition = new Vector3 (cameraOffset.x, newPosition.y, newPosition.z);
		}

		return newPosition;
	}

	void FixedUpdate ()
	{
		checkUpdateCompass ();

		if (!mapEnabled) {
			return;
		}

		//if the map is shown in the minimap or the map menu is open, check every icon and map object to place every element in the proper place
		if (miniMapWindowEnabledInGame || mapOpened) {
			mapWindowBorderSizeDelta = mapWindowBorder.sizeDelta;
			mapWindowSizeDelta = mapWindow.sizeDelta;

			if (!playerUseMapObjectInformation) {
				playerPosition = mainMapCamera.WorldToViewportPoint (player.transform.position + playerIconOffset);
				playerIconPositionX = (playerPosition.x * mapWindowSizeDelta.x) - (mapWindowSizeDelta.x * 0.5f);
				playerIconPositionY = (playerPosition.y * mapWindowSizeDelta.y) - (mapWindowSizeDelta.y * 0.5f);
				playerIconPosition = new Vector2 (playerIconPositionX, playerIconPositionY);
				playerMapIcon.anchoredPosition = playerIconPosition;

				playerIconChild.sizeDelta = Vector2.Lerp (playerIconChild.sizeDelta, originalPlayerMapIconSize * currentIconSizeMultiplier, getDeltaTime () * changeIconSizeSpeed);
			} 

			if (showInfoIconInsideCursor) {
				currentDistanceToMapCursor = 10000;
				currentMapObjectIndexOnMapCursor = -1;
			}

			mapObjectsCount = mapObjects.Count;

			for (int i = 0; i < mapObjectsCount; i++) {
				if (mapObjects [i].mapObject != null) {

					currentMapObjectInfo = mapObjects [i];

					currentMapIcon = currentMapObjectInfo.mapIcon;

					if (currentMapIcon != null) {

						if ((currentMapObjectInfo.buildingIndex == currentBuildingIndex || currentMapObjectInfo.visibleAllBuildings) &&
						    ((currentMapObjectInfo.hasFloorAssigned && currentMapObjectInfo.floorNumber == currentFloorNumber) ||
						    !showIconsByFloor || currentMapObjectInfo.visibleAllFloors || currentMapObjectInfo.canBeShowTemporaly)) {

							zoomToActivateIconsValue = zoomToActivateIcons;

							if (mapViewIn2dActive) {
								zoomToActivateIconsValue = zoomToActivateIcons3d;
							}

							if (currentCameraSize <= zoomToActivateIconsValue) {
								canEnableCurrentIcon = true;
							} else {
								canEnableCurrentIcon = false;
							}

							if (!mapViewIn2dActive && !showIconsOn3dView) {
								canEnableCurrentIcon = false;
							}

							if (currentMapObjectInfo.isActivated && canEnableCurrentIcon) {
								if (currentMapIcon == null) {
									//print (mapObjects [i].name);
									return;
								}

								setMapGameObjectState (currentMapIcon.gameObject, true);

								mapObjectPosition = currentMapObjectInfo.mapObject.transform.position + currentMapObjectInfo.offset;
								mapObjectViewPoint = mainMapCamera.WorldToViewportPoint (mapObjectPosition);

								mapObjectPosition2d = new Vector2 ((mapObjectViewPoint.x * mapWindowSizeDelta.x) - (mapWindowSizeDelta.x * 0.5f),
									(mapObjectViewPoint.y * mapWindowSizeDelta.y) - (mapWindowSizeDelta.y * 0.5f));

								mapWindowBorderSizeDeltaX = (mapWindowBorderSizeDelta.x * 0.5f) - borderOffScreen;
								mapWindowBorderSizeDeltaY = (mapWindowBorderSizeDelta.y * 0.5f) - borderOffScreen;

								if (showOffScreenIcons) {
									mapObjectPosition2d.x = Mathf.Clamp (mapObjectPosition2d.x, -mapWindowBorderSizeDeltaX, mapWindowBorderSizeDeltaX);
									mapObjectPosition2d.y = Mathf.Clamp (mapObjectPosition2d.y, -mapWindowBorderSizeDeltaY, mapWindowBorderSizeDeltaY);
								}

								currenIconSize = iconSize;

								if (usingCircleMap && !mapOpened) {
									currentMapObjectDistance = Vector2.Distance (mapObjectPosition2d, mapRender.anchoredPosition);

									if (currentMapObjectDistance > circleMapRadius) {
										fromOriginToObject = mapObjectPosition2d - mapRender.anchoredPosition;
										fromOriginToObject *= circleMapRadius / currentMapObjectDistance;
										mapObjectPosition2d = mapRender.anchoredPosition + fromOriginToObject;

										currenIconSize = offScreenIconSize;
										currentIconOffscreen = true;
									} else {
										currenIconSize = iconSize;
										currentIconOffscreen = false;
									}
								} else {
									if (mapObjectPosition2d.x == mapWindowBorderSizeDeltaX || mapObjectPosition2d.y == mapWindowBorderSizeDeltaY ||
									    -mapObjectPosition2d.x == mapWindowBorderSizeDeltaX || -mapObjectPosition2d.y == mapWindowBorderSizeDeltaY) {
										currenIconSize = offScreenIconSize;
										currentIconOffscreen = true;
									} else {
										currenIconSize = iconSize;
										currentIconOffscreen = false;
									}
								}

								currenIconSize *= currentIconSizeMultiplier;
								currenIconSize += currentMapObjectInfo.extraIconSizeOnMap;

								currentMapIcon.anchoredPosition = mapObjectPosition2d;
								currentMapIcon.sizeDelta = Vector2.Lerp (currentMapIcon.sizeDelta, new Vector2 (currenIconSize, currenIconSize), getDeltaTime () * changeIconSizeSpeed);

								mapIconRotation = Vector3.zero;

								if (currentMapObjectInfo.followCameraRotation) {
									mapIconRotation = new Vector3 (0, 0, mapCameraRotation.y);
								} else if (currentMapObjectInfo.followObjectRotation) {
									mapIconRotation = new Vector3 (0, 0, -currentMapObjectInfo.mapObject.transform.eulerAngles.y);
								}

								currentMapIcon.eulerAngles = mapIconRotation;

								if (currentIconOffscreen) {
									if (!mapViewIn2dActive && hideOffscreenIconsOn3dView) {
										setMapGameObjectState (currentMapIcon.gameObject, false);
									}
								}

								//get the closest icon to the map cursor to get its info
								if (mapOpened && useMapCursor && mapCursor != null && showInfoIconInsideCursor) {
									float currentDistance = GKC_Utils.distance (mapCursorRectTransform.position, currentMapIcon.position);

									if (currentDistance < currentDistanceToMapCursor) {
										currentDistanceToMapCursor = currentDistance;
										currentMapObjectIndexOnMapCursor = i;
									}
								}
							} else {
								setMapGameObjectState (currentMapIcon.gameObject, false);
							}

							currentMapTextName = currentMapObjectInfo.textName;

							if (currentMapTextName != null) {
								if (useTextInIcons) {
									if (mapOpened && !changingCameraSize) {
										zoomToActivateTextIconsValue = zoomToActivateTextIcons;

										if (!mapViewIn2dActive) {
											zoomToActivateTextIconsValue = zoomToActivateTextIcons3d;
										}

										if (currentCameraSize <= zoomToActivateTextIconsValue && !currentIconOffscreen) {
											setMapGameObjectState (currentMapTextName.gameObject, true);

											currentMapTextName.position = currentMapIcon.position + (textIconsOffset.y * (currenIconSize / maxIconSize)) * vectorUp +
											(textIconsOffset.x * (currenIconSize / maxIconSize)) * Vector3.right;

										} else {
											setMapGameObjectState (currentMapTextName.gameObject, false);
										}
									} else {
										setMapGameObjectState (currentMapTextName.gameObject, false);
									}
								} else {
									if (mapOpened) {
										setMapGameObjectState (currentMapTextName.gameObject, false);
									}
								}
							}
						} else {
							setMapGameObjectState (currentMapIcon.gameObject, false);
						}
					} else {
						print ("Map Icon for object " + currentMapObjectInfo.name + " wasn't found, make sure the map icon is configured correctly in the map creator.");
					}
				} else {
					checkCurrentMapIconPressedParent ();

					if (mapObjects [i].mapIcon != null) {
						Destroy (mapObjects [i].mapIcon.gameObject);
					}

					checkCompassIcon (i);
					mapObjects.RemoveAt (i);

					mapObjectsCount = mapObjects.Count;
				}
			}

			if (mapOpened && useMapCursor && mapCursor != null && showInfoIconInsideCursor) {
				if (currentDistanceToMapCursor < maxDistanceToMapIcon) {
					if (currentMapObjectIndexOnMapCursor > -1 && mapObjectsCount > 0) {
						currentIconOnMapCursor = mapObjects [currentMapObjectIndexOnMapCursor].mapIcon;
					}

					if (currentIconOnMapCursor != previousIconOnMapCursor) {
						previousIconOnMapCursor = currentIconOnMapCursor;

						if (currentIconOnMapCursor != null) {
							getMapObjectInfo (currentMapObjectIndexOnMapCursor);

							checkCurrentIconPressed (true, previousIconOnMapCursor);
						} 
					}
				} else {
					if (currentIconOnMapCursor != null) {
						removeMapObjectInfo ();

						currentIconOnMapCursor = null;
						previousIconOnMapCursor = null;
					}
				}
			}
		}
	}

	public void checkCurrentMapIconPressedParent ()
	{
		if (useCurrentMapIconPressed) {
			if (currenMapIconPressed != null) {
				currenMapIconPressed.transform.SetParent (mapWindow);

				if (currenMapIconPressed.activeSelf) {
					currenMapIconPressed.SetActive (false);
				}
			}
		}
	}

	void Update ()
	{
		if (!floorsConfiguredByTriggers && mapEnabled) {
			if (changeFloorWithTriggers) {
				currentBuildingIndex = mapCreatorManager.getBuldingIndexByName (currentBuildingName);

				if (currentBuildingIndex != -1) {
					mapCreatorManager.setCurrentFloorByFloorNumber (currentBuildingIndex, currentFloorNumber);

					setCurrentFloorNumber (currentFloorNumber);

					setCurrentBuildingName (currentBuildingName);

					updateCurrentBuilding (currentBuildingIndex);

					mapCreatorManager.setCurrentBuildingByBuildingIndex (currentBuildingIndex);
				} else {
					if (showWarningMessages) {
						print ("WARNING: Building with name " + currentBuildingName + " not found, make sure it is correctly configured on map creator");
					}
				}
			}
			floorsConfiguredByTriggers = true;
		}

		if (mapEnabled) {

			//set the map camera position
			if (!mapOpened) {
				newCameraPosition = getCameraPosition ();

				mapCamera.transform.position = Vector3.Lerp (mapCamera.transform.position, newCameraPosition, getDeltaTime () * playerIconMovementSpeed);
			}

			//set the map camera rotation
			if (!mapOpened) {
				if (rotateMap) {
					mapCameraRotation = mapCamera.transform.eulerAngles;
					mapCameraRotation.y = player.transform.eulerAngles.y;

					if (smoothRotationMap) {
						playerMapIcon.rotation = Quaternion.identity;
						mapCamera.transform.rotation = Quaternion.Slerp (mapCamera.transform.rotation, Quaternion.Euler (mapCameraRotation), getDeltaTime () * rotationSpeed);
					} else {
						mapCamera.transform.eulerAngles = mapCameraRotation;
					}
				} else {
					mapCamera.transform.eulerAngles = Vector3.zero;
					mapCameraRotation = Vector3.zero;

					if (mapCameraMovementType == mapCameraMovement.XZ) {
						mapCameraRotation.z = -player.transform.eulerAngles.y;
					} else if (mapCameraMovementType == mapCameraMovement.XY) {
						mapCameraRotation.z = -player.transform.eulerAngles.y;
					} else if (mapCameraMovementType == mapCameraMovement.YZ) {
						mapCameraRotation.z = player.transform.eulerAngles.y - 90;
					}

					playerMapIcon.eulerAngles = mapCameraRotation;
				}
			} else {
				if (rotateMap && mapViewIn2dActive && !resetingCameraPosition) {
					mapCamera.transform.eulerAngles = Vector3.zero;
				}
			}
				
			//calculate the closest floor according to vertical distance with the player
			if (getClosestFloorToPlayerByDistance) {
				if (!mapOpened) {
					currentFloorIndex = getClosestFloorToPlayer ();

					if (previousCurrentFloorIndex != currentFloorIndex) {
						previousCurrentFloorIndex = currentFloorIndex;

						updateCurrentFloorEnabledState ();

						if (playerMapObjectInformation != null) {
							playerMapObjectInformation.setNewBuildingAndFloorIndex (currentBuildingIndex, currentFloorIndex);
						}
					}
				}
			}

			//if the map menu is opened, check the presses in the screen and enable the controls to interact with it
			if (mapOpened) {
				//check for touch input from the mouse or the finger
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

					//in the touch begin phase
					if (!movingMap) {
						//get a list with all the objects under mouse or the finger tap
						if (currentTouch.phase == TouchPhase.Began) {
							captureRaycastResults.Clear ();
							PointerEventData p = new PointerEventData (EventSystem.current);
							p.position = currentTouch.position;
							p.clickCount = i;
							p.dragging = false;
							EventSystem.current.RaycastAll (p, captureRaycastResults);

							bool mapIconFound = false;

							int temporalMapObjectsCount = mapObjects.Count;

							foreach (RaycastResult r in captureRaycastResults) {
								if (r.gameObject == mapRender.gameObject) {
									//check the current key pressed with the finger
									movingMap = true;
									beginTouchPosition = new Vector3 (currentTouch.position.x, currentTouch.position.y, 0);
								}

								for (int j = 0; j < temporalMapObjectsCount; j++) { 
									if (mapObjects [j].mapIcon.gameObject == r.gameObject) {
										getMapObjectInfo (j);

										mapIconFound = true;

										checkCurrentIconPressed (true, mapObjects [j].mapIcon);
									}
								}

								if (!mapIconFound) {
									checkCurrentIconPressed (false, null);
								}
							}
						}
					}

					//the current touch press is being moved
					if ((currentTouch.phase == TouchPhase.Moved || currentTouch.phase == TouchPhase.Stationary) && movingMap) {
						Vector3 globalTouchPosition = new Vector3 (currentTouch.position.x, currentTouch.position.y, 0);
						Vector3 differenceVector = globalTouchPosition - beginTouchPosition;

						if (differenceVector.sqrMagnitude > 1 * 1) {
							differenceVector.Normalize ();
						}

						beginTouchPosition = globalTouchPosition;
						Vector3 moveInput = Vector3.zero;

						if (mapViewIn2dActive) {
							//if (mapCameraMovementType == mapCameraMovement.XZ) {
							moveInput = differenceVector.y * mainMapCameraTransform.up + differenceVector.x * mainMapCameraTransform.right;	
//							} else if (mapCameraMovementType == mapCameraMovement.XY) {
//								moveInput = differenceVector.y * mainMapCameraTransform.forward + differenceVector.x * mainMapCameraTransform.right;	
//							} else if (mapCameraMovementType == mapCameraMovement.YZ) {
//								moveInput = differenceVector.y * mainMapCameraTransform.forward + differenceVector.x * mainMapCameraTransform.forward;	
//							}

							zoomAmountMultiplier = currentCameraSize / zoomWhenOpen;

							mapCamera.transform.position -= (mouseDragMapSpeed * zoomAmountMultiplier) * moveInput;
						} else {
//							moveInput = differenceVector.y * mapSystemPivotTransform.forward + differenceVector.x * mapSystemPivotTransform.right;	
							//							mapSystemPivotTransform.position -= moveInput * mouseDragMapSpeed;
						}
					}

					//if the touch ends, reset the rotation of the joystick, the current axis values and the zoom buttons positions
					if (currentTouch.phase == TouchPhase.Ended) {
						movingMap = false;
					}
				}

				//move the map using input from current control (keyboard or gamepad)
				axisValues = playerInput.getPlayerMovementAxis ();

				horizontalInput = axisValues.x;
				verticalInput = axisValues.y;

				if (playerInput.getPlayerRawMovementAxis () != Vector2.zero) {
					Vector3 moveInput = Vector3.zero;

					if (mapViewIn2dActive) {
						//if (mapCameraMovementType == mapCameraMovement.XZ) {
						moveInput = verticalInput * mainMapCameraTransform.up + horizontalInput * mainMapCameraTransform.right;	
//						} else if (mapCameraMovementType == mapCameraMovement.XY) {
//							moveInput = verticalInput * mainMapCameraTransform.forward + horizontalInput * mainMapCameraTransform.right;	
//						} else if (mapCameraMovementType == mapCameraMovement.YZ) {
//							moveInput = verticalInput * mainMapCameraTransform.forward + horizontalInput * mainMapCameraTransform.right;	
//						}

						mapCamera.transform.Translate (moveInput * keysDragMapSpeed);
					} else {
						moveInput = verticalInput * Vector3.forward + horizontalInput * Vector3.right;	
						mapSystemPivotTransform.Translate (moveInput * map3dPositionSpeed);
					}
				}

				//rotate the map with the mouse input
				if (!mapViewIn2dActive) {
					mouseValues = playerInput.getPlayerMouseAxis ();

					horizontalInput = mouseValues.x;
					verticalInput = mouseValues.y;

					if ((horizontalInput != 0 || verticalInput != 0) && movingMap) {

						currentLookAngle.x += horizontalInput;
						currentLookAngle.y -= verticalInput;

						currentLookAngle.x = Mathf.Clamp (currentLookAngle.x, rangeAngleY.x, rangeAngleY.y);

						currentLookAngle.y = Mathf.Clamp (currentLookAngle.y, rangeAngleX.x, rangeAngleX.y);

						currentPivotRotation = Quaternion.Euler (0, currentLookAngle.x, 0);

						currentCameraRotation = Quaternion.Euler (currentLookAngle.y, 0, 0);

						mapSystemPivotTransform.localRotation = Quaternion.Slerp (mapSystemPivotTransform.localRotation, currentPivotRotation, getDeltaTime () * map3dRotationSpeed);
						mapSystemCameraTransform.localRotation = Quaternion.Slerp (mapSystemCameraTransform.localRotation, currentCameraRotation, getDeltaTime () * map3dRotationSpeed);
					}

					if (use3dMeshForPlayer) {
						if (player3dMesh != null) {
							player3dMesh.transform.position = player.transform.position;
						}
					}
				}

				//check input used to make zoom or change between floors
				if (currentMouseWheelPositiveValue) {
					zoomInEnabled ();

					usingMouseWheelZoomIn = true;
					usingMouseWheelZoomPreviouosly = true;
				} else {
					usingMouseWheelZoomIn = false;
				}

				if (currentMouseWheelNegativeValue) {
					zoomOutEnabled ();

					usingMouseWheelZoomOut = true;
					usingMouseWheelZoomPreviouosly = true;
				} else {
					usingMouseWheelZoomOut = false;
				}
					
				currentMouseWheelPositiveValue = false;
				currentMouseWheelNegativeValue = false;

				if (usingMouseWheelZoomPreviouosly && !usingMouseWheelZoomOut && !usingMouseWheelZoomIn) {
					zoomInDisabled ();
					zoomOutDisabled ();
					usingMouseWheelZoomPreviouosly = false;
				}

				if (!changingCameraSize) {

					if (!usingScrollbarZoom) {
						if (mapViewIn2dActive) {
							currentCameraSize = currentOrthographicSize;
						} else {
							currentCameraSize = currentFieldOfView;
						}

						if (mapViewIn2dActive) {
							zoomScrollbar.value = currentCameraSize / (maxZoom - minZoom);
						} else {
							zoomScrollbar.value = currentCameraSize / (maxZoom3d - minZoom3d);
						}
					}

					if (zoomingIn) {
						if (mapViewIn2dActive) {
							currentCameraSize -= getDeltaTime () * zoomSpeed;

							if (currentCameraSize < minZoom) {
								currentCameraSize = minZoom;
							} 
						} else {
							currentCameraSize -= getDeltaTime () * zoomSpeed3d;

							if (currentCameraSize < minZoom3d) {
								currentCameraSize = minZoom3d;
							} 
						}
					}

					if (zoomingOut) {
						if (mapViewIn2dActive) {
							currentCameraSize += getDeltaTime () * zoomSpeed;

							if (currentCameraSize > maxZoom) {
								currentCameraSize = maxZoom;
							} 
						} else {
							currentCameraSize += getDeltaTime () * zoomSpeed3d;

							if (currentCameraSize > maxZoom3d) {
								currentCameraSize = maxZoom3d;
							} 
						}
					}

					if (mapViewIn2dActive) {
						assignMapCameraOrthographicValue (Mathf.Lerp (currentOrthographicSize, currentCameraSize, getDeltaTime () * zoomSpeed));
					} else {
						assignMapCameraFovValue (Mathf.Lerp (currentFieldOfView, currentCameraSize, getDeltaTime () * zoomSpeed3d));
					}
				}

				currentFloorNumberText.text = (currentFloorNumber).ToString ();
			}
		}
	}

	public void checkUpdateCompass ()
	{
		if (compassEnabled && !mapOpened) {
			updateCompassIcons ();
		}
	}

	public void checkCurrentIconPressed (bool state, Transform mapIconTransform)
	{
		if (useCurrentMapIconPressed) {
			if (state) {
				currenMapIconPressed.transform.SetParent (mapIconTransform);
				currenMapIconPressed.transform.localPosition = Vector3.zero;
			}

			if (currenMapIconPressed.activeSelf != state) {
				currenMapIconPressed.SetActive (state);
			}
		}
	}

	int compassIconListCount;

	Vector3 iconTargetTransformPosition;

	Vector3 vectorUp = Vector3.up;

	Quaternion quaternionIdentity = Quaternion.identity;

	Vector3 compassPlayerPosition;

	Vector3 currentPlayerDirection;

	public void updateCompassIcons ()
	{
		//set the compass values according to player orientation
		if (compassEnabled) {
			compassPlayerPosition = player.transform.position;

			compassDirections.SetPositionAndRotation (compassPlayerPosition, quaternionIdentity);

			playerCompassPosition = new Vector3 (compassPlayerPosition.x, 0, compassPlayerPosition.z);

			bool firstPersonActive = playerControllerManager.isPlayerOnFirstPerson ();
				
			bool useCameraDirection = false;

			if (firstPersonActive) {
				if (!usePlayerTransformOrientationOnCompassOnFirstPerson) {
					useCameraDirection = true;
				}
			} else {
				if (playerControllerManager.isLockedCameraStateActive ()) {
					if (!usePlayerTransformOrientationOnCompassOnLockedCamera) {
						useCameraDirection = true;
					}
				} else {
					if (!usePlayerTransformOrientationOnCompassOnThirdPerson) {
						useCameraDirection = true;
					}
				}
			}

			if (useCameraDirection) {
				currentPlayerDirection = playerControllerManager.getPlayerCameraGameObject ().transform.forward;
			} else {
				currentPlayerDirection = player.transform.forward;
			}

			compassIconListCount = compassIconList.Count;

			for (int i = 0; i < compassIconListCount; i++) {

				currentCompassIconInfo = compassIconList [i];

				currentIconTargetTransform = currentCompassIconInfo.iconTargetTransform;

				if (currentIconTargetTransform != null) {
					iconTargetTransformPosition = currentIconTargetTransform.position;

					compassTargetPosition = new Vector3 (iconTargetTransformPosition.x, 0, iconTargetTransformPosition.z);

					compassDirection = compassTargetPosition - playerCompassPosition;
					compassDirection = compassDirection / compassDirection.magnitude;

					float directionDistance = GKC_Utils.distance (compassDirection, currentPlayerDirection);

					if (Mathf.Abs (currentCompassIconInfo.lastIconAngle - directionDistance) > 0.01f) {

						currentCompassIconInfo.lastIconAngle = directionDistance;

						currentCompassTargetRotation = Vector3.SignedAngle (compassDirection, currentPlayerDirection, vectorUp);

						float angleABS = Mathf.Abs (currentCompassTargetRotation);

						float angleDifference = Mathf.Abs (currentCompassIconInfo.currentCompassTargetRotation - angleABS);

						if (angleDifference > minDifferenceToUpdateDistanceToObject || !currentCompassIconInfo.firstValueAssigned) {

							currentCompassIconInfo.firstValueAssigned = true;

							currentCompassIconInfo.currentCompassTargetRotation = angleABS;

							compassTargetRotation = currentCompassTargetRotation;

							if (compassTargetRotation > 360) {
								compassTargetRotation = compassTargetRotation % 360;
							}

							reversedCompassTargetRotation = compassTargetRotation;

							if (reversedCompassTargetRotation > 180) {
								reversedCompassTargetRotation = reversedCompassTargetRotation - 360;
							}

							if (currentCompassTargetRotation < 0) {
								currentCompassOrientation = reversedCompassTargetRotation;
							} else {
								currentCompassOrientation = compassTargetRotation;
							}

							Vector2 newPosition = new Vector2 ((-currentCompassOrientation * 2 + currentCompassTargetRotation) * compassScale, currentCompassIconInfo.verticalOffset);

							if (currentCompassIconInfo.useCompassClamp) {
								newPosition.x = Mathf.Clamp (newPosition.x, maximumLeftDistance, maximumRightDistance);
							}

							currentCompassIconInfo.iconRectTransform.anchoredPosition = newPosition;
						}
					}
				} else {
					removeCompassIcon (currentCompassIconInfo.iconGameObject);

					Destroy (currentCompassIconInfo.iconGameObject);
				
					return;
				}
			}
		}
	}

	void removeCompassIcon (GameObject iconToRemove)
	{
		for (int i = 0; i < compassIconList.Count; i++) {
			if (compassIconList [i].iconGameObject == iconToRemove) {
				compassIconList.RemoveAt (i);

				return;
			}
		}
	}

	public void checkCompassIcon (int index)
	{
		mapObjectInfo temporalMapObjectInfo = mapObjects [index];

		if (temporalMapObjectInfo.compassIcon != null) {
			removeCompassIcon (temporalMapObjectInfo.compassIcon.iconGameObject);

			Destroy (temporalMapObjectInfo.compassIcon.iconGameObject);
		}
	}

	public compassIconInfo addCompassIcon (GameObject objectToFollow, GameObject newCompassIcon, float verticalOffset, bool useCompassClamp)
	{
		compassIconInfo newCompassIconInfo = new compassIconInfo ();

		newCompassIconInfo.Name = objectToFollow.name;
		newCompassIconInfo.iconGameObject = newCompassIcon;
		newCompassIconInfo.iconTransform = newCompassIcon.transform;

		newCompassIconInfo.iconRectTransform = newCompassIcon.GetComponent<RectTransform> ();
		newCompassIconInfo.iconTargetTransform = objectToFollow.transform;
		newCompassIconInfo.verticalOffset = verticalOffset;

		newCompassIconInfo.useCompassClamp = useCompassClamp;

		compassIconList.Add (newCompassIconInfo);

		return newCompassIconInfo;
	}
		
	//MANAGE MAP CAMERA STATES AND VALUES
	public void setCameraSize (float value)
	{
		if (cameraSizeCoroutine != null) {
			StopCoroutine (cameraSizeCoroutine);
		}

		cameraSizeCoroutine = StartCoroutine (setCameraSizeCoroutine (value));
	}

	IEnumerator setCameraSizeCoroutine (float value)
	{
		changingCameraSize = true;

		if (miniMapWindowEnabledInGame) {
			float t = 0.0f;

			while (t < 1) {
				t += getDeltaTime ();

				if (mapViewIn2dActive) {
					if (currentOrthographicSize != value) {
						assignMapCameraOrthographicValue (Mathf.Lerp (currentOrthographicSize, value, t));
					}
				} else {
					if (currentFieldOfView != value) {
						assignMapCameraFovValue (Mathf.Lerp (currentFieldOfView, value, t));
					}
				}

				yield return null;
			}
		} else {
			if (mapViewIn2dActive) {
				assignMapCameraOrthographicValue (value);
			} else {
				assignMapCameraFovValue (value);
			}
		}

		changingCameraSize = false;
	}

	public void assignMapCameraFovValue (float value)
	{
		if (viewTo3dHasBeenActive) {
			if (!mapViewIn2dActive) {
				if (currentFieldOfView != value) {
					mainMapCamera.projectionMatrix = Matrix4x4.Perspective (value, aspect, near, far);
				}
			}
		} else {
			mainMapCamera.fieldOfView = value;
		}

		currentFieldOfView = value;
	}

	public void assignMapCameraOrthographicValue (float value)
	{
		if (viewTo3dHasBeenActive) {
			if (mapViewIn2dActive) {
				if (currentOrthographicSize != value) {
					mainMapCamera.projectionMatrix = Matrix4x4.Ortho (-value * aspect, value * aspect, -value, value, near, far);
				}
			}
		} else {
			mainMapCamera.orthographicSize = value;
		}

		currentOrthographicSize = value;
	}

	public void setNewObjectToFollow (GameObject newObject)
	{
		player = newObject;
	}

	//MANAGE MAP CAMERA FOR 2D AND 3D VIEW
	public void set2dOr3ddMapView (bool state)
	{
		if (!map3dEnabled || mapViewIn2dActive == state) {
			return;
		}

		viewTo3dHasBeenActive = true;

		mapViewIn2dActive = state;
		mapCreatorManager.enableOrDisabled2dMap (mapViewIn2dActive, currentBuildingIndex);
		mapCreatorManager.enableOrDisabled3dMap (!mapViewIn2dActive, currentBuildingIndex);
	
		if (mapViewIn2dActive) {
			ortho = Matrix4x4.Ortho (-zoomWhenOpen * aspect, zoomWhenOpen * aspect, -zoomWhenOpen, zoomWhenOpen, near, far);

			resetCameraPivotRotation (Vector3.zero, Vector3.zero, true);

			updateCurrentFloorNumber ();
		} else {

			perspective = Matrix4x4.Perspective (zoomWhenOpen3d, aspect, near, far);

			BlendToMatrix (perspective, maxTimeBetweenTransition);

			mapCreatorManager.setFloorActiveState (currentBuildingIndex, currentFloorIndex, false);
			//floors [currentFloorIndex].floor.gameObject.SetActive (false);
		}

		if (setColorOnCurrent3dMapPart) {
			mapCreatorManager.set3dMapPartMeshColor (colorOnCurrent3dMapPart, mapViewIn2dActive, currentBuildingIndex, currentFloorIndex, currentMapPartIndex);
		}

		enableOrDisableAllFloorsFoundMapIcon (!mapViewIn2dActive);

		if (use3dMeshForPlayer) {
			player3dMesh.SetActive (!mapViewIn2dActive);
		}
	}

	public void setCurrentMapPartIndex (int newValue)
	{
		currentMapPartIndex = newValue;
	}

	public void setQuick2dMap ()
	{
		if (mapViewIn2dActive) {
			return;
		}

		mapViewIn2dActive = true;
		mapCreatorManager.enableOrDisabled3dMap (!mapViewIn2dActive, currentBuildingIndex);

		ortho = Matrix4x4.Ortho (-zoomWhenClose * aspect, zoomWhenClose * aspect, -zoomWhenClose, zoomWhenClose, near, far);

		mapSystemPivotTransform.localRotation = Quaternion.identity;
		mapSystemCameraTransform.localRotation = Quaternion.identity;

		mainMapCamera.projectionMatrix = ortho;

		resetMapCamera3dValues ();

		enableOrDisableAllFloorsFoundMapIcon (!mapViewIn2dActive);

		if (use3dMeshForPlayer) {
			player3dMesh.SetActive (!mapViewIn2dActive);
		}
	}

	public void BlendToMatrix (Matrix4x4 targetMatrix, float duration)
	{
		if (cameraProjectionChangeCoroutine != null) {
			StopCoroutine (cameraProjectionChangeCoroutine);
		}

		cameraProjectionChangeCoroutine = StartCoroutine (LerpFromTo (mainMapCamera.projectionMatrix, targetMatrix, duration));
	}

	IEnumerator LerpFromTo (Matrix4x4 src, Matrix4x4 dest, float duration)
	{
		changingCameraSize = true;

		for (float t = 0; t < duration;) {
			t += getDeltaTime () * transtionTo3dSpeed;

			mainMapCamera.projectionMatrix = MatrixLerp (src, dest, t);

			yield return null;
		}

		mainMapCamera.projectionMatrix = dest;

		if (!mapViewIn2dActive) {
			mapSystemPivotTransform.position = mapCamera.transform.position - vectorUp * cameraOffset.y;

			mapCamera.transform.SetParent (mapSystemCameraTransform);

			Vector3 mapCameraTransformEulerRotation = new Vector3 (inital3dCameraRotation.x, 0, 0);
			Vector3 mapPivotTransformEulerRotation = new Vector3 (0, inital3dCameraRotation.y, 0);

			resetCameraPivotRotation (mapPivotTransformEulerRotation, mapCameraTransformEulerRotation, false);
		}

		changingCameraSize = false;
	}

	public static Matrix4x4 MatrixLerp (Matrix4x4 from, Matrix4x4 to, float time)
	{
		Matrix4x4 ret = new Matrix4x4 ();

		for (int i = 0; i < 16; i++) {
			ret [i] = Mathf.Lerp (from [i], to [i], time);
		}

		return ret;
	}

	public void resetMapCamera3dValues ()
	{
		Vector3 newPosition = player.transform.position;

		mapCamera.transform.rotation = Quaternion.identity;
		mapCamera.transform.SetParent (mapCreatorManager.transform);

		currentLookAngle = Vector2.zero;
		currentPivotRotation = Quaternion.identity;
		currentCameraRotation = Quaternion.identity;
		mapSystemPivotTransform.localRotation = Quaternion.identity;
		mapSystemCameraTransform.localRotation = Quaternion.identity;
	}

	public void resetCameraPivotRotation (Vector3 pivotEulerRotation, Vector3 cameraEulerRotation, bool resetCameraValues)
	{
		if (cameraPivotResetRotationCoroutine != null) {
			StopCoroutine (cameraPivotResetRotationCoroutine);
		}

		cameraPivotResetRotationCoroutine = StartCoroutine (resetCameraPivotRotationCoroutine (pivotEulerRotation, cameraEulerRotation, resetCameraValues));
	}

	IEnumerator resetCameraPivotRotationCoroutine (Vector3 pivotEulerRotation, Vector3 cameraEulerRotation, bool resetCameraValues)
	{
		resetingCameraPosition = true;

		Quaternion pivotTargetRotation = Quaternion.Euler (pivotEulerRotation);
		Quaternion cameraTargetRotation = Quaternion.Euler (cameraEulerRotation);

		float t = 0;

		while (t < 0.5f || mapSystemPivotTransform.localRotation != pivotTargetRotation || mapSystemCameraTransform.localRotation != cameraTargetRotation) {
			t += getDeltaTime ();

			mapSystemPivotTransform.localRotation = Quaternion.Slerp (mapSystemPivotTransform.localRotation, pivotTargetRotation, t * reset3dCameraSpeed);
			mapSystemCameraTransform.localRotation = Quaternion.Slerp (mapSystemCameraTransform.localRotation, cameraTargetRotation, t * reset3dCameraSpeed);

			yield return null;
		}

		currentLookAngle.x = pivotEulerRotation.y;

		currentLookAngle.y = cameraEulerRotation.x;

		if (resetCameraValues) {
			resetMapCamera3dValues ();
		}

		if (mapViewIn2dActive) {
			BlendToMatrix (ortho, maxTimeBetweenTransition);
		}

		resetingCameraPosition = false;
	}

	public void recenterCameraPosition ()
	{
		if (resetCameraPositionCoroutine != null) {
			StopCoroutine (resetCameraPositionCoroutine);
		}

		resetCameraPositionCoroutine = StartCoroutine (recenterCameraPositionCoroutine ());
	}

	IEnumerator recenterCameraPositionCoroutine ()
	{
		Vector3 targetPosition = player.transform.position;

		if (mapViewIn2dActive) {
			targetPosition.y += cameraOffset.y;
		}

		float t = 0;
		while (t < 1 && GKC_Utils.distance (mapCamera.transform.position, targetPosition) > 0.1f) {
			t += getDeltaTime ();

			if (mapViewIn2dActive) {
				mapCamera.transform.position = Vector3.Lerp (mapCamera.transform.position, targetPosition, t * recenterCameraSpeed);
			} else {
				mapSystemPivotTransform.position = Vector3.Lerp (mapSystemPivotTransform.position, targetPosition, t * recenterCameraSpeed);
			}

			yield return null;
		}
	}

	//MANAGE MAP CAMERA ZOOM
	public void zoomInEnabled ()
	{
		zoomingIn = true;
	}

	public void zoomInDisabled ()
	{
		zoomingIn = false;
	}

	public void zoomOutEnabled ()
	{
		zoomingOut = true;
	}

	public void zoomOutDisabled ()
	{
		zoomingOut = false;
	}

	bool usingScrollbarZoom;

	public void setUsingScrollbarZoomState (bool state)
	{
		usingScrollbarZoom = state;
	}

	public void setZoomByScrollBar (Scrollbar mainZoomScrollbar)
	{
		float scrollbarValue = mainZoomScrollbar.value;
		float currentMaxZoom = maxZoom3d;

		if (mapViewIn2dActive) {
			currentMaxZoom = maxZoom;
		}

		float currentMinZoom = minZoom3d;

		if (mapViewIn2dActive) {
			currentMinZoom = minZoom;
		}

		currentCameraSize = scrollbarValue * maxZoom;

		currentCameraSize = Mathf.Clamp (currentCameraSize, currentMinZoom, currentMaxZoom);
	}

	//MANAGE MAP ICONS ELEMENTS
	//add a new object which will be visible in the radar. It can be an enemy, and ally or a target
	public void addMapObject (bool visibleInAllBuildings, bool visibleInAllFloors, bool calculateFloorAtStart, GameObject obj, string type, Vector3 offset, int ID, 
	                          int mapPartIndex, int buildingIndex, float extraIconSizeOnMap, bool followCameraRotation, bool followObjectRotation, 
	                          bool setCustomCompassSettings, bool useCompassIcon, GameObject compassIconPrefab, float verticalOffset)
	{
		mapObjectInfo newMapObject = new mapObjectInfo ();

		newMapObject.name = type;
		newMapObject.buildingIndex = buildingIndex;
		newMapObject.extraIconSizeOnMap = extraIconSizeOnMap;
		newMapObject.followCameraRotation = followCameraRotation;
		newMapObject.followObjectRotation = followObjectRotation;

		newMapObject.offset = offset;

		newMapObject.visibleAllBuildings = visibleInAllBuildings;
		newMapObject.visibleAllFloors = visibleInAllFloors;

		bool alreadyAdded = false;

		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.mapObject == obj) {
				alreadyAdded = true;

				newMapObject = temporalMapObjectInfo;

				if (newMapObject != null && newMapObject.mapIcon != null) {
					Destroy (newMapObject.mapIcon.gameObject);
				}
			}
		}
			
		bool iconFound = false;

		int mapIconTypesCount = mapIconTypes.Count;

		for (int i = 0; i < mapIconTypesCount; i++) {
			mapIconType currentMapIconType = mapIconTypes [i];

			if (currentMapIconType.typeName == type) {

//				print ("adding " + mapIconTypes [i].typeName + " type to " + obj.name);

				GameObject icon = (GameObject)Instantiate (currentMapIconType.icon.gameObject, mapWindow.transform.position, Quaternion.identity, mapWindow.transform);

				icon.transform.localScale = Vector3.one;

				icon.name = icon.name.Replace ("Clone", obj.name);

				newMapObject.typeName = type;
				newMapObject.mapIcon = icon.GetComponent<RectTransform> ();
				newMapObject.isActivated = true;
				newMapObject.mapPartOwnerFound = true;

				if (alreadyAdded) {
					if (useTextInIcons) {
						GameObject newMapTextIcon = addMapTextIcon (newMapObject);
						newMapObject.textName = newMapTextIcon.GetComponent<RectTransform> ();
					}
				}

				iconFound = true;

				if (setCustomCompassSettings && useCompassIcon) {
					GameObject newCompassIcon = (GameObject)Instantiate (compassIconPrefab, Vector3.zero, Quaternion.identity, compassElementsParent.transform);

					newCompassIcon.transform.localScale = Vector3.one;

					newMapObject.compassIcon = addCompassIcon (obj, newCompassIcon, verticalOffset, true);
				} else {
					if (currentMapIconType.useCompassIcon) {
						GameObject newCompassIcon = (GameObject)Instantiate (currentMapIconType.compassIconPrefab, Vector3.zero, 
							                            Quaternion.identity, compassElementsParent.transform);
						
						newCompassIcon.transform.localScale = Vector3.one;

						newMapObject.compassIcon = addCompassIcon (obj, newCompassIcon, currentMapIconType.verticalOffset, true);
					}
				}
			}
		}

		if (!iconFound) {
			if (showWarningMessages) {
				print ("WARNING: the icon type " + type + " is not configured in the map icon manager, check the configuration in the map system inspector");
			}
		}

		if (!alreadyAdded) {
			newMapObject.mapObject = obj;
			bool calculateFloor = false;

			mapObjectInformation currentMapObjectInformation = obj.GetComponent<mapObjectInformation> ();

			if (currentMapObjectInformation != null) {
				if (calculateFloorAtStart) {
					calculateFloor = true;
				} else {
					if (!newMapObject.visibleAllFloors) {
						if (buildingIndex < buildingList.Count && currentMapObjectInformation.floorIndex < buildingList [buildingIndex].floors.Count) {
							newMapObject.floorNumber = buildingList [buildingIndex].floors [currentMapObjectInformation.floorIndex].floorNumber;
							newMapObject.floorIndex = currentMapObjectInformation.floorIndex;

							newMapObject.hasFloorAssigned = true;
						} else {
							if (showWarningMessages) {
								print ("WARNING: map object for " + currentMapObjectInformation.gameObject.name + " hasn't been configured properly, please check its settings");
							}
						}
					}
				}

				newMapObject.name = currentMapObjectInformation.getMapObjectName ();

				if (currentMapObjectInformation.useEventsOnChangeFloor || currentMapObjectInformation.canChangeBuildingAndFloor) {
					newMapObject.hasMapObjectInformation = true;
					newMapObject.mapObjectInformationManager = currentMapObjectInformation;

					if (currentMapObjectInformation.useEventsOnChangeFloor) {
						newMapObject.useEventsOnChangeFloor = true;
					}

					if (currentMapObjectInformation.canChangeBuildingAndFloor) {
						currentMapObjectInformation.setCurrentMapObjectInfo (newMapObject);
					}
				}
			} else {
				calculateFloor = true;
			}

			if (calculateFloor) {
				float distance = Mathf.Infinity;

				if (buildingIndex < buildingList.Count) {
					List<floorInfo> currentFloorsToCheck = buildingList [buildingIndex].floors;

					int currentFloorsToCheckCount = currentFloorsToCheck.Count;

					for (int i = 0; i < currentFloorsToCheckCount; i++) {
						float currentDistance = Mathf.Abs (newMapObject.mapObject.transform.position.y - currentFloorsToCheck [i].floor.position.y);

						if (currentDistance < distance) {
							distance = currentDistance;
							newMapObject.floorNumber = currentFloorsToCheck [i].floorNumber;
							newMapObject.floorIndex = i;
							newMapObject.hasFloorAssigned = true;
						}
					}
				} else {
					if (showWarningMessages) {
						print ("WARNING: the building index found is bigger than the actual list of buildings, check the settings on the map are correct");
					}
				}
			}

			if (useTextInIcons && iconFound) {
				GameObject newMapTextIcon = addMapTextIcon (newMapObject);
				newMapObject.textName = newMapTextIcon.GetComponent<RectTransform> ();
			}

			newMapObject.ID = ID;
			newMapObject.mapPartIndex = mapPartIndex;

			mapObjects.Add (newMapObject);
		}
	}

	//remove a dead enemy, ally or a reached target
	public void removeMapObject (GameObject obj, bool isPathElement)
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.mapObject == obj) {
				bool removeComponentWhenObjectiveReached = true;

				if (!isPathElement) {
					mapObjectInformation currentMapObjectInformation = obj.GetComponent<mapObjectInformation> ();

					if (currentMapObjectInformation != null) {
						removeComponentWhenObjectiveReached = currentMapObjectInformation.removeComponentWhenObjectiveReachedEnabled ();

						if (removeComponentWhenObjectiveReached) {

							checkCurrentMapIconPressedParent ();

							Destroy (currentMapObjectInformation);
						}
					}
				}

				checkCurrentMapIconPressedParent ();

				if (temporalMapObjectInfo.mapIcon != null) {
					Destroy (temporalMapObjectInfo.mapIcon.gameObject);
				}

				if (temporalMapObjectInfo.typeName.Equals ("Mark")) {
					Destroy (temporalMapObjectInfo.mapObject);	
				}

				checkCompassIcon (i);

				mapObjects.RemoveAt (i);

				if (!removeComponentWhenObjectiveReached) {

					if (screenObjectivesManager != null) {
						screenObjectivesManager.removeGameObjectFromList (obj);
					}
				}

				return;
			}
		}
	}

	public void setnewBuilingAndFloorIndexToMapObject (mapObjectInfo mapObjectInfoToModify, int newBuildingIndex, int newFloorIndex)
	{
		mapObjectInfoToModify.buildingIndex = newBuildingIndex;

		mapObjectInfoToModify.floorIndex = newFloorIndex;

		if (newBuildingIndex >= 0 && newFloorIndex >= 0 &&
		    buildingList.Count > newBuildingIndex && buildingList [newBuildingIndex].floors.Count > newFloorIndex) {

			mapObjectInfoToModify.floorNumber = buildingList [newBuildingIndex].floors [newFloorIndex].floorNumber;
		}

		if (mapObjectInfoToModify.hasMapObjectInformation) {
			mapObjectInfoToModify.mapObjectInformationManager.checkEventOnChangeFloor (currentBuildingIndex, currentFloorIndex);
		}
	}

	public GameObject addMapTextIcon (mapObjectInfo newMapObject)
	{
		if (newMapObject == null) {
			print ("null");
		}

		GameObject textIcon = (GameObject)Instantiate (mapObjectTextIcon, newMapObject.mapIcon.position, Quaternion.identity, newMapObject.mapIcon);

		textIcon.transform.localScale = Vector3.one;

		if (textIcon.activeSelf) {
			textIcon.SetActive (false);
		}

		string newMapTextIconName = newMapObject.name;

		if (newMapTextIconName != null) {
			newMapTextIconName.Replace (" ", "\n");
		}

		Text currentTextIcon = textIcon.GetComponent<Text> ();
		currentTextIcon.text = newMapTextIconName;
		currentTextIcon.raycastTarget = false;
		currentTextIcon.color = mapObjectTextIconColor;
		currentTextIcon.fontSize = mapObjectTextSize;

		return textIcon;
	}

	public void placeMark ()
	{
		if (currentFloorIndex > -1 && floors.Count > 0) {
			GameObject newMark = (GameObject)Instantiate (markPrefab, Vector3.zero, Quaternion.identity);

			newMark.transform.position = new Vector3 (mapCamera.transform.position.x, floors [currentFloorIndex].floor.position.y, mapCamera.transform.position.z);
			newMark.name = "Mark_" + (markList.Count + 1).ToString ();

			markInfo newMarkInfo = new markInfo ();
			newMarkInfo.name = newMark.name;
			newMarkInfo.mapObject = newMark;
			newMarkInfo.index = markList.Count + 1;

			markList.Add (newMarkInfo);

			mapObjectInformation mapObjectInformationManager = newMark.GetComponent<mapObjectInformation> ();

			if (setMarkOnCurrenBuilding) {
				mapObjectInformationManager.setNewBuildingIndex (currentBuildingIndex);
			}

			if (setMarkOnCurrentFloor) {
				mapObjectInformationManager.setNewFloorIndex (currentFloorIndex);
			}

			mapObjectInformationManager.createMapIconInfo ();
		}
	}

	public void removeMark ()
	{
		if (currentMark != null) {
			int markListCount = markList.Count;

			bool markFound = false;

			for (int k = 0; k < markListCount; k++) { 
				if (!markFound && markList [k].mapObject == currentMark.mapObject) {
					markList.RemoveAt (k);

					markFound = true;
				}
			}

			removeMapObject (currentMark.mapObject, false);

			if (screenObjectivesManager != null) {
				screenObjectivesManager.removeGameObjectFromList (currentMark.mapObject);
			}

			checkCurrentMapIconPressedParent ();

			Destroy (currentMark.mapObject);

			Destroy (currentMark.mapIcon.gameObject);

			if (currentMark.compassIcon != null) {
				removeCompassIcon (currentMark.compassIcon.iconGameObject);

				Destroy (currentMark.compassIcon.iconGameObject);
			}

			mapObjects.Remove (currentMark);

			currentMark = null;

			removeMarkButtonImage.color = disabledRemoveMarkColor;
		}
	}

	public void changeMapObjectIconFloor (GameObject objectToSearch, int newFloorIndex)
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];
				
			if (temporalMapObjectInfo.mapObject == objectToSearch) {
				temporalMapObjectInfo.floorNumber = newFloorIndex;
			}
		}
	}

	public void changeMapObjectIconFloorByPosition (GameObject objectToSearch)
	{
		if (!mapEnabled) {
			return;
		}

		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.mapObject != null && temporalMapObjectInfo.mapObject == objectToSearch) {

				int newFloorNumber = getClosestFloorToMapObject (temporalMapObjectInfo.mapObject.transform);

				if (newFloorNumber > -1) {
					temporalMapObjectInfo.floorNumber = floors [newFloorNumber].floorNumber;
				}
			}
		}
	}

	public int getClosestFloorToMapObject (Transform mapObjectTransform)
	{
		if (mapObjectTransform == null) {
			if (showWarningMessages) {
				print ("WARNING: floor to check sent to the map is empty, make sure the map system is properly configured");
			}

			return -1;
		}

		int index = -1;
		float distance = Mathf.Infinity;

		for (int i = 0; i < floors.Count; i++) {
			float currentDistance = Mathf.Abs (mapObjectTransform.position.y - floors [i].floor.position.y);

			if (currentDistance < distance) {
				distance = currentDistance;
				index = i;
			}
		}

		return index;
	}

	public int getIconTypeIndexByName (string iconTypeName)
	{
		int mapIconTypesCount = mapIconTypes.Count;

		for (int i = 0; i < mapIconTypesCount; i++) {
			if (mapIconTypes [i].typeName.Equals (iconTypeName)) {
				return i;
			}
		}

		return -1;
	}


	//Functions to manage input
	public void inputOpenOrCloseMapMenu ()
	{
		if (mapEnabled) {
			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			openOrCloseMap (!mapOpened);
		}
	}

	public void inputNextFloor ()
	{
		if (mapEnabled && mapOpened) {
			checkNextFloor ();
		}
	}

	public void inputPreviousFloor ()
	{
		if (mapEnabled && mapOpened) {
			checkPrevoiusFloor ();
		}
	}

	public void inputZoomIn ()
	{
		if (mapEnabled && mapOpened) {
			currentMouseWheelPositiveValue = true;
		}
	}

	public void inputZoomOut ()
	{
		if (mapEnabled && mapOpened) {
			currentMouseWheelNegativeValue = true;
		}
	}


	public void enableOrDisableAllMapIconType (Slider iconSlider)
	{
		bool value = true;

		if (iconSlider.value == 0) {
			value = false;
		} 

		string typeName = "";

		int glossaryListCount = glossaryList.Count;

		for (int i = 0; i < glossaryListCount; i++) {
			typeName = glossaryList [i].typeName;
			enableOrDisableMapIconTypeByName (value, typeName);
		}
	}

	public void enableOrDisableMapIconType (Slider iconSlider)
	{
		bool value = true;

		if (iconSlider.value == 0) {
			value = false;
		} 

		string typeName = "";

		int glossaryListCount = glossaryList.Count;

		for (int i = 0; i < glossaryListCount; i++) {
			if (glossaryList [i].slider == iconSlider) {
				typeName = glossaryList [i].typeName;
			}
		}

		enableOrDisableMapIconTypeByName (value, typeName);
	}

	public void enableOrDisableMapIconTypeByName (bool value, string typeName)
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.typeName.Equals (typeName)) {
				if (!value || temporalMapObjectInfo.mapPartOwnerFound) {
					temporalMapObjectInfo.isActivated = value;
				}
			}
		}

		int sliderValue = 0;

		if (value) {
			sliderValue = 1;
		} 

		int glossaryListCount = glossaryList.Count;

		for (int i = 0; i < glossaryListCount; i++) {
			if (glossaryList [i].typeName.Equals (typeName)) {
				glossaryList [i].slider.value = sliderValue;
			}
		}
	}

	public void enableOrDisableAllFloorsFoundMapIcon (bool value)
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.mapPartOwnerFound && temporalMapObjectInfo.buildingIndex == currentBuildingIndex) {
				temporalMapObjectInfo.canBeShowTemporaly = value;
			}
		}
	}

	public void enableOrDisableSingleMapIconByID (int ID, bool value)
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.ID == ID) {
				temporalMapObjectInfo.isActivated = value;
				temporalMapObjectInfo.mapPartOwnerFound = value;
			}
		}
	}

	public void enableOrDisableSingleMapIconByMapPartIndex (int mapPartBuildingIndex, int mapPartIndex, int mapFloorIndex, bool value)
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.mapPartIndex == mapPartIndex &&
			    temporalMapObjectInfo.floorIndex == mapFloorIndex &&
			    temporalMapObjectInfo.buildingIndex == mapPartBuildingIndex) {

				temporalMapObjectInfo.isActivated = value;
				temporalMapObjectInfo.mapPartOwnerFound = value;
			}
		}
	}

	public void checkEventOnChangeFloorForMapObjectInformation ()
	{
		int temporalMapObjectsCount = mapObjects.Count;

		for (int i = 0; i < temporalMapObjectsCount; i++) {
			mapObjectInfo temporalMapObjectInfo = mapObjects [i];

			if (temporalMapObjectInfo.useEventsOnChangeFloor && temporalMapObjectInfo.hasMapObjectInformation) {
				if (temporalMapObjectInfo.mapObjectInformationManager != null) {
					temporalMapObjectInfo.mapObjectInformationManager.checkEventOnChangeFloor (currentBuildingIndex, currentFloorIndex);
				}
			}
		}
	}

	public void enableOrDisableTextInIcons (Slider textInIconsSlider)
	{
		if (textInIconsSlider.value == 0) {
			useTextInIcons = false;
		} else {
			useTextInIcons = true;
		}
	}

	public void getMapObjectInfo (int mapObjectIndex)
	{
		mapObjectInfo temporalMapObjectInfo = mapObjects [mapObjectIndex];

		mapObjectInformation currentMapObjectInformation = temporalMapObjectInfo.mapObject.GetComponent<mapObjectInformation> ();

		if (currentMapObjectInformation != null) {
			mapObjectNameField.text = currentMapObjectInformation.getMapObjectName ();
			mapObjectInfoField.text = currentMapObjectInformation.description;

			if (currentMapObjectInformation.typeName.Equals (beaconIconTypeName)) {
				quickTravelButtonImage.color = originalQuickTravelColor;

				currentQuickTravelStation = temporalMapObjectInfo.mapObject;
			} else {
				quickTravelButtonImage.color = disabledQuickTravelColor;
			}

			if (temporalMapObjectInfo.typeName.Equals (markIconTypeName)) {
				currentMark = temporalMapObjectInfo;

				removeMarkButtonImage.color = originalRemoveMarkColor;
			}
		} else {
			removeMapObjectInfo ();
		}
	}

	//MANAGE OTHER MAP ELEMENTS
	public void assignMapWindowBorder ()
	{
		if (miniMapWindowWithMask && mapOpened) {
			mapWindowBorder = mapWindowMask;
		} else {
			mapWindowBorder = mapWindow;
		}
	}

	public void activateQuickTravel ()
	{
		if (currentQuickTravelStation != null) {
			quickTravelStationSystem currentQuickTravelStationSystem = currentQuickTravelStation.GetComponent<quickTravelStationSystem> ();

			if (playerControllerManager.isPlayerDriving ()) {
				currentQuickTravelStationSystem.travelToThisStation (playerControllerManager.getCurrentVehicle ().transform);
			} else {
				currentQuickTravelStationSystem.travelToThisStation (player.transform);
			}

			mapObjectInformation quickTravelStationMapObjectInformation = currentQuickTravelStation.GetComponent<mapObjectInformation> ();

			currentQuickTravelStation = null;

			quickTravelButtonImage.color = disabledQuickTravelColor;

			openOrCloseMap (false);

			if (playerMapObjectInformation != null && quickTravelStationMapObjectInformation != null) {
				playerMapObjectInformation.setNewBuildingAndFloorIndex (quickTravelStationMapObjectInformation.getBuildingIndex (), quickTravelStationMapObjectInformation.getFloorIndex ());
			}
		}
	}

	//MANAGE MAP MENU
	public void openOrCloseMap (bool state)
	{
		if (mapEnabled && (!playerControllerManager.isPlayerMenuActive () || mapOpened) && !playerControllerManager.isUsingDevice () && !pauseManager.isGamePaused ()) {
			mapOpened = state;

			assignMapWindowBorder ();

			pauseManager.openOrClosePlayerMenu (mapOpened, mapMenu.transform, useBlurUIPanel);

			if (mapMenu.activeSelf != mapOpened) {
				mapMenu.SetActive (mapOpened);
			}

			pauseManager.setIngameMenuOpenedState (ingameMenuName, mapOpened, true);

			if (mapOpened) {
				currentIconSizeMultiplier = openMapIconSizeMultiplier;
				targetMapPosition = mapWindowTargetPosition.position;
				targetScale = mapWindowTargetPosition.sizeDelta;

				removeMapObjectInfo ();
			} else {
				currentIconSizeMultiplier = 1;
				targetMapPosition = originalMapPosition;
				targetScale = originalMapScale;
			}

			pauseManager.enableOrDisablePlayerMenu (mapOpened, true, false);

			if (!mapViewIn2dActive) {
				setQuick2dMap ();
			}

			checkChangeMapPositionCoroutine ();

			if (mapOpened) {
				setCameraSize (zoomWhenOpen);

				previousBuildingIndex = currentBuildingIndex;

			} else {
				setCameraSize (zoomWhenClose);
				if (mapIndexEnabled) {
					showOrHideMapIndexWindow (false);
				}

				//get the current building where the player is right now when he close the map menu
				updateCurrentBuilding (previousBuildingIndex);

				//get the current floor where the player is right now when he close the map menu
				currentFloorIndex = getClosestFloorToPlayer ();

				updateCurrentFloorEnabledState ();

				updateCurrentBuildingEnabledState ();

				checkCurrentIconPressed (false, null);
			}

			if (useMapCursor && mapCursor != null) {
				mapCursor.SetActive (mapOpened);

				if (mapOpened) {
					mapCursor.transform.SetAsLastSibling ();
				}
			}
		}
	}

	public void openOrCLoseMapFromTouch ()
	{
		openOrCloseMap (!mapOpened);
	}

	public void removeMapObjectInfo ()
	{
		mapObjectInfoField.text = "";
		mapObjectNameField.text = "";
	}

	//MANAGE MAP WINDOW SIZE AND POSITION
	public void checkChangeMapPositionCoroutine ()
	{
		if (moveMapCoroutine != null) {
			StopCoroutine (moveMapCoroutine);
		}

		moveMapCoroutine = StartCoroutine (changeMapPositionCoroutine ());
	}

	IEnumerator changeMapPositionCoroutine ()
	{
		bool changeMapWindowParent = false;

		if (miniMapWindowEnabledInGame) {
			if (miniMapWindowSmoothOpening) {
				for (float t = 0; t < 1;) {
					t += getDeltaTime ();

					mapWindow.position = Vector2.Lerp (mapWindow.position, targetMapPosition, t);
					mapWindow.sizeDelta = Vector2.Lerp (mapWindow.sizeDelta, targetScale, t);
					mapRender.sizeDelta = Vector2.Lerp (mapRender.sizeDelta, targetScale, t);

					yield return null;
				}
			} else {
				mapWindow.position = targetMapPosition;
				mapWindow.sizeDelta = targetScale;
				mapRender.sizeDelta = targetScale;

				if (miniMapWindowWithMask) {
					changeMapWindowParent = true;
				}
			}
		} else {
			enableOrDisableMapWindow (mapOpened);

			mapWindow.position = targetMapPosition;
			mapWindow.sizeDelta = targetScale;
			mapRender.sizeDelta = targetScale;

			changeMapWindowParent = true;
		}

		if (changeMapWindowParent) {
			if (mapOpened) {
				mapWindow.SetParent (mapWindowTargetPosition.parent);
			} else {
				mapWindow.SetParent (mapMenu.transform.parent);
			}
		}

		if (usingCircleMap) {
			if (mapOpened) {
				mapRender.SetParent (mapWindow);
			} else {
				mapRender.SetParent (mapCircleTransform);
			}

			mapRender.SetSiblingIndex (0);
		}
	}

	public float getDeltaTime ()
	{
		if (pauseManager.isRegularTimeScaleActive ()) {
			return Time.deltaTime;
		}

		return Time.unscaledDeltaTime;
	}

	public void enableOrDisableMapRotation (Slider mapRotationSlider)
	{
		if (mapRotationSlider.value == 0) {
			rotateMap = false;
		} else {
			rotateMap = true;
		}

		if (!rotateMap) {
			mapCamera.transform.localEulerAngles = Vector3.zero;
		}
	}

	//MANAGE MAP MENUS
	public void enableOrDisableMiniMap (bool state)
	{
		if (state) {
			if (miniMapWindowEnabledInGame) {
				enableOrDisableMapWindow (true);

				if (compassEnabled) {
					enableOrDisableCompass (true);
				}
			}
		} else {
			enableOrDisableMapWindow (false);

			if (compassEnabled) {
				enableOrDisableCompass (false);
			}
		}
	}

	void enableOrDisableMapWindow (bool state)
	{
		if (mapWindow.gameObject.activeSelf != state) {
			mapWindow.gameObject.SetActive (state);
		}
	}

	public void enableOrDisableCompass (bool state)
	{
		if (compassWindow.gameObject.activeSelf != state) {
			compassWindow.gameObject.SetActive (state);
		}
	}

	public void enableOrDisableHUD (bool state)
	{
		if (mapEnabled) {
			enableOrDisableMiniMap (state);
		}

		if (compassEnabled) {
			enableOrDisableCompass (state);
		}
	}

	public void changeMapIndexWindowState ()
	{
		showOrHideMapIndexWindow (!mapIndexWindow.activeSelf);
	}

	public void showOrHideMapIndexWindow (bool state)
	{
		mapIndexWindow.SetActive (state);

		mapIndexEnabled = state;
	}

	public void disableButtons ()
	{
		removeMarkButtonImage.color = disabledRemoveMarkColor;
		quickTravelButtonImage.color = disabledQuickTravelColor;
	}

	public void setMapGameObjectState (GameObject mapObject, bool state)
	{
		if (mapObject.activeSelf != state) {
			mapObject.SetActive (state);
		}
	}

	//MANAGE MAP CURRENT FLOOR INDEX AND INFO
	public int getClosestFloorToPlayer ()
	{
		int index = -1;

		float distance = Mathf.Infinity;

		int floorsCount = floors.Count;

		for (int i = 0; i < floorsCount; i++) {
			floorInfo currentFloorInfo = floors [i];

			if (currentFloorInfo.floor != null) {
				float currentDistance = Mathf.Abs (player.transform.position.y - currentFloorInfo.floor.position.y);

				if (currentDistance < distance) {
					distance = currentDistance;
					index = i;
				}
			} else {
				if (showWarningMessages) {
					print ("WARNING: Field floor is empty in the map creator, make sure every floor of every building is properly configured");
				}
			}
		}

		if (index > -1) {
			currentFloorNumber = floors [index].floorNumber;
		}

		return index;
	}

	public void checkNextFloor ()
	{
		if (!mapViewIn2dActive) {
			return;
		}

		if (currentFloorIndex + 1 <= floors.Count - 1) {
			currentFloorIndex++;
		}

		updateCurrentFloorNumber ();
	}

	public void checkPrevoiusFloor ()
	{
		if (!mapViewIn2dActive) {
			return;
		}

		if ((currentFloorIndex - 1) >= 0) {
			currentFloorIndex--;
		}

		updateCurrentFloorNumber ();
	}

	public void updateCurrentFloorNumber ()
	{
		updateCurrentFloorEnabledState ();

		if (floors.Count > currentFloorIndex && currentFloorIndex > -1) {
			currentFloorNumber = floors [currentFloorIndex].floorNumber;

			updateCurrentBuildingNameText ();
		} else {
			if (showWarningMessages) {
				print ("WARNING: The system is trying to access to a floor index too high to the current building, " +
				"make sure the triggers to change floor and building are placed properly.");
			}
		}

		checkEventOnChangeFloorForMapObjectInformation ();
	}

	public void updateCurrentFloorEnabledState ()
	{
//		for (int i = 0; i < floors.Count; i++) {
//			if (i == currentFloorIndex) {
//				if (!floors [i].floor.gameObject.activeSelf) {
//					floors [i].floor.gameObject.SetActive (true);
//				}
//			} else {
//				if (floors [i].floor.gameObject.activeSelf) {
//					floors [i].floor.gameObject.SetActive (false);
//				}
//			}
//		}

		mapCreatorManager.setCurrentFloorByFloorIndex (currentBuildingIndex, currentFloorIndex);
	}

	public void setCurrentFloorNumber (int floorNumber)
	{
		currentFloorNumber = floorNumber;
	}

	public void setCurrentFloorIndex (int floorIndex)
	{
		currentFloorIndex = floorIndex;
	}

	//MANAGE MAP CURRENT BUILDING INDEX, VALUES, NAMES, ETC....
	public void checkNextBuilding ()
	{
		if (!mapViewIn2dActive) {
			mapCreatorManager.enableOrDisabled3dMap (false, currentBuildingIndex);
			enableOrDisableAllFloorsFoundMapIcon (false);
		}

		if (currentBuildingIndex + 1 <= buildingList.Count - 1) {
			currentBuildingIndex++;
		}

		updateCurrentBuildingNumber ();
	}

	public void checkPrevoiusBuilding ()
	{
		if (!mapViewIn2dActive) {
			mapCreatorManager.enableOrDisabled3dMap (false, currentBuildingIndex);
			enableOrDisableAllFloorsFoundMapIcon (false);
		}

		if ((currentBuildingIndex - 1) >= 0) {
			currentBuildingIndex--;
		}

		updateCurrentBuildingNumber ();
	}

	public void updateCurrentBuildingNumber ()
	{
		updateCurrentBuildingEnabledState ();

		updateCurrentBuilding (currentBuildingIndex);

		if (currentBuilding != null && currentBuilding.useCameraPositionOnMapMenu) {
			Vector3 newPosition = currentBuilding.cameraPositionOnMapMenu.position;

			if (mapViewIn2dActive) {

				mainMapCameraTransform.rotation = currentBuilding.cameraPositionOnMapMenu.rotation;

				if (currentBuilding.useCameraOffset) {
					mapCamera.transform.position = new Vector3 (newPosition.x, newPosition.y + cameraOffset.y, newPosition.z);
				} else {
					mapCamera.transform.position = newPosition;
				}
			} else {
				mapCamera.transform.localPosition = vectorUp * cameraOffset.y;
				mapSystemPivotTransform.position = newPosition;
			}
		}

		if (!mapViewIn2dActive) {
			mapCreatorManager.enableOrDisabled3dMap (true, currentBuildingIndex);
			enableOrDisableAllFloorsFoundMapIcon (true);

			mapCreatorManager.setFloorActiveState (currentBuildingIndex, currentFloorIndex, false);
			//floors [currentFloorIndex].floor.gameObject.SetActive (false);
		}
	}

	public void updateCurrentBuildingEnabledState ()
	{
//		for (int i = 0; i < buildingList.Count; i++) {
//			if (i == currentBuildingIndex) {
//				if (!buildingList [i].buildingFloorsParent.gameObject.activeSelf) {
//					buildingList [i].buildingFloorsParent.gameObject.SetActive (true);
//				}
//			} else {
//				if (buildingList [i].buildingFloorsParent.gameObject.activeSelf) {
//					buildingList [i].buildingFloorsParent.gameObject.SetActive (false);
//				}
//			}
//		}

		mapCreatorManager.setCurrentBuildingByBuildingIndex (currentBuildingIndex);
	}

	public void updateCurrentBuilding (int buildingIndex)
	{
		if (buildingIndex >= buildingList.Count || buildingIndex < 0) {
			if (showWarningMessages) {
				print ("WARNING: building index doesn't exist in the list, make sure the map system is configured correctly");
			}

			return;
		}

		currentBuilding = buildingList [buildingIndex];
		floors = currentBuilding.floors;

		currentFloorIndex = getClosestFloorToPlayer ();

		currentBuildingIndex = buildingIndex;
		currentBuildingName = currentBuilding.Name;

		updateCurrentFloorNumber ();
	}

	public void updateCurrentBuildingNameText ()
	{
		currentMapZoneText.text = currentBuildingName.ToUpper () + " - " + floors [currentFloorIndex].Name.ToUpper ();
	}

	public void setCurrentBuildingName (string newName)
	{
		currentBuildingName = newName;
	}

	public void setMapOrientation (int mapCameraMovementTypeIndex, Transform mapOrientationTransform)
	{
		mapCameraMovement newMapCameraMovementType = (mapCameraMovement)mapCameraMovementTypeIndex;

		if (mapCameraMovementType != newMapCameraMovementType) {
			//			print ("new orientation");

			mapCameraMovementType = newMapCameraMovementType;

			cameraOffset = mapOrientationTransform.position;

			mapCamera.transform.position = getCameraPosition ();

			if (mainMapCameraTransform == null) {
				mainMapCameraTransform = mainMapCamera.transform;
			}

			mainMapCameraTransform.eulerAngles = mapOrientationTransform.eulerAngles;
		}
	}


	//START EDITOR FUNCTIONS
	public void searchBuildingList ()
	{
		checkGetMapCreatorManager ();

		if (mapCreatorManager != null) {
			floors.Clear ();

			buildingList.Clear ();

			bool floorListAssigned = false;

			int buildingListCount = mapCreatorManager.buildingList.Count;

			for (int i = 0; i < buildingListCount; i++) {
				buildingInfo newBuildingInfo = new buildingInfo ();

				mapCreator.buildingInfo currentBuildingInfo = mapCreatorManager.buildingList [i];

				newBuildingInfo.Name = currentBuildingInfo.Name;
				newBuildingInfo.isCurrentMap = currentBuildingInfo.isCurrentMap;
				newBuildingInfo.isInterior = currentBuildingInfo.isInterior;

				newBuildingInfo.buildingFloorsParent = currentBuildingInfo.buildingFloorsParent;

				newBuildingInfo.useCameraPositionOnMapMenu = currentBuildingInfo.useCameraPositionOnMapMenu;
				newBuildingInfo.useCameraOffset = currentBuildingInfo.useCameraOffset;

				if (newBuildingInfo.useCameraPositionOnMapMenu) {
					newBuildingInfo.cameraPositionOnMapMenu = currentBuildingInfo.cameraPositionOnMapMenu;
				}

				int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

				for (int j = 0; j < buildingFloorsListCount; j++) {
					floorInfo newFloorInfo = new floorInfo ();

					mapCreator.floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

					newFloorInfo.Name = currentFloorInfo.Name;
					newFloorInfo.floorNumber = currentFloorInfo.floorNumber;
					newFloorInfo.floor = currentFloorInfo.floor.transform;

					newBuildingInfo.floors.Add (newFloorInfo);
				}

				if (newBuildingInfo.isCurrentMap) {
					floors = newBuildingInfo.floors;

					currentBuilding = newBuildingInfo;

					floorListAssigned = true;

					currentBuildingIndex = i;
				}

				buildingList.Add (newBuildingInfo);
			}

			if (!floorListAssigned) {
				if (buildingList.Count > 0) {
					floors = buildingList [currentBuildingIndex].floors;
				}
			}

			mapCreatorManager.addNewPlayer (this, player, true);
		} else {
			print ("WARNING: no map creator was found in the scene. To use the map, please place a map creator prefab and configure buildings and floors on it");
		}

		updateComponent ();
	}

	public void addPlayerMapSystemToMapCreator ()
	{
		checkGetMapCreatorManager ();

		updateComponent ();

		mapCreatorManager.addNewPlayer (this, player, true);
	}

	public void addNewMapCreator ()
	{
		GameObject newMapCreatorGameObject = (GameObject)Instantiate (mapCreatorPrefab, Vector3.zero, Quaternion.identity);

		newMapCreatorGameObject.name = "Map Creator";

		updateComponent ();

		addPlayerMapSystemToMapCreator ();
	}

	public void selectMapCreatorObject ()
	{
		if (mapCreatorManager != null) {
			GKC_Utils.setActiveGameObjectInEditor (mapCreatorManager.gameObject);
		}
	}

	public void checkGetMapCreatorManager ()
	{
		if (mapCreatorManager == null) {
			mapCreatorManager = FindObjectOfType<mapCreator> ();
		}
	}

	public void updateEditorMapInfo ()
	{
		getEditorBuildingList ();

		getEditorFloorList ();

		getEditorMapPartList ();
	}

	public void getEditorBuildingList ()
	{
		if (buildingList.Count > 0) {
			buildingListString = new string[buildingList.Count];

			for (int i = 0; i < buildingList.Count; i++) {
				string name = buildingList [i].Name;
				buildingListString [i] = name;
			}
		} else {
			print ("Not buildings were found. To use the map object information component, first add and configure different floors in the map creator component. Check" +
			"the documentation of the asset related to the Map System for a better explanation");

			buildingListString = new string[0];

			currentBuildingIndex = 0;
		}

		updateComponent ();
	}

	public void getEditorFloorList ()
	{		
		if (buildingList.Count > 0) {
			if (currentBuildingIndex >= 0 && currentBuildingIndex < buildingList.Count) {
				if (buildingList [currentBuildingIndex].floors.Count > 0) {
					floorListString = new string[buildingList [currentBuildingIndex].floors.Count ];

					for (int i = 0; i < buildingList [currentBuildingIndex].floors.Count; i++) {
						if (buildingList [currentBuildingIndex].floors [i].floor) {
							string name = buildingList [currentBuildingIndex].floors [i].floor.gameObject.name;
							floorListString [i] = name;
						}
					}

					if (floorListString.Length <= currentFloorIndex) {
						currentFloorIndex = 0;
					}

					updateComponent ();
				}
			}
		}
	}

	public void getEditorMapPartList ()
	{
		checkGetMapCreatorManager ();

		if (mapCreatorManager != null) {
			if (currentFloorIndex >= 0 && currentBuildingIndex >= 0) {
				List<mapCreator.buildingInfo> temporalBuildingList = mapCreatorManager.buildingList;

				bool mapElementFound = false;

				if (currentBuildingIndex < temporalBuildingList.Count) {
					List<mapCreator.floorInfo> temporalBuildingFloorsList = temporalBuildingList [currentBuildingIndex].buildingFloorsList;

					if (currentFloorIndex < temporalBuildingFloorsList.Count) {
						List<GameObject> temporalMapPartsList = temporalBuildingFloorsList [currentFloorIndex].mapPartsList;

						mapPartListString = new string[temporalMapPartsList.Count];

						for (int i = 0; i < temporalMapPartsList.Count; i++) {
							string name = temporalMapPartsList [i].name;
							mapPartListString [i] = name;
						}

						mapElementFound = true;
					} 
				}

				if (!mapElementFound) {
					currentFloorIndex = 0;

					mapPartListString = new string[0];
				}

				updateComponent ();
			}
		}
	}

	public void updateMapIconTypes (List<mapIconType> newMapIconTypes)
	{
		mapIconTypes = newMapIconTypes;

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Map System Info", gameObject);
	}

	[System.Serializable]
	public class mapObjectInfo
	{
		public string name;
		public string typeName;
		public GameObject mapObject;
		public RectTransform mapIcon;

		public int buildingIndex;
		public int floorIndex;

		public int floorNumber;
		public int mapPartIndex;

		public Vector3 offset;
		public bool isActivated;
		public RectTransform textName;
		public int ID;
		public bool mapPartOwnerFound;
		public bool canBeShowTemporaly;
		public bool visibleAllFloors;
		public bool hasFloorAssigned;

		public bool visibleAllBuildings;

		public bool hasMapObjectInformation;
		public bool useEventsOnChangeFloor;
		public mapObjectInformation mapObjectInformationManager;

		public float extraIconSizeOnMap;
		public bool followCameraRotation;

		public bool followObjectRotation;

		public compassIconInfo compassIcon;
	}

	[System.Serializable]
	public class floorInfo
	{
		public string Name;
		public int floorNumber;
		public Transform floor;
	}

	[System.Serializable]
	public class buildingInfo
	{
		public string Name;
		public List<floorInfo> floors = new List<floorInfo> ();
		public bool isCurrentMap;
		public bool isInterior;
		public bool useCameraPositionOnMapMenu;
		public Transform cameraPositionOnMapMenu;
		public Transform buildingFloorsParent;
		public bool useCameraOffset;
	}

	[System.Serializable]
	public class markInfo
	{
		public string name;
		public int index;
		public GameObject mapObject;
	}

	[System.Serializable]
	public class glossaryElementInfo
	{
		public Text textName;
		public string typeName;
		public Slider slider;
	}

	[System.Serializable]
	public class compassIconInfo
	{
		public string Name;
		public GameObject iconGameObject;
		public Transform iconTransform;

		public RectTransform iconRectTransform;
		public Transform iconTargetTransform;
		public float verticalOffset;

		public float lastIconAngle;

		public bool useCompassClamp;
	
		public float currentCompassTargetRotation;

		public bool firstValueAssigned;
	}
}