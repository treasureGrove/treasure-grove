using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class initialPopUpWindow : EditorWindow
{
	GUISkin guiSkin;

	Texture2D GKCLogo = null;
	Vector2 rectSize = new Vector2 (670, 690);

	GUIStyle titleStyle = new GUIStyle ();
	GUIStyle style = new GUIStyle ();

	Vector2 screenResolution;

	float windowHeightPercentage = 0.7f;

	float minHeight = 700f;

	Vector2 scrollPos1;

	public bool checkForPresetsActive;

	Rect windowRect = new Rect ();

	[MenuItem ("Game Kit Controller/Initial Pop Up", false, 601)]
	public static void AboutGKC ()
	{
		GetWindow<initialPopUpWindow> ();
	}

	string messageText = "IMPORTANT: \n\n" +

	                     " * TAGS AND LAYERS AND INPUT SETTINGS IMPORT\n\n" +
	                     "Make sure the TAG/LAYERS and INPUT have been imported properly. " +
	                     "You can find the TAGMANAGER and INPUTMANAGER presets on the folder: \n\n" +
	                     "Assets->Game Kit Controller->Presets \n\n" +
	                     "Press each one in order for these settings to be added automatically to the project." +
	                     "You can also import these presets with the button at the end of this window." +



	                     "\n\n\n" +


	                     " * CHARACTERS FALLING IN GROUND OR MISSING ANIMATIONS\n\n" +


	                     "You may notice that the player or AI is crossing " +
	                     "the ground or not playing some animations properly.\n\n" +

	                     "It is not a bug, make sure to import the animation " +
	                     "package from the public repository of the asset.\n\n" +

	                     "It is better explained on the doc, it is a group " +
	                     "of animations used on the action system" +
	                     " examples and the melee combat, just as placeholder, " +
	                     "so you can replace them at any moment, as any animation " +
	                     "will work properly.\n\n" +

	                     "Import the package, close and open unity " +
	                     "and voila, all configured by it self.\n\n" +

	                     "You can use the alternative link for the animations package if you prefer.\n\n\n" +

	                     "Remove the object on the scene called 'INITIAL POP UP WINDOW' to disable this message.";

	void OnEnable ()
	{
		GKCLogo = (Texture2D)Resources.Load ("Logo_reworked", typeof(Texture2D));

		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float windowHeight = screenResolution.y * windowHeightPercentage;

		windowHeight = Mathf.Clamp (windowHeight, minHeight, screenResolution.y);

		rectSize = new Vector2 (670, windowHeight);
	}

	public void Init ()
	{

	}

	void OnDisable ()
	{
		checkOnCloseWindow ();
	}

	void OnGUI ()
	{
		this.titleContent = new GUIContent ("GKC Initial Info");
		this.minSize = rectSize;

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label (GKCLogo, GUILayout.MaxHeight (100));
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		GUILayout.BeginVertical ("window");

		GUILayout.BeginHorizontal ("box");
		GUILayout.FlexibleSpace ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		titleStyle.normal.textColor = Color.white;
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.fontSize = 17;
		titleStyle.alignment = TextAnchor.MiddleCenter;
		GUILayout.Label ("Game Kit Controller Initial Info", titleStyle);

		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		style = new GUIStyle (EditorStyles.helpBox);
		style.richText = true;

		style.fontSize = 17;
		style.fontStyle = FontStyle.Bold;
		GUILayout.BeginHorizontal ();

		scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

		//EditorGUILayout.HelpBox ("", MessageType.Info);

		EditorGUILayout.LabelField (messageText, style);

		EditorGUILayout.EndScrollView ();

		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Public Repository")) {
			Application.OpenURL ("https://github.com/sr3888/GKC-Public-Repository");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Alternative Package Google Drive Link")) {
			Application.OpenURL ("https://drive.google.com/file/d/1iABx_lIPx5-JXxED5zs9G3VoJ_6132xB/view?usp=share_link");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Tutorial To Use Generic Models")) {
			Application.OpenURL ("https://www.youtube.com/watch?v=XABt9LvzRaY");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Apply InputManager & TagManager Presets Project Settings")) {
			applyPresetSystem.GKCapplyProjectSettings ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Close")) {
			checkOnCloseWindow ();
			this.Close ();
		}

		GUILayout.EndVertical ();
	}

	void checkOnCloseWindow ()
	{
		openInitialPopUpWindow mainOpenInitialPopUpWindow = FindObjectOfType<openInitialPopUpWindow> ();

		if (mainOpenInitialPopUpWindow != null) {
			mainOpenInitialPopUpWindow.setShowInitialPopWindowEnabledState (false);

			GKC_Utils.updateComponent (mainOpenInitialPopUpWindow);

			GKC_Utils.updateDirtyScene ("Update Initial Pop Up", mainOpenInitialPopUpWindow.gameObject);
		}

		mainManagerAdministrator mainMainManagerAdministrator = FindObjectOfType<mainManagerAdministrator> ();

		if (mainMainManagerAdministrator != null) {
			mainMainManagerAdministrator.setSpawnManagerOnAwakeDisableddByName ("INITIAL POP UP WINDOW");
		}

		if (checkForPresetsActive) {
			applyPresetSystem.GKCapplyProjectSettings ();

			checkForPresetsActive = false;
		}
	}
}
#endif