using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(setGravity))]
public class setGravityEditor : Editor
{
	SerializedProperty useWithPlayer;
	SerializedProperty useWithNPC;
	SerializedProperty useWithVehicles;

	SerializedProperty playerTag;
	SerializedProperty friendTag;
	SerializedProperty enemyTag;

	SerializedProperty useWithAnyRigidbody;
	SerializedProperty checkOnlyForArtificialGravitySystem;
	SerializedProperty typeOfTrigger;
	SerializedProperty setGravityMode;
	SerializedProperty setRegularGravity;
	SerializedProperty setZeroGravity;
	SerializedProperty disableZeroGravity;
	SerializedProperty useCustomGravityDirection;
	SerializedProperty customGravityDirection;
	SerializedProperty useCenterPoint;
	SerializedProperty centerPoint;
	SerializedProperty useCenterPointForRigidbodies;
	SerializedProperty useInverseDirectionToCenterPoint;
	SerializedProperty changeGravityDirectionActive;
	SerializedProperty rotateToSurfaceSmoothly;
	SerializedProperty setCircumnavigateSurfaceState;
	SerializedProperty circumnavigateSurfaceState;
	SerializedProperty setCheckSurfaceInFrontState;
	SerializedProperty checkSurfaceInFrontState;
	SerializedProperty setCheckSurfaceBelowLedgeState;
	SerializedProperty checkSurfaceBelowLedgeState;
	SerializedProperty preservePlayerVelocity;
	SerializedProperty storeSetGravityManager;
	SerializedProperty setTargetParent;
	SerializedProperty setRigidbodiesParent;
	SerializedProperty targetParent;

	SerializedProperty useAnimation;
	SerializedProperty animationName;
	SerializedProperty mainAnimation;

	SerializedProperty useCenterPointList;
	SerializedProperty useCenterIfPointListTooClose;
	SerializedProperty useCenterPointListForRigidbodies;
	SerializedProperty centerPointList;

	SerializedProperty dropObjectIfGabbed;
	SerializedProperty dropObjectOnlyIfNotGrabbedPhysically;

	SerializedProperty setCustomGravityForceOnCharactersEnabled;

	SerializedProperty setCustomGravityForce;
	SerializedProperty customGravityForce;

	SerializedProperty useInitialObjectsOnGravityList;

	SerializedProperty initialObjectsOnGravityList;

	setGravity manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		useWithPlayer = serializedObject.FindProperty ("useWithPlayer");
		useWithNPC = serializedObject.FindProperty ("useWithNPC");
		useWithVehicles = serializedObject.FindProperty ("useWithVehicles");

		playerTag = serializedObject.FindProperty ("playerTag");
		friendTag = serializedObject.FindProperty ("friendTag");
		enemyTag = serializedObject.FindProperty ("enemyTag");

		useWithAnyRigidbody = serializedObject.FindProperty ("useWithAnyRigidbody");
		checkOnlyForArtificialGravitySystem = serializedObject.FindProperty ("checkOnlyForArtificialGravitySystem");
		typeOfTrigger = serializedObject.FindProperty ("typeOfTrigger");
		setGravityMode = serializedObject.FindProperty ("setGravityMode");
		setRegularGravity = serializedObject.FindProperty ("setRegularGravity");
		setZeroGravity = serializedObject.FindProperty ("setZeroGravity");
		disableZeroGravity = serializedObject.FindProperty ("disableZeroGravity");
		useCustomGravityDirection = serializedObject.FindProperty ("useCustomGravityDirection");
		customGravityDirection = serializedObject.FindProperty ("customGravityDirection");
		useCenterPoint = serializedObject.FindProperty ("useCenterPoint");
		centerPoint = serializedObject.FindProperty ("centerPoint");
		useCenterPointForRigidbodies = serializedObject.FindProperty ("useCenterPointForRigidbodies");
		useInverseDirectionToCenterPoint = serializedObject.FindProperty ("useInverseDirectionToCenterPoint");
		changeGravityDirectionActive = serializedObject.FindProperty ("changeGravityDirectionActive");
		rotateToSurfaceSmoothly = serializedObject.FindProperty ("rotateToSurfaceSmoothly");
		setCircumnavigateSurfaceState = serializedObject.FindProperty ("setCircumnavigateSurfaceState");
		circumnavigateSurfaceState = serializedObject.FindProperty ("circumnavigateSurfaceState");
		setCheckSurfaceInFrontState = serializedObject.FindProperty ("setCheckSurfaceInFrontState");
		checkSurfaceInFrontState = serializedObject.FindProperty ("checkSurfaceInFrontState");
		setCheckSurfaceBelowLedgeState = serializedObject.FindProperty ("setCheckSurfaceBelowLedgeState");
		checkSurfaceBelowLedgeState = serializedObject.FindProperty ("checkSurfaceBelowLedgeState");
		preservePlayerVelocity = serializedObject.FindProperty ("preservePlayerVelocity");
		storeSetGravityManager = serializedObject.FindProperty ("storeSetGravityManager");
		setTargetParent = serializedObject.FindProperty ("setTargetParent");
		setRigidbodiesParent = serializedObject.FindProperty ("setRigidbodiesParent");
		targetParent = serializedObject.FindProperty ("targetParent");

