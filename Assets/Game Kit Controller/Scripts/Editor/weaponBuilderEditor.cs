using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(weaponBuilder))]
public class weaponBuilderEditor : Editor
{
	SerializedProperty weaponName;
	SerializedProperty relativePathUsableWeaponPrefab;
	SerializedProperty usableWeaponPrefab;
	SerializedProperty layerToPlaceWeaponPrefab;
	SerializedProperty armsParent;
	SerializedProperty currentArmsModel;
	SerializedProperty replaceArmsModel;
	SerializedProperty newArmsModel;
	SerializedProperty weaponPartInfoList;
	SerializedProperty weaponSettingsInfoList;
	SerializedProperty buildWeaponExplanation;
	SerializedProperty showGizmo;
	SerializedProperty showGizmoLabel;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandle;
	SerializedProperty mainPlayerWeaponsManager;
	SerializedProperty weaponParent;
	SerializedProperty mainIKWeaponSystem;
	SerializedProperty mainPlayerWeaponSystem;
	SerializedProperty weaponMeshParent;
	SerializedProperty newWeaponMeshParent;
	SerializedProperty weaponViewTransform;

	SerializedProperty weaponPartsNotUsedOnAIList;

	weaponBuilder manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	GUIStyle style = new GUIStyle ();

	weaponBuilder.partInfo currentPartInfo;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		weaponName = serializedObject.FindProperty ("weaponName");
		relativePathUsableWeaponPrefab = serializedObject.FindProperty ("relativePathUsableWeaponPrefab");
		usableWeaponPrefab = serializedObject.FindProperty ("usableWeaponPrefab");
		layerToPlaceWeaponPrefab = serializedObject.FindProperty ("layerToPlaceWeaponPrefab");
		armsParent = serializedObject.FindProperty ("armsParent");
		currentArmsModel = serializedObject.FindProperty ("currentArmsModel");
		replaceArmsModel = serializedObject.FindProperty ("replaceArmsModel");
		newArmsModel = serializedObject.FindProperty ("newArmsModel");
		weaponPartInfoList = serializedObject.FindProperty ("weaponPartInfoList");
		weaponSettingsInfoList = serializedObject.FindProperty ("weaponSettingsInfoList");
		buildWeaponExplanation = serializedObject.FindProperty ("buildWeaponExplanation");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showGizmoLabel = serializedObject.FindProperty ("showGizmoLabel");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandle = serializedObject.FindProperty ("useHandle");
		mainPlayerWeaponsManager = serializedObject.FindProperty ("mainPlayerWeaponsManager");
		weaponParent = serializedObject.FindProperty ("weaponParent");
		mainIKWeaponSystem = serializedObject.FindProperty ("mainIKWeaponSystem");
		mainPlayerWeaponSystem = serializedObject.FindProperty ("mainPlayerWeaponSystem");
		weaponMeshParent = serializedObject.FindProperty ("weaponMeshParent");
		newWeaponMeshParent = serializedObject.FindProperty ("newWeaponMeshParent");
		weaponViewTransform = serializedObject.FindProperty ("weaponViewTransform");

		weaponPartsNotUsedOnAIList = serializedObject.FindProperty ("weaponPartsNotUsedOnAIList");

