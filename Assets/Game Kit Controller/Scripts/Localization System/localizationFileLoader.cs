using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Text;

namespace GKC.Localization
{
	public class localizationFileLoader
	{
		public StreamReader mainFile;

		public StringBuilder mainFileText;

		private char lineSeparator = '\n';
		private char surround = '"';
		private string[] fieldSeparator = { "\",\"" };

		private string fileContent;

		public void loadFile (string filePath, bool useResourcesPath)
		{
			if (useResourcesPath) {
				TextAsset textFile = Resources.Load (filePath) as TextAsset;

				if (textFile != null) {
					fileContent = textFile.ToString ();
				}
			} else {
				mainFile = new StreamReader (filePath); 
			}

			mainFileText = new StringBuilder ();

			if (useResourcesPath) {
				mainFileText.Append (fileContent);
			} else {
				mainFileText.Append (mainFile.ReadToEnd ());

				mainFile.Close ();
			}
		}

		public Dictionary<string, string> GetDictionaryValues (string attributeID)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> ();

			if (mainFileText == null) {
				Debug.Log ("WARNING: Dictionary not found when updating file, make sure to create a localization file before");

				return null;
			}

			string[] lines = mainFileText.ToString ().Split (lineSeparator);

			int attributeIndex = -1;

			string[] headers = lines [0].Split (fieldSeparator, StringSplitOptions.None);

			for (int i = 0; i < headers.Length; i++) {
				if (headers [i].Contains (attributeID)) {
					attributeIndex = i;

					break;
				}
			}

			if (attributeIndex > -1) {

				Regex textTileParser = new Regex (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
	
				for (int i = 1; i < lines.Length; i++) {

					string line = lines [i];

					string[] fields = textTileParser.Split (line);

					for (int j = 0; j < fields.Length; j++) {
						fields [j] = fields [j].TrimStart (' ', surround);
						fields [j] = fields [j].TrimEnd ('\r', surround);
					}

					if (fields.Length > attributeIndex) {
						var key = fields [0];

						if (dictionary.ContainsKey (key)) {
							continue;
						}

						var value = fields [attributeIndex];

						dictionary.Add (key, value);
					}
				}

				return dictionary;
			} 

			return null;
		}

