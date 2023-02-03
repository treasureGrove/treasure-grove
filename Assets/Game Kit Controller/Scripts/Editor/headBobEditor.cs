using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(headBob))]
public class headBobEditor : Editor
{
	SerializedProperty headBobEnabled;
	SerializedProperty staticIdleName;
	SerializedProperty currentState;
	SerializedProperty resetSpeed;
	SerializedProperty useDynamicIdle;
	SerializedProperty dynamicIdleName;
	SerializedProperty timeToActiveDynamicIdle;
	SerializedProperty shakeCameraInLockedMode;
	SerializedProperty externalShakeEnabled;
	SerializedProperty externalForceStateName;
	SerializedProperty jumpStartStateName;
	SerializedProperty jumpEndStateName;
	SerializedProperty jumpStartMaxIncrease;
	SerializedProperty jumpStartSpeed;
	SerializedProperty jumpEndMaxDecrease;
	SerializedProperty jumpEndSpeed;
	SerializedProperty jumpResetSpeed;
	SerializedProperty bobStatesList;
	SerializedProperty firstPersonMode;
	SerializedProperty externalShakingActive;
	SerializedProperty headBobCanBeUsed;
	SerializedProperty externalShakeInfoList;
	SerializedProperty playerControllerManager;
	SerializedProperty playerCameraManager;
	SerializedProperty playerBobState;

	SerializedProperty externalShakeManager;

	SerializedProperty mainManagerName;

	headBob headBobManager;

	bool sameValue;
	string shakeName;
	string useShakeInThird;
	bool shakeInThirdEnabled;
	bool shakeInFirstEnabled;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		headBobEnabled = serializedObject.FindProperty ("headBobEnabled");
		staticIdleName = serializedObject.FindProperty ("staticIdleName");
		currentState = serializedObject.FindProperty ("currentState");
		resetSpeed = serializedObject.FindProperty ("resetSpeed");
		useDynamicIdle = serializedObject.FindProperty ("useDynamicIdle");
		dynamicIdleName = serializedObject.FindProperty ("dynamicIdleName");
		timeToActiveDynamicIdle = serializedObject.FindProperty ("timeToActiveDynamicIdle");
		shakeCameraInLockedMode = serializedObject.FindProperty ("shakeCameraInLockedMode");
		externalShakeEnabled = serializedObject.FindProperty ("externalShakeEnabled");
		externalForceStateName = serializedObject.FindProperty ("externalForceStateName");
		jumpStartStateName = serializedObject.FindProperty ("jumpStartStateName");
		jumpEndStateName = serializedObject.FindProperty ("jumpEndStateName");
		jumpStartMaxIncrease = serializedObject.FindProperty ("jumpStartMaxIncrease");
		jumpStartSpeed = serializedObject.FindProperty ("jumpStartSpeed");
		jumpEndMaxDecrease = serializedObject.FindProperty ("jumpEndMaxDecrease");
		jumpEndSpeed = serializedObject.FindProperty ("jumpEndSpeed");
		jumpResetSpeed = serializedObject.FindProperty ("jumpResetSpeed");
		bobStatesList = serializedObject.FindProperty ("bobStatesList");
		firstPersonMode = serializedObject.FindProperty ("firstPersonMode");
		externalShakingActive = serializedObject.FindProperty ("externalShakingActive");
		headBobCanBeUsed = serializedObject.FindProperty ("headBobCanBeUsed");
		externalShakeInfoList = serializedObject.FindProperty ("externalShakeInfoList");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		playerBobState = serializedObject.FindProperty ("playerBobState");

		externalShakeManager = serializedObject.FindProperty ("externalShakeManager");

		mainManagerName = serializedObject.FindProperty ("mainManagerName");

		headBobManager = (headBob)target;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical(GUILayout.Height(30));

		GUILayout.BeginVertical("Settings", "window");
		EditorGUILayout.PropertyField(headBobEnabled);
		EditorGUILayout.PropertyField(staticIdleName);

		EditorGUILayout.PropertyField(currentState);

		EditorGUILayout.PropertyField(resetSpeed);

		EditorGUILayout.PropertyField(shakeCameraInLockedMode);

		EditorGUILayout.PropertyField(mainManagerName);
		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.BeginVertical("Dynamic Idle Settings", "window");
		EditorGUILayout.PropertyField(useDynamicIdle);
		if (useDynamicIdle.boolValue) {
			EditorGUILayout.PropertyField(dynamicIdleName);
			EditorGUILayout.PropertyField(timeToActiveDynamicIdle);
		}

		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.BeginVertical("External Shake Settings", "window");
		EditorGUILayout.PropertyField(externalShakeEnabled);
		if (externalShakeEnabled.boolValue) {
			EditorGUILayout.PropertyField(externalForceStateName);
		}

		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.BeginVertical("Jump Settings", "window");
		EditorGUILayout.PropertyField(jumpStartStateName);
		EditorGUILayout.PropertyField(jumpEndStateName);

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(jumpStartMaxIncrease);
		EditorGUILayout.PropertyField(jumpStartSpeed);
		EditorGUILayout.PropertyField(jumpEndMaxDecrease);
		EditorGUILayout.PropertyField(jumpEndSpeed);
		EditorGUILayout.PropertyField(jumpResetSpeed);

		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.BeginVertical("Bob States List", "window");
		showBobList(bobStatesList);
		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.BeginVertical("Player Bob State", "window");
		GUILayout.BeginHorizontal();
		GUILayout.Label("First Person View");
		GUILayout.Label("" + firstPersonMode.boolValue);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("External Shake Active");
		GUILayout.Label("" + externalShakingActive.boolValue);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Head Bob can be used");
		GUILayout.Label("" + headBobCanBeUsed.boolValue);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		if (externalShakeEnabled.boolValue) {
			EditorGUILayout.Space();

			GUILayout.BeginVertical("External Shakes States List", "window");
			showExternalShakeInfoList(externalShakeInfoList);
			GUILayout.EndVertical();
		}

