using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapCreator))]
public class mapCreatorEditor : Editor
{
	SerializedProperty floorMaterial;
	SerializedProperty mapLayer;
	SerializedProperty triggerToChangeBuildingPrefab;
	SerializedProperty triggerToChangeFloorPrefab;
	SerializedProperty triggerToChangeDynamicObjectPrefab;
	SerializedProperty mapPart3dMeshMaterial;
	SerializedProperty mapPart3dMaterialColor;
	SerializedProperty layerToPlaceElements;
	SerializedProperty useRaycastToPlaceElements;
	SerializedProperty mapPartEnabledTriggerScale;
	SerializedProperty enabledTriggerGizmoColor;
	SerializedProperty showGizmo;
	SerializedProperty updateAllMapTilesEveryFrame;
	SerializedProperty showMapPartsGizmo;
	SerializedProperty useSameLineColor;
	SerializedProperty mapLinesColor;
	SerializedProperty gizmoLabelColor;
	SerializedProperty showMapPartsTextGizmo;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty showVertexHandles;
	SerializedProperty showBuildingTriggersLine;
	SerializedProperty buildingTriggersLineColor;
	SerializedProperty showBuildingTriggers;
	SerializedProperty buildingTriggersColor;
	SerializedProperty showBuildingTriggersCubes;
	SerializedProperty buildingTriggersCubesColor;
	SerializedProperty showFloorTriggersLine;
	SerializedProperty floorTriggersLineColor;
	SerializedProperty showFloorTriggers;
	SerializedProperty floorTriggersColor;
	SerializedProperty showFloorTriggersCubes;
	SerializedProperty floorTriggersCubesColor;
	SerializedProperty showMapPartEnabledTrigger;
	SerializedProperty generate3dMeshesActive;
	SerializedProperty generate3dMeshesShowGizmo;
	SerializedProperty generateFull3dMapMeshes;
	SerializedProperty mapPart3dHeight;
	SerializedProperty mapPart3dOffset;
	SerializedProperty buildingListProp;
	SerializedProperty mapSystemInfoList;
	SerializedProperty mapIconTypes;

	SerializedProperty show3dSettings;

	mapCreator manager;
	bool shapeChangedSinceLastRepaint;

	List<mapCreator.buildingInfo> buildingList = new List<mapCreator.buildingInfo> ();

	Color buttonColor;

	string buttonMessage;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		floorMaterial = serializedObject.FindProperty ("floorMaterial");
		mapLayer = serializedObject.FindProperty ("mapLayer");
		triggerToChangeBuildingPrefab = serializedObject.FindProperty ("triggerToChangeBuildingPrefab");
		triggerToChangeFloorPrefab = serializedObject.FindProperty ("triggerToChangeFloorPrefab");
		triggerToChangeDynamicObjectPrefab = serializedObject.FindProperty ("triggerToChangeDynamicObjectPrefab");
		mapPart3dMeshMaterial = serializedObject.FindProperty ("mapPart3dMeshMaterial");
		mapPart3dMaterialColor = serializedObject.FindProperty ("mapPart3dMaterialColor");
		layerToPlaceElements = serializedObject.FindProperty ("layerToPlaceElements");
		useRaycastToPlaceElements = serializedObject.FindProperty ("useRaycastToPlaceElements");
		mapPartEnabledTriggerScale = serializedObject.FindProperty ("mapPartEnabledTriggerScale");
		enabledTriggerGizmoColor = serializedObject.FindProperty ("enabledTriggerGizmoColor");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		updateAllMapTilesEveryFrame = serializedObject.FindProperty ("updateAllMapTilesEveryFrame");
		showMapPartsGizmo = serializedObject.FindProperty ("showMapPartsGizmo");
		useSameLineColor = serializedObject.FindProperty ("useSameLineColor");
		mapLinesColor = serializedObject.FindProperty ("mapLinesColor");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		showMapPartsTextGizmo = serializedObject.FindProperty ("showMapPartsTextGizmo");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		showVertexHandles = serializedObject.FindProperty ("showVertexHandles");
		showBuildingTriggersLine = serializedObject.FindProperty ("showBuildingTriggersLine");
		buildingTriggersLineColor = serializedObject.FindProperty ("buildingTriggersLineColor");
		showBuildingTriggers = serializedObject.FindProperty ("showBuildingTriggers");
		buildingTriggersColor = serializedObject.FindProperty ("buildingTriggersColor");
		showBuildingTriggersCubes = serializedObject.FindProperty ("showBuildingTriggersCubes");
		buildingTriggersCubesColor = serializedObject.FindProperty ("buildingTriggersCubesColor");
		showFloorTriggersLine = serializedObject.FindProperty ("showFloorTriggersLine");
		floorTriggersLineColor = serializedObject.FindProperty ("floorTriggersLineColor");
		showFloorTriggers = serializedObject.FindProperty ("showFloorTriggers");
		floorTriggersColor = serializedObject.FindProperty ("floorTriggersColor");
		showFloorTriggersCubes = serializedObject.FindProperty ("showFloorTriggersCubes");
		floorTriggersCubesColor = serializedObject.FindProperty ("floorTriggersCubesColor");
		showMapPartEnabledTrigger = serializedObject.FindProperty ("showMapPartEnabledTrigger");
		generate3dMeshesActive = serializedObject.FindProperty ("generate3dMeshesActive");
		generate3dMeshesShowGizmo = serializedObject.FindProperty ("generate3dMeshesShowGizmo");
		generateFull3dMapMeshes = serializedObject.FindProperty ("generateFull3dMapMeshes");
		mapPart3dHeight = serializedObject.FindProperty ("mapPart3dHeight");
		mapPart3dOffset = serializedObject.FindProperty ("mapPart3dOffset");
		buildingListProp = serializedObject.FindProperty ("buildingList");
		mapSystemInfoList = serializedObject.FindProperty ("mapSystemInfoList");
		mapIconTypes = serializedObject.FindProperty ("mapIconTypes");

