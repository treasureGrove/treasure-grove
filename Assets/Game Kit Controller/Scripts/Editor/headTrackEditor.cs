using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(headTrack))]
public class headTrackEditor : Editor
{
	SerializedProperty headTrackEnabled;

	SerializedProperty updateIKEnabled;

	SerializedProperty head;
	SerializedProperty headWeight;
	SerializedProperty bodyWeight;
	SerializedProperty rotationSpeed;
	SerializedProperty weightChangeSpeed;
	SerializedProperty useTimeToChangeTarget;
	SerializedProperty timeToChangeTarget;
	SerializedProperty obstacleLayer;
	SerializedProperty useHeadTrackTarget;
	SerializedProperty headTrackTargeTransform;
	SerializedProperty lookInCameraDirection;
	SerializedProperty cameraTargetToLook;
	SerializedProperty cameraHeadWeight;
	SerializedProperty cameraBodyWeight;
	SerializedProperty cameraRangeAngleX;
	SerializedProperty cameraRangeAngleY;

	SerializedProperty lookInOppositeDirectionExtraRange;

	SerializedProperty lookInOppositeDirectionOutOfRange;
	SerializedProperty oppositeCameraTargetToLook;
	SerializedProperty oppositeCameraTargetToLookParent;
	SerializedProperty oppositeCameraParentRotationSpeed;
	SerializedProperty lookBehindIfMoving;
	SerializedProperty lookBehindRotationSpeed;
	SerializedProperty useDeadZone;
	SerializedProperty deadZoneLookBehind;
	SerializedProperty maxDistanceToHeadToLookAtCameraTarget;
	SerializedProperty useHeadRangeRotation;
	SerializedProperty useTargetsToIgnoreList;
	SerializedProperty targetsToIgnoreList;
	SerializedProperty playerCanLookState;
	SerializedProperty useSmoothHeadTrackDisable;
	SerializedProperty currentHeadWeight;
	SerializedProperty currentbodyWeight;
	SerializedProperty positionToLookFound;
	SerializedProperty lookTargetList;
	SerializedProperty animator;
	SerializedProperty playerControllerManager;
	SerializedProperty playerCameraManager;
	SerializedProperty IKManager;
	SerializedProperty showGizmo;
	SerializedProperty gizmoRadius;
	SerializedProperty arcGizmoRadius;

	headTrack manager;

	Vector2 rangeAngleX;
	Vector2 rangeAngleY;
	Vector3 headPosition;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		headTrackEnabled = serializedObject.FindProperty ("headTrackEnabled");
		updateIKEnabled= serializedObject.FindProperty ("updateIKEnabled");

		head = serializedObject.FindProperty ("head");
		headWeight = serializedObject.FindProperty ("headWeight");
		bodyWeight = serializedObject.FindProperty ("bodyWeight");
		rotationSpeed = serializedObject.FindProperty ("rotationSpeed");
		weightChangeSpeed = serializedObject.FindProperty ("weightChangeSpeed");
		useTimeToChangeTarget = serializedObject.FindProperty ("useTimeToChangeTarget");
		timeToChangeTarget = serializedObject.FindProperty ("timeToChangeTarget");
		obstacleLayer = serializedObject.FindProperty ("obstacleLayer");
		useHeadTrackTarget = serializedObject.FindProperty ("useHeadTrackTarget");
		headTrackTargeTransform = serializedObject.FindProperty ("headTrackTargeTransform");
		lookInCameraDirection = serializedObject.FindProperty ("lookInCameraDirection");
		cameraTargetToLook = serializedObject.FindProperty ("cameraTargetToLook");
		cameraHeadWeight = serializedObject.FindProperty ("cameraHeadWeight");
		cameraBodyWeight = serializedObject.FindProperty ("cameraBodyWeight");
		cameraRangeAngleX = serializedObject.FindProperty ("cameraRangeAngleX");
		cameraRangeAngleY = serializedObject.FindProperty ("cameraRangeAngleY");

