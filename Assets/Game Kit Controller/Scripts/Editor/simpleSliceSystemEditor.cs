using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor (typeof(simpleSliceSystem))]
public class simpleSliceSystemEditor : Editor
{
	simpleSliceSystem manager;

	void OnEnable ()
	{
		manager = (simpleSliceSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Body Parts")) {
			manager.searchBodyParts ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Create Ragdoll Prefab")) {
			manager.createRagdollPrefab ();
		}

		EditorGUILayout.Space ();
	}
}