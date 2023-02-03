using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(putObjectSystem))]
public class putObjectSystemEditor : Editor
{
	SerializedProperty putObjectSystemEnabled;
	SerializedProperty useCertainObjectToPlace;
	SerializedProperty certainObjectToPlace;
	SerializedProperty objectNameToPlace;
	SerializedProperty placeToPutObject;
	SerializedProperty placeObjectPositionSpeed;
	SerializedProperty placeObjectRotationSpeed;
	SerializedProperty disableObjectOnceIsPlaced;
	SerializedProperty useRotationLimit;
	SerializedProperty maxUpRotationAngle;
	SerializedProperty maxForwardRotationAngle;
	SerializedProperty usePositionLimit;
	SerializedProperty maxPositionDistance;
	SerializedProperty needsOtherObjectPlacedBefore;
	SerializedProperty numberOfObjectsToPlaceBefore;
	SerializedProperty waitToObjectPlacedToCallEvent;
	SerializedProperty objectPlacedEvent;
	SerializedProperty objectRemovedEvent;
	SerializedProperty useLimitToCheckIfObjectRemoved;
	SerializedProperty minDistanceToRemoveObject;
	SerializedProperty useSoundEffectOnObjectPlaced;
	SerializedProperty soundEffectOnObjectPlaced;
	SerializedProperty useSoundEffectOnObjectRemoved;
	SerializedProperty soundEffectOnObjectRemoved;

	SerializedProperty mainTrigger;
	SerializedProperty mainAudioSource;
	SerializedProperty objectInsideTrigger;
	SerializedProperty objectPlaced;
	SerializedProperty currentObjectPlaced;
	SerializedProperty currentObjectToPlaceSystem;
	SerializedProperty checkingIfObjectIsRemoved;

	SerializedProperty minWaitToPutSameObjectAgain;
	SerializedProperty updateTriggerStateAfterWait;

