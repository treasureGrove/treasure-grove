using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class languageElementChecker : MonoBehaviour
{
	[Space]
	[Header ("Language Settings")]
	[Space]

	public List<UIElementLanguageInfo> UIElementLanguageInfoList = new List<UIElementLanguageInfo> ();

	public virtual void updateLanguageOnElement ()
	{
		string currentLanguage = GKC_Utils.getCurrentLanguage ();

		updateLanguageOnElement (currentLanguage);
	}

	public virtual void updateLanguageOnElement (string currentLanguage)
	{

	}

	[System.Serializable]
	public class UIElementLanguageInfo
	{
		public string language;

		[TextArea (3, 5)] public string textContent;

		[TextArea (3, 5)] public string extraTextContent;
	}
}
