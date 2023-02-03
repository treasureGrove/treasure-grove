using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(customCharacterControllerManager))]
public class customCharacterControllerManagerEditor : Editor
{
	customCharacterControllerManager manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	void OnEnable ()
	{
		manager = (customCharacterControllerManager)target;
	}

	GUIStyle style = new GUIStyle ();

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField ("EDITOR BUTTONS", style);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Current Generic Model")) {
			manager.toggleCharacterModelMeshOnEditor (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Humanoid Model")) {
			manager.toggleCharacterModelMeshOnEditor (false);
		}

		EditorGUILayout.Space ();
	
		if (GUILayout.Button ("Set Capsule Values From Main Character")) {
			manager.setCapsuleValuesFromMainCharacter ();
		}

		EditorGUILayout.Space ();
	}
}
#endif