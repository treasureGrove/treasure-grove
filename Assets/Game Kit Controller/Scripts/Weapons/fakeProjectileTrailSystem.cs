using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeProjectileTrailSystem : MonoBehaviour
{
	public bool trailEnabled = true;

	public bool trailActive;
	public Vector3 targetPosition;
	public TrailRenderer mainTrail;
	public float reduceTrailTimeSpeed = 4;
	public bool movementActive = true;

	public bool disableInsteadOfDestroyActive;

	public float speedMultiplier;

	public float maxSpawnTime = 5;

	public bool destroyTrailAfterTime;

	float timerSpeed;

	Vector3 originalPosition;

	float lastTimeInstantiated;

	void Update ()
	{
		if (trailActive) {
			if (movementActive) {

				timerSpeed += Time.deltaTime / speedMultiplier;
				transform.position = Vector3.Lerp (originalPosition, targetPosition, timerSpeed);

			} else {
				mainTrail.time -= Time.deltaTime * reduceTrailTimeSpeed;

				if (mainTrail.time <= 0) {
					if (disableInsteadOfDestroyActive) {
						gameObject.SetActive (false);

						resetFakeProjectileTrail ();
					} else {
						Destroy (gameObject);
					}
				}
			}

			if (destroyTrailAfterTime) {
				if (Time.time > lastTimeInstantiated + maxSpawnTime) {
					if (disableInsteadOfDestroyActive) {
						gameObject.SetActive (false);

						resetFakeProjectileTrail ();
					} else {
						Destroy (gameObject);
					}
				}
			}
		}
	}

	public void instantiateFakeProjectileTrail (Vector3 newTargetPosition)
	{
		if (trailEnabled) {
			if (!gameObject.activeSelf) {
				gameObject.SetActive (true);
			}

			originalPosition = transform.position;

			transform.SetParent (null);

			trailActive = true;

			Vector3 newTargetDirection = newTargetPosition - transform.position;

			transform.rotation = Quaternion.LookRotation (newTargetDirection);

			targetPosition = newTargetPosition;

			lastTimeInstantiated = Time.time;
		} else {
			gameObject.SetActive (false);
		}
	}

	public void setSpeedMultiplier (float newValue)
	{
		speedMultiplier = newValue;
	}

	public void stopTrailMovement ()
	{
		movementActive = false;
	}

	public void setDestroyTrailAfterTimeState (bool state)
	{
		destroyTrailAfterTime = state;
	}

	public void changeDestroyForSetActiveFunction (bool state)
	{
		disableInsteadOfDestroyActive = state;
	}

	public void resetFakeProjectileTrail ()
	{
		if (trailActive) {
			trailActive = false;

			mainTrail.time = 1;

			timerSpeed = 0;

			movementActive = true;

			destroyTrailAfterTime = false;

			mainTrail.Clear ();
		}
	}
}