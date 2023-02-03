using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class AddNewActionSystemToCharacterCreatorEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (660, 600);

	bool actionAdded;

	float timeToBuild = 0.2f;
	float timer;

	string prefabsPath = "";

	playerActionSystem currentPlayerActionSystem;

	public string actionSystemCategoryName = "Others";

	public string actionSystemName;
	public float actionSystemDuration;
	public float actionSystemSpeed = 1;
	public bool useActionSystemID;
	public int actionSystemID;
	public string actionSystemAnimationName;

	public bool useCustomPrefabPath;
	public string customPrefabPath;

	public GameObject currentActionSystemPrefab;

	bool actionSystemAssigned;

	GUIStyle style = new GUIStyle ();

	GUIStyle labelStyle = new GUIStyle ();

	float windowHeightPercentage = 0.6f;

	Vector2 screenResolution;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	[MenuItem ("Game Kit Controller/Add Action System To Character", false, 25)]
	public static void addActionSystemToCharacter ()
	{
		GetWindow<AddNewActionSystemToCharacterCreatorEditor> ();
	}

	void OnEnable ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		//		Debug.Log (screenResolution + " " + partsHeight + " " + settingsHeight + " " + previewHeight);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		if (totalHeight < 600) {
			totalHeight = 600;
		}

		rectSize = new Vector2 (660, totalHeight);

		prefabsPath = pathInfoValues.getRegularActionSystemPrefabPath ();

		customPrefabPath = prefabsPath;

		resetCreatorValues ();

		checkCurrentObjectSelected (Selection.activeGameObject);
	}

	void checkCurrentObjectSelected (GameObject currentCharacterSelected)
	{
		if (currentCharacterSelected != null) {
			playerComponentsManager currentPlayerComponentsManager = currentCharacterSelected.GetComponentInChildren<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

				if (currentPlayerActionSystem != null) {
					actionSystemAssigned = true;

					currentActionSystemPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (prefabsPath, typeof(GameObject)) as GameObject;
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
		if (actionAdded) {

		} else {

		}

		currentPlayerActionSystem = null;

		actionAdded = false;

		actionSystemAssigned = false;

		actionSystemCategoryName = "Others";

		actionSystemName = "";
		actionSystemDuration = 0;
		actionSystemSpeed = 1;
		useActionSystemID = false;
		actionSystemID = 0;
		actionSystemAnimationName = "";

		useCustomPrefabPath = false;

		Debug.Log ("Action window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Actions", null, "Add New Action System To Character");

		GUILayout.BeginVertical ("Add New Action System To Character", "window");

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

		EditorGUILayout.LabelField ("Configure the settings of the new action and press the button 'Add New Action To Character'. \n\n" +
		"If not character is selected in the hierarchy, select one and press the button 'Check Current Object Selected'.\n\n" +
		"Once the action info is configured and created, remember to go to the character animator and configure the animation state on it, " +
		"along its transitions and conditions values.", style);
		
		GUILayout.EndHorizontal ();

		if (currentPlayerActionSystem == null) {

			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.HelpBox ("", MessageType.Warning);

			style = new GUIStyle (EditorStyles.helpBox);
			style.richText = true;

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 17;
			EditorGUILayout.LabelField ("WARNING: No Character was found, make sure to select the player or an " +
			"humanoid AI to add an action to it.", style);

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Current Object Selected")) {
				checkCurrentObjectSelected (Selection.activeGameObject);
			}
		} else {
			if (actionSystemAssigned) {

				scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

				GUILayout.FlexibleSpace ();

				EditorGUILayout.Space ();

				labelStyle.fontStyle = FontStyle.Bold;

				EditorGUILayout.LabelField ("ACTION SYSTEM INFO", labelStyle);

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Action Category Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				actionSystemCategoryName = (string)EditorGUILayout.TextField ("", actionSystemCategoryName);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Action Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				actionSystemName = (string)EditorGUILayout.TextField ("", actionSystemName);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Action Duration", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				actionSystemDuration = (float)EditorGUILayout.FloatField ("", actionSystemDuration);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Action Speed", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				actionSystemSpeed = (float)EditorGUILayout.FloatField ("", actionSystemSpeed);
				GUILayout.EndHorizontal ();
		
				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Use Action ID", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				useActionSystemID = (bool)EditorGUILayout.Toggle (useActionSystemID);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				if (useActionSystemID) {
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Action ID", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					actionSystemID = (int)EditorGUILayout.IntField (actionSystemID);
					GUILayout.EndHorizontal ();
				} else {
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Action Animation Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					actionSystemAnimationName = (string)EditorGUILayout.TextField (actionSystemAnimationName);
					GUILayout.EndHorizontal ();
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Action System Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				currentActionSystemPrefab = EditorGUILayout.ObjectField (currentActionSystemPrefab, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				EditorGUILayout.Space ();

				EditorGUILayout.Space ();

				showCustomPrefabPathOptions ();

				EditorGUILayout.EndScrollView ();

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add New Action To Character")) {
					addActionSystem ();
				}

				if (GUILayout.Button ("Cancel")) {
					this.Close ();
				}
			}
		}

		GUILayout.EndVertical ();
	}

	void showCustomPrefabPathOptions ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Custom Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCustomPrefabPath = (bool)EditorGUILayout.Toggle ("", useCustomPrefabPath);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (useCustomPrefabPath) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Custom Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customPrefabPath = EditorGUILayout.TextField ("", customPrefabPath);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Prefabs On New Path")) {
				prefabsPath = customPrefabPath;

				checkCurrentObjectSelected (Selection.activeGameObject);
			}
		}
	}

	void addActionSystem ()
	{
		if (currentActionSystemPrefab != null) {
			GameObject actionSystemObject = (GameObject)Instantiate (currentActionSystemPrefab, Vector3.zero, Quaternion.identity);
			actionSystemObject.name = actionSystemName;

			actionSystemObject.transform.SetParent (currentPlayerActionSystem.transform);
			actionSystemObject.transform.localPosition = Vector3.zero;
			actionSystemObject.transform.localRotation = Quaternion.identity;
	
			actionSystem currentActionSystem = actionSystemObject.GetComponent<actionSystem> ();

			if (currentActionSystem != null) {
				currentActionSystem.addNewActionFromEditor (actionSystemName, actionSystemDuration, actionSystemSpeed,
					useActionSystemID, actionSystemID, actionSystemAnimationName);

				if (currentPlayerActionSystem.addNewActionFromEditor (currentActionSystem, actionSystemCategoryName, actionSystemName, true, "")) {
					Debug.Log ("Action System created and adjusted to character properly");
				} else {
					Debug.Log ("Action System not created properly, check the player action system component on the character selected");
				}

				GKC_Utils.setActiveGameObjectInEditor (currentPlayerActionSystem.gameObject);
			}
		} else {
			Debug.Log ("WARNING: no prefab found on path " + prefabsPath);
		}

		actionAdded = true;
	}

	void Update ()
	{
		if (actionAdded) {
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