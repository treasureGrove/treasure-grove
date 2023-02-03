using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerActionSystem))]
public class playerActionSystemEditor : Editor
{
	playerActionSystem manager;

	SerializedProperty currentArrayElement;

	void OnEnable ()
	{
		manager = (playerActionSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();
	
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

//		if (GUILayout.Button ("Check Actions")) {
//			manager.checkActions ();
//		}

		if (GUILayout.Button ("\n UPDATE ACTION LIST \n\n (PRESS ON ANY NEW SETTING) \n")) {
			manager.updateActionList (false);
		}

		EditorGUILayout.Space ();
	}
}

#endif