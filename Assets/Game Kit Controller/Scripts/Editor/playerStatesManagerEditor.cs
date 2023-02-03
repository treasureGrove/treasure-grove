using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerStatesManager))]
public class playerStatesManagerEditor : Editor
{
	SerializedProperty openPlayerModeMenuEnabled;
	SerializedProperty changeModeByInputEnabled;

	SerializedProperty switchBetweenPlayerControlsEnabled;

	SerializedProperty changePlayerControlEnabled;

	SerializedProperty useDefaultPlayersMode;
	SerializedProperty defaultPlayersModeName;

	SerializedProperty changeModeEnabled;
	SerializedProperty closeMenuWhenModeSelected;
	SerializedProperty canSetRegularModeActive;
	SerializedProperty useBlurUIPanel;
	SerializedProperty defaultControlStateName;
	SerializedProperty menuOpened;
	SerializedProperty currentControlStateName;
	SerializedProperty playersMode;

	SerializedProperty currentPlayersModeName;

	SerializedProperty playerControlList;
	SerializedProperty audioSourceList;
	SerializedProperty playerStateInfoList;
	SerializedProperty eventToEnableComponents;
	SerializedProperty eventToDisableComponents;
	SerializedProperty useEventIfSystemDisabled;
	SerializedProperty eventIfSystemDisabled;

	SerializedProperty showEventsSettings;

	SerializedProperty showElementSettings;

	SerializedProperty currentPlayerModeText;

	SerializedProperty pauseManager;
	SerializedProperty playerControlModeMenu;
	SerializedProperty currentPlayerControlModeImage;
	SerializedProperty powersManager;
	SerializedProperty grabManager;
	SerializedProperty scannerManager;
	SerializedProperty playerManager;
	SerializedProperty playerCameraManager;
	SerializedProperty gravityManager;
	SerializedProperty weaponsManager;
	SerializedProperty combatManager;
	SerializedProperty IKSystemManager;
	SerializedProperty usingDevicesManager;
	SerializedProperty overrideElementManager;
	SerializedProperty headBobManager;
	SerializedProperty damageInScreenManager;


	SerializedProperty mainUpperBodyRotationSystem;

	SerializedProperty mainFindObjectivesSystem;

	SerializedProperty mainAINavMesh;

	SerializedProperty mainOxygenSystem;
	SerializedProperty mainPlayerAbilitiesSystem;

	Color buttonColor;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		openPlayerModeMenuEnabled = serializedObject.FindProperty ("openPlayerModeMenuEnabled");

		switchBetweenPlayerControlsEnabled = serializedObject.FindProperty ("switchBetweenPlayerControlsEnabled");

		changeModeByInputEnabled = serializedObject.FindProperty ("changeModeByInputEnabled");

		changePlayerControlEnabled = serializedObject.FindProperty ("changePlayerControlEnabled");

		useDefaultPlayersMode = serializedObject.FindProperty ("useDefaultPlayersMode");
		defaultPlayersModeName = serializedObject.FindProperty ("defaultPlayersModeName");

		changeModeEnabled = serializedObject.FindProperty ("changeModeEnabled");
		closeMenuWhenModeSelected = serializedObject.FindProperty ("closeMenuWhenModeSelected");
		canSetRegularModeActive = serializedObject.FindProperty ("canSetRegularModeActive");
		useBlurUIPanel = serializedObject.FindProperty ("useBlurUIPanel");
		defaultControlStateName = serializedObject.FindProperty ("defaultControlStateName");
		menuOpened = serializedObject.FindProperty ("menuOpened");
		currentControlStateName = serializedObject.FindProperty ("currentControlStateName");
		playersMode = serializedObject.FindProperty ("playersMode");

		currentPlayersModeName = serializedObject.FindProperty ("currentPlayersModeName");

		playerControlList = serializedObject.FindProperty ("playerControlList");
		audioSourceList = serializedObject.FindProperty ("audioSourceList");
		playerStateInfoList = serializedObject.FindProperty ("playerStateInfoList");
		eventToEnableComponents = serializedObject.FindProperty ("eventToEnableComponents");
		eventToDisableComponents = serializedObject.FindProperty ("eventToDisableComponents");
		useEventIfSystemDisabled = serializedObject.FindProperty ("useEventIfSystemDisabled");
		eventIfSystemDisabled = serializedObject.FindProperty ("eventIfSystemDisabled");

		showElementSettings = serializedObject.FindProperty ("showElementSettings");
		showEventsSettings= serializedObject.FindProperty ("showEventsSettings");

		currentPlayerModeText = serializedObject.FindProperty ("currentPlayerModeText");