		EditorGUILayout.Space();

		GUILayout.BeginVertical("Player Elements Settings", "window");
		EditorGUILayout.PropertyField(playerControllerManager);
		EditorGUILayout.PropertyField(playerCameraManager);
		EditorGUILayout.PropertyField(externalShakeManager);
		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.EndVertical();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties();
		}
	}

	void showElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobTransformStyle"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enableBobIn"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("posAmount"), new GUIContent ("Position Amount"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("posSpeed"), new GUIContent ("Position Speed"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("posSmooth"), new GUIContent ("Position Smooth"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eulAmount"), new GUIContent ("Rotation Amount"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eulSpeed"), new GUIContent ("Rotation Speed"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eulSmooth"), new GUIContent ("Rotation Smooth"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		GUILayout.EndVertical ();
	}

	void showBobList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Bob States List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Bob States: \t" + list.arraySize);
			
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
						expanded = true;
						showElementInfo (list.GetArrayElementAtIndex (i));
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

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Current State Info", "window");
			EditorGUILayout.PropertyField (playerBobState);
			if (playerBobState.isExpanded) {
				showElementInfo (playerBobState);
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showExternalShakeInfoList(SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide External Shake Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space();

			GUILayout.Label("Number Of External Shakes: \t" + list.arraySize);

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Shake")) {
				list.arraySize++;
			}

			if (GUILayout.Button("Clear")) {
				list.arraySize = 0;
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex(i).isExpanded = true;
				}
			}

			if (GUILayout.Button("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex(i).isExpanded = false;
				}
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal("box");
				EditorGUILayout.Space();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), false);
					if (list.GetArrayElementAtIndex(i).isExpanded) {
						expanded = true;
						showExternalShakeElementInfo(list.GetArrayElementAtIndex(i), i);
					}

					EditorGUILayout.Space();

					GUILayout.EndVertical();
				}

				GUILayout.EndHorizontal();
				if (expanded) {
					GUILayout.BeginVertical();
				} else {
					GUILayout.BeginHorizontal();
				}

				if (GUILayout.Button("v")) {
					if (i >= 0) {
						list.MoveArrayElement(i, i + 1);
					}
				}

				if (GUILayout.Button("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement(i, i - 1);
					}
				}

				if (expanded) {
					GUILayout.EndVertical();
				} else {
					GUILayout.EndHorizontal();
				}

				GUILayout.EndHorizontal();
			}
		}
	}

	void showExternalShakeElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sameValueBothViews"));
		sameValue = list.FindPropertyRelative ("sameValueBothViews").boolValue;
		shakeName = "Third Person Damage Shake";
		useShakeInThird = "Shake In Third Person Enabled";
		if (sameValue) {
			shakeName = "Damage Shake";
			useShakeInThird = "Shake Enabled";
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageShakeInThirdPerson"), new GUIContent (useShakeInThird), false);
		if (!sameValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageShakeInFirstPerson"), new GUIContent ("Shake In First Person Enabled"), false);
		}

		shakeInThirdEnabled = list.FindPropertyRelative ("useDamageShakeInThirdPerson").boolValue;
		shakeInFirstEnabled = list.FindPropertyRelative ("useDamageShakeInFirstPerson").boolValue;
		if (shakeInThirdEnabled) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("thirdPersonDamageShake"), new GUIContent (shakeName), false);
			if (list.FindPropertyRelative ("thirdPersonDamageShake").isExpanded) {
				GUILayout.BeginVertical (shakeName, "window");
				showExternalShakeElementInfoContent (list.FindPropertyRelative ("thirdPersonDamageShake"));
				GUILayout.EndVertical ();
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Test Shake")) {
					if (Application.isPlaying) {
						headBobManager.setExternalShakeStateByIndex (index, false);
					}
				}

				EditorGUILayout.Space ();

				if (Application.isPlaying) {
					if (GUILayout.Button ("Set Shake In Manager List")) {
						headBobManager.setShakeInManagerList (index);
					}

					EditorGUILayout.Space ();
				}
			}
		}
		if (shakeInFirstEnabled) {
			if (!list.FindPropertyRelative ("sameValueBothViews").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonDamageShake"));
				if (list.FindPropertyRelative ("firstPersonDamageShake").isExpanded) {
					GUILayout.BeginVertical ("First Person Damage Shake", "window");
					showExternalShakeElementInfoContent (list.FindPropertyRelative ("firstPersonDamageShake"));
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Test Shake")) {
						if (Application.isPlaying) {
							headBobManager.setExternalShakeStateByIndex (index, true);
						}
					}
				}
			}
		}
		GUILayout.EndVertical ();
	}

	void showExternalShakeElementInfoContent (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePositionSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePositionSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("decreaseShakeInTime"));
		if (list.FindPropertyRelative ("decreaseShakeInTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("decreaseShakeSpeed"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayBeforeStartDecrease"));
		if (list.FindPropertyRelative ("useDelayBeforeStartDecrease").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayBeforeStartDecrease"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("repeatShake"));
		if (list.FindPropertyRelative ("repeatShake").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberOfRepeats"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayBetweenRepeats"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("externalShakeDelay"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUnscaledTime"));
	}
}
#endif