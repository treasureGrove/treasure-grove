using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(externalShakeListManager))]
public class externalShakeListManagerEditor : Editor
{
	SerializedProperty externalShakeInfoList;

	externalShakeListManager manager;

	GUIStyle buttonStyle = new GUIStyle ();

	bool sameValue;
	string shakeName;
	string useShakeInThird;
	bool shakeInThirdEnabled;
	bool shakeInFirstEnabled;
	bool expanded;

	void OnEnable ()
	{
		externalShakeInfoList = serializedObject.FindProperty ("externalShakeInfoList");

		manager = (externalShakeListManager)target;
	}

	public override void OnInspectorGUI()
	{
		GUILayout.BeginVertical(GUILayout.Height(30));

		EditorGUILayout.Space();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical("External Shakes States List", "window");
		showExternalShakeInfoList(externalShakeInfoList);
		GUILayout.EndVertical();

		EditorGUILayout.Space();

		if (GUILayout.Button("Update All Head Bob")) {
			manager.udpateAllHeadbobShakeList();
		}

		EditorGUILayout.Space();

		GUILayout.EndVertical();

		EditorGUILayout.Space();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties();
		}
	}

	void showExternalShakeInfoList(SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Multi Axes List", buttonStyle)) {
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
				expanded = false;
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

				if (GUILayout.Button("x")) {
					list.DeleteArrayElementAtIndex(i);
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
		shakeName = "Third Person Shake Values";

		useShakeInThird = "Shake In Third Person Enabled";

		if (sameValue) {
			shakeName = "Shake Values";
			useShakeInThird = "Shake Enabled";
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageShakeInThirdPerson"), new GUIContent (useShakeInThird), false);

		if (!sameValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageShakeInFirstPerson"), new GUIContent ("Shake In First Person Enabled"), false);
		}

		EditorGUILayout.Space ();

		shakeInThirdEnabled = list.FindPropertyRelative ("useDamageShakeInThirdPerson").boolValue;
		shakeInFirstEnabled = list.FindPropertyRelative ("useDamageShakeInFirstPerson").boolValue;

		if (shakeInThirdEnabled) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("thirdPersonDamageShake"), new GUIContent (shakeName), false);
			if (list.FindPropertyRelative ("thirdPersonDamageShake").isExpanded) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical (shakeName, "window");
				showExternalShakeElementInfoContent (list.FindPropertyRelative ("thirdPersonDamageShake"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Test Shake")) {
					if (Application.isPlaying) {
						manager.setExternalShakeStateByIndex (index, false);
					}
				}

				EditorGUILayout.Space ();
			}
		}

		if (shakeInFirstEnabled) {
			if (!list.FindPropertyRelative ("sameValueBothViews").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonDamageShake"));
				if (list.FindPropertyRelative ("firstPersonDamageShake").isExpanded) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("First Person Shake Values", "window");
					showExternalShakeElementInfoContent (list.FindPropertyRelative ("firstPersonDamageShake"));
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Test Shake")) {
						if (Application.isPlaying) {
							manager.setExternalShakeStateByIndex (index, true);
						}
					}

					EditorGUILayout.Space ();
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