		public void addKey (string filePath, string key, string value, string languageName)
		{
			#if UNITY_EDITOR

			string[] lines = mainFileText.ToString ().Split (lineSeparator);

			int languageIndex = -1;

			string[] headers = lines [0].Split (fieldSeparator, StringSplitOptions.None);

			for (int i = 0; i < headers.Length; i++) {
				if (headers [i].Contains (languageName)) {
					languageIndex = i;

					break;
				}
			}

			if (languageIndex > -1) {
				string[] keys = new string [lines.Length];

				for (int i = 0; i < lines.Length; i++) {
					string line = lines [i];

					keys [i] = line.Split (fieldSeparator, StringSplitOptions.None) [0];
				}

				int keyIndex = -1;

				for (int i = 0; i < keys.Length; i++) {
					if (keys [i].Contains (key)) {
						keyIndex = i;

						break;
					}
				}

				if (keyIndex > -1) {
					Debug.Log ("Adding new value to existing key " + languageIndex + " " + headers.Length);

					string line = lines [keyIndex];

					Debug.Log (line);

					Regex textTileParser = new Regex (",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

					string[] fields = textTileParser.Split (line);

					string newLine = "";

					for (int j = 0; j < fields.Length; j++) {
						fields [j] = fields [j].TrimStart (' ', surround);
						fields [j] = fields [j].TrimEnd ('\r', surround);

						Debug.Log (fields [j]);

						if (languageIndex == j) {
							newLine += string.Format ("\"{0}\"", value);
						} else {
							newLine += string.Format ("\"{0}\"", fields [j]);
						}

						if (j < fields.Length - 1) {
							newLine += ",";
						}
					}

					Debug.Log (newLine);

					string newContent = mainFileText.ToString ().Replace (line, newLine);

					Debug.Log (newContent);

					File.WriteAllText (filePath, newContent);

					GKC_Utils.refreshAssetDatabase ();

				} else {
					Debug.Log ("Adding new key " + key + " with index " + languageIndex + " for language " + languageName
					+ " with an amount of languages of " + headers.Length);

					string appended = string.Format ("\n\"{0}\"", key);

					int numberOfLanguages = headers.Length;

					for (int i = 1; i < numberOfLanguages; i++) {
						appended += ",";

						if (languageIndex == i) {
							appended += string.Format ("\"{0}\"", value);
						} else {
							appended += string.Format ("\"{0}\"", "");
						}
					}

					File.AppendAllText (filePath, appended); 

					GKC_Utils.refreshAssetDatabase ();
				}
			}

			#endif
		}

		public void removeKey (string fileName, string filePath, string key, bool useResourcesPath)
		{
			#if UNITY_EDITOR

			if (useResourcesPath) {
				TextAsset textFile = Resources.Load (filePath) as TextAsset;

				if (textFile != null) {
					fileContent = textFile.ToString ();
				}
			} else {
				mainFile = new StreamReader (filePath); 
			}

			mainFileText = new StringBuilder ();

			if (useResourcesPath) {
				mainFileText.Append (fileContent);
			} else {
				mainFileText.Append (mainFile.ReadToEnd ());

				mainFile.Close ();
			}


			string[] lines = mainFileText.ToString ().Split (lineSeparator);

			string[] keys = new string [lines.Length];

			for (int i = 0; i < lines.Length; i++) {
				string line = lines [i];

				keys [i] = line.Split (fieldSeparator, StringSplitOptions.None) [0];
			}

			int index = -1;

			for (int i = 0; i < keys.Length; i++) {
				if (keys [i].Contains (key)) {
					index = i;

					break;
				}
			}

			if (index > -1) {
				string[] newLines;

				newLines = lines.Where (w => w != lines [index]).ToArray ();

				string replaced = string.Join (lineSeparator.ToString (), newLines);

				File.WriteAllText (filePath, replaced);

				GKC_Utils.refreshAssetDatabase ();
			}

			#endif
		}

		public void addLanguage (string filePath, string languageName)
		{
			#if UNITY_EDITOR

			string[] lines = mainFileText.ToString ().Split (lineSeparator);

			int languageIndex = -1;

			string[] headers = lines [0].Split (fieldSeparator, StringSplitOptions.None);

			for (int i = 0; i < headers.Length; i++) {
				if (headers [i].Contains (languageName)) {
					languageIndex = i;
					break;
				}
			}

			if (languageIndex > -1) {
				Debug.Log ("Language already added");
			} else {
				Debug.Log ("Adding new language: " + languageName);

				string newFileContent = "";

				string newLanguage = "," + string.Format ("\"{0}\"", languageName);

				string line = lines [0];

				line = line.Replace ("\r", "").Replace ("\n", "");

				string newLine = line + newLanguage;

				newLine = newLine.Replace ("\r", "").Replace ("\n", "");

				newFileContent += newLine;

				newFileContent += "\n";
			
				for (int i = 1; i < lines.Length; i++) {

					lines [i] = lines [i].Replace ("\r", "").Replace ("\n", "");

					string currentNewLine = lines [i];

					string separation = ",";

					currentNewLine += separation + "\"" + "\"";

					currentNewLine = currentNewLine.Replace ("\r", "").Replace ("\n", "");

					currentNewLine += "\n";

					newFileContent += currentNewLine;
				}
			
				File.WriteAllText (filePath, newFileContent);

				GKC_Utils.refreshAssetDatabase ();

				GKC_Utils.addLanguage (languageName);
			}

			#endif
		}

		public void addLanguageListToNewLocalizationFile (string filePath)
		{
			#if UNITY_EDITOR

			if (mainFileText == null) {
				Debug.Log ("Main File Text Not Found");

				return;
			}

			string fileContent = mainFileText.ToString ();

			if (fileContent.Length == 0) {

				List<string> languageNameList = GKC_Utils.getCurrentLanguageList ();

				fileContent += string.Format ("\"{0}\"", "key");
				
				for (int i = 0; i < languageNameList.Count; i++) {
					fileContent += ",";

					fileContent += string.Format ("\"{0}\"", languageNameList [i]);
				}

				File.AppendAllText (filePath, fileContent);

				GKC_Utils.refreshAssetDatabase ();

				Debug.Log ("Adding language list to new file created");
			}
			#endif
		}
	}
}