using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(simpleEventSystem))]
public class simpleEventSystemEditor : Editor
{
	simpleEventSystem manager;

	void OnEnable ()
	{
		manager = (simpleEventSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Call Event")) {
			manager.activateDevice ();
		}

		EditorGUILayout.Space ();
	}
}
#endif