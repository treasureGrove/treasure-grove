using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swimTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool swimZoneActive = true;

	[Space]
	[Header ("Vehicle Settings")]
	[Space]

	public bool checkVehiclesOnEnteringWater;
	public bool disableVehiclesInteractionOnEnterWater;
	public bool ejectPassengersOnVehicleOnEnterWater;
	public bool explodeVehiclesAfterXTime;
	public float timeToExplodeVehiclesAfterXTime;

	public bool setVehicleGravityForce;
	public float newVehicleGravityForce;

	public bool reduceVehicleSpeedOnEnterWater;
	public float reducedVehicleSpeedMultiplier;

	public LayerMask vehicleLayerToCheck;

	[Space]
	[Space]

	public bool checkVehiclesOnExitingWater;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEvents;
	public bool useRemoteEventOnSwimStart;
	public List<string> remoteEventNameListOnSwimStart = new List<string> ();

	public bool useRemoteEventOnSwimEnd;
	public List<string> remoteEventNameListOnSwimEnd = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public List<GameObject> vehicleDetectedList = new List<GameObject> ();

	public bool showGizmo;
	public Color gizmoColor = Color.red;
	public float gizmoRadius = 0.1f;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform swimZoneTransform;

	GameObject currentPlayer;


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
		if (!swimZoneActive) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {

			if (isEnter) {
				if (checkVehiclesOnEnteringWater) {
					GameObject objectToCheck = col.gameObject;
			
					if ((1 << objectToCheck.layer & vehicleLayerToCheck.value) == 1 << objectToCheck.layer) {

						GameObject vehicleGameObject = applyDamage.getVehicle (objectToCheck);

						if (vehicleGameObject != null) {
							if (vehicleDetectedList.Contains (vehicleGameObject)) {
								return;
							} else {
								vehicleDetectedList.Add (vehicleGameObject);
							}
						}

						if (disableVehiclesInteractionOnEnterWater) {
							applyDamage.setVehicleInteractionTriggerState (objectToCheck, false);
						}

						if (ejectPassengersOnVehicleOnEnterWater) {
							applyDamage.ejectAllPassengersFromVehicle (objectToCheck);
						}

						if (explodeVehiclesAfterXTime) {
							applyDamage.activateSelfDestructionOnVehicleExternally (objectToCheck, timeToExplodeVehiclesAfterXTime);
						}

						if (setVehicleGravityForce) {
							applyDamage.setNewVehicleGravityForce (objectToCheck, newVehicleGravityForce);
						}

						if (reduceVehicleSpeedOnEnterWater) {
							applyDamage.setReducedVehicleSpeed (objectToCheck, reducedVehicleSpeedMultiplier);
						}

						checkRemoteEvents (true, objectToCheck);
					}
				} 
			} else {
				if (checkVehiclesOnExitingWater) {
					GameObject objectToCheck = col.gameObject;

					if ((1 << objectToCheck.layer & vehicleLayerToCheck.value) == 1 << objectToCheck.layer) {

						GameObject vehicleGameObject = applyDamage.getVehicle (objectToCheck);

						if (vehicleGameObject != null) {
							if (!vehicleDetectedList.Contains (vehicleGameObject)) {
								return;
							} else {
								vehicleDetectedList.Remove (vehicleGameObject);
							}
						}

						if (setVehicleGravityForce) {
							applyDamage.setOriginalGravityForce (objectToCheck);
						}

						checkRemoteEvents (false, objectToCheck);
					}
				}
			}

			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior swimExternalControllerBehavior = currentPlayerComponentsManager.getSwimExternalControllerBehavior ();

				if (swimExternalControllerBehavior != null) {
					swimSystem currentSwimSystem = swimExternalControllerBehavior.GetComponent<swimSystem> ();

					currentSwimSystem.setSwimZoneTransform (swimZoneTransform);

					currentSwimSystem.setSwimSystemActivestate (true);

					checkRemoteEvents (true, currentPlayer);
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior swimExternalControllerBehavior = currentPlayerComponentsManager.getSwimExternalControllerBehavior ();

				if (swimExternalControllerBehavior != null) {
					swimSystem currentSwimSystem = swimExternalControllerBehavior.GetComponent<swimSystem> ();

					currentSwimSystem.setSwimSystemActivestate (false);

					currentSwimSystem.setSwimZoneTransform (null);

					checkRemoteEvents (false, currentPlayer);
				}
			}
		}
	}

	void checkRemoteEvents (bool state, GameObject objectToCheck)
	{
		if (!useRemoteEvents) {
			return;
		}

		if (state) {
			if (useRemoteEventOnSwimStart) {
				remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();
			
				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnSwimStart.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnSwimStart [i]);
					}
				}
			}
		} else {
			if (useRemoteEventOnSwimEnd) {
				remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnSwimEnd.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnSwimEnd [i]);
					}
				}
			}
		}
	}

	//Draw gizmos
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
			if (swimZoneTransform != null) {
				Gizmos.color = gizmoColor;

				Vector3 position = swimZoneTransform.position;
				Gizmos.DrawSphere (position, gizmoRadius);

				Debug.DrawRay (position, transform.position, Color.white);
			}
		}
	}
}
