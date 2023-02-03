using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inputActionManager))]
public class inputActionManagerEditor : Editor
{
	SerializedProperty showDebugActions;
	SerializedProperty inputActivated;
	SerializedProperty overrideInputValuesActive;
	SerializedProperty inputPaused;
	SerializedProperty inputCurrentlyActive;

	SerializedProperty manualControlActive;
	SerializedProperty manualHorizontalInput;
	SerializedProperty manualVerticalInput;
	SerializedProperty manualMouseHorizontalInput;
	SerializedProperty manualMouseVerticalInput;

	SerializedProperty setAutomaticValuesOnHorizontalInput;

	SerializedProperty setAutomaticValuesOnVerticalInput;

	SerializedProperty setAutomaticValuesOnMouseHorizontalInput;

	SerializedProperty setAutomaticValuesOnVerticalVerticalInput;


	SerializedProperty usingTouchMovementJoystick;
	SerializedProperty enableTouchJoysticks;

	SerializedProperty multiAxesList;

	SerializedProperty useAxisAsHorizontalMovementInput;
	SerializedProperty useAxisAsVerticalMovementInput;

	SerializedProperty horizontalMovementInputLerpSpeed;
	SerializedProperty verticalMovementInputLerpSpeed;

	SerializedProperty movementAxis;
	SerializedProperty mouseAxis;
	SerializedProperty rawMovementAxis;

	SerializedProperty useExternalVehicleTouchControlsEnabled;

	SerializedProperty disableTouchJoysticksOnExternalVehicleEnabled;

	SerializedProperty eventToActivateTouchControlsOnExternalVehicle;
	SerializedProperty eventToDeactivateTouchControlsOnExternalVehicle;

	inputActionManager manager;

	string[] currentStringList;
	bool expanded;
	int currentListIndex;
	GUIStyle style = new GUIStyle ();

	int multiAxesStringIndex;
	int axesStringIndex;

	inputActionManager.multiAxes currentMultiAxes;
	inputActionManager.Axes currentAxes;

	GUIStyle buttonStyle = new GUIStyle ();

	bool showAxisValues;

	void OnEnable ()
	{
		showDebugActions = serializedObject.FindProperty ("showDebugActions");
		inputActivated = serializedObject.FindProperty ("inputActivated");
		overrideInputValuesActive = serializedObject.FindProperty ("overrideInputValuesActive");
		inputPaused = serializedObject.FindProperty ("inputPaused");
		inputCurrentlyActive = serializedObject.FindProperty ("inputCurrentlyActive");

		manualControlActive = serializedObject.FindProperty ("manualControlActive");
		manualHorizontalInput = serializedObject.FindProperty ("manualHorizontalInput");
		manualVerticalInput = serializedObject.FindProperty ("manualVerticalInput");
		manualMouseHorizontalInput = serializedObject.FindProperty ("manualMouseHorizontalInput");
		manualMouseVerticalInput = serializedObject.FindProperty ("manualMouseVerticalInput");

		setAutomaticValuesOnHorizontalInput = serializedObject.FindProperty ("setAutomaticValuesOnHorizontalInput");

		setAutomaticValuesOnVerticalInput = serializedObject.FindProperty ("setAutomaticValuesOnVerticalInput");

		setAutomaticValuesOnMouseHorizontalInput = serializedObject.FindProperty ("setAutomaticValuesOnMouseHorizontalInput");

		setAutomaticValuesOnVerticalVerticalInput = serializedObject.FindProperty ("setAutomaticValuesOnVerticalVerticalInput");


		usingTouchMovementJoystick = serializedObject.FindProperty ("usingTouchMovementJoystick");
		enableTouchJoysticks = serializedObject.FindProperty ("enableTouchJoysticks");

		multiAxesList = serializedObject.FindProperty ("multiAxesList");

		useAxisAsHorizontalMovementInput = serializedObject.FindProperty ("useAxisAsHorizontalMovementInput");
		useAxisAsVerticalMovementInput = serializedObject.FindProperty ("useAxisAsVerticalMovementInput");

		horizontalMovementInputLerpSpeed = serializedObject.FindProperty ("horizontalMovementInputLerpSpeed");
		verticalMovementInputLerpSpeed = serializedObject.FindProperty ("verticalMovementInputLerpSpeed");

		movementAxis = serializedObject.FindProperty ("movementAxis");
		mouseAxis = serializedObject.FindProperty ("mouseAxis");
		rawMovementAxis = serializedObject.FindProperty ("rawMovementAxis");

		useExternalVehicleTouchControlsEnabled = serializedObject.FindProperty ("useExternalVehicleTouchControlsEnabled");

		disableTouchJoysticksOnExternalVehicleEnabled = serializedObject.FindProperty ("disableTouchJoysticksOnExternalVehicleEnabled");

		eventToActivateTouchControlsOnExternalVehicle = serializedObject.FindProperty ("eventToActivateTouchControlsOnExternalVehicle");
		eventToDeactivateTouchControlsOnExternalVehicle = serializedObject.FindProperty ("eventToDeactivateTouchControlsOnExternalVehicle");

		manager = (inputActionManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useAxisAsHorizontalMovementInput);
		EditorGUILayout.PropertyField (useAxisAsVerticalMovementInput);
		EditorGUILayout.PropertyField (horizontalMovementInputLerpSpeed);
		EditorGUILayout.PropertyField (verticalMovementInputLerpSpeed);
		EditorGUILayout.PropertyField (showDebugActions);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input State Debug", "window");
		GUILayout.Label ("Input Activated \t" + inputActivated.boolValue);
		GUILayout.Label ("Override Input Active\t" + overrideInputValuesActive.boolValue);
		GUILayout.Label ("Input Paused\t" + inputPaused.boolValue);
		GUILayout.Label ("Currently Active\t" + inputCurrentlyActive.boolValue);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Axis Values", buttonStyle)) {
			showAxisValues = !showAxisValues;
		}

