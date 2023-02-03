using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(ragdollActivator))]
[CanEditMultipleObjects]
public class ragdollActivatorEditor : Editor
{
	SerializedProperty typeOfDeath;
	SerializedProperty timeToGetUp;
	SerializedProperty checkPlayerOnGroundToGetUp;
	SerializedProperty getUpDelay;
	SerializedProperty useDeathSound;
	SerializedProperty deathSound;
	SerializedProperty mainAudioSource;
	SerializedProperty getUpFromBellyAnimatorName;
	SerializedProperty getUpFromBackAnimatorName;
	SerializedProperty deathAnimatorName;
	SerializedProperty deathAnimationID;
	SerializedProperty actionIDAnimatorName;
	SerializedProperty useGetUpFromBellyAfterDeathActive;
	SerializedProperty ragdollToMecanimBlendTime;
	SerializedProperty layer;
	SerializedProperty maxRagdollVelocity;
	SerializedProperty maxVelocityToGetUp;
	SerializedProperty extraForceOnRagdoll;
	SerializedProperty eventOnEnterRagdoll;
	SerializedProperty eventOnExitRagdoll;
	SerializedProperty eventOnDeath;
	SerializedProperty playerState;
	SerializedProperty currentState;
	SerializedProperty onGround;
	SerializedProperty canMove;
	SerializedProperty ragdollCanReceiveDamageOnImpact;
	SerializedProperty minTimeToReceiveDamageOnImpact;
	SerializedProperty minVelocityToReceiveDamageOnImpact;
	SerializedProperty receiveDamageOnImpactMultiplier;
	SerializedProperty minTimToReceiveImpactDamageAgain;
	SerializedProperty usedByAI;
	SerializedProperty timeToShowMenu;
	SerializedProperty tagForColliders;
	SerializedProperty showComponents;

	SerializedProperty characterBody;

	SerializedProperty rootMotion;
	SerializedProperty headTransform;
	SerializedProperty leftFootTransform;
	SerializedProperty rightFootTransform;

	SerializedProperty hipsRigidbody;


	SerializedProperty playerCOM;
	SerializedProperty weaponsManager;
	SerializedProperty mainCameraTransform;
	SerializedProperty statesManager;
	SerializedProperty healthManager;
	SerializedProperty mainCollider;
	SerializedProperty playerInput;
	SerializedProperty mainAnimator;
	SerializedProperty playerControllerManager;
	SerializedProperty cameraManager;
	SerializedProperty powersManager;
	SerializedProperty gravityManager;
	SerializedProperty IKSystemManager;
	SerializedProperty mainRigidbody;
	SerializedProperty stepsManager;
	SerializedProperty combatManager;
	SerializedProperty mainRagdollBuilder;

	SerializedProperty activateRagdollAfterDeath;
	SerializedProperty delayToActivateRagdollAfterDeath;

	SerializedProperty canMoveCharacterOnRagdollState;
	SerializedProperty moveCharacterOnRagdollStateForceAmount;
	SerializedProperty moveCharacterOnRagdollStateForceMode;

	SerializedProperty extraRagdollsInfoList;

	ragdollActivator manager;

