using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(addPatrolSystemToAI))]
public class addPatrolSystemToAIEditor : Editor
{
	addPatrolSystemToAI manager;

	void OnEnable ()
	{
		manager = (addPatrolSystemToAI)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Patrol System To AI")) {
			manager.addPatrolSystem ();
		}

		EditorGUILayout.Space ();
	}
}
#endif