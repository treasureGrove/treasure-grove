using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class plasmaCutterSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public Transform mainRotor;

	public float initialRotorRotation;

	public bool useFixedVerticalHorizontalRotations;

	public float angleRotationAmount;

	public float rotationSpeed;

	public float manualRotationAmount;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnFireWeapon;

	public UnityEvent eventOnRotateWeapon;

	bool rotating;

	bool rotationToggleState;

	Coroutine rotationCoroutine;

	void Start ()
	{
		mainRotor.localEulerAngles = new Vector3 (0, 0, initialRotorRotation);
	}

	public void firePlasmaCutter ()
	{
		eventOnFireWeapon.Invoke ();
	}

	public void rotateRotorToRight ()
	{
		mainRotor.Rotate (Vector3.forward * manualRotationAmount);
	}

	public void rotateRotorToLeft ()
	{
		mainRotor.Rotate (-Vector3.forward * manualRotationAmount);
	}

	public void changeRotorRotation ()
	{
		if (rotating) {
			return;
		}

		float rotationAmount = 0;

		if (useFixedVerticalHorizontalRotations) {
			rotationToggleState = !rotationToggleState;

			if (rotationToggleState) {
				rotationAmount = 90;
			} else {
				rotationAmount = 0;
			}
		} else {
			rotationAmount += angleRotationAmount;

			if (rotationAmount > 360) {
				rotationAmount = 360 - rotationAmount;
			}
		}

		eventOnRotateWeapon.Invoke ();

		stopChangeRotorRotation ();

		rotationCoroutine = StartCoroutine (stopChangeRotorRotationCoroutine (rotationAmount));
	}

	public void stopChangeRotorRotation ()
	{
		if (rotationCoroutine != null) {
			StopCoroutine (rotationCoroutine);
		}
	}

	IEnumerator stopChangeRotorRotationCoroutine (float rotationAmount)
	{
		rotating = true;

		Vector3 eulerTarget = Vector3.zero;

		eulerTarget.z = rotationAmount;

		Quaternion rotationTarget = Quaternion.Euler (eulerTarget);

		float t = 0;

		float movementTimer = 0;

		bool targetReached = false;

		float angleDifference = 0;

		while (!targetReached) {
			t += Time.deltaTime * rotationSpeed; 
			mainRotor.localRotation = Quaternion.Lerp (mainRotor.localRotation, rotationTarget, t);

			movementTimer += Time.deltaTime;

			angleDifference = Quaternion.Angle (mainRotor.localRotation, rotationTarget);

			movementTimer += Time.deltaTime;

			if (angleDifference < 0.2f || movementTimer > 2) {
				targetReached = true;
			}
			yield return null;
		}

		rotating = false;
	}
}