	Color defBackgroundColor;
	string buttonText;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		typeOfDeath = serializedObject.FindProperty ("typeOfDeath");
		timeToGetUp = serializedObject.FindProperty ("timeToGetUp");
		checkPlayerOnGroundToGetUp = serializedObject.FindProperty ("checkPlayerOnGroundToGetUp");
		getUpDelay = serializedObject.FindProperty ("getUpDelay");
		useDeathSound = serializedObject.FindProperty ("useDeathSound");
		deathSound = serializedObject.FindProperty ("deathSound");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		getUpFromBellyAnimatorName = serializedObject.FindProperty ("getUpFromBellyAnimatorName");
		getUpFromBackAnimatorName = serializedObject.FindProperty ("getUpFromBackAnimatorName");
		deathAnimatorName = serializedObject.FindProperty ("deathAnimatorName");
		deathAnimationID = serializedObject.FindProperty ("deathAnimationID");
		actionIDAnimatorName = serializedObject.FindProperty ("actionIDAnimatorName");
		useGetUpFromBellyAfterDeathActive = serializedObject.FindProperty ("useGetUpFromBellyAfterDeathActive");
		ragdollToMecanimBlendTime = serializedObject.FindProperty ("ragdollToMecanimBlendTime");
		layer = serializedObject.FindProperty ("layer");
		maxRagdollVelocity = serializedObject.FindProperty ("maxRagdollVelocity");
		maxVelocityToGetUp = serializedObject.FindProperty ("maxVelocityToGetUp");
		extraForceOnRagdoll = serializedObject.FindProperty ("extraForceOnRagdoll");
		eventOnEnterRagdoll = serializedObject.FindProperty ("eventOnEnterRagdoll");
		eventOnExitRagdoll = serializedObject.FindProperty ("eventOnExitRagdoll");
		eventOnDeath = serializedObject.FindProperty ("eventOnDeath");
		playerState = serializedObject.FindProperty ("playerState");
		currentState = serializedObject.FindProperty ("currentState");
		onGround = serializedObject.FindProperty ("onGround");
		canMove = serializedObject.FindProperty ("canMove");
		ragdollCanReceiveDamageOnImpact = serializedObject.FindProperty ("ragdollCanReceiveDamageOnImpact");
		minTimeToReceiveDamageOnImpact = serializedObject.FindProperty ("minTimeToReceiveDamageOnImpact");
		minVelocityToReceiveDamageOnImpact = serializedObject.FindProperty ("minVelocityToReceiveDamageOnImpact");
		receiveDamageOnImpactMultiplier = serializedObject.FindProperty ("receiveDamageOnImpactMultiplier");
		minTimToReceiveImpactDamageAgain = serializedObject.FindProperty ("minTimToReceiveImpactDamageAgain");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		timeToShowMenu = serializedObject.FindProperty ("timeToShowMenu");
		tagForColliders = serializedObject.FindProperty ("tagForColliders");
		showComponents = serializedObject.FindProperty ("showComponents");

		characterBody = serializedObject.FindProperty ("characterBody");

		rootMotion = serializedObject.FindProperty ("rootMotion");
		headTransform = serializedObject.FindProperty ("headTransform");
		leftFootTransform = serializedObject.FindProperty ("leftFootTransform");
		rightFootTransform = serializedObject.FindProperty ("rightFootTransform");
		hipsRigidbody = serializedObject.FindProperty ("hipsRigidbody");

		playerCOM = serializedObject.FindProperty ("playerCOM");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		statesManager = serializedObject.FindProperty ("statesManager");
		healthManager = serializedObject.FindProperty ("healthManager");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		playerInput = serializedObject.FindProperty ("playerInput");
		mainAnimator = serializedObject.FindProperty ("mainAnimator");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		cameraManager = serializedObject.FindProperty ("cameraManager");
		powersManager = serializedObject.FindProperty ("powersManager");
		gravityManager = serializedObject.FindProperty ("gravityManager");
		IKSystemManager = serializedObject.FindProperty ("IKSystemManager");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");
		stepsManager = serializedObject.FindProperty ("stepsManager");
		combatManager = serializedObject.FindProperty ("combatManager");
		mainRagdollBuilder = serializedObject.FindProperty ("mainRagdollBuilder");

		activateRagdollAfterDeath = serializedObject.FindProperty ("activateRagdollAfterDeath");
		delayToActivateRagdollAfterDeath = serializedObject.FindProperty ("delayToActivateRagdollAfterDeath");

		canMoveCharacterOnRagdollState = serializedObject.FindProperty ("canMoveCharacterOnRagdollState");
		moveCharacterOnRagdollStateForceAmount = serializedObject.FindProperty ("moveCharacterOnRagdollStateForceAmount");
		moveCharacterOnRagdollStateForceMode = serializedObject.FindProperty ("moveCharacterOnRagdollStateForceMode");

		extraRagdollsInfoList = serializedObject.FindProperty ("extraRagdollsInfoList");

		manager = (ragdollActivator)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (typeOfDeath);
		EditorGUILayout.PropertyField (timeToGetUp);
		EditorGUILayout.PropertyField (checkPlayerOnGroundToGetUp);

