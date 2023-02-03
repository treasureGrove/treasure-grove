using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(UMAIgnoreTransformAndChilds))]
public class UMAIgnoreTransformAndChildsEditor : Editor
{
	UMAIgnoreTransformAndChilds manager;

	void OnEnable ()
	{
		manager = (UMAIgnoreTransformAndChilds)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Transforms To Ignore")) {
			manager.setChildrenList ();
		}
		if (GUILayout.Button ("Remove Transforms To Ignore")) {
			manager.removeChildList ();
		}

		EditorGUILayout.Space ();
	}
}
#endif