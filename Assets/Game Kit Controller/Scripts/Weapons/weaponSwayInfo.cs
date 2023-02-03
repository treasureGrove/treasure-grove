using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class weaponSwayInfo
{
	public bool useSway;

	public bool usePositionSway;
	public float swayPositionVertical = 0.02f;
	public float swayPositionHorizontal = 0.03f;
	public float swayPositionMaxAmount = 0.03f;
	public float swayPositionSmooth = 2;
	public float resetSwayPositionSmooth = 2;

	public bool useRotationSway;
	public float swayRotationVertical = 20;
	public float swayRotationHorizontal = 30;
	public float swayRotationSmooth = 3;
	public float resetSwayRotationSmooth = 3;

	public bool useBobPosition;
	public Vector3 bobPositionSpeed = new Vector3 (5, 10, 3);
	public Vector3 bobPositionAmount = new Vector3 (0.01f, 0.05f, 0.05f);

	public bool useInputMultiplierForBobPosition;

	public bool useBobRotation;
	public float bobRotationVertical = 15;
	public float bobRotationHorizontal = 10;

	public bool useInputMultiplierForBobRotation;

	public Vector3 movingExtraPosition;
	public float swayPositionRunningMultiplier = 1;
	public float swayRotationRunningMultiplier = 1;
	public float bobPositionRunningMultiplier = 1;
	public float bobRotationRunningMultiplier = 1;
	[Range (0, 1)] public float swayPositionPercentageAiming;
	[Range (0, 1)] public float swayRotationPercentageAiming;
	[Range (0, 1)] public float bobPositionPercentageAiming;
	[Range (0, 1)] public float bobRotationPercentageAiming;

	public bool useSwayPositionClamp;
	public Vector2 swayPositionHorizontalClamp;
	public Vector2 swayPositionVerticalClamp;

	public bool useSwayRotationClamp;
	public Vector2 swayRotationClampX = new Vector2 (-20, 20);
	public Vector2 swayRotationClampY;
	public Vector2 swayRotationClampZ = new Vector3 (-15, 15);

	public float minMouseAmountForSway = 0.2f;
}