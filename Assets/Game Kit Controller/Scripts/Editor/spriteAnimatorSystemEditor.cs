using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(spriteAnimatorSystem))]
public class spriteAnimatorSystemEditor : Editor
{
	spriteAnimatorSystem manager;

	void OnEnable ()
	{
		manager = (spriteAnimatorSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Sprite Animator")) {
			manager.setSpriteAnimatorActiveStateFromEditor (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable Sprite Animator")) {
			manager.setSpriteAnimatorActiveStateFromEditor (false);
		}
	}
}
#endif