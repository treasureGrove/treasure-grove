using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapObjectInformation))]
public class mapObjectInformationEditor : Editor
{
	SerializedProperty ID;
	//	SerializedProperty Name;
	SerializedProperty mapObjectName;

	SerializedProperty description;
	SerializedProperty mapObjectProp;
	SerializedProperty typeNameList;
	SerializedProperty typeIndex;
	SerializedProperty typeName;
	SerializedProperty visibleInAllBuildings;
	SerializedProperty visibleInAllFloors;
	SerializedProperty calculateFloorAtStart;

	SerializedProperty buildingIndex;

	SerializedProperty currentBuilding;
	SerializedProperty floorIndex;
	SerializedProperty currentFloor;
	SerializedProperty useCustomValues;
	SerializedProperty offset;
	SerializedProperty extraIconSizeOnMap;
	SerializedProperty followCameraRotation;

	SerializedProperty followObjectRotation;

	SerializedProperty canChangeBuildingAndFloor;
	SerializedProperty activateAtStart;
	SerializedProperty disableWhenPlayerHasReached;
	SerializedProperty useCloseDistance;
	SerializedProperty showOffScreenIcon;
	SerializedProperty showMapWindowIcon;
	SerializedProperty showDistance;
	SerializedProperty showDistanceOffScreen;
	SerializedProperty triggerRadius;
	SerializedProperty triggerColor;
	SerializedProperty objectiveOffset;
	SerializedProperty removeComponentWhenObjectiveReached;
	SerializedProperty setCustomCompassSettings;
	SerializedProperty useCompassIcon;
	SerializedProperty compassIconPrefab;
	SerializedProperty verticalOffset;
	SerializedProperty useCustomObjectiveColor;
	SerializedProperty removeCustomObjectiveColor;
	SerializedProperty objectiveColor;
	SerializedProperty callEventWhenPointReached;
	SerializedProperty pointReachedEvent;
	SerializedProperty belongToMapPart;
	SerializedProperty mapPartList;
	SerializedProperty mapPartName;
	SerializedProperty mapPartIndex;
	SerializedProperty useEventsOnChangeFloor;
	SerializedProperty useEventOnEnabledFloor;
	SerializedProperty evenOnEnabledFloor;
	SerializedProperty useEventOnDisabledFloor;
	SerializedProperty evenOnDisabledFloor;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelOffset;
	SerializedProperty gizmoLabelColor;
	SerializedProperty offsetShowGizmo;
	SerializedProperty offsetGizmoColor;
	SerializedProperty offsetRadius;

	SerializedProperty mainScreenObjectivesManagerName;

	mapObjectInformation mapObject;

	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		ID = serializedObject.FindProperty ("ID");
//		Name = serializedObject.FindProperty ("name");
		mapObjectName = serializedObject.FindProperty ("mapObjectName");

		description = serializedObject.FindProperty ("description");
		mapObjectProp = serializedObject.FindProperty ("mapObject");
		typeNameList = serializedObject.FindProperty ("typeNameList");
		typeIndex = serializedObject.FindProperty ("typeIndex");
		typeName = serializedObject.FindProperty ("typeName");
		visibleInAllBuildings = serializedObject.FindProperty ("visibleInAllBuildings");
		visibleInAllFloors = serializedObject.FindProperty ("visibleInAllFloors");
		calculateFloorAtStart = serializedObject.FindProperty ("calculateFloorAtStart");

		buildingIndex = serializedObject.FindProperty ("buildingIndex");

		currentBuilding = serializedObject.FindProperty ("currentBuilding");
		floorIndex = serializedObject.FindProperty ("floorIndex");
		currentFloor = serializedObject.FindProperty ("currentFloor");
		useCustomValues = serializedObject.FindProperty ("useCustomValues");
		offset = serializedObject.FindProperty ("offset");
		extraIconSizeOnMap = serializedObject.FindProperty ("extraIconSizeOnMap");
		followCameraRotation = serializedObject.FindProperty ("followCameraRotation");

		followObjectRotation= serializedObject.FindProperty ("followObjectRotation");

