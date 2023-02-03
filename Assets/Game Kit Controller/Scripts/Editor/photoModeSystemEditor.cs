using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(photoModeSystem))]
public class photoModeSystemEditor : Editor
{
	photoModeSystem manager;

	void OnEnable ()
	{
		manager = (photoModeSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable/Disable Photo Mode")) {
			manager.enableOrDisablePhotoMode ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Take Photo")) {
			manager.takePhoto ();
		}

		EditorGUILayout.Space ();
	}
}
#endif