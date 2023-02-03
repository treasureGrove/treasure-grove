using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class characterCustomizationManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool characterCustomizationEnabled = true;

	public string objectCategoryName = "Object";
	public string blendShapesCategoryName = "BlendShapes";
	public string accessoriesCategoryName = "Accessories";

	[Space]
	[Header ("Main Character Base Model Settings")]
	[Space]

	public characterObjectTypeInfo mainCharacterBaseInfo;

	[Space]
	[Header ("Character Sets Settings")]
	[Space]

	public List<characterObjectTypeInfo> characterObjectTypeInfoList = new List<characterObjectTypeInfo> ();

	[Space]
	[Header ("Character Accesories Settings")]
	[Space]

	public List<characterObjectTypeInfo> characterAccessoriesTypeInfoList = new List<characterObjectTypeInfo> ();

	[Space]
	[Header ("Character Blend Shapes Settings")]
	[Space]

	public List<characterBlendShapeTypeInfo> characterBlendShapeTypeInfoList = new List<characterBlendShapeTypeInfo> ();

	[Space]
	[Header ("Head Settings")]
	[Space]

	public string headObjectName = "Head";

	public List<string> objectsOnHeadList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool keepAllCharacterMeshesDisabledActive;

	public bool headActiveTemporaly;

	[Space]
	[Header ("Current Elements Debug")]
	[Space]

	public List<string> currentPiecesList = new List<string> ();

	public List<string> currentAccessoriesList = new List<string> ();

	public List<temporalBlendshapeInfo> temporalBlendshapeInfoList = new List<temporalBlendshapeInfo> ();

	[Space]
	[Header ("Editor Settings")]
	[Space]

	public string armorSetToCreatePieceMeshesObjects;

	[Space]
	[Header ("Durability Settings")]
	[Space]

	public bool useEventOnDurabilityAffectedEnabled;

	[Space]

	public List<durabilityPartInfo> durabilityPartInfoList = new List<durabilityPartInfo> ();


	bool headActivePreviously;


	public void resetAllCharacterObjectstoMainBaseModel ()
	{
		disableAndClearAccessoriesList ();

		disableAndClearAllCharacterObjectList ();

		setMainCharacterBaseInfoFullActiveState (true);
	}

	//Functions to activate select full set templates
	public void setCustomizationFromTemplate (characterAspectCustomizationTemplate newCharacterAspectCustomizationTemplate, bool isFullSet)
	{
		if (isFullSet) {
			resetAllCharacterObjectstoMainBaseModel ();
		}

		for (int i = 0; i < newCharacterAspectCustomizationTemplate.characterCustomizationTypeInfoList.Count; i++) {
			for (int j = 0; j < newCharacterAspectCustomizationTemplate.characterCustomizationTypeInfoList [i].characterCustomizationInfoList.Count; j++) {
				setCustomizationFromOptionInfo (newCharacterAspectCustomizationTemplate.characterCustomizationTypeInfoList [i].characterCustomizationInfoList [j], isFullSet);
			}
		}
	}

	void setCustomizationFromOptionInfo (characterCustomizationInfo newCharacterCustomizationInfo, bool isFullSet)
	{
		if (showDebugPrint) {
			print (newCharacterCustomizationInfo.Name);
		}

		bool isObjectCategory = newCharacterCustomizationInfo.typeName.Equals (objectCategoryName);
		bool isAccessoriesCategory = newCharacterCustomizationInfo.typeName.Equals (accessoriesCategoryName);

		if (showDebugPrint) {
			print ("isObjectCategory " + isObjectCategory + " isAccessoriesCategory " + isAccessoriesCategory);
		}

		if (isObjectCategory || isAccessoriesCategory) {
			List<characterObjectTypeInfo> currentCharacterObjectTypeInfoList = new List<characterObjectTypeInfo> ();

			if (isObjectCategory) {
				currentCharacterObjectTypeInfoList = characterObjectTypeInfoList;
			}

			if (isAccessoriesCategory) {
				currentCharacterObjectTypeInfoList = characterAccessoriesTypeInfoList;
			}
				
			for (int i = 0; i < currentCharacterObjectTypeInfoList.Count; i++) {
				if (!isFullSet) {
					for (int j = 0; j < currentCharacterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
						if (newCharacterCustomizationInfo.categoryName.Equals (currentCharacterObjectTypeInfoList [i].categoryName)) {

							characterObjectInfo currentInfo = currentCharacterObjectTypeInfoList [i].characterObjectInfoList [j];

							if (currentInfo.elementObject.activeSelf != false) {
								currentInfo.elementObject.SetActive (false);

								if (isAccessoriesCategory) {
									checkCurrentAccessoriesList (currentInfo.Name, false);
								} else {
									checkCurrentPieceList (currentInfo.Name, false);
								}
							}
						}
					}
				}


				//set random bool value
				bool boolValue = newCharacterCustomizationInfo.boolValue;

				if (newCharacterCustomizationInfo.useRandomBoolValue) {
					int boolInteger = Random.Range (0, 2);

					boolValue = (boolInteger == 0);

					if (showDebugPrint) {
						print ("Random bool value is " + boolValue);
					}
				}


				//check the type of option to use for the elements to change its active state
				if (newCharacterCustomizationInfo.multipleElements) {
					bool isCurrentOption = (currentCharacterObjectTypeInfoList [i].Name.Equals (newCharacterCustomizationInfo.Name));

					if (isCurrentOption) {

						for (int j = 0; j < currentCharacterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
							characterObjectInfo currentInfo = currentCharacterObjectTypeInfoList [i].characterObjectInfoList [j];

							if (!keepAllCharacterMeshesDisabledActive || !boolValue) {
								if (currentInfo.elementObject.activeSelf != boolValue) {
									currentInfo.elementObject.SetActive (boolValue);
								}
							}

							if (isAccessoriesCategory) {
								checkCurrentAccessoriesList (currentInfo.Name, boolValue);
							} else {
								checkCurrentPieceList (currentInfo.Name, boolValue);
							}

							if (!isAccessoriesCategory) {
								if (!currentInfo.ignoreDisableSameTypeName) {
									GameObject mainCharacterBaseInfoObject = getMainCharacterBaseInfoObjectByTypeName (currentInfo.typeName);

									if (mainCharacterBaseInfoObject != null) {
										if (mainCharacterBaseInfoObject != currentInfo.elementObject) {
											mainCharacterBaseInfoObject.SetActive (false);
										}
									}
								}
							}
						}
					}
				} else if (newCharacterCustomizationInfo.useRandomObjectFromCategoryName) {
					bool isCurrentOption = (currentCharacterObjectTypeInfoList [i].Name.Equals (newCharacterCustomizationInfo.Name));

					if (isCurrentOption) {
						int randomIndex = Random.Range (0, currentCharacterObjectTypeInfoList [i].characterObjectInfoList.Count);

						characterObjectInfo currentInfo = currentCharacterObjectTypeInfoList [i].characterObjectInfoList [randomIndex];

						if (!keepAllCharacterMeshesDisabledActive || !boolValue) {
							if (currentInfo.elementObject.activeSelf != boolValue) {
								currentInfo.elementObject.SetActive (boolValue);
							}
						}

						if (isAccessoriesCategory) {
							checkCurrentAccessoriesList (currentInfo.Name, boolValue);
						} else {
							checkCurrentPieceList (currentInfo.Name, boolValue);
						}

						if (!isAccessoriesCategory) {
							if (!currentInfo.ignoreDisableSameTypeName) {
								GameObject mainCharacterBaseInfoObject = getMainCharacterBaseInfoObjectByTypeName (currentInfo.typeName);

								if (mainCharacterBaseInfoObject != null) {
									if (mainCharacterBaseInfoObject != currentInfo.elementObject) {
										mainCharacterBaseInfoObject.SetActive (false);
									}
								}
							}
						}
					}
				} else {
					for (int j = 0; j < currentCharacterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
						characterObjectInfo currentInfo = currentCharacterObjectTypeInfoList [i].characterObjectInfoList [j];

						bool isCurrentOption = (currentInfo.Name.Equals (newCharacterCustomizationInfo.Name));

						if (isCurrentOption) {
							if (!keepAllCharacterMeshesDisabledActive || !boolValue) {
								if (currentInfo.elementObject.activeSelf != boolValue) {
									currentInfo.elementObject.SetActive (boolValue);
								}
							}

							if (isAccessoriesCategory) {
								checkCurrentAccessoriesList (currentInfo.Name, boolValue);
							} else {
								checkCurrentPieceList (currentInfo.Name, boolValue);
							}

							if (!isAccessoriesCategory) {
								if (!currentInfo.ignoreDisableSameTypeName) {
									GameObject mainCharacterBaseInfoObject = getMainCharacterBaseInfoObjectByTypeName (currentInfo.typeName);

									if (mainCharacterBaseInfoObject != null) {
										if (mainCharacterBaseInfoObject != currentInfo.elementObject) {
											mainCharacterBaseInfoObject.SetActive (false);
										}
									}
								}
							}
						}
					}
				}
			}
		} else if (newCharacterCustomizationInfo.typeName.Equals (blendShapesCategoryName)) {
			setBlendShapeValue (newCharacterCustomizationInfo.Name, newCharacterCustomizationInfo.floatValue, 
				newCharacterCustomizationInfo.useRandomFloatValue, -1);
		}

		if (isAccessoriesCategory) {
			checkDisableAccessoryByNameAndCategory (newCharacterCustomizationInfo.categoryName, newCharacterCustomizationInfo.Name);
		}
	}

	void disableAndClearAccessoriesList ()
	{
		for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
			for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

				if (currentAccessoriesList.Contains (currentAccessoryInfo.Name)) {
					if (currentAccessoryInfo.elementObject.activeSelf) {
						currentAccessoryInfo.elementObject.SetActive (false);
					}
				}
			}
		}

		currentAccessoriesList.Clear ();
	}

	void disableAndClearAllCharacterObjectList ()
	{
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentPiecesList.Contains (currentInfo.Name)) {			
					if (currentInfo.elementObject.activeSelf) {
						currentInfo.elementObject.SetActive (false);
					}

					checkCurrentPieceList (currentInfo.Name, false);
				}
			}
		}
	}

	GameObject getMainCharacterBaseInfoObjectByTypeName (string typeName)
	{
//		print ("checking for " + typeName);

		int characterObjectInfoListCount = mainCharacterBaseInfo.characterObjectInfoList.Count;

		for (int i = 0; i < characterObjectInfoListCount; i++) {
			characterObjectInfo currentCharacterObjectInfo = mainCharacterBaseInfo.characterObjectInfoList [i];

			if (currentCharacterObjectInfo.typeName.Equals (typeName)) {
				return currentCharacterObjectInfo.elementObject;
			}
		}

		return null;
	}


	//Main functions to enable or disable character objects
	public void checkDisableAccessoryByNameAndCategory (string categoryName, string objectName)
	{
		string categoryEquipped = "";

		int accessoryCategoryIndex = -1;
		int accessoryIndex = -1;

		for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
			for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

				if (characterAccessoriesTypeInfoList [i].categoryName.Equals (categoryName) && objectName.Equals (currentAccessoryInfo.Name)) {
					categoryEquipped = currentAccessoryInfo.typeName;

					accessoryCategoryIndex = i;

					accessoryIndex = j;
				}
			}
		}

		if (categoryEquipped != "") {
			for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
				for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
					characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

					if (currentPiecesList.Contains (currentInfo.Name) &&
					    categoryEquipped.Equals (currentInfo.typeName)) {

						if (currentInfo.disableAccesoriesSameTypeName) {
							characterObjectInfo currentAccessoryInfoToDisable = characterAccessoriesTypeInfoList [accessoryCategoryIndex].characterObjectInfoList [accessoryIndex];

							bool disableObjectResult = true;

							if (currentInfo.useCustomAccessoryTypeListToDisable) {
								if (!currentInfo.customAccessoryTypeListToDisable.Contains (characterAccessoriesTypeInfoList [accessoryCategoryIndex].categoryName)) {
									disableObjectResult = false;
								}
							}

							if (disableObjectResult) {
								if (currentAccessoryInfoToDisable.elementObject.activeSelf) {
									currentAccessoryInfoToDisable.elementObject.SetActive (false);
								}
							}
						}
					}
				}
			}
		}
	}

	public bool setObjectState (bool boolValue, string objectName, bool disableOtherObjects, 
	                            string categoryOfObjectsToDisable, bool equippingObject)
	{
		bool objectFound = false;

		if (showDebugPrint) {
			print ("sending toggle value " + objectName + " " + boolValue + " " + disableOtherObjects + " " + categoryOfObjectsToDisable);
		}

		if (disableOtherObjects) {
			for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {

				for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
					if (categoryOfObjectsToDisable.Equals (characterObjectTypeInfoList [i].categoryName)) {

						characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

						bool checkResult = false;

						if (keepAllCharacterMeshesDisabledActive) {
							if (currentPiecesList.Contains (currentInfo.Name)) {
								checkResult = true;
							}
						} else {
							if (currentInfo.elementObject.activeSelf != false) {
								checkResult = true;
							}
						}

						if (checkResult) {
							currentInfo.elementObject.SetActive (false);

							checkCurrentPieceList (currentInfo.Name, false);
						}
					}
				}
			}

			for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {

				for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
					if (categoryOfObjectsToDisable.Equals (characterAccessoriesTypeInfoList [i].categoryName)) {

						characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

						bool checkResult = false;

						if (keepAllCharacterMeshesDisabledActive) {
							if (currentAccessoriesList.Contains (currentAccessoryInfo.Name)) {
								checkResult = true;
							}
						} else {
							if (currentAccessoryInfo.elementObject.activeSelf != false) {
								checkResult = true;
							}
						}

						if (checkResult) {
							currentAccessoryInfo.elementObject.SetActive (false);

							checkCurrentAccessoriesList (currentAccessoryInfo.Name, false);
						}
					}
				}
			}
		}

		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				bool isCurrentOption = (currentInfo.Name.Equals (objectName));

				if (isCurrentOption) {
					bool checkResult = false;

					if (keepAllCharacterMeshesDisabledActive) {
						if (currentPiecesList.Contains (currentInfo.Name) || boolValue) {
							checkResult = true;
						}
					} else {
						if (equippingObject) {
							checkResult = true;
						} else {
							if (currentInfo.elementObject.activeSelf != boolValue) {
								checkResult = true;
							}
						}
					}

					if (checkResult) {
						if (!keepAllCharacterMeshesDisabledActive) {
							currentInfo.elementObject.SetActive (boolValue);
						}

						checkCurrentPieceList (currentInfo.Name, boolValue);
					}

					objectFound = true;
				}
			}
		}

		if (!objectFound) {
			for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
				for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
					characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

					bool isCurrentOption = (currentAccessoryInfo.Name.Equals (objectName));

					if (isCurrentOption) {
						bool checkResult = false;

						if (keepAllCharacterMeshesDisabledActive) {
							if (currentAccessoriesList.Contains (currentAccessoryInfo.Name) || boolValue) {
								checkResult = true;
							}
						} else {
							if (currentAccessoryInfo.elementObject.activeSelf != boolValue) {
								checkResult = true;
							}
						}

						if (checkResult) {
							if (!keepAllCharacterMeshesDisabledActive) {
								currentAccessoryInfo.elementObject.SetActive (boolValue);
							}

							checkCurrentAccessoriesList (currentAccessoryInfo.Name, boolValue);
						}

						objectFound = true;
					}
				}
			}
		}

		return objectFound;
	}

	public void setBlendShapeValue (string blendShapeName, float blendShapeValue, bool useRandomFloatValue, float sliderMaxValue)
	{
		for (int i = 0; i < characterBlendShapeTypeInfoList.Count; i++) {
			for (int j = 0; j < characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList.Count; j++) {
				characterBlendShapeInfo currentInfo = characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList [j];

				bool isCurrentOption = (currentInfo.Name.Equals (blendShapeName));

				if (showDebugPrint) {
					print (blendShapeName + " " + isCurrentOption + " " + blendShapeValue + " " + sliderMaxValue);
				}

				if (isCurrentOption) {

					Vector2 blendShapeLimits = currentInfo.blendShapeLimits;

					if (sliderMaxValue > -1) {
						blendShapeValue = (blendShapeLimits.y / sliderMaxValue) * blendShapeValue;
					}

					if (useRandomFloatValue) {
						blendShapeValue = Random.Range (blendShapeLimits.x, blendShapeLimits.y);
					} else {
						blendShapeValue = Mathf.Clamp (blendShapeValue, blendShapeLimits.x, blendShapeLimits.y);
					}

					characterBlendShapeTypeInfoList [i].mainSkinnedMeshRenderer.SetBlendShapeWeight (currentInfo.blendShapeIndex, blendShapeValue);
				}
			}
		}
	}

	public float getBlendShapeValue (string blendShapeName, float sliderMaxValue)
	{
		for (int i = 0; i < characterBlendShapeTypeInfoList.Count; i++) {
			for (int j = 0; j < characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList.Count; j++) {
				characterBlendShapeInfo currentInfo = characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList [j];

				bool isCurrentOption = (currentInfo.Name.Equals (blendShapeName));

				if (isCurrentOption) {

					float blendShapeValue = characterBlendShapeTypeInfoList [i].mainSkinnedMeshRenderer.GetBlendShapeWeight (currentInfo.blendShapeIndex);

					return blendShapeValue / (characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList [j].blendShapeLimits.y / sliderMaxValue);
				}
			}
		}

		return -1;
	}

	public List<string> getExtraTypeNameListObjectByName (string objectName)
	{
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {

			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentInfo.useExtraTypeNameList && currentInfo.Name.Equals (objectName)) {
					return currentInfo.extraTypeNameList;
				}
			}
		}

		for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {

			for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

				if (currentAccessoryInfo.useExtraTypeNameList && currentAccessoryInfo.Name.Equals (objectName)) {
					return currentAccessoryInfo.extraTypeNameList;
				}
			}
		}

		return null;
	}

	public void setEditActiveState (bool state)
	{
		
	}

	public List<temporalBlendshapeInfo> getTemporalBlendshapeInfoList ()
	{
		temporalBlendshapeInfoList.Clear ();

		for (int i = 0; i < characterBlendShapeTypeInfoList.Count; i++) {
			for (int j = 0; j < characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList.Count; j++) {
				characterBlendShapeInfo currentInfo = characterBlendShapeTypeInfoList [i].characterBlendShapeInfoList [j];

				float blendShapeValue = characterBlendShapeTypeInfoList [i].mainSkinnedMeshRenderer.GetBlendShapeWeight (currentInfo.blendShapeIndex);

				temporalBlendshapeInfo newTemporalBlendshapeInfo = new temporalBlendshapeInfo ();

				newTemporalBlendshapeInfo.Name = currentInfo.Name;
				newTemporalBlendshapeInfo.blendShapeValue = blendShapeValue;

				temporalBlendshapeInfoList.Add (newTemporalBlendshapeInfo);
			}
		}

		return temporalBlendshapeInfoList;
	}

	//Functions to enable or disable the head meshes if needed
	public void enableOrDisableHeadTemporaly (bool state)
	{
		if (headActiveTemporaly == state) {
			return;
		}

		headActiveTemporaly = state;

		GameObject mainCharacterBaseInfoObject = getMainCharacterBaseInfoObjectByTypeName (headObjectName);

		if (state) {
			headActivePreviously = isObjectActiveOnMainCharacterBaseInfo (headObjectName);

			if (!headActivePreviously) {
				if (mainCharacterBaseInfoObject != null && mainCharacterBaseInfoObject.activeSelf != state) {
					mainCharacterBaseInfoObject.SetActive (state);
				}
			}

		} else {
			if (!headActivePreviously) {
				if (mainCharacterBaseInfoObject != null && mainCharacterBaseInfoObject.activeSelf != state) {
					mainCharacterBaseInfoObject.SetActive (state);
				}
			}
		}

		bool setActiveStateValue = !state;

		if (isObjectTypeOnCurrentPiecesList (headObjectName)) {
			for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
				for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
					characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

					if (currentPiecesList.Contains (currentInfo.Name) && currentInfo.typeName.Equals (headObjectName)) {

						if (!keepAllCharacterMeshesDisabledActive || !setActiveStateValue) {
							if (currentInfo.elementObject.activeSelf != setActiveStateValue) {
								currentInfo.elementObject.SetActive (setActiveStateValue);
							}
						}
					}
				}
			}

			for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
				for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {

					characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

//					print ("checking accessory on head state " + characterAccessoriesTypeInfoList [i].Name + " " + currentInfo.Name
//					+ " " + (currentAccessoriesList.Contains (currentInfo.Name)));

					if (objectsOnHeadList.Contains (characterAccessoriesTypeInfoList [i].Name) &&
					    currentAccessoriesList.Contains (currentAccessoryInfo.Name)) {

//						print ("object located, setting state " + setActiveStateValue);

						if (!keepAllCharacterMeshesDisabledActive || setActiveStateValue) {
							if (currentAccessoryInfo.elementObject.activeSelf == setActiveStateValue) {
								bool setActiveResult = !setActiveStateValue;

								if (!keepAllCharacterMeshesDisabledActive) {
									if (!setActiveResult) {
										if (checkIfAccessoryCanRemainActive (currentAccessoryInfo.typeName, characterAccessoriesTypeInfoList [i].categoryName)) {
											setActiveResult = true;
										}
									}
								}

								currentAccessoryInfo.elementObject.SetActive (setActiveResult);
							}
						}
					}
				}
			}
		}
	}

	bool isObjectTypeOnCurrentPiecesList (string categoryName)
	{
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentPiecesList.Contains (currentInfo.Name) && currentInfo.typeName.Equals (categoryName)) {

					return true;
				}
			}
		}

		return false;
	}

	bool isObjectActiveOnMainCharacterBaseInfo (string categoryName)
	{
		int characterObjectInfoListCount = mainCharacterBaseInfo.characterObjectInfoList.Count;

		for (int i = 0; i < characterObjectInfoListCount; i++) {
			characterObjectInfo currentCharacterObjectInfo = mainCharacterBaseInfo.characterObjectInfoList [i];

			if (currentCharacterObjectInfo.typeName.Equals (categoryName)) {

				return currentCharacterObjectInfo.elementObject.activeSelf;
			}
		}

		return false;
	}

	bool checkIfAccessoryCanRemainActive (string typeName, string accessoryCategoryName)
	{
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentPiecesList.Contains (currentInfo.Name) && typeName.Equals (currentInfo.typeName)) {

					if (currentInfo.disableAccesoriesSameTypeName) {
						bool disableObjectResult = true;

						if (currentInfo.useCustomAccessoryTypeListToDisable) {
							if (!currentInfo.customAccessoryTypeListToDisable.Contains (accessoryCategoryName)) {
								disableObjectResult = false;
							}
						}

						return !disableObjectResult;
					}
				}
			}
		}

		return false;
	}


	//Functions to check the change of view to third or first person
	public void checkCameraViewToFirstOrThirdPerson (bool state)
	{
		if (!characterCustomizationEnabled) {
			return;
		}

		//If state is true, it is activating the first person view, so the meshes are disabled

		if (!state) {
			setMainCharacterBaseInfoFullActiveState (true);
		}

		//set main armor pieces state to show or hide all
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentPiecesList.Contains (currentInfo.Name)) {
					if (currentInfo.elementObject.activeSelf == state) {
						currentInfo.elementObject.SetActive (!state);
					}

					GameObject mainCharacterBaseInfoObject = getMainCharacterBaseInfoObjectByTypeName (currentInfo.typeName);

					if (mainCharacterBaseInfoObject != null) {
						//check base model parts to show or hide all
						if (!state) {
//							mainCharacterBaseInfoObject.SetActive (false);
//						} else {
							if (!currentInfo.ignoreDisableSameTypeName) {
							
								if (mainCharacterBaseInfoObject != currentInfo.elementObject) {
									mainCharacterBaseInfoObject.SetActive (false);
								}
							}
						}
					}
				}
			}
		}

		//check accessories which can be shown in regular state or hide all
		for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
			for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

				if (currentAccessoriesList.Contains (currentAccessoryInfo.Name)) {
					if (currentAccessoryInfo.elementObject.activeSelf == state) {
						currentAccessoryInfo.elementObject.SetActive (!state);

						if (!state) {
							checkDisableAccessoryByNameAndCategory (characterAccessoriesTypeInfoList [i].categoryName, currentAccessoryInfo.Name);
						}
					}
				}
			}
		}
			
		if (state) {
			setMainCharacterBaseInfoFullActiveState (false);
		}

		keepAllCharacterMeshesDisabledActive = state;
	}

	void setMainCharacterBaseInfoFullActiveState (bool state)
	{
		int characterObjectInfoListCount = mainCharacterBaseInfo.characterObjectInfoList.Count;

		for (int i = 0; i < characterObjectInfoListCount; i++) {
			characterObjectInfo currentCharacterObjectInfo = mainCharacterBaseInfo.characterObjectInfoList [i];

			if (currentCharacterObjectInfo.elementObject.activeSelf != state) {
				currentCharacterObjectInfo.elementObject.SetActive (state);
			}
		}
	}

	public void checkEquippedStateOnObject (bool state, string objectName, string categoryName)
	{
		bool disableAccesoriesSameTypeName = false;

		bool ignoreDisableSameTypeName = false;

		bool useCustomAccessoryTypeListToDisable = false;

		List<string> customAccessoryTypeListToDisable = new List<string> ();

		GameObject currentObjectEquipped = null;

		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (objectName.Contains (currentInfo.Name) && categoryName.Equals (currentInfo.typeName)) {

					disableAccesoriesSameTypeName = currentInfo.disableAccesoriesSameTypeName;

					useCustomAccessoryTypeListToDisable = currentInfo.useCustomAccessoryTypeListToDisable;

					if (useCustomAccessoryTypeListToDisable) {
						customAccessoryTypeListToDisable = currentInfo.customAccessoryTypeListToDisable;
					}

					ignoreDisableSameTypeName = currentInfo.ignoreDisableSameTypeName;

					currentObjectEquipped = currentInfo.elementObject;
				}
			}
		}