		show3dSettings = serializedObject.FindProperty ("show3dSettings");

		manager = (mapCreator)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				buildingList = manager.buildingList;

				for (int i = 0; i < buildingList.Count; i++) {
					for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
						if (manager.buildingList [i].buildingFloorsList [j].floor != null) {
							Vector3 floorPosition = buildingList [i].buildingFloorsList [j].floor.transform.position;
							Handles.color = Color.red;
							Handles.Label (floorPosition, buildingList [i].buildingFloorsList [j].Name);					

							if (manager.useHandleForVertex) {
								Handles.color = manager.gizmoLabelColor;
								EditorGUI.BeginChangeCheck ();

								Vector3 oldPoint = floorPosition;
								Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (buildingList [i].buildingFloorsList [j].floor.transform, "move Floor");
									buildingList [i].buildingFloorsList [j].floor.transform.position = newPoint;
								}   
							}
						}

						if (manager.showFloorTriggers) {
							for (int k = 0; k < buildingList [i].buildingFloorsList [j].triggerToChangeFloorList.Count; k++) {
								Gizmos.color = Color.green;
								Handles.Label (buildingList [i].buildingFloorsList [j].triggerToChangeFloorList [k].transform.position, 
									buildingList [i].buildingFloorsList [j].Name + " Trigger " + j);					
							}
						}
					}

					if (manager.showBuildingTriggers) {
						for (int k = 0; k < buildingList [i].triggerToChangeBuildingList.Count; k++) {
							Gizmos.color = Color.green;
							Handles.Label (buildingList [i].triggerToChangeBuildingList [k].transform.position, 
								buildingList [i].Name + " Trigger " + k);					
						}
					}
				}
			}

			if (manager.updateAllMapTilesEveryFrame) {
				DrawMap ();
			} else {
				Event guiEvent = Event.current;

				if (guiEvent.type == EventType.Repaint) {
					DrawMap ();
				}
			}
		}
	}

	public void DrawMap ()
	{
		if (shapeChangedSinceLastRepaint || manager.updateAllMapTilesEveryFrame) {
			manager.calculateAllMapTileMesh ();
		}

		shapeChangedSinceLastRepaint = false;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (floorMaterial);
		EditorGUILayout.PropertyField (mapLayer);
		EditorGUILayout.PropertyField (triggerToChangeBuildingPrefab);
		EditorGUILayout.PropertyField (triggerToChangeFloorPrefab);	
		EditorGUILayout.PropertyField (triggerToChangeDynamicObjectPrefab);

		EditorGUILayout.PropertyField (mapPart3dMeshMaterial);
		EditorGUILayout.PropertyField (mapPart3dMaterialColor);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (layerToPlaceElements);
		EditorGUILayout.PropertyField (useRaycastToPlaceElements);
		EditorGUILayout.PropertyField (mapPartEnabledTriggerScale);
		EditorGUILayout.PropertyField (enabledTriggerGizmoColor);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (updateAllMapTilesEveryFrame);
			EditorGUILayout.PropertyField (showMapPartsGizmo);
			EditorGUILayout.PropertyField (useSameLineColor);
			EditorGUILayout.PropertyField (mapLinesColor);	
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (showMapPartsTextGizmo);	

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Handle Vertex Settings", "window");
			EditorGUILayout.PropertyField (useHandleForVertex);
			if (useHandleForVertex.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
			}
			EditorGUILayout.PropertyField (showVertexHandles);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Building Triggers Settings", "window");
			EditorGUILayout.PropertyField (showBuildingTriggersLine);
			if (showBuildingTriggersLine.boolValue) {
				EditorGUILayout.PropertyField (buildingTriggersLineColor);
			}
			EditorGUILayout.PropertyField (showBuildingTriggers);
			if (showBuildingTriggers.boolValue) {
				EditorGUILayout.PropertyField (buildingTriggersColor);
			}
			EditorGUILayout.PropertyField (showBuildingTriggersCubes);	
			if (showBuildingTriggersCubes.boolValue) {
				EditorGUILayout.PropertyField (buildingTriggersCubesColor);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Floors Triggers Settings", "window");
			EditorGUILayout.PropertyField (showFloorTriggersLine);
			if (showFloorTriggersLine.boolValue) {
				EditorGUILayout.PropertyField (floorTriggersLineColor);
			}
			EditorGUILayout.PropertyField (showFloorTriggers);
			if (showFloorTriggers.boolValue) {
				EditorGUILayout.PropertyField (floorTriggersColor);
			}
			EditorGUILayout.PropertyField (showFloorTriggersCubes);	
			if (showFloorTriggersCubes.boolValue) {
				EditorGUILayout.PropertyField (floorTriggersCubesColor);
			}

			EditorGUILayout.PropertyField (showMapPartEnabledTrigger);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Building List", "window");
		showBuildingList (buildingListProp);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map System List Settings", "window");
		showMapSystemInfoList (mapSystemInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Icons List", "window");
		showMapIconsTypesList (mapIconTypes);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Map Icons On Map System")) {
			manager.updateMapIconTypesOnMapSystem ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Object Information Settings", "window");
		if (GUILayout.Button ("Set Map Object Information ID")) {
			manager.setMapObjectInformationID ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Map System On Map Objects")) {
			manager.updateMapObjectInformationMapSystem ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Tiles Settings", "window");
		if (GUILayout.Button ("Generate/Update All Map Tiles")) {
			shapeChangedSinceLastRepaint = true;
			SceneView.RepaintAll ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Remove All Map Tiles")) {
			manager.removeAllMapTileMesh ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();

		if (show3dSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide 3d Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonMessage = "Show 3d Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			show3dSettings.boolValue = !show3dSettings.boolValue;
		}
		EditorGUILayout.EndVertical ();

		GUI.backgroundColor = buttonColor;

		if (show3dSettings.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Map Part 3d Mesh Settings", "window");
			EditorGUILayout.PropertyField (generate3dMeshesActive);
			if (generate3dMeshesActive.boolValue) {
				EditorGUILayout.PropertyField (generate3dMeshesShowGizmo);
				EditorGUILayout.PropertyField (generateFull3dMapMeshes);
				EditorGUILayout.PropertyField (mapPart3dHeight);
				EditorGUILayout.PropertyField (mapPart3dOffset);

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Enable 3d Mesh")) {
					manager.enableOrDisableMapPart3dMesh (true);
				}

				if (GUILayout.Button ("Disable 3d Mesh")) {
					manager.enableOrDisableMapPart3dMesh (false);
				}

				if (GUILayout.Button ("Update Mesh Position")) {
					manager.updateMapPart3dMeshPosition ();
				}

				if (GUILayout.Button ("Generate 3d Mesh")) {
					manager.generateMapPart3dMesh ();
				}

				if (GUILayout.Button ("Remove 3d Mesh")) {
					manager.removeMapPart3dMesh ();
				}

				if (GUILayout.Button ("Enable/Disable 3d Generation")) {
					manager.setGenerate3dMapPartMeshStateFromEditor ();
				}
			}

			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showBuildingList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Buildings: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Building")) {
				manager.addNewBuilding ();
			}
			if (GUILayout.Button ("Clear List")) {
				manager.removeAllBuildings ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Buildings")) {
				manager.enableOrDisableAllBuildings (false);
			}
			if (GUILayout.Button ("Enable All Buildings")) {
				manager.enableOrDisableAllBuildings (true);
			}
			GUILayout.EndHorizontal ();
		
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set All Buildings Info")) {
				manager.getAllBuildings ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showBuildingListInfo (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removeBuilding (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showBuildingListInfo (SerializedProperty list, int buildingIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buildingMapParent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buildingFloorsParent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isInterior"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentMap"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buildingMapEnabled"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraPositionOnMapMenu"));
		if (list.FindPropertyRelative ("useCameraPositionOnMapMenu").boolValue) {
			if (!list.FindPropertyRelative ("cameraPositionOnMapMenu").objectReferenceValue) {
				if (GUILayout.Button ("Add Camera Position On Map Menu")) {
					manager.addCameraPositionOnMapMenu (buildingIndex);
				}
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraPositionOnMapMenu"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraOffset"));
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rename Building")) {
			manager.renameBuilding (buildingIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger To Change Building", "window");
		showChangeBuildingTriggerList (list.FindPropertyRelative ("triggerToChangeBuildingList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger For Dynamic Objects", "window");
		showDynamicObjectsTriggerList (list.FindPropertyRelative ("triggerForDynamicObjectsList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Building Floors List", "window");
		showFloorList (list.FindPropertyRelative ("buildingFloorsList"), buildingIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}


	void showFloorListInfo (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floor"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorEnabled"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rename Floor")) {
			manager.renameFloor (buildingIndex, floorIndex);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Trigger For Dynamic Object")) {
			manager.addTriggerToDynamicObject (buildingIndex, floorIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Trigger To Change Floor", "window");
		showChangeFloorTriggerList (list.FindPropertyRelative ("triggerToChangeFloorList"), buildingIndex, floorIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Parts List", "window");
		showMapPartsList (list.FindPropertyRelative ("mapPartsList"), buildingIndex, floorIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showFloorList (SerializedProperty list, int buildingIndex)
	{
		GUILayout.BeginVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Floors: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor")) {
				manager.addNewFloor (buildingIndex);
			}
			if (GUILayout.Button ("Clear List")) {
				manager.removeAllFloors (buildingIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Floors")) {
				manager.enableOrDisableBuilding (false, buildingIndex);
			}
			if (GUILayout.Button ("Enable All Floors")) {
				manager.enableOrDisableBuilding (true, buildingIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFloorListInfo (list.GetArrayElementAtIndex (i), buildingIndex, i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removeFloor (buildingIndex, i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showMapPartsList (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Parts: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor Part")) {
				manager.addNewMapPartFromMapCreator (buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllMapParts (buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get All Floor Parts")) {
				manager.GetAllFloorParts (buildingIndex, floorIndex);
			}

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disable All Floor Parts")) {
				manager.enableOrDisableAllFloorParts (false, buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Enable All Floor Parts")) {
				manager.enableOrDisableAllFloorParts (true, buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();

			if (generate3dMeshesActive.boolValue) {
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable All Floor Parts Generate 3d Mesh")) {
					manager.setGenerate3dMapPartMeshStateToFloorFromEditor (false, buildingIndex, floorIndex);
				}
				if (GUILayout.Button ("Enable All Floor Generate 3d Mesh")) {
					manager.setGenerate3dMapPartMeshStateToFloorFromEditor (true, buildingIndex, floorIndex);
				}
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ("box");
				if (i < list.arraySize && i >= 0) {
					GUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
					GUILayout.BeginHorizontal ();
					if (GUILayout.Button ("x")) {
						manager.removeMapPart (buildingIndex, floorIndex, i);
						return;
					}

					if (GUILayout.Button ("Enable Map Part")) {
						manager.enableOrDisableFloorPart (true, buildingIndex, floorIndex, i);
					}
					if (GUILayout.Button ("Disable Map Part")) {
						manager.enableOrDisableFloorPart (false, buildingIndex, floorIndex, i);
					}
					if (GUILayout.Button ("o")) {
						manager.selectCurrentMapPart (buildingIndex, floorIndex, i);
					}
					GUILayout.EndHorizontal ();

					GameObject floorPart = list.GetArrayElementAtIndex (i).objectReferenceValue as GameObject;
					if (floorPart) {
						mapTileBuilder currentMapTile = floorPart.GetComponent<mapTileBuilder> ();

						EditorGUILayout.Space ();

						GUILayout.BeginVertical ();
						currentMapTile.mapPartEnabled = EditorGUILayout.Toggle ("Enabled", currentMapTile.mapPartEnabled);
						if (generate3dMeshesActive.boolValue) {
							currentMapTile.generate3dMapPartMesh = EditorGUILayout.Toggle ("Generate 3d mesh", currentMapTile.generate3dMapPartMesh);
						}
						EditorUtility.SetDirty (currentMapTile);
						GUILayout.EndVertical ();
					}

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showChangeFloorTriggerList (SerializedProperty list, int buildingIndex, int floorIndex)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Triggers: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Trigger")) {
				manager.addTriggerToChangeFloor (buildingIndex, floorIndex);
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeTriggerToChangeFloorList (buildingIndex, floorIndex);
			}
			GUILayout.EndHorizontal ();
		
			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					manager.removeTriggerToChangeFloor (buildingIndex, floorIndex, i);
				}
				if (GUILayout.Button ("O")) {
					manager.selectCurrentFloorTrigger (buildingIndex, floorIndex, i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showChangeBuildingTriggerList (SerializedProperty list, int buildingIndex)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Triggers: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Trigger")) {
				manager.addTriggerToChangeBuilding (buildingIndex);
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeTriggerToChangeBuildingList (buildingIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					manager.removeTriggerToChangeBuilding (buildingIndex, i);
				}
				if (GUILayout.Button ("O")) {
					manager.selectChangeBuildingTrigger (buildingIndex, i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showDynamicObjectsTriggerList (SerializedProperty list, int buildingIndex)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Triggers: " + list.arraySize);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clear")) {
				manager.removeTriggerToDynamicObjectList (buildingIndex);
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					manager.removeTriggerToDynamicObject (buildingIndex, i);
				}
				if (GUILayout.Button ("O")) {
					manager.selectDynamicObjectsTrigger (buildingIndex, i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showMapSystemInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Map System: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Map")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();
		
			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showMapSystemInfoListElement (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showMapSystemInfoListElement (SerializedProperty list, int mapSystemIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainMapSystem"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Map Icon Types From This Map System")) {
			manager.setMapIconTypes (mapSystemIndex);
		}
		GUILayout.EndVertical ();
	}

	void showMapIconsTypesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Map Icons: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Icon")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showMapIconsTypesListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showMapIconsTypesListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("typeName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showIconPreview"));
		bool showIconPreview = list.FindPropertyRelative ("showIconPreview").boolValue;
		if (showIconPreview) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Icon Preview \t");
			GUILayout.BeginHorizontal ("box", GUILayout.Width (50));
			if (list.FindPropertyRelative ("icon").objectReferenceValue) {
				RectTransform icon = list.FindPropertyRelative ("icon").objectReferenceValue as RectTransform;
				Object texture = new Object ();
				if (icon.GetComponent<RawImage> ()) {
					texture = icon.GetComponent<RawImage> ().texture;
				} else if (icon.GetComponent<Image> ()) {
					texture = icon.GetComponent<Image> ().sprite;
				}
				Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
				GUILayout.Label (myTexture, GUILayout.Width (50));
			}
			GUILayout.EndHorizontal ();
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCompassIcon"));
		if (list.FindPropertyRelative ("useCompassIcon").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("compassIconPrefab"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalOffset"));
		}

		GUILayout.EndVertical ();
	}

}
#endif