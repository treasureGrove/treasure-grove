using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(pauseOrResumePlayerControllerAndCameraSystem))]
public class pauseOrResumePlayerControllerAndCameraSystemEditor : Editor
{
	pauseOrResumePlayerControllerAndCameraSystem manager;

	void OnEnable ()
	{
		manager = (pauseOrResumePlayerControllerAndCameraSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Pause Player")) {
			manager.pauseOrPlayPlayerComponents (true);
		}

		if (GUILayout.Button ("Resume Player")) {
			manager.pauseOrPlayPlayerComponents (false);
		}
	}
}
#endif