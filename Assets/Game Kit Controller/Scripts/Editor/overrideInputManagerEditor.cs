using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(overrideInputManager))]
public class overrideInputManagerEditor : Editor
{
	SerializedProperty inputEnabled;
	SerializedProperty isPlayerController;
	SerializedProperty playerInput;
	SerializedProperty showDebugActions;
	SerializedProperty destroyObjectOnStopOverride;
	SerializedProperty eventToDestroy;
	SerializedProperty startOverrideFunction;
	SerializedProperty stopOverrideFunction;
	SerializedProperty usePreOverrideFunctions;
	SerializedProperty preStartOverrideFunction;
	SerializedProperty preStopOverrideFunction;
	SerializedProperty activateActionScreen;
	SerializedProperty actionScreenName;
	SerializedProperty multiAxesList;
	SerializedProperty overrideCameraControllerManager;

	overrideInputManager manager;

	bool inputListOpened;
	Color defBackgroundColor;
	string[] currentStringList;
	int currentListIndex;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		inputEnabled = serializedObject.FindProperty ("inputEnabled");
		isPlayerController = serializedObject.FindProperty ("isPlayerController");
		playerInput = serializedObject.FindProperty ("playerInput");
		showDebugActions = serializedObject.FindProperty ("showDebugActions");
		destroyObjectOnStopOverride = serializedObject.FindProperty ("destroyObjectOnStopOverride");
		eventToDestroy = serializedObject.FindProperty ("eventToDestroy");
		startOverrideFunction = serializedObject.FindProperty ("startOverrideFunction");
		stopOverrideFunction = serializedObject.FindProperty ("stopOverrideFunction");
		usePreOverrideFunctions = serializedObject.FindProperty ("usePreOverrideFunctions");
		preStartOverrideFunction = serializedObject.FindProperty ("preStartOverrideFunction");
		preStopOverrideFunction = serializedObject.FindProperty ("preStopOverrideFunction");
		activateActionScreen = serializedObject.FindProperty ("activateActionScreen");
		actionScreenName = serializedObject.FindProperty ("actionScreenName");
		multiAxesList = serializedObject.FindProperty ("multiAxesList");
		overrideCameraControllerManager = serializedObject.FindProperty ("overrideCameraControllerManager");

		manager = (overrideInputManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical (GUILayout.Height (30));

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (inputEnabled);
		EditorGUILayout.PropertyField (isPlayerController);
		EditorGUILayout.PropertyField (playerInput);
		EditorGUILayout.PropertyField (showDebugActions);
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (destroyObjectOnStopOverride);
		if (destroyObjectOnStopOverride.boolValue) {
			EditorGUILayout.PropertyField (eventToDestroy);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Event Settings", "window");
		EditorGUILayout.PropertyField (startOverrideFunction);
		EditorGUILayout.PropertyField (stopOverrideFunction);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pre Event Settings", "window");
		EditorGUILayout.PropertyField (usePreOverrideFunctions);
		EditorGUILayout.PropertyField (preStartOverrideFunction);
		EditorGUILayout.PropertyField (preStopOverrideFunction);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Screen Settings", "window");
		EditorGUILayout.PropertyField (activateActionScreen);
		if (activateActionScreen.boolValue) {
			EditorGUILayout.PropertyField (actionScreenName);	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Multi Axes List", "window");
		showMultiAxesList (multiAxesList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Override Camera Controller", "window");
		EditorGUILayout.PropertyField (overrideCameraControllerManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showMultiAxesListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		currentStringList = manager.multiAxesList [index].multiAxesStringList;
		if (currentStringList.Length > 0) {
			manager.multiAxesList [index].multiAxesStringIndex = EditorGUILayout.Popup ("Axe ", manager.multiAxesList [index].multiAxesStringIndex, currentStringList);
			currentListIndex = manager.multiAxesList [index].multiAxesStringIndex;
			if (currentListIndex >= 0) {
				manager.multiAxesList [index].axesName = currentStringList [currentListIndex];
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List", "window");
		showAxesList (list.FindPropertyRelative ("axes"), index);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showMultiAxesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.Label ("Number Of Axes: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Axes")) {
				manager.addNewAxes ();
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Axes")) {
				manager.setMultiAxesEnabledState (true);
			}
			if (GUILayout.Button ("Disable All Axes")) {
				manager.setMultiAxesEnabledState (false);
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Input List")) {
				manager.updateMultiAxesList ();
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
						showMultiAxesListElementInfo (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
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
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
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
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showAxesListElementInfo (SerializedProperty list, int multiAxesIndex, int axesIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));
		currentStringList = manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringList;
		if (currentStringList.Length > 0) {
			manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringIndex = EditorGUILayout.Popup ("Axe ", 
				manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringIndex, currentStringList);
			currentListIndex = manager.multiAxesList [multiAxesIndex].axes [axesIndex].axesStringIndex;
			if (currentListIndex >= 0 && axesIndex < manager.multiAxesList [multiAxesIndex].axes.Count && currentListIndex < currentStringList.Length) {
				manager.multiAxesList [multiAxesIndex].axes [axesIndex].Name = currentStringList [currentListIndex];
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonPressType"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedOnPausedGame"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonEvent"));
		GUILayout.EndVertical ();
	}

	void showAxesList (SerializedProperty list, int multiAxesIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.Label ("Number Of Actions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
				manager.addNewAction (multiAxesIndex);
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Actions")) {
				manager.setAllActionsEnabledState (multiAxesIndex, true);
			}
			if (GUILayout.Button ("Disable All Actions")) {
				manager.setAllActionsEnabledState (multiAxesIndex, false);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Input List")) {
				manager.updateAxesList (multiAxesIndex);
			}

			if (GUILayout.Button ("Set All Actions")) {
				manager.setAllAxesList (multiAxesIndex);
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
						showAxesListElementInfo (list.GetArrayElementAtIndex (i), multiAxesIndex, i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
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
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
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
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif