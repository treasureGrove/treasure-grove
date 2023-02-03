using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(characterFactionManager))]
public class characterFactionManagerEditor : Editor
{
	SerializedProperty factionManager;

	SerializedProperty characterTransform;
	SerializedProperty checkForFriendlyFactionAttackers;
	SerializedProperty changeFactionRelationWithFriendlyAttackers;
	SerializedProperty factionStringList;
	SerializedProperty factionIndex;
	SerializedProperty factionName;
	SerializedProperty currentDetectedEnemyList;

	SerializedProperty mainManagerName;

	characterFactionManager manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		factionManager = serializedObject.FindProperty ("factionManager");
		characterTransform = serializedObject.FindProperty ("characterTransform");

		checkForFriendlyFactionAttackers = serializedObject.FindProperty ("checkForFriendlyFactionAttackers");
		changeFactionRelationWithFriendlyAttackers = serializedObject.FindProperty ("changeFactionRelationWithFriendlyAttackers");
		factionStringList = serializedObject.FindProperty ("factionStringList");
		factionIndex = serializedObject.FindProperty ("factionIndex");
		factionName = serializedObject.FindProperty ("factionName");
		currentDetectedEnemyList = serializedObject.FindProperty ("currentDetectedEnemyList");

		mainManagerName = serializedObject.FindProperty ("mainManagerName");

		manager = (characterFactionManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical (GUILayout.Height (30));

		GUILayout.BeginVertical ("Main Components", "window");

		EditorGUILayout.PropertyField (factionManager);

		EditorGUILayout.PropertyField (characterTransform);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");

		EditorGUILayout.PropertyField (checkForFriendlyFactionAttackers);
		EditorGUILayout.PropertyField (changeFactionRelationWithFriendlyAttackers);

		EditorGUILayout.PropertyField (mainManagerName);
	
		EditorGUILayout.Space ();

		if (factionStringList.arraySize > 0) {
			factionIndex.intValue = EditorGUILayout.Popup ("Faction ", factionIndex.intValue, manager.getFactionStringList ());

			if (factionIndex.intValue < manager.factionStringList.Length) {
				factionName.stringValue = manager.factionStringList [factionIndex.intValue];
			}
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Detected Enemy List (Debug)", "window");
		showSimpleList (currentDetectedEnemyList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Faction List")) {
			manager.getFactionListFromEditor ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize);

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif