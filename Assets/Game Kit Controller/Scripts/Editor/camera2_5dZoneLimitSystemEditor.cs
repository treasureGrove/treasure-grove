using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(camera2_5dZoneLimitSystem))]
[CanEditMultipleObjects]
public class camera2_5dZoneLimitSystemEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		camera2_5dZoneLimitSystem manager = (camera2_5dZoneLimitSystem)target;

		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Current Configuration")) {
			manager.setConfigurationToPlayer ();
		}

		EditorGUILayout.Space ();
	}
}
#endif