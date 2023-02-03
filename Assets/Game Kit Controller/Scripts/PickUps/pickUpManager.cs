using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class pickUpManager : MonoBehaviour
{
	public bool showIconsActive = true;

	public List<pickUpElementInfo> mainPickUpList = new List<pickUpElementInfo> ();

	public List<pickUpIconInfo> pickUpIconList = new List<pickUpIconInfo> ();

	public List<playerPickupIconManager> playerPickupIconManagerList = new List<playerPickupIconManager> ();

	int currentID = 0;

	public void addNewPlayer (playerPickupIconManager newPlayer)
	{
		if (!showIconsActive) {
			return;
		}

		playerPickupIconManagerList.Add (newPlayer);
	}

	//set what type of pick up is this object, and the object that the icon has to follow
	public void setPickUpIcon (GameObject target, string pickupIconGeneralName, string pickupIconName)
	{
		if (!showIconsActive) {
			return;
		}

		pickUpIconInfo newIcon = new pickUpIconInfo ();
		newIcon.ID = currentID;
		newIcon.target = target;

		newIcon.targetTransform = target.transform;

		Texture iconTexture = null;

		GameObject iconPrefab = null;

		int pickupListIndex = mainPickUpList.FindIndex (s => s.pickUpType.ToLower () == pickupIconGeneralName.ToLower ());

		if (pickupListIndex > -1) {
			pickUpElementInfo currentPickupElementInfo = mainPickUpList [pickupListIndex];

			bool useGeneralIcon = currentPickupElementInfo.useGeneralIcon;

			int customIndexTexture = -1;

			if (!pickupIconName.Equals ("")) {
				int pickupTypeIndex = currentPickupElementInfo.pickUpTypeList.FindIndex (s => s.Name.ToLower () == pickupIconName.ToLower ());

				if (pickupTypeIndex > -1) {
					customIndexTexture = pickupTypeIndex;

					useGeneralIcon = false;
				}
			}

			if (useGeneralIcon) {
				iconTexture = currentPickupElementInfo.generalIcon;
			} else {
				if (customIndexTexture > -1) {
					iconTexture = currentPickupElementInfo.pickUpTypeList [customIndexTexture].pickupIcon;
				}
			}

			bool useCustomIconPrefab = currentPickupElementInfo.useCustomIconPrefab;

			if (customIndexTexture > -1) {
				if (currentPickupElementInfo.pickUpTypeList [customIndexTexture].useCustomIconPrefab) {
					iconPrefab = currentPickupElementInfo.pickUpTypeList [customIndexTexture].customIconPrefab;

					useCustomIconPrefab = false;
				}
			}

			if (useCustomIconPrefab) {
				iconPrefab = currentPickupElementInfo.customIconPrefab;
			}
		}

		pickUpIconList.Add (newIcon);

		for (int i = 0; i < playerPickupIconManagerList.Count; i++) {
			playerPickupIconManagerList [i].setPickUpIcon (target, iconTexture, currentID, iconPrefab);
		}

		currentID++;
	}

	//destroy the icon
	public void removeTarget (GameObject target)
	{
		if (!showIconsActive) {
			return;
		}

		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].target != null) {
				if (pickUpIconList [i].target == target) {
					removeAtTarget (pickUpIconList [i].ID, i);

					return;
				}
			}
		}
	}

	public void removeAtTarget (int objectID, int objectIndex)
	{
		if (!showIconsActive) {
			return;
		}

		for (int i = 0; i < playerPickupIconManagerList.Count; i++) {
			playerPickupIconManagerList [i].removeAtTargetByID (objectID);
		}

		pickUpIconList.RemoveAt (objectIndex);
	}

	public void removeElementFromPickupListCalledByPlayer (int objectID)
	{
		if (!showIconsActive) {
			return;
		}

		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].ID == objectID) {
				pickUpIconList.RemoveAt (i);
				return;
			}
		}
	}

	public void setPauseState (bool state, GameObject iconObject)
	{
		if (!showIconsActive) {
			return;
		}

		for (int i = 0; i < pickUpIconList.Count; i++) {
			if (pickUpIconList [i].target == iconObject) {
				
				pickUpIconList [i].paused = state;

				for (int j = 0; j < playerPickupIconManagerList.Count; j++) {
					playerPickupIconManagerList [j].setPauseState (state, i);
				}
			}
		}
	}

	public void setNamesConfigured (int currentIndex)
	{
		pickUpElementInfo currentpickUpElementInfo = mainPickUpList [currentIndex];

		for (int i = 0; i < currentpickUpElementInfo.pickUpTypeList.Count; i++) {
			if (currentpickUpElementInfo.pickUpTypeList [i].pickUpObject != null) {
				currentpickUpElementInfo.pickUpTypeList [i].Name = currentpickUpElementInfo.pickUpTypeList [i].pickUpObject.name;
			}
		}
	}

	public void addNewPickup ()
	{
		pickUpElementInfo newPickupElementInfo = new pickUpElementInfo ();
		newPickupElementInfo.pickUpType = "New Pickup Type";
		mainPickUpList.Add (newPickupElementInfo);

		udpateComponent ();
	}

	public void addNewPickupToList (int index)
	{
		pickUpElementInfo.pickUpTypeElementInfo newPickupTypeElementInfo = new pickUpElementInfo.pickUpTypeElementInfo ();
		newPickupTypeElementInfo.Name = "New Pickup";
		mainPickUpList [index].pickUpTypeList.Add (newPickupTypeElementInfo);

		udpateComponent ();
	}

	public void udpateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}