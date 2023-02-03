using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(weaponAttachmentSystem))]
public class weaponAttachmentSystemEditor : Editor
{
	SerializedProperty useOffsetPanels;
	SerializedProperty canChangeAttachmentWithNumberKeys;
	SerializedProperty thirdPersonCameraMovementSpeed;
	SerializedProperty canEditWeaponWithoutAttchments;
	SerializedProperty useSmoothTransitionFreeCamera;
	SerializedProperty useSmoothTransitionLockedCamera;
	SerializedProperty setPickedAttachments;
	SerializedProperty startEditWeaponSound;
	SerializedProperty stopEditWeaponSound;
	SerializedProperty UILinesScaleMultiplier;
	SerializedProperty dualWeaponOffsetScale;
	SerializedProperty disableHUDWhenEditingAttachments;
	SerializedProperty showCurrentAttachmentHoverInfo;
	SerializedProperty editingAttachments;
	SerializedProperty showElementSettings;
	SerializedProperty weaponSystem;
	SerializedProperty IKWeaponManager;
	SerializedProperty weaponsManager;
	SerializedProperty attachmentInfoGameObject;
	SerializedProperty attachmentSlotGameObject;
	SerializedProperty weaponAttachmentsMenu;
	SerializedProperty attachmentHoverInfoPanelGameObject;
	SerializedProperty attachmentHoverInfoText;
	SerializedProperty weaponsCamera;
	SerializedProperty mainAudioSource;
	SerializedProperty removeAttachmentSound;
	SerializedProperty attachmentInfoList;
	SerializedProperty showGizmo;
	SerializedProperty showDualWeaponsGizmo;

	SerializedProperty showDebugPrint;

	weaponAttachmentSystem manager;

	Color buttonColor;

	string inputListOpenedText;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		useOffsetPanels = serializedObject.FindProperty ("useOffsetPanels");
		canChangeAttachmentWithNumberKeys = serializedObject.FindProperty ("canChangeAttachmentWithNumberKeys");
		thirdPersonCameraMovementSpeed = serializedObject.FindProperty ("thirdPersonCameraMovementSpeed");
		canEditWeaponWithoutAttchments = serializedObject.FindProperty ("canEditWeaponWithoutAttchments");
		useSmoothTransitionFreeCamera = serializedObject.FindProperty ("useSmoothTransitionFreeCamera");
		useSmoothTransitionLockedCamera = serializedObject.FindProperty ("useSmoothTransitionLockedCamera");
		setPickedAttachments = serializedObject.FindProperty ("setPickedAttachments");
		startEditWeaponSound = serializedObject.FindProperty ("startEditWeaponSound");
		stopEditWeaponSound = serializedObject.FindProperty ("stopEditWeaponSound");
		UILinesScaleMultiplier = serializedObject.FindProperty ("UILinesScaleMultiplier");
		dualWeaponOffsetScale = serializedObject.FindProperty ("dualWeaponOffsetScale");
		disableHUDWhenEditingAttachments = serializedObject.FindProperty ("disableHUDWhenEditingAttachments");
		showCurrentAttachmentHoverInfo = serializedObject.FindProperty ("showCurrentAttachmentHoverInfo");
		editingAttachments = serializedObject.FindProperty ("editingAttachments");
		showElementSettings = serializedObject.FindProperty("showElementSettings");
		weaponSystem = serializedObject.FindProperty ("weaponSystem");
		IKWeaponManager = serializedObject.FindProperty ("IKWeaponManager");
		weaponsManager = serializedObject.FindProperty("weaponsManager");
		attachmentInfoGameObject = serializedObject.FindProperty ("attachmentInfoGameObject");
		attachmentSlotGameObject = serializedObject.FindProperty ("attachmentSlotGameObject");
		weaponAttachmentsMenu = serializedObject.FindProperty ("weaponAttachmentsMenu");
		attachmentHoverInfoPanelGameObject = serializedObject.FindProperty ("attachmentHoverInfoPanelGameObject");
		attachmentHoverInfoText = serializedObject.FindProperty ("attachmentHoverInfoText");
		weaponsCamera = serializedObject.FindProperty ("weaponsCamera");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		removeAttachmentSound = serializedObject.FindProperty ("removeAttachmentSound");
		attachmentInfoList = serializedObject.FindProperty ("attachmentInfoList");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showDualWeaponsGizmo = serializedObject.FindProperty ("showDualWeaponsGizmo");

		showDebugPrint= serializedObject.FindProperty ("showDebugPrint");

