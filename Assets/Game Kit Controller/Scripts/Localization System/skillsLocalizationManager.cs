using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GKC.Localization
{
	public class skillsLocalizationManager : languageLocalizationManager
	{
		//static fields
		public static Dictionary<string, string> currentLocalization;

		public static bool localizationInitialized;

		public static localizationFileLoader newLocalizationFileLoader;

		public static bool languageLocated;


		public static string fileNameValue;

		public static string fileFormatValue;
		public static string currentFilePathValue;

		public static bool localizationEnabledValue;

		public static bool useResourcesPathValue;

		public static bool languageFileNotFound;

		void Awake ()
		{
			localizationInitialized = false;

			localizationEnabledValue = false;

			useResourcesPathValue = false;

			languageFileNotFound = false;

			checkFilePathValues ();
		}

		public void checkFilePathValues ()
		{
			fileNameValue = fileName;

			fileFormatValue = fileFormat;

			localizationEnabledValue = localizationEnabled;

			useResourcesPathValue = isUseResourcesPathActive ();

			currentFilePathValue = getCurrentFilePath ();
		}

		public override void updateFileName ()
		{
			checkFilePathValues ();

			checkIfLanguageFileExists ();

			addLanguageListToNewLocalizationFile ();
		}

		public static void updateLocalizationFile ()
		{
			newLocalizationFileLoader = new localizationFileLoader ();

			string newFilePath = "";

			if (useResourcesPathValue) {
				newFilePath = fileNameValue;
			} else {
				newFilePath = currentFilePathValue + fileNameValue + fileFormatValue;
			}

			newLocalizationFileLoader.loadFile (newFilePath, useResourcesPathValue);

			string currentLanguage = GKC_Utils.getCurrentLanguage ();

			updateDictionary (currentLanguage);

			localizationInitialized = true;
		}

		public static void updateDictionary (string currentLanguage)
		{
			currentLocalization = newLocalizationFileLoader.GetDictionaryValues (currentLanguage);

			languageLocated = currentLocalization != null;
		}

		public static string GetLocalizedValue (string key)
		{
			if (!localizationEnabledValue) {
				return key;
			}

			if (languageFileNotFound) {
				return key;
			}

			if (!localizationInitialized) {
				string newFilePath = "";

				if (useResourcesPathValue) {
					newFilePath = fileNameValue;
				} else {
					newFilePath = currentFilePathValue + fileNameValue + fileFormatValue;
				}

				languageFileNotFound = !languageFileExists (newFilePath, useResourcesPathValue, fileNameValue);

				if (languageFileNotFound) {
					return key;
				} else {
					updateLocalizationFile ();
				}
			}


			if (!languageLocated) {
				return key;
			}

			string value = key;

			currentLocalization.TryGetValue (key, out value);

			if (value == null) {
				value = key;
			}

			return value;
		}


		//Editor Functions
		public override Dictionary<string, string> getDictionaryForEditor ()
		{
			updateLocalizationFileFromEditor ();

			return currentLocalization;
		}

		public override void updateLocalizationFileFromEditor ()
		{
			newLocalizationFileLoader = new localizationFileLoader ();

			string newFilePath = "";

			if (isUseResourcesPathActive ()) {
				newFilePath = fileName;
			} else {
				newFilePath = getCurrentFilePath () + fileName + fileFormat;
			}

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			updateDictionary (currentLanguageToEdit);

			if (currentLocalization == null) {
				updateFileName ();
			}
		}

		public override string GetLocalizedValueFromEditor (string key)
		{
			updateLocalizationFileFromEditor ();

			string value = key;

			currentLocalization.TryGetValue (key, out value);

			return value;
		}

		public override void addKey (string key, string value)
		{
			if (value == null || value == "") {
				return;
			}

			if (value.Contains ("\"")) {
				value.Replace ('"', '\"');
			}

			if (newLocalizationFileLoader == null) {
				newLocalizationFileLoader = new localizationFileLoader ();
			}

			string newFilePath = "";

			if (isUseResourcesPathActive ()) {
				newFilePath = fileName;
			} else {
				newFilePath = getCurrentFilePath () + fileName + fileFormat;
			}

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			newLocalizationFileLoader.addLanguageListToNewLocalizationFile (newFilePath);

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			newLocalizationFileLoader.addKey (newFilePath, key, value, currentLanguageToEdit);

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			updateDictionary (currentLanguageToEdit);
		}

		public override void removeKey (string key)
		{
			if (newLocalizationFileLoader == null) {
				newLocalizationFileLoader = new localizationFileLoader ();
			}

			string newFilePath = getCurrentFilePath () + fileName + fileFormat;

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			newLocalizationFileLoader.removeKey (newFilePath, newFilePath, key, isUseResourcesPathActive ());

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			updateDictionary (currentLanguageToEdit);
		}

		public override void addLanguage (string languageName)
		{
			if (languageName == null || languageName == "") {
				return;
			}

			if (languageName.Contains ("\"")) {
				languageName.Replace ('"', '\"');
			}

			if (newLocalizationFileLoader == null) {
				newLocalizationFileLoader = new localizationFileLoader ();
			}

			string newFilePath = "";

			if (isUseResourcesPathActive ()) {
				newFilePath = fileName;
			} else {
				newFilePath = getCurrentFilePath () + fileName + fileFormat;
			}

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			newLocalizationFileLoader.addLanguage (newFilePath, languageName);

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			updateDictionary (currentLanguageToEdit);
		}

		public override void addLanguageListToNewLocalizationFile ()
		{
			if (newLocalizationFileLoader == null) {
				newLocalizationFileLoader = new localizationFileLoader ();
			}

			string newFilePath = "";

			if (isUseResourcesPathActive ()) {
				newFilePath = fileName;
			} else {
				newFilePath = getCurrentFilePath () + fileName + fileFormat;
			}

			newLocalizationFileLoader.loadFile (newFilePath, isUseResourcesPathActive ());

			newLocalizationFileLoader.addLanguageListToNewLocalizationFile (newFilePath);
		}


		public override void updateLocalizationFileExternally ()
		{
			if (!localizationEnabled) {
				return;
			}

			string newFilePath = "";

			if (isUseResourcesPathActive ()) {
				newFilePath = fileName;
			} else {
				newFilePath = getCurrentFilePath () + fileName + fileFormat;
			}

			bool languageFileNotFoundResult = !languageFileExists (newFilePath, isUseResourcesPathActive (), fileName);

			if (languageFileNotFoundResult) {
				if (showDebugPrint) {
					print ("localization file not found, cancelling action");
				}

				return;
			}

			updateLocalizationFile ();

			updateSystemElements ();

			checkEventsOnLanguageChange ();
		}

		void updateSystemElements ()
		{

		}
	}
}