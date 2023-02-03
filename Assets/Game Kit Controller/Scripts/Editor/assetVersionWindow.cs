using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class assetVersionWindow : EditorWindow
{
	GUISkin guiSkin;

	Texture2D GKCLogo = null;
	Vector2 rect = new Vector2 (450, 640);

	GUIStyle style = new GUIStyle ();
	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		GKCLogo = (Texture2D)Resources.Load ("Logo_reworked", typeof(Texture2D));
	}

	[MenuItem ("Game Kit Controller/About GKC", false, 602)]
	public static void AboutGKC ()
	{
		GetWindow<assetVersionWindow> ();
	}

	void OnGUI ()
	{
		this.titleContent = new GUIContent ("About GKC");
		this.minSize = rect;

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label (GKCLogo, GUILayout.MaxHeight (170));
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		GUILayout.BeginVertical ("window");

		GUILayout.BeginVertical ("box");
		GUILayout.FlexibleSpace ();

		style.normal.textColor = Color.white;
		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;

		GUILayout.Label ("Game Kit Controller\n", style);
	
		style.fontSize = 15;

		GUILayout.Label ("Version: 3.4", style);

		GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 14;


		if (GUILayout.Button ("Online Gitbook Docs", buttonStyle)) {
			Application.OpenURL ("https://game-kit-controller.gitbook.io/docs/");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Open Tutorial Videos", buttonStyle)) {
			Application.OpenURL ("https://youtu.be/lZB_5b4tUm0?list=PLYVCbGEtbhxVjZ9C41fwTDynTpVkCP9iA");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Go to the Forum", buttonStyle)) {
			Application.OpenURL ("https://forum.unity.com/threads/released-game-kit-controller-engine-with-weapons-vehicles-more-2-9-local-multiplayer.351456/");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Join Discord", buttonStyle)) {
			Application.OpenURL ("https://discord.gg/kUpeRZ8https://discord.gg/kUpeRZ8");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Public Repository", buttonStyle)) {
			Application.OpenURL ("https://github.com/sr3888/GKC-Public-Repository");
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Review Asset", buttonStyle)) {
			Application.OpenURL ("https://assetstore.unity.com/packages/templates/systems/game-kit-controller-40995#reviews");
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Close", buttonStyle)) {
			this.Close ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox ("IMPORTANT: If you update your GKC version, you can delete the previous GKC folder " +
			"or overwrite it. Make sure you have a backup of your project before.", MessageType.Info);

		GUILayout.EndVertical ();
	}
}
#endif