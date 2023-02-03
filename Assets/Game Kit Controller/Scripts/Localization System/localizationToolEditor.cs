using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GKC.Localization
{
	#if UNITY_EDITOR
	using UnityEditor;

	public class searcherLocalizationToolEditor: EditorWindow
	{
		public static void Open (languageLocalizationManager newLanguageLocalizationManager)
		{
			var window = (searcherLocalizationToolEditor)ScriptableObject.CreateInstance (typeof(searcherLocalizationToolEditor));

			window.titleContent = new GUIContent ("Search Language Keys Window");

			Vector2 mouse = GUIUtility.GUIToScreenPoint (Event.current.mousePosition);
			Rect r = new Rect (mouse.x - 350, mouse.y + 10, 10, 10);

			window.ShowAsDropDown (r, new Vector2 (1000, 400));

			window.currentLanguageLocalizationManager = newLanguageLocalizationManager;
		}

		public string value;
		public Vector2 scroll;
		public Dictionary<string, string> dictionary;

		public languageLocalizationManager currentLanguageLocalizationManager;

		public bool dictionaryAssigned = false;


		void OnDisabled ()
		{
			dictionaryAssigned = false;
		}

		public void OnGUI ()
		{
			EditorGUILayout.BeginHorizontal ("Box");
			EditorGUILayout.LabelField ("Search: ", EditorStyles.boldLabel);
			value = EditorGUILayout.TextField (value);
			EditorGUILayout.EndHorizontal ();

			getSearchResults ();
		}

		private void getSearchResults ()
		{
			if (value == null) {
				value = "";
			}

			if (!dictionaryAssigned) {
				dictionary = currentLanguageLocalizationManager.getDictionaryForEditor ();

				if (dictionary != null) {
					dictionaryAssigned = true;
				} else {
					Debug.Log ("Language " + currentLanguageLocalizationManager.currentLanguageToEdit + " not found, " +
					"close the window and write a proper language name");
				}
			}

			if (dictionaryAssigned) {
				EditorGUILayout.BeginVertical ();

				scroll = EditorGUILayout.BeginScrollView (scroll);

				foreach (KeyValuePair<string, string> element in dictionary) {
					if (element.Key.ToLower ().Contains (value.ToLower ()) || element.Value.ToLower ().Contains (value.ToLower ())) {
						EditorGUILayout.BeginHorizontal ("box");

						EditorGUILayout.BeginVertical (GUILayout.MaxWidth (30));
						Texture closeIcon = (Texture)Resources.Load ("Delete Icon");

						GUIContent content = new GUIContent (closeIcon, "Remove Key");

						if (GUILayout.Button (content, GUILayout.MaxWidth (20), GUILayout.MaxHeight (20))) {

							if (EditorUtility.DisplayDialog ("Remove Key " + element.Key + "?", "This will remove the element from localization, are you sure?", "Do it")) {
								currentLanguageLocalizationManager.removeKey (element.Key);

								GKC_Utils.refreshAssetDatabase ();

								currentLanguageLocalizationManager.updateLocalizationFileFromEditor ();

								dictionary = currentLanguageLocalizationManager.getDictionaryForEditor ();
							}
						}

						Texture addIcon = (Texture)Resources.Load ("Store Icon");

						GUIContent addContent = new GUIContent (addIcon, "Add Key");

						if (GUILayout.Button (addContent, GUILayout.MaxWidth (20), GUILayout.MaxHeight (20))) {
							addKeyLocalizationToolEditor.Open (currentLanguageLocalizationManager, element.Key, element.Value);

							this.Close ();
						}
						EditorGUILayout.EndVertical ();

						EditorGUILayout.TextField (element.Key, EditorStyles.textArea, GUILayout.MaxHeight (50), GUILayout.Width (250));

						EditorGUILayout.LabelField (element.Value, EditorStyles.textArea);

						EditorGUILayout.EndHorizontal ();
					}
				}

				EditorGUILayout.EndScrollView ();
				EditorGUILayout.EndVertical ();
			}
		}
	}

	public class addKeyLocalizationToolEditor : EditorWindow
	{
		public static void Open (languageLocalizationManager newLanguageLocalizationManager, string mainKey = "", string mainValue = "")
		{
			var window = (addKeyLocalizationToolEditor)ScriptableObject.CreateInstance (typeof(addKeyLocalizationToolEditor));

			window.titleContent = new GUIContent ("Add Language Key Window");
			window.ShowUtility ();

			window.currentLanguageLocalizationManager = newLanguageLocalizationManager;

			window.currentLanguageLocalizationManager.updateLocalizationFileFromEditor ();

			Vector2 mouse = GUIUtility.GUIToScreenPoint (Event.current.mousePosition);
			Rect r = new Rect (mouse.x - 350, mouse.y + 10, 10, 10);

			if (mainKey != "") {
				window.key = mainKey; 
			}

			if (mainValue != "") {
				window.value = mainValue;
			}

			window.ShowAsDropDown (r, new Vector2 (460, 180));
		}

		public string key;
		public string value;

		public languageLocalizationManager currentLanguageLocalizationManager;

		public void OnGUI ()
		{
			key = EditorGUILayout.TextField ("Key: ", key);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Value: ", GUILayout.MaxWidth (50));

			EditorStyles.textArea.wordWrap = true;

			value = EditorGUILayout.TextArea (value, EditorStyles.textArea, GUILayout.Height (100), GUILayout.Width (400));

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add")) {
				if (key != "" && value != "" && key != null && value != null) {
					string currentKey = currentLanguageLocalizationManager.GetLocalizedValueFromEditor (key);
				
					Debug.Log ("Key obtained:" + currentKey);
			
					currentLanguageLocalizationManager.addKey (key, value);

					Debug.Log ("Checking key: " + key.ToString () + " with value: " + value.ToString ());

					GKC_Utils.refreshAssetDatabase ();

					currentLanguageLocalizationManager.updateLocalizationFileFromEditor ();

					this.Close ();
				}
			}

			minSize = new Vector2 (460, 180);

			maxSize = minSize;
		}
	}

	public class addLanguageLocalizationToolEditor : EditorWindow
	{
		public static void Open (languageLocalizationManager newLanguageLocalizationManager)
		{
			var window = (addLanguageLocalizationToolEditor)ScriptableObject.CreateInstance (typeof(addLanguageLocalizationToolEditor));

			window.titleContent = new GUIContent ("Add Language Editor Window");
			window.ShowUtility ();

			window.currentLanguageLocalizationManager = newLanguageLocalizationManager;

			window.currentLanguageLocalizationManager.updateLocalizationFileFromEditor ();

			Vector2 mouse = GUIUtility.GUIToScreenPoint (Event.current.mousePosition);
			Rect r = new Rect (mouse.x - 350, mouse.y + 10, 10, 10);

			window.ShowAsDropDown (r, new Vector2 (400, 190));
		}

		public string key;

		public languageLocalizationManager currentLanguageLocalizationManager;

		public void OnGUI ()
		{
			key = EditorGUILayout.TextField ("Language: ", key);

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add")) {
				if (key != "" && key != null) {
					Debug.Log ("Checking to add Language key: " + key.ToString ());

					currentLanguageLocalizationManager.addLanguage (key);

					GKC_Utils.refreshAssetDatabase ();

					currentLanguageLocalizationManager.updateLocalizationFileFromEditor ();
				}
			}

			minSize = new Vector2 (400, 190);

			maxSize = minSize;
		}
	}

	#endif
}