		canChangeBuildingAndFloor = serializedObject.FindProperty ("canChangeBuildingAndFloor");
		activateAtStart = serializedObject.FindProperty ("activateAtStart");
		disableWhenPlayerHasReached = serializedObject.FindProperty ("disableWhenPlayerHasReached");
		useCloseDistance = serializedObject.FindProperty ("useCloseDistance");
		showOffScreenIcon = serializedObject.FindProperty ("showOffScreenIcon");
		showMapWindowIcon = serializedObject.FindProperty ("showMapWindowIcon");
		showDistance = serializedObject.FindProperty ("showDistance");
		showDistanceOffScreen = serializedObject.FindProperty ("showDistanceOffScreen");
		triggerRadius = serializedObject.FindProperty ("triggerRadius");
		triggerColor = serializedObject.FindProperty ("triggerColor");
		objectiveOffset = serializedObject.FindProperty ("objectiveOffset");
		removeComponentWhenObjectiveReached = serializedObject.FindProperty ("removeComponentWhenObjectiveReached");
		setCustomCompassSettings = serializedObject.FindProperty ("setCustomCompassSettings");
		useCompassIcon = serializedObject.FindProperty ("useCompassIcon");
		compassIconPrefab = serializedObject.FindProperty ("compassIconPrefab");
		verticalOffset = serializedObject.FindProperty ("verticalOffset");
		useCustomObjectiveColor = serializedObject.FindProperty ("useCustomObjectiveColor");
		removeCustomObjectiveColor = serializedObject.FindProperty ("removeCustomObjectiveColor");
		objectiveColor = serializedObject.FindProperty ("objectiveColor");
		callEventWhenPointReached = serializedObject.FindProperty ("callEventWhenPointReached");
		pointReachedEvent = serializedObject.FindProperty ("pointReachedEvent");
		belongToMapPart = serializedObject.FindProperty ("belongToMapPart");
		mapPartList = serializedObject.FindProperty ("mapPartList");
		mapPartName = serializedObject.FindProperty ("mapPartName");
		mapPartIndex = serializedObject.FindProperty ("mapPartIndex");
		useEventsOnChangeFloor = serializedObject.FindProperty ("useEventsOnChangeFloor");
		useEventOnEnabledFloor = serializedObject.FindProperty ("useEventOnEnabledFloor");
		evenOnEnabledFloor = serializedObject.FindProperty ("evenOnEnabledFloor");
		useEventOnDisabledFloor = serializedObject.FindProperty ("useEventOnDisabledFloor");
		evenOnDisabledFloor = serializedObject.FindProperty ("evenOnDisabledFloor");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelOffset = serializedObject.FindProperty ("gizmoLabelOffset");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		offsetShowGizmo = serializedObject.FindProperty ("offsetShowGizmo");
		offsetGizmoColor = serializedObject.FindProperty ("offsetGizmoColor");
		offsetRadius = serializedObject.FindProperty ("offsetRadius");

		mainScreenObjectivesManagerName = serializedObject.FindProperty ("mainScreenObjectivesManagerName");

		mapObject = (mapObjectInformation)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			
			if (mapObject.showGizmo) {
				if (mapObject.disableWhenPlayerHasReached) {
					style.normal.textColor = mapObject.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;

					Handles.Label (mapObject.transform.position + mapObject.transform.up * mapObject.triggerRadius + mapObject.transform.up * mapObject.gizmoLabelOffset, 
						"Objective: " + mapObject.gameObject.name, style);
				}

				if (mapObject.offsetShowGizmo) {
					style.normal.textColor = mapObject.offsetGizmoColor;
					style.alignment = TextAnchor.MiddleCenter;

					Handles.Label (mapObject.transform.position + mapObject.offset, "Offset \n Position", style);
				}
			}

//			if (Selection.activeGameObject == mapObject.gameObject) {
//				mapObject.getMapObjectInformation ();
//			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("ID Settings", "window");
		GUILayout.Label ("Map Object ID: " + ID.intValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");

//		EditorGUILayout.PropertyField (Name);
		EditorGUILayout.PropertyField (mapObjectName);
		EditorGUILayout.PropertyField (description);
		EditorGUILayout.PropertyField (mapObjectProp);
		EditorGUILayout.PropertyField (mainScreenObjectivesManagerName);	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Object Settings", "window");
		if (typeNameList.arraySize > 0) {
			typeIndex.intValue = EditorGUILayout.Popup ("Map Icon Type", typeIndex.intValue, mapObject.typeNameList);
			typeName.stringValue = mapObject.typeNameList [typeIndex.intValue];
		}

		EditorGUILayout.PropertyField (visibleInAllBuildings);
		EditorGUILayout.PropertyField (visibleInAllFloors);
		EditorGUILayout.PropertyField (calculateFloorAtStart);

		if (mapObject.buildingList != null && mapObject.buildingList.Length > 0) {
			buildingIndex.intValue = EditorGUILayout.Popup ("Building Number", buildingIndex.intValue, mapObject.buildingList);

			if (buildingIndex.intValue >= 0 && mapObject.typeNameList.Length > buildingIndex.intValue) {
				currentBuilding.stringValue = mapObject.typeNameList [buildingIndex.intValue];
			}
				
			if (mapObject.floorList != null && mapObject.floorList.Length > 0) {
				floorIndex.intValue = EditorGUILayout.Popup ("Floor Number", floorIndex.intValue, mapObject.floorList);

				if (floorIndex.intValue >= 0 && mapObject.floorList.Length > floorIndex.intValue) {
					currentFloor.stringValue = mapObject.floorList [floorIndex.intValue];
				}
			}
		}

		EditorGUILayout.PropertyField (useCustomValues);

		EditorGUILayout.PropertyField (offset);
		EditorGUILayout.PropertyField (extraIconSizeOnMap);
		EditorGUILayout.PropertyField (followCameraRotation);
		EditorGUILayout.PropertyField (followObjectRotation);
		EditorGUILayout.PropertyField (canChangeBuildingAndFloor);
		EditorGUILayout.PropertyField (activateAtStart);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Map Values")) {
			mapObject.getMapObjectInformation ();
			mapObject.checkIfMapObjectInformationFound ();
		}

//		if (GUILayout.Button ("Change Names")) {
//			mapObject.changeNames ();
//		}


		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Advanced Settings", "window");
		EditorGUILayout.PropertyField (disableWhenPlayerHasReached);
		if (disableWhenPlayerHasReached.boolValue) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the Objective options", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useCloseDistance);
			EditorGUILayout.PropertyField (showOffScreenIcon);
			EditorGUILayout.PropertyField (showMapWindowIcon);
			EditorGUILayout.PropertyField (showDistance);
			EditorGUILayout.PropertyField (showDistanceOffScreen);
			EditorGUILayout.PropertyField (triggerRadius);
			EditorGUILayout.PropertyField (triggerColor);
			EditorGUILayout.PropertyField (objectiveOffset);

