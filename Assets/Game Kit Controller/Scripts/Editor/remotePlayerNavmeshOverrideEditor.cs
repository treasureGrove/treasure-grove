using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(remotePlayerNavmeshOverride))]
public class remotePlayerNavmeshOverrideEditor : Editor
{
	remotePlayerNavmeshOverride manager;

	void OnEnable ()
	{
		manager = (remotePlayerNavmeshOverride)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Player Navmesh")) {
			manager.enablePlayerNavMeshState ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable Player Navmesh")) {
			manager.disablePlayerNavMeshState ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Player Navmesh Position")) {
			manager.setPlayerNavMeshTargetPosition ();
		}

		EditorGUILayout.Space ();
	}
}
#endif