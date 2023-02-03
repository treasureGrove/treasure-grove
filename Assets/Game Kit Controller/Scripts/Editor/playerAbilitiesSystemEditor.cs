using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerAbilitiesSystem))]
public class playerAbilitiesSystemEditor : Editor
{
	playerAbilitiesSystem manager;

	SerializedProperty currentArrayElement;

	void OnEnable ()
	{
		manager = (playerAbilitiesSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable All Abilities")) {
			if (!Application.isPlaying) {
				manager.enableOrDisableAllAbilitiesFromEditor (true);
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable All Abilities")) {
			if (!Application.isPlaying) {
				manager.enableOrDisableAllAbilitiesFromEditor (false);
			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable All Abilities Can Be Shown On Wheel")) {
			if (!Application.isPlaying) {
				manager.enableOrDisableAllAbilitiesCanBeShownOnWheelFromEditor (true);
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable All Abilities Can Be Shown On Wheel")) {
			if (!Application.isPlaying) {
				manager.enableOrDisableAllAbilitiesCanBeShownOnWheelFromEditor (false);
			}
		}

		EditorGUILayout.Space ();
	}
}

#endif