using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapTileBuilder))]
[CanEditMultipleObjects]
public class mapTileBuilderEditor : Editor
{
	SerializedProperty eventTriggerList;
	SerializedProperty textMeshList;
	SerializedProperty mapTileCreated;
	SerializedProperty mapPartParent;
	SerializedProperty mapPartBuildingIndex;
	SerializedProperty mapPartFloorIndex;
	SerializedProperty mapPartIndex;
	SerializedProperty mapPartRendererOffset;
	SerializedProperty newPositionOffset;
	SerializedProperty mapPartEnabled;
	SerializedProperty useOtherColorIfMapPartDisabled;
	SerializedProperty colorIfMapPartDisabled;
	SerializedProperty showGizmo;
	SerializedProperty showEnabledTrigger;
	SerializedProperty showVerticesDistance;
	SerializedProperty mapLinesColor;
	SerializedProperty mapPartMaterialColor;
	SerializedProperty cubeGizmoScale;
	SerializedProperty gizmoLabelColor;
	SerializedProperty showVertexHandles;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty mapPartName;
	SerializedProperty verticesPosition;
	SerializedProperty extraMapPartsToActive;
	SerializedProperty generate3dMapPartMesh;
	SerializedProperty onlyUse3dMapPartMesh;
	SerializedProperty generate3dMeshesShowGizmo;
	SerializedProperty mapPart3dMeshCreated;
	SerializedProperty mapPart3dHeight;
	SerializedProperty mapPart3dOffset;
	SerializedProperty mapPart3dGameObject;

	mapTileBuilder builder;
	GUIStyle style = new GUIStyle ();
	Vector3 center;
	Vector3 currentVertexPosition;
	Quaternion currentVertexRotation;
	Transform currentVertex;

	Vector3 oldPoint;
	Vector3 newPoint;
	float distance;
	Vector3 snapValue = new Vector3 (.25f, .25f, .25f);
	string currentName;

	string eventTriggerAdded;
	string textMeshAdded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		eventTriggerList = serializedObject.FindProperty ("eventTriggerList");
		textMeshList = serializedObject.FindProperty ("textMeshList");
		mapTileCreated = serializedObject.FindProperty ("mapTileCreated");
		mapPartParent = serializedObject.FindProperty ("mapPartParent");
		mapPartBuildingIndex = serializedObject.FindProperty ("mapPartBuildingIndex");
		mapPartFloorIndex = serializedObject.FindProperty ("mapPartFloorIndex");
		mapPartIndex = serializedObject.FindProperty ("mapPartIndex");
		mapPartRendererOffset = serializedObject.FindProperty ("mapPartRendererOffset");
		newPositionOffset = serializedObject.FindProperty ("newPositionOffset");
		mapPartEnabled = serializedObject.FindProperty ("mapPartEnabled");
		useOtherColorIfMapPartDisabled = serializedObject.FindProperty ("useOtherColorIfMapPartDisabled");
		colorIfMapPartDisabled = serializedObject.FindProperty ("colorIfMapPartDisabled");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showEnabledTrigger = serializedObject.FindProperty ("showEnabledTrigger");
		showVerticesDistance = serializedObject.FindProperty ("showVerticesDistance");
		mapLinesColor = serializedObject.FindProperty ("mapLinesColor");
		mapPartMaterialColor = serializedObject.FindProperty ("mapPartMaterialColor");
		cubeGizmoScale = serializedObject.FindProperty ("cubeGizmoScale");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		showVertexHandles = serializedObject.FindProperty ("showVertexHandles");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		mapPartName = serializedObject.FindProperty ("mapPartName");
		verticesPosition = serializedObject.FindProperty ("verticesPosition");
		extraMapPartsToActive = serializedObject.FindProperty ("extraMapPartsToActive");
		generate3dMapPartMesh = serializedObject.FindProperty ("generate3dMapPartMesh");
		onlyUse3dMapPartMesh = serializedObject.FindProperty ("onlyUse3dMapPartMesh");
		generate3dMeshesShowGizmo = serializedObject.FindProperty ("generate3dMeshesShowGizmo");
		mapPart3dMeshCreated = serializedObject.FindProperty ("mapPart3dMeshCreated");
		mapPart3dHeight = serializedObject.FindProperty ("mapPart3dHeight");
		mapPart3dOffset = serializedObject.FindProperty ("mapPart3dOffset");
		mapPart3dGameObject = serializedObject.FindProperty ("mapPart3dGameObject");

