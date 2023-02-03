using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerTutorialSystem))]
public class playerTutorialSystemEditor : Editor
{
	SerializedProperty tutorialsEnabled;
	SerializedProperty instantiateTutorialPanelIfNotFound;
	SerializedProperty minTimeToPressKey;
	SerializedProperty tutorialOpened;
	SerializedProperty mainTutorialUISystemFound;
	SerializedProperty tutorialInfoList;
	SerializedProperty tutorialPanelPrefab;
	SerializedProperty playerControllerManager;
	SerializedProperty pauseManager;
	SerializedProperty mainAudioSource;
	SerializedProperty mainTutorialUISystem;

	playerTutorialSystem manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		tutorialsEnabled = serializedObject.FindProperty  ("tutorialsEnabled");
		instantiateTutorialPanelIfNotFound = serializedObject.FindProperty ("instantiateTutorialPanelIfNotFound");
		minTimeToPressKey = serializedObject.FindProperty ("minTimeToPressKey");
		tutorialOpened = serializedObject.FindProperty ("tutorialOpened");
		mainTutorialUISystemFound = serializedObject.FindProperty ("mainTutorialUISystemFound");
		tutorialInfoList = serializedObject.FindProperty ("tutorialInfoList");
		tutorialPanelPrefab = serializedObject.FindProperty ("tutorialPanelPrefab");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		pauseManager = serializedObject.FindProperty ("pauseManager");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		mainTutorialUISystem = serializedObject.FindProperty ("mainTutorialUISystem");

		manager = (playerTutorialSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (tutorialsEnabled);
		EditorGUILayout.PropertyField (instantiateTutorialPanelIfNotFound);
		EditorGUILayout.PropertyField (minTimeToPressKey);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tutorial State (Debug)", "window");
		EditorGUILayout.PropertyField (tutorialOpened);
		EditorGUILayout.PropertyField (mainTutorialUISystemFound);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Tutorial Info List", "window");
		showTutorialInfoList (tutorialInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Tutorial Panel To Scene")) {
			manager.instantiateTutorialPanelPrefab ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (tutorialPanelPrefab);
		EditorGUILayout.PropertyField (playerControllerManager);
		EditorGUILayout.PropertyField (pauseManager);
		EditorGUILayout.PropertyField (mainAudioSource);
	
		EditorGUILayout.PropertyField (mainTutorialUISystem);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showTutorialInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("panelGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("unlockCursorOnTutorialActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useActionButtonToMoveThroughTutorial"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pressAnyButtonToNextTutorial"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("timeToEnableKeys"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("openTutorialDelay"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setCustomTimeScale"));
		if (list.FindPropertyRelative ("setCustomTimeScale").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customTimeScale"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundOnTutorialOpen"));
		if (list.FindPropertyRelative ("useSoundOnTutorialOpen").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("soundOnTutorialOpen"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playTutorialOnlyOnce"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tutorial Video Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsVideo"));

		if (list.FindPropertyRelative ("containsVideo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNextPanelWhenVideoEnds"));

			EditorGUILayout.Space ();

			showTutorialVideoList (list.FindPropertyRelative ("videoInfoList"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tutorial Panel List", "window");
		showTutorialPanelList (list.FindPropertyRelative ("tutorialPanelList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Activate Tutorial (Debug)")) {
			if (Application.isPlaying) {
				manager.activatingTutorialByNameFromEditor ();
				manager.activateTutorialByName (list.FindPropertyRelative ("Name").stringValue);
			}
		}
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showTutorialInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Tutorial Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();
		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Tutorials: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Tutorial")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showTutorialInfoListElement (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showTutorialPanelList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Tutorial Panel List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Panels: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Panel")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showTutorialPanelListElement (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showTutorialPanelListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("panelGameObject"));
		GUILayout.EndVertical ();
	}

	void showTutorialVideoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Tutorial Video List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Videos: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Video")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showTutorialVideoListElement (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showTutorialVideoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("videoRawImage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("videoFile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useVideoAudio"));
		if (list.FindPropertyRelative ("useVideoAudio").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("videoAudioVolume"));	
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("loopVideo"));
		GUILayout.EndVertical ();
	}
}
#endif