using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class mapObjectInformation : MonoBehaviour
{
	//	public string name;

	public string mapObjectName;

	[TextArea (3, 10)]
	public string description;

	public GameObject mapObject;

	public float offsetRadius;

	public bool showOffScreenIcon = true;
	public bool showMapWindowIcon = true;
	public bool showDistance = true;
	public bool showDistanceOffScreen;
	public bool isActivate = true;

	public bool visibleInAllBuildings;
	public bool visibleInAllFloors;
	public bool calculateFloorAtStart;

	public bool useCloseDistance = true;
	public float triggerRadius = 5;
	public Color triggerColor = Color.blue;
	public float gizmoLabelOffset;
	public Color gizmoLabelColor = Color.white;
	public int typeIndex;
	public string typeName;
	public string[] typeNameList;
	public int floorIndex;
	public string currentFloor;
	public string[] floorList;

	public float extraIconSizeOnMap;
	public bool followCameraRotation;
	public bool followObjectRotation;
	public Vector3 offset;
	public mapSystem mapManager;

	public bool useCustomObjectiveColor;
	public Color objectiveColor;
	public bool removeCustomObjectiveColor;
	public float objectiveOffset;

	public bool removeComponentWhenObjectiveReached = true;

	public bool disableWhenPlayerHasReached;

	public int buildingIndex;
	public string buildingName;
	public string[] buildingList;
	public string currentBuilding;

	public bool belongToMapPart;
	public string mapPartName;
	public string[] mapPartList;
	public int mapPartIndex;
	public mapCreator mapCreatorManager;

	public bool useCustomValues;

	public int ID;

	public bool callEventWhenPointReached;
	public UnityEvent pointReachedEvent = new UnityEvent ();

	public bool useEventsOnChangeFloor;
	public bool useEventOnEnabledFloor;
	public UnityEvent evenOnEnabledFloor;
	public bool useEventOnDisabledFloor;
	public UnityEvent evenOnDisabledFloor;

	public bool canChangeBuildingAndFloor;

	public bool activateAtStart = true;

	public Color offsetGizmoColor;
	public bool offsetShowGizmo;
	public bool showGizmo;

	string objectiveIconName;

	public mapSystem.mapObjectInfo currentMapObjectInfo;

	public bool setCustomCompassSettings;
	public bool useCompassIcon;
	public GameObject compassIconPrefab;
	public float verticalOffset;

	public bool mapObjectAssigned;

	public string mainScreenObjectivesManagerName = "Screen Objectives Manager";

	screenObjectivesSystem screenObjectivesManager;

	bool mapObjectRemoved;

	void Start ()
	{
		checkAssignMapObject ();

		if (activateAtStart) {
			createMapIconInfo ();
		}

		StartCoroutine (checkIfBelongToMapPartCoroutine ());
	}

	IEnumerator checkIfBelongToMapPartCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);

		checkIfBelongToMapPart ();
	}

	void checkIfBelongToMapPart ()
	{
		checkGetMapCreatorManager ();

		bool mapObjectAssignedCorrectly = false;

		if (belongToMapPart) {
			if (mapCreatorManager != null) {
				if (buildingIndex < mapCreatorManager.buildingList.Count) {

					List<mapCreator.floorInfo> temporalBuildingFloorsList = mapCreatorManager.buildingList [buildingIndex].buildingFloorsList;

					if (floorIndex < temporalBuildingFloorsList.Count) {

						List<mapTileBuilder> temporalMapTileBuilder = temporalBuildingFloorsList [floorIndex].mapTileBuilderList;

						if (mapPartIndex < temporalMapTileBuilder.Count) {
							if (!temporalMapTileBuilder [mapPartIndex].mapPartEnabled) {
								//print ("inactive");
								mapCreatorManager.enableOrDisableSingleMapIconByID (ID, false);
							}

							mapObjectAssignedCorrectly = true;
						}
					}
				} 

				if (!mapObjectAssignedCorrectly) {
					string objectName = gameObject.name;

					if (mapObject != null) {
						objectName = mapObject.name;
					}

					print ("WARNING: map object information not properly configured in object " + objectName);
				}
			}
		}
	}

	public void checkAssignMapObject ()
	{
		if (!mapObjectAssigned) {
			mapObject = gameObject;

			mapObjectAssigned = true;
		}
	}

	public void createMapIconInfo ()
	{
		checkAssignMapObject ();

		if (!belongToMapPart) {
			mapPartIndex = -1;
		}

		if (typeName != "") {
			if (disableWhenPlayerHasReached) {
					
				if (showMapWindowIcon) {
					checkGetMapCreatorManager ();

					if (mapCreatorManager != null) {
						mapCreatorManager.addMapObject (visibleInAllBuildings, visibleInAllFloors, false, mapObject,
							typeName, offset, -1, -1, buildingIndex, extraIconSizeOnMap, followCameraRotation,
							followObjectRotation, setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);

						mapObjectRemoved = false;
					}
				}

				getScreenObjectivesManager ();

				if (screenObjectivesManager != null) {
					screenObjectivesManager.addElementToScreenObjectiveList (mapObject, useCloseDistance, triggerRadius, showOffScreenIcon, 
						showDistance, showDistanceOffScreen, typeName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor, objectiveOffset);
				}

			} else {
				checkGetMapCreatorManager ();

				if (mapCreatorManager != null) {
					mapCreatorManager.addMapObject (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, mapObject, 
						typeName, offset, ID, mapPartIndex, buildingIndex, extraIconSizeOnMap, followCameraRotation, 
						followObjectRotation, setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);

					mapObjectRemoved = false;
				}

				if (useCustomValues) {

					getScreenObjectivesManager ();

					if (screenObjectivesManager != null) {
						screenObjectivesManager.addElementToScreenObjectiveList (mapObject, useCloseDistance, triggerRadius, 
							showOffScreenIcon, showDistance, showDistanceOffScreen, objectiveIconName, useCustomObjectiveColor, objectiveColor, 
							removeCustomObjectiveColor, 0);
					}
				}
			}
		} else {
			string objectName = gameObject.name;

			if (mapObject != null) {
				objectName = mapObject.name;
			}

			print ("WARNING: map object information not properly configured in object " + objectName);
		}
	}

	public void getScreenObjectivesManager ()
	{
		if (screenObjectivesManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainScreenObjectivesManagerName, typeof(screenObjectivesSystem));

			screenObjectivesManager = FindObjectOfType<screenObjectivesSystem> ();
		}
	}

	public void addMapObject (string mapIconType)
	{
		checkAssignMapObject ();

		if (mapCreatorManager != null) {
			mapCreatorManager.addMapObject (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, mapObject, 
				mapIconType, offset, ID, mapPartIndex, buildingIndex, extraIconSizeOnMap, followCameraRotation, 
				followObjectRotation, setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);

			mapObjectRemoved = false;
		}
	}

	public void removeMapObject ()
	{
		checkAssignMapObject ();

		getScreenObjectivesManager ();

		//remove the object from the screen objective system in case it was added as a mark
		if (screenObjectivesManager != null) {
			screenObjectivesManager.removeGameObjectFromList (mapObject);
		}

		//remove object of the map
		if (mapCreatorManager != null) {
			mapCreatorManager.removeMapObject (mapObject, false);
		}

		mapObjectRemoved = true;
	}

	void OnDestroy ()
	{
		if (GKC_Utils.isApplicationPlaying () && Time.deltaTime > 0) {
//			print ("DESTROYYYYYY map checking if playing");

			if (!mapObjectRemoved) {
				removeMapObject ();
			}
		}
	}

	public void setPathElementInfo (bool showOffScreenIconInfo, bool showMapWindowIconInfo, bool showDistanceInfo)
	{
		typeName = "Path Element";
		showGizmo = true;
		showOffScreenIcon = showOffScreenIconInfo;
		showMapWindowIcon = showMapWindowIconInfo;
		showDistance = showDistanceInfo;
	}

	public void getMapIconTypeList ()
	{
		checkGetMapManager ();

		if (mapManager != null) {
			typeNameList = new string[mapManager.mapIconTypes.Count];

			for (int i = 0; i < mapManager.mapIconTypes.Count; i++) {
				typeNameList [i] = mapManager.mapIconTypes [i].typeName;
			}

			updateComponent ();
		}
	}

	public void getBuildingList ()
	{
		checkGetMapManager ();

		checkGetMapCreatorManager ();

		if (mapManager != null) {
			if (mapManager.buildingList.Count > 0) {
				buildingList = new string[mapManager.buildingList.Count];

				for (int i = 0; i < mapManager.buildingList.Count; i++) {
					string newName = mapManager.buildingList [i].Name;

					buildingList [i] = newName;
				}

				updateComponent ();
			} else {
				print ("Not buildings were found. To use the map object information component, first add and configure different floors in the map " +
				"creator component. Check the documentation of the asset related to the Map System for a better explanation");
			}
		}
	}

	public void getFloorList ()
	{		
		if (mapManager != null && mapCreatorManager != null) {
			if (mapManager.buildingList.Count > 0) {
				if (buildingIndex >= 0 && buildingIndex < mapCreatorManager.buildingList.Count) {

					List<mapSystem.floorInfo> temporalFloors = mapManager.buildingList [buildingIndex].floors;

					if (temporalFloors.Count > 0) {
						floorList = new string[temporalFloors.Count ];

						for (int i = 0; i < temporalFloors.Count; i++) {
							if (temporalFloors [i].floor != null) {
								string newName = temporalFloors [i].floor.gameObject.name;

								floorList [i] = newName;
							}
						}

						updateComponent ();

						if (belongToMapPart) {
							getMapPartList ();
						}
					}
				}
			} else {
				print ("Not floors were found. To use the map object information component, first add and configure different floors in the map creator component. Check" +
				"the documentation of the asset related to the Map System for a better explanation");
			}
		}
	}

	public void checkGetMapManager ()
	{
		if (mapManager == null) {
			mapManager = FindObjectOfType<mapSystem> ();
		}
	}

	public void checkGetMapCreatorManager ()
	{
		if (mapCreatorManager == null) {
			mapCreatorManager = FindObjectOfType<mapCreator> ();
		}
	}

	public void getCurrentMapManager ()
	{
		checkGetMapManager ();

		checkGetMapCreatorManager ();

		updateComponent ();
	}

	public void getMapPartList ()
	{
		if (mapCreatorManager != null) {
			if (floorIndex >= 0 && buildingIndex >= 0) {
				bool mapAssignedCorrectly = false;

				List<mapCreator.buildingInfo> temporalBuildingList = mapCreatorManager.buildingList;

				if (buildingIndex < temporalBuildingList.Count) {

					List<mapCreator.floorInfo> temporalBuildingFloorsList = temporalBuildingList [buildingIndex].buildingFloorsList;

					if (floorIndex < temporalBuildingFloorsList.Count) {
						mapPartList = new string[temporalBuildingFloorsList [floorIndex].mapPartsList.Count];

						for (int i = 0; i < temporalBuildingFloorsList [floorIndex].mapPartsList.Count; i++) {
							string newName = temporalBuildingFloorsList [floorIndex].mapPartsList [i].name;

							mapPartList [i] = newName;
						}

						mapAssignedCorrectly = true;
					}
				} 

				if (!mapAssignedCorrectly) {
					floorIndex = 0;

					mapPartList = new string[0];
				}

				updateComponent ();
			}
		}
	}

	public void getIconTypeIndexByName (string iconTypeName)
	{
		int index = mapManager.getIconTypeIndexByName (iconTypeName);

		if (index != -1) {
			typeIndex = index;
			typeName = iconTypeName;
		}
	}

	public void getMapObjectInformation ()
	{
		getBuildingList ();

		getFloorList ();

		getMapIconTypeList ();
	}

	public void checkIfMapObjectInformationFound ()
	{
		if (mapManager != null) {

			if (mapManager.buildingList.Count == 0) {
				buildingList = new string[0];
			} else {
				if (buildingIndex > mapManager.buildingList.Count - 1 || buildingIndex < 0) {
					buildingIndex = 0;

					floorList = new string[0];

					floorIndex = -1;
				}

				if (floorIndex > floorList.Length - 1) {
					floorIndex = 0;
				}
			}
		}

		updateComponent ();
	}

	//	public void changeNames ()
	//	{
	//		mapObjectInformation[] mapObjectInformationList = FindObjectsOfType<mapObjectInformation> ();
	//
	//		foreach (mapObjectInformation currentmapObjectInformation in mapObjectInformationList) {
	//			currentmapObjectInformation.changeName ();
	//		}
	//	}
	//
	//	public void changeName ()
	//	{
	//		mapObjectName = this.name;
	//
	//		print ("Name changed to " + mapObjectName + " on " + gameObject.name);
	//
	//		updateComponent ();
	//	}

	public void setCustomValues (bool visibleInAllBuildingsValue, bool visibleInAllFloorsValue, bool calculateFloorAtStartValue, bool useCloseDistanceValue, 
	                             float triggerRadiusValue, bool showOffScreenIconValue, bool showMapWindowIconValue, bool showDistanceValue, bool showDistanceOffScreenValue, string objectiveIconNameValue,
	                             bool useCustomObjectiveColorValue, Color objectiveColorValue, bool removeCustomObjectiveColorValue)
	{
		useCustomValues = true;

		visibleInAllBuildings = visibleInAllBuildingsValue;
		visibleInAllFloors = visibleInAllFloorsValue;
		calculateFloorAtStart = calculateFloorAtStartValue;

		useCloseDistance = useCloseDistanceValue;
		triggerRadius = triggerRadiusValue;
		showOffScreenIcon = showOffScreenIconValue;
		showMapWindowIcon = showMapWindowIconValue;
		showDistance = showDistanceValue;
		showDistanceOffScreen = showDistanceOffScreenValue;
		objectiveIconName = objectiveIconNameValue;
		useCustomObjectiveColor = useCustomObjectiveColorValue;
		objectiveColor = objectiveColorValue;
		removeCustomObjectiveColor = removeCustomObjectiveColorValue;
	}

	public void changeMapObjectIconFloor (int newFloorIndex)
	{
		checkAssignMapObject ();

		mapCreatorManager.changeMapObjectIconFloor (mapObject, newFloorIndex);
	}

	public void changeMapObjectIconFloorByPosition ()
	{
		checkAssignMapObject ();

		mapCreatorManager.changeMapObjectIconFloorByPosition (mapObject);
	}

	public void enableSingleMapIconByID ()
	{
		mapCreatorManager.enableOrDisableSingleMapIconByID (ID, true);
	}

	public void assignID (int newID)
	{
		ID = newID;

		updateComponent ();
	}

	public void checkPointReachedEvent ()
	{
		if (callEventWhenPointReached) {
			if (pointReachedEvent.GetPersistentEventCount () > 0) {
				pointReachedEvent.Invoke ();
			}
		}
	}

	//Functions called and used to link a map object information and a map object info in the map system, so if the state changes in one of them, the other can update its state too

	public void checkEventOnChangeFloor (int currentBuildingIndex, int currentFloorIndex)
	{
		if (useEventsOnChangeFloor) {
			if (currentBuildingIndex == buildingIndex && floorIndex == currentFloorIndex) {
				if (useEventOnEnabledFloor) {
					evenOnEnabledFloor.Invoke ();
				}
			} else {
				if (useEventOnDisabledFloor) {
					evenOnDisabledFloor.Invoke ();
				}
			}
		}
	}

	public void setCurrentMapObjectInfo (mapSystem.mapObjectInfo newMapObjectInfo)
	{
		if (canChangeBuildingAndFloor) {
			currentMapObjectInfo = newMapObjectInfo;
		}
	}

	public void setNewBuildingAndFloorIndex (int newBuildingIndex, int newFloorIndex)
	{
		if (canChangeBuildingAndFloor) {
			buildingIndex = newBuildingIndex;
			floorIndex = newFloorIndex;

			mapCreatorManager.setnewBuilingAndFloorIndexToMapObject (currentMapObjectInfo, newBuildingIndex, newFloorIndex);
		}
	}

	public void setNewBuildingAndFloorIndexByInspector ()
	{
		if (canChangeBuildingAndFloor) {
			mapCreatorManager.setnewBuilingAndFloorIndexToMapObject (currentMapObjectInfo, buildingIndex, floorIndex);
		}
	}

	public void setNewBuildingIndex (int newBuildingIndex)
	{
		buildingIndex = newBuildingIndex;
	}

	public void setNewFloorIndex (int newFloorIndex)
	{
		floorIndex = newFloorIndex;
	}

	public int getBuildingIndex ()
	{
		return buildingIndex;
	}

	public int getFloorIndex ()
	{
		return floorIndex;
	}

	public bool removeComponentWhenObjectiveReachedEnabled ()
	{
		return removeComponentWhenObjectiveReached;
	}

	public void setMapObjectName (string newName)
	{
		mapObjectName = newName;
	}

	public string getMapObjectName ()
	{
		if (mapObjectName == "") {
			return this.name;
		} else {
			return mapObjectName;
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Map Object Information Element " + gameObject.name, gameObject);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (transform.position, triggerRadius);

			if (offsetShowGizmo) {
				Gizmos.color = offsetGizmoColor;
				Gizmos.DrawSphere (transform.position + offset, offsetRadius);
			}
		}
	}
}