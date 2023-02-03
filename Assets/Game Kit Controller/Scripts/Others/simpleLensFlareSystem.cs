using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleLensFlareSystem : MonoBehaviour
{
	public LensFlare mainLensFlare;
	public float fadeLensFlareSpeed = 4;

	public float lensActiveDuration = 2;
	public GameObject lensFlareGameObject;

	Coroutine mainCoroutine;

	public void enableOrDisableLesnFlare (bool state)
	{
		if (mainCoroutine != null) {
			StopCoroutine (mainCoroutine);
		}

		mainCoroutine = StartCoroutine (enableOrDisableLesnFlareCoroutine (state));
	}

	IEnumerator enableOrDisableLesnFlareCoroutine (bool state)
	{
		lensFlareGameObject.SetActive (true);

		yield return new WaitForSeconds (lensActiveDuration);

		lensFlareGameObject.SetActive (false);

//		float targetValue = 0;
//
//		if (state) {
//			targetValue = 1;
//		}
//
//		bool targetReached = false;
//
//		float t = 0;
//
//		Color currentColor = mainLensFlare.color;
//
//		while (!targetReached) {
//			t += Time.deltaTime / fadeLensFlareSpeed; 
//
//			currentColor.a = Mathf.Lerp (currentColor.a, targetValue, t);
//
//			mainLensFlare.color = currentColor;
//
//			if (mainLensFlare.color.a == targetValue) {
//				targetReached = true;
//			}
//
//			yield return null;
//		}
	}
}
