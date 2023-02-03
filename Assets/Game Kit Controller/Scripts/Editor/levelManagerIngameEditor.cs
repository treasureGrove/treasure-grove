using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(levelManagerIngame))]
public class levelManagerIngameEditor : Editor
{
	levelManagerIngame manager;

	void OnEnable ()
	{
		manager = (levelManagerIngame)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Open Next Scene")) {
			manager.openNextSceneOnEditor ();
		}

		EditorGUILayout.Space ();
	}
}
#endif