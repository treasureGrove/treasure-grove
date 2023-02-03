using UnityEngine;
using System.Collections;

namespace GKC.Localization
{
	#if UNITY_EDITOR
	using UnityEditor;

	[CustomEditor (typeof(interactionObjectsLocalizationManager))]
	public class interactionObjectsLocalizationManagerEditor : Editor
	{
		interactionObjectsLocalizationManager manager;

		void OnEnable ()
		{
			manager = (interactionObjectsLocalizationManager)target;
		}

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();

			EditorGUILayout.Space ();

			GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

			EditorGUILayout.Space ();

			Rect lastRect = GUILayoutUtility.GetLastRect();

			EditorGUILayout.BeginVertical ();

			Texture buttonIcon = (Texture)Resources.Load ("Search Icon");

			GUIContent buttonContent = new GUIContent (buttonIcon, "Search Key");

			Rect position = new Rect (lastRect.x + 50, lastRect.y + 10, 30, 30);

			if (GUI.Button (position, buttonContent)) {
				manager.updateFileName ();

				searcherLocalizationToolEditor.Open (manager);
			}

			EditorGUILayout.Space ();

			buttonIcon = (Texture)Resources.Load ("Store Icon");

			buttonContent = new GUIContent (buttonIcon, "Add Key");

			position.x += 50;

			if (GUI.Button (position, buttonContent)) {
				manager.updateFileName ();

				addKeyLocalizationToolEditor.Open (manager);
			}

			EditorGUILayout.Space ();

			buttonIcon = (Texture)Resources.Load ("Language Icon");

			buttonContent = new GUIContent (buttonIcon, "Add Language");

			position.x += 50;

			if (GUI.Button (position, buttonContent)) {
				manager.updateFileName ();

				addLanguageLocalizationToolEditor.Open (manager);
			}

			EditorGUILayout.Space ();

			buttonIcon = (Texture)Resources.Load ("Reload Icon");

			buttonContent = new GUIContent (buttonIcon, "Update Language Name");

			position.x += 50;

			if (GUI.Button (position, buttonContent)) {
				manager.updateFileName ();

				manager.updateComponent ();
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();
		}
	}

	#endif
}