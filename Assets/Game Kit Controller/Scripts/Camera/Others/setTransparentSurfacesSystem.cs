using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setTransparentSurfacesSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkSurfaceEnabled = true;

	public Shader transparentShader;
	public float alphaBlendSpeed = 5;
	public float alphaTransparency = 0.2f;

	public bool useAlphaColor = true;
	public string transparentAmountName = "_TransparentAmount";

	[Space]
	[Header ("Debug")]
	[Space]

	public List<surfaceInfo> surfaceInfoList = new List<surfaceInfo> ();
	public List<GameObject> surfaceGameObjectList = new List<GameObject> ();

	surfaceInfo newSurfaceInfo;
	materialInfo newMaterialInfo;

	int transparentAmountID = -1;

	public void addNewSurface (GameObject newSurface, Shader customShader)
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		newSurfaceInfo = new surfaceInfo ();
		newSurfaceInfo.surfaceGameObject = newSurface;

		Component[] components = newSurfaceInfo.surfaceGameObject.GetComponentsInChildren (typeof(Renderer));
		foreach (Renderer child in components) {
			if (child.material.shader != null) {
				int materialsLength = child.materials.Length;

				for (int j = 0; j < materialsLength; j++) {
					Material currentMaterial = child.materials [j];

					newMaterialInfo = new materialInfo ();
					newMaterialInfo.surfaceMaterial = currentMaterial;

					Color alpha = currentMaterial.color;

					newMaterialInfo.originalAlphaColor = alpha.a;

					newSurfaceInfo.materialInfoList.Add (newMaterialInfo);
					newSurfaceInfo.originalShader.Add (currentMaterial.shader);

					if (customShader != null) {
						currentMaterial.shader = customShader;

						newSurfaceInfo.customShader = customShader;
					} else {
						currentMaterial.shader = transparentShader;

						newSurfaceInfo.customShader = transparentShader;
					}

					newSurfaceInfo.materialsAmount++;
				}
			}
		}

		setSurfaceTransparent (newSurfaceInfo);
	
		surfaceInfoList.Add (newSurfaceInfo);

		surfaceGameObjectList.Add (newSurfaceInfo.surfaceGameObject);
	}

	public bool listContainsSurface (GameObject surfaceToCheck)
	{
		if (!checkSurfaceEnabled) {
			return false;
		}

		if (surfaceGameObjectList.Contains (surfaceToCheck)) {
			return true;
		}

		return false;
	}

	public void setSurfaceToRegular (int surfaceIndex, bool removeSurfaceAtEnd)
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		surfaceInfo currentSurfaceToCheck = surfaceInfoList [surfaceIndex];

		currentSurfaceToCheck.changingToOriginalActive = true;
		currentSurfaceToCheck.currentMaterialsChangedAmount = 0;
		currentSurfaceToCheck.changingToTransparentActive = false;

		currentSurfaceToCheck.changingToOriginalTemporaly = !removeSurfaceAtEnd;

		for (int j = 0; j < currentSurfaceToCheck.materialInfoList.Count; j++) {
			setAlphaValue (currentSurfaceToCheck, j, currentSurfaceToCheck.materialInfoList [j].surfaceMaterial, 
				false, currentSurfaceToCheck.originalShader [j], currentSurfaceToCheck.materialInfoList [j].originalAlphaColor, removeSurfaceAtEnd);
		}
	}

	public void checkSurfaceToSetTransparentAgain (GameObject surfaceToCheck)
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		int surfaceIndex = surfaceGameObjectList.IndexOf (surfaceToCheck);

		if (surfaceIndex > -1) {
			surfaceInfo currentSurfaceToCheck = surfaceInfoList [surfaceIndex];

			if (currentSurfaceToCheck.changingToOriginalActive && !currentSurfaceToCheck.changingToOriginalTemporaly) {
//				print ("changing to original, stopping");
				setSurfaceTransparent (currentSurfaceToCheck);
			}
		}
	}

	public void setSurfaceTransparent (surfaceInfo currentSurfaceToCheck)
	{
		currentSurfaceToCheck.changingToOriginalActive = false;
		currentSurfaceToCheck.currentMaterialsChangedAmount = 0;
		currentSurfaceToCheck.changingToTransparentActive = true;

		for (int j = 0; j < currentSurfaceToCheck.materialInfoList.Count; j++) {
			setAlphaValue (currentSurfaceToCheck, j, currentSurfaceToCheck.materialInfoList [j].surfaceMaterial, 
				true, currentSurfaceToCheck.originalShader [j], currentSurfaceToCheck.materialInfoList [j].originalAlphaColor, false);
		}
	}


	public void setAlphaValue (surfaceInfo currentSurfaceToCheck, int surfaceIndex, Material currentMaterial, 
	                           bool changingToTransparent, Shader originalShader, float originalAlphaColor, bool removeSurfaceAtEnd)
	{
		if (currentSurfaceToCheck.materialInfoList [surfaceIndex].alphaBlendCoroutine != null) {
			StopCoroutine (currentSurfaceToCheck.materialInfoList [surfaceIndex].alphaBlendCoroutine);
		}

		currentSurfaceToCheck.materialInfoList [surfaceIndex].alphaBlendCoroutine = 
			StartCoroutine (setAlphaValueCoroutine (currentSurfaceToCheck, currentMaterial,
			changingToTransparent, originalShader, originalAlphaColor, removeSurfaceAtEnd));
	}

	IEnumerator setAlphaValueCoroutine (surfaceInfo currentSurfaceToCheck, Material currentMaterial, bool changingToTransparent, 
	                                    Shader originalShader, float originalAlphaColor, bool removeSurfaceAtEnd)
	{
		float targetValue = originalAlphaColor;
		if (changingToTransparent) {
			targetValue = alphaTransparency;

			if (currentSurfaceToCheck.changingToOriginalTemporaly) {
				currentMaterial.shader = currentSurfaceToCheck.customShader;
			}
		}

		if (useAlphaColor) {
			Color alpha = currentMaterial.color;

			while (alpha.a != targetValue) {
				alpha.a = Mathf.MoveTowards (alpha.a, targetValue, Time.deltaTime * alphaBlendSpeed);
				currentMaterial.color = alpha;

				yield return null;
			}
		} else {
			if (transparentAmountID == -1) {
				transparentAmountID = Shader.PropertyToID (transparentAmountName);
			}

			float currentTransparentAmount = currentMaterial.GetFloat (transparentAmountID);

			while (currentTransparentAmount != targetValue) {
				currentTransparentAmount = Mathf.MoveTowards (currentTransparentAmount, targetValue, Time.deltaTime * alphaBlendSpeed);
				currentMaterial.SetFloat (transparentAmountID, currentTransparentAmount);

				yield return null;
			}
		}

		if (!changingToTransparent) {
			currentMaterial.shader = originalShader;
		}

		if (currentSurfaceToCheck.changingToOriginalActive) {
			currentSurfaceToCheck.currentMaterialsChangedAmount++;

			if (removeSurfaceAtEnd) {
				if (currentSurfaceToCheck.currentMaterialsChangedAmount == currentSurfaceToCheck.materialsAmount) {
					surfaceInfoList.Remove (currentSurfaceToCheck);

					surfaceGameObjectList.Remove (currentSurfaceToCheck.surfaceGameObject);
				}
			}
		} else {
			currentSurfaceToCheck.currentMaterialsChangedAmount++;

			if (currentSurfaceToCheck.currentMaterialsChangedAmount == currentSurfaceToCheck.materialsAmount) {
				currentSurfaceToCheck.changingToTransparentActive = false;

				currentSurfaceToCheck.changingToOriginalTemporaly = false;
			}
		}
	}

	public void checkSurfacesToRemove ()
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		for (int i = 0; i < surfaceInfoList.Count; i++) {
			surfaceInfo currentSurfaceToCheck = surfaceInfoList [i];
	
			if (currentSurfaceToCheck.numberOfPlayersFound == 0) {
				setSurfaceToRegular (i, true);
			}
		}
	}

	public void changeSurfacesToTransparentOrRegularTemporaly (int playerID, int surfaceIndex, bool setTransparentSurface)
	{
		if (surfaceInfoList [surfaceIndex].playerIDs.Contains (playerID)) {
			if (setTransparentSurface) {
//				print ("transparent");
				setSurfaceTransparent (surfaceInfoList [surfaceIndex]);
			} else {
//				print ("regular");
				setSurfaceToRegular (surfaceIndex, false);
			}
		}
	}

	public void addPlayerIDToSurface (int playerID, GameObject surfaceToCheck)
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		for (int i = 0; i < surfaceInfoList.Count; i++) {
			if (surfaceInfoList [i].surfaceGameObject == surfaceToCheck) {
				if (!surfaceInfoList [i].playerIDs.Contains (playerID)) {
					surfaceInfoList [i].playerIDs.Add (playerID);

					surfaceInfoList [i].numberOfPlayersFound++;

					return;
				}
			}
		}
	}

	public void removePlayerIDToSurface (int playerID, int surfaceIndex)
	{
		if (!checkSurfaceEnabled) {
			return;
		}

		if (surfaceInfoList [surfaceIndex].playerIDs.Contains (playerID)) {
			surfaceInfoList [surfaceIndex].playerIDs.Remove (playerID);

			surfaceInfoList [surfaceIndex].numberOfPlayersFound--;
		}
	}

	[System.Serializable]
	public class surfaceInfo
	{
		public GameObject surfaceGameObject;
		public List<Shader> originalShader = new List<Shader> ();
		public List<materialInfo> materialInfoList = new List<materialInfo> ();
		public List<int> playerIDs = new List<int> ();
		public int numberOfPlayersFound;

		public bool changingToTransparentActive;
		public bool changingToOriginalActive;
		public bool changingToOriginalTemporaly;

		public int materialsAmount;
		public int currentMaterialsChangedAmount;

		public Shader customShader;
	}

	[System.Serializable]
	public class materialInfo
	{
		public Material surfaceMaterial;
		public Coroutine alphaBlendCoroutine;
		public float originalAlphaColor;
	}
}
