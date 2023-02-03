using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(electronicDevice))]
public class electronicDeviceEditor : Editor
{
	SerializedProperty useOnlyForTrigger;
	SerializedProperty functionToSetPlayer;

	SerializedProperty activateDeviceOnTriggerEnter;

	SerializedProperty useMoveCameraToDevice;
	SerializedProperty disableDeviceWhenStopUsing;
	SerializedProperty stopUsingDeviceWhenUnlock;
	SerializedProperty disableAndRemoveDeviceWhenUnlock;
	SerializedProperty useFreeInteraction;
	SerializedProperty useFreeInteractionEvent;
	SerializedProperty freeInteractionEvent;
	SerializedProperty functionToUseDevice;
	SerializedProperty usingDevice;
	SerializedProperty deviceCanBeUsed;
	SerializedProperty playerInside;
	SerializedProperty activateEventIfUnableToUseDevice;
	SerializedProperty unableToUseDeviceEvent;
	SerializedProperty unlockFunctionCall;
	SerializedProperty lockFunctionCall;
	SerializedProperty activateEventOnTriggerStay;
	SerializedProperty triggerStayEvent;
	SerializedProperty eventOnTriggerStayRate;
	SerializedProperty activateEventOnTriggerEnter;
	SerializedProperty triggerEnterEvent;
	SerializedProperty sendPlayerOnTriggerEnter;
	SerializedProperty eventToSendPlayerOnTriggerEnter;
	SerializedProperty activateEventOnTriggerExit;
	SerializedProperty triggerExitEvent;
	SerializedProperty sendPlayerOnTriggerExit;
	SerializedProperty eventToSendPlayerOnTriggerExit;
	SerializedProperty sendCurrentPlayerOnEvent;
	SerializedProperty setCurrentPlayerEvent;
	SerializedProperty useEventOnStartUsingDevice;
	SerializedProperty eventOnStartUsingDevice;
	SerializedProperty useEventOnStopUsingDevice;
	SerializedProperty eventOnStopUsingDevice;

	SerializedProperty cameraMovementManager;
	SerializedProperty deviceMovementManager;

	SerializedProperty disableDeviceInteractionAfterUsingOnce;

	void OnEnable ()
	{
		useOnlyForTrigger = serializedObject.FindProperty ("useOnlyForTrigger");
		functionToSetPlayer = serializedObject.FindProperty ("functionToSetPlayer");

		activateDeviceOnTriggerEnter = serializedObject.FindProperty ("activateDeviceOnTriggerEnter");

		useMoveCameraToDevice = serializedObject.FindProperty ("useMoveCameraToDevice");
		disableDeviceWhenStopUsing = serializedObject.FindProperty ("disableDeviceWhenStopUsing");
		stopUsingDeviceWhenUnlock = serializedObject.FindProperty ("stopUsingDeviceWhenUnlock");
		disableAndRemoveDeviceWhenUnlock = serializedObject.FindProperty ("disableAndRemoveDeviceWhenUnlock");
		useFreeInteraction = serializedObject.FindProperty ("useFreeInteraction");
		useFreeInteractionEvent = serializedObject.FindProperty ("useFreeInteractionEvent");
		freeInteractionEvent = serializedObject.FindProperty ("freeInteractionEvent");
		functionToUseDevice = serializedObject.FindProperty ("functionToUseDevice");
		usingDevice = serializedObject.FindProperty ("usingDevice");
		deviceCanBeUsed = serializedObject.FindProperty ("deviceCanBeUsed");
		playerInside = serializedObject.FindProperty ("playerInside");
		activateEventIfUnableToUseDevice = serializedObject.FindProperty ("activateEventIfUnableToUseDevice");
		unableToUseDeviceEvent = serializedObject.FindProperty ("unableToUseDeviceEvent");
		unlockFunctionCall = serializedObject.FindProperty ("unlockFunctionCall");
		lockFunctionCall = serializedObject.FindProperty ("lockFunctionCall");
		activateEventOnTriggerStay = serializedObject.FindProperty ("activateEventOnTriggerStay");
		triggerStayEvent = serializedObject.FindProperty ("triggerStayEvent");
		eventOnTriggerStayRate = serializedObject.FindProperty ("eventOnTriggerStayRate");
		activateEventOnTriggerEnter = serializedObject.FindProperty ("activateEventOnTriggerEnter");
		triggerEnterEvent = serializedObject.FindProperty ("triggerEnterEvent");
		sendPlayerOnTriggerEnter = serializedObject.FindProperty ("sendPlayerOnTriggerEnter");
		eventToSendPlayerOnTriggerEnter = serializedObject.FindProperty ("eventToSendPlayerOnTriggerEnter");
		activateEventOnTriggerExit = serializedObject.FindProperty ("activateEventOnTriggerExit");
		triggerExitEvent = serializedObject.FindProperty ("triggerExitEvent");
		sendPlayerOnTriggerExit = serializedObject.FindProperty ("sendPlayerOnTriggerExit");
		eventToSendPlayerOnTriggerExit = serializedObject.FindProperty ("eventToSendPlayerOnTriggerExit");
		sendCurrentPlayerOnEvent = serializedObject.FindProperty ("sendCurrentPlayerOnEvent");
		setCurrentPlayerEvent = serializedObject.FindProperty ("setCurrentPlayerEvent");
		useEventOnStartUsingDevice = serializedObject.FindProperty ("useEventOnStartUsingDevice");
		eventOnStartUsingDevice = serializedObject.FindProperty ("eventOnStartUsingDevice");
		useEventOnStopUsingDevice = serializedObject.FindProperty ("useEventOnStopUsingDevice");
		eventOnStopUsingDevice = serializedObject.FindProperty ("eventOnStopUsingDevice");

		cameraMovementManager = serializedObject.FindProperty ("cameraMovementManager");
		deviceMovementManager = serializedObject.FindProperty ("deviceMovementManager");

		disableDeviceInteractionAfterUsingOnce = serializedObject.FindProperty ("disableDeviceInteractionAfterUsingOnce");
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useOnlyForTrigger);
		EditorGUILayout.PropertyField (functionToSetPlayer);

