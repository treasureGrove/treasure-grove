using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mapZoneUnlocker : MonoBehaviour
{
	public List<buildingInfo> buildingList = new List<buildingInfo> ();

	public float initialIndex = 0;
	public float finalIndex = 0;

	public void unlockMapZone ()
	{
		mapCreator mapCreatorManager = FindObjectOfType<mapCreator> ();

		for (int i = 0; i < buildingList.Count; i++) {
			for (int j = 0; j < buildingList [i].buildingFloorsList.Count; j++) {
				if (buildingList [i].buildingFloorsList [j].floorEnabled) {
					for (int k = 0; k < buildingList [i].buildingFloorsList [j].mapPartsList.Count; k++) {
						if (buildingList [i].buildingFloorsList [j].mapPartsList [k].mapPartEnabled) {
							mapCreatorManager.buildingList [i].buildingFloorsList [j].mapTileBuilderList [k].enableMapPart ();
						}
					}
				}
			}
		}
	}

	public void enableOrDisableAllFloorParts (bool state, int buildingIndex, int floorIndex)
	{
		for (int j = 0; j < buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList.Count; j++) {
			buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [j].mapPartEnabled = state;
		}

		updateComponent ();
	}

	public void enableOrDisableMapPartsRange (bool state, int buildingIndex, int floorIndex)
	{
		for (int j = 0; j < buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList.Count; j++) {
			if (j >= Mathf.Round (initialIndex) && j < Mathf.Round (finalIndex)) {
				buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [j].mapPartEnabled = state;
			} else {
				buildingList [buildingIndex].buildingFloorsList [floorIndex].mapPartsList [j].mapPartEnabled = false;
			}
		}

		updateComponent ();
	}

	public void searchBuildingList ()
	{
		mapCreator currentMapCreator = FindObjectOfType<mapCreator> ();

		if (currentMapCreator != null) {
			buildingList.Clear ();

			for (int i = 0; i < currentMapCreator.buildingList.Count; i++) {
				buildingInfo newBuildingInfo = new buildingInfo ();

				newBuildingInfo.Name = currentMapCreator.buildingList [i].Name;

				for (int j = 0; j < currentMapCreator.buildingList [i].buildingFloorsList.Count; j++) {
					floorInfo newFloorInfo = new floorInfo ();

					newFloorInfo.Name = currentMapCreator.buildingList [i].buildingFloorsList [j].Name;
					newFloorInfo.floorEnabled = currentMapCreator.buildingList [i].buildingFloorsList [j].floorEnabled;

					newFloorInfo.mapPartsList = new List<mapPartInfo> ();

					for (int k = 0; k < currentMapCreator.buildingList [i].buildingFloorsList [j].mapTileBuilderList.Count; k++) {
						mapTileBuilder currentMapTileBuilder = currentMapCreator.buildingList [i].buildingFloorsList [j].mapTileBuilderList [k];

						mapPartInfo newMapPartInfo = new mapPartInfo ();

						if (currentMapTileBuilder != null) {
							newMapPartInfo.mapTileBuilderManager = currentMapTileBuilder;
							newMapPartInfo.mapPartName = currentMapTileBuilder.mapPartName;
						} else {
							print ("Warning, map tile builder component not found, make sure to use the button Set All Buildings Info or Get All Floor parts in every building " +
							"in the Map Creator inspector, too assign the elements needed to the map system");
						}

						newFloorInfo.mapPartsList.Add (newMapPartInfo);
					}

					newBuildingInfo.buildingFloorsList.Add (newFloorInfo);

				}

				buildingList.Add (newBuildingInfo);
			}
		}

		updateComponent ();
	}

	public void clearAllBuildingList ()
	{
		buildingList.Clear ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class floorInfo
	{
		public string Name;
		public bool floorEnabled;
		public List<mapPartInfo> mapPartsList = new List<mapPartInfo> ();
	}

	[System.Serializable]
	public class buildingInfo
	{
		public string Name;
		public List<floorInfo> buildingFloorsList = new List<floorInfo> ();
	}

	[System.Serializable]
	public class mapPartInfo
	{
		public string mapPartName;
		public bool mapPartEnabled;
		public mapTileBuilder mapTileBuilderManager;
	}
}
