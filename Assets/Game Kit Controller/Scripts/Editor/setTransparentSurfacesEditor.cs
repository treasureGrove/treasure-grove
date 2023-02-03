using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(setTransparentSurfaces))]
public class setTransparentSurfacesEditor : Editor
{
	setTransparentSurfaces manager;

	void OnEnable ()
	{
		manager = (setTransparentSurfaces)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Check Surfaces")) {
			if (Application.isPlaying) {
				manager.setCheckSurfaceActiveState (true);
			}
		}
		if (GUILayout.Button ("Disable Check Surfaces")) {
			if (Application.isPlaying) {
				manager.setCheckSurfaceActiveState (false);
			}
		}

		EditorGUILayout.Space ();
	}
}
#endif