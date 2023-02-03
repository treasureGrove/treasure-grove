using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(doorSystem))]
public class doorSystemEditor : Editor
{
	SerializedProperty movementType;
	SerializedProperty animationName;
	SerializedProperty rotateInBothDirections;
	SerializedProperty openSound;
	SerializedProperty closeSound;
	SerializedProperty doorTypeInfo;
	SerializedProperty doorState;
	SerializedProperty openSpeed;
	SerializedProperty hologram;
	SerializedProperty closeDoorOnTriggerExit;
	SerializedProperty closeAfterTime;
	SerializedProperty timeToClose;
	SerializedProperty setMapIconsOnDoor;
	SerializedProperty locked;
	SerializedProperty openDoorWhenUnlocked;
	SerializedProperty useSoundOnUnlock;
	SerializedProperty unlockSound;
	SerializedProperty unlockAudioSource;
	SerializedProperty tagListToOpen;
	SerializedProperty useEventOnOpenAndClose;
	SerializedProperty openEvent;
	SerializedProperty closeEvent;
	SerializedProperty useEventOnUnlockDoor;
	SerializedProperty evenOnUnlockDoor;
	SerializedProperty useEventOnLockDoor;
	SerializedProperty eventOnLockDoor;
	SerializedProperty useEventOnDoorFound;
	SerializedProperty eventOnUnlockedDoorFound;
	SerializedProperty eventOnLockedDoorFound;
	SerializedProperty soundSource;
	SerializedProperty mapObjectInformationManager;
	SerializedProperty mainAnimation;
	SerializedProperty hologramDoorManager;
	SerializedProperty deviceStringActionManager;
	SerializedProperty moving;
	SerializedProperty showGizmo;
	SerializedProperty gizmoArrowLength;
	SerializedProperty gizmoArrowLineLength;
	SerializedProperty gizmoArrowAngle;
	SerializedProperty gizmoArrowColor;
	SerializedProperty doorsInfo;

	doorSystem manager;
	bool usesAnimation;

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		movementType = serializedObject.FindProperty ("movementType");
		animationName = serializedObject.FindProperty ("animationName");
		rotateInBothDirections = serializedObject.FindProperty ("rotateInBothDirections");
		openSound = serializedObject.FindProperty ("openSound");
		closeSound = serializedObject.FindProperty ("closeSound");
		doorTypeInfo = serializedObject.FindProperty ("doorTypeInfo");
		doorState = serializedObject.FindProperty ("doorState");
		openSpeed = serializedObject.FindProperty ("openSpeed");
		hologram = serializedObject.FindProperty ("hologram");
		closeDoorOnTriggerExit = serializedObject.FindProperty ("closeDoorOnTriggerExit");
		closeAfterTime = serializedObject.FindProperty ("closeAfterTime");
		timeToClose = serializedObject.FindProperty ("timeToClose");
		setMapIconsOnDoor = serializedObject.FindProperty ("setMapIconsOnDoor");
		locked = serializedObject.FindProperty ("locked");
		openDoorWhenUnlocked = serializedObject.FindProperty ("openDoorWhenUnlocked");
		useSoundOnUnlock = serializedObject.FindProperty ("useSoundOnUnlock");
		unlockSound = serializedObject.FindProperty ("unlockSound");
		unlockAudioSource = serializedObject.FindProperty ("unlockAudioSource");
		tagListToOpen = serializedObject.FindProperty ("tagListToOpen");
		useEventOnOpenAndClose = serializedObject.FindProperty ("useEventOnOpenAndClose");
		openEvent = serializedObject.FindProperty ("openEvent");
		closeEvent = serializedObject.FindProperty ("closeEvent");
		useEventOnUnlockDoor = serializedObject.FindProperty ("useEventOnUnlockDoor");
		evenOnUnlockDoor = serializedObject.FindProperty ("evenOnUnlockDoor");
		useEventOnLockDoor = serializedObject.FindProperty ("useEventOnLockDoor");
		eventOnLockDoor = serializedObject.FindProperty ("eventOnLockDoor");
		useEventOnDoorFound = serializedObject.FindProperty ("useEventOnDoorFound");
		eventOnUnlockedDoorFound = serializedObject.FindProperty ("eventOnUnlockedDoorFound");
		eventOnLockedDoorFound = serializedObject.FindProperty ("eventOnLockedDoorFound");
		soundSource = serializedObject.FindProperty ("soundSource");
		mapObjectInformationManager = serializedObject.FindProperty ("mapObjectInformationManager");
		mainAnimation = serializedObject.FindProperty ("mainAnimation");
		hologramDoorManager = serializedObject.FindProperty ("hologramDoorManager");
		deviceStringActionManager = serializedObject.FindProperty ("deviceStringActionManager");
		moving = serializedObject.FindProperty ("moving");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoArrowLength = serializedObject.FindProperty ("gizmoArrowLength");
		gizmoArrowLineLength = serializedObject.FindProperty ("gizmoArrowLineLength");
		gizmoArrowAngle = serializedObject.FindProperty ("gizmoArrowAngle");
		gizmoArrowColor = serializedObject.FindProperty ("gizmoArrowColor");
		doorsInfo = serializedObject.FindProperty ("doorsInfo");

