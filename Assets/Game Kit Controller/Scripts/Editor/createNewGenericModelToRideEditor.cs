using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class createNewGenericModelToRideEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (550, 550);

	float timeToBuild = 0.2f;
	float timer;

	float windowHeightPercentage = 0.5f;

	Vector2 screenResolution;

	public GameObject newGenericModel;
	public string newGenericName;
	public Avatar genericModelAvatar;
	public RuntimeAnimatorController originalAnimatorController;

	bool characterAdded;

	public bool useCustomGenericCharacterPrefab;

	public GameObject genericCharacterPrefab;

	string prefabsPath = "";

	GUIStyle style = new GUIStyle ();

	float maxLayoutWidht = 220;

	[MenuItem ("Game Kit Controller/Generic Models/Create New Generic Model Controller To Ride", false, 2)]
	public static void createNewGenericModelToRide ()
	{
		GetWindow<createNewGenericModelToRideEditor> ();
	}

	void OnEnable ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		if (totalHeight < 550) {
			totalHeight = 550;
		}

		rectSize = new Vector2 (550, totalHeight);

		prefabsPath = pathInfoValues.getGenericCharacterToRidePrefabPath ();
	
		genericCharacterPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (prefabsPath, typeof(GameObject)) as GameObject;
	
		resetCreatorValues ();
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		newGenericModel = null;
		newGenericName = "";

		genericModelAvatar = null;
		originalAnimatorController = null;

		characterAdded = false;

		Debug.Log ("Generic Model window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Generic Model To Ride Creator", null, "Generic Model To Ride Creator");

		GUILayout.BeginVertical ("Generic Model To Ride", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		GUILayout.BeginHorizontal ();

		EditorGUILayout.HelpBox ("", MessageType.Info);

		style = new GUIStyle (EditorStyles.helpBox);
		style.richText = true;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 17;

		EditorGUILayout.LabelField ("Configure the generic model elements and press the button 'Create Character'. \n\n" +
		"Make sure to create a duplicate of the Generic Model Animator Template (use the search tool on the project window). \n\n" +
		"Then set the animations of the new generic model on that new animator, " +
		"and assign that as the new animator to use on the Animator Controller field of this wizard.", style);

		GUILayout.EndHorizontal ();
	
		GUILayout.FlexibleSpace ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		newGenericName = (string)EditorGUILayout.TextField (newGenericName, GUILayout.ExpandWidth (true)); 
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("New Generic Model", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		newGenericModel = EditorGUILayout.ObjectField (newGenericModel, typeof(GameObject), true, GUILayout.ExpandWidth (true))as GameObject;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Generic Model Avatar", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		genericModelAvatar = EditorGUILayout.ObjectField (genericModelAvatar, typeof(Avatar), true, GUILayout.ExpandWidth (true))as Avatar;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Animator Controller", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		originalAnimatorController = EditorGUILayout.ObjectField (originalAnimatorController, typeof(RuntimeAnimatorController), true, GUILayout.ExpandWidth (true))as RuntimeAnimatorController;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
			
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Custom Generic Character Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCustomGenericCharacterPrefab = (bool)EditorGUILayout.Toggle (useCustomGenericCharacterPrefab, GUILayout.ExpandWidth (true));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (useCustomGenericCharacterPrefab) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Generic Character Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			genericCharacterPrefab = EditorGUILayout.ObjectField (genericCharacterPrefab, typeof(GameObject), true, GUILayout.ExpandWidth (true))as GameObject;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Cancel")) {
			this.Close ();
		}

		string textButton = "Create Character";

		if (GUILayout.Button (textButton)) {
					
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

		GUILayout.EndHorizontal ();

		GUILayout.EndVertical ();
	}

	void addCharacter ()
	{
		if (genericCharacterPrefab == null) {
			Debug.Log ("WARNING: something went wrong when trying to use the configured generic character or get the prefab in the path " + prefabsPath);

			return;
		}

		if (genericCharacterPrefab.GetComponent<Animator> () == null) {
			Debug.Log ("WARNING: the model selected doesn't have an animator, make sure to configure it properly");

			return;
		}

		GameObject newCustomCharacterObject = (GameObject)Instantiate (genericCharacterPrefab, Vector3.zero, Quaternion.identity);

		newCustomCharacterObject.name = newGenericName + " Generic Model Vehicle To Ride"; 

		genericRagdollBuilder currentGenericRagdollBuilder = newCustomCharacterObject.GetComponentInChildren<genericRagdollBuilder> ();


		GameObject previousGenericModel = currentGenericRagdollBuilder.characterBody;

		Transform genericModelParent = previousGenericModel.transform.parent;

		GameObject genericModelCreated = (GameObject)Instantiate (newGenericModel, Vector3.zero, Quaternion.identity, genericModelParent);

		genericModelCreated.transform.localPosition = Vector3.zero;
		genericModelCreated.transform.localRotation = Quaternion.identity;


		Animator genericModelAnimator = genericModelCreated.GetComponent<Animator> ();

		genericModelAnimator.enabled = false;



		customCharacterControllerBase currentCustomCharacterControllerBase = newCustomCharacterObject.GetComponentInChildren<customCharacterControllerBase> ();

		currentCustomCharacterControllerBase.originalAnimatorController = originalAnimatorController;

		currentCustomCharacterControllerBase.originalAvatar = genericModelAvatar;

		currentCustomCharacterControllerBase.mainAnimator.runtimeAnimatorController = originalAnimatorController;

		currentCustomCharacterControllerBase.mainAnimator.avatar = genericModelAvatar;

		currentCustomCharacterControllerBase.characterGameObject = genericModelCreated;

		currentCustomCharacterControllerBase.characterMeshesList.Clear ();

		currentCustomCharacterControllerBase.characterMeshesList.Add (genericModelCreated);


		if (currentGenericRagdollBuilder != null) {
			currentGenericRagdollBuilder.ragdollName = newGenericName + " Ragdoll";

			currentGenericRagdollBuilder.characterBody = currentCustomCharacterControllerBase.characterGameObject;

			currentGenericRagdollBuilder.removeRagdoll ();

			GKC_Utils.updateComponent (currentGenericRagdollBuilder);

			GKC_Utils.updateDirtyScene ("Update Generic Ragdoll elements", currentGenericRagdollBuilder.gameObject);
		}

		vehicleWeaponSystem currentVehicleWeaponSystem = newCustomCharacterObject.GetComponentInChildren<vehicleWeaponSystem> ();

		if (currentVehicleWeaponSystem != null) {
			currentVehicleWeaponSystem.setMainVehicleWeaponsGameObjectParent (genericModelCreated.transform);

			GKC_Utils.updateComponent (currentVehicleWeaponSystem);
		}

		followObjectPositionUpdateSystem currentFollowObjectPositionSystem = newCustomCharacterObject.GetComponentInChildren<followObjectPositionUpdateSystem> ();

		if (currentFollowObjectPositionSystem != null) {
			currentFollowObjectPositionSystem.setObjectToFollowFromEditor (genericModelCreated.transform);

			currentFollowObjectPositionSystem.setEnabledState (false);

			GKC_Utils.updateComponent (currentFollowObjectPositionSystem);

			GKC_Utils.updateDirtyScene ("Update Follow Object Position elements", currentFollowObjectPositionSystem.gameObject);
		}


		deviceStringAction currentDeviceStringAction = newCustomCharacterObject.GetComponentInChildren<deviceStringAction> ();

		if (currentDeviceStringAction != null) {
			currentDeviceStringAction.setNewDeviceName (newGenericName);

			GKC_Utils.updateComponent (currentDeviceStringAction);
		}

		bodyMountPointsSystem mainBodyMountPointsSystem = newCustomCharacterObject.GetComponentInChildren<bodyMountPointsSystem> ();

		if (mainBodyMountPointsSystem != null) {
			mainBodyMountPointsSystem.setNewAnimator (genericModelAnimator);

			mainBodyMountPointsSystem.setCharacterBodyMountPointsInfoList ();

			GKC_Utils.updateComponent (mainBodyMountPointsSystem);
		}

		vehicleHUDManager currentVehicleHUDManager = newCustomCharacterObject.GetComponentInChildren<vehicleHUDManager> ();

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.setVehicleName (newGenericName);

			GKC_Utils.updateComponent (currentVehicleHUDManager);
		}

		DestroyImmediate (previousGenericModel);
			
		characterAdded = true;
	}

	void Update ()
	{
		if (characterAdded) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					timer = 0;

					this.Close ();
				}
			}
		}
	}
}
#endif