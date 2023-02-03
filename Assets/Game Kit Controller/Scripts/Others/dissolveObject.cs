using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dissolveObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool dissolveObjectEnabled = true;

	public float timeToDestroyObject = 0.9f;

	public float dissolveDelay;

	public float dissolveSpeed = 0.2f;
	public float currentFadeValue = 0;

	[Space]
	[Header ("Shader Settings")]
	[Space]

	public Shader shaderToApply;
	public Texture dissolveTexture;
	public Color dissolveColor;
	public float dissolveColorAlpha;

	public string dissolveShaderFieldName = "_Amount";
	public string dissolveShaderTextureFieldName = "_DissolveTexture";
	public string dissolveShaderColorFieldName = "_DissolveColor";
	public string dissolveShaderAlphaColorFieldName = "_DissolveColorAlpha";

	[Space]
	[Header ("Object To Dissolve Settings")]
	[Space]

	public GameObject objectToDissolve;

	public bool useGameObjectList;
	public List<GameObject> gameObjectList = new List<GameObject> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectToDissolveFound;

	public typeObjectFound currentTypeObjectFound;

	public List<Renderer> rendererParts = new List<Renderer> ();

	public List<Shader> originalShader = new List<Shader> ();
	public List<Material> materialList = new List<Material> ();

	public enum typeObjectFound
	{
		vehicle,
		regularObject,
		npc
	}


	float lastTimeDissolveActivated;

	Renderer currentRenderer;

	Coroutine dissolveCoroutine;

	public void stopDissolveCoroutine ()
	{
		if (dissolveCoroutine != null) {
			StopCoroutine (dissolveCoroutine);
		}
	}

	IEnumerator activateDissolveCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			if (objectToDissolveFound) {
				if (Time.time > dissolveDelay + lastTimeDissolveActivated) {
					currentFadeValue += Time.deltaTime * dissolveSpeed;

					if (currentFadeValue <= 1) {
						int rendererPartsCount = rendererParts.Count;

						for (int i = 0; i < rendererPartsCount; i++) {
							currentRenderer = rendererParts [i];

							if (currentRenderer != null) {
								int materialsLength = currentRenderer.materials.Length;

								for (int j = 0; j < materialsLength; j++) {
									currentRenderer.materials [j].SetFloat (dissolveShaderFieldName, currentFadeValue);
								}
							}
						}
					}

					if (Time.time > lastTimeDissolveActivated + timeToDestroyObject) {
						destroyObject ();

						stopDissolveCoroutine ();
					} 
				}
			}
		}
	}

	public void activateDissolve (GameObject newOjectToDissolve)
	{
		if (!dissolveObjectEnabled) {
			return;
		}

		objectToDissolve = newOjectToDissolve;

		if (objectToDissolve.GetComponent<Rigidbody> ()) {

			objectToDissolveFound = true;

			currentTypeObjectFound = typeObjectFound.regularObject;
		} 

		bool isCharacter = applyDamage.isCharacter (objectToDissolve);

		if (isCharacter) {
			objectToDissolve = applyDamage.getCharacter (objectToDissolve);

			objectToDissolveFound = true;

			currentTypeObjectFound = typeObjectFound.npc;
		} else {

			bool isVehicle = applyDamage.isVehicle (objectToDissolve);

			if (isVehicle) {
				objectToDissolve = applyDamage.getVehicle (objectToDissolve);

				objectToDissolveFound = true;

				currentTypeObjectFound = typeObjectFound.vehicle;
			}
		}

		if (objectToDissolveFound && objectToDissolve != null) {

			outlineObjectSystem currentOutlineObjectSystem = objectToDissolve.GetComponent<outlineObjectSystem> ();

			if (currentOutlineObjectSystem != null) {
				currentOutlineObjectSystem.disableOutlineAndRemoveUsers ();
			}

			storeRenderElements ();
		}

		lastTimeDissolveActivated = Time.time;

		stopDissolveCoroutine ();

		dissolveCoroutine = StartCoroutine (activateDissolveCoroutine ());
	}

	public void destroyObject ()
	{
		if (objectToDissolve == null) {
			return;
		}

		if (useGameObjectList) {
			for (int i = 0; i < gameObjectList.Count; i++) {
				if (gameObjectList [i] != null) {
					Destroy (gameObjectList [i]);
				}
			}
		}
			
		if (currentTypeObjectFound == typeObjectFound.vehicle) {
			vehicleHUDManager currenVehicleHUDManager = objectToDissolve.GetComponent<vehicleHUDManager> ();

			if (currenVehicleHUDManager != null) {
				currenVehicleHUDManager.destroyVehicleAtOnce ();
			}
		}

		if (currentTypeObjectFound == typeObjectFound.npc) {
			playerController currentPlayerController = objectToDissolve.GetComponent<playerController> ();

			if (currentPlayerController != null) {
				currentPlayerController.destroyCharacterAtOnce ();
			} else {
				Destroy (objectToDissolve);
			}
		}

		if (currentTypeObjectFound == typeObjectFound.regularObject) {
			Destroy (objectToDissolve);
		}
	}

	public void storeRenderElements ()
	{
		rendererParts.Clear ();

		originalShader.Clear ();

		materialList.Clear ();

		if (useGameObjectList) {
			for (int i = 0; i < gameObjectList.Count; i++) {
				if (gameObjectList [i] != null) {
					Component[] components = gameObjectList [i].GetComponentsInChildren (typeof(Renderer));

					foreach (Renderer child in components) {
						if (child.material.shader != null && !child.GetComponent<ParticleSystem> ()) {
							rendererParts.Add (child);
						}
					}
				}
			}
		} else {
			Component[] components = objectToDissolve.GetComponentsInChildren (typeof(Renderer));

			foreach (Renderer child in components) {
				if (child.material.shader != null && !child.GetComponent<ParticleSystem> ()) {
					rendererParts.Add (child);
				}
			}
		}

		int rendererPartsCount = rendererParts.Count;

		for (int i = 0; i < rendererPartsCount; i++) {
			currentRenderer = rendererParts [i];

			if (currentRenderer != null) {

				int materialsLength = currentRenderer.materials.Length;

				for (int j = 0; j < materialsLength; j++) {
					Material currentMaterial = currentRenderer.materials [j];

					originalShader.Add (currentMaterial.shader);

					materialList.Add (currentMaterial);

					currentMaterial.shader = shaderToApply;
					currentMaterial.SetTexture (dissolveShaderTextureFieldName, dissolveTexture);
					currentMaterial.SetColor (dissolveShaderColorFieldName, dissolveColor);
					currentMaterial.SetFloat (dissolveShaderAlphaColorFieldName, dissolveColorAlpha);
				}
			}
		}
	}

	public void setDissolveObjectEnabledState (bool state)
	{
		dissolveObjectEnabled = state;
	}

	public void cancelDissolve ()
	{
		objectToDissolveFound = false;

		if (Time.time > dissolveDelay + lastTimeDissolveActivated) {
			for (int j = 0; j < materialList.Count; j++) {
				materialList [j].shader = originalShader [j];
			}
		}

		stopDissolveCoroutine ();
	}
}