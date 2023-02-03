using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(footStepManager))]
public class footStepsManagerEditor : Editor
{
	SerializedProperty stepsEnabled;
	SerializedProperty soundsEnabled;
	SerializedProperty typeOfFootStep;
	SerializedProperty layer;
	SerializedProperty leftFoot;
	SerializedProperty rightFoot;
	SerializedProperty defaultSurfaceName;
	SerializedProperty useFeetVolumeRangeClamps;
	SerializedProperty feetVolumeRangeClamps;
	SerializedProperty useFootPrints;
	SerializedProperty useFootPrintsFromStates;
	SerializedProperty rightFootPrint;
	SerializedProperty leftFootPrint;
	SerializedProperty maxFootPrintDistance;
	SerializedProperty distanceBetweenPrintsInFisrtPerson;
	SerializedProperty useFootPrintMaxAmount;
	SerializedProperty footPrintMaxAmount;
	SerializedProperty removeFootPrintsInTime;
	SerializedProperty timeToRemoveFootPrints;
	SerializedProperty vanishFootPrints;
	SerializedProperty vanishSpeed;
	SerializedProperty useFootParticles;
	SerializedProperty useFootParticlesFromStates;
	SerializedProperty footParticles;
	SerializedProperty footSteps;
	SerializedProperty useFootStepStateList;
	SerializedProperty currentFootStepStateName;
	SerializedProperty noiseDetectionLayer;
	SerializedProperty footStepStateList;
	SerializedProperty useNoiseMesh;
	SerializedProperty showNoiseDetectionGizmo;
	SerializedProperty playerManager;
	SerializedProperty leftFootStep;
	SerializedProperty rightFootStep;
	SerializedProperty leftFootCollider;
	SerializedProperty rightFootCollider;
	SerializedProperty cameraAudioSource;

	SerializedProperty usePoolingSystemEnabled;

	SerializedProperty mainNoiseMeshManagerName;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		stepsEnabled = serializedObject.FindProperty ("stepsEnabled");
		soundsEnabled = serializedObject.FindProperty ("soundsEnabled");
		typeOfFootStep = serializedObject.FindProperty ("typeOfFootStep");
		layer = serializedObject.FindProperty ("layer");
		leftFoot = serializedObject.FindProperty ("leftFoot");
		rightFoot = serializedObject.FindProperty ("rightFoot");
		defaultSurfaceName = serializedObject.FindProperty ("defaultSurfaceName");
		useFeetVolumeRangeClamps = serializedObject.FindProperty ("useFeetVolumeRangeClamps");
		feetVolumeRangeClamps = serializedObject.FindProperty ("feetVolumeRangeClamps");
		useFootPrints = serializedObject.FindProperty ("useFootPrints");
		useFootPrintsFromStates = serializedObject.FindProperty ("useFootPrintsFromStates");
		rightFootPrint = serializedObject.FindProperty ("rightFootPrint");
		leftFootPrint = serializedObject.FindProperty ("leftFootPrint");
		maxFootPrintDistance = serializedObject.FindProperty ("maxFootPrintDistance");
		distanceBetweenPrintsInFisrtPerson = serializedObject.FindProperty ("distanceBetweenPrintsInFisrtPerson");
		useFootPrintMaxAmount = serializedObject.FindProperty ("useFootPrintMaxAmount");
		footPrintMaxAmount = serializedObject.FindProperty ("footPrintMaxAmount");
		removeFootPrintsInTime = serializedObject.FindProperty ("removeFootPrintsInTime");
		timeToRemoveFootPrints = serializedObject.FindProperty ("timeToRemoveFootPrints");
		vanishFootPrints = serializedObject.FindProperty ("vanishFootPrints");
		vanishSpeed = serializedObject.FindProperty ("vanishSpeed");
		useFootParticles = serializedObject.FindProperty ("useFootParticles");
		useFootParticlesFromStates = serializedObject.FindProperty ("useFootParticlesFromStates");
		footParticles = serializedObject.FindProperty ("footParticles");
		footSteps = serializedObject.FindProperty ("footSteps");
		useFootStepStateList = serializedObject.FindProperty ("useFootStepStateList");
		currentFootStepStateName = serializedObject.FindProperty ("currentFootStepStateName");
		noiseDetectionLayer = serializedObject.FindProperty ("noiseDetectionLayer");
		footStepStateList = serializedObject.FindProperty ("footStepStateList");
		useNoiseMesh = serializedObject.FindProperty ("useNoiseMesh");
		showNoiseDetectionGizmo = serializedObject.FindProperty ("showNoiseDetectionGizmo");
		playerManager = serializedObject.FindProperty ("playerManager");
		leftFootStep = serializedObject.FindProperty ("leftFootStep");
		rightFootStep = serializedObject.FindProperty ("rightFootStep");
		leftFootCollider = serializedObject.FindProperty ("leftFootCollider");
		rightFootCollider = serializedObject.FindProperty ("rightFootCollider");
		cameraAudioSource = serializedObject.FindProperty ("cameraAudioSource");

		usePoolingSystemEnabled = serializedObject.FindProperty ("usePoolingSystemEnabled");

