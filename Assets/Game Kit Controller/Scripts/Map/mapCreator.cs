using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mapCreator : MonoBehaviour
{
	public List<buildingInfo> buildingList = new List<buildingInfo> ();

	public LayerMask layerToPlaceElements;
	public bool useRaycastToPlaceElements;
	public Vector3 mapPartEnabledTriggerScale;

	public Color enabledTriggerGizmoColor;

	public Material floorMaterial;

	public GameObject triggerToChangeBuildingPrefab;
	public GameObject triggerToChangeFloorPrefab;

	public GameObject triggerToChangeDynamicObjectPrefab;

	public string mapLayer;

	public bool showGizmo;
	public bool showMapPartsGizmo;
	public bool showMapPartsTextGizmo;
	public bool showMapPartEnabledTrigger;
	public bool useSameLineColor;
	public Color mapLinesColor;

	public bool showBuildingTriggersLine;
	public Color buildingTriggersLineColor;
	public bool showBuildingTriggers;
	public Color buildingTriggersColor;
	public bool showBuildingTriggersCubes;
	public Color buildingTriggersCubesColor;

	public bool showFloorTriggersLine;
	public Color floorTriggersLineColor;
	public bool showFloorTriggers;
	public Color floorTriggersColor;
	public bool showFloorTriggersCubes;
	public Color floorTriggersCubesColor;

	public Color gizmoLabelColor;
	public bool useHandleForVertex;
	public float handleRadius;
	public bool showVertexHandles;

	public bool generate3dMeshesActive;
	public bool generate3dMeshesShowGizmo;
	public bool generateFull3dMapMeshes;
	public Material mapPart3dMeshMaterial;

	public Color mapPart3dMaterialColor = Color.white;

	public Vector3 mapPart3dOffset;
	public float mapPart3dHeight = 1;

	public List<mapSystem> mapSystemList = new List<mapSystem> ();

	public List<mapSystemInfo> mapSystemInfoList = new List<mapSystemInfo> ();

	public List<mapIconType> mapIconTypes = new List<mapIconType> ();

	bool floorsConfiguredByTriggers;

	public bool generate3dMapPartMesh;

	int lastMapObjectInformationIDAssigned = 1;

	public bool updateAllMapTilesEveryFrame;

	buildingInfo currentBuildingInfoToUse;
	floorInfo currentFloorInfoToUse;

	GameObject currentPlayerDetected;

	GameObject currentCharacterToUse;

	public bool show3dSettings;

	void Awake ()
	{
		int mapSystemInfoListCount = mapSystemInfoList.Count;

		for (int i = 0; i < mapSystemInfoListCount; i++) {
			mapSystemList.Add (mapSystemInfoList [i].mainMapSystem);
		}

		for (int i = mapSystemList.Count - 1; i >= 0; i--) {
			if (mapSystemList [i] == null) {
				mapSystemList.RemoveAt (i);

				print ("WARNING: the map system list configured in Map Creator inspector contains some missing element " +
				"related to a player map system, make sure it is properly configured");
			}
		}

		if (mapSystemInfoListCount == 0) {
			print ("The Map System and the Map Creator are not configured correctly. If both objects are in the scene, go to Map System inspector, " +
			"Map Floor And Icons, and press Search Floor List, to assign the created map floors to the map system");

			return;
		}
			
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				int mapTileBuilderListCount = currentFloorInfo.mapTileBuilderList.Count;

				if (!currentFloorInfo.floorEnabled) {
					for (int k = 0; k < mapTileBuilderListCount; k++) {
						currentFloorInfo.mapTileBuilderList [k].setMapPartEnabledState (false);
					}
				}
			}
		}
	}

	void Start ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {

				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				int mapTileBuilderListCount = currentFloorInfo.mapTileBuilderList.Count;

				for (int k = 0; k < mapTileBuilderListCount; k++) {
					mapTileBuilder currentMapTileBuilder = currentFloorInfo.mapTileBuilderList [k];

					if (currentMapTileBuilder != null) {
						currentMapTileBuilder.createMapTileElement ();
					} else {
						print ("Warning: there is a map tile builder element in the list which is not assigned correctly with index " + k + ", use the button to get all floor parts");
					}
				}
			}
		}
	}

	//START OF INGAME FUNCTIONS
	//Operations on all the map system components
	public int getAndIncreaselastMapObjectInformationIDAssigned ()
	{
		lastMapObjectInformationIDAssigned++;

		return lastMapObjectInformationIDAssigned;
	}

	public void setlastMapObjectInformationIDAssignedValue (int newValue)
	{
		lastMapObjectInformationIDAssigned = newValue;
	}

	public void addMapObject (bool visibleInAllBuildings, bool visibleInAllFloors, bool calculateFloorAtStart, GameObject obj, string type, Vector3 offset, int ID, 
	                          int mapPartIndex, int buildingIndex, float extraIconSizeOnMap, bool followCameraRotation, bool followObjectRotation, 
	                          bool setCustomCompassSettings, bool useCompassIcon, GameObject compassIconPrefab, float verticalOffset)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].addMapObject (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, obj, 
				type, offset, ID, mapPartIndex, buildingIndex, extraIconSizeOnMap, followCameraRotation, followObjectRotation, setCustomCompassSettings, useCompassIcon, compassIconPrefab, verticalOffset);
		}
	}

	public void removeMapObject (GameObject obj, bool isPathElement)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].removeMapObject (obj, isPathElement);
		}
	}

	public void enableOrDisableSingleMapIconByID (int ID, bool value)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].enableOrDisableSingleMapIconByID (ID, value);
		}
	}

	public void changeMapObjectIconFloor (GameObject objectToSearch, int newFloorIndex)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].changeMapObjectIconFloor (objectToSearch, newFloorIndex);
		}
	}

	public void changeMapObjectIconFloorByPosition (GameObject objectToSearch)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].changeMapObjectIconFloorByPosition (objectToSearch);
		}
	}

	public void setnewBuilingAndFloorIndexToMapObject (mapSystem.mapObjectInfo mapObjectInfoToModify, int newBuildingIndex, int newFloorIndex)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].setnewBuilingAndFloorIndexToMapObject (mapObjectInfoToModify, newBuildingIndex, newFloorIndex);
		}
	}

	public int getBuldingIndexByName (string buildingName)
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			if (buildingList [i].Name.Equals (buildingName)) {
				return i;
			}
		}

		return -1;
	}

	public void getAllBuildings ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			int buildingFloorsListCount = buildingList [i].buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				GetAllFloorParts (i, j);
			}
		}
	}

	public void changeCurrentFloor (GameObject currentTriggerFloor)
	{
		int newFloorIndex = -1;

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				if (currentFloorInfo.floor == currentTriggerFloor) {
//					print (buildingList [i].buildingFloorsList [j].Name);
					setCurrentFloorByFloorNumber (i, currentFloorInfo.floorNumber);

					newFloorIndex = j;
				}
			}
		}

		if (newFloorIndex != -1) {
			int mapSystemInfoListCount = mapSystemInfoList.Count;

			for (int i = 0; i < mapSystemInfoListCount; i++) {
				mapSystemInfo currentMapSystemInfo = mapSystemInfoList [i];

				if (currentMapSystemInfo.playerGameObject == currentPlayerDetected) {
					currentMapSystemInfo.mainMapSystem.setCurrentFloorIndex (newFloorIndex);

					currentMapSystemInfo.mainMapSystem.updateCurrentFloorNumber ();
				}
			}
		}
	}

	public void setCurrentFloorByFloorNumber (int buildingIndex, int floorNumber)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

		for (int j = 0; j < buildingFloorsListCount; j++) {
			floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

			if (currentFloorInfo.floor != null) {
				if (currentFloorInfo.floorNumber == floorNumber) {
					//print (currentBuildingInfo.buildingFloorsList [j].Name);
					if (!currentFloorInfo.floor.activeSelf) {
						currentFloorInfo.floor.SetActive (true);
					}
				} else {
					if (currentFloorInfo.floor.activeSelf) {
						currentFloorInfo.floor.SetActive (false);
					}
				}
			}
		}
	}

	public void setCurrentFloorByFloorIndex (int buildingIndex, int floorIndex)
	{
		if (buildingIndex < 0 || floorIndex < 0) {
			return;
		}

		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;
		
		for (int j = 0; j < buildingFloorsListCount; j++) {
			floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

			if (currentFloorInfo.floor != null) {
				if (j == floorIndex) {
					//print (currentBuildingInfo.buildingFloorsList [j].Name);
					if (!currentFloorInfo.floor.activeSelf) {
						currentFloorInfo.floor.SetActive (true);
					}
				} else {
					if (currentFloorInfo.floor.activeSelf) {
						currentFloorInfo.floor.SetActive (false);
					}
				}
			}
		}
	}

	public void setFloorActiveState (int buildingIndex, int floorIndex, bool state)
	{
		if (buildingIndex < 0 || floorIndex < 0) {
			return;
		}

		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		if (currentFloorInfo.floor != null && currentFloorInfo.floor.activeSelf != state) {
			currentFloorInfo.floor.SetActive (state);
		}
	}

	public void enableOrDisableSingleMapIconByMapPartIndex (int mapPartBuildingIndex, int mapPartIndex, int mapPartFloorIndex, bool value)
	{
		int mapSystemListCount = mapSystemList.Count;

		for (int i = 0; i < mapSystemListCount; i++) {
			mapSystemList [i].enableOrDisableSingleMapIconByMapPartIndex (mapPartBuildingIndex, mapPartIndex, mapPartFloorIndex, value);
		}
	}

	public void enableOrDisabled2dMap (bool state, int buildingIndex)
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			if (i == buildingIndex) {
				if (buildingList [i].buildingFloorsParent.gameObject.activeSelf != state) {
					buildingList [i].buildingFloorsParent.gameObject.SetActive (state);
				}
			}
		}
	}

	public void setCurrentObjectToChangeBuildingInfo (GameObject newCharacter)
	{
		currentCharacterToUse = newCharacter;
	}

	public void setCurrentMapPartIndex (int newValue)
	{
		int mapSystemInfoListCount = mapSystemInfoList.Count;

		for (int i = 0; i < mapSystemInfoListCount; i++) {
			mapSystemInfo currentMapSystemInfo = mapSystemInfoList [i];

			if (currentCharacterToUse == currentMapSystemInfo.playerGameObject) {
				if (currentMapSystemInfo.mainMapSystem != null) {
					currentMapSystemInfo.mainMapSystem.setCurrentMapPartIndex (newValue);
				}
			}
		}
	}

	public void getPlayerGameObject (GameObject newPlayerDetected)
	{
		currentPlayerDetected = newPlayerDetected;
	}

	public void changeCurrentBuilding (GameObject currentTriggerBuilding)
	{
//		print (currentTriggerBuilding.name);

		int buildingListCount = buildingList.Count;

		int mapSystemInfoListCount = mapSystemInfoList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			if (buildingList [i].buildingMapParent.gameObject == currentTriggerBuilding) {
				setCurrentBuildingByBuildingIndex (i);

				for (int j = 0; j < mapSystemInfoListCount; j++) {
					mapSystemInfo currentMapSystemInfo = mapSystemInfoList [j];

					if (currentMapSystemInfo.playerGameObject == currentPlayerDetected) {
						currentMapSystemInfo.mainMapSystem.updateCurrentBuilding (i);

						currentMapSystemInfo.mainMapSystem.updateCurrentFloorNumber ();
					}
				}
			}
		}
	}

	public void changeCurrentBuilding (int newCurrentBuildingIndex, GameObject newCharacter)
	{
		setCurrentBuildingByBuildingIndex (newCurrentBuildingIndex);

		int mapSystemInfoListCount = mapSystemInfoList.Count;

		for (int i = 0; i < mapSystemInfoListCount; i++) {
			mapSystemInfo currentMapSystemInfo = mapSystemInfoList [i];

			if (currentMapSystemInfo.playerGameObject == newCharacter) {
				currentMapSystemInfo.mainMapSystem.updateCurrentBuilding (newCurrentBuildingIndex);
				currentMapSystemInfo.mainMapSystem.updateCurrentFloorNumber ();
			}
		}
	}

	public void setCurrentBuildingByBuildingIndex (int buildingIndex)
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			if (i == buildingIndex) {
				if (!currentBuildingInfo.buildingFloorsParent.gameObject.activeSelf) {
					currentBuildingInfo.buildingFloorsParent.gameObject.SetActive (true);
				}
			} else {
				if (currentBuildingInfo.buildingFloorsParent.gameObject.activeSelf) {
					currentBuildingInfo.buildingFloorsParent.gameObject.SetActive (false);
				}
			}
		}
	}

	public void setBuildingAndFloorInfoToObject (GameObject objectToCheck, Transform buildingParent, Transform floorParent)
	{
		mapObjectInformation currentMapObjectInformation = objectToCheck.GetComponent<mapObjectInformation> ();

		if (currentMapObjectInformation != null) {

			int buildingListCount = buildingList.Count;

			for (int i = 0; i < buildingListCount; i++) {
				buildingInfo currentBuildingInfo = buildingList [i];

				if (currentBuildingInfo.buildingMapParent == buildingParent) {
					for (int j = 0; j < currentBuildingInfo.buildingFloorsList.Count; j++) {

						floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

						if (currentFloorInfo.floor != null && currentFloorInfo.floor.transform == floorParent) {
							currentMapObjectInformation.setNewBuildingAndFloorIndex (i, j);

							return;
						}
					}
				}
			}
		}
	}
	//END OF INGAME FUNCTIONS

	//START OF 3d MAP FUNCTIONS
	//3d map managemement
	public void addMapPart3dMeshToFloorParent (GameObject mapPart3dMesh, GameObject mapPartGameObject)
	{
		int mapBuildingIndex = -1;
		int mapFloorNumber = -1;
		int mapPartIndex = -1;

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {

			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				if (currentFloorInfo.mapPartsList.Contains (mapPartGameObject) && mapFloorNumber == -1) {
					mapFloorNumber = j;

					mapPartIndex = currentFloorInfo.mapPartsList.IndexOf (mapPartGameObject);

					mapBuildingIndex = i;
				}
			}
		}

		if (mapFloorNumber != -1 && mapPartIndex != -1 && mapBuildingIndex != -1) {
			floorInfo currentFloorInfo = buildingList [mapBuildingIndex].buildingFloorsList [mapFloorNumber];

			if (currentFloorInfo.mapPart3dMeshesParent == null) {
				currentFloorInfo.mapPart3dMeshesParent = new GameObject ().transform;
				currentFloorInfo.mapPart3dMeshesParent.SetParent (buildingList [mapBuildingIndex].buildingMapParent);
				currentFloorInfo.mapPart3dMeshesParent.name = "Map Part3d Meshes Parent " + currentFloorInfo.Name;
			}

			if (currentFloorInfo.mapPart3dMeshesParent != null) {
				mapPart3dMesh.transform.SetParent (currentFloorInfo.mapPart3dMeshesParent);

				if (mapPartIndex == currentFloorInfo.mapPartsList.Count - 1) {
					if (currentFloorInfo.mapPart3dMeshesParent.gameObject.activeSelf) {
						currentFloorInfo.mapPart3dMeshesParent.gameObject.SetActive (false);
					}
				}
			}
		} else {
			print ("Map floor or map part " + mapPartGameObject.name + " not found");
		}
	}

	public void enableOrDisabled3dMap (bool state, int buildingIndex)
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			if (i == buildingIndex) {
				buildingInfo currentBuildingInfo = buildingList [i];

				int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

				for (int j = 0; j < buildingFloorsListCount; j++) {
					floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

					if (currentFloorInfo.mapPart3dMeshesParent != null) {
						if (currentFloorInfo.mapPart3dMeshesParent.gameObject.activeSelf != state) {
							currentFloorInfo.mapPart3dMeshesParent.gameObject.SetActive (state);
						}
					}
				}
			}
		}
	}

	public void updateMapPart3dMeshPosition ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				if (!currentFloorInfo.floorEnabled) {

					int mapTileBuilderListCount = currentFloorInfo.mapTileBuilderList.Count;
					
					for (int k = 0; k < mapTileBuilderListCount; k++) {
						currentFloorInfo.mapTileBuilderList [k].updateMapPart3dMeshPosition (mapPart3dOffset);
					}
				}
			}
		}
	}

	public void enableOrDisableMapPart3dMesh (bool state)
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				if (!currentFloorInfo.floorEnabled) {
					int mapTileBuilderListCount = currentFloorInfo.mapTileBuilderList.Count;

					for (int k = 0; k < mapTileBuilderListCount; k++) {
						currentFloorInfo.mapTileBuilderList [k].enableOrDisableMapPart3dMesh (state);
					}
				}
			}
		}
	}

	public void set3dMapPartMeshColor (Color newColor, bool setOriginalColor, int currentBuildingIndex, int currentFloorIndex, int currentMapPartIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [currentBuildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [currentFloorIndex];

		if (setOriginalColor) {
			currentFloorInfo.mapTileBuilderList [currentMapPartIndex].setOriginal3dMapPartMaterialColor ();
		} else {
			currentFloorInfo.mapTileBuilderList [currentMapPartIndex].set3dMapPartMaterialColor (newColor);
		}
	}

	//3d MAP EDITOR FUNCTIONS
	public void removeMapPart3dMesh ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				if (!currentFloorInfo.floorEnabled) {
					int mapTileBuilderListCount = currentFloorInfo.mapTileBuilderList.Count;

					for (int k = 0; k < mapTileBuilderListCount; k++) {
						currentFloorInfo.mapTileBuilderList [k].removeMapPart3dMesh ();
					}
				}
			}
		}
	}

	public void generateMapPart3dMesh ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			int buildingFloorsListCount = currentBuildingInfo.buildingFloorsList.Count;

			for (int j = 0; j < buildingFloorsListCount; j++) {
				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

				if (!currentFloorInfo.floorEnabled) {
					int mapTileBuilderListCount = currentFloorInfo.mapTileBuilderList.Count;

					for (int k = 0; k < mapTileBuilderListCount; k++) {
						currentFloorInfo.mapTileBuilderList [k].generateMapPart3dMesh (mapPart3dHeight);
					}
				}
			}
		}
	}

	public void setGenerate3dMapPartMeshState ()
	{
		generate3dMapPartMesh = !generate3dMapPartMesh;

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
				for (int k = 0; k < buildingList [i].buildingFloorsList [j].mapTileBuilderList.Count; k++) {
					buildingList [i].buildingFloorsList [j].mapTileBuilderList [k].setGenerate3dMapPartMeshState (generate3dMapPartMesh);
				}
			}
		}
	}

	public void setGenerate3dMapPartMeshStateFromEditor ()
	{
		generate3dMapPartMesh = !generate3dMapPartMesh;

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
				for (int k = 0; k < buildingList [i].buildingFloorsList [j].mapTileBuilderList.Count; k++) {
					buildingList [i].buildingFloorsList [j].mapTileBuilderList [k].setGenerate3dMapPartMeshStateFromEditor (generate3dMapPartMesh);
				}
			}
		}
	}

	public void setGenerate3dMapPartMeshStateToFloor (bool value, int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		for (int i = 0; i < currentFloorInfo.mapTileBuilderList.Count; i++) {
			currentFloorInfo.mapTileBuilderList [i].setGenerate3dMapPartMeshState (value);
		}
	}

	public void setGenerate3dMapPartMeshStateToFloorFromEditor (bool value, int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		for (int i = 0; i < currentFloorInfo.mapTileBuilderList.Count; i++) {
			currentFloorInfo.mapTileBuilderList [i].setGenerate3dMapPartMeshStateFromEditor (value);
		}

		updateComponent ();
	}
	//END OF 3d MAP FUNCTIONS


	//START EDITOR FUNCTIONS
	//Floor map parts management
	public void addNewMapPart (Transform mapPartParent)
	{
		GameObject currentFloor = mapPartParent.gameObject;
		GameObject newMapPart = new GameObject ();
		newMapPart.transform.SetParent (currentFloor.transform);
		newMapPart.transform.localPosition = Vector3.zero;
		newMapPart.transform.localRotation = Quaternion.Euler (90, 0, 0);

		mapTileBuilder newMapTileBuilder = newMapPart.AddComponent<mapTileBuilder> ();
		newMapTileBuilder.setMapManager (this);
		newMapTileBuilder.setMapPartParent (mapPartParent);

		floorInfo currentFloorInfo = new floorInfo ();
		int buildingIndex = -1;
		int floorIndex = -1;

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
				if (buildingList [i].buildingFloorsList [j].floor == currentFloor) {
					currentFloorInfo = buildingList [i].buildingFloorsList [j];
					buildingIndex = i;
					floorIndex = j;
				}
			}
		}

		newMapTileBuilder.setMapPartBuildingIndex (buildingIndex);
		newMapTileBuilder.setMapPartFlooorIndex (floorIndex);
		newMapTileBuilder.setMapPartIndex (currentFloorInfo.mapPartsList.Count);

		newMapPart.name = (currentFloorInfo.mapPartsList.Count + 1).ToString ("000");
		currentFloorInfo.mapPartsList.Add (newMapPart);

		currentFloorInfo.mapTileBuilderList.Add (newMapTileBuilder);

		updateComponent ();
	}

	public void addNewMapPartFromMapCreator (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		GameObject currentFloor = currentFloorInfo.floor;
		GameObject newMapPart = new GameObject ();

		newMapPart.transform.SetParent (currentFloor.transform);

		if ((floorIndex - 1) >= 0) {
			newMapPart.transform.localPosition = Vector3.zero;
			//currentBuildingInfo.buildingFloorsList [floorIndex - 1].floor.transform.localPosition;
		}

		newMapPart.transform.localRotation = Quaternion.Euler (90, 0, 0);

		mapTileBuilder newMapTileBuilder = newMapPart.AddComponent<mapTileBuilder> ();
		newMapTileBuilder.setMapManager (this);
		newMapTileBuilder.setMapPartParent (currentFloorInfo.floor.transform);

		newMapTileBuilder.setMapPartBuildingIndex (buildingIndex);
		newMapTileBuilder.setMapPartFlooorIndex (floorIndex);
		newMapTileBuilder.setMapPartIndex (currentFloorInfo.mapPartsList.Count);

		newMapPart.name = (currentFloorInfo.mapPartsList.Count + 1).ToString ("000");
		currentFloorInfo.mapPartsList.Add (newMapPart);

		currentFloorInfo.mapTileBuilderList.Add (newMapTileBuilder);

		updateComponent ();
	}

	public void removeMapPart (int buildingIndex, int floorIndex, int mapPartIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];
		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		GameObject currentMapPart = currentFloorInfo.mapPartsList [mapPartIndex];

		if (currentMapPart != null) {
			DestroyImmediate (currentMapPart);
		}

		currentFloorInfo.mapPartsList.RemoveAt (mapPartIndex);

		GetAllFloorParts (buildingIndex, floorIndex);

		updateComponent ();
	}

	public void removeMapPart (Transform mapPartParent, GameObject mapPartToDelete)
	{
		GameObject currentFloor = mapPartParent.gameObject;

		floorInfo currentFloorInfo = new floorInfo ();
		int buildingIndex = -1;
		int floorIndex = -1;

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
				if (buildingList [i].buildingFloorsList [j].floor == currentFloor) {
					currentFloorInfo = buildingList [i].buildingFloorsList [j];
					buildingIndex = i;
					floorIndex = j;
				}
			}
		}

		currentFloorInfo.mapPartsList.Remove (mapPartToDelete);

		DestroyImmediate (mapPartToDelete);

		GetAllFloorParts (buildingIndex, floorIndex);

		updateComponent ();
	}

	public void removeAllMapParts (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		for (int i = 0; i < currentFloorInfo.mapPartsList.Count; i++) {
			if (currentFloorInfo.mapPartsList [i] != null) {
				DestroyImmediate (currentFloorInfo.mapPartsList [i]);
			}
		}

		currentFloorInfo.mapPartsList.Clear ();

		updateComponent ();
	}

	public void duplicateMapPart (Transform mapPartParent, GameObject mapPartToDuplicate)
	{
		GameObject currentFloor = mapPartParent.gameObject;

		GameObject newMapPart = (GameObject)Instantiate (mapPartToDuplicate, Vector3.zero, Quaternion.identity);

		newMapPart.transform.SetParent (currentFloor.transform);

		newMapPart.transform.localPosition = mapPartToDuplicate.transform.localPosition;
		newMapPart.transform.localRotation = mapPartToDuplicate.transform.localRotation;

		floorInfo currentFloorInfo = new floorInfo ();

		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
				if (buildingList [i].buildingFloorsList [j].floor == currentFloor) {
					currentFloorInfo = buildingList [i].buildingFloorsList [j];
				}
			}
		}

		newMapPart.name = (currentFloorInfo.mapPartsList.Count + 1).ToString ("000");
		currentFloorInfo.mapPartsList.Add (newMapPart);

		mapTileBuilder newMapTileBuilder = newMapPart.GetComponent<mapTileBuilder> ();
		newMapTileBuilder.setMapPartIndex (currentFloorInfo.mapTileBuilderList.Count);
		currentFloorInfo.mapTileBuilderList.Add (newMapTileBuilder);

		updateComponent ();
	}

	//Map floors management
	public void addNewFloor (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo newFloorInfo = new floorInfo ();
		newFloorInfo.floorNumber = currentBuildingInfo.buildingFloorsList.Count;

		GameObject newFloor = new GameObject ();
		newFloor.transform.SetParent (currentBuildingInfo.buildingFloorsParent);
		newFloor.transform.localPosition = Vector3.zero;
		newFloor.transform.localRotation = Quaternion.identity;
		newFloor.name = "Floor-" + (newFloorInfo.floorNumber).ToString ("000");
		newFloorInfo.Name = newFloor.name;
		newFloorInfo.floor = newFloor;
		newFloorInfo.floorEnabled = true;

		currentBuildingInfo.buildingFloorsList.Add (newFloorInfo);

		GameObject newMapPart = new GameObject ();
		newMapPart.transform.SetParent (newFloor.transform);
		newMapPart.transform.localPosition = Vector3.zero;
		newMapPart.transform.localRotation = Quaternion.Euler (90, 0, 0);

		mapTileBuilder newMapTileBuilder = newMapPart.AddComponent<mapTileBuilder> ();
		newMapTileBuilder.setMapManager (this);
		newMapTileBuilder.setMapPartParent (newFloor.transform);

		newMapTileBuilder.setMapPartBuildingIndex (buildingIndex);
		newMapTileBuilder.setMapPartFlooorIndex (currentBuildingInfo.buildingFloorsList.IndexOf (newFloorInfo));
		newMapTileBuilder.setMapPartIndex (newFloorInfo.mapPartsList.Count);

		newMapPart.name = (newFloorInfo.mapPartsList.Count + 1).ToString ("000");
		newFloorInfo.mapPartsList.Add (newMapPart);

		newFloorInfo.mapTileBuilderList.Add (newMapTileBuilder);

		updateComponent ();
	}

	public void removeFloor (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		GameObject currentFloor = currentFloorInfo.floor;

		for (int i = 0; i < currentFloorInfo.mapPartsList.Count; i++) {
			removeMapPart (buildingIndex, floorIndex, i);
		}

		removeTriggerToChangeFloorList (buildingIndex, floorIndex);

		currentBuildingInfo.buildingFloorsList.RemoveAt (floorIndex);

		DestroyImmediate (currentFloor);

		for (int j = 0; j < currentBuildingInfo.buildingFloorsList.Count; j++) {
			GetAllFloorParts (buildingIndex, j);
		}

		updateComponent ();
	}

	public void removeAllFloors (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		for (int i = 0; i < currentBuildingInfo.buildingFloorsList.Count; i++) {
			floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [i];

			GameObject currentFloor = currentFloorInfo.floor;

			for (int j = 0; j < currentFloorInfo.mapPartsList.Count; j++) {
				if (currentFloorInfo.mapPartsList [j] != null) {
					DestroyImmediate (currentFloorInfo.mapPartsList [j]);
				}
			}

			removeTriggerToChangeFloorList (buildingIndex, i);

			DestroyImmediate (currentFloor);
		}

		currentBuildingInfo.buildingFloorsList.Clear ();

		updateComponent ();
	}

	public void renameFloor (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		currentFloorInfo.floor.name = currentFloorInfo.Name;

		updateComponent ();
	}

	//Map floors triggers management
	public void addTriggerToChangeFloor (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		if (currentFloorInfo.currentFloorParent == null) {
			GameObject currentFloorParent = new GameObject ();
			currentFloorParent.name = "Trigger Parent Floor " + currentFloorInfo.floorNumber;
			currentFloorParent.transform.SetParent (currentBuildingInfo.buildingMapParent);
			currentFloorParent.transform.localPosition = Vector3.zero;
			currentFloorParent.transform.localRotation = Quaternion.identity;
			currentFloorInfo.currentFloorParent = currentFloorParent.transform;
		}

		GameObject newTriggerToChangeFloor = (GameObject)Instantiate (triggerToChangeFloorPrefab, Vector3.zero, Quaternion.identity);

		if (useRaycastToPlaceElements) {

			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceElements)) {
					newTriggerToChangeFloor.transform.position = hit.point + Vector3.up * 0.1f;
				}
			}
		}

		newTriggerToChangeFloor.transform.localScale = mapPartEnabledTriggerScale;

		eventTriggerSystem currentEventTriggerSystem = newTriggerToChangeFloor.GetComponent<eventTriggerSystem> ();
		currentEventTriggerSystem.eventList [0].objectToCall = gameObject;
		currentEventTriggerSystem.eventList [0].objectToSend = currentFloorInfo.floor;

		newTriggerToChangeFloor.transform.SetParent (currentFloorInfo.currentFloorParent);
		newTriggerToChangeFloor.name = "Trigger To Change Floor " + (currentFloorInfo.floorNumber) + "-" + currentFloorInfo.triggerToChangeFloorList.Count;

		currentFloorInfo.triggerToChangeFloorList.Add (newTriggerToChangeFloor);

		updateComponent ();
	}

	public void removeTriggerToChangeFloor (int buildingIndex, int floorIndex, int triggerIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		GameObject currentChangeTrigger = currentFloorInfo.triggerToChangeFloorList [triggerIndex];

		if (currentChangeTrigger != null) {
			DestroyImmediate (currentChangeTrigger);
		}

		currentFloorInfo.triggerToChangeFloorList.RemoveAt (triggerIndex);

		if (currentFloorInfo.triggerToChangeFloorList.Count == 0) {
			if (currentFloorInfo.currentFloorParent != null) {
				DestroyImmediate (currentFloorInfo.currentFloorParent.gameObject);
			}
		}

		updateComponent ();
	}

	public void removeTriggerToChangeFloorList (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		for (int i = 0; i < currentFloorInfo.triggerToChangeFloorList.Count; i++) {
			GameObject currentChangeTrigger = currentFloorInfo.triggerToChangeFloorList [i];

			if (currentChangeTrigger != null) {
				DestroyImmediate (currentChangeTrigger);
			}
		}

		currentFloorInfo.triggerToChangeFloorList.Clear ();

		if (currentFloorInfo.currentFloorParent != null) {
			DestroyImmediate (currentFloorInfo.currentFloorParent.gameObject);
		}

		updateComponent ();
	}

	public void enableOrDisableAllFloorParts (bool state, int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		for (int j = 0; j < currentFloorInfo.mapTileBuilderList.Count; j++) {
			currentFloorInfo.mapTileBuilderList [j].setMapPartEnabledStateFromEditor (state);
		}

		updateComponent ();
	}

	public void enableOrDisableFloorPart (bool state, int buildingIndex, int floorIndex, int mapPartIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		currentFloorInfo.mapTileBuilderList [mapPartIndex].setMapPartEnabledStateFromEditor (state);

		updateComponent ();
	}

	//Map building management
	public void renameBuilding (int index)
	{
		buildingInfo currentBuildingInfo = buildingList [index];

		currentBuildingInfo.buildingMapParent.name = currentBuildingInfo.Name;

		updateComponent ();
	}

	public void addNewBuilding ()
	{
		buildingInfo currentBuildingInfo = new buildingInfo ();

		GameObject newBuildingParent = new GameObject ();
		newBuildingParent.transform.SetParent (transform);
		newBuildingParent.transform.localPosition = Vector3.zero;
		newBuildingParent.transform.localRotation = Quaternion.identity;

		newBuildingParent.name = "Building-" + (buildingList.Count + 1).ToString ("000");
		currentBuildingInfo.Name = newBuildingParent.name;
		currentBuildingInfo.buildingMapParent = newBuildingParent.transform;
		currentBuildingInfo.buildingMapEnabled = true;

		GameObject newBuildingFloorsParent = new GameObject ();
		newBuildingFloorsParent.name = "Building Content";
		newBuildingFloorsParent.transform.SetParent (newBuildingParent.transform);
		newBuildingFloorsParent.transform.localPosition = Vector3.zero;
		newBuildingFloorsParent.transform.localRotation = Quaternion.identity;
		currentBuildingInfo.buildingFloorsParent = newBuildingFloorsParent.transform;

		buildingList.Add (currentBuildingInfo);

		addNewFloor (buildingList.Count - 1);

		updateComponent ();
	}

	public void removeBuilding (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		GameObject currentBuilding = currentBuildingInfo.buildingMapParent.gameObject;

		for (int j = 0; j < currentBuildingInfo.buildingFloorsList.Count; j++) {
			floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

			for (int k = 0; k < currentFloorInfo.mapPartsList.Count; k++) {
				removeMapPart (buildingIndex, j, k);
			}
		}

		//removeTriggerToChangeFloorList (buildingIndex, floorIndex);

		buildingList.RemoveAt (buildingIndex);

		DestroyImmediate (currentBuilding);

		getAllBuildings ();

		updateComponent ();
	}

	public void removeAllBuildings ()
	{
		for (int i = 0; i < buildingList.Count; i++) {
			buildingInfo currentBuildingInfo = buildingList [i];

			for (int j = 0; j < currentBuildingInfo.buildingFloorsList.Count; j++) {

				floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];
				GameObject currentFloor = currentFloorInfo.floor;

				for (int k = 0; k < currentFloorInfo.mapPartsList.Count; k++) {
					if (currentFloorInfo.mapPartsList [k] != null) {
						DestroyImmediate (currentFloorInfo.mapPartsList [k]);
					}
				}

				removeTriggerToChangeFloorList (i, j);

				DestroyImmediate (currentFloor);
			}

			currentBuildingInfo.buildingFloorsList.Clear ();

			DestroyImmediate (currentBuildingInfo.buildingMapParent.gameObject);
		}

		buildingList.Clear ();

		updateComponent ();
	}


	public void enableOrDisableAllBuildings (bool state)
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			enableOrDisableBuilding (state, i);
		}

		updateComponent ();
	}

	public void enableOrDisableBuilding (bool state, int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		for (int j = 0; j < currentBuildingInfo.buildingFloorsList.Count; j++) {
			floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [j];

			for (int k = 0; k < currentFloorInfo.mapTileBuilderList.Count; k++) {
				currentFloorInfo.mapTileBuilderList [k].setMapPartEnabledState (state);
			}
		}

		updateComponent ();
	}
		
	//Map building triggers management
	public void addTriggerToChangeBuilding (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		if (currentBuildingInfo.triggerToChangeBuildingParent == null) {
			GameObject currentBuildingParent = new GameObject ();
			currentBuildingParent.name = "Trigger Parent Building " + currentBuildingInfo.Name;
			currentBuildingParent.transform.SetParent (currentBuildingInfo.buildingMapParent);
			currentBuildingParent.transform.localPosition = Vector3.zero;
			currentBuildingParent.transform.localRotation = Quaternion.identity;
			currentBuildingInfo.triggerToChangeBuildingParent = currentBuildingParent.transform;
		}

		GameObject newTriggerToChangeBuilding = (GameObject)Instantiate (triggerToChangeBuildingPrefab, Vector3.zero, Quaternion.identity);

		if (useRaycastToPlaceElements) {
			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceElements)) {
					newTriggerToChangeBuilding.transform.position = hit.point + Vector3.up * 0.1f;
				}
			}
		}

		newTriggerToChangeBuilding.transform.localScale = mapPartEnabledTriggerScale;

		eventTriggerSystem currentEventTriggerSystem = newTriggerToChangeBuilding.GetComponent<eventTriggerSystem> ();
		currentEventTriggerSystem.eventList [0].objectToCall = gameObject;
		currentEventTriggerSystem.eventList [0].objectToSend = currentBuildingInfo.buildingMapParent.gameObject;

		newTriggerToChangeBuilding.transform.SetParent (currentBuildingInfo.triggerToChangeBuildingParent);
		newTriggerToChangeBuilding.name = "Trigger To Change Building-" + currentBuildingInfo.Name + "-" + currentBuildingInfo.triggerToChangeBuildingList.Count;

		currentBuildingInfo.triggerToChangeBuildingList.Add (newTriggerToChangeBuilding);

		updateComponent ();
	}

	public void removeTriggerToChangeBuilding (int buildingIndex, int triggerIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		GameObject currentChangeTrigger = currentBuildingInfo.triggerToChangeBuildingList [triggerIndex];

		if (currentChangeTrigger != null) {
			DestroyImmediate (currentChangeTrigger);
		}

		currentBuildingInfo.triggerToChangeBuildingList.RemoveAt (triggerIndex);

		if (currentBuildingInfo.triggerToChangeBuildingList.Count == 0) {
			if (currentBuildingInfo.triggerToChangeBuildingParent != null) {
				DestroyImmediate (currentBuildingInfo.triggerToChangeBuildingParent.gameObject);
			}
		}

		updateComponent ();
	}

	public void removeTriggerToChangeBuildingList (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		for (int i = 0; i < currentBuildingInfo.triggerToChangeBuildingList.Count; i++) {
			GameObject currentChangeTrigger = currentBuildingInfo.triggerToChangeBuildingList [i];

			if (currentChangeTrigger != null) {
				DestroyImmediate (currentChangeTrigger);
			}
		}

		currentBuildingInfo.triggerToChangeBuildingList.Clear ();

		if (currentBuildingInfo.triggerToChangeBuildingParent != null) {
			DestroyImmediate (currentBuildingInfo.triggerToChangeBuildingParent.gameObject);
		}

		updateComponent ();
	}

	public void addTriggerToDynamicObject (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		if (currentBuildingInfo.triggerToChangeDynamicObjectsParent == null) {
			GameObject currentBuildingParent = new GameObject ();
			currentBuildingParent.name = "Trigger Dynamic Objects " + currentBuildingInfo.Name;
			currentBuildingParent.transform.SetParent (currentBuildingInfo.buildingMapParent);
			currentBuildingParent.transform.localPosition = Vector3.zero;
			currentBuildingParent.transform.localRotation = Quaternion.identity;
			currentBuildingInfo.triggerToChangeDynamicObjectsParent = currentBuildingParent.transform;
		}

		GameObject newTriggerToChangeDynamicObject = (GameObject)Instantiate (triggerToChangeDynamicObjectPrefab, Vector3.zero, Quaternion.identity);

		if (useRaycastToPlaceElements) {
			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceElements)) {
					newTriggerToChangeDynamicObject.transform.position = hit.point + Vector3.up * 0.1f;
				}
			}
		}

		newTriggerToChangeDynamicObject.transform.localScale = mapPartEnabledTriggerScale;

		buildingAndFloorObjectInfoSystem currentBuildingAndFloorObjectInfoSystem = newTriggerToChangeDynamicObject.GetComponent<buildingAndFloorObjectInfoSystem> ();

		currentBuildingAndFloorObjectInfoSystem.setBuildingFloorInfo (currentBuildingInfo.buildingMapParent, currentBuildingInfo.buildingFloorsList [floorIndex].floor.transform, this);

		newTriggerToChangeDynamicObject.transform.SetParent (currentBuildingInfo.triggerToChangeDynamicObjectsParent);

		newTriggerToChangeDynamicObject.name = "Trigger Dynamic Object-" + currentBuildingInfo.Name + "-" + currentBuildingInfo.buildingFloorsList [floorIndex].Name + " " +
		(currentBuildingInfo.triggerForDynamicObjectsList.Count + 1);

		currentBuildingInfo.triggerForDynamicObjectsList.Add (newTriggerToChangeDynamicObject);

		updateComponent ();
	}

	public void removeTriggerToDynamicObject (int buildingIndex, int triggerIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		GameObject currentChangeTrigger = currentBuildingInfo.triggerForDynamicObjectsList [triggerIndex];

		if (currentChangeTrigger != null) {
			DestroyImmediate (currentChangeTrigger);
		}

		currentBuildingInfo.triggerForDynamicObjectsList.RemoveAt (triggerIndex);

		if (currentBuildingInfo.triggerForDynamicObjectsList.Count == 0) {
			if (currentBuildingInfo.triggerToChangeDynamicObjectsParent != null) {
				DestroyImmediate (currentBuildingInfo.triggerToChangeDynamicObjectsParent.gameObject);
			}
		}

		updateComponent ();
	}

	public void removeTriggerToDynamicObjectList (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		for (int i = 0; i < currentBuildingInfo.triggerForDynamicObjectsList.Count; i++) {
			GameObject currentChangeTrigger = currentBuildingInfo.triggerForDynamicObjectsList [i];

			if (currentChangeTrigger != null) {
				DestroyImmediate (currentChangeTrigger);
			}
		}

		currentBuildingInfo.triggerForDynamicObjectsList.Clear ();

		if (currentBuildingInfo.triggerToChangeDynamicObjectsParent != null) {
			DestroyImmediate (currentBuildingInfo.triggerToChangeDynamicObjectsParent.gameObject);
		}

		updateComponent ();
	}

	public void GetAllFloorParts (int buildingIndex, int floorIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];

		floorInfo currentFloorInfo = currentBuildingInfo.buildingFloorsList [floorIndex];

		currentFloorInfo.mapPartsList.Clear ();
		currentFloorInfo.mapTileBuilderList.Clear ();
		List<GameObject> currentMapPartsList = currentFloorInfo.mapPartsList;
		int mapPartIndex = 0;

		print ("Updating info of building " + buildingIndex + " " + buildingList [buildingIndex].Name);

		foreach (Transform child in currentFloorInfo.floor.transform) {
			print ("Updating info of floor " + mapPartIndex + " " + currentFloorInfo.Name);

			mapPartIndex++;

			string newName = (mapPartIndex).ToString ("000");

			child.gameObject.name = newName;

			mapTileBuilder newMapTileBuilder = child.gameObject.GetComponent<mapTileBuilder> ();

			newMapTileBuilder.setMapManager (this);
			newMapTileBuilder.setInternalName (newName);
			newMapTileBuilder.setMapPartParent (currentFloorInfo.floor.transform);
			newMapTileBuilder.setMapPartBuildingIndex (buildingIndex);
			newMapTileBuilder.setMapPartFlooorIndex (floorIndex);
			newMapTileBuilder.setMapPartIndex (mapPartIndex - 1);

			currentMapPartsList.Add (child.gameObject);

			currentFloorInfo.mapTileBuilderList.Add (newMapTileBuilder);
		}

		updateComponent ();
	}

	public void calculateAllMapTileMesh ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			currentBuildingInfoToUse = buildingList [i];

			for (int j = 0; j < currentBuildingInfoToUse.buildingFloorsList.Count; j++) {
				currentFloorInfoToUse = currentBuildingInfoToUse.buildingFloorsList [j];

				for (int k = 0; k < currentFloorInfoToUse.mapTileBuilderList.Count; k++) {
					currentFloorInfoToUse.mapTileBuilderList [k].calculateMapTileMesh ();

					GKC_Utils.updateComponent (currentFloorInfoToUse.mapTileBuilderList [k]);
				}
			}
		}
	}

	public void removeAllMapTileMesh ()
	{
		int buildingListCount = buildingList.Count;

		for (int i = 0; i < buildingListCount; i++) {
			currentBuildingInfoToUse = buildingList [i];

			for (int j = 0; j < currentBuildingInfoToUse.buildingFloorsList.Count; j++) {
				currentFloorInfoToUse = currentBuildingInfoToUse.buildingFloorsList [j];

				for (int k = 0; k < currentFloorInfoToUse.mapTileBuilderList.Count; k++) {
					currentFloorInfoToUse.mapTileBuilderList [k].removeMapTileRenderer ();

					GKC_Utils.updateComponent (currentFloorInfoToUse.mapTileBuilderList [k]);
				}
			}
		}
	}

	public void setMapObjectInformationID ()
	{
		lastMapObjectInformationIDAssigned = 1;

		mapObjectInformation[] mapObjectInformationList = FindObjectsOfType<mapObjectInformation> ();

		foreach (mapObjectInformation mapObjectInfo in mapObjectInformationList) {
			print (mapObjectInfo.name + " with ID: " + lastMapObjectInformationIDAssigned);

			mapObjectInfo.assignID (lastMapObjectInformationIDAssigned);

			lastMapObjectInformationIDAssigned++;
		}

		setlastMapObjectInformationIDAssignedValue (lastMapObjectInformationIDAssigned);

		print ("\n Object ID assigned to every map object information component in the level");

		updateComponent ();
	}




	//EXTRA EDITOR FUNCTIONS
	public List<mapIconType> getMapIconTypes ()
	{
		return mapIconTypes;
	}

	public void setMapIconTypes (int mapSystemIndex)
	{
		mapIconTypes.Clear ();

		for (int i = 0; i < mapSystemInfoList [mapSystemIndex].mainMapSystem.mapIconTypes.Count; i++) {
			mapIconTypes.Add (mapSystemInfoList [mapSystemIndex].mainMapSystem.mapIconTypes [i]);
		}

		updateComponent ();
	}

	public void updateMapIconTypesOnMapSystem ()
	{
		for (int i = 0; i < mapSystemInfoList.Count; i++) {

			mapSystemInfoList [i].mainMapSystem.updateMapIconTypes (mapIconTypes);
		}

		updateComponent ();
	}

	public void addCameraPositionOnMapMenu (int buildingIndex)
	{
		buildingInfo currentBuildingInfo = buildingList [buildingIndex];
		GameObject newCameraPositionOnMapMenu = new GameObject ();

		newCameraPositionOnMapMenu.name = currentBuildingInfo.Name + "-Camera Position On Map Menu";
		newCameraPositionOnMapMenu.transform.SetParent (currentBuildingInfo.buildingMapParent);
		newCameraPositionOnMapMenu.transform.localPosition = Vector3.zero;
		newCameraPositionOnMapMenu.transform.eulerAngles = new Vector3 (90, 0, 0);

		currentBuildingInfo.cameraPositionOnMapMenu = newCameraPositionOnMapMenu.transform;

		if (useRaycastToPlaceElements) {
			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceElements)) {
					newCameraPositionOnMapMenu.transform.position = hit.point + Vector3.up * 0.1f;
				} else {
					newCameraPositionOnMapMenu.transform.position = new Vector3 (editorCameraPosition.x, newCameraPositionOnMapMenu.transform.position.y, editorCameraPosition.z);
				}
			}
		}

		updateComponent ();
	}

	public void addNewPlayer (mapSystem newMapSystem, GameObject newPlayer, bool callingFunctionOnEditor)
	{
		for (int i = 0; i < mapSystemInfoList.Count; i++) {	
			if (mapSystemInfoList [i].playerGameObject == null) {
				mapSystemInfoList.RemoveAt (i);

				i--;
			}
		}

		bool playerFound = false;
		int playerIndex = 0;

		for (int i = 0; i < mapSystemInfoList.Count; i++) {	
			if (mapSystemInfoList [i].playerGameObject == newPlayer) {
				playerFound = true;
				playerIndex = i;
			}
		}

		if (playerFound) {
			mapSystemInfoList [playerIndex].playerGameObject = newPlayer;
			mapSystemInfoList [playerIndex].mainMapSystem = newMapSystem;
		} else {
			mapSystemInfo newMapSystemInfo = new mapSystemInfo ();
			newMapSystemInfo.Name = "Player " + (mapSystemInfoList.Count + 1);
			newMapSystemInfo.playerGameObject = newPlayer;
			newMapSystemInfo.mainMapSystem = newMapSystem;

			mapSystemInfoList.Add (newMapSystemInfo);
		}

		print (newPlayer.name + " added to the Map Creator");

		if (callingFunctionOnEditor) {
			updateComponent ();
		}
	}

	public void selectChangeBuildingTrigger (int buildingIndex, int triggerIndex)
	{
		GKC_Utils.setActiveGameObjectInEditor (buildingList [buildingIndex].triggerToChangeBuildingList [triggerIndex]);
	}

	public void selectDynamicObjectsTrigger (int buildingIndex, int triggerIndex)
	{
		GKC_Utils.setActiveGameObjectInEditor (buildingList [buildingIndex].triggerForDynamicObjectsList [triggerIndex]);
	}

	public void selectCurrentFloorTrigger (int buildingIndex, int floorIndex, int triggerIndex)
	{
		GKC_Utils.setActiveGameObjectInEditor (buildingList [buildingIndex].buildingFloorsList [floorIndex].triggerToChangeFloorList [triggerIndex].gameObject);
	}

	public void selectCurrentMapPart (int buildingIndex, int floorIndex, int mapPartIndex)
	{
		GKC_Utils.setActiveGameObjectInEditor (buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [mapPartIndex]);
	}

	public void updateMapObjectInformationMapSystem ()
	{
		mapObjectInformation[] mapObjectInformationList = FindObjectsOfType<mapObjectInformation> ();

		foreach (mapObjectInformation mapObjectInfo in mapObjectInformationList) {
			mapObjectInfo.getCurrentMapManager ();
		}

		print ("\n Map Object Information components in the level updated");
	}
	//END OF EDITOR FUNCTIONS


	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Map Creator", gameObject);
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
		if (showGizmo && !Application.isPlaying) {
			int buildingListCount = buildingList.Count;

			for (int i = 0; i < buildingListCount; i++) {
				for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
					if (buildingList [i].buildingFloorsList [j].floor != null) {
						Gizmos.color = Color.green;

						Gizmos.DrawWireSphere (buildingList [i].buildingFloorsList [j].floor.transform.position, 0.8f);

						if (j + 1 <= buildingList [i].buildingFloorsList.Count - 1) {
							if (buildingList [i].buildingFloorsList [j + 1].floor != null) {
								Gizmos.color = Color.white;

								Gizmos.DrawLine (buildingList [i].buildingFloorsList [j].floor.transform.position, buildingList [i].buildingFloorsList [j + 1].floor.transform.position);
							}
						}

						for (int k = 0; k < buildingList [i].buildingFloorsList [j].triggerToChangeFloorList.Count; k++) {
							if (showFloorTriggersLine) {
								Gizmos.color = floorTriggersLineColor;

								Gizmos.DrawLine (buildingList [i].buildingFloorsList [j].floor.transform.position, 
									buildingList [i].buildingFloorsList [j].triggerToChangeFloorList [k].transform.position);
							}

							if (showFloorTriggers) {
								Gizmos.color = floorTriggersColor;

								Gizmos.DrawSphere (buildingList [i].buildingFloorsList [j].triggerToChangeFloorList [k].transform.position, 0.8f);
							}

							if (showFloorTriggersCubes) {
								Gizmos.color = floorTriggersCubesColor;

								Gizmos.DrawCube (buildingList [i].buildingFloorsList [j].triggerToChangeFloorList [k].transform.position, 
									buildingList [i].buildingFloorsList [j].triggerToChangeFloorList [k].transform.localScale);
							}
						}
					}
				}

				for (int k = 0; k < buildingList [i].triggerToChangeBuildingList.Count; k++) {
					if (showBuildingTriggersLine) {
						Gizmos.color = buildingTriggersLineColor;

						Gizmos.DrawLine (buildingList [i].buildingMapParent.position, 
							buildingList [i].triggerToChangeBuildingList [k].transform.position);
					}

					if (showBuildingTriggers) {
						Gizmos.color = buildingTriggersColor;

						Gizmos.DrawSphere (buildingList [i].triggerToChangeBuildingList [k].transform.position, 0.8f);
					}

					if (showBuildingTriggersCubes) {
						Gizmos.color = buildingTriggersCubesColor;

						Gizmos.DrawCube (buildingList [i].triggerToChangeBuildingList [k].transform.position, 
							buildingList [i].triggerToChangeBuildingList [k].transform.localScale);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class floorInfo
	{
		public string Name;
		public int floorNumber;
		public GameObject floor;
		public bool floorEnabled;
		public Transform currentFloorParent;
		public Transform mapPart3dMeshesParent;

		public List<GameObject> triggerToChangeFloorList = new List<GameObject> ();

		public List<GameObject> mapPartsList = new List<GameObject> ();

		public List<mapTileBuilder> mapTileBuilderList = new List<mapTileBuilder> ();
	}

	[System.Serializable]
	public class buildingInfo
	{
		public string Name;
		public Transform buildingMapParent;
		public Transform buildingFloorsParent;
		public List<floorInfo> buildingFloorsList = new List<floorInfo> ();

		public List<GameObject> triggerToChangeBuildingList = new List<GameObject> ();
		public Transform triggerToChangeBuildingParent;

		public bool useCameraPositionOnMapMenu;
		public Transform cameraPositionOnMapMenu;
		public bool useCameraOffset = true;

		public bool isCurrentMap;
		public bool isInterior;
		public bool buildingMapEnabled;

		public List<GameObject> triggerForDynamicObjectsList = new List<GameObject> ();
		public Transform triggerToChangeDynamicObjectsParent;
	}

	[System.Serializable]
	public class mapSystemInfo
	{
		public string Name;
		public GameObject playerGameObject;
		public mapSystem mainMapSystem;
	}
}
