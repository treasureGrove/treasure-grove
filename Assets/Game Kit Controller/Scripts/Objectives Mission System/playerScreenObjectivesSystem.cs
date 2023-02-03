using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class playerScreenObjectivesSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showObjectivesActive = true;
	public bool showObjectivesPaused;
	[Range (0, 1)] public float iconOffset;

	public float minDifferenceToUpdateDistanceToObject = 0.4f;

	public string mainManagerName = "Screen Objectives Manager";

	public bool hideObjectivesIfMaxDistanceReached;
	public float maxDistanceToHideObjectives = 2000;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<screenObjectivesSystem.objectiveInfo> objectiveList = new List<screenObjectivesSystem.objectiveInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Transform objectiveIconParent;
	public Transform player;

	public Camera mainCamera;
	public screenObjectivesSystem screenObjectivesManager;
	public playerCamera mainPlayerCamera;

	Vector3 currentMapObjectPosition;
	float currentDistace;
	Vector3 screenPoint;

	bool targetOnScreen;

	Vector3 screenCenter;

	float angle;
	float cos;
	float sin;
	float m;
	Vector3 screenBounds;
	mapCreator mapCreatorManager;

	screenObjectivesSystem.objectiveInfo currentObjective;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	float screenWidth;
	float screenHeight;

	string stringField;

	int objectiveListCount;

	Vector3 currenPlayerPosition;

	Vector3 vector3Forward = Vector3.forward;

	bool ignoreCurrentIconState;

	void Awake ()
	{
		if (screenObjectivesManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(screenObjectivesSystem));

			screenObjectivesManager = FindObjectOfType<screenObjectivesSystem> ();
		} 

		if (screenObjectivesManager != null) {
			screenObjectivesManager.addNewPlayer (this);
		} else {
			showObjectivesActive = false;
		}
	}

	void Start ()
	{
		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
	}

	void FixedUpdate ()
	{
		if (!showObjectivesActive || showObjectivesPaused) {
			return;
		}

		objectiveListCount = objectiveList.Count;

		if (objectiveListCount == 0) {
			return;
		}

		if (!usingScreenSpaceCamera) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}

		currenPlayerPosition = player.position;

		for (int i = 0; i < objectiveListCount; i++) {
			currentObjective = objectiveList [i];

			if (currentObjective.mapObject != null && currentObjective.iconTransform != null) {
				currentMapObjectPosition = currentObjective.mapObject.transform.position;

				if (currentObjective.iconOffset > 0) {
					currentMapObjectPosition += currentObjective.mapObject.transform.up * currentObjective.iconOffset;
				}

				currentDistace = GKC_Utils.distance (currenPlayerPosition, currentMapObjectPosition);

				if (currentObjective.useCloseDistance && currentDistace < currentObjective.closeDistance) {
					removeElementFromListByPlayer (currentObjective);

					return;
				}

				if (currentObjective.showIconPaused) {
					if (currentObjective.offScreenIcon.activeSelf || currentObjective.onScreenIcon.activeSelf) {
						currentObjective.onScreenIcon.SetActive (false);

						currentObjective.offScreenIcon.SetActive (false);

						currentObjective.iconText.gameObject.SetActive (false);
					}
				} else {
					//get the target position from global to local in the screen
					targetOnScreen = false;

					if (usingScreenSpaceCamera) {
						screenPoint = mainCamera.WorldToViewportPoint (currentMapObjectPosition);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
					} else {
						screenPoint = mainCamera.WorldToScreenPoint (currentMapObjectPosition);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
					}

					ignoreCurrentIconState = false;

					if (hideObjectivesIfMaxDistanceReached) {
						if (currentDistace > maxDistanceToHideObjectives) {
							ignoreCurrentIconState = true;
						}
					}

					if (ignoreCurrentIconState) {
						if (currentObjective.offScreenIcon.activeSelf || currentObjective.onScreenIcon.activeSelf) {
							currentObjective.onScreenIcon.SetActive (false);

							currentObjective.offScreenIcon.SetActive (false);

							currentObjective.iconText.gameObject.SetActive (false);
						}
					} else {
						//if the target is visible in the screnn, set the icon position and the distance in the text component
						if (targetOnScreen) {
							//change the icon from offscreen to onscreen
							if (!currentObjective.onScreenIcon.activeSelf) {
								currentObjective.onScreenIcon.SetActive (true);

								currentObjective.offScreenIcon.SetActive (false);

								if (currentObjective.showDistance) {
									currentObjective.iconText.gameObject.SetActive (true);
								} else {
									currentObjective.iconText.gameObject.SetActive (false);
								}

								currentObjective.iconTransform.rotation = Quaternion.identity;
							}

							if (usingScreenSpaceCamera) {
								iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
								currentObjective.iconTransform.anchoredPosition = iconPosition2d;
							} else {
								currentObjective.iconTransform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
							}

							if (currentObjective.showDistance) {
								if (currentObjective.lastDistance == 0 || Mathf.Abs (currentObjective.lastDistance - currentDistace) > minDifferenceToUpdateDistanceToObject) {
									currentObjective.lastDistance = currentDistace;
									stringField = Mathf.Round (currentDistace).ToString ();
									currentObjective.iconText.text = stringField;
								}
							}
						} 

					//if the target is off screen, change the icon to an arrow to follow the target position and also rotate the arrow to the target direction
					else {
							if (currentObjective.showOffScreenIcon) {
								//change the icon from onscreen to offscreen
								if (!currentObjective.offScreenIcon.activeSelf) {
									currentObjective.onScreenIcon.SetActive (false);

									currentObjective.offScreenIcon.SetActive (true);

									currentObjective.iconText.gameObject.SetActive (false);

									if (currentObjective.showDistanceOffScreen) {
										currentObjective.iconText.gameObject.SetActive (true);
									} else {
										currentObjective.iconText.gameObject.SetActive (false);
									}
								}

								if (usingScreenSpaceCamera) {
									iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

									if (screenPoint.z < 0) {
										iconPosition2d *= -1;
									}
									
									angle = Mathf.Atan2 (iconPosition2d.y, iconPosition2d.x);
									angle -= 90 * Mathf.Deg2Rad;
									cos = Mathf.Cos (angle);
									sin = -Mathf.Sin (angle);
									m = cos / sin;
									screenBounds = halfMainCanvasSizeDelta * iconOffset;
									if (cos > 0) {
										iconPosition2d = new Vector2 (screenBounds.y / m, screenBounds.y);
									} else {
										iconPosition2d = new Vector2 (-screenBounds.y / m, -screenBounds.y);
									}

									if (iconPosition2d.x > screenBounds.x) {
										iconPosition2d = new Vector2 (screenBounds.x, screenBounds.x * m);
									} else if (iconPosition2d.x < -screenBounds.x) {
										iconPosition2d = new Vector2 (-screenBounds.x, -screenBounds.x * m);
									}

									currentObjective.iconTransform.anchoredPosition = iconPosition2d;
								} else {
									if (screenPoint.z < 0) {
										screenPoint *= -1;
									}

									screenCenter = new Vector3 (screenWidth, screenHeight, 0) / 2;
									screenPoint -= screenCenter;
									angle = Mathf.Atan2 (screenPoint.y, screenPoint.x);
									angle -= 90 * Mathf.Deg2Rad;
									cos = Mathf.Cos (angle);
									sin = -Mathf.Sin (angle);
									m = cos / sin;
									screenBounds = screenCenter * iconOffset;
									if (cos > 0) {
										screenPoint = new Vector3 (screenBounds.y / m, screenBounds.y, 0);
									} else {
										screenPoint = new Vector3 (-screenBounds.y / m, -screenBounds.y, 0);
									}

									if (screenPoint.x > screenBounds.x) {
										screenPoint = new Vector3 (screenBounds.x, screenBounds.x * m, 0);
									} else if (screenPoint.x < -screenBounds.x) {
										screenPoint = new Vector3 (-screenBounds.x, -screenBounds.x * m, 0);
									}

									//set the position and rotation of the arrow
									screenPoint += screenCenter;

									currentObjective.iconTransform.position = screenPoint;
								}

								currentObjective.iconTransform.eulerAngles = vector3Forward * (angle * Mathf.Rad2Deg);

								if (currentObjective.showDistanceOffScreen) {
									if (currentObjective.lastDistance == 0 || Mathf.Abs (currentObjective.lastDistance - currentDistace) > minDifferenceToUpdateDistanceToObject) {
										currentObjective.lastDistance = currentDistace;
										stringField = Mathf.Round (currentDistace).ToString ();
										currentObjective.iconText.text = stringField;
									}

									currentObjective.iconText.transform.rotation = Quaternion.identity;
								}
							} else {
								if (currentObjective.onScreenIcon.activeSelf || currentObjective.iconText.gameObject.activeSelf) {
									currentObjective.onScreenIcon.SetActive (false);
									currentObjective.iconText.gameObject.SetActive (false);
								}
							}
						}
					}
				}
			} else {
				removeElementFromListByPlayer (currentObjective);
			}
		}
	}

	//get the renderer parts of the target to set its colors with the objective color, to see easily the target to reach
	public void addElementToList (screenObjectivesSystem.objectiveInfo newObjectiveToAdd, GameObject screenIconPrefab, int objectID)
	{
		GameObject newScreenIcon = (GameObject)Instantiate (screenIconPrefab, Vector3.zero, Quaternion.identity);

		screenObjectivesSystem.objectiveInfo newObjective = new screenObjectivesSystem.objectiveInfo ();

		newObjective.Name = newObjectiveToAdd.Name;
		newObjective.mapObject = newObjectiveToAdd.mapObject;
		newObjective.ID = objectID;

		newObjective.useCloseDistance = newObjectiveToAdd.useCloseDistance;
		newObjective.closeDistance = newObjectiveToAdd.closeDistance;
		newObjective.showOffScreenIcon = newObjectiveToAdd.showOffScreenIcon;
		newObjective.showDistance = newObjectiveToAdd.showDistance;
		newObjective.showDistanceOffScreen = newObjectiveToAdd.showDistanceOffScreen;

		newObjective.iconOffset = newObjectiveToAdd.iconOffset;
		newObjective.showIconPaused = newObjectiveToAdd.showIconPaused;

		newObjective.iconTransform = newScreenIcon.GetComponent<RectTransform> ();
		newObjective.iconTransform.SetParent (objectiveIconParent);
		newObjective.iconTransform.localScale = Vector3.one;
		newObjective.iconTransform.localPosition = Vector3.zero;

		objectiveIconInfo newObjectiveIcon = newScreenIcon.GetComponent<objectiveIconInfo> ();
		newObjective.onScreenIcon = newObjectiveIcon.onScreenIcon;
		newObjective.offScreenIcon = newObjectiveIcon.offScreenIcon;
		newObjective.iconText = newObjectiveIcon.iconText;

		objectiveList.Add (newObjective);
	}

	//if the target is reached, disable all the parameters and clear the list, so a new objective can be added in any moment
	public void removeElementFromList (int objectID)
	{
		for (int i = 0; i < objectiveList.Count; i++) {
			if (objectiveList [i].ID == objectID) {
				if (objectiveList [i].iconTransform != null) {
					Destroy (objectiveList [i].iconTransform.gameObject);
				}

				objectiveList.RemoveAt (i);
				return;
			}
		}
	}

	public void removeElementFromListByPlayer (screenObjectivesSystem.objectiveInfo objectiveListElement)
	{
		if (objectiveListElement.iconTransform != null) {
			Destroy (objectiveListElement.iconTransform.gameObject);
		}

		screenObjectivesManager.removeElementFromObjectiveListCalledByPlayer (objectiveListElement.ID, player.gameObject);

		objectiveList.Remove (objectiveListElement);
	}

	public void pauseOrResumeShowObjectives (bool state)
	{
		showObjectivesPaused = state;

		objectiveIconParent.gameObject.SetActive (!showObjectivesPaused);
	}

	public void addElementToPlayerList (GameObject obj, bool addMapIcon, bool useCloseDistance, float radiusDistance, bool showOffScreen, bool showMapWindowIcon, 
	                                    bool showDistanceInfo, bool showDistanceOffScreenInfo, string objectiveIconName, bool useCustomObjectiveColor, Color objectiveColor, 
	                                    bool removeCustomObjectiveColor, int buildingIndex, float iconOffset, bool addIconOnRestOfPlayers)
	{

		if (objectAlreadyOnList (obj)) {
			return;
		}

		screenObjectivesSystem.objectiveInfo newObjective = new screenObjectivesSystem.objectiveInfo ();

		newObjective.Name = obj.name;
		newObjective.mapObject = obj;
		newObjective.iconOffset = iconOffset;
		newObjective.ID = screenObjectivesManager.getCurrentID ();
		screenObjectivesManager.increaseCurrentID ();

		int currentObjectiveIconIndex = -1;

		for (int i = 0; i < screenObjectivesManager.objectiveIconList.Count; i++) {
			if (currentObjectiveIconIndex == -1 && screenObjectivesManager.objectiveIconList [i].name.Equals (objectiveIconName)) {
				currentObjectiveIconIndex = i;
			}
		}

		if (currentObjectiveIconIndex != -1) {

			screenObjectivesSystem.objectiveIconElement currentObjectiveIconElement = screenObjectivesManager.objectiveIconList [currentObjectiveIconIndex];

			if (radiusDistance < 0) {
				radiusDistance = currentObjectiveIconElement.minDefaultDistance;
			}

			newObjective.useCloseDistance = useCloseDistance;
			newObjective.closeDistance = radiusDistance;
			newObjective.showOffScreenIcon = showOffScreen;
			newObjective.showDistance = showDistanceInfo;
			newObjective.showDistanceOffScreen = showDistanceOffScreenInfo;

			if (currentObjectiveIconElement.changeObjectiveColors && !removeCustomObjectiveColor) {

				int propertyNameID = Shader.PropertyToID ("_Color");

				Component[] components = obj.GetComponentsInChildren (typeof(Renderer));
				foreach (Renderer child in components) {
					if (child.material.HasProperty (propertyNameID)) {

						int materialsLength = child.materials.Length;

						for (int j = 0; j < materialsLength; j++) {
							Material currentMaterial = child.materials [j];

							newObjective.materials.Add (currentMaterial);
							newObjective.originalColor.Add (currentMaterial.color);

							if (useCustomObjectiveColor) {
								currentMaterial.color = objectiveColor;
							} else {
								currentMaterial.color = currentObjectiveIconElement.objectiveColor;
							}
						}
					}
				}
			}

//			print ("1 " + obj.name);

			//add the target to the radar, to make it also visible there
			if (mapCreatorManager == null) {
				getMapCreatorManagerComponent ();
			}

//			print ("2 " + obj.name);

			if (mapCreatorManager != null && addMapIcon && showMapWindowIcon) {
				mapCreatorManager.addMapObject (true, true, false, obj, "Objective", Vector3.zero, -1, -1, buildingIndex, 0, false, false, false, false, null, 0);
			}

//			print ("3 " + obj.name);

			addElementToList (newObjective, currentObjectiveIconElement.iconInfoElement.gameObject, newObjective.ID);

//			print ("4 " + obj.name);

			screenObjectivesManager.addElementFromPlayerList (newObjective, currentObjectiveIconElement, player, addIconOnRestOfPlayers);
	
//			print ("5 " + obj.name);
		} else {
			print ("Element not found in objective icon list");
		}
	}

	public void getMapCreatorManagerComponent ()
	{
		mapCreatorManager = FindObjectOfType<mapCreator> ();
	}

	public bool objectAlreadyOnList (GameObject objectToCheck)
	{
		if (objectToCheck == null) {
			return true;
		}

		for (int i = 0; i < objectiveList.Count; i++) {
			if (objectiveList [i].mapObject == objectToCheck) {
				return true;
			}
		}

		return false;
	}

	public void removeElementFromListByPlayer (GameObject objectToCheck)
	{
		if (objectToCheck == null) {
			return;
		}

		for (int i = 0; i < objectiveList.Count; i++) {
			if (objectiveList [i].mapObject == objectToCheck) {
				if (objectiveList [i].iconTransform != null) {
					Destroy (objectiveList [i].iconTransform.gameObject);
				}

				int currentID = objectiveList [i].ID;

				objectiveList.Remove (objectiveList [i]);

				screenObjectivesManager.removeElementFromObjectiveListCalledByPlayer (currentID, player.gameObject);

				return;
			}
		}
	}

	public void removeElementListFromListByPlayer (List<Transform> objectToCheckList)
	{
		if (objectToCheckList == null || objectToCheckList.Count == 0) {
			return;
		}

		for (int i = 0; i < objectToCheckList.Count; i++) {
			if (objectToCheckList [i] != null) {
				removeElementFromListByPlayer (objectToCheckList [i].gameObject);
			}
		}
	}

	public void setShowObjectivesActiveState (bool state)
	{
		showObjectivesActive = state;
	}
}