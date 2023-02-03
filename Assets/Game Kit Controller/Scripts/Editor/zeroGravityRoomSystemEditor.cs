 using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(zeroGravityRoomSystem))]
public class zeroGravityRoomSystemEditor : Editor
{
	SerializedProperty roomHasRegularGravity;
	SerializedProperty roomHasZeroGravity;
	SerializedProperty gravityDirectionTransform;
	SerializedProperty explanation;
	SerializedProperty useNewGravityOutside;
	SerializedProperty outsideHasZeroGravity;
	SerializedProperty outsideGravityDirectionTransform;
	SerializedProperty objectsAffectedByGravity;
	SerializedProperty changeGravityForceForObjects;
	SerializedProperty newGravityForceForObjects;
	SerializedProperty charactersAffectedByGravity;
	SerializedProperty changeGravityForceForCharacters;
	SerializedProperty newGravityForceForCharacters;
	SerializedProperty addForceToObjectsOnZeroGravityState;
	SerializedProperty forceAmountToObjectOnZeroGravity;
	SerializedProperty forceDirectionToObjectsOnZeroGravity;
	SerializedProperty forceModeToObjectsOnZeroGravity;
	SerializedProperty addInitialForceToObjectsOnZeroGravityState;
	SerializedProperty initialForceToObjectsOnZeroGravity;
	SerializedProperty initialForceRadiusToObjectsOnZeroGravity;
	SerializedProperty initialForcePositionToObjectsOnZeroGravity;
	SerializedProperty playerTag;
	SerializedProperty nonAffectedObjectsTagList;
	SerializedProperty charactersTagList;
	SerializedProperty highestPointPoisition;
	SerializedProperty lowestPointPosition;
	SerializedProperty roomPointsParent;
	SerializedProperty roomPointsList;
	SerializedProperty charactersInsideListAtStart;
	SerializedProperty charactersInsideList;
	SerializedProperty addObjectsInsideParent;
	SerializedProperty objectsInsideParent;
	SerializedProperty objectsInsideList;
	SerializedProperty useSounds;
	SerializedProperty mainAudioSource;
	SerializedProperty regularGravitySound;
	SerializedProperty zeroGravitySound;
	SerializedProperty customGravitySound;
	SerializedProperty useSoundsOnCharacters;
	SerializedProperty soundOnEntering;
	SerializedProperty soundOnExiting;
	SerializedProperty debugModeActive;
	SerializedProperty debugModeListActive;
	SerializedProperty objectInfoList;
	SerializedProperty showGizmo;
	SerializedProperty centerGizmoScale;
	SerializedProperty roomCenterColor;
	SerializedProperty gizmoLabelColor;
	SerializedProperty linesColor;
	SerializedProperty useHandleForWaypoints;

	zeroGravityRoomSystem manager;
	string labelText;
	GUIStyle style = new GUIStyle ();

	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform waypoint;
	string currentName;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		roomHasRegularGravity = serializedObject.FindProperty ("roomHasRegularGravity");
		roomHasZeroGravity = serializedObject.FindProperty ("roomHasZeroGravity");
		gravityDirectionTransform = serializedObject.FindProperty ("gravityDirectionTransform");
		explanation = serializedObject.FindProperty ("explanation");
		useNewGravityOutside = serializedObject.FindProperty ("useNewGravityOutside");
		outsideHasZeroGravity = serializedObject.FindProperty ("outsideHasZeroGravity");
		outsideGravityDirectionTransform = serializedObject.FindProperty ("outsideGravityDirectionTransform");
		objectsAffectedByGravity = serializedObject.FindProperty ("objectsAffectedByGravity");
		changeGravityForceForObjects = serializedObject.FindProperty ("changeGravityForceForObjects");
		newGravityForceForObjects = serializedObject.FindProperty ("newGravityForceForObjects");
		charactersAffectedByGravity = serializedObject.FindProperty ("charactersAffectedByGravity");
		changeGravityForceForCharacters = serializedObject.FindProperty ("changeGravityForceForCharacters");
		newGravityForceForCharacters = serializedObject.FindProperty ("newGravityForceForCharacters");
		addForceToObjectsOnZeroGravityState = serializedObject.FindProperty ("addForceToObjectsOnZeroGravityState");
		forceAmountToObjectOnZeroGravity = serializedObject.FindProperty ("forceAmountToObjectOnZeroGravity");
		forceDirectionToObjectsOnZeroGravity = serializedObject.FindProperty ("forceDirectionToObjectsOnZeroGravity");
		forceModeToObjectsOnZeroGravity = serializedObject.FindProperty ("forceModeToObjectsOnZeroGravity");
		addInitialForceToObjectsOnZeroGravityState = serializedObject.FindProperty ("addInitialForceToObjectsOnZeroGravityState");
		initialForceToObjectsOnZeroGravity = serializedObject.FindProperty ("initialForceToObjectsOnZeroGravity");
		initialForceRadiusToObjectsOnZeroGravity = serializedObject.FindProperty ("initialForceRadiusToObjectsOnZeroGravity");
		initialForcePositionToObjectsOnZeroGravity = serializedObject.FindProperty ("initialForcePositionToObjectsOnZeroGravity");
		playerTag = serializedObject.FindProperty ("playerTag");

