using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class addNewGenericModelToControllerEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (550, 460);

	float timeToBuild = 0.2f;
	float timer;

	GUIStyle style = new GUIStyle ();

	float windowHeightPercentage = 0.42f;

	Vector2 screenResolution;

	playerComponentsManager mainPlayerComponentsManager;
	customCharacterControllerManager mainCustomCharacterControllerManager;

	public GameObject newGenericModel;
	public string newGenericName;
	public Avatar genericModelAvatar;
	public RuntimeAnimatorController originalAnimatorController;

	public GameObject temporalCharacterCreated;

	public bool characterIsAI;

	bool componentsLocated;

	bool characterAdded;

	bool customizatingCharacterSettingsActive;

	bool addGenericCharacterPrefab;

	bool usePrefabList;

	GameObject genericCharacterPrefab;

	public string[] prefabList;
	public int prefabIndex;
	string newPrefabName;

	bool prefabListAssigned;

	string prefabsPath = "";

	string characterPrefabPath = "Assets/Game Kit Controller/Prefabs/";
	string AITemplatePath = "AI/";
	string AITemplatePrefabName = "AI Main Template Prefab";

	customCharacterControllerBaseBuilder currentCustomCharacterControllerBaseBuilder;

	GameObject newCustomCharacterControllerCreatedGameObject;

	buildPlayer.settingsInfo currentSettingsInfo;
	buildPlayer.settingsInfoCategory currentSettingsInfoCategory;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	[MenuItem ("Game Kit Controller/Generic Models/Add New Generic Model To Controller", false, 2)]
	public static void addNewGenericModelToController ()
	{
		GetWindow<addNewGenericModelToControllerEditor> ();
	}

	void OnEnable ()
	{
		prefabListAssigned = false;

		newPrefabName = "";

		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		//		Debug.Log (screenResolution + " " + partsHeight + " " + settingsHeight + " " + previewHeight);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		if (totalHeight < 500) {
			totalHeight = 500;
		}

		rectSize = new Vector2 (550, totalHeight);

		prefabsPath = pathInfoValues.getGenericCharacterPrefabsPath ();

		resetCreatorValues ();

		checkCurrentCharacterSelected (Selection.activeGameObject);
	}

	void checkCurrentCharacterSelected (GameObject currentCharacterSelected)
	{
		if (currentCharacterSelected != null) {
			mainPlayerComponentsManager = currentCharacterSelected.GetComponentInChildren<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				mainCustomCharacterControllerManager = mainPlayerComponentsManager.getCustomCharacterControllerManager ();

				if (mainCustomCharacterControllerManager != null) {
					componentsLocated = true;

					if (!Directory.Exists (prefabsPath)) {
						Debug.Log ("WARNING: " + prefabsPath + " path doesn't exist, make sure the path is from an existing folder in the project");

						return;
					}

					string[] search_results = null;

					search_results = System.IO.Directory.GetFiles (prefabsPath, "*.prefab");

					if (search_results.Length > 0) {
						prefabList = new string[search_results.Length];
						int currentPrefabIndex = 0;

						foreach (string file in search_results) {
							//must convert file path to relative-to-unity path (and watch for '\' character between Win/Mac)
							GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (file, typeof(GameObject)) as GameObject;

							if (currentPrefab) {
								string currentPrefabName = currentPrefab.name;
								prefabList [currentPrefabIndex] = currentPrefabName;
								currentPrefabIndex++;
							} else {
								Debug.Log ("WARNING: something went wrong when trying to get the prefab in the path " + file);
							}
						}

						prefabListAssigned = true;
					} else {
						Debug.Log ("Prefab not found in path " + prefabsPath);

						prefabList = new string[0];
					}
				}
			}
		}
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		if (!characterAdded) {
			if (temporalCharacterCreated != null) {
				Debug.Log ("Wizard closed without adding new generic model and temporal character instantiated, removing temporal object");

				DestroyImmediate (temporalCharacterCreated);
			}
		}

		newGenericModel = null;
		newGenericName = "";
	
		genericModelAvatar = null;
		originalAnimatorController = null;

		mainCustomCharacterControllerManager = null;

		mainPlayerComponentsManager = null;

		componentsLocated = false;

		characterAdded = false;

		customizatingCharacterSettingsActive = false;

		characterIsAI = false;

		prefabListAssigned = false;

		addGenericCharacterPrefab = false;

		usePrefabList = false;

		temporalCharacterCreated = null;

		newCustomCharacterControllerCreatedGameObject = null;

		Debug.Log ("Generic Model window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Generic", null, "Add Generic Model To Controller");

		GUILayout.BeginVertical ("Add Generic Model To Controller", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		if (customizatingCharacterSettingsActive) {
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 26;
			style.alignment = TextAnchor.MiddleCenter;

			if (currentCustomCharacterControllerBaseBuilder != null) {

				if (currentCustomCharacterControllerBaseBuilder.settingsInfoCategoryList.Count > 0) {
					style.normal.textColor = Color.white;
					style.fontStyle = FontStyle.Bold;
					style.alignment = TextAnchor.MiddleCenter;
					style.fontSize = 18;

					GUILayout.Label ("Character Settings Info List", style);

					GUILayout.BeginVertical ("", "window");

					scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

					for (int i = 0; i < currentCustomCharacterControllerBaseBuilder.settingsInfoCategoryList.Count; i++) { 

						EditorGUILayout.Space ();

						currentSettingsInfoCategory = currentCustomCharacterControllerBaseBuilder.settingsInfoCategoryList [i];

						GUILayout.BeginHorizontal (GUILayout.Width (450));
						GUILayout.Label (currentSettingsInfoCategory.Name.ToUpper (), style);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 
							currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

							if (currentSettingsInfo.settingEnabled) {
								GUILayout.BeginHorizontal (GUILayout.Width (450));
								GUILayout.Label (currentSettingsInfo.Name, EditorStyles.boldLabel);

								if (currentSettingsInfo.useBoolState) {
									currentSettingsInfo.boolState = (bool)EditorGUILayout.Toggle ("", currentSettingsInfo.boolState);
								}

								if (currentSettingsInfo.useFloatValue) {
									currentSettingsInfo.floatValue = (float)EditorGUILayout.FloatField ("", currentSettingsInfo.floatValue);
								}

								if (currentSettingsInfo.useStringValue) {
									currentSettingsInfo.stringValue = (string)EditorGUILayout.TextField ("", currentSettingsInfo.stringValue);
								}

								if (currentSettingsInfo.useVector3Value) {
									currentSettingsInfo.vector3Value = (Vector3)EditorGUILayout.Vector3Field ("", currentSettingsInfo.vector3Value);
								}

								if (currentSettingsInfo.useRegularValue) {
									currentSettingsInfo.regularValue = (bool)EditorGUILayout.Toggle ("", currentSettingsInfo.regularValue);
								}

								GUILayout.EndHorizontal ();

								if (currentSettingsInfo.useFieldExplanation) {
									GUILayout.BeginHorizontal (GUILayout.Width (450));
									EditorGUILayout.HelpBox (currentSettingsInfo.fieldExplanation, MessageType.None);
									GUILayout.EndHorizontal ();
								}
							}
						}

						EditorGUILayout.Space ();
						EditorGUILayout.Space ();
					}   

					EditorGUILayout.Space ();

					EditorGUILayout.EndScrollView ();

					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Apply Settings")) {
					currentCustomCharacterControllerBaseBuilder.adjustSettings ();
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Close")) {
					this.Close ();
				}
			} else {
				this.Close ();
			}
		} else {
			GUILayout.BeginHorizontal ();

			EditorGUILayout.HelpBox ("", MessageType.Info);

			style = new GUIStyle (EditorStyles.helpBox);
			style.richText = true;

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 17;

			EditorGUILayout.LabelField ("Configure the generic model elements and press the button 'Create Character'. \n\n" +
			"If not character is selected in the hierarchy, select one and press the button 'Check Current Object Selected'.\n\n" +
			"This process is the same on Player or AI, make sure to select the character (player or AI) on the hierarchy to add a generic model to it.", style);

			GUILayout.EndHorizontal ();

			if (mainCustomCharacterControllerManager == null) {

				GUILayout.FlexibleSpace ();

				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("", MessageType.Warning);

				style = new GUIStyle (EditorStyles.helpBox);
				style.richText = true;

				style.fontStyle = FontStyle.Bold;
				style.fontSize = 17;
				EditorGUILayout.LabelField ("WARNING: No Character was found, make sure to select the player or an " +
				"AI to add new generic model to it.", style);

				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Check Current Object Selected")) {
					checkCurrentCharacterSelected (Selection.activeGameObject);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Enemy AI Prefab In Scene For Generic Model")) {
					addAIPrefabToConfigureNewGenericModel (true, false);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Friend AI Prefab In Scene For Generic Model")) {
					addAIPrefabToConfigureNewGenericModel (false, false);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Neutral AI Prefab In Scene For Generic Model")) {
					addAIPrefabToConfigureNewGenericModel (false, true);
				}
			} else {
				if (componentsLocated) {

					GUILayout.FlexibleSpace ();

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("box");

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Use Generic Character Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					addGenericCharacterPrefab = (bool)EditorGUILayout.Toggle ("", addGenericCharacterPrefab, GUILayout.ExpandWidth (true));
					GUILayout.EndHorizontal ();
			
					EditorGUILayout.Space ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Character Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					newGenericName = (string)EditorGUILayout.TextField (newGenericName, GUILayout.ExpandWidth (true)); 
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					if (addGenericCharacterPrefab) {

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Use Prefab List", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						usePrefabList = (bool)EditorGUILayout.Toggle (usePrefabList);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						if (usePrefabList) {
							if (prefabListAssigned) {
								if (prefabList.Length > 0) {

									GUILayout.FlexibleSpace ();

									EditorGUILayout.Space ();

									if (prefabIndex < prefabList.Length) {
										prefabIndex = EditorGUILayout.Popup ("Generic Model Prefab", prefabIndex, prefabList);

										newPrefabName = prefabList [prefabIndex];  
									}
								}
							} 
						} else {
							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Generic Character Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							genericCharacterPrefab = EditorGUILayout.ObjectField (genericCharacterPrefab, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;

							GUILayout.EndHorizontal ();

							EditorGUILayout.Space ();
						}
					} else {
						windowRect = GUILayoutUtility.GetLastRect ();
//						windowRect.position = new Vector2 (0, windowRect.position.y);
						windowRect.width = this.maxSize.x;

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Generic Model", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						newGenericModel = EditorGUILayout.ObjectField (newGenericModel, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Generic Avatar", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						genericModelAvatar = EditorGUILayout.ObjectField (genericModelAvatar, typeof(Avatar), true, GUILayout.ExpandWidth (true)) as Avatar;
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Animator Controller", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						originalAnimatorController = EditorGUILayout.ObjectField (originalAnimatorController, typeof(RuntimeAnimatorController), true, GUILayout.ExpandWidth (true)) as RuntimeAnimatorController;
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
					}

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Character is AI", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					characterIsAI = (bool)EditorGUILayout.Toggle (characterIsAI, GUILayout.ExpandWidth (true));
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();

					EditorGUILayout.Space ();

					EditorGUILayout.Space ();

					GUILayout.BeginHorizontal ();

					if (GUILayout.Button ("Cancel")) {
						this.Close ();
					}

					string textButton = "Create Character";

					if (addGenericCharacterPrefab) {
						textButton = "Add Character";
					}

					if (GUILayout.Button (textButton)) {
						if (addGenericCharacterPrefab) {
							if (usePrefabList) {
								if (prefabListAssigned) {
									addCharacter ();

									return;
								} else {
									Debug.Log ("WARNING: Not all elements for the new character have been assigned, make sure to add the proper fileds to it");

								}
							} else {
								if (genericCharacterPrefab != null &&
								    newGenericName != "") {
									addCharacter ();

									return;
								} else {
									Debug.Log ("WARNING: Not all elements for the new character have been assigned, make sure to add the proper fileds to it");

								}
							}
						} else {
							if (newGenericModel != null &&
							    originalAnimatorController != null &&
							    genericModelAvatar != null &&
							    newGenericName != "") {

								addCharacter ();

								return;
							} else {
								Debug.Log ("WARNING: Not all elements for the new character have been assigned, make sure to add the proper fileds to it");
							}
						}
					}

					GUILayout.EndHorizontal ();
				}
			}
		}

		GUILayout.EndVertical ();
	}

	void addAIPrefabToConfigureNewGenericModel (bool AIIsEnemy, bool isNeutral)
	{
		string characterName = AITemplatePrefabName;

		string prefabPath = characterPrefabPath + AITemplatePath;

		prefabPath += characterName;

		prefabPath += ".prefab";

		GameObject newCharacterPrefabObject = (GameObject)AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));

		if (newCharacterPrefabObject != null) {
			GameObject newCharacterObject = GameObject.Instantiate (newCharacterPrefabObject, Vector3.zero, Quaternion.identity) as GameObject;

			newCustomCharacterControllerCreatedGameObject = newCharacterObject;

			newCharacterObject.name = newCharacterPrefabObject.name;

			checkCurrentCharacterSelected (newCharacterObject);

			temporalCharacterCreated = newCharacterObject;

			buildPlayer currentBuildPlayer = newCharacterObject.GetComponent<buildPlayer> ();

			if (currentBuildPlayer != null) {
				currentBuildPlayer.setCharacterValuesAndAdjustSettingsExternally (false, true, AIIsEnemy, isNeutral, "Combat", false, 
					false, false, false);
			}
		} else {
			Debug.Log ("Prefab on path " + prefabPath + " not found");
		}
	}

	void addCharacter ()
	{
		if (addGenericCharacterPrefab) {

			customCharacterControllerBase currentCustomCharacterControllerBase = null;

			if (usePrefabList) {
				string pathForObject = prefabsPath + newPrefabName + ".prefab";

				GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (pathForObject, typeof(GameObject)) as GameObject;

				if (currentPrefab != null) {
					currentCustomCharacterControllerBase = currentPrefab.GetComponent<customCharacterControllerBase> ();
				}
			} else {
				currentCustomCharacterControllerBase = genericCharacterPrefab.GetComponent<customCharacterControllerBase> ();
			}

			if (currentCustomCharacterControllerBase != null) {
				currentCustomCharacterControllerBase.setAIValues = characterIsAI;

				mainCustomCharacterControllerManager.addNewCustomCharacterControllerToSpawn (currentCustomCharacterControllerBase.gameObject, newGenericName);
			
				if (mainCustomCharacterControllerManager != null) {
					GKC_Utils.setActiveGameObjectInEditor (mainCustomCharacterControllerManager.gameObject);
				}
			}
		} else {
			GameObject newCustomCharacterObject = (GameObject)Instantiate (mainCustomCharacterControllerManager.customCharacterPrefab,
				                                      Vector3.zero, Quaternion.identity, mainCustomCharacterControllerManager.transform);

			newCustomCharacterObject.transform.localPosition = Vector3.zero;
			newCustomCharacterObject.transform.localRotation = Quaternion.identity;

			newCustomCharacterObject.name = "Custom Character Controller " + newGenericName;
			
			customCharacterControllerBase currentCustomCharacterControllerBase = newCustomCharacterObject.GetComponent<customCharacterControllerBase> ();

			GameObject genericModelCreated = (GameObject)Instantiate (newGenericModel, Vector3.zero, Quaternion.identity, currentCustomCharacterControllerBase.gameObject.transform);

			genericModelCreated.transform.localPosition = Vector3.zero;
			genericModelCreated.transform.localRotation = Quaternion.identity;

			Animator genericModelAnimator = genericModelCreated.GetComponent<Animator> ();

			genericModelAnimator.enabled = false;


			currentCustomCharacterControllerBase.originalAnimatorController = originalAnimatorController;

			currentCustomCharacterControllerBase.originalAvatar = genericModelAvatar;


			currentCustomCharacterControllerBase.characterGameObject = genericModelCreated;

			currentCustomCharacterControllerBase.characterMeshesList.Add (genericModelCreated);

			genericModelCreated.SetActive (false);

			currentCustomCharacterControllerBase.setNewCameraStates = true;

			currentCustomCharacterControllerBase.newCameraStateThirdPerson = newGenericName + " View Third Person";

			currentCustomCharacterControllerBase.newCameraStateFirstPerson = newGenericName + " View First Person";

			currentCustomCharacterControllerBase.customRagdollInfoName = newGenericName + " Ragdoll";


			playerController mainPlayerController = mainPlayerComponentsManager.getPlayerController ();

			currentCustomCharacterControllerBase.mainAnimator = mainPlayerController.getCharacterAnimator ();

			currentCustomCharacterControllerBase.setAIValues = characterIsAI;

			mainCustomCharacterControllerManager.addNewCustomCharacterController (currentCustomCharacterControllerBase, newGenericName);


			genericRagdollBuilder currentGenericRagdollBuilder = newCustomCharacterObject.GetComponent<genericRagdollBuilder> ();

			if (currentGenericRagdollBuilder != null) {
				currentGenericRagdollBuilder.ragdollName = newGenericName + " Ragdoll";

				currentGenericRagdollBuilder.characterBody = currentCustomCharacterControllerBase.characterGameObject;

				currentGenericRagdollBuilder.mainRagdollActivator = mainPlayerComponentsManager.getRagdollActivator ();

				GKC_Utils.updateComponent (currentGenericRagdollBuilder);

				GKC_Utils.updateDirtyScene ("Update Generic Ragdoll elements", currentGenericRagdollBuilder.gameObject);
			}


			followObjectPositionSystem currentFollowObjectPositionSystem = newCustomCharacterObject.GetComponentInChildren<followObjectPositionSystem> ();

			if (currentFollowObjectPositionSystem != null) {
				currentFollowObjectPositionSystem.setObjectToFollowFromEditor (mainPlayerController.transform);

				GKC_Utils.updateComponent (currentFollowObjectPositionSystem);

				GKC_Utils.updateDirtyScene ("Update Follow Object Position elements", currentFollowObjectPositionSystem.gameObject);
			}

			newCustomCharacterControllerCreatedGameObject = newCustomCharacterObject;

			if (newCustomCharacterControllerCreatedGameObject != null) {
				GKC_Utils.setActiveGameObjectInEditor (newCustomCharacterControllerCreatedGameObject);
			}
		}

		characterAdded = true;
	}

	void Update ()
	{
		if (characterAdded && !customizatingCharacterSettingsActive) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					timer = 0;

					customizatingCharacterSettingsActive = true;

					if (newCustomCharacterControllerCreatedGameObject != null) {
						currentCustomCharacterControllerBaseBuilder = newCustomCharacterControllerCreatedGameObject.GetComponent<customCharacterControllerBaseBuilder> ();
					}

					Repaint ();
				}
			}
		}
	}
}
#endif