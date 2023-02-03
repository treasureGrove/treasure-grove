using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class weaponCreatorEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();

	public GameObject currentWeapon;

	bool weaponCreated;

	Vector2 scrollPos1;
	Vector2 scrollPos2;
	Vector2 scrollPos3;
	Vector2 scrollPos4;

	float timeToBuild = 0.2f;
	float timer;
	string assetsPath;

	string prefabsPath = "";

	string currentWeaponName = "New Weapon";

	bool buttonPressed;

	public GameObject weaponGameObject;

	public weaponBuilder currentWeaponBuilder;

	weaponBuilder.partInfo currentWeaponPartInfo;

	weaponBuilder.settingsInfo currentWeaponSettingsInfo;

	public bool useCustomAmmoMesh;
	public GameObject customAmmoMesh;
	public Texture customAmmoIcon;
	public string customAmmoDescription;


	bool showGizmo = true;
	bool useHandle = true;

	bool mainUseWeaponPartState;

	GUIStyle style = new GUIStyle ();

	float windowHeightPercentage = 0.7f;

	Vector2 rectSize = new Vector2 (590, 650);

	float minHeight = 650f;

	Vector2 screenResolution;

	bool selectingWeapon;

	bool weaponRemovedFromCreator;

	playerWeaponsManager currentPlayerWeaponsManager;

	public string[] weaponList;
	public int weaponIndex;
	string newWeaponName;

	GameObject currentPlayer;

	bool weaponSelectedAtStart;

	Vector2 previousRectSize;

	[Range (0.01f, 5)] float newWeaponScale = 1;

	public Texture weaponIconTexture;

	public Texture weaponInventorySlotIcon;

	public string weaponInventoryDescription;

	bool showInventoryObjectNameAlreadyExistsMessage;

	float maxLayoutWidht = 200;

	[MenuItem ("Game Kit Controller/Create New Weapon/Create New Fire Weapon", false, 201)]
	public static void createNewWeapon ()
	{
		GetWindow<weaponCreatorEditor> ();
	}

	void OnEnable ()
	{
		selectingWeapon = false;

		newWeaponName = "";

		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float windowHeight = screenResolution.y * windowHeightPercentage;

		windowHeight = Mathf.Clamp (windowHeight, minHeight, screenResolution.y);

		rectSize = new Vector2 (590, windowHeight);

		resetCreatorValues ();

		currentWeapon = Selection.activeGameObject;

		if (currentWeapon != null) {
			if (currentWeapon.GetComponent<weaponBuilder> ()) {
				weaponSelectedAtStart = true;
			} else {
				weaponSelectedAtStart = true;
			}
		}

		checkCurrentWeaponSelected ();
	}

	void checkCurrentWeaponSelected ()
	{
		currentPlayer = GameObject.FindWithTag ("Player");

		bool weaponFound = false;

		if (currentWeapon != null) {
			currentWeaponBuilder = currentWeapon.GetComponent<weaponBuilder> ();

			if (currentWeaponBuilder != null) {
				GameObject newWeaponObject = (GameObject)Instantiate (currentWeapon, Vector3.zero, Quaternion.identity);
				Transform newWeaponObjectTransform = newWeaponObject.transform;

				currentWeapon = newWeaponObject;
		
				newWeaponObjectTransform.SetParent (currentWeaponBuilder.weaponParent);
				newWeaponObjectTransform.localPosition = Vector3.zero;
				newWeaponObjectTransform.localRotation = Quaternion.identity;

				currentWeaponBuilder = currentWeapon.GetComponent<weaponBuilder> ();

				currentWeaponBuilder.weaponMeshParent.transform.position = Vector3.up * 1000;

				currentWeaponBuilder.weaponViewTransform.SetParent (currentWeaponBuilder.transform);

				currentWeaponBuilder.weaponMeshParent.transform.rotation = Quaternion.identity;
				currentWeaponBuilder.weaponMeshParent.transform.localScale = Vector3.one * 10;

				currentWeaponBuilder.alignViewWithWeaponCameraPosition ();

				currentWeaponName = "New Weapon";

				currentWeapon.name = currentWeaponName;

				setExpandOrCollapsePartElementsListState (false);

				setExpandOrCollapseSettingsElementsListState (false);

				weaponFound = true;

				GKC_Utils.setActiveGameObjectInEditor (currentWeapon);
			}
		}

		if (!weaponFound) {
			currentWeapon = null;

			if (currentPlayer != null) {
				currentPlayerWeaponsManager = currentPlayer.GetComponent<playerWeaponsManager> ();

				if (currentPlayerWeaponsManager != null) {
					if (newWeaponName.Equals ("")) {

						if (currentPlayerWeaponsManager.weaponsList.Count > 0) {
							int weaponListCount = 0;

							for (int i = 0; i < currentPlayerWeaponsManager.weaponsList.Count; i++) {
								IKWeaponSystem currentIKWeaponSystem = currentPlayerWeaponsManager.weaponsList [i];

								if (currentIKWeaponSystem != null) {
									weaponListCount++;
								}
							}

							weaponList = new string[weaponListCount];

							int currentWeaponIndex = 0;

							for (int i = 0; i < currentPlayerWeaponsManager.weaponsList.Count; i++) {
								IKWeaponSystem currentIKWeaponSystem = currentPlayerWeaponsManager.weaponsList [i];

								if (currentIKWeaponSystem != null) {
									string name = currentIKWeaponSystem.getWeaponSystemName ();
									weaponList [currentWeaponIndex] = name;
									currentWeaponIndex++;
								}
							}
						} else {
							weaponList = new string[0];
						}

						weaponIndex = 0;

						selectingWeapon = true;
					} else {
						selectingWeapon = false;

						currentWeapon = currentPlayerWeaponsManager.getIKWeaponSystem (newWeaponName).gameObject;

						checkCurrentWeaponSelected ();
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
		if (weaponCreated) {

		} else {
			if (currentWeaponBuilder != null) {
				DestroyImmediate (currentWeaponBuilder.gameObject);

				Debug.Log ("destroy instantiated weapon");
			}
		}

		currentWeapon = null;

		if (currentWeaponBuilder != null) {
			currentWeaponBuilder.removeTemporalWeaponParts ();

			currentWeaponBuilder.showGizmo = false;
			currentWeaponBuilder.useHandle = false;
		}

		currentWeaponBuilder = null;

		weaponCreated = false;

		mainUseWeaponPartState = false;

		weaponRemovedFromCreator = false;

		newWeaponName = "";

		newWeaponScale = 1;

		weaponSelectedAtStart = false;

		showInventoryObjectNameAlreadyExistsMessage = false;

		useCustomAmmoMesh = false;

		Debug.Log ("Creator window closed");
	}

	void OnGUI ()
	{
		if (guiSkin == null) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}

		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Weapon", null, "Game Kit Controller Weapon Creator");

		scrollPos3 = EditorGUILayout.BeginScrollView (scrollPos3, false, false);

		GUILayout.BeginVertical (GUILayout.Width (580));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Creator Window", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		GUILayout.BeginVertical ("box");

		scrollPos4 = EditorGUILayout.BeginScrollView (scrollPos4, false, false);

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Weapon Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		currentWeaponName = (string)EditorGUILayout.TextField (currentWeaponName, GUILayout.ExpandWidth (true)); 
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Current Weapon", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		currentWeapon = EditorGUILayout.ObjectField (currentWeapon, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Weapon Icon", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		weaponIconTexture = EditorGUILayout.ObjectField (weaponIconTexture, typeof(Texture), true, GUILayout.ExpandWidth (true)) as Texture;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Quick Access Slot Icon", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		weaponInventorySlotIcon = EditorGUILayout.ObjectField (weaponInventorySlotIcon, typeof(Texture), true, GUILayout.ExpandWidth (true)) as Texture;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Weapon Description: ", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		weaponInventoryDescription = EditorGUILayout.TextArea (weaponInventoryDescription, EditorStyles.textArea, GUILayout.Height (40), GUILayout.ExpandWidth (true));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Custom Ammo Mesh", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCustomAmmoMesh = (bool)EditorGUILayout.Toggle (useCustomAmmoMesh, GUILayout.ExpandWidth (true));
		GUILayout.EndHorizontal ();

		if (useCustomAmmoMesh) {
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Custom Ammo Mesh", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customAmmoMesh = EditorGUILayout.ObjectField (customAmmoMesh, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Custom Ammo Icon", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customAmmoIcon = EditorGUILayout.ObjectField (customAmmoIcon, typeof(Texture), true, GUILayout.ExpandWidth (true)) as Texture;
			GUILayout.EndHorizontal ();
		
			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Ammo Description: ", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customAmmoDescription = EditorGUILayout.TextArea (customAmmoDescription, EditorStyles.textArea, GUILayout.Height (40), GUILayout.ExpandWidth (true));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.EndScrollView ();

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

		if (currentWeapon != null && currentWeaponBuilder == null) {
			if (weaponRemovedFromCreator || !weaponSelectedAtStart) {

				weaponRemovedFromCreator = false;

				checkCurrentWeaponSelected ();

				Debug.Log ("Weapon reassigned on creator");
				
				setExpandOrCollapsePartElementsListState (false);

				GKC_Utils.setActiveGameObjectInEditor (currentWeapon);
			}
		}

		if (currentWeaponBuilder == null) {
			GUILayout.FlexibleSpace ();
		}

		GUILayout.EndVertical ();

		if (currentWeaponBuilder != null) {

			EditorGUILayout.Space ();

			style.normal.textColor = Color.white;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 18;

			GUILayout.Label ("Weapon Settings Info List", style);

			GUILayout.BeginVertical ("", "window");

//			GUILayout.FlexibleSpace ();

			scrollPos2 = EditorGUILayout.BeginScrollView (scrollPos2, false, false);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Replace Arms", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentWeaponBuilder.replaceArmsModel = (bool)EditorGUILayout.Toggle ("", currentWeaponBuilder.replaceArmsModel, GUILayout.ExpandWidth (true));
			GUILayout.EndHorizontal ();

			if (currentWeaponBuilder.replaceArmsModel) {
				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("New Arms", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				currentWeaponBuilder.newArmsModel = EditorGUILayout.ObjectField (currentWeaponBuilder.newArmsModel, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
				
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Weapon Scale", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			newWeaponScale = EditorGUILayout.Slider (newWeaponScale, 0.01f, 5, GUILayout.ExpandWidth (true));
			GUILayout.EndHorizontal ();

			currentWeaponBuilder.newWeaponMeshParent.localScale = Vector3.one * newWeaponScale;

			EditorGUILayout.Space ();

			for (int i = 0; i < currentWeaponBuilder.weaponSettingsInfoList.Count; i++) { 

				currentWeaponSettingsInfo = currentWeaponBuilder.weaponSettingsInfoList [i];

				GUILayout.BeginHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (currentWeaponSettingsInfo.Name, EditorStyles.boldLabel, GUILayout.MaxWidth (250));
				if (currentWeaponSettingsInfo.useBoolState) {
					currentWeaponSettingsInfo.boolState = (bool)EditorGUILayout.Toggle ("", currentWeaponSettingsInfo.boolState, GUILayout.ExpandWidth (true));
				}

				if (currentWeaponSettingsInfo.useFloatValue) {
					currentWeaponSettingsInfo.floatValue = (float)EditorGUILayout.FloatField ("", currentWeaponSettingsInfo.floatValue, GUILayout.ExpandWidth (true));
				}
				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}   

			EditorGUILayout.EndScrollView ();

			GUILayout.EndVertical ();


			style.normal.textColor = Color.white;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 18;

			GUILayout.Label ("Weapon Parts Info List", style);


			GUILayout.BeginVertical ("", "window");

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Show Handle", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			useHandle = (bool)EditorGUILayout.Toggle (useHandle);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

			for (int i = 0; i < currentWeaponBuilder.weaponPartInfoList.Count; i++) { 

				currentWeaponPartInfo = currentWeaponBuilder.weaponPartInfoList [i];

				currentWeaponPartInfo.expandElement = EditorGUILayout.Foldout (currentWeaponPartInfo.expandElement, currentWeaponPartInfo.Name);

				if (currentWeaponPartInfo.expandElement) {
					GUILayout.BeginVertical ("label");

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("New Weapon Mesh", EditorStyles.boldLabel, GUILayout.MaxWidth (150));
					currentWeaponPartInfo.newWeaponMesh = 
						EditorGUILayout.ObjectField (currentWeaponPartInfo.newWeaponMesh, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
					
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					if (currentWeaponPartInfo.newWeaponMesh == null) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Remove Weapon Part If No New Mesh Added", EditorStyles.boldLabel);
						currentWeaponPartInfo.removeWeaponPartIfNoNewMesh = (bool)EditorGUILayout.Toggle ("", currentWeaponPartInfo.removeWeaponPartIfNoNewMesh, GUILayout.MaxWidth (50));
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
					} else {
						currentWeaponPartInfo.removeWeaponPartIfNoNewMesh = false;

						if (currentWeaponPartInfo.temporalWeaponMesh != null) {
							if (!currentWeaponBuilder.useHandle) {
								currentWeaponPartInfo.newWeaponMeshPositionOffset = (Vector3)EditorGUILayout.Vector3Field ("Position Offset", currentWeaponPartInfo.newWeaponMeshPositionOffset);
								currentWeaponPartInfo.newWeaponMeshEulerOffset = (Vector3)EditorGUILayout.Vector3Field ("Euler Offset", currentWeaponPartInfo.newWeaponMeshEulerOffset);
							}

							EditorGUILayout.Space ();

							if (GUILayout.Button ("O")) {
								GKC_Utils.setActiveGameObjectInEditor (currentWeaponPartInfo.temporalWeaponMesh);
							}
						}
					}

					GUILayout.EndVertical ();
				}

				if (!currentWeaponPartInfo.removeWeaponPartIfNoNewMesh) {
					if (currentWeaponPartInfo.newWeaponMesh != null) {
						if (!currentWeaponPartInfo.temporalWeaponMeshInstantiated) {
							currentWeaponPartInfo.temporalWeaponMeshInstantiated = true;

							if (currentWeaponPartInfo.currentWeaponMesh != null) {
								currentWeaponPartInfo.currentWeaponMesh.SetActive (false);
							}

							if (currentWeaponPartInfo.extraWeaponPartMeshesList.Count > 0) {
								for (int j = 0; j < currentWeaponPartInfo.extraWeaponPartMeshesList.Count; j++) { 
									if (currentWeaponPartInfo.extraWeaponPartMeshesList [j] != null) {
										currentWeaponPartInfo.extraWeaponPartMeshesList [j].SetActive (false);
									}
								}
							}
						}

						if (currentWeaponPartInfo.newWeaponMesh != currentWeaponPartInfo.temporalNewWeaponMesh) {

							if (currentWeaponPartInfo.temporalWeaponMesh != null) {
								DestroyImmediate (currentWeaponPartInfo.temporalWeaponMesh);
							}

							currentWeaponPartInfo.temporalNewWeaponMesh = currentWeaponPartInfo.newWeaponMesh;

							GameObject newWeaponPart = (GameObject)Instantiate (currentWeaponPartInfo.newWeaponMesh, Vector3.zero, Quaternion.identity);

							Transform newWeaponPartTransform = newWeaponPart.transform;

							newWeaponPartTransform.SetParent (currentWeaponPartInfo.weaponMeshParent);
							newWeaponPartTransform.localPosition = Vector3.zero;
							newWeaponPartTransform.localRotation = Quaternion.identity;
							newWeaponPartTransform.localScale = Vector3.one;

							currentWeaponPartInfo.temporalWeaponMesh = newWeaponPart;
						}
					} else {
						if (currentWeaponPartInfo.currentWeaponMesh != null && !currentWeaponPartInfo.currentWeaponMesh.activeSelf) {
							currentWeaponPartInfo.currentWeaponMesh.SetActive (true);
						}

						if (currentWeaponPartInfo.extraWeaponPartMeshesList.Count > 0) {
							for (int j = 0; j < currentWeaponPartInfo.extraWeaponPartMeshesList.Count; j++) { 
								if (currentWeaponPartInfo.extraWeaponPartMeshesList [j] != null && !currentWeaponPartInfo.extraWeaponPartMeshesList [j].activeSelf) {
									currentWeaponPartInfo.extraWeaponPartMeshesList [j].SetActive (true);
								}
							}
						}

						if (currentWeaponPartInfo.temporalWeaponMeshInstantiated) {

							if (currentWeaponPartInfo.temporalWeaponMesh != null) {
								DestroyImmediate (currentWeaponPartInfo.temporalWeaponMesh);
							}

							currentWeaponPartInfo.temporalWeaponMeshInstantiated = false;
						}
					}

					if (currentWeaponPartInfo.temporalWeaponMesh != null) {
						if (!currentWeaponBuilder.useHandle) {
							currentWeaponPartInfo.temporalWeaponMesh.transform.localPosition = Vector3.zero + currentWeaponPartInfo.newWeaponMeshPositionOffset;
							currentWeaponPartInfo.temporalWeaponMesh.transform.localEulerAngles = Vector3.zero + currentWeaponPartInfo.newWeaponMeshEulerOffset;
						}
					}
				} else {
					if (currentWeaponPartInfo.currentWeaponMesh != null && currentWeaponPartInfo.currentWeaponMesh.activeSelf) {
						currentWeaponPartInfo.currentWeaponMesh.SetActive (false);
					}

					if (currentWeaponPartInfo.extraWeaponPartMeshesList.Count > 0) {
						for (int j = 0; j < currentWeaponPartInfo.extraWeaponPartMeshesList.Count; j++) { 
							if (currentWeaponPartInfo.extraWeaponPartMeshesList [j] != null && currentWeaponPartInfo.extraWeaponPartMeshesList [j].activeSelf) {
								currentWeaponPartInfo.extraWeaponPartMeshesList [j].SetActive (false);
							}
						}
					}
				}

				EditorGUILayout.Space ();
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
				setUseWeaponPartsListState (!mainUseWeaponPartState);
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();

			currentWeaponBuilder.showGizmo = showGizmo;
			currentWeaponBuilder.useHandle = useHandle;

			if (showInventoryObjectNameAlreadyExistsMessage) {

				EditorGUILayout.Space ();

				string extraInstructions = "WARNING: The weapon name used for the inventory is already in use. Make sure to use an unique name for it.";
	
				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("", MessageType.Warning);

				style = new GUIStyle (EditorStyles.helpBox);
				style.richText = true;

				style.fontStyle = FontStyle.Bold;
				style.fontSize = 17;
				EditorGUILayout.LabelField ("WARNING: " + extraInstructions, style);

				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
				
			if (currentWeapon == null) {
				resetCreatorValues ();

				weaponRemovedFromCreator = true;

				Debug.Log ("Weapon removed from creator");

				selectingWeapon = true;

				checkCurrentWeaponSelected ();
			}

		} else {
			if (selectingWeapon) {
				if (weaponList.Length > 0) {
					if (weaponIndex < weaponList.Length) {
						weaponIndex = EditorGUILayout.Popup ("Weapon To Build", weaponIndex, weaponList);

						newWeaponName = weaponList [weaponIndex];  
					}

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Select Weapon Type")) {
						currentWeapon = currentPlayer;

						checkCurrentWeaponSelected ();

						return;
					}

					if (GUILayout.Button ("Cancel")) {
						this.Close ();
					}
				} else {
					GUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("", MessageType.Info);

					style = new GUIStyle (EditorStyles.helpBox);
					style.richText = true;

					style.fontStyle = FontStyle.Bold;
					style.fontSize = 16;

					EditorGUILayout.LabelField ("There are not weapons configured on the current player on the scene. " +
					"Go to Assets -> Game Kit Controller -> Prefabs -> Weapons -> Usable Weapons and drop the weapon template you want to use on the scene.\n" +
					"Then, select that weapon on the hierarchy and close and open the weapon creator wizard again or assign it on the Current Weapon field to use that weapon as template.\n" +
					"Finally, after creating the weapon, follow the steps of the tutorial video to add a new weapon to any character, selecting the new weapon created for it.", style);
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					EditorGUILayout.Space ();
				}
			}
			GUILayout.BeginHorizontal ();

			EditorGUILayout.HelpBox ("", MessageType.Info);

			style = new GUIStyle (EditorStyles.helpBox);
			style.richText = true;

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 17;

			EditorGUILayout.LabelField ("Select a weapon from the 'Weapon To Build' list and press the button Select Weapon Type. \n" +
			"Or, drop a weapon on 'Current Weapon' field from one of the weapons located in player's body, to build one of similar characteristics.\n\n" +
			"Once that is done, drop the Weapon mesh prefab in the scene and assign its parts into the fields you need.\n\n" +
			"If the 'Weapon To Build' list is empty, add first a weapon to the character using the 'Add Fire Weapon To Character' wizard, " +
			"and add the weapon type that is closest to the configuration that you want to use for the new weapon. \n\n" +
			"Then, open this wizard again and select the new type configured, and customize the new weapon as you need.", style);
			GUILayout.EndHorizontal ();

			if (currentWeapon != null && currentWeaponBuilder == null) {

				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("", MessageType.Warning);

				style = new GUIStyle (EditorStyles.helpBox);
				style.richText = true;

				style.fontStyle = FontStyle.Bold;
				style.fontSize = 17;
				EditorGUILayout.LabelField ("WARNING: The object placed in the field 'Current Weapon' is not a Weapon." +
				" Please, make sure to assign a GKC Weapon prefab on that field.", style);

				GUILayout.EndHorizontal ();
			}
		}

		if (currentWeapon != null && currentWeaponBuilder != null) {
			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Cancel")) {
				this.Close ();
			}

			if (GUILayout.Button ("Align View")) {
				if (currentWeaponBuilder != null) {
					currentWeaponBuilder.alignViewWithWeaponCameraPosition ();

					GKC_Utils.setActiveGameObjectInEditor (currentWeapon);
				}
			}

			if (GUILayout.Button ("Reset Weapon")) {
				if (currentWeaponBuilder != null) {
					currentWeaponBuilder.removeTemporalWeaponParts ();
				}
			}

			if (GUILayout.Button ("Create Weapon")) {
				createWeapon ();
			}

			GUILayout.EndHorizontal ();
		}

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.EndScrollView ();
	}

	void createWeapon ()
	{
		showInventoryObjectNameAlreadyExistsMessage = false;

		if (GKC_Utils.checkIfInventoryObjectNameExits (currentWeaponName)) {

			Debug.Log ("Weapon name already exists");

			showInventoryObjectNameAlreadyExistsMessage = true;

			rectSize.y += 70;

			return;
		}

		prefabsPath = pathInfoValues.getUsableWeaponsPrefabsPath () + "/";

		string prefabPath = prefabsPath + currentWeaponName;

		prefabPath += ".prefab";

		bool instantiateGameObject = false;

		if (currentWeapon != null) {
			weaponGameObject = currentWeapon;
		} else {
			instantiateGameObject = true;

			weaponGameObject = (GameObject)AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));
		}

		if (weaponGameObject != null) {
			if (instantiateGameObject) {

			} else {
				currentWeaponBuilder.weaponMeshParent.transform.localScale = Vector3.one;

				currentWeaponBuilder.weaponMeshParent.transform.localRotation = Quaternion.identity;

				currentWeaponBuilder.weaponViewTransform.SetParent (currentWeaponBuilder.weaponMeshParent.transform);

				currentWeaponBuilder.weaponMeshParent.transform.localPosition = Vector3.zero;

				currentWeaponBuilder.setNewWeaponIconTexture (weaponIconTexture);

				currentWeaponBuilder.setNewWeaponDescription (weaponInventoryDescription);

				currentWeaponBuilder.setWeaponInventorySlotIconInEditor (weaponInventorySlotIcon);

				if (useCustomAmmoMesh) {
					currentWeaponBuilder.setCustomAmmoMeshInfo (customAmmoMesh, customAmmoIcon, customAmmoDescription);
				}
			}

			currentWeaponBuilder.setNewWeaponName (currentWeaponName);

			weaponCreated = true;
		} else {
			Debug.Log ("Weapon prefab not found in path " + prefabPath);
		}
	}

	void Update ()
	{
		if (weaponCreated) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					currentWeaponBuilder.buildWeapon ();

					timer = 0;

					this.Close ();
				}
			}
		}
	}

	public void setExpandOrCollapsePartElementsListState (bool state)
	{
		if (currentWeaponBuilder != null) {
			for (int i = 0; i < currentWeaponBuilder.weaponPartInfoList.Count; i++) {
				currentWeaponPartInfo = currentWeaponBuilder.weaponPartInfoList [i];
				currentWeaponPartInfo.expandElement = state;
			}
		}
	}

	public void setUseWeaponPartsListState (bool state)
	{
		if (currentWeaponBuilder != null) {
			mainUseWeaponPartState = state;

			for (int i = 0; i < currentWeaponBuilder.weaponPartInfoList.Count; i++) {
				currentWeaponPartInfo = currentWeaponBuilder.weaponPartInfoList [i];
				currentWeaponPartInfo.removeWeaponPartIfNoNewMesh = mainUseWeaponPartState;
			}
		}
	}

	public void setExpandOrCollapseSettingsElementsListState (bool state)
	{
		if (currentWeaponBuilder != null) {
			for (int i = 0; i < currentWeaponBuilder.weaponSettingsInfoList.Count; i++) {
				currentWeaponSettingsInfo = currentWeaponBuilder.weaponSettingsInfoList [i];
				currentWeaponSettingsInfo.expandElement = state;
			}
		}
	}
}
#endif