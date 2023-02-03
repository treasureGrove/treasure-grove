using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(meleeWeaponTransformInfoSystem))]
public class meleeWeaponTransformInfoSystemEditor : Editor
{
	meleeWeaponTransformInfoSystem manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	void OnEnable ()
	{
		manager = (meleeWeaponTransformInfoSystem)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo) {
			if (Application.isPlaying) {
				if (manager.currentWeaponObjectTransform != null) {
					showPositionHandle (manager.currentWeaponObjectTransform.transform, "Edit Melee Weapon Hand Transform Values");
				}

				if (manager.currentWeaponMeshGameObject != null) {
					showPositionHandle (manager.currentWeaponMeshGameObject.transform, "Edit Melee Weapon Mesh Transform Values");
				}
			}
		}
	}


	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Copy Transform Values")) {
			manager.copyTransformValuesToBuffer ();
		}
		if (GUILayout.Button ("Paste Transform Values")) {
			manager.pasteTransformValuesToBuffer ();
		}

		if (GUILayout.Button ("Clear Values")) {
			manager.cleanPositionsOnScriptable ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Toggle Edit Weapon Values In-Game")) {
			manager.toggleEditWeaponTransformValuesIngameStateState ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Handle Gizmo")) {
			manager.toggleShowHandleGizmo ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Select Current Weapon Object In-Game")) {
			manager.selectCurrentWeaponObjectInGame ();
		}

		EditorGUILayout.Space ();
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
}
#endif