		manager = (weaponBuilder)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo && manager.useHandle) {
			if (!Application.isPlaying) {

				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				for (int i = 0; i < manager.weaponPartInfoList.Count; i++) {

					currentPartInfo = manager.weaponPartInfoList [i];

					if (!currentPartInfo.removeWeaponPartIfNoNewMesh) {
						if (currentPartInfo.temporalWeaponMesh != null) {
							if (manager.showGizmoLabel) {
								Handles.Label (currentPartInfo.temporalWeaponMesh.transform.position, currentPartInfo.Name, style);
							}

							showPositionHandle (currentPartInfo.temporalWeaponMesh.transform, "move temporal Weapon Mesh" + i);
						} else {
							if (currentPartInfo.objectAlwaysUsed) {
								if (currentPartInfo.currentWeaponMesh) {
									if (manager.showGizmoLabel) {
										Handles.Label (currentPartInfo.currentWeaponMesh.transform.position, currentPartInfo.Name, style);
									}

									showPositionHandle (currentPartInfo.currentWeaponMesh.transform, "move current Weapon Mesh" + i);
								}
							}
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

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (weaponName);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Usable Weapon Prefab Settings", "window");
		EditorGUILayout.PropertyField (relativePathUsableWeaponPrefab);
		EditorGUILayout.PropertyField (usableWeaponPrefab);
		EditorGUILayout.PropertyField (layerToPlaceWeaponPrefab);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Create/Update Usable Weapon Prefab")) {
			manager.createUsableWeaponPrefab ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate Weapon In Scene")) {
			manager.instantiateUsableWeaponPrefabInScene ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Arms Settings", "window");
		EditorGUILayout.PropertyField (armsParent);
		EditorGUILayout.PropertyField (currentArmsModel);
		EditorGUILayout.PropertyField (replaceArmsModel);
		if (replaceArmsModel.boolValue) {
			EditorGUILayout.PropertyField (newArmsModel);
		}
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Replace Arms")) {
			manager.checkArmsToReplace ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable/Disable Arms Meshes")) {
			manager.toogleArmsMeshActiveState ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Parts Info List", "window");
		showWeaponPartInfoList (weaponPartInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Settings List", "window");
		showWeaponSettingsInfoList (weaponSettingsInfoList);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Current Settings")) {
			manager.adjustWeaponSettingsFromEditor ();
		}
			
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons Parts Not Used On AI", "window");
		showSimpleList (weaponPartsNotUsedOnAIList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Align View To Weapon")) {
			manager.alignViewWithWeaponCameraPosition ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Open Weapon Creator Wizard")) {
			weaponCreatorEditor.createNewWeapon ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Explanation", "window");
		EditorGUILayout.PropertyField (buildWeaponExplanation);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		EditorGUILayout.PropertyField (showGizmoLabel);
		EditorGUILayout.PropertyField (gizmoLabelColor);
		EditorGUILayout.PropertyField (gizmoRadius);
		EditorGUILayout.PropertyField (useHandle);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (mainPlayerWeaponsManager);
		EditorGUILayout.PropertyField (weaponParent);
		EditorGUILayout.PropertyField (mainIKWeaponSystem);
		EditorGUILayout.PropertyField (mainPlayerWeaponSystem);
		EditorGUILayout.PropertyField (weaponMeshParent);
		EditorGUILayout.PropertyField (newWeaponMeshParent);
		EditorGUILayout.PropertyField (weaponViewTransform);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showWeaponPartInfoList (SerializedProperty list)
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
						showWeaponPartInfoListElement (list.GetArrayElementAtIndex (i), true);
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

	void showWeaponPartInfoListElement (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeWeaponPartIfNoNewMesh"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponMeshParent"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentWeaponMesh"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectAlwaysUsed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Extra Weapon Part Meshes List", "window");
		showSimpleList (list.FindPropertyRelative ("extraWeaponPartMeshesList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("IK Positions Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsIKTransform"));
		if (list.FindPropertyRelative ("containsIKTransform").boolValue) {
			showSimpleList (list.FindPropertyRelative ("IKPositionsListOnMesh"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnUseWeaponPartEnabled"));
		if (list.FindPropertyRelative ("useEventOnUseWeaponPartEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnUseWeaponPartEnabled"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnUseWeaponPartDisabled"));
		if (list.FindPropertyRelative ("useEventOnUseWeaponPartDisabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnUseWeaponPartDisabled"));
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showWeaponSettingsInfoList (SerializedProperty list)
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
						showWeaponSettingsInfoListElement (list.GetArrayElementAtIndex (i), true);
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

	void showWeaponSettingsInfoListElement (SerializedProperty list, bool showListNames)
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