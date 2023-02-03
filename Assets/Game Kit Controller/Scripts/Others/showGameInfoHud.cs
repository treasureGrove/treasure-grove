using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class showGameInfoHud : MonoBehaviour
{
	public List<hudElementInfo> hudElements = new List<hudElementInfo> ();

	public enum elementType
	{
		Text,
		Slider,
		Panel
	}

	public GameObject getHudElement (string parentName, string elementName)
	{
		for (int i = 0; i < hudElements.Count; i++) {
			if (hudElements [i].name.Equals (parentName)) {
				for (int j = 0; j < hudElements [i].rectTransformList.Count; j++) {
					if (hudElements [i].rectTransformList [j].name.Equals (elementName)) {
						return hudElements [i].rectTransformList [j].rectTransform.gameObject;
					}
				}
			}
		}

		return null;
	}

	public List<GameObject> getHudElements (string parentName, List<string> elementNames)
	{
		List<GameObject> hudElementList = new  List<GameObject> ();

		for (int i = 0; i < hudElements.Count; i++) {
			if (hudElements [i].name.Equals (parentName)) {
				for (int j = 0; j < hudElements [i].rectTransformList.Count; j++) {
					if (elementNames.Contains (hudElements [i].rectTransformList [j].name)) {
						hudElementList.Add (hudElements [i].rectTransformList [j].rectTransform.gameObject);
					}
				}
			}
		}

		return hudElementList;
	}

	public GameObject getHudElementParent (string parentName)
	{
		for (int i = 0; i < hudElements.Count; i++) {
			if (hudElements [i].name.Equals (parentName)) {
				return hudElements [i].hudParent.gameObject;
			}
		}

		return null;
	}

	[System.Serializable]
	public class hudElementInfo
	{
		public string name;
		public GameObject hudParent;
		public List<rectTransformInfo> rectTransformList = new List<rectTransformInfo> ();
	}

	[System.Serializable]
	public class rectTransformInfo
	{
		public string name;
		public RectTransform rectTransform;
		public elementType hudElementyType;
	}
}