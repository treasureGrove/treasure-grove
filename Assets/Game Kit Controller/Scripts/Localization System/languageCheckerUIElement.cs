using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GKC.Localization
{
	public class languageCheckerUIElement : languageElementChecker
	{
		[Header ("Custom Settings")]
		[Space]

		public Text mainText;

		[Space]
		[Header ("Text Settings")]
		[Space]

		public bool useLanguageLocalizationManager = true;
		public string localizationKey;

		public bool checkEmptyKey = true;

		public bool setFullTextWithCapsEnabled;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool showDebugPrint;


		bool localizationKeyAssigned;

		public override void updateLanguageOnElement (string currentLanguage)
		{
			if (checkEmptyKey) {
				if (!localizationKeyAssigned) {
					if (localizationKey == "" || localizationKey == null) {
						if (mainText == null) {
							mainText = GetComponentInChildren<Text> ();
						}

						localizationKey = mainText.text;
					}

					localizationKeyAssigned = true;
				}

				if (showDebugPrint) {
					print (localizationKeyAssigned + " " + localizationKey + " " + mainText.text);
				}
			}

			if (useLanguageLocalizationManager) {
				string newText = UIElementsLocalizationManager.GetLocalizedValue (localizationKey);

				if (setFullTextWithCapsEnabled) {
					newText = newText.ToUpper ();
				}

				mainText.text = newText;

				if (showDebugPrint) {
					print (currentLanguage + " " + mainText.text);
				}
			} else {
				for (int i = 0; i < UIElementLanguageInfoList.Count; i++) {
					if (UIElementLanguageInfoList [i].language.Equals (currentLanguage)) {
						mainText.text = UIElementLanguageInfoList [i].textContent;

						if (showDebugPrint) {
							print (currentLanguage + " " + mainText.text);
						}

						return;
					}
				}
			}
		}
	}
}
