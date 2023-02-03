using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class replaceMaterialSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool replaceMaterialsEnabled = true;

	[Space]
	[Header ("Main Settings")]
	[Space]

	public GameObject objectToUse;
	public Material materialToReplace;

	public List<renderInfo> renderInfoList = new List<renderInfo> ();

	Renderer currentRenderer;

	renderInfo currentRenderInfo;


	public void setMaterialState (bool state)
	{
		if (!replaceMaterialsEnabled) {
			return;
		}

		checkObjectMaterials ();

		int renderInfoListCount = renderInfoList.Count;

		for (int i = 0; i < renderInfoListCount; i++) {
			currentRenderInfo = renderInfoList [i];

			currentRenderer = currentRenderInfo.mainRenderer;

			int materialsLength = currentRenderer.materials.Length;

			Material[] allMats = currentRenderer.materials;

			if (state) {
				for (int m = 0; m < materialsLength; m++) {
					allMats [m] = materialToReplace;
				}
			} else {
				int materialListCount = currentRenderInfo.materialList.Count;

				for (int m = 0; m < materialListCount; m++) {
					allMats [m] = currentRenderInfo.materialList [m];
				}
			}

			currentRenderer.materials = allMats;
		}
	}

	public void checkObjectMaterials ()
	{
		if (renderInfoList.Count == 0) {
			Component[] components = objectToUse.GetComponentsInChildren (typeof(Renderer));
			foreach (Renderer child in components) {
				if (!child.GetComponent<ParticleSystem> ()) {
					renderInfo newRenderInfo = new renderInfo ();

					newRenderInfo.mainRenderer = child;

					int materialsLength = child.materials.Length;

					for (int i = 0; i < materialsLength; i++) {
						newRenderInfo.materialList.Add (child.materials [i]);
					}

					renderInfoList.Add (newRenderInfo);
				}
			}
		}
	}

	[System.Serializable]
	public class renderInfo
	{
		public Renderer mainRenderer;
		public List<Material> materialList = new List<Material> ();
	}
}