			EditorGUILayout.PropertyField (removeComponentWhenObjectiveReached);


			EditorGUILayout.PropertyField (setCustomCompassSettings);
			if (setCustomCompassSettings.boolValue) {
				EditorGUILayout.PropertyField (useCompassIcon);
				EditorGUILayout.PropertyField (compassIconPrefab);
				EditorGUILayout.PropertyField (verticalOffset);
			}
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useCustomObjectiveColor);
			if (useCustomObjectiveColor.boolValue) {
				EditorGUILayout.PropertyField (removeCustomObjectiveColor);
				if (!removeCustomObjectiveColor.boolValue) {
					EditorGUILayout.PropertyField (objectiveColor);
				}
			}
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Unity Event When Point Reached Settings", "window");
			EditorGUILayout.PropertyField (callEventWhenPointReached);
			if (callEventWhenPointReached.boolValue) {
				EditorGUILayout.PropertyField (pointReachedEvent);
			}

			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Part Owner Settings", "window");
		EditorGUILayout.PropertyField (belongToMapPart);
		if (belongToMapPart.boolValue) {
			if (mapPartList.arraySize > 0) {
				mapPartIndex.intValue = EditorGUILayout.Popup ("Map Part Owner", mapPartIndex.intValue, mapObject.mapPartList);
				if (mapPartList.arraySize > mapPartIndex.intValue) {
					mapPartName.stringValue = mapObject.mapPartList [mapPartIndex.intValue];
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Floor Changed Events Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnChangeFloor);
		if (useEventsOnChangeFloor.boolValue) {
			EditorGUILayout.PropertyField (useEventOnEnabledFloor);
			if (useEventOnEnabledFloor.boolValue) {
				EditorGUILayout.PropertyField (evenOnEnabledFloor);
			}
			EditorGUILayout.PropertyField (useEventOnDisabledFloor);
			if (useEventOnDisabledFloor.boolValue) {
				EditorGUILayout.PropertyField (evenOnDisabledFloor);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window");
		EditorGUILayout.PropertyField (buildingIndex);
		EditorGUILayout.PropertyField (floorIndex);
		if (GUILayout.Button ("Set Current Building/Floor Index")) {
			if (Application.isPlaying) {
				mapObject.setNewBuildingAndFloorIndexByInspector ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelOffset);
			EditorGUILayout.PropertyField (gizmoLabelColor);

			if (offset.vector3Value != Vector3.zero) {
				EditorGUILayout.PropertyField (offsetShowGizmo);
				if (offsetShowGizmo.boolValue) {
					EditorGUILayout.PropertyField (offsetGizmoColor);
					EditorGUILayout.PropertyField (offsetRadius);
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif