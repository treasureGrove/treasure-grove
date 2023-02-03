using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addMapObjectInformation : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool activateAtStart;
	public bool activateOnEnable;

	public bool callCreateMapIconInfoIfComponentExists;

	[Space]
	[Header ("Map Settings")]
	[Space]

	public bool addMapIcon;
	public string mapObjectName;
	public string mapObjectTypeName;
	[TextArea (3, 10)] public string description;

	public bool visibleInAllBuildings;
	public bool visibleInAllFloors;
	public bool calculateFloorAtStart;

	public bool setFloorNumber;
	public int buildingIndex;
	public int floorNumber;

	[Space]
	[Header ("Objective Screen Icon Settings")]
	[Space]

	public bool addIconOnScreen;
	public float triggerRadius;
	public bool showOffScreenIcon;
	public bool useCloseDistance;
	public bool showMapWindowIcon;
	public bool showDistance;
	public bool showDistanceOffScreen;
	public string objectiveIconName;
	public float objectiveOffset;

	public bool useCustomObjectiveColor;
	public Color objectiveColor;
	public bool removeCustomObjectiveColor;

	public string mainManagerName = "Screen Objectives Manager";

	mapObjectInformation mapObjectInformationManager;
	mapCreator mapCreatorManager;

	screenObjectivesSystem mainscreenObjectivesSystem;

	bool componentAlreadyExists;

	void Start ()
	{
		if (activateAtStart) {
			activateMapObject ();
		}
	}

	void OnEnable ()
	{
		if (activateOnEnable) {
			activateMapObject ();
		}
	}

	public void activateMapObject ()
	{
		if (addMapIcon) {

			if (mapCreatorManager == null) {
				mapCreatorManager = FindObjectOfType<mapCreator> ();
			}

			if (mapCreatorManager == null) {
				print ("Warning: there is no map system configured, so the object " + gameObject.name + " won't use a new map object icon");

				return;
			}

			if (mapObjectInformationManager == null) {
				mapObjectInformationManager = gameObject.AddComponent<mapObjectInformation> ();
			} else {
				componentAlreadyExists = true;
			}

			if (mapObjectInformationManager == null) {
				mapObjectInformationManager = gameObject.GetComponent<mapObjectInformation> ();
			}

			mapObjectInformationManager.assignID (mapCreatorManager.getAndIncreaselastMapObjectInformationIDAssigned ());

			mapObjectInformationManager.setMapObjectName (mapObjectName);

			if (addIconOnScreen) {
				mapObjectInformationManager.setCustomValues (visibleInAllBuildings, visibleInAllFloors, calculateFloorAtStart, useCloseDistance, 
					triggerRadius, showOffScreenIcon, showMapWindowIcon, showDistance, showDistanceOffScreen, objectiveIconName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor);
			}

			if (setFloorNumber) {
				mapObjectInformationManager.floorIndex = floorNumber;
				mapObjectInformationManager.buildingIndex = buildingIndex;
			}

			mapObjectInformationManager.getMapObjectInformation ();
			mapObjectInformationManager.getIconTypeIndexByName (mapObjectTypeName);
			mapObjectInformationManager.description = description;

			if (componentAlreadyExists) {
				if (callCreateMapIconInfoIfComponentExists) {
					mapObjectInformationManager.createMapIconInfo ();

				}
			}
		} else {
			if (addIconOnScreen) {
				if (mainscreenObjectivesSystem == null) {
					GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(screenObjectivesSystem));

					mainscreenObjectivesSystem = FindObjectOfType<screenObjectivesSystem> ();
				}

				if (mainscreenObjectivesSystem != null) {
					mainscreenObjectivesSystem.addElementToScreenObjectiveList (gameObject, useCloseDistance, triggerRadius, showOffScreenIcon, 
						showDistance, showDistanceOffScreen, objectiveIconName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor, objectiveOffset);
				}
			}
		}
	}

	public void removeMapObject ()
	{
		if (mainscreenObjectivesSystem == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(screenObjectivesSystem));

			mainscreenObjectivesSystem = FindObjectOfType<screenObjectivesSystem> ();
		}

		if (mainscreenObjectivesSystem != null) {
			mainscreenObjectivesSystem.removeGameObjectFromList (gameObject);
		}
	}
}
