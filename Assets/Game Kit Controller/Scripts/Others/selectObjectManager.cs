using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class selectObjectManager : MonoBehaviour
{
	public List<objectToSelectInfo> objectToSelectInfoList = new List<objectToSelectInfo> ();

	[TextArea (3, 10)]public string explanation = "Press the o button on any of the below elements to select the object or " +
	                                              "component of this character, using it as a shortcut to select directly the object on the hierarchy without need to search " +
	                                              "manually for some element.";


	public List<string> objectSearchResultList = new List<string> ();

	public string objectSearcherName;

	public bool searchObjectsActive;


	public void clearObjectsSearcResultList ()
	{
		objectSearchResultList.Clear ();

		objectSearcherName = "";

		searchObjectsActive = false;
	}

	public void showObjectsBySearchName ()
	{
		if (objectSearcherName != null && objectSearcherName != "") {
			objectSearchResultList.Clear ();

			searchObjectsActive = true;

			string currentTextToSearch = objectSearcherName;

			if (currentTextToSearch != "") {
				currentTextToSearch = currentTextToSearch.ToLower ();

				for (int i = 0; i < objectToSelectInfoList.Count; i++) {
					objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [i];

					if (currentObjectToSelectInfo.Name != "") { 
						string objectName = currentObjectToSelectInfo.Name.ToLower ();

						if (objectName.Contains (currentTextToSearch) ||
						    objectName.Equals (currentTextToSearch)) {

							if (!objectSearchResultList.Contains (currentObjectToSelectInfo.Name)) {
								objectSearchResultList.Add (currentObjectToSelectInfo.Name);
							}
						}
					}
				}
			}
		}
	}

	public void selectObjectByName (string objectName)
	{
		int curretIndex = objectToSelectInfoList.FindIndex (s => s.Name.Equals (objectName));

		if (curretIndex > -1) {
			selectObjectByIndex (curretIndex);
		}
	}

	public void selectObjectByIndex (int index)
	{
		objectToSelectInfo currentObjectToSelectInfo = objectToSelectInfoList [index];

		if (currentObjectToSelectInfo.useComponentSelection) {
			#if UNITY_EDITOR

			string commandCheck = currentObjectToSelectInfo.componentSelectionCommand;

			commandCheck = commandCheck.Replace (" ", "");

			if (commandCheck != "") {
				EditorApplication.ExecuteMenuItem (currentObjectToSelectInfo.componentSelectionCommand);
			}

			#endif
		} else {
			if (currentObjectToSelectInfo.objectToSelect != null) {
				GKC_Utils.setActiveGameObjectInEditor (currentObjectToSelectInfo.objectToSelect);
			}
		}
	}

	[System.Serializable]
	public class objectToSelectInfo
	{
		public string Name;

		public GameObject objectToSelect;

		public bool useComponentSelection;
		[TextArea (3, 10)]public string componentSelectionCommand;
	}
}
