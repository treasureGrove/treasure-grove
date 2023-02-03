using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(objectiveManager))]
public class objectiveManagerEditor : Editor
{
	objectiveManager manager;

	void OnEnable ()
	{
		manager = (objectiveManager)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get All Station Systems On Level And Assign Info")) {
			manager.getAllStationSystemOnLevelAndAssignInfoToAllMissions ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get All Station Systems On Level")) {
			manager.getAllStationSystemOnLevelByEditor ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Clear Station System List")) {
			manager.clearStationSystemList ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate Mission Station")) {
			manager.instantiateMissionStation ();
		}

		EditorGUILayout.Space ();
			
		if (GUILayout.Button ("Instantiate Mission System")) {
			manager.instantiateMissionSystem ();
		}
			
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate Empty Mission Station System")) {
			manager.instantiateEmptyMissionSystem ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate Character Mission System")) {
			manager.instantiateCharacterMissionSystem ();
		}

		EditorGUILayout.Space ();
	}
}
#endif