		pauseManager = serializedObject.FindProperty ("pauseManager");
		playerControlModeMenu = serializedObject.FindProperty ("playerControlModeMenu");
		currentPlayerControlModeImage = serializedObject.FindProperty ("currentPlayerControlModeImage");
		powersManager = serializedObject.FindProperty ("powersManager");
		grabManager = serializedObject.FindProperty ("grabManager");
		scannerManager = serializedObject.FindProperty ("scannerManager");
		playerManager = serializedObject.FindProperty ("playerManager");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		gravityManager = serializedObject.FindProperty ("gravityManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		combatManager = serializedObject.FindProperty ("combatManager");
		IKSystemManager = serializedObject.FindProperty ("IKSystemManager");
		usingDevicesManager = serializedObject.FindProperty ("usingDevicesManager");
		overrideElementManager = serializedObject.FindProperty ("overrideElementManager");
		headBobManager = serializedObject.FindProperty ("headBobManager");
		damageInScreenManager = serializedObject.FindProperty ("damageInScreenManager");


		mainUpperBodyRotationSystem = serializedObject.FindProperty ("mainUpperBodyRotationSystem");

		mainFindObjectivesSystem = serializedObject.FindProperty ("mainFindObjectivesSystem");

		mainAINavMesh = serializedObject.FindProperty ("mainAINavMesh");

		mainOxygenSystem = serializedObject.FindProperty ("mainOxygenSystem");
		mainPlayerAbilitiesSystem = serializedObject.FindProperty ("mainPlayerAbilitiesSystem");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Player Modes Settings", "window");
		EditorGUILayout.PropertyField (changeModeByInputEnabled);
		EditorGUILayout.PropertyField (changeModeEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Control Settings", "window");
		EditorGUILayout.PropertyField (openPlayerModeMenuEnabled);
		EditorGUILayout.PropertyField (changePlayerControlEnabled);
		EditorGUILayout.PropertyField (canSetRegularModeActive);
		EditorGUILayout.PropertyField (closeMenuWhenModeSelected);
		EditorGUILayout.PropertyField (useBlurUIPanel);
		EditorGUILayout.PropertyField (switchBetweenPlayerControlsEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player's Mode and Control Settings", "window");
		EditorGUILayout.PropertyField (useDefaultPlayersMode);
		if (useDefaultPlayersMode.boolValue) {
			EditorGUILayout.PropertyField (defaultPlayersModeName);
		}
		EditorGUILayout.PropertyField (defaultControlStateName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("States", "window");
		GUILayout.Label ("Menu Opened\t\t" + menuOpened.boolValue);
		GUILayout.Label ("Current Player Mode Name\t" + currentPlayersModeName.stringValue);
		GUILayout.Label ("Current Control State Name\t" + currentControlStateName.stringValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Players Mode List", "window");
		showPlayersMode (playersMode);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Control List", "window");
		showPlayerControlList (playerControlList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Audio Source List", "window");
		showAudioSourceList (audioSourceList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player States List", "window");
		showPlayerStateInfoList (playerStateInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		if (showEventsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Show Events Settings")) {
			showEventsSettings.boolValue = !showEventsSettings.boolValue;
		}
		GUI.backgroundColor = buttonColor;

		if (showEventsSettings.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event Settings", "window");
			EditorGUILayout.PropertyField (eventToEnableComponents);
			EditorGUILayout.PropertyField (eventToDisableComponents);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventIfSystemDisabled);
			if (useEventIfSystemDisabled.boolValue) {
				EditorGUILayout.PropertyField (eventIfSystemDisabled);
			}
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		if (showElementSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Show Player Components")) {
			showElementSettings.boolValue = !showElementSettings.boolValue;
		}
		GUI.backgroundColor = buttonColor;

		if (showElementSettings.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Components", "window");
			EditorGUILayout.PropertyField (currentPlayerModeText);
			EditorGUILayout.PropertyField (pauseManager);
			EditorGUILayout.PropertyField (playerControlModeMenu);
			EditorGUILayout.PropertyField (currentPlayerControlModeImage);
			EditorGUILayout.PropertyField (powersManager);
			EditorGUILayout.PropertyField (grabManager);
			EditorGUILayout.PropertyField (scannerManager);
			EditorGUILayout.PropertyField (playerManager);
			EditorGUILayout.PropertyField (playerCameraManager);
			EditorGUILayout.PropertyField (gravityManager);
			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (combatManager);
			EditorGUILayout.PropertyField (IKSystemManager);
			EditorGUILayout.PropertyField (usingDevicesManager);
			EditorGUILayout.PropertyField (overrideElementManager);
			EditorGUILayout.PropertyField (headBobManager);
			EditorGUILayout.PropertyField (damageInScreenManager);

			EditorGUILayout.PropertyField (mainUpperBodyRotationSystem);
			EditorGUILayout.PropertyField (mainFindObjectivesSystem);
			EditorGUILayout.PropertyField (mainAINavMesh);
		
			EditorGUILayout.PropertyField (mainOxygenSystem);
			EditorGUILayout.PropertyField (mainPlayerAbilitiesSystem);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showAudioSourceList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Source: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Source")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
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
						showAudioSourceListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
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

	void showAudioSourceListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSourceName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSource"));
		GUILayout.EndVertical ();
	}

	void showPlayersMode (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Modes: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Mode")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
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
						showPlayersModeElement (list.GetArrayElementAtIndex (i));
						expanded = true;
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

	void showPlayersModeElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("nameMode"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("modeEnabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activatePlayerModeEvent"));
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivatePlayerModeEvent"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showPlayerControlList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Control: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Control")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
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
						showPlayerControlListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
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

	void showPlayerControlListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("modeEnabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Icon Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("modeTexture"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("avoidToSetRegularModeWhenActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateControlModeEvent"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateControlModeEvent"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showPlayerStateInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
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
						showPlayerStateInfoListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
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

	void showPlayerStateInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateEnabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnState"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}
}
#endif