//		print (disableAccesoriesSameTypeName + " " + state + " " + categoryName + " " + objectName
//		+ " " + ignoreDisableSameTypeName + " " + useCustomAccessoryTypeListToDisable);

		if (disableAccesoriesSameTypeName) {
			for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
				for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
					characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

					if (categoryName.Equals (currentAccessoryInfo.typeName) &&
					    currentAccessoriesList.Contains (currentAccessoryInfo.Name)) {

						if (disableAccesoriesSameTypeName) {

							bool disableObjectResult = true;

							if (state) {
								if (useCustomAccessoryTypeListToDisable) {
									if (customAccessoryTypeListToDisable.Count == 0) {
										disableObjectResult = false;
									} else {
										if (!customAccessoryTypeListToDisable.Contains (characterAccessoriesTypeInfoList [i].categoryName)) {
											disableObjectResult = false;
										}
									}
								}
							}

							if (disableObjectResult) {
								if (currentAccessoryInfo.elementObject.activeSelf == state) {
									if (!keepAllCharacterMeshesDisabledActive || state) {
										currentAccessoryInfo.elementObject.SetActive (!state);
									}
								}
							}
						}
					}
				}
			}
		}

		bool baseModelObjectState = state;

		if (ignoreDisableSameTypeName && state) {
			baseModelObjectState = !state;
		}

		int characterObjectInfoListCount = mainCharacterBaseInfo.characterObjectInfoList.Count;

		for (int i = 0; i < characterObjectInfoListCount; i++) {
			characterObjectInfo currentCharacterObjectInfo = mainCharacterBaseInfo.characterObjectInfoList [i];

			if (currentCharacterObjectInfo.typeName.Equals (categoryName)) {

				if (currentCharacterObjectInfo.elementObject.activeSelf == baseModelObjectState) {
					if (!keepAllCharacterMeshesDisabledActive || baseModelObjectState) {
						bool canSetActiveState = true;

						//if the same object equipped is the same that is on the base model, ignore to disable its mesh
						if (baseModelObjectState) {
							if (currentCharacterObjectInfo.elementObject == currentObjectEquipped) {
								canSetActiveState = false;
							}
						}

						if (canSetActiveState) {
							currentCharacterObjectInfo.elementObject.SetActive (!baseModelObjectState);
						}
					}
				}

				if (showDebugPrint) {
					print ("checking to enable or disable base mesh " + baseModelObjectState + "  " + categoryName + "  " + objectName);
				}
			}
		}

		List<string> extraTypeNameList = getExtraTypeNameListObjectByName (objectName);

		if (extraTypeNameList != null && extraTypeNameList.Count > 0) {
			for (int i = 0; i < characterObjectInfoListCount; i++) {
				characterObjectInfo currentCharacterObjectInfo = mainCharacterBaseInfo.characterObjectInfoList [i];

				if (extraTypeNameList.Contains (currentCharacterObjectInfo.typeName)) {

					if (currentCharacterObjectInfo.elementObject.activeSelf == baseModelObjectState) {
						if (!keepAllCharacterMeshesDisabledActive || baseModelObjectState) {
							currentCharacterObjectInfo.elementObject.SetActive (!baseModelObjectState);
						}
					}

					if (showDebugPrint) {
						print ("checking to enable or disable base mesh " + baseModelObjectState + "  " + categoryName + "  " + objectName);
					}
				}
			}
		}
	}


	//FUNCTIONS USED TO INITIALIZE THE ELEMENTS ON LOAD GAME
	public void setBlendshapeList (List<temporalBlendshapeInfo> newTemporalBlendshapeInfoList)
	{
		for (int i = 0; i < newTemporalBlendshapeInfoList.Count; i++) {

			setBlendShapeValue (newTemporalBlendshapeInfoList [i].Name, newTemporalBlendshapeInfoList [i].blendShapeValue, false, -1);
		}
	}

	public List<string> getCurrentPiecesList ()
	{
		return currentPiecesList;
	}

	public List<string> getCurrentAccessoriesList ()
	{
		return currentAccessoriesList;
	}

	public void setCurrentAccessoriesList (List<string> newTemporalAccessoriesList)
	{
		currentAccessoriesList = newTemporalAccessoriesList;

		for (int i = 0; i < characterAccessoriesTypeInfoList.Count; i++) {
			for (int j = 0; j < characterAccessoriesTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentAccessoryInfo = characterAccessoriesTypeInfoList [i].characterObjectInfoList [j];

				if (currentAccessoriesList.Contains (currentAccessoryInfo.Name)) {
					if (!currentAccessoryInfo.elementObject.activeSelf) {
						if (!keepAllCharacterMeshesDisabledActive) {
							currentAccessoryInfo.elementObject.SetActive (true);
						}

						checkDisableAccessoryByNameAndCategory (characterAccessoriesTypeInfoList [i].categoryName, currentAccessoryInfo.Name);
					}
				}
			}
		}
	}


	//Other functions
	public string getArmorClothPieceByName (string categoryName)
	{
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentPiecesList.Contains (currentInfo.Name) && currentInfo.typeName.Equals (categoryName)) {

					return currentInfo.Name;
				}
			}
		}

		return "";
	}

	public string getArmorClothCategoryByName (string objectName)
	{
		for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
			for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
				characterObjectInfo currentInfo = characterObjectTypeInfoList [i].characterObjectInfoList [j];

				if (currentInfo.Name.Equals (objectName)) {

					return currentInfo.typeName;
				}
			}
		}

		return "";
	}

	public bool checkIfObjectAlreadyOnCurrentPiecesList (string pieceName)
	{
		return currentPiecesList.Contains (pieceName);
	}

	void checkCurrentPieceList (string pieceName, bool checkToAdd)
	{
		if (checkToAdd) {
			if (!currentPiecesList.Contains (pieceName)) {
				currentPiecesList.Add (pieceName);
			}
		} else {
			if (currentPiecesList.Contains (pieceName)) {
				currentPiecesList.Remove (pieceName);
			}
		}
	}

	void checkCurrentAccessoriesList (string pieceName, bool checkToAdd)
	{
		if (checkToAdd) {
			if (!currentAccessoriesList.Contains (pieceName)) {
				currentAccessoriesList.Add (pieceName);
			}
		} else {
			if (currentAccessoriesList.Contains (pieceName)) {
				currentAccessoriesList.Remove (pieceName);
			}
		}
	}

	//Durability Functions
	public void checkEventOnDurabilityAffected (string bodyPartName)
	{
		if (useEventOnDurabilityAffectedEnabled) {
			if (showDebugPrint) {
				print ("checking event on durability on body part " + bodyPartName);
			}

			for (int i = 0; i < durabilityPartInfoList.Count; i++) {
				if (durabilityPartInfoList [i].Name.Equals (bodyPartName)) {
					durabilityPartInfoList [i].eventOnDurabilityAffectedEnabled.Invoke ();

					return;
				}
			}
		}
	}


	//Editor functions
	public void createPieceMeshesObjectsFromSetByName ()
	{
		if (armorSetToCreatePieceMeshesObjects != "") {
			for (int i = 0; i < characterObjectTypeInfoList.Count; i++) {
				if (characterObjectTypeInfoList [i].Name.Equals (armorSetToCreatePieceMeshesObjects)) {
					print (armorSetToCreatePieceMeshesObjects + " located ");

					for (int j = 0; j < characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
						GKC_Utils.createPieceMeshesObjectsFromSetByName (characterObjectTypeInfoList [i].characterObjectInfoList [j].elementObject,
							characterObjectTypeInfoList [i].characterObjectInfoList [j].Name);
					}

					armorSetToCreatePieceMeshesObjects = "";

					return;
				}
			}
		}
	}

	public void checkCameraViewToFirstOrThirdPersonOnEditor (bool state)
	{
		keepAllCharacterMeshesDisabledActive = state;

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Character Customization Manager Values", gameObject);
	}

	[System.Serializable]
	public class characterObjectInfo
	{
		public string Name;

		public string typeName;

		public GameObject elementObject;

		[Space]

		public bool ignoreDisableSameTypeName;

		[Space]

		public bool disableAccesoriesSameTypeName;

		public bool useCustomAccessoryTypeListToDisable;

		public List<string> customAccessoryTypeListToDisable = new List<string> ();

		[Space]
		public bool useExtraTypeNameList;

		public List<string> extraTypeNameList = new List<string> ();
	}

	[System.Serializable]
	public class characterObjectTypeInfo
	{
		public string Name;

		public string categoryName;

		[Space]

		public List<characterObjectInfo> characterObjectInfoList = new List<characterObjectInfo> ();
	}

	[System.Serializable]
	public class characterBlendShapeInfo
	{
		public string Name;

		public int blendShapeIndex;
		public float blendShapeValue;
		public Vector2 blendShapeLimits;
	}

	[System.Serializable]
	public class characterBlendShapeTypeInfo
	{
		public string Name;
		public SkinnedMeshRenderer mainSkinnedMeshRenderer;

		[Space]

		public List<characterBlendShapeInfo> characterBlendShapeInfoList = new List<characterBlendShapeInfo> ();
	}

	[System.Serializable]
	public class temporalBlendshapeInfo
	{
		public string Name;

		public float blendShapeValue;
	}


	[System.Serializable]
	public class durabilityPartInfo
	{
		public string Name;

		[Space]

		public UnityEvent eventOnDurabilityAffectedEnabled;
	}
}