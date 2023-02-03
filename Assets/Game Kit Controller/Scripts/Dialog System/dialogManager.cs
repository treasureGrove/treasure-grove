using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool dialogSystemEnabled = true;

	public int dialogScene;

	public bool saveCurrentDialogContentToSaveFile = true;

	public LayerMask layerToPlaceObjects;

	[Space]
	[Header ("Dialog Content System List Settings")]
	[Space]

	public List<dialogContentSystem> dialogContentSystemList = new List<dialogContentSystem> ();

	[Space]
	[Header ("Prefabs List")]
	[Space]

	public GameObject dialogContentPrefab;
	public GameObject externalDialogPrefab;

	[HideInInspector] public GameObject lastObjectInstantiated;

	public void getAllDialogContentSystemOnLevel ()
	{
		dialogContentSystemList.Clear ();

		dialogContentSystem[] newDialogContentSystemList = FindObjectsOfType<dialogContentSystem> ();

		foreach (dialogContentSystem currentDialogContentSystem in newDialogContentSystemList) {
			if (!dialogContentSystemList.Contains (currentDialogContentSystem)) {
				dialogContentSystemList.Add (currentDialogContentSystem);
			}
		}
	}

	public void getAllDialogContentOnLevelAndAssignInfo ()
	{
		getAllDialogContentSystemOnLevel ();

		int dialogContentID = 0;

		for (int i = 0; i < dialogContentSystemList.Count; i++) {
			dialogContentSystemList [i].assignIDToDialog (dialogContentID);

			dialogContentSystemList [i].assignDialogContentScene (dialogScene);

			dialogContentID++;
		}

		updateComponent ();
	}

	public void clearDialogContentList ()
	{
		dialogContentSystemList.Clear ();

		updateComponent ();
	}

	public dialogContentSystem getDialogContentSystem (int dialogContentID, int dialogContentScene)
	{
		int dialogContentSystemListCount = dialogContentSystemList.Count;

		//Return the element on scene currently found by ID
		for (int i = 0; i < dialogContentSystemListCount; i++) {
			dialogContentSystem currentDialogContentSystem = dialogContentSystemList [i];

			if (currentDialogContentSystem != null) {
				if (currentDialogContentSystem.dialogContentScene == dialogContentScene && currentDialogContentSystem.dialogContentID == dialogContentID) {

					return currentDialogContentSystem;
				}
			}
		}

		return null;
	}

	public void instantiateDialogContentSystem ()
	{
		instantateObjectOnLevel (dialogContentPrefab);
	}

	public void instantiateExternalDialogSystem ()
	{
		instantateObjectOnLevel (externalDialogPrefab);
	}

	public void instantateObjectOnLevel (GameObject objectToInstantiate)
	{
		Vector3 positionToInstantiate = Vector3.zero;
	
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;
			RaycastHit hit;
			if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceObjects)) {
				positionToInstantiate = hit.point + Vector3.up * 0.05f;
			}
		}

		if (objectToInstantiate) {

			GameObject newCameraTransformElement = (GameObject)Instantiate (objectToInstantiate, positionToInstantiate, Quaternion.identity);
			newCameraTransformElement.name = objectToInstantiate.name;

			lastObjectInstantiated = newCameraTransformElement;

		} else {
			print ("WARNING: prefab gameObject is empty, make sure it is assigned correctly");
		}
	}

	public GameObject getLastObjectInstantiated ()
	{
		return lastObjectInstantiated;
	}

	public void addDialogContentToCharacter (Transform newDialogContentParent, GameObject newDialogContentOwner)
	{
		instantiateDialogContentSystem ();

		GameObject newDialogContentGameObject = getLastObjectInstantiated ();

		dialogContentSystem newDialogContentSystem = newDialogContentGameObject.GetComponent<dialogContentSystem> ();

		if (newDialogContentSystem != null) {
			newDialogContentSystem.setDialogOwnerGameObject (newDialogContentOwner);
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}