		manager = (doorSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Options", "window");
		EditorGUILayout.PropertyField (movementType);

		if (movementType.enumValueIndex == 2) {
			EditorGUILayout.PropertyField (animationName);
			usesAnimation = true;
		} else if (movementType.enumValueIndex == 1) {
			EditorGUILayout.PropertyField (rotateInBothDirections);
			usesAnimation = false;
		} else {
			usesAnimation = false;
		}

		EditorGUILayout.PropertyField (openSound);
		EditorGUILayout.PropertyField (closeSound);
		EditorGUILayout.PropertyField (doorTypeInfo);
		EditorGUILayout.PropertyField (doorState);
		EditorGUILayout.PropertyField (openSpeed);
		EditorGUILayout.PropertyField (hologram);
		EditorGUILayout.PropertyField (closeDoorOnTriggerExit);
		EditorGUILayout.PropertyField (closeAfterTime);
		if (closeAfterTime.boolValue) {
			EditorGUILayout.PropertyField (timeToClose);
		}
		EditorGUILayout.PropertyField (setMapIconsOnDoor);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Settings", "window");
		EditorGUILayout.PropertyField (locked);
		if (locked.boolValue) {
			EditorGUILayout.PropertyField (openDoorWhenUnlocked);

			EditorGUILayout.PropertyField (useSoundOnUnlock);
			if (useSoundOnUnlock.boolValue) {
				EditorGUILayout.PropertyField (unlockSound);
				EditorGUILayout.PropertyField (unlockAudioSource);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Allowed Tag List", "window");
		showAllowedTagList (tagListToOpen);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Open/Close Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnOpenAndClose);
		if (useEventOnOpenAndClose.boolValue) {
			EditorGUILayout.PropertyField (openEvent);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (closeEvent);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Lock/Unlock Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnUnlockDoor);
		if (useEventOnUnlockDoor.boolValue) {
			EditorGUILayout.PropertyField (evenOnUnlockDoor);
		}
		EditorGUILayout.PropertyField (useEventOnLockDoor);
		if (useEventOnLockDoor.boolValue) {
			EditorGUILayout.PropertyField (eventOnLockDoor);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Door Found Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnDoorFound);
		if (useEventOnDoorFound.boolValue) {
			EditorGUILayout.PropertyField (eventOnUnlockedDoorFound);
			EditorGUILayout.PropertyField (eventOnLockedDoorFound);
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (soundSource);
		EditorGUILayout.PropertyField (mapObjectInformationManager);
		EditorGUILayout.PropertyField (mainAnimation);
		EditorGUILayout.PropertyField (hologramDoorManager);
		EditorGUILayout.PropertyField (deviceStringActionManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Door State", "window");		
		GUILayout.Label ("Moving\t " + moving.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!usesAnimation) {

			GUILayout.BeginVertical ("Gizmo Options", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoArrowLength);
				EditorGUILayout.PropertyField (gizmoArrowLineLength);
				EditorGUILayout.PropertyField (gizmoArrowAngle);
				EditorGUILayout.PropertyField (gizmoArrowColor);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Doors List", "window");
			showDoorList (doorsInfo);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showDoorInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("doorMesh"));
		if (manager.movementType == doorSystem.doorMovementType.translate) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("openedPosition"));
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotatedPosition"));
		}
		GUILayout.EndVertical ();
	}

	void showDoorList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Door List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Doors: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Door")) {
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showDoorInfo (list.GetArrayElementAtIndex (i));
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

	void showAllowedTagList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Tag List To Open", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Tag")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif