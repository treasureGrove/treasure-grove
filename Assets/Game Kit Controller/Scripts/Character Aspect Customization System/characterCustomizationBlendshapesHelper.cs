using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterCustomizationBlendshapesHelper : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public GameObject playerGameObject;

	public characterCustomizationManager mainCharacterCustomizationManager;

	[Space]
	[Header ("Debug Temporal BlendShapes")]
	[Space]

	public List<characterCustomizationManager.temporalBlendshapeInfo> temporalBlendshapeInfoList = new List<characterCustomizationManager.temporalBlendshapeInfo> ();

	[Space]
	[Header ("Debug Temporal Accessories")]
	[Space]

	public List<string> temporalCurrentAccessoriesList = new List<string> ();


	bool componentLocated;


	public void storeBlendshapesCustomization ()
	{
		if (!componentLocated) {
			checkMainCHaracterCustomizatationManager ();
		}

		if (componentLocated) {
			temporalBlendshapeInfoList = mainCharacterCustomizationManager.getTemporalBlendshapeInfoList ();

			temporalCurrentAccessoriesList = mainCharacterCustomizationManager.getCurrentAccessoriesList ();
		}
	}

	public void setBlendshapeList ()
	{
		if (!componentLocated) {
			checkMainCHaracterCustomizatationManager ();
		}
			
		if (componentLocated) {
			mainCharacterCustomizationManager.setBlendshapeList (temporalBlendshapeInfoList);

			mainCharacterCustomizationManager.setCurrentAccessoriesList (temporalCurrentAccessoriesList);
		}
	}

	public void checkMainCHaracterCustomizatationManager ()
	{
		if (!componentLocated) {
			if (mainCharacterCustomizationManager == null && playerGameObject != null) {
				mainCharacterCustomizationManager = playerGameObject.GetComponentInChildren<characterCustomizationManager> ();
			}

			componentLocated = mainCharacterCustomizationManager != null;
		}
	}
}