		manager = (weaponAttachmentSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useOffsetPanels);
		EditorGUILayout.PropertyField (canChangeAttachmentWithNumberKeys);
		EditorGUILayout.PropertyField (thirdPersonCameraMovementSpeed);
		EditorGUILayout.PropertyField (canEditWeaponWithoutAttchments);
		EditorGUILayout.PropertyField (useSmoothTransitionFreeCamera);
		EditorGUILayout.PropertyField (useSmoothTransitionLockedCamera);
		EditorGUILayout.PropertyField (setPickedAttachments);
		EditorGUILayout.PropertyField (startEditWeaponSound);
		EditorGUILayout.PropertyField (stopEditWeaponSound);
		EditorGUILayout.PropertyField (UILinesScaleMultiplier);
		EditorGUILayout.PropertyField (dualWeaponOffsetScale);
		EditorGUILayout.PropertyField (disableHUDWhenEditingAttachments);
		EditorGUILayout.PropertyField (showCurrentAttachmentHoverInfo);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachment State", "window");
		GUILayout.Label ("Editing Attachments\t " + editingAttachments.boolValue);
		EditorGUILayout.PropertyField (showDebugPrint);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;

		EditorGUILayout.BeginVertical ();

		if (showElementSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showElementSettings.boolValue = !showElementSettings.boolValue;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		if (showElementSettings.boolValue) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Element Settings", "window");
			EditorGUILayout.PropertyField (weaponSystem);	
			EditorGUILayout.PropertyField (IKWeaponManager);
			EditorGUILayout.PropertyField (weaponsManager);	
			EditorGUILayout.PropertyField (attachmentInfoGameObject);
			EditorGUILayout.PropertyField (attachmentSlotGameObject);
			EditorGUILayout.PropertyField (weaponAttachmentsMenu);
			EditorGUILayout.PropertyField (attachmentHoverInfoPanelGameObject);
			EditorGUILayout.PropertyField (attachmentHoverInfoText);
			EditorGUILayout.PropertyField (weaponsCamera);
			EditorGUILayout.PropertyField (mainAudioSource);
			EditorGUILayout.PropertyField (removeAttachmentSound);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachment List", "window");
		showAttachmentPlaceList (attachmentInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		EditorGUILayout.PropertyField (showDualWeaponsGizmo);
		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showAttachmentPlaceListElement (SerializedProperty list, int attachmentPlaceIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentPlaceEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("noAttachmentText"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentPlaceTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("offsetAttachmentPlaceTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dualWeaponOffsetAttachmentPlaceTransform"));	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("offsetAttachmentPointTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentAttachmentSelectedIndex"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects To Replace List", "window");
		showObjectsToReplaceList (list.FindPropertyRelative ("objectToReplace"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachments List", "window");
		showAttachmentList (list.FindPropertyRelative ("attachmentPlaceInfoList"), attachmentPlaceIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showAttachmentPlaceList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Attachment Places: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Place")) {
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Places")) {
				manager.enableOrDisableAllAttachmentPlaces (true);
			}

			if (GUILayout.Button ("Disable All Places")) {
				manager.enableOrDisableAllAttachmentPlaces (false);
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
						showAttachmentPlaceListElement (list.GetArrayElementAtIndex (i), i);
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
		GUILayout.EndVertical ();
	}

	void showObjectsToReplaceList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Objects: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
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

	void showAttachmentList (SerializedProperty list, int attachmentPlaceIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Attachment: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Attachment")) {
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Attachments")) {
				manager.enableOrDisableAllAttachment (true, attachmentPlaceIndex);
			}

			if (GUILayout.Button ("Disable All Attachments")) {
				manager.enableOrDisableAllAttachment (false, attachmentPlaceIndex);
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
						showAttachmentListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showAttachmentListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("onlyEnabledWhileCarrying"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentUseHUD"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("selectAttachmentSound"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachments Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateEvent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateEvent"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPress"));
		if (list.FindPropertyRelative ("useEventOnPress").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPress"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPressDown"));
		if (list.FindPropertyRelative ("useEventOnPressDown").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPressDown"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPressUp"));
		if (list.FindPropertyRelative ("useEventOnPressUp").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPressUp"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventHandPosition"));
		if (list.FindPropertyRelative ("useEventHandPosition").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateEventHandPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateEventHandPosition"));
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachments Hover Info Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAttachmentHoverInfo"));
		if (list.FindPropertyRelative ("useAttachmentHoverInfo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentHoverInfo"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}
}
#endif