		lookInOppositeDirectionExtraRange= serializedObject.FindProperty ("lookInOppositeDirectionExtraRange");

		lookInOppositeDirectionOutOfRange = serializedObject.FindProperty ("lookInOppositeDirectionOutOfRange");
		oppositeCameraTargetToLook = serializedObject.FindProperty ("oppositeCameraTargetToLook");
		oppositeCameraTargetToLookParent = serializedObject.FindProperty ("oppositeCameraTargetToLookParent");
		oppositeCameraParentRotationSpeed = serializedObject.FindProperty ("oppositeCameraParentRotationSpeed");
		lookBehindIfMoving = serializedObject.FindProperty ("lookBehindIfMoving");
		lookBehindRotationSpeed = serializedObject.FindProperty ("lookBehindRotationSpeed");
		useDeadZone = serializedObject.FindProperty ("useDeadZone");
		deadZoneLookBehind = serializedObject.FindProperty ("deadZoneLookBehind");
		maxDistanceToHeadToLookAtCameraTarget = serializedObject.FindProperty ("maxDistanceToHeadToLookAtCameraTarget");
		useHeadRangeRotation = serializedObject.FindProperty ("useHeadRangeRotation");
		useTargetsToIgnoreList = serializedObject.FindProperty ("useTargetsToIgnoreList");
		targetsToIgnoreList = serializedObject.FindProperty ("targetsToIgnoreList");
		playerCanLookState = serializedObject.FindProperty ("playerCanLookState");
		useSmoothHeadTrackDisable = serializedObject.FindProperty ("useSmoothHeadTrackDisable");
		currentHeadWeight = serializedObject.FindProperty ("currentHeadWeight");
		currentbodyWeight = serializedObject.FindProperty ("currentbodyWeight");
		positionToLookFound = serializedObject.FindProperty ("positionToLookFound");
		lookTargetList = serializedObject.FindProperty ("lookTargetList");
		animator = serializedObject.FindProperty ("animator");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		IKManager = serializedObject.FindProperty ("IKManager");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		arcGizmoRadius = serializedObject.FindProperty ("arcGizmoRadius");

