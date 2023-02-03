using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(cameraCaptureSystem))]
public class cameraCaptureSystemEditor : Editor
{
	cameraCaptureSystem manager;

	void OnEnable ()
	{
		manager = (cameraCaptureSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Take Capture")) {
			manager.takeCaptureWithCameraEditor ();
		}

		EditorGUILayout.Space ();
	}
}
#endif