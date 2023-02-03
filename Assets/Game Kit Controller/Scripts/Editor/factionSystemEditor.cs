using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(factionSystem))]
public class factionSystemEditor : Editor
{
	SerializedProperty factionList;
	SerializedProperty characterFactionManagerList;

	int factionIndex;

	factionSystem manager;
	bool expanded;

	string[] factionStringList;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		factionList = serializedObject.FindProperty ("factionList");
		characterFactionManagerList = serializedObject.FindProperty ("characterFactionManagerList");

		manager = (factionSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Faction List", "window");
		showFactionList (factionList);
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Character List (Debug)", "window");
		showCharacterInfo (characterFactionManagerList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showFactionList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Faction List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Factions: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Faction")) {
				manager.addFaction ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Factions List")) {
				manager.updateFactionsList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Factions List Names")) {
				manager.updateFactionListNames ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showFactionListElement (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removeFaction (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showFactionListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("turnToEnemyIfAttack"));
		if (list.FindPropertyRelative ("turnToEnemyIfAttack").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("turnFactionToEnemy"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("friendlyFireTurnIntoEnemies"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Relation With Other Factions", "window");
		showRelationWithOtherFactions (list.FindPropertyRelative ("relationWithFactions"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showRelationWithOtherFactions (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Relation With Other Factions", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Factions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showRelationWithOtherFactionsElement (list.GetArrayElementAtIndex (i), i);
					}
				}
				GUILayout.EndVertical ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showRelationWithOtherFactionsElement (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		factionStringList = manager.factionStringList;
				
		if (factionStringList.Length > 0) {

			factionIndex = list.FindPropertyRelative ("factionIndex").intValue;

			factionIndex = EditorGUILayout.Popup ("Faction ", factionIndex, factionStringList);

			if (factionIndex < factionStringList.Length) {
				list.FindPropertyRelative ("factionName").stringValue = factionStringList [factionIndex];
			}

			list.FindPropertyRelative ("factionIndex").intValue = factionIndex;

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("relation"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("factionIndex"));
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showCharacterInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Character Faction Manager List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Characters: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

}
#endif