using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerCharactersManager))]
public class playerCharactersManagerEditor : Editor
{
	SerializedProperty searchPlayersAtStart;
	SerializedProperty currentNumberOfPlayers;
	SerializedProperty mainCharacter;
	SerializedProperty newPlayerPositionOffset;
	SerializedProperty regularReferenceResolution;
	SerializedProperty splitReferenceResolution;
	SerializedProperty cameraStateNameToUse;
	SerializedProperty setCurrentCharacterToControlAndAIAtStart;
	SerializedProperty currentCharacterToControlName;
	SerializedProperty delayTimeToChangeBetweenCharacters;
	SerializedProperty playerList;
	SerializedProperty cameraStatesList;
	SerializedProperty extraCharacterList;

	SerializedProperty configureLocalMultiplayerCharactersOnAwake;
	SerializedProperty numberOfPlayersOnAwake;
	SerializedProperty cameraStateNameOnAwake;

	SerializedProperty mainDynamicSplitScreenSystem;

	SerializedProperty extraPlayerPrefab;

	SerializedProperty initialMultiplayerSpawnPosition;

	playerCharactersManager manager;

	Color defBackgroundColor;
	bool showEventSettings;

	string buttonMessage;

	GUIStyle buttonStyle = new GUIStyle ();

	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		searchPlayersAtStart = serializedObject.FindProperty ("searchPlayersAtStart");
		currentNumberOfPlayers = serializedObject.FindProperty ("currentNumberOfPlayers");
		mainCharacter = serializedObject.FindProperty ("mainCharacter");
		newPlayerPositionOffset = serializedObject.FindProperty ("newPlayerPositionOffset");
		regularReferenceResolution = serializedObject.FindProperty ("regularReferenceResolution");
		splitReferenceResolution = serializedObject.FindProperty ("splitReferenceResolution");
		cameraStateNameToUse = serializedObject.FindProperty ("cameraStateNameToUse");
		setCurrentCharacterToControlAndAIAtStart = serializedObject.FindProperty ("setCurrentCharacterToControlAndAIAtStart");
		currentCharacterToControlName = serializedObject.FindProperty ("currentCharacterToControlName");
		delayTimeToChangeBetweenCharacters = serializedObject.FindProperty ("delayTimeToChangeBetweenCharacters");
		playerList = serializedObject.FindProperty ("playerList");
		cameraStatesList = serializedObject.FindProperty ("cameraStatesList");
		extraCharacterList = serializedObject.FindProperty ("extraCharacterList");

		configureLocalMultiplayerCharactersOnAwake = serializedObject.FindProperty ("configureLocalMultiplayerCharactersOnAwake");
		numberOfPlayersOnAwake = serializedObject.FindProperty ("numberOfPlayersOnAwake");
		cameraStateNameOnAwake = serializedObject.FindProperty ("cameraStateNameOnAwake");

		mainDynamicSplitScreenSystem = serializedObject.FindProperty ("mainDynamicSplitScreenSystem");

		extraPlayerPrefab = serializedObject.FindProperty ("extraPlayerPrefab");

		initialMultiplayerSpawnPosition = serializedObject.FindProperty ("initialMultiplayerSpawnPosition");

		manager = (playerCharactersManager)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (searchPlayersAtStart);
		EditorGUILayout.PropertyField (currentNumberOfPlayers);
		EditorGUILayout.PropertyField (mainCharacter);

		EditorGUILayout.PropertyField (newPlayerPositionOffset);
		EditorGUILayout.PropertyField (regularReferenceResolution);
		EditorGUILayout.PropertyField (splitReferenceResolution);

		EditorGUILayout.PropertyField (cameraStateNameToUse);