		useAnimation = serializedObject.FindProperty ("useAnimation");
		animationName = serializedObject.FindProperty ("animationName");
		mainAnimation = serializedObject.FindProperty ("mainAnimation");

		useCenterPointList = serializedObject.FindProperty ("useCenterPointList");
		useCenterIfPointListTooClose = serializedObject.FindProperty ("useCenterIfPointListTooClose");
		useCenterPointListForRigidbodies = serializedObject.FindProperty ("useCenterPointListForRigidbodies");
		centerPointList = serializedObject.FindProperty ("centerPointList");

		dropObjectIfGabbed = serializedObject.FindProperty ("dropObjectIfGabbed");
		dropObjectOnlyIfNotGrabbedPhysically = serializedObject.FindProperty ("dropObjectOnlyIfNotGrabbedPhysically");

		setCustomGravityForceOnCharactersEnabled = serializedObject.FindProperty ("setCustomGravityForceOnCharactersEnabled");

		setCustomGravityForce = serializedObject.FindProperty ("setCustomGravityForce");
		customGravityForce = serializedObject.FindProperty ("customGravityForce");

		useInitialObjectsOnGravityList = serializedObject.FindProperty ("useInitialObjectsOnGravityList");

		initialObjectsOnGravityList = serializedObject.FindProperty ("initialObjectsOnGravityList");

