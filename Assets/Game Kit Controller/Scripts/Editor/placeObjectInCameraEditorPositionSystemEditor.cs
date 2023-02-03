using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(placeObjectInCameraEditorPositionSystem))]
public class placeObjectInCameraEditorPositionSystemEditor : Editor
{
	placeObjectInCameraEditorPositionSystem manager;

	void OnEnable ()
	{
		manager = (placeObjectInCameraEditorPositionSystem)target;
	}

	GUIStyle style = new GUIStyle ();


	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField ("EDITOR BUTTONS", style);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Move Object To Camera Position")) {
			manager.moveObjects ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Adjust Offset To Object Bounds")) {
			manager.setOffsetFromRendererBound ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Snap To Ground Surface")) {
			manager.snapToGrounSurface ();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Rotate Object To Left")) {
			manager.rotateObject (-1);
		}

		if (GUILayout.Button ("Rotate Object To Right")) {
			manager.rotateObject (1);
		}

		if (GUILayout.Button ("Reset Rotation")) {
			manager.resetObjectRotation ();
		}

		if (GUILayout.Button ("Select Object")) {
			manager.selectObject ();
		}

		EditorGUILayout.Space ();
	}
}
#endif