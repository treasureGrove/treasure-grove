using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCullingSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cullingSystemEnabled = true;

	public float minDistanceToApplyShader = 1;
	public float minCullingShaderValue = 0.2f;
	public float slerpCullingSpeed = 1;

	public float stippleSize;

	public string transparentAmountName = "_TransparentAmount";

	public bool setStippleSize;
	public string stippleSizeName = "_StippleSize";

	[Space]
	[Header ("Player GameObject List")]
	[Space]

	public List<GameObject> playerGameObjectList = new List<GameObject> ();
	public List<GameObject> playerGameObjectWithChildList = new List<GameObject> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool pauseCullingState;
	public float cullingAmountToApply;
	public bool cullingActive;
	public float currentDistance;
	public float currentCullingValue;
	public bool materialsChangedChecked;

	[Space]
	[Header ("Components")]
	[Space]

	public Shader cullingShader;
	public Transform playerTransform;
	public Transform cameraTransform;
	public playerCamera mainPlayerCamera;

	public List<Renderer> rendererParts = new List<Renderer> ();
	public List<Shader> originalShader = new List<Shader> ();
	public List<Material> materialList = new List<Material> ();

	float previousCullingValue;

	bool materialsStored;

	int transparentAmountID;

	int stippleSizeID;

	float previousStippleSize = -1;

	int materialsCount;

	Material temporalMaterial;

	void Start ()
	{
		if (rendererParts.Count == 0) {
			storePlayerRenderer ();
		}

		transparentAmountID = Shader.PropertyToID (transparentAmountName);

		stippleSizeID = Shader.PropertyToID (stippleSizeName);
	}

	void FixedUpdate ()
	{
		if (!cullingSystemEnabled) {
			return;
		}

		currentDistance = GKC_Utils.distance (playerTransform.position, cameraTransform.position);

		if (!pauseCullingState && currentDistance < minDistanceToApplyShader) {
			cullingAmountToApply = currentDistance / minDistanceToApplyShader;

			cullingActive = true;
		} else {
			cullingActive = false;

			cullingAmountToApply = 1;
		}

		if (cullingActive) {
			if (mainPlayerCamera.isFirstPersonActive ()) {
				cullingActive = false;
			} else {
				if (mainPlayerCamera.isPlayerAiming () || !mainPlayerCamera.isCameraTypeFree ()) {
					cullingAmountToApply = 1;
				}
			}
		}
			
		if (materialsChangedChecked != cullingActive) {
			materialsChangedChecked = cullingActive;

			if (!materialsStored) {
				getMaterialList ();

				materialsStored = true;
			}

			materialsCount = materialList.Count;

			for (int j = 0; j < materialsCount; j++) {
				temporalMaterial = materialList [j];

				if (cullingActive) {
					temporalMaterial.shader = cullingShader;

					if (setStippleSize) {
						if (stippleSize != previousStippleSize) {
							previousStippleSize = stippleSize;

							temporalMaterial.SetFloat (stippleSizeID, stippleSize);
						}
					}
				} else {
					temporalMaterial.shader = originalShader [j];
				}
			}
		}
			
		currentCullingValue = Mathf.Lerp (currentCullingValue, cullingAmountToApply, Time.deltaTime * slerpCullingSpeed);

		if (currentCullingValue <= 1) {
			if (Mathf.Abs (currentCullingValue - previousCullingValue) > 0.01f) {

				materialsCount = materialList.Count;

				for (int j = 0; j < materialsCount; j++) {
					materialList [j].SetFloat (transparentAmountID, currentCullingValue);
				}

				previousCullingValue = currentCullingValue;
			}
		}
	}

	public void setPauseCullingState (bool state)
	{
		pauseCullingState = state;
	}

	public void getMaterialList ()
	{
		if (materialList.Count == 0) {
			int rendererPartsCount = rendererParts.Count;

			for (int i = 0; i < rendererPartsCount; i++) {
				Renderer currentRenderer = rendererParts [i];

				if (currentRenderer != null) {
					
					if (currentRenderer.material.shader != null) {
						int materialsLength = currentRenderer.materials.Length;

						for (int j = 0; j < materialsLength; j++) {

							Material currentMaterial = currentRenderer.materials [j];
							
							if (currentMaterial != null) {
								
								materialList.Add (currentMaterial);

								originalShader.Add (currentMaterial.shader);
							}
						}
					}
				}
			}
		}
	}

	public void clearRendererList ()
	{
		rendererParts.Clear ();

		originalShader.Clear ();
	}

	public void storePlayerRenderer ()
	{
		clearRendererList ();

		for (int i = 0; i < playerGameObjectList.Count; i++) {
			GameObject currentObject = playerGameObjectList [i];

			if (currentObject != null) {
				Renderer currentRenderer = currentObject.GetComponent<Renderer> ();

				rendererParts.Add (currentRenderer);
			}
		}

		for (int i = 0; i < playerGameObjectWithChildList.Count; i++) {
			GameObject currentObject = playerGameObjectWithChildList [i];

			if (currentObject != null) {
				Component[] components = currentObject.GetComponentsInChildren (typeof(Renderer));
				foreach (Renderer child in components) {

					rendererParts.Add (child);
				}
			}
		}

		print ("Total amount of " + rendererParts.Count + " render found");
	}

	public void storePlayerRendererFromEditor ()
	{
		storePlayerRenderer ();

		updateComponent ();
	}

	public void clearRendererListFromEditor ()
	{
		clearRendererList ();

		updateComponent ();
	}

	public void setCullingSystemEnabledState (bool state)
	{
		cullingSystemEnabled = state;
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
