using UnityEngine;
using System.Collections;
using System;

public class upperBodyRotationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool shakeUpperBodyEnabled = true;

	[Space]
	[Header ("Head Settings")]
	[Space]

	public Vector3 headLookVector = Vector3.forward;
	public Vector3 headUpVector = Vector3.up;

	[Space]
	[Header ("Rotation Settings")]
	[Space]

	public float horizontalThresholdAngleDifference = 5;
	public float verticalThresholdAngleDifference = 5;
	public float horizontalBendingMultiplier = 1;
	public float verticalBendingMultiplier = 1;
	public float horizontalMaxAngleDifference = 90;
	public float verticalMaxAngleDifference = 90;
	public float maxBendingAngle = 90;
	public float horizontalResponsiveness = 10;
	public float verticalResponsiveness = 10;

	public float upperBodyRotationSpeed = 5;
	public float rotationSpeed = 5;

	public bool chestRotationEnabled = true;
	public bool spineRotationEnabled = true;
	public bool extraHorizontalRotationEnabled = true;

	public bool horizontalRotationEnabled = true;
	public bool verticalRotationEnabled = true;

	[Space]
	[Header ("Chest Orientation Settings")]
	[Space]

	public Vector3 chestUpVector = new Vector3 (0, 0, 1);

	[Space]
	[Header ("Spine And Chest Transforms")]
	[Space]

	public Transform spineTransform;
	public Transform chestTransform;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool IKUpperBodyActive;
	public float currentExtraRotation;
	public bool usingWeaponRotationPoint;

	public bool usingTemporalObjectToFollow;
	public Transform temporalObjectToFollow;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform objectToFollow;
	public Transform weaponRotationPointToFollow;
	public playerController playerControllerManager;

	bool usingDualWeapon;

	Transform currentWeaponRotationPoint;

	float currentRotation;
	float rotationXValue;

	weaponRotationPointInfo rotationPointInfo;

	int currentRotationDirection = 1;

	Quaternion chestRotation;
	Quaternion spineRotation;

	float angleH;
	float angleV;
	Vector3 dirUp;
	Vector3 referenceLookDir;
	Vector3 referenceUpDir;
	float originalAngleDifference;
	Coroutine changeExtraRotation;

	Vector3 extraRotation;

	float auxCurrentExtraRotation;

	Coroutine shakeUpperBodyRotationCoroutine;

	Quaternion parentRot;
	Quaternion parentRotInv;
	Vector3 lookDirWorld;
	Vector3 lookDirGoal;

	Vector3 rightOfTarget;
	Vector3 lookDirGoalinHPlane;

	float hAngle;
	float vAngle;
	float hAngleThr;
	float vAngleThr;

	float hAngleABS;
	float vAngleABS;

	float hAngleSign;
	float vAngleSign;

	Vector3 referenceRightDir;
	Vector3 upDirGoal;
	Vector3 lookDir;
	Quaternion lookRot;
	Quaternion dividedRotation;

	Quaternion finalLookRotation;

	bool isPlayerDead;

	Transform currentRightWeaponRotationPoint;
	Transform currentLeftWeaponRotationPoint;

	weaponRotationPointInfo rightRotationPointInfo;
	weaponRotationPointInfo leftRotationPointInfo;

	int currentRightRotationDirection = 1;
	int currentLeftRotationDirection = 1;

	bool weaponRotationPointToFollowLocated;

	float originalHorizontalBendingMultiplier;
	float originalVerticalBendingMultiplier;

	void Start ()
	{
		parentRot = spineTransform.parent.rotation;
		parentRotInv = Quaternion.Inverse (parentRot);

		referenceLookDir = parentRotInv * transform.rotation * headLookVector.normalized;
		referenceUpDir = parentRotInv * transform.rotation * headUpVector.normalized;
		dirUp = referenceUpDir;
		originalAngleDifference = maxBendingAngle;

		weaponRotationPointToFollowLocated = weaponRotationPointToFollow != null;

		originalHorizontalBendingMultiplier = horizontalBendingMultiplier;
		originalVerticalBendingMultiplier = verticalBendingMultiplier;
	}

	void FixedUpdate ()
	{
		if (IKUpperBodyActive) {

			isPlayerDead = playerControllerManager.isPlayerDead ();

			if (isPlayerDead) {
				return;
			}

			parentRot = spineTransform.parent.rotation;
			parentRotInv = Quaternion.Inverse (parentRot);

			// Desired look direction in world space
			if (usingTemporalObjectToFollow) {
				lookDirWorld = (temporalObjectToFollow.position - chestTransform.position).normalized;
			} else {
				lookDirWorld = (objectToFollow.position - chestTransform.position).normalized;
			}

			// Desired look directions in neck parent space
			lookDirGoal = (parentRotInv * lookDirWorld);

			// Get the horizontal and vertical rotation angle to look at the target
			hAngle = AngleAroundAxis (referenceLookDir, lookDirGoal, referenceUpDir);

			rightOfTarget = Vector3.Cross (referenceUpDir, lookDirGoal);
			lookDirGoalinHPlane = lookDirGoal - Vector3.Project (lookDirGoal, referenceUpDir);

			vAngle = AngleAroundAxis (lookDirGoalinHPlane, lookDirGoal, rightOfTarget);


			hAngleABS = Mathf.Abs (hAngle);
			vAngleABS = Mathf.Abs (vAngle);

			hAngleSign = Mathf.Sign (hAngle);
			vAngleSign = Mathf.Sign (vAngle);


			// Handle threshold angle difference, bending multiplier, and max angle difference here
			hAngleThr = Mathf.Max (0, hAngleABS - horizontalThresholdAngleDifference) * hAngleSign;			
			vAngleThr = Mathf.Max (0, vAngleABS - verticalThresholdAngleDifference) * vAngleSign;

			hAngle = Mathf.Max (Mathf.Abs (hAngleThr) * Mathf.Abs (horizontalBendingMultiplier), hAngleABS
			- horizontalMaxAngleDifference) * hAngleSign * Mathf.Sign (horizontalBendingMultiplier);
			
			vAngle = Mathf.Max (Mathf.Abs (vAngleThr) * Mathf.Abs (verticalBendingMultiplier), vAngleABS
			- verticalMaxAngleDifference) * vAngleSign * Mathf.Sign (verticalBendingMultiplier);

			// Handle max bending angle here
			hAngle = Mathf.Clamp (hAngle, -maxBendingAngle, maxBendingAngle);
			vAngle = Mathf.Clamp (vAngle, -maxBendingAngle, maxBendingAngle);

			referenceRightDir = Vector3.Cross (referenceUpDir, referenceLookDir);

			// Lerp angles
			angleH = Mathf.Lerp (angleH, hAngle, Time.deltaTime * horizontalResponsiveness);
			angleV = Mathf.Lerp (angleV, vAngle, Time.deltaTime * verticalResponsiveness);

			if (!horizontalRotationEnabled) {
				angleH = 0;
			}

			if (!verticalRotationEnabled) {
				angleV = 0;
			}

			// Get direction
			lookDirGoal = Quaternion.AngleAxis (angleH, referenceUpDir) *
			Quaternion.AngleAxis (angleV, referenceRightDir) *
			referenceLookDir;

			// Make look and up perpendicular
			upDirGoal = referenceUpDir;
			Vector3.OrthoNormalize (ref lookDirGoal, ref upDirGoal);

			// Interpolated look and up directions in neck parent space
			lookDir = lookDirGoal;
			dirUp = Vector3.Slerp (dirUp, upDirGoal, Time.deltaTime * 5);
			Vector3.OrthoNormalize (ref lookDir, ref dirUp);

			// Look rotation in world space
			lookRot = ((parentRot * Quaternion.LookRotation (lookDir, dirUp)) *
			Quaternion.Inverse (parentRot * Quaternion.LookRotation (referenceLookDir, referenceUpDir)));

			if (upperBodyRotationSpeed > 0) {
				finalLookRotation = Quaternion.Slerp (finalLookRotation, lookRot, Time.deltaTime * upperBodyRotationSpeed);
			} else {
				finalLookRotation = lookRot;
			}

			// Distribute rotation over all joints in segment
			dividedRotation = Quaternion.Slerp (Quaternion.identity, finalLookRotation, 0.5f);

			chestRotation = dividedRotation;
			spineRotation = dividedRotation;
		}

		if (usingWeaponRotationPoint) {
			checkWeaponRotationPoint ();
		}
	}

	void LateUpdate ()
	{
		if (IKUpperBodyActive) {

			if (!isPlayerDead) {
				if (chestRotationEnabled) {
					chestTransform.rotation = chestRotation * chestTransform.rotation;
				}

				if (spineRotationEnabled) {
					spineTransform.rotation = spineRotation * spineTransform.rotation;
				}

				if (extraHorizontalRotationEnabled) {
					extraRotation = chestUpVector * currentExtraRotation;

					chestTransform.localEulerAngles = new Vector3 (chestTransform.localEulerAngles.x, chestTransform.localEulerAngles.y, chestTransform.localEulerAngles.z) + extraRotation;
				}
			}
		}	
	}

	void checkWeaponRotationPoint ()
	{
		if (IKUpperBodyActive && weaponRotationPointToFollowLocated) {
			currentRotation = weaponRotationPointToFollow.localEulerAngles.x;

			if (usingDualWeapon) {
				if (currentRightWeaponRotationPoint != null) {

					if (currentRotation > 180) {
						rotationXValue = (360 - currentRotation) / rightRotationPointInfo.rotationUpPointAmountMultiplier;

						if (rightRotationPointInfo.useRotationUpClamp) {
							rotationXValue = Mathf.Clamp (rotationXValue, 0, rightRotationPointInfo.rotationUpClampAmount);
						}

						rotationXValue *= (-1) * currentRightRotationDirection;

					} else {
						rotationXValue = currentRotation / rightRotationPointInfo.rotationDownPointAmountMultiplier;

						if (rightRotationPointInfo.useRotationDownClamp) {
							rotationXValue = Mathf.Clamp (rotationXValue, 0, rightRotationPointInfo.rotationDownClamp);
						}

						rotationXValue *= currentRightRotationDirection;
					}

					Quaternion weaponRotationPointTarget = Quaternion.Euler (new Vector3 (rotationXValue, 0, 0));

					currentRightWeaponRotationPoint.localRotation = 
							Quaternion.Lerp (currentRightWeaponRotationPoint.localRotation, weaponRotationPointTarget, Time.deltaTime * rightRotationPointInfo.rotationPointSpeed);
				}

				if (currentLeftWeaponRotationPoint != null) {
					if (currentRotation > 180) {
						rotationXValue = (360 - currentRotation) / leftRotationPointInfo.rotationUpPointAmountMultiplier;

						if (leftRotationPointInfo.useRotationUpClamp) {
							rotationXValue = Mathf.Clamp (rotationXValue, 0, leftRotationPointInfo.rotationUpClampAmount);
						}

						rotationXValue *= (-1) * currentLeftRotationDirection;

					} else {
						rotationXValue = currentRotation / leftRotationPointInfo.rotationDownPointAmountMultiplier;

						if (leftRotationPointInfo.useRotationDownClamp) {
							rotationXValue = Mathf.Clamp (rotationXValue, 0, leftRotationPointInfo.rotationDownClamp);
						}

						rotationXValue *= currentLeftRotationDirection;
					}

					Quaternion weaponRotationPointTarget = Quaternion.Euler (new Vector3 (rotationXValue, 0, 0));

					currentLeftWeaponRotationPoint.localRotation = 
							Quaternion.Lerp (currentLeftWeaponRotationPoint.localRotation, weaponRotationPointTarget, Time.deltaTime * leftRotationPointInfo.rotationPointSpeed);
				}
			} else {
				if (currentRotation > 180) {
					rotationXValue = (360 - currentRotation) / rotationPointInfo.rotationUpPointAmountMultiplier;

					if (rotationPointInfo.useRotationUpClamp) {
						rotationXValue = Mathf.Clamp (rotationXValue, 0, rotationPointInfo.rotationUpClampAmount);
					}

					rotationXValue *= (-1) * currentRotationDirection;

				} else {
					rotationXValue = currentRotation / rotationPointInfo.rotationDownPointAmountMultiplier;

					if (rotationPointInfo.useRotationDownClamp) {
						rotationXValue = Mathf.Clamp (rotationXValue, 0, rotationPointInfo.rotationDownClamp);
					}

					rotationXValue *= currentRotationDirection;
				}

				Quaternion weaponRotationPointTarget = Quaternion.Euler (new Vector3 (rotationXValue, 0, 0));

				currentWeaponRotationPoint.localRotation = 
						Quaternion.Lerp (currentWeaponRotationPoint.localRotation, weaponRotationPointTarget, Time.deltaTime * rotationPointInfo.rotationPointSpeed);
			}
		} else {
			if (usingDualWeapon) {

				bool rightWeaponIsReset = false;
				bool leftWeaponIsReset = false;

				if (currentRightWeaponRotationPoint != null) {
					if (currentRightWeaponRotationPoint.localRotation != Quaternion.identity) {
						currentRightWeaponRotationPoint.localRotation = 
								Quaternion.Lerp (currentRightWeaponRotationPoint.localRotation, Quaternion.identity, Time.deltaTime * rightRotationPointInfo.rotationPointSpeed);
					} else {
						rightWeaponIsReset = true;
					}
				} else {
					rightWeaponIsReset = true;
				}

				if (currentLeftWeaponRotationPoint != null) {
					if (currentLeftWeaponRotationPoint.localRotation != Quaternion.identity) {
						currentLeftWeaponRotationPoint.localRotation = 
								Quaternion.Lerp (currentLeftWeaponRotationPoint.localRotation, Quaternion.identity, Time.deltaTime * leftRotationPointInfo.rotationPointSpeed);
					} else {
						leftWeaponIsReset = true;
					}
				} else {
					leftWeaponIsReset = true;
				}

				if (rightWeaponIsReset && leftWeaponIsReset) {
					usingWeaponRotationPoint = false;
				}
			} else {
				if (currentWeaponRotationPoint != null) {
					if (currentWeaponRotationPoint.localRotation != Quaternion.identity) {

						currentWeaponRotationPoint.localRotation = 
								Quaternion.Lerp (currentWeaponRotationPoint.localRotation, Quaternion.identity, Time.deltaTime * rotationPointInfo.rotationPointSpeed);
					} else {
						usingWeaponRotationPoint = false;
					}
				}
			}
		}
	}

	// The angle between dirA and dirB around axis
	public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		// Project A and B onto the plane orthogonal target axis
		dirA = dirA - Vector3.Project (dirA, axis);
		dirB = dirB - Vector3.Project (dirB, axis);

		// Find (positive) angle between A and B
		float angle = Vector3.Angle (dirA, dirB);
		return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}

	public void setCurrentBodyRotation (float bodyRotation)
	{
		currentExtraRotation = bodyRotation;
		auxCurrentExtraRotation = currentExtraRotation;
	}

	public void enableOrDisableIKUpperBody (bool value)
	{
		if (value) {
			IKUpperBodyActive = value;
		}

		checkSetExtraRotationCoroutine (value);
	}

	void checkSetExtraRotationCoroutine (bool state)
	{
		if (changeExtraRotation != null) {
			StopCoroutine (changeExtraRotation);
		}

		changeExtraRotation = StartCoroutine (setExtraRotation (state));
	}

	IEnumerator setExtraRotation (bool state)
	{
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotationSpeed;
			if (state) {
				maxBendingAngle = Mathf.Lerp (maxBendingAngle, originalAngleDifference, t);
			} else {
				maxBendingAngle = Mathf.Lerp (maxBendingAngle, 0, t);
			}
			yield return null;
		}

		if (!state) {
			IKUpperBodyActive = false;
		}
	}

	public bool isIKUpperBodyActive ()
	{
		return IKUpperBodyActive;
	}

	public void checkShakeUpperBodyRotationCoroutine (float extraAngleValue, float speedValue)
	{
		if (!shakeUpperBodyEnabled || !IKUpperBodyActive) {
			return;
		}

		stopShakeUpperBodyRotation ();

		shakeUpperBodyRotationCoroutine = StartCoroutine (shakeUpperBodyRotation (extraAngleValue, speedValue));
	}

	public void stopShakeUpperBodyRotation ()
	{
		if (shakeUpperBodyRotationCoroutine != null) {
			StopCoroutine (shakeUpperBodyRotationCoroutine);
		}
	}

	IEnumerator shakeUpperBodyRotation (float extraAngleValue, float speedValue)
	{
		float angleTarget = auxCurrentExtraRotation + extraAngleValue;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * speedValue;
			currentExtraRotation = Mathf.Lerp (currentExtraRotation, angleTarget, t);
			yield return null;
		}

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * speedValue;
			currentExtraRotation = Mathf.Lerp (currentExtraRotation, auxCurrentExtraRotation, t);
			yield return null;
		}
	}

	public void setCurrentWeaponRotationPoint (Transform newWeaponRotationPoint, weaponRotationPointInfo newRotationPointInfo, int newRotationDirection)
	{
		currentWeaponRotationPoint = newWeaponRotationPoint;

		rotationPointInfo = newRotationPointInfo;

		currentRotationDirection = newRotationDirection;
	}

	public void setUsingWeaponRotationPointState (bool state)
	{
		usingWeaponRotationPoint = state;
	}

	public void setUsingDualWeaponState (bool state)
	{
		usingDualWeapon = state;
	}

	public void setCurrentRightWeaponRotationPoint (Transform newWeaponRotationPoint, weaponRotationPointInfo newRotationPointInfo, int newRotationDirection)
	{
		currentRightWeaponRotationPoint = newWeaponRotationPoint;

		rightRotationPointInfo = newRotationPointInfo;

		currentRightRotationDirection = newRotationDirection;
	}

	public void setCurrentLeftWeaponRotationPoint (Transform newWeaponRotationPoint, weaponRotationPointInfo newRotationPointInfo, int newRotationDirection)
	{
		currentLeftWeaponRotationPoint = newWeaponRotationPoint;

		leftRotationPointInfo = newRotationPointInfo;

		currentLeftRotationDirection = newRotationDirection;
	}

	public void setTemporalObjectToFollow (Transform newObject)
	{
		temporalObjectToFollow = newObject;

		usingTemporalObjectToFollow = temporalObjectToFollow != null;
	}

	public void setHorizontalBendingMultiplierValue (float newValue)
	{
		horizontalBendingMultiplier = newValue;
	}

	public void setOriginalHorizontalBendingMultiplier ()
	{
		setHorizontalBendingMultiplierValue (originalHorizontalBendingMultiplier);
	}

	public void setVerticalBendingMultiplierValue (float newValue)
	{
		verticalBendingMultiplier = newValue;
	}

	public void setOriginalVerticalBendingMultiplier ()
	{
		setVerticalBendingMultiplierValue (originalVerticalBendingMultiplier);
	}
		
	public void setNewChestUpVectorValue (Vector3 newValue)
	{
		chestUpVector = newValue;

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}