		nonAffectedObjectsTagList = serializedObject.FindProperty ("nonAffectedObjectsTagList");
		charactersTagList = serializedObject.FindProperty ("charactersTagList");
		highestPointPoisition = serializedObject.FindProperty ("highestPointPoisition");
		lowestPointPosition = serializedObject.FindProperty ("lowestPointPosition");
		roomPointsParent = serializedObject.FindProperty ("roomPointsParent");
		roomPointsList = serializedObject.FindProperty ("roomPointsList");
		charactersInsideListAtStart = serializedObject.FindProperty ("charactersInsideListAtStart");
		charactersInsideList = serializedObject.FindProperty ("charactersInsideList");
		addObjectsInsideParent = serializedObject.FindProperty ("addObjectsInsideParent");
		objectsInsideParent = serializedObject.FindProperty ("objectsInsideParent");
		objectsInsideList = serializedObject.FindProperty ("objectsInsideList");
		useSounds = serializedObject.FindProperty ("useSounds");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		regularGravitySound = serializedObject.FindProperty ("regularGravitySound");
		zeroGravitySound = serializedObject.FindProperty ("zeroGravitySound");
		customGravitySound = serializedObject.FindProperty ("customGravitySound");
		useSoundsOnCharacters = serializedObject.FindProperty ("useSoundsOnCharacters");
		soundOnEntering = serializedObject.FindProperty ("soundOnEntering");
		soundOnExiting = serializedObject.FindProperty ("soundOnExiting");
		debugModeActive = serializedObject.FindProperty ("debugModeActive");
		debugModeListActive = serializedObject.FindProperty ("debugModeListActive");
		objectInfoList = serializedObject.FindProperty ("objectInfoList");

		showGizmo = serializedObject.FindProperty ("showGizmo");
		centerGizmoScale = serializedObject.FindProperty ("centerGizmoScale");
		roomCenterColor = serializedObject.FindProperty ("roomCenterColor");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		linesColor = serializedObject.FindProperty ("linesColor");
		useHandleForWaypoints = serializedObject.FindProperty ("useHandleForWaypoints");

