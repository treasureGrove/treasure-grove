using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(characterCustomizationManager))]
public class characterCustomizationManagerEditor : Editor
{
	characterCustomizationManager manager;

	void OnEnable ()
	{
		manager = (characterCustomizationManager)target;
	}

	GUIStyle style = new GUIStyle ();


	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField ("EDITOR BUTTONS", style);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Create Piece Meshes Objects From Set")) {
			manager.createPieceMeshesObjectsFromSetByName ();
		}

		EditorGUILayout.Space ();
	}
}
#endif