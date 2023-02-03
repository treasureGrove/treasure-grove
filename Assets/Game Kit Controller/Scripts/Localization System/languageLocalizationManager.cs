using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System;

namespace GKC.Localization
{
	public class languageLocalizationManager : MonoBehaviour
	{
		[Space]
		[Header ("Language Tool Settings")]
		[Space]

		public string currentLanguageToEdit = "English";

		public string filePath = "Assets/Game Kit Controller/Scripts/Editor/Resources/";

		public string filePathBuild = "./Localization/";

		public string fileFormat = ".csv";

		public string fileName;

		[Space]
		[Header ("Other Settings")]
		[Space]

		public bool useLocalFilePath;

		public bool useResourcesPath;

		public bool localizationEnabled = true;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool showDebugPrint;

		[Space]
		[Header ("Event Settings")]
		[Space]

		public bool useEventsOnLanguageChange;
		public UnityEvent eventOnLanguageChange;
	

		//Virtual functions
		public virtual void updateFileName ()
		{
			
		}

		public virtual void updateLocalizationFileExternally ()
		{
			
		}

		public virtual void checkEventsOnLanguageChange ()
		{
			if (useEventsOnLanguageChange) {
				eventOnLanguageChange.Invoke ();
			}
		}

		public virtual Dictionary<string, string> getDictionaryForEditor ()
		{
			
			return null;
		}

		public virtual void updateLocalizationFileFromEditor ()
		{
			
		}

		public virtual string GetLocalizedValueFromEditor (string key)
		{
			
			return "";
		}

		public virtual void addKey (string key, string value)
		{
			
		}

		public virtual void removeKey (string key)
		{
			
		}

		public virtual void addLanguage (string languageName)
		{
			
		}

		public virtual void addLanguageListToNewLocalizationFile ()
		{
			
		}


		//Main functions
		public string getCurrentFilePath ()
		{
			if (Application.isEditor && !useLocalFilePath) {
				return filePath;
			} else {
				return filePathBuild;
			}
		}

		public bool isUseResourcesPathActive ()
		{
			if (useResourcesPath) {
				return true;
			}

			if (touchJoystick.checkTouchPlatform ()) {
				return true;
			}

			return false;
		}

		public void checkIfLanguageFileExists ()
		{
			string currentFiletPath = getCurrentFilePath ();

			string newFilePath = currentFiletPath + fileName + fileFormat;

			if (showDebugPrint) {
				print ("Checking if " + fileName + " exists on path: " + newFilePath);
			}

			if (isUseResourcesPathActive ()) {
				TextAsset textFile = Resources.Load (fileName) as TextAsset;

				if (textFile == null) {
					string m_path = Application.dataPath + "/Resources/" + fileName + ".csv";

					print (m_path);

					FileStream file = File.Open (newFilePath, FileMode.OpenOrCreate); 

					file.Close ();
				}

			} else {
				if (System.IO.File.Exists (newFilePath)) {
					if (showDebugPrint) {
						print ("File Located");
					}
				} else {
					if (showDebugPrint) {
						print (newFilePath + " doesn't exist");
					}

					FileStream file = File.Open (newFilePath, FileMode.OpenOrCreate); 

					file.Close ();
				}
			}
		}

		public static bool languageFileExists (string newFilePath, bool useResourcesPathValue, string fileNameValue)
		{
			if (useResourcesPathValue) {
				TextAsset textFile = Resources.Load (fileNameValue) as TextAsset;

				if (textFile != null) {
					return true;
				}
			}

			if (System.IO.File.Exists (newFilePath)) {
				return true;
			} 

			return false;
		}

		//EDITOF FUNCTIONS
		public void updateComponent ()
		{
			GKC_Utils.updateComponent (this);

			GKC_Utils.updateDirtyScene ("Update Localization " + gameObject.name, gameObject);
		}
	}
}