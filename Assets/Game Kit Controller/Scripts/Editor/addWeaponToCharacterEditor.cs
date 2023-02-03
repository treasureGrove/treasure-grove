using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class addWeaponToCharacterEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (600, 600);

	bool weaponAdded;

	float timeToBuild = 0.2f;
	float timer;

	string prefabsPath = "";

	public GameObject weaponGameObject;

	GUIStyle style = new GUIStyle ();

	GUIStyle labelStyle = new GUIStyle ();

	float windowHeightPercentage = 0.5f;

	Vector2 screenResolution;

	playerWeaponsManager currentPlayerWeaponsManager;

	Transform currentPlayerWeaponsParent;

	public string[] weaponList;
	public int weaponIndex;
	string newWeaponName;

	bool weaponListAssigned;

	public bool removeAttachmentSystemFromWeapon;
	public bool removeWeapon3dHudPanel;
	public bool weaponUsedOnAI;
	public float newWeaponDamage;
	public int newAmmoAmount;
	public float newFireRate;
	public bool useInfiniteAmmoAmount;

	public bool useCustomWeaponPrefabPath;
	public string customWeaponPrefabPath;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	[MenuItem ("Game Kit Controller/Add Weapon To Character/Add Fire Weapon To Character", false, 205)]
	public static void addWeaponToCharacter ()
	{
		GetWindow<addWeaponToCharacterEditor> ();
	}

	void OnEnable ()
	{
		weaponListAssigned = false;

		newWeaponName = "";

		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		//		Debug.Log (screenResolution + " " + partsHeight + " " + settingsHeight + " " + previewHeight);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		if (totalHeight < 500) {
			totalHeight = 500;
		}

		rectSize = new Vector2 (600, totalHeight);

		prefabsPath = pathInfoValues.getUsableWeaponsPrefabsPath () + "/";

		customWeaponPrefabPath = prefabsPath;

		resetCreatorValues ();

		checkCurrentWeaponSelected (Selection.activeGameObject);
	}

	void checkCurrentWeaponSelected (GameObject currentCharacterSelected)
	{
		if (currentCharacterSelected != null) {
			currentPlayerWeaponsManager = currentCharacterSelected.GetComponentInChildren<playerWeaponsManager> ();
		
			if (currentPlayerWeaponsManager != null) {
				currentPlayerWeaponsParent = currentPlayerWeaponsManager.getWeaponsParent ();

				if (!Directory.Exists (prefabsPath)) {
					Debug.Log ("WARNING: " + prefabsPath + " path doesn't exist, make sure the path is from an existing folder in the project");

					return;
				}

				string[] search_results = null;

				search_results = System.IO.Directory.GetFiles (prefabsPath, "*.prefab");

				if (search_results.Length > 0) {
					weaponList = new string[search_results.Length];
					int currentWeaponIndex = 0;
				
					foreach (string file in search_results) {
						//must convert file path to relative-to-unity path (and watch for '\' character between Win/Mac)
						GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (file, typeof(GameObject)) as GameObject;

						if (currentPrefab) {
							string currentWeaponName = currentPrefab.name;
							weaponList [currentWeaponIndex] = currentWeaponName;
							currentWeaponIndex++;
						} else {
							Debug.Log ("WARNING: something went wrong when trying to get the prefab in the path " + file);
						}
					}

					weaponListAssigned = true;
				} else {
					Debug.Log ("Weapon prefab not found in path " + prefabsPath);

					weaponList = new string[0];
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
		if (weaponAdded) {

		} else {
			
		}

		currentPlayerWeaponsManager = null;

		weaponAdded = false;

		weaponListAssigned = false;

		removeAttachmentSystemFromWeapon = false;

		removeWeapon3dHudPanel = false;

		weaponUsedOnAI = false;

		newWeaponDamage = 0;

		newAmmoAmount = 0;

		useInfiniteAmmoAmount = false;

		newFireRate = 0;

		useCustomWeaponPrefabPath = false;

		Debug.Log ("Weapon window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Weapons", null, "Add Weapon To Character");

		GUILayout.BeginVertical ("Add Fire Weapon To Character", "window");

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

		EditorGUILayout.LabelField ("Select a weapon from the 'Weapon To Add' list and press the button 'Add Weapon To Character'. \n\n" +
		"If not character is selected in the hierarchy, select one and press the button 'Check Current Object Selected'.\n\n", style);
		GUILayout.EndHorizontal ();

		if (currentPlayerWeaponsManager == null) {

			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.HelpBox ("", MessageType.Warning);

			style = new GUIStyle (EditorStyles.helpBox);
			style.richText = true;

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 17;
			EditorGUILayout.LabelField ("WARNING: No Character was found, make sure to select the player or an " +
			"humanoid AI to add a weapon to it.", style);

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Current Object Selected")) {
				checkCurrentWeaponSelected (Selection.activeGameObject);
			}
		} else {
			if (weaponListAssigned) {
				if (weaponList.Length > 0) {

					scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

					GUILayout.FlexibleSpace ();

					EditorGUILayout.Space ();
						
					if (weaponIndex < weaponList.Length) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Weapon To Add", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						weaponIndex = EditorGUILayout.Popup (weaponIndex, weaponList, GUILayout.ExpandWidth (true));
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						newWeaponName = weaponList [weaponIndex];  

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Remove Attachments", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						removeAttachmentSystemFromWeapon = (bool)EditorGUILayout.Toggle ("", removeAttachmentSystemFromWeapon);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Remove HUD 3d Panel", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						removeWeapon3dHudPanel = (bool)EditorGUILayout.Toggle ("", removeWeapon3dHudPanel);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Weapon Used On AI", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						weaponUsedOnAI = (bool)EditorGUILayout.Toggle (weaponUsedOnAI);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
						EditorGUILayout.Space ();

						labelStyle.fontStyle = FontStyle.Bold;

						EditorGUILayout.LabelField ("Weapon Stats", labelStyle);

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("New Weapon Damage", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						newWeaponDamage = (float)EditorGUILayout.FloatField (newWeaponDamage);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Infinite Ammo", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						useInfiniteAmmoAmount = (bool)EditorGUILayout.Toggle (useInfiniteAmmoAmount);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						if (!useInfiniteAmmoAmount) {
							GUILayout.BeginHorizontal ();
							GUILayout.Label ("New Ammo Amount", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							newAmmoAmount = (int)EditorGUILayout.IntField (newAmmoAmount);
							GUILayout.EndHorizontal ();

							EditorGUILayout.Space ();

						}

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("New Fire Rate", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						newFireRate = (float)EditorGUILayout.FloatField (newFireRate);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
					
					}

					EditorGUILayout.Space ();

					showCustomPrefabPathOptions ();

					EditorGUILayout.EndScrollView ();

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Add Weapon To Character")) {
						addWeapon ();
					}

					if (GUILayout.Button ("Cancel")) {
						this.Close ();
					}
				}
			} else {
				GUILayout.FlexibleSpace ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("", MessageType.Warning);

				style = new GUIStyle (EditorStyles.helpBox);
				style.richText = true;

				style.fontStyle = FontStyle.Bold;
				style.fontSize = 17;
				EditorGUILayout.LabelField ("WARNING: No weapons prefabs where found on the path " + prefabsPath, style);

				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				showCustomPrefabPathOptions ();
			}
		}

		GUILayout.EndVertical ();
	}

	void showCustomPrefabPathOptions ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Custom Weapon Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCustomWeaponPrefabPath = (bool)EditorGUILayout.Toggle ("", useCustomWeaponPrefabPath);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (useCustomWeaponPrefabPath) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Custom Weapon Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customWeaponPrefabPath = EditorGUILayout.TextField ("", customWeaponPrefabPath);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Prefabs On New Path")) {
				prefabsPath = customWeaponPrefabPath;

				checkCurrentWeaponSelected (Selection.activeGameObject);
			}
		}
	}

	void addWeapon ()
	{
		string pathForWeapon = prefabsPath + newWeaponName + ".prefab";

		GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (pathForWeapon, typeof(GameObject)) as GameObject;

		if (currentPrefab != null) {
			GameObject newWeaponCreated = (GameObject)Instantiate (currentPrefab, Vector3.zero, Quaternion.identity);
			newWeaponCreated.name = newWeaponName;

			newWeaponCreated.transform.SetParent (currentPlayerWeaponsParent);
			newWeaponCreated.transform.localPosition = Vector3.zero;
			newWeaponCreated.transform.localRotation = Quaternion.identity;

			weaponAdded = true;

			if (removeAttachmentSystemFromWeapon) {
				weaponAttachmentSystem currentWeaponAttachmentSystem = newWeaponCreated.GetComponentInChildren<weaponAttachmentSystem> ();

				if (currentWeaponAttachmentSystem != null) {
					DestroyImmediate (currentWeaponAttachmentSystem.gameObject);
				}
			}

			playerWeaponSystem currentPlayerWeaponSystem = newWeaponCreated.GetComponentInChildren<playerWeaponSystem> ();

			if (currentPlayerWeaponSystem != null) {
				if (removeWeapon3dHudPanel) {
				
					GameObject weaponHUDGameObject = currentPlayerWeaponSystem.getWeaponHUDGameObject ();

					if (weaponHUDGameObject != null) {
						DestroyImmediate (weaponHUDGameObject);
					}
				}

				if (newWeaponDamage > 0) {
					currentPlayerWeaponSystem.setProjectileDamage (newWeaponDamage);
				}
					
				currentPlayerWeaponSystem.setInfiniteAmmoValueFromEditor (useInfiniteAmmoAmount);

				if (newAmmoAmount > 0) {
					currentPlayerWeaponSystem.setRemainAmmoAmountFromEditor (newAmmoAmount);
				}

				if (newFireRate > 0) {
					currentPlayerWeaponSystem.setFireRateFromEditor (newFireRate);
				}
			}

			if (weaponUsedOnAI) {
				weaponBuilder currentWeaponBuilder = newWeaponCreated.GetComponent<weaponBuilder> ();

				if (currentWeaponBuilder != null) {
					currentWeaponBuilder.checkWeaponsPartsToRemoveOnAI ();
				}

				IKWeaponSystem currentIKWeaponSystem = newWeaponCreated.GetComponent<IKWeaponSystem> ();

				if (currentIKWeaponSystem != null) {
					currentIKWeaponSystem.setWeaponEnabledState (true);

					GameObject weaponMesh = currentIKWeaponSystem.getWeaponSystemManager ().weaponSettings.weaponMesh;

					if (weaponMesh != null) {
						currentPlayerWeaponsManager.setWeaponPartLayerFromCameraView (weaponMesh, false);
					}
				}
			}

			currentPlayerWeaponsManager.setWeaponList ();
		
		} else {
			Debug.Log ("WARNING: no prefab found on path " + prefabsPath + newWeaponName);
		}
	}

	void Update ()
	{
		if (weaponAdded) {
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