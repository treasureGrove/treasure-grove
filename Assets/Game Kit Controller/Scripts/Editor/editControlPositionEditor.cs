using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(editControlPosition))]
public class editControlPositionEditor : Editor
{
	editControlPosition manager;

	void OnEnable ()
	{
		manager = (editControlPosition)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Touch Buttons Components")) {
			manager.getTouchButtonsComponents ();
		}

		EditorGUILayout.Space ();
	}
}
#endif