		EditorGUILayout.PropertyField (getUpDelay);
		EditorGUILayout.PropertyField (useDeathSound);
		if (useDeathSound.boolValue) {
			EditorGUILayout.PropertyField (deathSound);
			EditorGUILayout.PropertyField (mainAudioSource);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (canMoveCharacterOnRagdollState);
		if (canMoveCharacterOnRagdollState.boolValue) {
			EditorGUILayout.PropertyField (moveCharacterOnRagdollStateForceAmount);
			EditorGUILayout.PropertyField (moveCharacterOnRagdollStateForceMode);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window");
		EditorGUILayout.PropertyField (getUpFromBellyAnimatorName);
		EditorGUILayout.PropertyField (getUpFromBackAnimatorName);
		EditorGUILayout.PropertyField (deathAnimatorName);
		EditorGUILayout.PropertyField (deathAnimationID);
		EditorGUILayout.PropertyField (actionIDAnimatorName);
		EditorGUILayout.PropertyField (useGetUpFromBellyAfterDeathActive);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (activateRagdollAfterDeath);
		if (activateRagdollAfterDeath.boolValue) {
			EditorGUILayout.PropertyField (delayToActivateRagdollAfterDeath);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (manager.typeOfDeath == ragdollActivator.deathType.ragdoll) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Ragdoll Physics Settings", "window");
			EditorGUILayout.PropertyField (ragdollToMecanimBlendTime);
			EditorGUILayout.PropertyField (layer);
			EditorGUILayout.PropertyField (maxRagdollVelocity);
			EditorGUILayout.PropertyField (maxVelocityToGetUp);
			EditorGUILayout.PropertyField (extraForceOnRagdoll);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ragdoll Events Settings", "window");
		EditorGUILayout.PropertyField (eventOnEnterRagdoll);
		EditorGUILayout.PropertyField (eventOnExitRagdoll);
		EditorGUILayout.PropertyField (eventOnDeath);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ragdoll State", "window");
		EditorGUILayout.PropertyField (playerState);
		EditorGUILayout.PropertyField (currentState);
		GUILayout.Label ("On Ground\t\t" + onGround.boolValue);
		GUILayout.Label ("Can Move\t\t" + canMove.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Receive Damage On Impact Settings", "window");
		EditorGUILayout.PropertyField (ragdollCanReceiveDamageOnImpact);
		if (ragdollCanReceiveDamageOnImpact.boolValue) {
			EditorGUILayout.PropertyField (minTimeToReceiveDamageOnImpact);
			EditorGUILayout.PropertyField (minVelocityToReceiveDamageOnImpact);
			EditorGUILayout.PropertyField (receiveDamageOnImpactMultiplier);
			EditorGUILayout.PropertyField (minTimToReceiveImpactDamageAgain);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Extra Ragdoll List Settings", "window");
		showExtraRagdollsInfoList (extraRagdollsInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!usedByAI.boolValue) {
			GUILayout.BeginVertical ("Player Settings", "window");
			EditorGUILayout.PropertyField (timeToShowMenu);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (usedByAI);
		EditorGUILayout.PropertyField (tagForColliders);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showComponents.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonText = "Hide Components";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonText = "Show Components";
		}
		if (GUILayout.Button (buttonText)) {
			showComponents.boolValue = !showComponents.boolValue;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showComponents.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (characterBody);

			EditorGUILayout.PropertyField (rootMotion);
			EditorGUILayout.PropertyField (headTransform);
			EditorGUILayout.PropertyField (leftFootTransform);
			EditorGUILayout.PropertyField (rightFootTransform);
			EditorGUILayout.PropertyField (hipsRigidbody);

			EditorGUILayout.PropertyField (playerCOM);
			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (mainCameraTransform);
			EditorGUILayout.PropertyField (statesManager);
			EditorGUILayout.PropertyField (healthManager);
			EditorGUILayout.PropertyField (mainCollider);
			EditorGUILayout.PropertyField (playerInput);
			EditorGUILayout.PropertyField (mainAnimator);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (cameraManager);
			EditorGUILayout.PropertyField (powersManager);
			EditorGUILayout.PropertyField (gravityManager);
			EditorGUILayout.PropertyField (IKSystemManager);
			EditorGUILayout.PropertyField (mainRigidbody);
			EditorGUILayout.PropertyField (stepsManager);
			EditorGUILayout.PropertyField (combatManager);
			EditorGUILayout.PropertyField (mainRagdollBuilder);
			GUILayout.EndVertical ();
		}
			
		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
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
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			EditorGUILayout.Space ();

//			GUILayout.BeginHorizontal ();
//			if (GUILayout.Button ("Add Function")) {
//				list.arraySize++;
//			}
//			if (GUILayout.Button ("Clear")) {
//				list.arraySize = 0;
//			}
//			GUILayout.EndHorizontal ();
//
//			EditorGUILayout.Space ();

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
			}
		}       
	}

	void showExtraRagdollsInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Type")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showExtraRagdollsInfoListElement (list.GetArrayElementAtIndex (i));
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

			GUILayout.EndVertical ();
		}       
	}

	void showExtraRagdollsInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentRagdoll"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("characterBody"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rootMotion"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hipsRigidbody"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationYOffset"));

		GUILayout.EndVertical ();
	}
}
#endif