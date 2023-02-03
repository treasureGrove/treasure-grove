using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerCullingSystem))]
public class playerCullingSystemEditor : Editor
{
	playerCullingSystem manager;

	void OnEnable ()
	{
		manager = (playerCullingSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Store Player Renderer")) {
			if (!Application.isPlaying) {
				manager.storePlayerRendererFromEditor ();
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Clear Player Renderer")) {
			if (!Application.isPlaying) {
				manager.clearRendererListFromEditor ();
			}
		}

		EditorGUILayout.Space ();
	}
}
#endif