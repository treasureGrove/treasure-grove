using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingAndFloorObjectInfoSystem : MonoBehaviour
{
	public mapCreator mapCreatorManager;
	public Transform buildingParent;
	public Transform floorParent;
	
	GameObject currentObjectToChangeBuildingInfo;

	public void setCurrentObjectToChangeBuildingInfo (GameObject newObject)
	{
		currentObjectToChangeBuildingInfo = newObject;
	}

	public void setBuildingAndFloorInfoToObject ()
	{
		mapCreatorManager.setBuildingAndFloorInfoToObject (currentObjectToChangeBuildingInfo, buildingParent, floorParent);
	}

	public void setBuildingFloorInfo (Transform newBuildingParent, Transform newFloorParent, mapCreator newMapCreator)
	{
		mapCreatorManager = newMapCreator;
		buildingParent = newBuildingParent;
		floorParent = newFloorParent;
	}
}
