using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(gameManager))]
public class gameManagerEditor : Editor
{
	gameManager manager;

	void OnEnable ()
	{
		manager = (gameManager)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Delete Player Prefs")) {
			manager.deletePlayerPrefs ();
		}

		if (GUILayout.Button ("Get Current Data Path")) {
			manager.getCurrentDataPath ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show Save Files Folder")) {
			manager.openCurrentSaveFolder ();
		}
			
		EditorGUILayout.Space ();
	}
}
#endif