using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class vehicleCreatorEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();

	public GameObject currentVehicle;

	bool vehicleCreated;

	float timeToBuild = 0.2f;
	float timer;

	string prefabsPath = "Assets/Game Kit Controller/Prefabs/Vehicles/Vehicles/";

	string currentVehicleName = "New Vehicle";

	bool buttonPressed;

	public GameObject vehicle;

	public vehicleBuilder currentVehicleBuilder;

	bool vehicleSelectedAtStart;

	vehicleBuilder.vehiclePartInfo currentVehiclePartInfo;

	vehicleBuilder.vehicleSettingsInfo currentVehicleSettingsInfo;

	bool showGizmo = true;
	bool useHandle = true;

	bool mainUseVehiclePartState = true;

	bool newVehicleMeshesAdded;

	GUIStyle style = new GUIStyle ();

	Vector2 screenResolution;

	Vector2 rectSize = new Vector2 (560, 600);

	float minHeight = 600f;

	float windowHeightPercentage = 0.7f;

	Vector2 scrollPos1;
	Vector2 scrollPos2;
	Vector2 scrollPos3;
	Vector2 scrollPos4;

	Vector2 previousRectSize;

	Transform currentTransform;

	float maxLayoutWidht = 200;

	[MenuItem ("Game Kit Controller/Create New Vehicle", false, 22)]
	public static void createNewVehicle ()
	{
		GetWindow<vehicleCreatorEditor> ();
	}

	void OnEnable ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float windowHeight = screenResolution.y * windowHeightPercentage;

		previousRectSize = Vector2.zero;

		windowHeight = Mathf.Clamp (windowHeight, minHeight, screenResolution.y);

		rectSize = new Vector2 (560, windowHeight);

