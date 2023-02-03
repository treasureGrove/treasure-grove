using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(setGameObjectActiveState))]
public class setGameObjectActiveStateEditor : Editor
{
	setGameObjectActiveState manager;

	void OnEnable ()
	{
		manager = (setGameObjectActiveState)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable/Disable Object")) {
			manager.changeActiveState ();
		}
	}
}
#endif