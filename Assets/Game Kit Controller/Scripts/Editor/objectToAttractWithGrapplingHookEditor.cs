using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(objectToAttractWithGrapplingHook))]
public class objectToAttractWithGrapplingHookEditor : Editor
{
	objectToAttractWithGrapplingHook manager;

	void OnEnable ()
	{
		manager = (objectToAttractWithGrapplingHook)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Rigidbody Elements")) {
			manager.searchRigidbodyElements ();
		}

		EditorGUILayout.Space ();
	}
}
#endif