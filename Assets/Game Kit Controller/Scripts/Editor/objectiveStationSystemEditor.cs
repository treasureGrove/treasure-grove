using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(objectiveStationSystem))]
public class objectiveStationSystemEditor : Editor
{
	objectiveStationSystem manager;

	void OnEnable ()
	{
		manager = (objectiveStationSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get All Missions Systems On Level")) {
			manager.getAllMissionsSystemOnLevel ();
		}
		EditorGUILayout.Space ();
	}
}
#endif