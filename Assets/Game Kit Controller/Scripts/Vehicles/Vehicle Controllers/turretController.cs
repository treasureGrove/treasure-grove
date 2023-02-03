using UnityEngine;
using System.Collections;

public class turretController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public otherVehicleParts vehicleParts;
	public vehicleSettings settings;

	float lookAngle;

	public override void vehicleUpdate ()
	{
		if (driving && settings.turretCanRotate) {
			horizontalAxis = mainInputActionManager.getPlayerMovementAxis ().x;

			if (!useHorizontalInputLerp && !touchPlatform) {
				rawAxisValues = mainInputActionManager.getPlayerRawMovementAxis ();
				horizontalAxis = rawAxisValues.x;
			}

			if (mainVehicleCameraController.currentState.useCameraSteer && horizontalAxis == 0) {
				localLook = vehicleParts.chassis.transform.InverseTransformDirection (mainVehicleCameraController.getLookDirection ());

				if (localLook.z < 0f) {
					localLook.x = Mathf.Sign (localLook.x);
				}

				steering = localLook.x;
				steering = Mathf.Clamp (steering, -1f, 1f);

				horizontalAxis = steering;
			}

			lookAngle -= horizontalAxis * settings.rotationSpeed;

			if (settings.rotationLimited) {
				lookAngle = Mathf.Clamp (lookAngle, -settings.clampTiltXTurret.x, settings.clampTiltXTurret.y);
			} 
		}
	}

	void FixedUpdate ()
	{
		if (driving && settings.turretCanRotate) {
			Quaternion targetRotation = Quaternion.Euler (0, -lookAngle, 0);

			vehicleParts.chassis.transform.localRotation = 
				Quaternion.Slerp (vehicleParts.chassis.transform.localRotation, targetRotation, Time.deltaTime * settings.smoothRotationSpeed); 
		}
	}

	//the player is getting on or off from the vehicle, so
	public override void changeVehicleState ()
	{
		driving = !driving;

		if (!driving) {
			StartCoroutine (resetTurretRotation ());

			lookAngle = 0;
		}
	}

	//the vehicle has been destroyed, so disabled every component in it
	public override void disableVehicle ()
	{
		//disable the controller
		this.enabled = false;
	}

	//reset the weapon rotation when the player gets off
	IEnumerator resetTurretRotation ()
	{
		Quaternion currentBaseYRotation = vehicleParts.chassis.transform.localRotation;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			vehicleParts.chassis.transform.localRotation = Quaternion.Slerp (currentBaseYRotation, Quaternion.identity, t);
			yield return null;
		}
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public GameObject chassis;
	}

	[System.Serializable]
	public class vehicleSettings
	{
		public bool turretCanRotate;
		public bool rotationLimited;
		public float rotationSpeed;
		public float smoothRotationSpeed = 5;
		public Vector2 clampTiltXTurret;
		public GameObject vehicleCamera;
	}
}
