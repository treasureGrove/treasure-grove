using UnityEngine;
using System.Collections;

[System.Serializable]
public class externalShakeInfo
{

	public Vector3 shakePosition;
	public Vector3 shakePositionSpeed;
	public float shakePositionSmooth;
	public Vector3 shakeRotation;
	public Vector3 shakeRotationSpeed;
	public float shakeRotationSmooth;
	public float shakeDuration;

	public bool decreaseShakeInTime;
	public float decreaseShakeSpeed;

	public bool useDelayBeforeStartDecrease;
	public float delayBeforeStartDecrease;

	public bool repeatShake;
	public int numberOfRepeats;
	public float delayBetweenRepeats;

	public float externalShakeDelay;

	public bool useUnscaledTime;

	public externalShakeInfo (externalShakeInfo newShake)
	{
		shakePosition = newShake.shakePosition;
		shakePositionSpeed = newShake.shakePositionSpeed;
		shakePositionSmooth = newShake.shakePositionSmooth;
		shakeRotation = newShake.shakeRotation;
		shakeRotationSpeed = newShake.shakeRotationSpeed;
		shakeRotationSmooth = newShake.shakeRotationSmooth;	
		shakeDuration = newShake.shakeDuration;
		decreaseShakeInTime = newShake.decreaseShakeInTime;
		decreaseShakeSpeed = newShake.decreaseShakeSpeed;

		useDelayBeforeStartDecrease = newShake.useDelayBeforeStartDecrease;
		delayBeforeStartDecrease = newShake.delayBeforeStartDecrease;

		repeatShake = newShake.repeatShake;
		numberOfRepeats = newShake.numberOfRepeats;
		delayBetweenRepeats = newShake.delayBetweenRepeats;

		externalShakeDelay = newShake.externalShakeDelay;

		useUnscaledTime = newShake.useUnscaledTime;
	}

	public externalShakeInfo ()
	{

	}
}
