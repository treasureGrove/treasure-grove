using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(simpleSwitch))]
public class simpleSwitchEditor : Editor
{
	SerializedProperty buttonEnabled;
	SerializedProperty useSingleSwitch;
	SerializedProperty buttonUsesAnimation;
	SerializedProperty switchAnimationName;
	SerializedProperty animationSpeed;
	SerializedProperty notUsableWhileAnimationIsPlaying;
	SerializedProperty pressSound;
	SerializedProperty sendCurrentUser;
	SerializedProperty useUnityEvents;
	SerializedProperty objectToCallFunctions;
	SerializedProperty switchTurnedOn;
	SerializedProperty turnOnEvent;
	SerializedProperty turnOffEvent;
	SerializedProperty objectToActive;
	SerializedProperty activeFunctionName;
	SerializedProperty sendThisButton;
	SerializedProperty audioSource;
	SerializedProperty buttonAnimation;
	SerializedProperty deviceStringActionManager;

	simpleSwitch manager;

	void OnEnable ()
	{
		buttonEnabled = serializedObject.FindProperty ("buttonEnabled");
		useSingleSwitch = serializedObject.FindProperty ("useSingleSwitch");
		buttonUsesAnimation = serializedObject.FindProperty ("buttonUsesAnimation");
		switchAnimationName = serializedObject.FindProperty ("switchAnimationName");
		animationSpeed = serializedObject.FindProperty ("animationSpeed");
		notUsableWhileAnimationIsPlaying = serializedObject.FindProperty ("notUsableWhileAnimationIsPlaying");
		pressSound = serializedObject.FindProperty ("pressSound");
		sendCurrentUser = serializedObject.FindProperty ("sendCurrentUser");
		useUnityEvents = serializedObject.FindProperty ("useUnityEvents");
		objectToCallFunctions = serializedObject.FindProperty ("objectToCallFunctions");
		switchTurnedOn = serializedObject.FindProperty ("switchTurnedOn");
		turnOnEvent = serializedObject.FindProperty ("turnOnEvent");
		turnOffEvent = serializedObject.FindProperty ("turnOffEvent");
		objectToActive = serializedObject.FindProperty ("objectToActive");
		activeFunctionName = serializedObject.FindProperty ("activeFunctionName");
		sendThisButton = serializedObject.FindProperty ("sendThisButton");
		audioSource = serializedObject.FindProperty ("audioSource");
		buttonAnimation = serializedObject.FindProperty ("buttonAnimation");
		deviceStringActionManager = serializedObject.FindProperty ("deviceStringActionManager");

		manager = (simpleSwitch)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (buttonEnabled);
		EditorGUILayout.PropertyField (useSingleSwitch);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window");
		EditorGUILayout.PropertyField (buttonUsesAnimation);
		if (buttonUsesAnimation.boolValue) {
			EditorGUILayout.PropertyField (switchAnimationName);
			EditorGUILayout.PropertyField (animationSpeed);
			EditorGUILayout.PropertyField (notUsableWhileAnimationIsPlaying);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window");
		EditorGUILayout.PropertyField (pressSound);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (sendCurrentUser);
		EditorGUILayout.PropertyField (useUnityEvents);
		if (useUnityEvents.boolValue) {
			
			EditorGUILayout.Space ();
			if (useSingleSwitch.boolValue) {
				EditorGUILayout.PropertyField (objectToCallFunctions);
			} else {
				EditorGUILayout.PropertyField (switchTurnedOn);	

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (turnOnEvent);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (turnOffEvent);
			}
		} else {
			EditorGUILayout.PropertyField (objectToActive);
			EditorGUILayout.PropertyField (activeFunctionName);
			EditorGUILayout.PropertyField (sendThisButton);	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window"); 
		EditorGUILayout.PropertyField (audioSource);
		EditorGUILayout.PropertyField (buttonAnimation);
		EditorGUILayout.PropertyField (deviceStringActionManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window");
		if (GUILayout.Button ("Trigger Button Event")) {
			if (Application.isPlaying) {
				manager.triggerButtonEventFromEditor ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}
}
#endif