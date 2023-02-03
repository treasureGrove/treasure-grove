using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingPlatform : MonoBehaviour
{
	public bool rotationActive = true;
	public float rotationSpeed;
	public float rotationDelay;
	public float rotationAmount;

	public Vector3 rotationAxis;

	Coroutine rotationCoroutine;

	void Start ()
	{
		if (rotationActive) {
			rotatePlatform ();
		}
	}

	void Update ()
	{
		
	}

	public void rotatePlatform ()
	{
		stopRotationCoroutine ();
		rotationCoroutine = StartCoroutine (rotatePlatformCoroutine ());
	}

	public void stopRotationCoroutine ()
	{
		if (rotationCoroutine != null) {
			StopCoroutine (rotationCoroutine);
		}
	}

	IEnumerator rotatePlatformCoroutine ()
	{
		Vector3 targetRotation = transform.eulerAngles + rotationAxis * rotationAmount;

		Quaternion initialRotation = transform.rotation;

		float t = 0.0f;
		while ( t  < rotationSpeed )
		{
			t += Time.deltaTime;
			transform.rotation = initialRotation * Quaternion.AngleAxis(t / rotationSpeed * rotationAmount, rotationAxis);
			yield return null;
		}

		transform.eulerAngles = targetRotation;

		yield return new WaitForSeconds (rotationDelay);

		if (rotationActive) {

			rotatePlatform ();
		}
	}

	public void setRotationActiveState (bool state)
	{
		rotationActive = state;
		if(rotationActive) {
			rotatePlatform();
		}else{
			stopRotationCoroutine ();
		}
	}
}
