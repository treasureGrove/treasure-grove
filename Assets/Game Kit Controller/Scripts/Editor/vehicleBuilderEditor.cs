using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(vehicleBuilder))]
public class vehicleBuilderEditor : Editor
{
	SerializedProperty vehicleName;
	SerializedProperty prefabsPath;
	SerializedProperty layerToPlaceVehicle;
	SerializedProperty placeVehicleOffset;
	SerializedProperty vehicle;
	SerializedProperty vehicleCamera;
	SerializedProperty mainVehicleHUDManager;
	SerializedProperty vehicleViewTransform;
	SerializedProperty vehiclePartInfoList;
	SerializedProperty vehicleSettingsInfoList;
	SerializedProperty buildVehicleExplanation;
	SerializedProperty showGizmo;
	SerializedProperty showGizmoLabel;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandle;

	vehicleBuilder manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	vehicleBuilder.vehiclePartInfo currentVehiclePartInfo;

	GUIStyle style = new GUIStyle ();

	Transform currentTransform;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		vehicleName = serializedObject.FindProperty ("vehicleName");
		prefabsPath = serializedObject.FindProperty ("prefabsPath");
		layerToPlaceVehicle = serializedObject.FindProperty ("layerToPlaceVehicle");
		placeVehicleOffset = serializedObject.FindProperty ("placeVehicleOffset");
		vehicle = serializedObject.FindProperty ("vehicle");
		vehicleCamera = serializedObject.FindProperty ("vehicleCamera");
		mainVehicleHUDManager = serializedObject.FindProperty ("mainVehicleHUDManager");
		vehicleViewTransform = serializedObject.FindProperty ("vehicleViewTransform");
		vehiclePartInfoList = serializedObject.FindProperty ("vehiclePartInfoList");
		vehicleSettingsInfoList = serializedObject.FindProperty ("vehicleSettingsInfoList");
		buildVehicleExplanation = serializedObject.FindProperty ("buildVehicleExplanation");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showGizmoLabel = serializedObject.FindProperty ("showGizmoLabel");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandle = serializedObject.FindProperty ("useHandle");

		manager = (vehicleBuilder)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo && manager.useHandle) {
			if (!Application.isPlaying) {

				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				for (int i = 0; i < manager.vehiclePartInfoList.Count; i++) {
					currentVehiclePartInfo = manager.vehiclePartInfoList [i];

					if (currentVehiclePartInfo.temporalVehicleMesh != null) {
						currentTransform = currentVehiclePartInfo.temporalVehicleMesh.transform;

						if (currentVehiclePartInfo.moveMeshParentInsteadOfPart) {
							if (currentVehiclePartInfo.vehicleMeshParent != null) {
								currentTransform = currentVehiclePartInfo.vehicleMeshParent;
							}
						}

						if (manager.showGizmoLabel) {
							Handles.Label (currentTransform.position, currentVehiclePartInfo.Name, style);
						}

						showPositionHandle (currentTransform, "move temporal Vehicle Mesh" + i);
					}

					if (currentVehiclePartInfo.showHandleTool) {
						if (currentVehiclePartInfo.currentVehicleMesh != null) {
							currentTransform = currentVehiclePartInfo.currentVehicleMesh.transform;

							if (currentVehiclePartInfo.moveMeshParentInsteadOfPart) {
								if (currentVehiclePartInfo.vehicleMeshParent != null) {
									currentTransform = currentVehiclePartInfo.vehicleMeshParent;
								}
							}

							if (manager.showGizmoLabel) {
								Handles.Label (currentTransform.position, currentVehiclePartInfo.Name, style);
							}
								
							showPositionHandle (currentTransform.transform, "move current Vehicle Mesh" + i);
						}
					}
				}
			}
		}
	}

	public void showPositionHandle (Transform currentTransform, string handleName)
	{
		currentRotationHandle = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

		EditorGUI.BeginChangeCheck ();

		curretPositionHandle = currentTransform.position;

		if (Tools.current == Tool.Move) {
			curretPositionHandle = Handles.DoPositionHandle (curretPositionHandle, currentRotationHandle);
		}

		currentRotationHandle = currentTransform.rotation;

		if (Tools.current == Tool.Rotate) {
			currentRotationHandle = Handles.DoRotationHandle (currentRotationHandle, curretPositionHandle);
		}

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (currentTransform, handleName);
			currentTransform.position = curretPositionHandle;
			currentTransform.rotation = currentRotationHandle;
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (50));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (vehicleName);
		EditorGUILayout.PropertyField (prefabsPath);
		EditorGUILayout.PropertyField (layerToPlaceVehicle);
		EditorGUILayout.PropertyField (placeVehicleOffset);

		EditorGUILayout.PropertyField (vehicle);
		EditorGUILayout.PropertyField (vehicleCamera);
		EditorGUILayout.PropertyField (mainVehicleHUDManager);
		EditorGUILayout.PropertyField (vehicleViewTransform);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Parts Info List", "window");
		showVehiclePartInfoList (vehiclePartInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Settings List", "window");
		showVehicleSettingsInfoList (vehicleSettingsInfoList);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Current Settings")) {
			manager.adjustVehicleSettingsFromEditor ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Align View To Vehicle")) {
			manager.alignViewWithVehicleCameraPosition ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Open Vehicle Creator Wizard")) {
			vehicleCreatorEditor.createNewVehicle ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Explanation", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (buildVehicleExplanation);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (showGizmo);
		EditorGUILayout.PropertyField (showGizmoLabel);
		EditorGUILayout.PropertyField (gizmoLabelColor);
		EditorGUILayout.PropertyField (gizmoRadius);
		EditorGUILayout.PropertyField (useHandle);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showVehiclePartInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Parts: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Part")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showVehiclePartInfoListElement (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showVehiclePartInfoListElement (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useVehiclePart"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("moveMeshParentInsteadOfPart"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("vehicleMeshParent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentVehicleMesh"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectAlwaysUsed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Extra Vehicle Part Meshes List", "window");
		showSimpleList (list.FindPropertyRelative ("extraVehiclePartMeshesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnUseVehiclePartEnabled"));
		if (list.FindPropertyRelative ("useEventOnUseVehiclePartEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnUseVehiclePartEnabled"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnUseVehiclePartDisabled"));
		if (list.FindPropertyRelative ("useEventOnUseVehiclePartDisabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnUseVehiclePartDisabled"));
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showVehicleSettingsInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Settings: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Settings")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showVehicleSettingsInfoListElement (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showVehicleSettingsInfoListElement (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		GUILayout.BeginVertical ("Bool Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoolState"));
		if (list.FindPropertyRelative ("useBoolState").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolState"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetBoolState"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Float Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFloatValue"));
		if (list.FindPropertyRelative ("useFloatValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("floatValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetFloatValue"));
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
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
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif