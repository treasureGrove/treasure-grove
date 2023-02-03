using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class spawnObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool spawnObjectEnabled = true;
	public GameObject objectToSpawn;
	public Transform spawnPosition;
	public float radiusToSpawn;

	public bool setObjectRotation = true;

	public bool setObjectScale;
	public Vector3 newObjectScale;

	public bool spawnAmountOfSameObject;
	public int amountOfSameObjectToSpawn;

	public bool spawnObjectsOnStart;

	[Space]
	[Header ("Spawn Local Position And Rotation Settings")]
	[Space]

	public bool useSpawnReferencePosition;
	public Transform spawnReferencePosition;
	public bool setSpawnedObjectParent;
	public Transform spawnedObjectParent;

	[Space]
	[Header ("Spawn Raycast Settings")]
	[Space]

	public bool useRaycastToSpawnObject;
	public LayerMask layerToSpawnObject;
	public bool useInfiniteRaycastDistance;
	public float raycastDistance;
	public Vector3 raycastSpawnObjectOffset;
	public bool launchRaycastToDownDirection;

	public bool adjustSpawnedObjectToSurfaceNormal;

	[Space]
	[Header ("Spawn List Of Objects Settings")]
	[Space]

	public bool spawnObjectList;
	public List<GameObject> objectListToSpawn = new List<GameObject> ();

	[Space]
	[Header ("Store Objects Settings")]
	[Space]

	public bool storeSpawnedObjects;
	public List<GameObject> spawnedObjectList = new List<GameObject> ();

	[Space]
	[Header ("Spawn Limit Settings")]
	[Space]

	public bool useSpawnLimitAmount;
	public int spawnLimitAmount;

	public bool checkIfObjectsSpawnedAreDead;

	public UnityEvent eventOnLimitReached;
	public UnityEvent eventOnLimitNotReached;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool sendSpawnedObjectOnEvent;
	public eventParameters.eventToCallWithGameObject eventToSendSPawnedObject;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool sendSpawnedObjectOnRemoteEvent;
	public string remoteEventToSendObject;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentNumberOfSpawnObjects;

	GameObject lastObjectSpawned;

	Vector3 raycastNormal;


	void Start ()
	{
		if (spawnObjectsOnStart) {
			activateSpawnObject ();
		}
	}

	public void activateSpawnObject ()
	{
		if (!spawnObjectEnabled) {
			return;
		}

		if (spawnObjectList) {
			for (int i = 0; i < objectListToSpawn.Count; i++) {
				if (objectListToSpawn [i] != null) {
					createObject (objectListToSpawn [i]);
				} else {
					print ("WARNING: There is no any object to spawn configured in the spawn objects component, make " +
					"sure to apply the proper settings to spawn the object.");
				}
			}

		} else {
			if (objectToSpawn != null) {
				if (spawnAmountOfSameObject) {
					for (int i = 0; i < amountOfSameObjectToSpawn; i++) {	
						createObject (objectToSpawn);
					}
				} else {
					createObject (objectToSpawn);
				}
			} else {
				print ("WARNING: There is no any object to spawn configured in the spawn objects component, make " +
				"sure to apply the proper settings to spawn the object.");
			}
		}
	}

	public void createObject (GameObject newObject)
	{
		if (useSpawnLimitAmount) {
			checkIfSpawnObjectsListNull ();

			if (currentNumberOfSpawnObjects >= spawnLimitAmount) {
				return;
			}
		}

		if (spawnPosition == null) {
			spawnPosition = transform;
		}

		Vector3 positionToSpawn = getPositionToSpawnObject ();

		Quaternion objectRotation = getRotationToSpawnObject ();

		GameObject objectToSpawnClone = (GameObject)Instantiate (newObject, positionToSpawn, objectRotation);

		objectToSpawnClone.name = objectToSpawn.name;

		if (setSpawnedObjectParent) {
			objectToSpawnClone.transform.SetParent (spawnedObjectParent);
		}

		if (useSpawnReferencePosition) {
			objectToSpawnClone.transform.localPosition = spawnReferencePosition.localPosition;
			objectToSpawnClone.transform.localRotation = spawnReferencePosition.localRotation;
		}

		if (setObjectScale) {
			objectToSpawnClone.transform.localScale = newObjectScale;
		}

		objectToSpawnClone.SetActive (true);

		if (adjustSpawnedObjectToSurfaceNormal) {

			if (raycastNormal != Vector3.zero) {

				Vector3 myForward = Vector3.Cross (objectToSpawnClone.transform.right, raycastNormal);
				Quaternion targetRotation = Quaternion.LookRotation (myForward, raycastNormal); 

				objectToSpawnClone.transform.rotation = targetRotation;
			}
		}
			
		if (storeSpawnedObjects) {
			spawnedObjectList.Add (objectToSpawnClone);

			lastObjectSpawned = objectToSpawnClone;
		}

		if (useSpawnLimitAmount) {
			checkIfSpawnObjectsListNull ();

			checkEventsOnSpawnLimits ();
		}

		if (sendSpawnedObjectOnEvent) {
			eventToSendSPawnedObject.Invoke (objectToSpawnClone);
		}

		checkRemoteEventsOnObject (objectToSpawnClone);

		raycastNormal = Vector3.zero;
	}

	void checkRemoteEventsOnObject (GameObject objectToCheck)
	{
		if (sendSpawnedObjectOnRemoteEvent) {
			remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem != null) {
				currentRemoteEventSystem.callRemoteEventWithGameObject (remoteEventToSendObject, gameObject);
			}
		}
	}

	Vector3 getPositionToSpawnObject ()
	{
		Vector3 positionToSpawn = spawnPosition.position;

		if (radiusToSpawn > 0) {
			Vector2 circlePosition = Random.insideUnitCircle * radiusToSpawn;
			Vector3 newSpawnPosition = new Vector3 (circlePosition.x, 0, circlePosition.y);
			positionToSpawn += newSpawnPosition;
		}

		raycastNormal = Vector3.zero;

		if (useRaycastToSpawnObject) {
			RaycastHit hit = new RaycastHit ();

			float newRaycastDistance = raycastDistance;

			if (useInfiniteRaycastDistance) {
				newRaycastDistance = Mathf.Infinity;
			}

			Vector3 raycastDirection = -spawnPosition.up;

			if (launchRaycastToDownDirection) {
				raycastDirection = -Vector3.up;
			}

			if (Physics.Raycast (positionToSpawn, raycastDirection, out hit, newRaycastDistance, layerToSpawnObject)) {
				positionToSpawn = hit.point + hit.normal * 0.1f + raycastSpawnObjectOffset;

				raycastNormal = hit.normal;
			}
		}


		return positionToSpawn;
	}

	Quaternion getRotationToSpawnObject ()
	{
		Quaternion objectRotation = Quaternion.identity;

		if (setObjectRotation) {
			objectRotation = spawnPosition.rotation;
		}

		return objectRotation;
	}

	public void checkIfSpawnObjectsListNull ()
	{
		for (int i = spawnedObjectList.Count - 1; i >= 0; i--) {
			if (spawnedObjectList [i] == null) {
				spawnedObjectList.RemoveAt (i);
			} else {
				if (checkIfObjectsSpawnedAreDead) {
					if (applyDamage.checkIfDeadOnObjectChilds (spawnedObjectList [i])) {
						spawnedObjectList.RemoveAt (i);
					}
				}
			}
		}

		currentNumberOfSpawnObjects = spawnedObjectList.Count;
	}

	public void removeSpawnedObjectFromList (GameObject objectToCheck)
	{
		for (int i = 0; i < spawnedObjectList.Count; i++) {	
			if (spawnedObjectList [i] == objectToCheck) {
				spawnedObjectList.RemoveAt (i);
			}
		}

		currentNumberOfSpawnObjects = spawnedObjectList.Count;
	}

	public GameObject getLastObjectSpawned ()
	{
		return lastObjectSpawned;
	}

	public void checkEventsOnSpawnLimits ()
	{
		if (currentNumberOfSpawnObjects >= spawnLimitAmount) {
			eventOnLimitReached.Invoke ();
		} else {
			eventOnLimitNotReached.Invoke ();
		}
	}

	public void destroySpawnedObjects ()
	{
		if (storeSpawnedObjects) {
			for (int i = 0; i < spawnedObjectList.Count; i++) {
				if (spawnedObjectList [i] != null) {
					Destroy (spawnedObjectList [i]);
				}
			}

			spawnedObjectList.Clear ();
		}
	}

	public void setAmountOfSameObjectToSpawn (int newValue)
	{
		amountOfSameObjectToSpawn = newValue;
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

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = Color.yellow;
			if (spawnPosition != null) {
				Gizmos.DrawWireSphere (spawnPosition.position, radiusToSpawn);
			} else {
				Gizmos.DrawWireSphere (transform.position, radiusToSpawn);
			}
		}
	}
}