		manager = (zeroGravityRoomSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				if (manager.roomPointsList.Count > 0) {

					for (int i = 0; i < manager.roomPointsList.Count; i++) {
						if (manager.roomPointsList [i]) {
							waypoint = manager.roomPointsList [i];

							Handles.Label (waypoint.position, waypoint.name, style);

							if (manager.useHandleForWaypoints) {
								currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? waypoint.rotation : Quaternion.identity;

								EditorGUI.BeginChangeCheck ();

								oldPoint = waypoint.position;
								oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
								if (EditorGUI.EndChangeCheck ()) {
									Undo.RecordObject (waypoint, "move waypoint" + i);
									waypoint.position = oldPoint;
								}
							}
						}
					}
				}

				currentName = manager.gameObject.name;
				Handles.Label (manager.roomCenter, currentName, style);

				Handles.Label (manager.highestPointPoisition.position, "Highest \n position", style);
				Handles.Label (manager.lowestPointPosition.position, "Lowest \n position", style);
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (roomHasRegularGravity);
		EditorGUILayout.PropertyField (roomHasZeroGravity);
		EditorGUILayout.PropertyField (gravityDirectionTransform);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (explanation);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("New Gravity Outside Of Room Settings", "window");
		EditorGUILayout.PropertyField (useNewGravityOutside);
		if (useNewGravityOutside.boolValue) {
			EditorGUILayout.PropertyField (outsideHasZeroGravity);
			EditorGUILayout.PropertyField (outsideGravityDirectionTransform);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Affected Settings", "window");
		EditorGUILayout.PropertyField (objectsAffectedByGravity);
		if (objectsAffectedByGravity.boolValue) {
			EditorGUILayout.PropertyField (changeGravityForceForObjects);
			if (changeGravityForceForObjects.boolValue) {
				EditorGUILayout.PropertyField (newGravityForceForObjects);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Characters Affected Settings", "window");
		EditorGUILayout.PropertyField (charactersAffectedByGravity);
		if (charactersAffectedByGravity.boolValue) {
			EditorGUILayout.PropertyField (changeGravityForceForCharacters);
			if (changeGravityForceForCharacters.boolValue) {
				EditorGUILayout.PropertyField (newGravityForceForCharacters);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity Settings", "window");
		EditorGUILayout.PropertyField (addForceToObjectsOnZeroGravityState);
		EditorGUILayout.PropertyField (forceAmountToObjectOnZeroGravity);
		EditorGUILayout.PropertyField (forceDirectionToObjectsOnZeroGravity);
		EditorGUILayout.PropertyField (forceModeToObjectsOnZeroGravity);

		EditorGUILayout.PropertyField (addInitialForceToObjectsOnZeroGravityState);
		EditorGUILayout.PropertyField (initialForceToObjectsOnZeroGravity);
		EditorGUILayout.PropertyField (initialForceRadiusToObjectsOnZeroGravity);
		EditorGUILayout.PropertyField (initialForcePositionToObjectsOnZeroGravity);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tags To Affect", "window");
		EditorGUILayout.PropertyField (playerTag);
		showGameObjectList (nonAffectedObjectsTagList, "No Affected Objects Tag List", "Tag");
		showGameObjectList (charactersTagList, "Characters Tag List", "Tag");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Room Points List", "window");
		EditorGUILayout.PropertyField (highestPointPoisition);
		EditorGUILayout.PropertyField (lowestPointPosition);
		EditorGUILayout.PropertyField (roomPointsParent);

		EditorGUILayout.Space ();
		showRoomPointList (roomPointsList, "Room Points List");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Characters Inside Room List", "window");

		showGameObjectList (charactersInsideListAtStart, "Characters Inside Room List At Start", "Characters");

		EditorGUILayout.Space ();

		showGameObjectList (charactersInsideList, "Characters Inside Room List", "Characters");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Inside Room List", "window");
		EditorGUILayout.PropertyField (addObjectsInsideParent);
		if (addObjectsInsideParent.boolValue) {
			EditorGUILayout.PropertyField (objectsInsideParent);
		}

		EditorGUILayout.Space ();

		showGameObjectList (objectsInsideList, "Objects Inside Room List", "Objects");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window");
		EditorGUILayout.PropertyField (useSounds);
		if (useSounds.boolValue) {
			EditorGUILayout.PropertyField (mainAudioSource);
			EditorGUILayout.PropertyField (regularGravitySound);
			EditorGUILayout.PropertyField (zeroGravitySound);
			EditorGUILayout.PropertyField (customGravitySound);
			EditorGUILayout.PropertyField (useSoundsOnCharacters);
			if (useSoundsOnCharacters.boolValue) {
				EditorGUILayout.PropertyField (soundOnEntering);
				EditorGUILayout.PropertyField (soundOnExiting);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window");
		EditorGUILayout.PropertyField (debugModeActive);
		if (debugModeActive.boolValue) {
			EditorGUILayout.PropertyField (debugModeListActive);

			showObjectsInfoList (objectInfoList);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (centerGizmoScale);
			EditorGUILayout.PropertyField (roomCenterColor);
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (linesColor);
			EditorGUILayout.PropertyField (useHandleForWaypoints);
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
			if (GUILayout.Button ("Add Element")) {
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
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showRoomPointList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				manager.addRoomPoint ();
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllRoomPoints ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Rename Room Points")) {
				manager.renameRoomPoints ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					manager.removeRoomPoint (i);
				}
				if (GUILayout.Button ("+")) {
					manager.addRoomPoint (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showObjectsInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);
	
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
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showObjectsInfoListElement (list.GetArrayElementAtIndex (i));
					}
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
					
				GUILayout.EndHorizontal ();
			}
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showObjectsInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isInside"));
		GUILayout.EndVertical ();
	}
}
#endif