using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outlineObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useOutlineEnabled = true;

	public string shaderOutlineWidthName = "_Outline";
	public string shaderOutlineColorName = "_OutlineColor";

	[Space]
	[Header ("Custom Outline Settings")]
	[Space]

	public bool useCustomOutlineValues;
	public float customOutlineWidth = 0.05f;
	public Color customOutlineColor = Color.yellow;
	public bool useCustomShader;
	public Shader customShader;

	public float customActiveDuration = 2;

	[Space]
	[Header ("Transparency Settings")]
	[Space]

	public bool useTransparencyActive = true;
	public bool transparencyActive;

	public bool useCustomTransparencyValues;
	public float customAlphaTransparency;

	[Space]
	[Header ("Others Settings")]
	[Space]

	public GameObject meshParent;

	[Space]
	[Space]

	public bool ignoreParticles;
	public bool ignoreLineRenderer;

	[Space]
	[Space]

	public bool useMeshesToIgnore;
	public List<Transform> meshesToIgnore = new List<Transform> ();

	[Space]
	[Space]

	public bool useCustomMeshList;
	public List<Transform> customMeshList = new List<Transform> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool outlineActive;
	public bool renderElementsStored;

	public bool outlineTemporaryActive;

	List<Renderer> rendererParts = new List<Renderer> ();
	List<Shader> originalShader = new List<Shader> ();

	Shader currentOutlineShader;
	float currentOutlineWidht;
	Color currentOutlieColor;

	List<Transform> objectsToIgnoreChildren = new List<Transform> ();

	List<playerController> playerControllerList = new List<playerController> ();

	bool meshesToIgnoreConfigured;

	int shaderOutlineWidthID = -1;
	int shaderOutlineColorID = -1;

	Coroutine temporalStateCoroutine;

	public void enableCustomOutlineStateTemporaly ()
	{
		if (gameObject.activeSelf) {
			stopUpdateTemporalStateCoroutine ();

			temporalStateCoroutine = StartCoroutine (updateTemporalStateCoroutine ());
		}
	}

	public void stopUpdateTemporalStateCoroutine ()
	{
		if (temporalStateCoroutine != null) {
			StopCoroutine (temporalStateCoroutine);
		}
	}

	IEnumerator updateTemporalStateCoroutine ()
	{
		if (!outlineTemporaryActive) {
			enableCustomOutlineState ();
		}

		outlineTemporaryActive = true;

		yield return new WaitForSecondsRealtime (customActiveDuration);

		disableCustomOutlineState ();

		outlineTemporaryActive = false;
	}

	public void enableCustomOutlineState ()
	{
		setOutlineState (true, customShader, customOutlineWidth, customOutlineColor, null);
	}

	public void disableCustomOutlineState ()
	{
		setOutlineState (false, customShader, customOutlineWidth, customOutlineColor, null);
	}

	public void setOutlineState (bool state, Shader shaderToApply, float shaderOutlineWidth,
	                             Color shaderOutlineColor, playerController newPlayerToCheck)
	{
		outlineActive = state;

		if (!useOutlineEnabled) {
			return;
		}

		if (outlineActive) {
			if (useCustomShader) {
				shaderToApply = customShader;
			}

			storeRenderElements ();

			if (shaderOutlineWidthID == -1) {
				shaderOutlineWidthID = Shader.PropertyToID (shaderOutlineWidthName);
			}

			if (shaderOutlineColorID == -1) {
				shaderOutlineColorID = Shader.PropertyToID (shaderOutlineColorName);
			}

			int rendererPartsCount = rendererParts.Count;

			for (int i = 0; i < rendererPartsCount; i++) {
				Renderer currentRenderer = rendererParts [i];

				if (currentRenderer != null) {
					int materialsLength = currentRenderer.materials.Length;

					for (int j = 0; j < materialsLength; j++) {
						Material currentMaterial = currentRenderer.materials [j];

						currentMaterial.shader = shaderToApply;

						if (useCustomOutlineValues) {
							currentMaterial.SetFloat (shaderOutlineWidthID, customOutlineWidth);
							currentMaterial.SetColor (shaderOutlineColorID, customOutlineColor);
						} else {
							currentMaterial.SetFloat (shaderOutlineWidthID, shaderOutlineWidth);
							currentMaterial.SetColor (shaderOutlineColorID, shaderOutlineColor);
						}
					}
				}
			}

			currentOutlineShader = shaderToApply;
			currentOutlineWidht = shaderOutlineWidth;
			currentOutlieColor = shaderOutlineColor;

			if (newPlayerToCheck != null && !playerControllerList.Contains (newPlayerToCheck)) {
				playerControllerList.Add (newPlayerToCheck);
			}
		} else {
			if (newPlayerToCheck != null) {
				if (playerControllerList.Contains (newPlayerToCheck)) {
					playerControllerList.Remove (newPlayerToCheck);
				}
			}

			if (playerControllerList.Count == 0) {

				int rendererPartsCount = rendererParts.Count;

				for (int i = 0; i < rendererPartsCount; i++) {

					Renderer currentRenderer = rendererParts [i];

					if (currentRenderer != null) {
						int materialsLength = currentRenderer.materials.Length;

						for (int j = 0; j < materialsLength; j++) {

							Material currentMaterial = currentRenderer.materials [j];

							currentMaterial.shader = originalShader [i];
						}
					}
				}
			}
		}
	}

	public bool isOutlineActive ()
	{
		return outlineActive;
	}

	public GameObject getMeshParent ()
	{
		if (meshParent == null) {
			meshParent = gameObject;
		}

		return meshParent;
	}

	public void storeRenderElements ()
	{
		if (meshParent == null) {
			meshParent = gameObject;
		}

		if (useMeshesToIgnore && !meshesToIgnoreConfigured) {
			int meshesToIgnoreCount = meshesToIgnore.Count;

			for (int i = 0; i < meshesToIgnoreCount; i++) {
				Transform currentMeshToIgnore = meshesToIgnore [i];

				if (currentMeshToIgnore != null) {
					Component[] childrens = currentMeshToIgnore.GetComponentsInChildren (typeof(Transform));

					foreach (Transform child in childrens) {
						objectsToIgnoreChildren.Add (child);
					}
				}
			}

			meshesToIgnoreConfigured = true;
		}

		if (!renderElementsStored) {
			renderElementsStored = true;

			Component[] components = meshParent.GetComponentsInChildren (typeof(Renderer));
			foreach (Renderer child in components) {

				bool addObject = true;

				if (useCustomMeshList) {
					if (!customMeshList.Contains (child.transform)) {
						addObject = false;
					}
				}

				if (addObject) {
					if (!ignoreParticles || !child.GetComponent<ParticleSystem> ()) {
						if (!ignoreLineRenderer || !child.GetComponent<LineRenderer> ()) {
					
							if (child.material.shader != null) {
								if (!useMeshesToIgnore || !checkChildsObjectsToIgnore (child.transform)) {
									rendererParts.Add (child);

									int materialsLength = child.materials.Length;

									for (int i = 0; i < materialsLength; i++) {
										originalShader.Add (child.materials [i].shader);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public bool checkChildsObjectsToIgnore (Transform obj)
	{
		bool value = false;

		if (meshesToIgnore.Contains (obj) || objectsToIgnoreChildren.Contains (obj)) {
			value = true;

			return value;
		}

		return value;
	}

	public void disableOutlineAndRemoveUsers ()
	{
		playerControllerList.Clear ();

		setOutlineState (false, null, 0, Color.white, null);

		useOutlineEnabled = false;

		outlineActive = false;

		transparencyActive = false;

		useTransparencyActive = false;
	}

	public void setTransparencyState (bool state, Shader shaderToApply, float alphaTransparency)
	{
		transparencyActive = state;

		if (!useTransparencyActive) {
			return;
		}

		if (transparencyActive) {

			storeRenderElements ();

			int rendererPartsCount = rendererParts.Count;

			for (int i = 0; i < rendererPartsCount; i++) {
				Renderer currentRenderer = rendererParts [i];

				if (currentRenderer != null) {
					int materialsLength = currentRenderer.materials.Length;

					for (int j = 0; j < materialsLength; j++) {
						Material currentMaterial = currentRenderer.materials [j];

						currentMaterial.shader = shaderToApply;

						Color alpha = currentMaterial.color;

						if (useCustomTransparencyValues) {
							alpha.a = customAlphaTransparency;
						} else {
							alpha.a = alphaTransparency;
						}

						currentMaterial.color = alpha;
					}
				}
			}
		} else {
			if (outlineActive) {
				setOutlineState (true, currentOutlineShader, currentOutlineWidht, currentOutlieColor, null);
			} else {
				int rendererPartsCount = rendererParts.Count;

				for (int i = 0; i < rendererPartsCount; i++) {
					Renderer currentRenderer = rendererParts [i];

					if (currentRenderer != null) {
						int materialsLength = currentRenderer.materials.Length;

						for (int j = 0; j < materialsLength; j++) {
							currentRenderer.materials [j].shader = originalShader [i];
						}
					}
				}
			}
		}
	}

	public bool isTransparencyActive ()
	{
		return transparencyActive;
	}
}