		EditorGUILayout.PropertyField (activateDeviceOnTriggerEnter);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");

		EditorGUILayout.PropertyField (useMoveCameraToDevice);
		EditorGUILayout.PropertyField (disableDeviceWhenStopUsing);
		EditorGUILayout.PropertyField (stopUsingDeviceWhenUnlock);
		EditorGUILayout.PropertyField (disableAndRemoveDeviceWhenUnlock);
		EditorGUILayout.PropertyField (disableDeviceInteractionAfterUsingOnce);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Component Settings", "window");
		EditorGUILayout.PropertyField (cameraMovementManager);
		EditorGUILayout.PropertyField (deviceMovementManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Free Interaction Settings", "window");
		EditorGUILayout.PropertyField (useFreeInteraction);
		if (useFreeInteraction.boolValue) {
			EditorGUILayout.PropertyField (useFreeInteractionEvent);
			if (useFreeInteractionEvent.boolValue) {
				EditorGUILayout.PropertyField (freeInteractionEvent);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Device Settings", "window");
		EditorGUILayout.PropertyField (functionToUseDevice);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Device State", "window");
		GUILayout.Label ("Using Device\t\t" + usingDevice.boolValue);
		GUILayout.Label ("Device Can Be Used\t\t" + deviceCanBeUsed.boolValue);
		GUILayout.Label ("Player Inside\t\t" + playerInside.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event If Unable To Use Device Settings", "window");
		EditorGUILayout.PropertyField (activateEventIfUnableToUseDevice);
		if (activateEventIfUnableToUseDevice.boolValue) {
			EditorGUILayout.PropertyField (unableToUseDeviceEvent);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Unlock Function Settings", "window");
		EditorGUILayout.PropertyField (unlockFunctionCall);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Lock Function Settings", "window");
		EditorGUILayout.PropertyField (lockFunctionCall);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event On Trigger Stay Settings", "window");
		EditorGUILayout.PropertyField (activateEventOnTriggerStay);
		if (activateEventOnTriggerStay.boolValue) {
			EditorGUILayout.PropertyField (triggerStayEvent);
			EditorGUILayout.PropertyField (eventOnTriggerStayRate);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event On Trigger Enter Settings", "window");
		EditorGUILayout.PropertyField (activateEventOnTriggerEnter);
		if (activateEventOnTriggerEnter.boolValue) {
			EditorGUILayout.PropertyField (triggerEnterEvent);
		}
		EditorGUILayout.PropertyField (sendPlayerOnTriggerEnter);
		if (sendPlayerOnTriggerEnter.boolValue) {
			EditorGUILayout.PropertyField (eventToSendPlayerOnTriggerEnter);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate Event On Trigger Exit Settings", "window");
		EditorGUILayout.PropertyField (activateEventOnTriggerExit);
		if (activateEventOnTriggerExit.boolValue) {
			EditorGUILayout.PropertyField (triggerExitEvent);
		}
		EditorGUILayout.PropertyField (sendPlayerOnTriggerExit);
		if (sendPlayerOnTriggerExit.boolValue) {
			EditorGUILayout.PropertyField (eventToSendPlayerOnTriggerExit);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Send Current Player On Event Settings", "window");
		EditorGUILayout.PropertyField (sendCurrentPlayerOnEvent);
		if (sendCurrentPlayerOnEvent.boolValue) {
			EditorGUILayout.PropertyField (setCurrentPlayerEvent);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Start/Stop Using Device Settings", "window");
		EditorGUILayout.PropertyField (useEventOnStartUsingDevice);
		if (useEventOnStartUsingDevice.boolValue) {
			EditorGUILayout.PropertyField (eventOnStartUsingDevice);
		}
		EditorGUILayout.PropertyField (useEventOnStopUsingDevice);
		if (useEventOnStopUsingDevice.boolValue) {
			EditorGUILayout.PropertyField (eventOnStopUsingDevice);
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif