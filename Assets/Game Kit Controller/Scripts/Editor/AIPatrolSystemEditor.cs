using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(AIPatrolSystem))]
public class AIPatrolSystemEditor : Editor
{
	AIPatrolSystem manager;

	void OnEnable ()
	{
		manager = (AIPatrolSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Pause Patrol On AI")) {
			manager.pausePatrolStateOnAI ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Resume Patrol On AI")) {
			manager.resumePatrolStateOnAI ();
		}

		EditorGUILayout.Space ();
	}
}
#endif