		manager = (headTrack)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo && manager.useHeadRangeRotation) {
			Handles.color = Color.white;

			rangeAngleX = manager.rangeAngleX;
			rangeAngleY = manager.rangeAngleY;
			headPosition = manager.head.position;

			Handles.DrawWireArc (headPosition, -manager.transform.up, manager.transform.forward, -rangeAngleY.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, manager.transform.up, manager.transform.forward, rangeAngleY.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (headPosition, manager.transform.up, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleY.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, -manager.transform.up, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleY.y)), manager.arcGizmoRadius);

			Handles.color = Color.white;
			Handles.DrawWireArc (headPosition, -manager.transform.right, manager.transform.forward, -rangeAngleX.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, manager.transform.right, manager.transform.forward, rangeAngleX.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (headPosition, manager.transform.right, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleX.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (headPosition, -manager.transform.right, -manager.transform.forward, (180 - Mathf.Abs (rangeAngleX.y)), manager.arcGizmoRadius);

			string text = "Head Range\n"
			              + "Y: " + (Mathf.Abs (rangeAngleX.x) + rangeAngleX.y)
			              + "\n" + "X: " + (Mathf.Abs (rangeAngleY.x) + rangeAngleY.y);

			Handles.color = Color.red;
			Handles.Label (headPosition + manager.transform.up, text);	
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (headTrackEnabled);
		EditorGUILayout.PropertyField (updateIKEnabled);
		if (headTrackEnabled.boolValue) {
			EditorGUILayout.PropertyField (head);
			EditorGUILayout.PropertyField (headWeight);
			EditorGUILayout.PropertyField (bodyWeight);
			EditorGUILayout.PropertyField (rotationSpeed);
			EditorGUILayout.PropertyField (weightChangeSpeed);	
			EditorGUILayout.PropertyField (useTimeToChangeTarget);
			if (useTimeToChangeTarget.boolValue) {
				EditorGUILayout.PropertyField (timeToChangeTarget);
			}
			EditorGUILayout.PropertyField (obstacleLayer);

			EditorGUILayout.PropertyField (useHeadTrackTarget);
			if (useHeadTrackTarget.boolValue) {
				EditorGUILayout.PropertyField (headTrackTargeTransform);
			}

			EditorGUILayout.Space ();

			if (!head.objectReferenceValue) {
				if (GUILayout.Button ("Assign head")) {
					manager.searchHead ();
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look Camera Direction Settings", "window");
		EditorGUILayout.PropertyField (lookInCameraDirection);
		if (lookInCameraDirection.boolValue) {
			EditorGUILayout.PropertyField (cameraTargetToLook);
			EditorGUILayout.PropertyField (cameraHeadWeight);
			EditorGUILayout.PropertyField (cameraBodyWeight);
			EditorGUILayout.PropertyField (cameraRangeAngleX);
			EditorGUILayout.PropertyField (cameraRangeAngleY);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (lookInOppositeDirectionOutOfRange);	
			if (lookInOppositeDirectionOutOfRange.boolValue) {
				EditorGUILayout.PropertyField (lookInOppositeDirectionExtraRange);	
				EditorGUILayout.PropertyField (oppositeCameraTargetToLook);
				EditorGUILayout.PropertyField (oppositeCameraTargetToLookParent);
				EditorGUILayout.PropertyField (oppositeCameraParentRotationSpeed);
				EditorGUILayout.PropertyField (lookBehindIfMoving);
				if (lookBehindIfMoving.boolValue) {
					EditorGUILayout.PropertyField (lookBehindRotationSpeed);
				}
				EditorGUILayout.PropertyField (useDeadZone);
				if (useDeadZone.boolValue) {
					EditorGUILayout.PropertyField (deadZoneLookBehind);
				}
				EditorGUILayout.PropertyField (maxDistanceToHeadToLookAtCameraTarget);
			}
		}
		GUILayout.EndVertical ();

		if (headTrackEnabled.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Range Settings", "window");
			EditorGUILayout.PropertyField (useHeadRangeRotation);
			if (useHeadRangeRotation.boolValue) {

				GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				manager.rangeAngleX.x = EditorGUILayout.FloatField (manager.rangeAngleX.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref manager.rangeAngleX.x, ref manager.rangeAngleX.y, -180, 180);
				manager.rangeAngleX.y = EditorGUILayout.FloatField (manager.rangeAngleX.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				manager.rangeAngleY.x = EditorGUILayout.FloatField (manager.rangeAngleY.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref manager.rangeAngleY.x, ref manager.rangeAngleY.y, -180, 180);
				manager.rangeAngleY.y = EditorGUILayout.FloatField (manager.rangeAngleY.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Targets To Ignore Settings", "window", GUILayout.Height (30));
			EditorGUILayout.PropertyField (useTargetsToIgnoreList);
			if (useTargetsToIgnoreList.boolValue) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Targets To Ignore List", "window");
				showSimpleList (targetsToIgnoreList);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Head Track State", "window");
			GUILayout.Label ("Can Look State\t" + playerCanLookState.boolValue);
			GUILayout.Label ("Smooth Disable\t" + useSmoothHeadTrackDisable.boolValue);
			GUILayout.Label ("Head Weight\t" + currentHeadWeight.floatValue);
			GUILayout.Label ("Body Weight\t" + currentbodyWeight.floatValue); 
			GUILayout.Label ("Look At Position\t" + positionToLookFound.boolValue);
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Targets To Look List", "window");
			showSimpleList (lookTargetList);
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Element", "window");
			EditorGUILayout.PropertyField (animator);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (playerCameraManager);
			EditorGUILayout.PropertyField (IKManager);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoRadius);
				EditorGUILayout.PropertyField (arcGizmoRadius);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
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
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif
