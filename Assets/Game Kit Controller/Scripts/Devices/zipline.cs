using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zipline : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float extraDistance;
	public float speed;
	public float maxSpeed;
	public float minSpeed;

	public bool ziplineConnected = true;
	public float connectZiplineSpeed = 5;

	[Space]
	[Header ("IK Settings")]
	[Space]

	public IKZiplineInfo IKZipline;


	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingZipline;
	public bool showGizmo;

	[Space]
	[Header ("Components")]
	[Space]

	public electronicDevice deviceManager;
	public Collider trigger;

	public Transform finalPosition;
	public Transform initialPosition;
	public Transform movingTransform;
	public Transform middleLine;
	public Transform middleLinePivot;

	GameObject currentPlayer;
	GameObject currentCamera;

	float originalSpeed;
	int i;
	bool stoppedByPlayer;

	playerInputManager playerInput;
	menuPause pauseManager;
	playerController playerControllerManager;

	playerComponentsManager mainPlayerComponentsManager;

	IKSystem currentIKSystem;
	usingDevicesSystem currentUsingDevicesSystem;

	Vector3 movingTransformTargetPosition;
	bool movingForward;
	bool movingBackward;

	Vector2 axisValues;

	Coroutine ziplineMovementCoroutine;
	Coroutine connectCoroutine;


	void Start ()
	{
		if (trigger == null) {
			trigger = GetComponent<Collider> ();
		}

		if (deviceManager == null) {
			deviceManager = GetComponent<electronicDevice> ();
		}

		originalSpeed = speed;

		Vector3 heading = finalPosition.position - movingTransform.position;
		float distance = heading.magnitude;
		Vector3 direction = heading / distance;
		Quaternion targetRotation = Quaternion.LookRotation (direction, movingTransform.up);

		movingTransform.eulerAngles = new Vector3 (movingTransform.eulerAngles.x, targetRotation.eulerAngles.y, movingTransform.eulerAngles.x);

		if (!ziplineConnected) {
			heading = middleLine.position - initialPosition.position;
			distance = heading.magnitude;

			middleLinePivot.localScale = new Vector3 (1, 1, distance);

			trigger.enabled = false;
		}
	}

	void Update ()
	{
		//if the player is using the zipline, move his position from the current position to the final
		if (usingZipline) {
			axisValues = playerInput.getPlayerMovementAxis ();
			if (axisValues.y > 0) {
				speed += Time.deltaTime * speed;
			}

			if (axisValues.y < 0) {
				speed -= Time.deltaTime * speed;
			}

			speed = Mathf.Clamp (speed, minSpeed, maxSpeed);
		}
	}

	//function called when the player use the interaction button, to use the zipline
	public void activateZipLine ()
	{
		usingZipline = !usingZipline;
		//if the player press the interaction button while he stills using the zipline, stop his movement and released from the zipline
		if (!usingZipline) {
			stoppedByPlayer = true;
		}

		changeZiplineState (usingZipline);
	}

	public void changeZiplineState (bool state)
	{
		//set the current state of the player in the IKSystem component, to enable or disable the ik positions
		currentIKSystem.ziplineState (state, IKZipline);

		//enable or disable the player's capsule collider
		currentPlayer.GetComponent<Collider> ().isTrigger = state;

		//if the player is using the zipline, then
		if (state) {
			//disable the trigger of the zipline, to avoid the player remove this device from the compoenet usingDeviceSystem when he exits from its trigger
			trigger.enabled = false;
			//set the position of the object which moves throught the zipline
			movingTransform.transform.position = initialPosition.transform.position;

			//disable the player controller component
			playerControllerManager.changeScriptState (false);

			playerControllerManager.setHeadTrackCanBeUsedState (false);

			playerControllerManager.enableOrDisablePlayerControllerScript (false);

			playerControllerManager.getRigidbody ().isKinematic = true;

			//set that the player is using a device
			playerControllerManager.setUsingDeviceState (state);

			pauseManager.usingDeviceState (state);

			//make the player and the camera a child of the object which moves in the zipline 
			currentPlayer.transform.SetParent (movingTransform);
			currentPlayer.transform.localRotation = Quaternion.identity;
			currentCamera.transform.SetParent (movingTransform);
			movingTransformTargetPosition = finalPosition.transform.position;
		} else {
			//the player stops using the zipline, so release him from it
			trigger.enabled = enabled;

			playerControllerManager.getRigidbody ().isKinematic = false;
			playerControllerManager.changeScriptState (true);

			playerControllerManager.setHeadTrackCanBeUsedState (true);

			playerControllerManager.enableOrDisablePlayerControllerScript (true);

			currentUsingDevicesSystem.disableIcon ();

			playerControllerManager.setUsingDeviceState (state);

			pauseManager.usingDeviceState (state);

			currentPlayer.transform.SetParent (null);
			currentCamera.transform.SetParent (null);

			//movingTransform.transform.position = initialPosition.transform.position;
			// If the player has stopped his movement before he reaches the end of the zipline, add an extra force in the zipline direction
			if (stoppedByPlayer) {
				playerControllerManager.useJumpPlatform ((movingTransform.forward - currentPlayer.transform.up * 0.5f) * (speed * 2), ForceMode.Impulse);
			}

			currentUsingDevicesSystem.clearDeviceList ();

			deviceManager.setUsingDeviceState (false);
			movingTransformTargetPosition = initialPosition.transform.position;
		}

		speed = originalSpeed;

		stoppedByPlayer = false;

		checkZiplineMovement ();
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

			currentCamera = playerControllerManager.getPlayerCameraGameObject ();

			playerInput = mainPlayerComponentsManager.getPlayerInputManager ();

			pauseManager = mainPlayerComponentsManager.getPauseManager ();
	
			currentIKSystem = mainPlayerComponentsManager.getIKSystem ();

			currentUsingDevicesSystem = mainPlayerComponentsManager.getUsingDevicesSystem ();
		}
	}

	public void checkZiplineMovement ()
	{
		if (ziplineMovementCoroutine != null) {
			StopCoroutine (ziplineMovementCoroutine);
		}

		ziplineMovementCoroutine = StartCoroutine (ziplineMovment ());
	}

	IEnumerator ziplineMovment ()
	{
		//while the platform moves from the previous waypoint to the next, then displace it
		while (GKC_Utils.distance (movingTransform.transform.position, movingTransformTargetPosition) >= 0.1f) {
			movingTransform.transform.position = Vector3.MoveTowards (movingTransform.transform.position, movingTransformTargetPosition, Time.deltaTime * speed);

			yield return null;
		}

		if (usingZipline) {
			usingZipline = false;
			changeZiplineState (usingZipline);
		}
	}

	public void setZiplineConnectedState (bool state)
	{
		ziplineConnected = state;
		trigger.enabled = state;

		if (ziplineConnected) {
			connectZipine ();
		}
	}

	public void connectZipine ()
	{
		if (connectCoroutine != null) {
			StopCoroutine (connectCoroutine);
		}

		connectCoroutine = StartCoroutine (connectZipineCoroutine ());
	}

	IEnumerator connectZipineCoroutine ()
	{
		float scaleZ = GKC_Utils.distance (middleLine.position, finalPosition.position);
		float distance = scaleZ + (scaleZ * extraDistance) + 0.5f;

		float currentMidleLineScale = 1;

		//while the platform moves from the previous waypoint to the next, then displace it
		while (currentMidleLineScale < distance) {
			currentMidleLineScale = Mathf.MoveTowards (currentMidleLineScale, distance, Time.deltaTime * connectZiplineSpeed);
			middleLinePivot.localScale = new Vector3 (1, 1, currentMidleLineScale);

			yield return null;
		}
	}

	//draw every ik position in the editor
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			for (i = 0; i < IKZipline.IKGoals.Count; i++) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (IKZipline.IKGoals [i].position.position, 0.1f);
			}

			for (i = 0; i < IKZipline.IKHints.Count; i++) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (IKZipline.IKHints [i].position.position, 0.1f);
			}

			Gizmos.color = Color.red;
			Gizmos.DrawSphere (IKZipline.bodyPosition.position, 0.1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (initialPosition.position, finalPosition.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (initialPosition.position, 0.2f);
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere (finalPosition.position, 0.2f);

			float scaleZ = GKC_Utils.distance (middleLine.position, finalPosition.position);
			middleLinePivot.transform.localScale = new Vector3 (1, 1, scaleZ + (scaleZ * extraDistance) + 0.5f);
			middleLine.LookAt (finalPosition);
			movingTransform.position = initialPosition.position;
		}
	}

	[System.Serializable]
	public class IKZiplineInfo
	{
		public List<IKGoalsZiplinePositions> IKGoals = new List<IKGoalsZiplinePositions> ();
		public List<IKHintsZiplinePositions> IKHints = new List<IKHintsZiplinePositions> ();
		public Transform bodyPosition;
	}

	[System.Serializable]
	public class IKGoalsZiplinePositions
	{
		public string Name;
		public AvatarIKGoal limb;
		public Transform position;
	}

	[System.Serializable]
	public class IKHintsZiplinePositions
	{
		public string Name;
		public AvatarIKHint limb;
		public Transform position;
	}
}