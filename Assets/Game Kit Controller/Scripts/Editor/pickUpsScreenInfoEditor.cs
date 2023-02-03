using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(pickUpsScreenInfo))]
public class pickUpsScreenInfoEditor : Editor
{
	SerializedProperty pickUpScreenInfoEnabled;
	SerializedProperty durationTimerPerText;
	SerializedProperty verticalDistance;
	SerializedProperty horizontalOffset;

	SerializedProperty horizontalIconOffset;

	SerializedProperty useIconsEnabled;
	SerializedProperty iconHeight;
	SerializedProperty verticalIconOffset;

	SerializedProperty adjustTextSizeDelta;
	SerializedProperty textSizeDeltaOffsetMultiplier;

	SerializedProperty usedByAI;
	SerializedProperty textToAddFromEditor;
	SerializedProperty originalIcon;
	SerializedProperty originalText;
	SerializedProperty originalTextRectTransform;
	SerializedProperty mainPlayerController;

	SerializedProperty pickupsInfoParent;

	pickUpsScreenInfo manager;

	void OnEnable ()
	{
		pickUpScreenInfoEnabled = serializedObject.FindProperty ("pickUpScreenInfoEnabled");
		durationTimerPerText = serializedObject.FindProperty ("durationTimerPerText");
		verticalDistance = serializedObject.FindProperty ("verticalDistance");
		horizontalOffset = serializedObject.FindProperty ("horizontalOffset");

		adjustTextSizeDelta = serializedObject.FindProperty ("adjustTextSizeDelta");
		textSizeDeltaOffsetMultiplier = serializedObject.FindProperty ("textSizeDeltaOffsetMultiplier");

		useIconsEnabled = serializedObject.FindProperty ("useIconsEnabled");
		iconHeight = serializedObject.FindProperty ("iconHeight");
		verticalIconOffset = serializedObject.FindProperty ("verticalIconOffset");
		horizontalIconOffset = serializedObject.FindProperty ("horizontalIconOffset");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		textToAddFromEditor = serializedObject.FindProperty ("textToAddFromEditor");
		originalIcon = serializedObject.FindProperty ("originalIcon");
		originalText = serializedObject.FindProperty ("originalText");
		originalTextRectTransform = serializedObject.FindProperty ("originalTextRectTransform");
		mainPlayerController = serializedObject.FindProperty ("mainPlayerController");

		pickupsInfoParent = serializedObject.FindProperty ("pickupsInfoParent");

		manager = (pickUpsScreenInfo)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical (GUILayout.Height (30));

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (pickUpScreenInfoEnabled);
		EditorGUILayout.PropertyField (durationTimerPerText);
		EditorGUILayout.PropertyField (verticalDistance);
		EditorGUILayout.PropertyField (horizontalOffset);
		EditorGUILayout.PropertyField (adjustTextSizeDelta);
		EditorGUILayout.PropertyField (textSizeDeltaOffsetMultiplier);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Icon Settings", "window");
		EditorGUILayout.PropertyField (useIconsEnabled);
		EditorGUILayout.PropertyField (iconHeight);
		EditorGUILayout.PropertyField (verticalIconOffset);
		EditorGUILayout.PropertyField (horizontalIconOffset);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Others Settings", "window");
		EditorGUILayout.PropertyField (usedByAI);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (textToAddFromEditor);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Text")) {
			if (Application.isPlaying) {
				manager.addTextFromEditor ();
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Text With Icon")) {
			if (Application.isPlaying) {
				manager.addTextAndIconFromEditor ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (pickupsInfoParent);
		EditorGUILayout.PropertyField (originalIcon);
		EditorGUILayout.PropertyField (originalText);
		EditorGUILayout.PropertyField (originalTextRectTransform);
		EditorGUILayout.PropertyField (mainPlayerController);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif