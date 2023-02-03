using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add button in the scanElementInfo script inspector
[CustomEditor (typeof(scanElementInfo))]
public class scanELementInfoEditor : Editor
{
	scanElementInfo manager;

	void OnEnable ()
	{
		manager = (scanElementInfo)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Element")) {
			manager.saveElement ();
		}

		if (GUILayout.Button ("Remove Element")) {
			manager.removeELement ();
		}
	}
}
#endif