//		Debug.Log (rectSize);

		resetCreatorValues ();

		currentVehicle = Selection.activeGameObject;

		if (currentVehicle != null) {
			currentVehicleBuilder = currentVehicle.GetComponent<vehicleBuilder> ();

			if (currentVehicleBuilder != null) {
				vehicleSelectedAtStart = true;

				adjustWindowInfoToNewVehicle ();
			} else {
				currentVehicle = null;
			}
		}
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void adjustWindowInfoToNewVehicle ()
	{
		if (currentVehicleBuilder != null) {
			currentVehicleName = currentVehicleBuilder.vehicleName;

			setExpandOrCollapsePartElementsListState (false);

			setExpandOrCollapseSettingsElementsListState (false);

			currentVehicleBuilder.alignViewWithVehicleCameraPosition ();

			currentVehicleBuilder.getMeshParentTransformValues ();
		}
	}

	void resetCreatorValues ()
	{
		currentVehicle = null;

		if (currentVehicleBuilder != null) {
			currentVehicleBuilder.removeTemporalVehicleParts ();

			currentVehicleBuilder.showGizmo = false;
			currentVehicleBuilder.useHandle = false;
		}

		currentVehicleBuilder = null;

		vehicleCreated = false;

		mainUseVehiclePartState = true;
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Vehicle", null, "Game Kit Controller Vehicle Creator");

		scrollPos3 = EditorGUILayout.BeginScrollView (scrollPos3, false, false);

		GUILayout.BeginVertical (GUILayout.Width (560));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Creator Window", "window", GUILayout.Width (540));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("box");

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Vehicle Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		currentVehicleName = (string)EditorGUILayout.TextField (currentVehicleName, GUILayout.ExpandWidth (true));
		GUILayout.EndHorizontal ();
 
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;
			
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Current Vehicle", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		currentVehicle = EditorGUILayout.ObjectField (currentVehicle, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Show Gizmo", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		showGizmo = (bool)EditorGUILayout.Toggle (showGizmo, GUILayout.ExpandWidth (true));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Window Height", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));

		if (previousRectSize != rectSize) {
			previousRectSize = rectSize;

			this.maxSize = rectSize;
		}

		rectSize.y = EditorGUILayout.Slider (rectSize.y, minHeight, screenResolution.y, GUILayout.ExpandWidth (true));

		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (GUI.changed || vehicleSelectedAtStart) {
			if (currentVehicle != null && currentVehicleBuilder == null) {

				vehicleSelectedAtStart = false;

				currentVehicleBuilder = currentVehicle.GetComponent<vehicleBuilder> ();

				adjustWindowInfoToNewVehicle ();

				GKC_Utils.setActiveGameObjectInEditor (currentVehicle);
			}
		}

		if (currentVehicleBuilder == null) {
			GUILayout.FlexibleSpace ();
		}

		GUILayout.EndVertical ();

		if (currentVehicleBuilder != null) {

			EditorGUILayout.Space ();

			style.normal.textColor = Color.white;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 18;

			GUILayout.Label ("Vehicle Parts Info List", style);

			GUILayout.BeginVertical ("", "window");

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Show Handle", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			useHandle = (bool)EditorGUILayout.Toggle (useHandle);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

			newVehicleMeshesAdded = false;

			for (int i = 0; i < currentVehicleBuilder.vehiclePartInfoList.Count; i++) { 

				currentVehiclePartInfo = currentVehicleBuilder.vehiclePartInfoList [i];

				currentVehiclePartInfo.expandElement = EditorGUILayout.Foldout (currentVehiclePartInfo.expandElement, currentVehiclePartInfo.Name);

				if (currentVehiclePartInfo.expandElement) {
					GUILayout.BeginVertical ("label");

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("New Vehicles Mesh", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					currentVehiclePartInfo.newVehicleMesh = 
						EditorGUILayout.ObjectField (currentVehiclePartInfo.newVehicleMesh, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();
					
					if (currentVehiclePartInfo.newVehicleMesh == null) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Use Vehicle Part", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentVehiclePartInfo.useVehiclePart = (bool)EditorGUILayout.Toggle (currentVehiclePartInfo.useVehiclePart);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
					
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Show Handle", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentVehiclePartInfo.showHandleTool = (bool)EditorGUILayout.Toggle (currentVehiclePartInfo.showHandleTool);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
					} else {
						currentVehiclePartInfo.useVehiclePart = true;

						if (currentVehiclePartInfo.temporalVehicleMesh != null) {
							if (!currentVehicleBuilder.useHandle) {
								currentVehiclePartInfo.newVehicleMeshPositionOffset = (Vector3)EditorGUILayout.Vector3Field ("Position Offset", currentVehiclePartInfo.newVehicleMeshPositionOffset);
								currentVehiclePartInfo.newVehicleMeshEulerOffset = (Vector3)EditorGUILayout.Vector3Field ("Euler Offset", currentVehiclePartInfo.newVehicleMeshEulerOffset);
							}

							EditorGUILayout.Space ();

							if (GUILayout.Button ("O")) {
								GKC_Utils.setActiveGameObjectInEditor (currentVehiclePartInfo.temporalVehicleMesh);
							}
						}
					}

					GUILayout.EndVertical ();
				}

				if (currentVehiclePartInfo.useVehiclePart) {
					if (currentVehiclePartInfo.newVehicleMesh != null) {
						if (!currentVehiclePartInfo.temporalVehicleMeshInstantiated) {
							currentVehiclePartInfo.temporalVehicleMeshInstantiated = true;

							if (currentVehiclePartInfo.currentVehicleMesh != null && currentVehiclePartInfo.currentVehicleMesh.activeSelf) {
								currentVehiclePartInfo.currentVehicleMesh.SetActive (false);
							}

							if (currentVehiclePartInfo.extraVehiclePartMeshesList.Count > 0) {
								for (int j = 0; j < currentVehiclePartInfo.extraVehiclePartMeshesList.Count; j++) { 
									if (currentVehiclePartInfo.extraVehiclePartMeshesList [j] != null && currentVehiclePartInfo.extraVehiclePartMeshesList [j].activeSelf) {
										currentVehiclePartInfo.extraVehiclePartMeshesList [j].SetActive (false);
									}
								}
							}
						}

						if (currentVehiclePartInfo.newVehicleMesh != currentVehiclePartInfo.temporalNewVehicleMesh) {

							if (currentVehiclePartInfo.temporalVehicleMesh) {
								DestroyImmediate (currentVehiclePartInfo.temporalVehicleMesh);
							}

							currentVehiclePartInfo.temporalNewVehicleMesh = currentVehiclePartInfo.newVehicleMesh;

							GameObject newVehiclePart = (GameObject)Instantiate (currentVehiclePartInfo.newVehicleMesh, Vector3.zero, Quaternion.identity);

							Transform newVehiclePartTransform = newVehiclePart.transform;

							newVehiclePartTransform.SetParent (currentVehiclePartInfo.vehicleMeshParent);
							newVehiclePartTransform.localPosition = Vector3.zero;
							newVehiclePartTransform.localRotation = Quaternion.identity;

							currentVehiclePartInfo.temporalVehicleMesh = newVehiclePart;
						}
					} else {
						if (currentVehiclePartInfo.currentVehicleMesh != null && !currentVehiclePartInfo.currentVehicleMesh.activeSelf) {
							currentVehiclePartInfo.currentVehicleMesh.SetActive (true);
						}

						if (currentVehiclePartInfo.extraVehiclePartMeshesList.Count > 0) {
							for (int j = 0; j < currentVehiclePartInfo.extraVehiclePartMeshesList.Count; j++) { 
								if (currentVehiclePartInfo.extraVehiclePartMeshesList [j] != null && !currentVehiclePartInfo.extraVehiclePartMeshesList [j].activeSelf) {
									currentVehiclePartInfo.extraVehiclePartMeshesList [j].SetActive (true);
								}
							}
						}

						if (currentVehiclePartInfo.temporalVehicleMeshInstantiated) {

							if (currentVehiclePartInfo.temporalVehicleMesh != null) {
								DestroyImmediate (currentVehiclePartInfo.temporalVehicleMesh);
							}

							currentVehiclePartInfo.temporalVehicleMeshInstantiated = false;
						}
					}

					if (currentVehiclePartInfo.temporalVehicleMesh != null) {
						if (!currentVehicleBuilder.useHandle) {

							if (currentVehiclePartInfo.moveMeshParentInsteadOfPart) {
								currentTransform = currentVehiclePartInfo.vehicleMeshParent;

								currentTransform.localPosition = currentVehiclePartInfo.originalMeshParentPosition + currentVehiclePartInfo.newVehicleMeshPositionOffset;
								currentTransform.localEulerAngles = currentVehiclePartInfo.originalMeshParentEulerAngles + currentVehiclePartInfo.newVehicleMeshEulerOffset;
							} else {
								currentTransform = currentVehiclePartInfo.temporalVehicleMesh.transform;

								currentTransform.localPosition = Vector3.zero + currentVehiclePartInfo.newVehicleMeshPositionOffset;
								currentTransform.localEulerAngles = Vector3.zero + currentVehiclePartInfo.newVehicleMeshEulerOffset;
							}
						}
					}
				} else {
					if (currentVehiclePartInfo.currentVehicleMesh != null && currentVehiclePartInfo.currentVehicleMesh.activeSelf) {
						currentVehiclePartInfo.currentVehicleMesh.SetActive (false);
					}

					if (currentVehiclePartInfo.extraVehiclePartMeshesList.Count > 0) {
						for (int j = 0; j < currentVehiclePartInfo.extraVehiclePartMeshesList.Count; j++) { 
							if (currentVehiclePartInfo.extraVehiclePartMeshesList [j] != null && currentVehiclePartInfo.extraVehiclePartMeshesList [j].activeSelf) {
								currentVehiclePartInfo.extraVehiclePartMeshesList [j].SetActive (false);
							}
						}
					}
				}

				EditorGUILayout.Space ();

				if (currentVehiclePartInfo.newVehicleMesh != null) {
					newVehicleMeshesAdded = true;
				}
			}   

			EditorGUILayout.EndScrollView ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				setExpandOrCollapsePartElementsListState (true);
			}
			if (GUILayout.Button ("Collapse All")) {
				setExpandOrCollapsePartElementsListState (false);
			}
			if (GUILayout.Button ("Toggle All Parts")) {
				setUseVehiclePartsListState (!mainUseVehiclePartState);
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();


			style.normal.textColor = Color.white;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 18;
			GUILayout.Label ("Vehicle Settings Info List", style);

			GUILayout.BeginVertical ("", "window");

			scrollPos2 = EditorGUILayout.BeginScrollView (scrollPos2, false, false);

			for (int i = 0; i < currentVehicleBuilder.vehicleSettingsInfoList.Count; i++) { 

				currentVehicleSettingsInfo = currentVehicleBuilder.vehicleSettingsInfoList [i];
			
				GUILayout.BeginHorizontal ();
				GUILayout.Label (currentVehicleSettingsInfo.Name, EditorStyles.boldLabel, GUILayout.MaxWidth (250));
				if (currentVehicleSettingsInfo.useBoolState) {
					currentVehicleSettingsInfo.boolState = (bool)EditorGUILayout.Toggle ("", currentVehicleSettingsInfo.boolState, GUILayout.ExpandWidth (true));
				}

				if (currentVehicleSettingsInfo.useFloatValue) {
					currentVehicleSettingsInfo.floatValue = (float)EditorGUILayout.FloatField ("", currentVehicleSettingsInfo.floatValue, GUILayout.ExpandWidth (true));
				}

				GUILayout.EndHorizontal ();
			}   
			
			EditorGUILayout.EndScrollView ();
	
			GUILayout.EndVertical ();

			currentVehicleBuilder.showGizmo = showGizmo;
			currentVehicleBuilder.useHandle = useHandle;

//			GUILayout.FlexibleSpace ();

			scrollPos4 = EditorGUILayout.BeginScrollView (scrollPos4, false, true);

			showMessage ("IMPORTANT: If the option appears, make sure to press with the right mouse button over t" +
			"he GKC vehicle and press the option 'Unpack Prefab Completely'. " +
			"Do the same with the new vehicle part meshes." +
			"\n\nAlso, make sure to add the colliders you need on the vehicle, like mesh colliders, or multiple " +
			"box colliders for more accuracy. Set the layer 'vehicle' to each new collider." +
			"\n\nOnce you finish that, press the button 'Get Vehicle Parts' in the Vehicle Hud Manager component, " +
			"in the vehicle controller gameObject it self, to update the vehicle state.", 11, MessageType.Info);
			
			EditorGUILayout.EndScrollView ();

			if (currentVehicle == null) {
				resetCreatorValues ();
			}

		} else {
			showMessage ("Make sure to select a vehicle prefab of GKC in the field 'Current Vehicle'.\n\n" +
			"Also, drop the vehicle prefab in the scene and assign that prefab into the field, instead of select the prefab from the folders.", 17, MessageType.Info);

			if (currentVehicle != null && currentVehicleBuilder == null) {
				showMessage ("WARNING: The object placed in the field 'Current Vehicle' is not a vehicle." +
				" Please, make sure to assign a GKC vehicle prefab on that field.", 17, MessageType.Warning);
			}
		}
			
		if (currentVehicle != null && currentVehicleBuilder != null) {
			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Cancel")) {
				this.Close ();
			}

			if (GUILayout.Button ("Align View")) {
				if (currentVehicleBuilder) {
					currentVehicleBuilder.alignViewWithVehicleCameraPosition ();

					GKC_Utils.setActiveGameObjectInEditor (currentVehicle);
				}
			}

			if (GUILayout.Button ("Reset Vehicle")) {
				if (currentVehicleBuilder) {
					currentVehicleBuilder.removeTemporalVehicleParts ();
				}
			}

			if (newVehicleMeshesAdded) {
				if (GUILayout.Button ("Create Vehicle")) {
					createVehicle ();
				}
			}

			GUILayout.EndHorizontal ();
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.EndScrollView ();
	}

	void createVehicle ()
	{
		string prefabPath = prefabsPath + currentVehicleName;

		prefabPath += ".prefab";

		bool instantiateGameObject = false;

		if (currentVehicle != null) {
			vehicle = currentVehicle;
		} else {
			instantiateGameObject = true;

			vehicle = (GameObject)AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));
		}

		if (vehicle != null) {
			createVehicleGameObject (instantiateGameObject);
		} else {
			Debug.Log ("Vehicle prefab not found in path " + prefabPath);
		}
	}

	void Update ()
	{
		if (vehicleCreated) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					currentVehicleBuilder.buildVehicle ();

					vehicleCreated = false;

					timer = 0;

					currentVehicleBuilder = null;

					this.Close ();
				}
			}
		}
	}

	public void createVehicleGameObject (bool instantiateGameObject)
	{
		bool newVehiclesMeshesAdded = false;

		for (int i = 0; i < currentVehicleBuilder.vehiclePartInfoList.Count; i++) { 
			if (!newVehiclesMeshesAdded) {
				currentVehiclePartInfo = currentVehicleBuilder.vehiclePartInfoList [i];

				if (currentVehiclePartInfo.newVehicleMesh) {
					newVehiclesMeshesAdded = true;
				}
			}
		}
		
		if (newVehiclesMeshesAdded) {
			if (instantiateGameObject) {
				vehicle = (GameObject)Instantiate (vehicle, Vector3.zero, Quaternion.identity);

				currentVehicleBuilder.placeVehicleInScene = true;
			}

			vehicle.name = currentVehicleName;

			vehicleCreated = true;
		}
	}

	public void setExpandOrCollapsePartElementsListState (bool state)
	{
		if (currentVehicleBuilder != null) {
			for (int i = 0; i < currentVehicleBuilder.vehiclePartInfoList.Count; i++) {
				currentVehiclePartInfo = currentVehicleBuilder.vehiclePartInfoList [i];
				currentVehiclePartInfo.expandElement = state;
			}
		}
	}

	public void setUseVehiclePartsListState (bool state)
	{
		if (currentVehicleBuilder != null) {
			mainUseVehiclePartState = state;

			for (int i = 0; i < currentVehicleBuilder.vehiclePartInfoList.Count; i++) {
				currentVehiclePartInfo = currentVehicleBuilder.vehiclePartInfoList [i];
				currentVehiclePartInfo.useVehiclePart = mainUseVehiclePartState;
			}
		}
	}

	public void setExpandOrCollapseSettingsElementsListState (bool state)
	{
		if (currentVehicleBuilder != null) {
			for (int i = 0; i < currentVehicleBuilder.vehicleSettingsInfoList.Count; i++) {
				currentVehicleSettingsInfo = currentVehicleBuilder.vehicleSettingsInfoList [i];
				currentVehicleSettingsInfo.expandElement = state;
			}
		}
	}

	public void showMessage (string messageContent, int fontSize, MessageType messageType)
	{
		GUILayout.BeginHorizontal ();
		EditorGUILayout.HelpBox ("", messageType);

		style = new GUIStyle (EditorStyles.helpBox);
		style.richText = true;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = fontSize;
		EditorGUILayout.LabelField (messageContent, style);

		GUILayout.EndHorizontal ();
	}
}
#endif