		EditorGUILayout.Space ();

		if (showAxisValues) {
			GUILayout.Label ("Movement Axis\t" + movementAxis.vector2Value);
			GUILayout.Label ("Mouse Axis\t" + mouseAxis.vector2Value);
			GUILayout.Label ("Raw Movement Axis\t" + rawMovementAxis.vector2Value);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Manual Control Settings", "window");
		EditorGUILayout.PropertyField (manualControlActive);
		if (manualControlActive.boolValue) {
			EditorGUILayout.PropertyField (manualHorizontalInput);
			EditorGUILayout.PropertyField (manualVerticalInput);
			EditorGUILayout.PropertyField (manualMouseHorizontalInput);
			EditorGUILayout.PropertyField (manualMouseVerticalInput);

			EditorGUILayout.PropertyField (setAutomaticValuesOnHorizontalInput);
			EditorGUILayout.PropertyField (setAutomaticValuesOnVerticalInput);
			EditorGUILayout.PropertyField (setAutomaticValuesOnMouseHorizontalInput);
			EditorGUILayout.PropertyField (setAutomaticValuesOnVerticalVerticalInput);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Controls Settings", "window");
		EditorGUILayout.PropertyField (usingTouchMovementJoystick);
		EditorGUILayout.PropertyField (enableTouchJoysticks);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("External Vehicle Touch Controls Settings", "window");
		EditorGUILayout.PropertyField (useExternalVehicleTouchControlsEnabled);
		if (useExternalVehicleTouchControlsEnabled.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (disableTouchJoysticksOnExternalVehicleEnabled);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventToActivateTouchControlsOnExternalVehicle);
			EditorGUILayout.PropertyField (eventToDeactivateTouchControlsOnExternalVehicle);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		style = new GUIStyle (EditorStyles.centeredGreyMiniLabel);
		style.fontSize = 10;
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.black;

		GUILayout.BeginVertical ("Multi Axes List", "window");
		showMultiAxesList (multiAxesList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showMultiAxesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Multi Axes List", buttonStyle)) {
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

	void showMultiAxesListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		currentMultiAxes = manager.multiAxesList [index];

		currentStringList = currentMultiAxes.multiAxesStringList;

		if (currentStringList.Length > 0) {
			multiAxesStringIndex = currentMultiAxes.multiAxesStringIndex;

			multiAxesStringIndex = EditorGUILayout.Popup ("Axes ", multiAxesStringIndex, currentStringList);

			currentMultiAxes.multiAxesStringIndex = multiAxesStringIndex;

			currentListIndex = multiAxesStringIndex;

			if (currentListIndex >= 0) {
				currentMultiAxes.axesName = currentStringList [currentListIndex];
			}
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List", "window");
		showAxesList (list.FindPropertyRelative ("axes"), index);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showAxesList (SerializedProperty list, int multiAxesIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Axes List", buttonStyle)) {
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

			if (list.arraySize == 0) {
				if (GUILayout.Button ("Set All Actions")) {
					manager.setAllAxesList (multiAxesIndex);
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

	void showAxesListElementInfo (SerializedProperty list, int multiAxesIndex, int axesIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));

		currentAxes = manager.multiAxesList [multiAxesIndex].axes [axesIndex];

		currentStringList = currentAxes.axesStringList;

		if (currentStringList.Length > 0) {
			axesStringIndex = currentAxes.axesStringIndex;

			axesStringIndex = EditorGUILayout.Popup ("Axes ", axesStringIndex, currentStringList);

			currentAxes.axesStringIndex = axesStringIndex;

			currentListIndex = axesStringIndex;

			if (currentListIndex >= 0 && axesIndex < manager.multiAxesList [multiAxesIndex].axes.Count && currentListIndex < currentStringList.Length) {
				currentAxes.Name = currentStringList [currentListIndex];
			}
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonPressType"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedOnPausedGame"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showInControlsMenu"));

		GUILayout.BeginHorizontal ("box");
		EditorGUILayout.LabelField ("Key Used in " + list.FindPropertyRelative ("actionName").stringValue + " ---> " +
		list.FindPropertyRelative ("keyButton").stringValue, style);
		GUILayout.EndVertical ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonEvent"));
		GUILayout.EndVertical ();
	}
}
#endif