		manager = (setGravity)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useWithPlayer);
		EditorGUILayout.PropertyField (useWithNPC);
		EditorGUILayout.PropertyField (useWithVehicles);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (playerTag);
		EditorGUILayout.PropertyField (friendTag);
		EditorGUILayout.PropertyField (enemyTag);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useWithAnyRigidbody);
		if (useWithAnyRigidbody.boolValue) {
			EditorGUILayout.PropertyField (checkOnlyForArtificialGravitySystem);
		}

		EditorGUILayout.PropertyField (typeOfTrigger);

		EditorGUILayout.PropertyField (setGravityMode);
		if (setGravityMode.boolValue) {
			EditorGUILayout.PropertyField (setRegularGravity);
			EditorGUILayout.PropertyField (setZeroGravity);
			EditorGUILayout.PropertyField (disableZeroGravity);

//			EditorGUILayout.Space();
//
//			GUILayout.BeginVertical("Teleport To Gravity Position Settings", "window");
//			EditorGUILayout.PropertyField(objectToUse.FindProperty("movePlayerToGravityPosition"));
//
//			if (objectToUse.FindProperty("movePlayerToGravityPosition").boolValue) {
//				EditorGUILayout.PropertyField (objectToUse.FindProperty("raycastPositionToGetGravityPosition"));
//				EditorGUILayout.PropertyField (objectToUse.FindProperty("layermaskToGetGravityPosition"));
//				EditorGUILayout.PropertyField (objectToUse.FindProperty("teleportSpeed"));
//				EditorGUILayout.PropertyField (objectToUse.FindProperty("rotationSpeed"));
//			} 
//			GUILayout.EndVertical();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setCustomGravityForce);
		if (setCustomGravityForce.boolValue) {
			EditorGUILayout.PropertyField (customGravityForce);
		}
		EditorGUILayout.PropertyField (setCustomGravityForceOnCharactersEnabled);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Grabbed Objects Settings", "window");
		EditorGUILayout.PropertyField (dropObjectIfGabbed);
		EditorGUILayout.PropertyField (dropObjectOnlyIfNotGrabbedPhysically);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gravity Settings", "window");
		EditorGUILayout.PropertyField (useCustomGravityDirection);
		if (useCustomGravityDirection.boolValue) {
			EditorGUILayout.PropertyField (customGravityDirection);
		} else {
			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Default Gravity Direction is the UP axis of this transform", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

		}
		EditorGUILayout.PropertyField (useCenterPoint);
		if (useCenterPoint.boolValue) {
			EditorGUILayout.PropertyField (centerPoint);
		}

		EditorGUILayout.PropertyField (useCenterPointForRigidbodies);
		if (useCenterPointForRigidbodies.boolValue) {
			EditorGUILayout.PropertyField (useInverseDirectionToCenterPoint);
		}

		EditorGUILayout.PropertyField (changeGravityDirectionActive);
		EditorGUILayout.PropertyField (rotateToSurfaceSmoothly);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setCircumnavigateSurfaceState);
		if (setCircumnavigateSurfaceState.boolValue) {
			EditorGUILayout.PropertyField (circumnavigateSurfaceState);

			EditorGUILayout.PropertyField (setCheckSurfaceInFrontState);
			if (setCheckSurfaceInFrontState.boolValue) {
				EditorGUILayout.PropertyField (checkSurfaceInFrontState);
			}

			EditorGUILayout.PropertyField (setCheckSurfaceBelowLedgeState);
			if (setCheckSurfaceBelowLedgeState.boolValue) {
				EditorGUILayout.PropertyField (checkSurfaceBelowLedgeState);
			}

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Remember to set the tag sphere to those objects that the player will be able to walk on", MessageType.None);
			GUI.color = Color.white;
		
			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (preservePlayerVelocity);

		EditorGUILayout.PropertyField (storeSetGravityManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Parent Settings", "window");
		EditorGUILayout.PropertyField (setTargetParent);
		EditorGUILayout.PropertyField (setRigidbodiesParent);
		if (setTargetParent.boolValue || setRigidbodiesParent.boolValue) {
			EditorGUILayout.PropertyField (targetParent);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window");
		EditorGUILayout.PropertyField (useAnimation);
		if (useAnimation.boolValue) {
			EditorGUILayout.PropertyField (animationName);
			EditorGUILayout.PropertyField (mainAnimation);
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Center Point List Settings", "window");

		EditorGUILayout.PropertyField (useCenterPointList);
		EditorGUILayout.PropertyField (useCenterIfPointListTooClose);
		EditorGUILayout.PropertyField (useCenterPointListForRigidbodies);

		EditorGUILayout.Space ();

		showSimpleList (centerPointList, "Room Points List");
		GUILayout.EndVertical ();

		//EditorGUILayout.Space ();

//		GUILayout.BeginVertical ("Gizmo Settings", "window");
//		EditorGUILayout.PropertyField (objectToUse.FindProperty ("showGizmo"));
//		if (objectToUse.FindProperty ("showGizmo").boolValue) {
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("centerGizmoScale"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("roomCenterColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("gizmoLabelColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("linesColor"));
//			EditorGUILayout.PropertyField (objectToUse.FindProperty ("useHandleForWaypoints"));
//		}
//		GUILayout.EndVertical();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Initial Objects On Gravity List Settings", "window");

		EditorGUILayout.PropertyField (useInitialObjectsOnGravityList);
		if (useInitialObjectsOnGravityList.boolValue) {
			EditorGUILayout.Space ();

			showSimpleList (initialObjectsOnGravityList, "Initial Objects On Gravity List");
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("In Game Options", "window");

		if (GUILayout.Button ("Reverse Gravity Direction")) {
			if (Application.isPlaying) {
				manager.reverseGravityDirection ();
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showGameObjectList (SerializedProperty list, string listName, string objectsTypeName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + objectsTypeName + ": \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Element")) {
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
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif