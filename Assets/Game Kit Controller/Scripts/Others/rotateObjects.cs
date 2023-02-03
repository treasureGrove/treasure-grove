using UnityEngine;
using System.Collections;

public class rotateObjects : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public Vector3 direccion;
	public float speed = 1;
	public bool rotationEnabled = true;

	public float resetRotationSpeed = 5;

	public Transform mainTransform;

	Coroutine rotationCoroutine;


	void Start ()
	{
		if (mainTransform == null) {
			mainTransform = transform;
		}
	}

	void FixedUpdate ()
	{
		if (rotationEnabled) {
			mainTransform.Rotate (direccion * (speed * Time.deltaTime));
		}
	}

	public void enableOrDisableRotation ()
	{
		rotationEnabled = !rotationEnabled;
	}

	public void increaseRotationSpeedTenPercentage ()
	{
		speed += speed * 0.1f;
	}

	public void setRotationEnabledState (bool state)
	{
		stopRotation ();

		rotationEnabled = state;
	}

	public void disableRotation ()
	{
		setRotationEnabledState (false);
	}

	public void enableRotation ()
	{
		setRotationEnabledState (true);
	}

	public void resetRotation ()
	{
		stopRotation ();

		rotationCoroutine = StartCoroutine (resetRotationCoroutine ());
	}

	public void stopRotation ()
	{
		if (rotationCoroutine != null) {
			StopCoroutine (rotationCoroutine);
		}
	}

	IEnumerator resetRotationCoroutine ()
	{
		Quaternion targetRotation = Quaternion.identity;

		float t = 0;

		while (transform.localRotation != targetRotation) {
			t += Time.deltaTime / resetRotationSpeed;

			transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRotation, t);

			yield return null;
		}
	}

	public void setRandomDirectionToCurrent ()
	{
		int newDirection = Random.Range (0, 2);

		if (newDirection == 0) {
			newDirection = -1;
		}

		direccion *= newDirection;
	}

	public void setNewRotationSpeed (float newValue)
	{
		speed = newValue;
	}

	public void setEnabledState (bool state)
	{
		enabled = state;
	}
}
