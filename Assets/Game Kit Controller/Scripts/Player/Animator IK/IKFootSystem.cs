using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSystem : OnAnimatorIKComponent
{
	[Header ("Main Settings")]
	[Space]

	public bool IKFootSystemEnabled = true;

	public LayerMask layerMask;

	public float IKWeightEnabledSpeed = 10;
	public float IKWeightDisabledSpeed = 2;
	public float extraFootOffset = 0.005f;

	[Space]
	[Header ("Hips Settings")]
	[Space]

	public float hipsMovementSpeed = 1.2f;
	public float hipsPositionUpClampAmount = 0.2f;
	public float hipsPositionDownClampAmount = -0.2f;

	public float hipsPositionUpMovingClampAmount = 0.2f;
	public float hipsPositionDownMovingClampAmount = -0.2f;

	[Space]
	[Header ("Leg Info List Settings")]
	[Space]

	public List<legInfo> legInfoList = new List<legInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool canUseIkFoot;
	public bool IKFootPaused;

	public bool removeIKValue;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController playerControllerManager;
	public Animator mainAnimator;
	public Transform playerTransform;

	public Transform hips;

	float newHipsOffset;

	bool usingIKFootPreviously;

	Vector3 hipsPosition;
	float currentHipsOffset;

	bool playerOnGround;

	bool currentLegIsRight;

	RaycastHit hit;

	legInfo currentLegInfo;

	float currentRaycastDistance;
	float currentAdjustmentSpeed;
	Vector3 currentRaycastPosition;
	float newRaycastDistance;

	float newOffset;

	Vector3 newFootPosition;
	Quaternion newFootRotation;
	float targetWeight;
	Vector3 localFootPosition;

	bool initialPositionAssigned;

	Vector3 newRaycastPosition;
	Vector3 newLocalHipsPosition;

	bool playerIsMoving;

	bool checkRaycast;

	int i;

	Vector3 currentTransfrormUp;

	Vector3 currentGravityNormal;

	float currentWeightToApply;

	AvatarIKGoal currentIKGoal;

	float minIKValue = 0.001f;

	Vector3 rightFootHitPoint;
	Vector3 leftFootHitPoint;


	void Start ()
	{

		for (i = 0; i < 2; i++) {
			currentLegInfo = legInfoList [i];

			currentLegInfo.offset = playerTransform.InverseTransformPoint (currentLegInfo.foot.position).y;
			currentLegInfo.maxLegLength = playerTransform.InverseTransformPoint (currentLegInfo.lowerLeg.position).y;
		}

		hipsPosition = hips.position;
	}

	void LateUpdate ()
	{
		if (!IKFootSystemEnabled) {
			return;
		}

		canUseIkFoot = (!playerControllerManager.isPlayerDead () &&
		!playerControllerManager.isPlayerDriving () &&
		!playerControllerManager.isPlayerOnFirstPerson () &&
		(!playerControllerManager.isPlayerOnFFOrZeroGravityModeOn () || playerControllerManager.isPlayerOnGround ()) &&
		!playerControllerManager.isUsingJetpack () &&
		!playerControllerManager.isFlyingActive ()) &&
		!playerControllerManager.isSwimModeActive ();

		if (usingIKFootPreviously != canUseIkFoot) {
			if (!usingIKFootPreviously) {
				hipsPosition = playerTransform.InverseTransformPoint (hips.position);
			}

			usingIKFootPreviously = canUseIkFoot;
		}

		if (!canUseIkFoot) {
			return;
		}

		hips.position = playerTransform.TransformPoint (hipsPosition);

		if (!initialPositionAssigned) {
			initialPositionAssigned = true;
		}
	}

	public override void updateOnAnimatorIKState ()
	{
		if (!IKFootSystemEnabled) {
			return;
		}

		if (!canUseIkFoot) {
			return;
		}

		playerOnGround = playerControllerManager.isPlayerOnGround ();

		newHipsOffset = 0f;

		currentTransfrormUp = playerTransform.up;

		if (playerOnGround) {
			for (i = 0; i < 2; i++) {
				currentLegInfo = legInfoList [i];
				//raycast from the foot

				currentRaycastPosition = getNewRaycastPosition (currentLegInfo.foot.position, currentLegInfo.lowerLeg.position, out currentRaycastDistance);
			
				newRaycastDistance = currentRaycastDistance + currentLegInfo.offset + currentLegInfo.maxLegLength - (extraFootOffset * 2);

				checkRaycast = true;

				if (float.IsNaN (currentRaycastPosition.x) && float.IsNaN (currentRaycastPosition.y) && float.IsNaN (currentRaycastPosition.z)) {
					checkRaycast = false;
				}

				if (checkRaycast) {
					if (Physics.Raycast (currentRaycastPosition, -currentTransfrormUp, out hit, newRaycastDistance, layerMask)) {
						currentLegInfo.raycastDistance = currentRaycastDistance;
						currentLegInfo.surfaceDistance = hit.distance;
						currentLegInfo.surfacePoint = hit.point;
						currentLegInfo.surfaceNormal = hit.normal;
					} else {
						currentLegInfo.surfaceDistance = float.MaxValue; 
					}
				} else {
					currentLegInfo.surfaceDistance = float.MaxValue;
				}

				//raycast from the toe, if a closer object is found, the raycast used is the one in the toe

				currentRaycastPosition = getNewRaycastPosition (currentLegInfo.toes.position, currentLegInfo.lowerLeg.position, out currentRaycastDistance);

				newRaycastDistance = currentRaycastDistance + currentLegInfo.offset + currentLegInfo.maxLegLength - (extraFootOffset * 2);

				checkRaycast = true;

				if (float.IsNaN (currentRaycastPosition.x) && float.IsNaN (currentRaycastPosition.y) && float.IsNaN (currentRaycastPosition.z)) {
					checkRaycast = false;
				}

				if (checkRaycast) {
					if (Physics.Raycast (currentRaycastPosition, -currentTransfrormUp, out hit, newRaycastDistance, layerMask)) {
						if (hit.distance < currentLegInfo.surfaceDistance && hit.normal == currentTransfrormUp) {
							currentLegInfo.raycastDistance = currentRaycastDistance;
							currentLegInfo.surfaceDistance = hit.distance;
							currentLegInfo.surfacePoint = hit.point;
							currentLegInfo.surfaceNormal = hit.normal;
						}
					}
				}

				if (currentLegInfo.surfaceDistance != float.MaxValue) {
					newOffset = currentLegInfo.surfaceDistance - currentLegInfo.raycastDistance - playerTransform.InverseTransformPoint (currentLegInfo.foot.position).y;

					if (newOffset > newHipsOffset) {
						newHipsOffset = newOffset;
					}
				}

				if (currentIKGoal == AvatarIKGoal.RightFoot) {
					rightFootHitPoint = currentLegInfo.surfacePoint;
				} else {
					leftFootHitPoint = currentLegInfo.surfacePoint;
				}
			}
		}

		playerIsMoving = playerControllerManager.isPlayerMoving (0);

		if (playerIsMoving) {
			newHipsOffset = Mathf.Clamp (newHipsOffset, hipsPositionDownMovingClampAmount, hipsPositionUpMovingClampAmount);
		} else {
			newHipsOffset = Mathf.Clamp (newHipsOffset, hipsPositionDownClampAmount, hipsPositionUpClampAmount);
		}

		if (IKFootPaused) {
			newHipsOffset = 0;
		}

		//set hips position
		if (initialPositionAssigned) {
			currentHipsOffset = Mathf.Lerp (currentHipsOffset, newHipsOffset, hipsMovementSpeed * Time.fixedDeltaTime);
		} else {
			currentHipsOffset = newHipsOffset;
		}

		hipsPosition = playerTransform.InverseTransformPoint (hips.position);

		hipsPosition.y -= currentHipsOffset;

		currentGravityNormal = playerControllerManager.getCurrentNormal ();

		//set position and rotation on player's feet
		for (i = 0; i < 2; i++) {
			currentLegInfo = legInfoList [i];

			currentIKGoal = currentLegInfo.IKGoal;

			newFootPosition = mainAnimator.GetIKPosition (currentIKGoal);

			newFootRotation = mainAnimator.GetIKRotation (currentIKGoal);

			targetWeight = currentLegInfo.IKWeight - 1;

			currentAdjustmentSpeed = IKWeightDisabledSpeed;

			if (playerOnGround) {
				if (currentLegInfo.surfaceDistance != float.MaxValue) {
					if (playerTransform.InverseTransformDirection (newFootPosition - currentLegInfo.surfacePoint).y - currentLegInfo.offset - extraFootOffset - currentHipsOffset < 0) {
						localFootPosition = playerTransform.InverseTransformPoint (newFootPosition);
						localFootPosition.y = playerTransform.InverseTransformPoint (currentLegInfo.surfacePoint).y;

						newFootPosition = playerTransform.TransformPoint (localFootPosition) + currentTransfrormUp * (currentLegInfo.offset + currentHipsOffset - extraFootOffset);
						newFootRotation = Quaternion.LookRotation (Vector3.Cross (currentLegInfo.surfaceNormal, newFootRotation * -Vector3.right), currentTransfrormUp);

						targetWeight = currentLegInfo.IKWeight + 1;
						currentAdjustmentSpeed = IKWeightEnabledSpeed;
					}
				}
			}

			removeIKValue = false;

			if (IKFootPaused) {
				removeIKValue = true;
			}

			if (!playerIsMoving && currentLegInfo.surfaceNormal == currentGravityNormal) {
				float footHeightDifference = Mathf.Abs (playerTransform.InverseTransformPoint (leftFootHitPoint).y) -
				                             Mathf.Abs (playerTransform.InverseTransformPoint (rightFootHitPoint).y);
				if (Mathf.Abs (footHeightDifference) < 0.02f) {
					removeIKValue = true;
				}
			}

			if (removeIKValue) {
				targetWeight = 0;
			}

			if (initialPositionAssigned) {
				currentLegInfo.IKWeight = Mathf.Clamp01 (Mathf.Lerp (currentLegInfo.IKWeight, targetWeight, currentAdjustmentSpeed * Time.fixedDeltaTime));
			} else {
				currentLegInfo.IKWeight = Mathf.Clamp01 (targetWeight);
			}
				
			currentWeightToApply = currentLegInfo.IKWeight;

			if (currentWeightToApply >= minIKValue) {

				mainAnimator.SetIKPosition (currentIKGoal, newFootPosition);
				mainAnimator.SetIKRotation (currentIKGoal, newFootRotation);

				mainAnimator.SetIKPositionWeight (currentIKGoal, currentWeightToApply);
				mainAnimator.SetIKRotationWeight (currentIKGoal, currentWeightToApply);
			}
		}
	}

	public Vector3 getNewRaycastPosition (Vector3 targetTransformPosition, Vector3 lowerLegPosition, out float newDistance)
	{
		newRaycastPosition = playerTransform.InverseTransformPoint (targetTransformPosition);
		newLocalHipsPosition = playerTransform.InverseTransformPoint (lowerLegPosition);

		newDistance = (newLocalHipsPosition.y - newRaycastPosition.y);
		newRaycastPosition.y = newLocalHipsPosition.y;

		return playerTransform.TransformPoint (newRaycastPosition);
	}

	public void setIKFootPausedState (bool state)
	{
		IKFootPaused = state;
	}

	public void setLegsInfo (Transform newHips, Transform rightLowerLeg, Transform leftLowerLeg, Transform rightFoot, Transform leftFoot, Transform rightToes, Transform leftToes)
	{
		hips = newHips;

		for (i = 0; i < legInfoList.Count; i++) {
			if (legInfoList [i].IKGoal == AvatarIKGoal.RightFoot) {
				legInfoList [i].lowerLeg = rightLowerLeg;
				legInfoList [i].foot = rightFoot;
				legInfoList [i].toes = rightToes;
			} else {
				legInfoList [i].lowerLeg = leftLowerLeg;
				legInfoList [i].foot = leftFoot;
				legInfoList [i].toes = leftToes;
			}
		}

		updateComponent ();
	}

	public void setIKFootSystemEnabledState (bool state)
	{
		IKFootSystemEnabled = state;
	}

	public void setIKFootSystemEnabledStateFromEditor (bool state)
	{
		setIKFootSystemEnabledState (state);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class legInfo
	{
		public string Name;
		public AvatarIKGoal IKGoal;
		public Transform lowerLeg;
		public Transform foot;
		public Transform toes;

		public float IKWeight;
		[HideInInspector] public float offset;
		[HideInInspector] public float maxLegLength;
		[HideInInspector] public float raycastDistance;
		[HideInInspector] public float surfaceDistance;
		[HideInInspector] public Vector3 surfacePoint;
		[HideInInspector] public Vector3 surfaceNormal;
	}
}