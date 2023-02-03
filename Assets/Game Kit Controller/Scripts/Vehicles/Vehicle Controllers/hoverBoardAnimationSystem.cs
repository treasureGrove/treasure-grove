using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hoverBoardAnimationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float extraBodyRotation = -20;
	public float extraSpineRotation = 30;
	public float limitBodyRotationX = 30;
	public float limitBodyRotationZ = 30;
	public float minSpineRotationX = 20;
	public float maxSpineRotationX = 330;
	public float maxArmsRotation = 25;

	public bool addMovementToBody;
	public bool addExtraMovementToBody;
	public bool addMovementToArms;
	public bool addMovementToHead;

	public float maxHeadRotationBackward;
	public float headRotationSpeed;
	public float minBackwardVelocityToHeadRotation;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform headLookCenter;

	public Transform bodyLookCenter;
	public Transform bodyLookPivot;

	public Rigidbody mainRigidbody;
	public hoverBoardController mainHoverBoardController;
	public Transform hoverBoardTransform;

	public Transform playerGravityCenter;

	upperBodyRotationSystem mainUpperBodyRotationSystem;

	Vector3 currentNormal;

	float gravityCenterAngleX;
	float gravityCenterAngleZ;
	float angleZ;
	float currentExtraBodyRotation;
	float currentExtraSpineRotation;
	float headLookCenterCurrentRotation;

	bool driving;

	Animator animator;
	Transform rightArm;
	Transform leftArm;

	bool firstPersonActive;

	bool animationActive;

	void Start ()
	{
		currentExtraBodyRotation = extraBodyRotation;
		currentExtraSpineRotation = extraSpineRotation;
	}

	void Update ()
	{
		if (driving) {
			firstPersonActive = mainHoverBoardController.firstPersonActive;

			if (!firstPersonActive && animationActive) {
				currentNormal = mainHoverBoardController.getNormal ();

				currentNormal = currentNormal.normalized;
				
				angleZ = Mathf.Asin (hoverBoardTransform.InverseTransformDirection (Vector3.Cross (currentNormal, hoverBoardTransform.up)).z) * Mathf.Rad2Deg;

				float angleX = Mathf.Asin (hoverBoardTransform.InverseTransformDirection (Vector3.Cross (currentNormal, hoverBoardTransform.up)).x) * Mathf.Rad2Deg;

				float gravityAngleZ = 0;

				if (Mathf.Abs (angleZ) > 1) {
					gravityAngleZ = -angleZ;
				} else {
					gravityAngleZ = 0;
				}

				float gravityAngleX = 0;

				if (Mathf.Abs (angleX) > 1) {
					gravityAngleX = -angleX;
				} else {
					gravityAngleX = 0;
				}

				gravityCenterAngleX = Mathf.Lerp (gravityCenterAngleX, gravityAngleX, Time.deltaTime * 5);
				gravityCenterAngleZ = Mathf.Lerp (gravityCenterAngleZ, gravityAngleZ, Time.deltaTime * 5);
				gravityCenterAngleX = Mathf.Clamp (gravityCenterAngleX, -limitBodyRotationX, limitBodyRotationX);
				gravityCenterAngleZ = Mathf.Clamp (gravityCenterAngleZ, -limitBodyRotationZ, limitBodyRotationZ);

				playerGravityCenter.localEulerAngles = new Vector3 (gravityCenterAngleX, currentExtraBodyRotation, gravityCenterAngleZ);

				float forwardSpeed = (mainRigidbody.transform.InverseTransformDirection (mainRigidbody.velocity).z) * 3f;
				float bodyRotation = extraBodyRotation;
				float spineRotation = extraSpineRotation;

				if (forwardSpeed < -2) {
					bodyRotation = -extraBodyRotation;
					spineRotation = -extraSpineRotation;
				} 

				currentExtraBodyRotation = Mathf.Lerp (currentExtraBodyRotation, bodyRotation, Time.deltaTime * 5);
				currentExtraSpineRotation = Mathf.Lerp (currentExtraSpineRotation, spineRotation, Time.deltaTime * 5);
			}
		}
	}

	void LateUpdate ()
	{
		if (driving && !firstPersonActive && animationActive) {
			if (addExtraMovementToBody) {
				Quaternion rotationX = Quaternion.FromToRotation (bodyLookPivot.InverseTransformDirection (hoverBoardTransform.right), 
					                       bodyLookPivot.InverseTransformDirection (hoverBoardTransform.forward));
				
				Vector3 directionX = rotationX.eulerAngles;

				Quaternion rotationZ = Quaternion.FromToRotation (bodyLookPivot.InverseTransformDirection (hoverBoardTransform.forward), 
					                       bodyLookPivot.InverseTransformDirection (hoverBoardTransform.forward));
				
				Vector3 directionZ = rotationZ.eulerAngles;

				float angleX = directionX.x;

				if (angleX > 180) {
					angleX = Mathf.Clamp (angleX, maxSpineRotationX, 360);
				} else {
					angleX = Mathf.Clamp (angleX, 0, minSpineRotationX);
				}

				bodyLookPivot.localEulerAngles = new Vector3 (angleX - angleZ, bodyLookPivot.localEulerAngles.y, directionZ.z - currentExtraSpineRotation);
			}

			if (addMovementToArms) {
				float armRotation = angleZ;
				armRotation = Mathf.Clamp (armRotation, -maxArmsRotation, maxArmsRotation);

				float rightArmRotationX = rightArm.localEulerAngles.x - armRotation;
				rightArm.localEulerAngles = new Vector3 (rightArmRotationX, rightArm.localEulerAngles.y, rightArm.localEulerAngles.z);

				float leftArmRotationX = leftArm.localEulerAngles.x + armRotation;
				leftArm.localEulerAngles = new Vector3 (leftArmRotationX, leftArm.localEulerAngles.y, leftArm.localEulerAngles.z);
			}

			if (addMovementToHead) {
				float headAngle = 0;

				if (mainHoverBoardController.motorInput < 0 &&
				    (Mathf.Abs (mainHoverBoardController.currentSpeed) > minBackwardVelocityToHeadRotation || mainHoverBoardController.motorInput < -0.8f)) {
					headAngle = maxHeadRotationBackward;
				}

				headLookCenterCurrentRotation = Mathf.Lerp (headLookCenterCurrentRotation, headAngle, Time.deltaTime * headRotationSpeed);

				headLookCenter.localEulerAngles = new Vector3 (0, headLookCenterCurrentRotation, 0);
			}
		}
	}

	//the player is getting on or off from the vehicle, so
	public void changeVehicleState (bool state)
	{
		driving = state;

		animationActive = state;

		//set the audio values if the player is getting on or off from the vehicle
		if (driving) {
			//set the same state in the gravity control components
			animator = hoverBoardTransform.GetComponentInChildren<Animator> ();

			mainUpperBodyRotationSystem = hoverBoardTransform.GetComponentInChildren<upperBodyRotationSystem> ();

			if (addMovementToBody) {
				if (mainUpperBodyRotationSystem != null) {
					mainUpperBodyRotationSystem.enableOrDisableIKUpperBody (true);

					mainUpperBodyRotationSystem.setTemporalObjectToFollow (bodyLookCenter);
				}
			}

			bodyLookPivot.localEulerAngles = Vector3.zero;

			if (animator != null) {
				rightArm = animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
				leftArm = animator.GetBoneTransform (HumanBodyBones.LeftUpperArm);
			}
		} else {
			if (addMovementToBody) {
				if (mainUpperBodyRotationSystem != null) {
					mainUpperBodyRotationSystem.enableOrDisableIKUpperBody (false);

					mainUpperBodyRotationSystem.setTemporalObjectToFollow (null);
				}
			}
		}
	}
}