		if (manager.cameraStatesListString != null && manager.cameraStatesListString.Length > 0) {

			manager.currentCameraStateIndex = EditorGUILayout.Popup ("Camera State To Use", manager.currentCameraStateIndex, manager.cameraStatesListString);
			if (manager.currentCameraStateIndex >= 0) {
				manager.cameraStateNameToUse = manager.cameraStatesListString [manager.currentCameraStateIndex];
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Camera States")) {
			manager.getCameraStateListString ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Configure/Spawn Multiple Players On Awake Settings", "window");
		EditorGUILayout.PropertyField (configureLocalMultiplayerCharactersOnAwake);
		EditorGUILayout.PropertyField (numberOfPlayersOnAwake);
		EditorGUILayout.PropertyField (cameraStateNameOnAwake);
		EditorGUILayout.PropertyField (extraPlayerPrefab);
		EditorGUILayout.PropertyField (initialMultiplayerSpawnPosition);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use AI As Partner To Switch Settings", "window");
		EditorGUILayout.PropertyField (setCurrentCharacterToControlAndAIAtStart);
		EditorGUILayout.PropertyField (currentCharacterToControlName);
		EditorGUILayout.PropertyField (delayTimeToChangeBetweenCharacters);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player List", "window");
		showPlayerList (playerList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera States List", "window");
		showCameraStatesList (cameraStatesList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Extra Character List", "window");
		showExtraCharacterList (extraCharacterList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 25;
		style.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField ("MAIN EDITOR BUTTONS", style);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Players On The Level")) {
			manager.searchPlayersOnTheLevel (false, true);
			manager.setExtraCharacterList ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Configure Players")) {
			manager.setNumberOfPlayers (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Camera Configuration")) {
			manager.setCameraConfiguration (true);
		}
			
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update HUD")) {
			manager.updateHUD (true);
		}

		EditorGUILayout.Space ();


		if (GUILayout.Button ("Set Player ID")) {
			manager.setPlayerID (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Assign Map Systems On Map Creator")) {
			manager.assignMapSystem (true);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera Settings", "window");

		if (GUILayout.Button ("Enable Single Camera State")) {
			manager.enableOrDisableSingleCameraState (true, true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable Single Camera State")) {
			manager.enableOrDisableSingleCameraState (false, true);
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dynamic Split Camera Settings", "window");
		EditorGUILayout.PropertyField (mainDynamicSplitScreenSystem);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Dynamic Split Camera State")) {
			manager.enableOrDisableDynamicSplitCameraState (true, true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable Dynamic Split Camera State")) {
			manager.enableOrDisableDynamicSplitCameraState (false, true);
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showPlayerList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Players: " + list.arraySize);

			EditorGUILayout.Space ();
		
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Player")) {
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
						showPlayerListElements (list.GetArrayElementAtIndex (i));
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
	}

	void showPlayerListElements (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useByAI"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("followMainPlayerOnSwitchCompanion"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useOnlyKeyboard"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerParentGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerControllerGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerCameraGameObject"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerControllerManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerCameraManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerInput"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainPlayerComponentsManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainFriendListManager"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainSaveGameSystem"));
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Toggle Use Only Keyboard/Gamepad")) {
			manager.toogleUseOnlyKeyboardGamepad (list.FindPropertyRelative ("Name").stringValue);
		}

		EditorGUILayout.Space ();

		showEventSettings = list.FindPropertyRelative ("showEventSettings").boolValue;

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showEventSettings) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Event Settings";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Event Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			showEventSettings = !showEventSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showEventSettings").boolValue = showEventSettings;

		if (showEventSettings) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetCharacterAsAI"));

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetCharacterAsPlayer"));

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("DEBUG OPTIONS (IN-GAME)", "window", GUILayout.Height (30));
		if (GUILayout.Button ("Set As Current Player Active")) {
			if (Application.isPlaying) {
				manager.setAsCurrentPlayerActive (list.FindPropertyRelative ("Name").stringValue);
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set as Current Character To Control")) {
			if (Application.isPlaying) {
				manager.setAsCurrentCharacterToControlByName (list.FindPropertyRelative ("Name").stringValue);
			}
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showCameraStatesList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Cameras: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Camera")) {
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
						showCameraStatesListElements (list.GetArrayElementAtIndex (i));
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
	}

	void showCameraStatesListElements (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberfOfPlayers"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Info List", "window");
		showCameraInfoList (list.FindPropertyRelative ("cameraInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showCameraInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Cameras: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Camera")) {
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
						showCameraInfoListElements (list.GetArrayElementAtIndex (i));
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
	}

	void showCameraInfoListElements (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newX"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newY"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newW"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("newH"));
		GUILayout.EndVertical ();
	}

	void showExtraCharacterList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Characters: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Character")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
			
				GUILayout.EndHorizontal ();
			}
		}
	}

}
#endif