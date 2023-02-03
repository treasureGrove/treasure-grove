using UnityEngine;
using System.Collections;

public class fadeObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float vanishSpeed;
	public Renderer meshRenderer;

	[Space]

	public bool vanishAtOnce;
	public float delayToVanishAtOnce;

	[Space]

	public bool sendObjectToPoolSystemToDisable;


	Color originalColor;

	bool originalColorStored;


	public void activeVanish (float newSpeed)
	{
		if (meshRenderer == null) {
			meshRenderer = GetComponentInChildren<Renderer> ();
		}

		if (meshRenderer == null) {
			return;
		}
			
		if (newSpeed > 0) {
			vanishSpeed = newSpeed;
		}

		if (sendObjectToPoolSystemToDisable) {
			if (originalColorStored) {
				meshRenderer.material.color = originalColor;
			} else {
				originalColor = meshRenderer.material.color;

				originalColorStored = true;
			}
		}

		if (!gameObject.activeSelf) {
			return;
		}

		if (!gameObject.activeInHierarchy) {
			return;
		}

		StartCoroutine (changeColorCoroutine ());
	}

	IEnumerator changeColorCoroutine ()
	{
		if (vanishAtOnce) {
			yield return new WaitForSeconds (delayToVanishAtOnce);

			if (!sendObjectToPoolSystemToDisable) {
				Destroy (gameObject);
			}
		} else {
			if (meshRenderer != null) {
				Color alpha = meshRenderer.material.color;
		
				while (alpha.a > 0) {
					alpha.a -= Time.deltaTime * vanishSpeed;

					meshRenderer.material.color = alpha;

					if (alpha.a <= 0) {
						if (!sendObjectToPoolSystemToDisable) {
							Destroy (gameObject);
						}
					}

					yield return null;
				}
			}

			yield return null;
		}

		if (sendObjectToPoolSystemToDisable) {
			GKC_PoolingSystem.Despawn (gameObject);
		}
	}
}