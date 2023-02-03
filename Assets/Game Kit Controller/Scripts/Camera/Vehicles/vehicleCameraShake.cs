using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vehicleCameraShake : MonoBehaviour
{
	public List<bobStates> bobStatesList = new List<bobStates> ();
	public bool headBobEnabled;
	public bool shakingActive;
	public float ResetSpeed;
	public bobStates playerBobState;
	public bool externalShake;
	public string externalForceStateName;

	bool mainCameraAssigned;

	Transform mainCamera;
	Coroutine coroutineToStop;
	Coroutine externalForceCoroutine;
	float externalShakeDuration;

	float eulTargetX;
	float eulTargetY;
	float eulTargetZ;
	Vector3 eulTarget;

	bool vehicleActive;

	string currentBobStateName = "";


	Coroutine updateCoroutine;

	//set a state in the current player state
	public void startShake (string shakeName)
	{
		if (!currentBobStateName.Equals (shakeName)) {
			//search the state recieved
			for (int i = 0; i < bobStatesList.Count; i++) {
				if (bobStatesList [i].Name.Equals (shakeName)) {
					//if found, set the state values, and the enable this state as the current state
					playerBobState = bobStatesList [i];
					bobStatesList [i].isCurrentState = true;

					currentBobStateName = playerBobState.Name;
					//print ("New Shake State " + playerBobState.Name);
				} else {
					//disable all the other states
					bobStatesList [i].isCurrentState = false;
				}
			}

			shakingActive = true;

			stopUpdateCoroutine ();
		
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		//if headbod enabled, check the current state
		if (mainCameraAssigned && headBobEnabled && shakingActive && playerBobState.stateEnabled) {
			movementBob (playerBobState);
		}
	}

	public void stopShake ()
	{
		shakingActive = false;

		if (mainCameraAssigned) {
			if (coroutineToStop != null) {
				StopCoroutine (coroutineToStop);
			}

			coroutineToStop = StartCoroutine (resetCameraTransform ());
		}

		currentBobStateName = "";

		stopUpdateCoroutine ();

		//print ("stop shake");
	}

	//check the info of the current state, to apply rotation, translation, both or anything according to the parameters of the botState
	void movementBob (bobStates state)
	{
		eulTargetX = Mathf.Sin (Time.time * state.eulSpeed.x) * state.eulAmount.x;
		eulTargetY = Mathf.Cos (Time.time * state.eulSpeed.y) * state.eulAmount.y;
		eulTargetZ = Mathf.Sin (Time.time * state.eulSpeed.z) * state.eulAmount.z;

		eulTarget = new Vector3 (eulTargetX, eulTargetY, eulTargetZ);

		mainCamera.localRotation = Quaternion.Lerp (mainCamera.localRotation, Quaternion.Euler (eulTarget), Time.deltaTime * state.eulSmooth);
	}

	IEnumerator resetCameraTransform ()
	{
		if (vehicleActive) {
			float i = 0.0f;
			float rate = ResetSpeed;

			//store the current rotation
			Quaternion currentQ = mainCamera.localRotation;

			Quaternion targetRotation = Quaternion.identity;

			while (i < 1.0f) {
				//reset the position and rotation of the camera to 0,0,0
				i += Time.deltaTime * rate;

				mainCamera.localRotation = Quaternion.Lerp (currentQ, targetRotation, i);

				yield return null;
			}
		}

		yield return null;
	}

	public void getCurrentCameraTransform (Transform currentCameraTransform)
	{
		mainCamera = currentCameraTransform;

		mainCameraAssigned = mainCamera != null;
	}

	public void setExternalShakeState (externalShakeInfo shakeInfo)
	{
		startShake (externalForceStateName);

		playerBobState.eulAmount = shakeInfo.shakeRotation;
		playerBobState.eulSmooth = shakeInfo.shakeRotationSmooth;
		playerBobState.eulSpeed = shakeInfo.shakeRotationSpeed;
		externalShakeDuration = shakeInfo.shakeDuration;

		setExternalShakeDuration ();
	}

	public void setExternalShakeDuration ()
	{
		externalShake = true;

		if (externalForceCoroutine != null) {
			StopCoroutine (externalForceCoroutine);
		}

		externalForceCoroutine = StartCoroutine (setExternalShakeDurationCoroutine ());
	}

	IEnumerator setExternalShakeDurationCoroutine ()
	{
		yield return new WaitForSeconds (externalShakeDuration);

		externalShake = false;

		stopShake ();

		yield return null;			
	}

	public void setVehicleActiveState (bool state)
	{
		vehicleActive = state;
	}

	[System.Serializable]
	public class bobStates
	{
		public string Name;
		public Vector3 eulAmount;
		public Vector3 eulSpeed;
		public float eulSmooth;
		public bool stateEnabled;
		public bool isCurrentState;
	}
}