	void OnEnable ()
	{
		putObjectSystemEnabled = serializedObject.FindProperty ("putObjectSystemEnabled");
		useCertainObjectToPlace = serializedObject.FindProperty ("useCertainObjectToPlace");
		certainObjectToPlace = serializedObject.FindProperty ("certainObjectToPlace");
		objectNameToPlace = serializedObject.FindProperty ("objectNameToPlace");
		placeToPutObject = serializedObject.FindProperty ("placeToPutObject");
		placeObjectPositionSpeed = serializedObject.FindProperty ("placeObjectPositionSpeed");
		placeObjectRotationSpeed = serializedObject.FindProperty ("placeObjectRotationSpeed");
		disableObjectOnceIsPlaced = serializedObject.FindProperty ("disableObjectOnceIsPlaced");
		useRotationLimit = serializedObject.FindProperty ("useRotationLimit");
		maxUpRotationAngle = serializedObject.FindProperty ("maxUpRotationAngle");
		maxForwardRotationAngle = serializedObject.FindProperty ("maxForwardRotationAngle");
		usePositionLimit = serializedObject.FindProperty ("usePositionLimit");
		maxPositionDistance = serializedObject.FindProperty ("maxPositionDistance");
		needsOtherObjectPlacedBefore = serializedObject.FindProperty ("needsOtherObjectPlacedBefore");
		numberOfObjectsToPlaceBefore = serializedObject.FindProperty ("numberOfObjectsToPlaceBefore");
		waitToObjectPlacedToCallEvent = serializedObject.FindProperty ("waitToObjectPlacedToCallEvent");
		objectPlacedEvent = serializedObject.FindProperty ("objectPlacedEvent");
		objectRemovedEvent = serializedObject.FindProperty ("objectRemovedEvent");
		useLimitToCheckIfObjectRemoved = serializedObject.FindProperty ("useLimitToCheckIfObjectRemoved");
		minDistanceToRemoveObject = serializedObject.FindProperty ("minDistanceToRemoveObject");
		useSoundEffectOnObjectPlaced = serializedObject.FindProperty ("useSoundEffectOnObjectPlaced");
		soundEffectOnObjectPlaced = serializedObject.FindProperty ("soundEffectOnObjectPlaced");
		useSoundEffectOnObjectRemoved = serializedObject.FindProperty ("useSoundEffectOnObjectRemoved");
		soundEffectOnObjectRemoved = serializedObject.FindProperty ("soundEffectOnObjectRemoved");

		mainTrigger = serializedObject.FindProperty ("mainTrigger");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		objectInsideTrigger = serializedObject.FindProperty ("objectInsideTrigger");
		objectPlaced = serializedObject.FindProperty ("objectPlaced");
		currentObjectPlaced = serializedObject.FindProperty ("currentObjectPlaced");
		currentObjectToPlaceSystem = serializedObject.FindProperty ("currentObjectToPlaceSystem");
		checkingIfObjectIsRemoved = serializedObject.FindProperty ("checkingIfObjectIsRemoved");

		minWaitToPutSameObjectAgain = serializedObject.FindProperty ("minWaitToPutSameObjectAgain");
		updateTriggerStateAfterWait = serializedObject.FindProperty ("updateTriggerStateAfterWait");
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (putObjectSystemEnabled);
		EditorGUILayout.PropertyField (useCertainObjectToPlace);
		if (useCertainObjectToPlace.boolValue) {
			EditorGUILayout.PropertyField (certainObjectToPlace);
		}
		EditorGUILayout.PropertyField (objectNameToPlace);

		EditorGUILayout.PropertyField (placeToPutObject);
		EditorGUILayout.PropertyField (placeObjectPositionSpeed);
		EditorGUILayout.PropertyField (placeObjectRotationSpeed);
		EditorGUILayout.PropertyField (disableObjectOnceIsPlaced);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (minWaitToPutSameObjectAgain);
		EditorGUILayout.PropertyField (updateTriggerStateAfterWait);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Limit Settings", "window");
		EditorGUILayout.PropertyField (useRotationLimit);
		if (useRotationLimit.boolValue) {
			EditorGUILayout.PropertyField (maxUpRotationAngle);
			EditorGUILayout.PropertyField (maxForwardRotationAngle);
		}
		EditorGUILayout.PropertyField (usePositionLimit);
		if (usePositionLimit.boolValue) {
			EditorGUILayout.PropertyField (maxPositionDistance);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects To Place Before Settings", "window");
		EditorGUILayout.PropertyField (needsOtherObjectPlacedBefore);
		if (needsOtherObjectPlacedBefore.boolValue) {
			EditorGUILayout.PropertyField (numberOfObjectsToPlaceBefore);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();	

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (waitToObjectPlacedToCallEvent);

		EditorGUILayout.Space ();	

		EditorGUILayout.PropertyField (objectPlacedEvent);

		EditorGUILayout.Space ();	

		EditorGUILayout.PropertyField (objectRemovedEvent);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Remove Object Settings", "window");
		EditorGUILayout.PropertyField (useLimitToCheckIfObjectRemoved);
		if (useLimitToCheckIfObjectRemoved.boolValue) {
			EditorGUILayout.PropertyField (minDistanceToRemoveObject);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sounds Settings", "window");
		EditorGUILayout.PropertyField (useSoundEffectOnObjectPlaced);
		if (useSoundEffectOnObjectPlaced.boolValue) {
			EditorGUILayout.PropertyField (soundEffectOnObjectPlaced);
		}
		EditorGUILayout.PropertyField (useSoundEffectOnObjectRemoved);
		if (useSoundEffectOnObjectRemoved.boolValue) {
			EditorGUILayout.PropertyField (soundEffectOnObjectRemoved);
		}
		EditorGUILayout.PropertyField (mainAudioSource);
		EditorGUILayout.PropertyField (mainTrigger);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window");
		EditorGUILayout.PropertyField (objectInsideTrigger);
		EditorGUILayout.PropertyField (objectPlaced);
		EditorGUILayout.PropertyField (currentObjectPlaced);
		EditorGUILayout.PropertyField (currentObjectToPlaceSystem);
		EditorGUILayout.PropertyField (checkingIfObjectIsRemoved);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}
}
#endif