using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(bodyMountPointsSystem))]
public class bodyMountPointsSystemEditor : Editor
{
	bodyMountPointsSystem manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	void OnEnable ()
	{
		manager = (bodyMountPointsSystem)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo && manager.editingMountPoint) {
			if (!Application.isPlaying) {
				if (manager.temporalMountPointTransform != null) {
					showPositionHandle (manager.temporalMountPointTransform.transform, "Edit Temporal Mount Point Transform");
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

		if (GUILayout.Button ("Toggle Edit Mount Point")) {
			manager.toggleEditMountPoint ();
		}
			
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Handle Gizmo")) {
			manager.toggleShowHandleGizmo ();
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