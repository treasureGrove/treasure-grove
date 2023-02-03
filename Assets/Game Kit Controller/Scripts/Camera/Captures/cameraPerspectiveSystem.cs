using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cameraPerspectiveSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<GameObject> objectToLookList = new List<GameObject> ();
	public float maxDistance;
	public float maxAngle;

	public float proximitySoundRate;
	public float maxPitchValue = 3;

	public LayerMask layerForUsers;

	public bool activateObjectOnCapture;
	public List<GameObject> objectToActiveList = new List<GameObject> ();

	public bool disableObjectOnCapture;
	public List<GameObject> objectToDisableList = new List<GameObject> ();

	[Space]
	[Header ("Sounds Settings")]
	[Space]

	public bool useCaptureSound;
	public AudioClip captureSound;

	public bool useProximitySound;
	public AudioClip proximitySound;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool captureTakenCorrectly;
	public bool playerInside;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventFunction;
	public UnityEvent eventFunction = new UnityEvent ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public float gizmoArrowLength = 1;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform positionToStay;
	public Transform positionToLook;

	public AudioSource bipAudioSource;
	public AudioSource captureAudioSource;

	GameObject currentPlayer;
	smartphoneDevice smartphoneDeviceManager;
	float lastTimePlayed;
	float triggerDistance;
	float distancePercentage = 1;
	float soundRate;

	Vector3 screenPoint;

	bool usingScreenSpaceCamera;

	bool targetOnScreen;

	playerComponentsManager mainPlayerComponentsManager;

	float screenWidth;
	float screenHeight;

	void Update ()
	{
		if (!captureTakenCorrectly) {
			if (playerInside && useProximitySound) {
				if (Time.time > lastTimePlayed + soundRate) {
					float currentDistance = GKC_Utils.distance (currentPlayer.transform.position, positionToStay.position);
					distancePercentage = currentDistance / triggerDistance;

					float pitchValue = 1 + ((1 - distancePercentage) * (maxPitchValue - 1));

					if (pitchValue <= 0) {
						pitchValue = 0.1f;
					}

					if (pitchValue > maxPitchValue) {
						pitchValue = maxPitchValue;
					}

					bipAudioSource.pitch = pitchValue;
					bipAudioSource.PlayOneShot (proximitySound);
					lastTimePlayed = Time.time;
					soundRate = distancePercentage;
				}
			}
		}
	}

	public void checkCurrentPlayerPosition (Transform playerPosition, Transform cameraTransform, Camera deviceCamera)
	{
		if (captureTakenCorrectly) {
			return;
		}

		float currentDistance = GKC_Utils.distance (playerPosition.position, positionToStay.position);
		if (currentDistance <= maxDistance) {

			Vector3 targetDir = positionToLook.position - cameraTransform.position;
			targetDir = targetDir.normalized;

			float dot = Vector3.Dot (targetDir, cameraTransform.forward);
			float currentAngleZ = Mathf.Acos (dot) * Mathf.Rad2Deg;  

			print (currentAngleZ);

			if (Mathf.Abs (currentAngleZ) < maxAngle) {  
				bool allObjectsInCamera = true;

				if (!usingScreenSpaceCamera) {
					screenWidth = Screen.width;
					screenHeight = Screen.height;
				}

				for (int i = 0; i < objectToLookList.Count; i++) {
					if (usingScreenSpaceCamera) {
						screenPoint = deviceCamera.WorldToViewportPoint (objectToLookList [i].transform.position);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
					} else {
						screenPoint = deviceCamera.WorldToScreenPoint (objectToLookList [i].transform.position);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
					}

					if (!targetOnScreen) {
						allObjectsInCamera = false;
					}
				}

				if (allObjectsInCamera) {
					playSound ();

					if (activateObjectOnCapture) {
						for (int i = 0; i < objectToActiveList.Count; i++) {
							objectToActiveList [i].SetActive (true);
						}
					}

					if (disableObjectOnCapture) {
						for (int i = 0; i < objectToDisableList.Count; i++) {
							objectToDisableList [i].SetActive (false);
						}
					}

					if (useEventFunction) {
						if (eventFunction.GetPersistentEventCount () > 0) {
							eventFunction.Invoke ();
						}
					}

					captureTakenCorrectly = true;

					if (smartphoneDeviceManager) {
						smartphoneDeviceManager.removeCurrentPerspectiveSystem ();
					}

					print ("capture done correctly");
				}
			}
		}
	}


	public void playSound ()
	{
		if (useCaptureSound) {
			GKC_Utils.checkAudioSourcePitch (captureAudioSource);
			captureAudioSource.PlayOneShot (captureSound);
		}
	}

	//check when the player enters or exits of the trigger in the device
	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if ((1 << col.gameObject.layer & layerForUsers.value) == 1 << col.gameObject.layer) {
			//if the player is entering in the trigger
			if (isEnter) {
				if (playerInside) {
					return;
				}
			
				currentPlayer = col.gameObject;

				mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

				smartphoneDeviceManager = currentPlayer.GetComponentInChildren<smartphoneDevice> ();

				if (smartphoneDeviceManager == null) {
					smartphoneDeviceManager = mainPlayerComponentsManager.getPlayerCamera ().gameObject.GetComponentInChildren<smartphoneDevice> ();
				}

				if (smartphoneDeviceManager) {
					smartphoneDeviceManager.setCurrentPerspectiveSystem (this);
				}

				usingScreenSpaceCamera = mainPlayerComponentsManager.getPlayerCamera ().isUsingScreenSpaceCamera ();

				playerInside = true;
				triggerDistance = GKC_Utils.distance (currentPlayer.transform.position, positionToStay.position);
				bipAudioSource.pitch = 1;

			} else {
				//if the player is leaving the trigger

				//if the player is the same that was using the device, the device can be used again
				if (col.gameObject == currentPlayer) {
					currentPlayer = null;
					playerInside = false;
				}
			}
		}
	}

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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			if (positionToStay && positionToLook) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (positionToStay.position, 0.3f);
				Gizmos.DrawSphere (positionToLook.position, 0.3f);

				Gizmos.DrawLine (positionToStay.position, positionToLook.position);

				Vector3 direction = positionToLook.position - positionToStay.position;
				direction = direction / direction.magnitude;
				float distance = GKC_Utils.distance (positionToLook.position, positionToStay.position);
				GKC_Utils.drawGizmoArrow (positionToStay.position, direction * distance, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

				positionToLook.rotation = Quaternion.LookRotation (direction);
			}
		}
	}
}