		builder = (mapTileBuilder)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (builder.showGizmo && builder.mapManager.showMapPartsGizmo) {
				if (builder.eventTriggerList.Count > 0) {
					style.normal.textColor = builder.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;

					if (builder.showEnabledTrigger && builder.mapManager.showMapPartEnabledTrigger) {
						for (int i = 0; i < builder.eventTriggerList.Count; i++) {
							if (builder.eventTriggerList [i]) {
								Handles.Label (builder.eventTriggerList [i].transform.position, "Event\n Trigger " + (i + 1), style);
								if (builder.useHandleForVertex) {
									Handles.color = builder.gizmoLabelColor;
									EditorGUI.BeginChangeCheck ();

									oldPoint = builder.eventTriggerList [i].transform.position;
									newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, builder.handleRadius, snapValue, Handles.CircleHandleCap);
									if (EditorGUI.EndChangeCheck ()) {
										Undo.RecordObject (builder.eventTriggerList [i].transform, "move Trigger" + i);
										builder.eventTriggerList [i].transform.position = newPoint;
									}   
								}
							}
						}
					}
				}

				if (builder.verticesPosition.Count > 0) {
					style.normal.textColor = builder.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;

					for (int i = 0; i < builder.verticesPosition.Count; i++) {
						if (builder.verticesPosition [i]) {
							currentVertex = builder.verticesPosition [i].transform;
							currentVertexPosition = currentVertex.position;

							Handles.Label (currentVertexPosition, currentVertex.name, style);

							if (builder.showVerticesDistance) {
								if (i + 1 < builder.verticesPosition.Count) {
									if (builder.verticesPosition [i + 1] != null) {
										center = Vector3.zero;
										center += currentVertexPosition;
										center += builder.verticesPosition [i + 1].position;
										center /= 2;
										distance = GKC_Utils.distance (currentVertexPosition, builder.verticesPosition [i + 1].position);
										Handles.Label (center, distance + " m", style);
									}

								}
								if (i == builder.verticesPosition.Count - 1) {
									if (builder.verticesPosition [0] != null) {
										center = Vector3.zero;
										center += currentVertexPosition;
										center += builder.verticesPosition [0].position;
										center /= 2;
										distance = GKC_Utils.distance (currentVertexPosition, builder.verticesPosition [0].position);
										Handles.Label (center, distance + " m", style);
									}
								}
							}

							if (builder.useHandleForVertex) {
								Handles.color = builder.gizmoLabelColor;
								EditorGUI.BeginChangeCheck ();

								oldPoint = currentVertexPosition;
								newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, builder.handleRadius, snapValue, Handles.CircleHandleCap);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (currentVertex, "move Handle" + i);
									currentVertex.transform.position = newPoint;
								}   
							}

							if (builder.showVertexHandles || builder.mapManager.showVertexHandles) {
								currentVertexRotation = Tools.pivotRotation == PivotRotation.Local ? currentVertex.rotation : Quaternion.identity;
								//Handles.DoPositionHandle (currentVertexPosition, currentVertexRotation);

								EditorGUI.BeginChangeCheck ();

								oldPoint = currentVertex.position;
								oldPoint = Handles.DoPositionHandle (oldPoint, currentVertexRotation);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (currentVertex, "move Vertex" + i);
									currentVertex.position = oldPoint;
								}
							}
						}
					}
				}
			}
			currentName = builder.gameObject.name;
			currentName = currentName.Substring (0, 3);
			Handles.Label (builder.center, "Part\n" + currentName, style);

			if (builder.generate3dMapPartMesh) {
				Handles.Label (builder.center + builder.mapPart3dOffset + builder.mapPart3dHeight * Vector3.up, "3d height\n" + currentName, style);
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical (GUILayout.Height (30));

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Map Part State", "window");
		eventTriggerAdded = "NO";
		if (eventTriggerList.arraySize > 0) {
			eventTriggerAdded = "YES";
		}

		GUILayout.Label ("Event Trigger Added \t\t" + eventTriggerAdded);
		textMeshAdded = "NO";
		if (textMeshList.arraySize > 0) {
			textMeshAdded = "YES";
		}

		GUILayout.Label ("Text Mesh Added \t\t" + textMeshAdded);
		GUILayout.Label ("Map Tile Created \t\t" + mapTileCreated.boolValue);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (mapPartParent);
		EditorGUILayout.PropertyField (mapPartBuildingIndex);
		EditorGUILayout.PropertyField (mapPartFloorIndex);
		EditorGUILayout.PropertyField (mapPartIndex);

		EditorGUILayout.PropertyField (mapPartRendererOffset);
		EditorGUILayout.PropertyField (newPositionOffset);
		EditorGUILayout.PropertyField (mapPartEnabled);
		if (!mapPartEnabled.boolValue) {
			EditorGUILayout.PropertyField (useOtherColorIfMapPartDisabled);
			if (useOtherColorIfMapPartDisabled.boolValue) {
				EditorGUILayout.PropertyField (colorIfMapPartDisabled);
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (showEnabledTrigger);
			EditorGUILayout.PropertyField (showVerticesDistance, new GUIContent ("Show Vertex Distance"), false);
			EditorGUILayout.PropertyField (mapLinesColor);
			EditorGUILayout.PropertyField (mapPartMaterialColor);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set Random Color")) {
				builder.setRandomMapPartColor ();
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (cubeGizmoScale);
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (showVertexHandles);
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Handle Vertex Settings", "window");
			EditorGUILayout.PropertyField (useHandleForVertex);
			if (useHandleForVertex.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
			}

			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rename Settings", "window");
		EditorGUILayout.PropertyField (mapPartName);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rename Map Part")) {
			builder.renameMapPart ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Transform List", "window");
		showVertexPositionList (verticesPosition, "Vertex position");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Text Mesh List", "window");
		showTextMeshList (textMeshList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!Application.isPlaying) {
			if (GUILayout.Button ("Rename All Vertex")) {
				builder.renameAllVertex ();
			}

			if (GUILayout.Button ("Reverse Vertex Order")) {
				builder.reverVertexOrder ();
			}

			if (GUILayout.Button ("Add New Map Part")) {
				builder.mapManager.addNewMapPart (builder.mapPartParent);
			}

			if (GUILayout.Button ("Duplicate Map Part")) {
				builder.mapManager.duplicateMapPart (builder.mapPartParent, builder.gameObject);
			}

			if (GUILayout.Button ("Remove Map Part")) {
				builder.mapManager.removeMapPart (builder.mapPartParent, builder.gameObject);
				return;
			}

			if (GUILayout.Button ("Add Trigger Event to Enable Map Part")) {
				builder.addEventTriggerToActive ();
			}

			EditorGUILayout.Space ();

			if (eventTriggerList.arraySize > 0) {
				GUILayout.BeginVertical ("Event Trigger List", "window");
				showEventTriggerList (eventTriggerList, "Event Trigger List");
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();
		}

		GUILayout.BeginVertical ("Extra Map Parts To Active", "window");
		showExtraMapPartsToActive (extraMapPartsToActive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Map Part 3d Mesh Settings", "window");
		EditorGUILayout.PropertyField (generate3dMapPartMesh);
		if (generate3dMapPartMesh.boolValue) {
			EditorGUILayout.PropertyField (onlyUse3dMapPartMesh);
			EditorGUILayout.PropertyField (generate3dMeshesShowGizmo);
			GUILayout.Label ("Map Part 3d Mesh Created: \t" + mapPart3dMeshCreated.boolValue);

			EditorGUILayout.PropertyField (mapPart3dHeight);
			EditorGUILayout.PropertyField (mapPart3dOffset);
			if (mapPart3dMeshCreated.boolValue) {
				EditorGUILayout.PropertyField (mapPart3dGameObject);

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Enable 3d Mesh")) {
					builder.enableOrDisableMapPart3dMesh (true);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable 3d Mesh")) {
					builder.enableOrDisableMapPart3dMesh (false);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Update Mesh Position")) {
					builder.updateMapPart3dMeshPositionFromEditor ();
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Generate 3d Mesh")) {
				builder.generateMapPart3dMeshFromEditor ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Remove 3d Mesh")) {
				builder.removeMapPart3dMeshFromEditor ();
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showVertexPositionList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Vertex: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Vertex")) {
				builder.addNewVertex (-1);
			}

			if (GUILayout.Button ("Clear")) {
				builder.removeAllVertex ();
			}

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}

				if (GUILayout.Button ("x")) {
					builder.removeVertex (i);
				}

				if (GUILayout.Button ("+")) {
					builder.addNewVertex (i);
				}

				GUILayout.EndHorizontal ();
			}
		}
	}

	void showEventTriggerList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Triggers: \t" + list.arraySize);

			EditorGUILayout.Space ();
			
			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Event Trigger")) {
				builder.addEventTriggerToActive ();
			}
			if (GUILayout.Button ("Clear")) {
				builder.removeAllEventTriggers ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Enable Event Triggers")) {
				builder.enableOrDisableEventTriggerList (true);
			}
			if (GUILayout.Button ("Disable Event Triggers")) {
				builder.enableOrDisableEventTriggerList (false);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					builder.removeEventTrigger (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showTextMeshList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of TextMesh: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Text Mesh")) {
				builder.addMapPartTextMesh ();
			}
			if (GUILayout.Button ("Clear")) {
				builder.removeAllTextMesh ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					builder.removeTextMesh (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showExtraMapPartsToActive (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Extra Parts: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Extra Part")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif