using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIMountManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool mountManagerEnabled = true;

	public string remoteEventToCallAIMount = "Set Player As Target For Mount AI";

	public string remoteEventToAssignAIMount = "Assign Player To Mount AI";

	public float minDistanceToCallAIMount = 20;

	public float minDistanceToTeleportAIMount = 100;

	public LayerMask layerToTeleportRaycast;

	public float raycastDistance;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnCallAIMount;

	public UnityEvent eventOnTeleportAIMountBefore;
	public UnityEvent eventOnTeleportAIMountAfter;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool currentMountAssigned;

	public GameObject currentMountGameObject;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform teleportAIMountTargetTransform;

	public Transform playerTransform;

	playerController mountPlayerController;

	Transform currentMountTransform;

	float lastTimeAIMountAssigned;

	remoteEventSystem currentRemoteEventSystem;

	public void assignCurrentMountGameObject (GameObject newObject)
	{
		if (!mountManagerEnabled) {
			return;
		}

		StartCoroutine (assignCurrentMountGameObjectCoroutine (newObject));
	}

	IEnumerator assignCurrentMountGameObjectCoroutine (GameObject newObject)
	{
		yield return new WaitForSeconds (0.01f);

		mountPlayerController = newObject.GetComponentInChildren<playerController> ();

		if (mountPlayerController != null) {
			currentMountGameObject = mountPlayerController.gameObject;

			currentMountTransform = currentMountGameObject.transform;

			currentRemoteEventSystem = currentMountGameObject.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem != null) {
				currentRemoteEventSystem.callRemoteEventWithGameObject (remoteEventToAssignAIMount, playerTransform.gameObject);
			}

			currentMountAssigned = true;

			lastTimeAIMountAssigned = Time.time;

			if (showDebugPrint) {
				print ("Mount Assigned " + currentMountGameObject.name);
			}
		}
	}

	public void removeCurrentMountGameObject ()
	{
		if (currentMountAssigned) {
			mountPlayerController = null;

			currentMountGameObject = null;

			currentMountAssigned = false;

			lastTimeAIMountAssigned = 0;

			if (showDebugPrint) {
				print ("Mount removed ");
			}
		}
	}

	public void callCurrentMount ()
	{
		if (!mountManagerEnabled) {
			return;
		}

		if (!currentMountAssigned) {
			return;
		}

		if (Time.time < lastTimeAIMountAssigned + 1) {
			return;
		}
			
		if (currentMountGameObject == null || applyDamage.checkIfDeadOnObjectChilds (currentMountGameObject)) {
			removeCurrentMountGameObject ();

			if (showDebugPrint) {
				print ("Mount is dead, removing ");
			}

			return;
		}

		bool mountAICalled = false;

		float distanceToPlayer = GKC_Utils.distance (playerTransform.position, currentMountGameObject.transform.position);

		if (distanceToPlayer < minDistanceToCallAIMount) {
			if (currentRemoteEventSystem != null) {
				currentRemoteEventSystem.callRemoteEventWithGameObject (remoteEventToCallAIMount, playerTransform.gameObject);

				mountAICalled = true;

				if (showDebugPrint) {
					print ("Calling mount from distance on navmesh");
				}
			}
		} else if (distanceToPlayer < minDistanceToTeleportAIMount) {
			eventOnTeleportAIMountBefore.Invoke ();

			Vector3 targetPosition = teleportAIMountTargetTransform.position;
			Quaternion targetRotation = teleportAIMountTargetTransform.rotation;

			RaycastHit hit = new RaycastHit ();

			if (Physics.Raycast (targetPosition, -Vector3.up, out hit, raycastDistance, layerToTeleportRaycast)) {
				targetPosition = hit.point + hit.normal * 0.1f;
			}
				
			currentMountTransform.position = targetPosition;
			currentMountTransform.rotation = targetRotation;

			currentRemoteEventSystem.callRemoteEventWithGameObject (remoteEventToCallAIMount, playerTransform.gameObject);

			mountAICalled = true;

			if (showDebugPrint) {
				print ("teleporting mount");
			}

			eventOnTeleportAIMountAfter.Invoke ();
		}

		if (mountAICalled) {
			eventOnCallAIMount.Invoke ();
		}
	}
}
