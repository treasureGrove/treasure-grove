using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryCaptureManagerTransparent))]
public class inventoryCaptureManagerTransparentEditor : Editor
{
	inventoryCaptureManagerTransparent manager;

	void OnEnable ()
	{
		manager = (inventoryCaptureManagerTransparent)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Take Capture")) {
			manager.takeCapture ();
		}

		EditorGUILayout.Space ();
	}
}
#endif