		mainNoiseMeshManagerName = serializedObject.FindProperty ("mainNoiseMeshManagerName");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Use Footstep Setting", "window");
		EditorGUILayout.PropertyField (stepsEnabled);

		EditorGUILayout.PropertyField (soundsEnabled);
		GUILayout.EndVertical ();

		if (stepsEnabled.boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (typeOfFootStep);
			EditorGUILayout.PropertyField (layer);
			EditorGUILayout.PropertyField (leftFoot);
			EditorGUILayout.PropertyField (rightFoot);
			EditorGUILayout.PropertyField (defaultSurfaceName);
			EditorGUILayout.PropertyField (usePoolingSystemEnabled);
			EditorGUILayout.PropertyField (mainNoiseMeshManagerName);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("General Volume Settings", "window");
			EditorGUILayout.PropertyField (useFeetVolumeRangeClamps);
			if (useFeetVolumeRangeClamps.boolValue) {
				EditorGUILayout.PropertyField (feetVolumeRangeClamps);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Foot Prints Settings", "window");
			EditorGUILayout.PropertyField (useFootPrints);
			if (useFootPrints.boolValue) {
				EditorGUILayout.PropertyField (useFootPrintsFromStates);

				if (!useFootPrintsFromStates.boolValue) {
					EditorGUILayout.PropertyField (rightFootPrint);
					EditorGUILayout.PropertyField (leftFootPrint);
				}

				EditorGUILayout.PropertyField (maxFootPrintDistance);
				EditorGUILayout.PropertyField (distanceBetweenPrintsInFisrtPerson);
				EditorGUILayout.PropertyField (useFootPrintMaxAmount);
				if (useFootPrintMaxAmount.boolValue) {
					EditorGUILayout.PropertyField (footPrintMaxAmount);
				}
				EditorGUILayout.PropertyField (removeFootPrintsInTime);
				if (removeFootPrintsInTime.boolValue) {
					EditorGUILayout.PropertyField (timeToRemoveFootPrints);
				}
				EditorGUILayout.PropertyField (vanishFootPrints);
				if (vanishFootPrints.boolValue) {
					EditorGUILayout.PropertyField (vanishSpeed);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Foot Particles Settings", "window");
			EditorGUILayout.PropertyField (useFootParticles);
			if (useFootParticles.boolValue) {
				
				EditorGUILayout.PropertyField (useFootParticlesFromStates);
				if (!useFootParticlesFromStates.boolValue) {
					EditorGUILayout.PropertyField (footParticles);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Footstep List", "window");
			showFootStepsList (footSteps);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Footstep State List", "window");
			EditorGUILayout.PropertyField (useFootStepStateList);
			if (useFootStepStateList.boolValue) {
				EditorGUILayout.PropertyField (currentFootStepStateName);

				EditorGUILayout.PropertyField (noiseDetectionLayer);

				EditorGUILayout.Space ();

				showFootStepStateList (footStepStateList);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useNoiseMesh);
			EditorGUILayout.PropertyField (showNoiseDetectionGizmo);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (playerManager);
			EditorGUILayout.PropertyField (leftFootStep);
			EditorGUILayout.PropertyField (rightFootStep);
			EditorGUILayout.PropertyField (leftFootCollider);
			EditorGUILayout.PropertyField (rightFootCollider);
			EditorGUILayout.PropertyField (cameraAudioSource);
			GUILayout.EndVertical ();
		}
			
		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties ();

			Repaint();
		}
	}

	void showFootStepsListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomPool"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (showListNames) {
			GUILayout.BeginVertical ("Pool Sounds", "window");
			showLowerList (list.FindPropertyRelative ("poolSounds"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.BeginVertical ("Surface System Detection Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkSurfaceSystem"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Terrain Detection Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkTerrain"));
		if (list.FindPropertyRelative ("checkTerrain").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("terrainTextureName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("terrainTextureIndex"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Foot Print Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFootPrints"));
		if (list.FindPropertyRelative ("useFootPrints").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rightFootPrint"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftFootPrint"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Foot Particles Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFootParticles"));
		if (list.FindPropertyRelative ("useFootParticles").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("footParticles"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showFootStepsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Surfaces: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Surface")) {
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

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFootStepsListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showFootStepStateList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of States: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFootStepStateListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showFootStepStateListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("feetVolumeRange"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stepInterval"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playSoundOnce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkPlayerOnGround"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreOnGround"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Noise Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoise"));
		if (list.FindPropertyRelative ("useNoise").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseRadius"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseExpandSpeed"));	
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoiseDetection"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoiseMesh"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseDecibels"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceNoiseDetection"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseID"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Others Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewStateAfterDelay"));
		if (list.FindPropertyRelative ("setNewStateAfterDelay").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newStateDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newStateName"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playCustomSound"));
		if (list.FindPropertyRelative ("playCustomSound").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomSoundList"));
			if (list.FindPropertyRelative ("useCustomSoundList").boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Custom Pool Sounds", "window");
				showLowerList (list.FindPropertyRelative ("poolCustomSounds"));
				GUILayout.EndVertical ();
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customSound"));
			}
		}
		GUILayout.EndVertical ();
	}

	void showLowerList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Sound")) {
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