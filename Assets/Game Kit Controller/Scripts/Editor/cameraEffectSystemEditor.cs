using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(cameraEffectSystem))]
public class cameraEffectSystemEditor : Editor
{
	cameraEffectSystem manager;

	void OnEnable ()
	{
		manager = (cameraEffectSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable/Disable Camera Effect Mode")) {
			manager.enableOrDisableCameraEffectActive ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Next Camera Effect")) {
			manager.setNextCameraEffect ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Previous Camera Effect")) {
			manager.setPreviousCameraEffect ();
		}

		EditorGUILayout.Space ();
	}
}
#endif