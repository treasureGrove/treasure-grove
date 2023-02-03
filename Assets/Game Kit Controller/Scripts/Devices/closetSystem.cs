using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closetSystem : MonoBehaviour
{
	public List<closetDoorInfo> closetDoorList = new List<closetDoorInfo> ();

	public doorOpenType openType;

	public enum doorOpenType
	{
		translate,
		rotate
	}

	public bool useSounds;

	public bool showGizmo;
	public float gizmoRadius;
	public float gizmoArrowLength = 1;
	public float gizmoArrowLineLength = 2.5f;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;
	public Color gizmoLabelColor = Color.black;

	int numberOfDoors;
	GameObject currentPlayer;
	usingDevicesSystem currentUsingDevicesSystem;

	GameObject objectToAddToCloset;
	GameObject objectToRemoveFromCloset;

	bool objectsToSpawnChecked;

	void Start ()
	{
		for (int i = 0; i < closetDoorList.Count; i++) {
			closetDoorList [i].originalPosition = closetDoorList [i].doorTransform.position;
			closetDoorList [i].originalRotation = closetDoorList [i].doorTransform.rotation;
			closetDoorList [i].deviceStringActionManager = closetDoorList [i].doorTransform.GetComponentInChildren<deviceStringAction> ();

			if (useSounds) {
				closetDoorList [i].mainAudioSource = closetDoorList [i].doorTransform.GetComponent<AudioSource> ();
			}
		}
	}

	public void changeDoorOpenCloseState (Transform currentDoor)
	{
		int doorIndex = -1;

		for (int i = 0; i < closetDoorList.Count; i++) {
			if (closetDoorList [i].doorTransform == currentDoor) {
				doorIndex = i;
			}
		}

		if (doorIndex > -1) {
			closetDoorInfo currentDoorInfo = closetDoorList [doorIndex];
			//stop the coroutine to translate the camera and call it again
			if (currentDoorInfo.doorMovement != null) {
				StopCoroutine (currentDoorInfo.doorMovement);
			}

			currentDoorInfo.doorMovement = StartCoroutine (openOrCloseDoorCoroutine (currentDoorInfo));
		}
	}

	IEnumerator openOrCloseDoorCoroutine (closetDoorInfo currentDoorInfo)
	{
		Vector3 targetPosition = currentDoorInfo.originalPosition;
		Quaternion targetRotation = currentDoorInfo.originalRotation;

		if (currentDoorInfo.useCloseRotationTransform) {
			targetRotation = currentDoorInfo.closeRotationTransform.rotation;
		}

		bool openingDoor = false;

		if (!currentDoorInfo.opened) {
			if (openType == doorOpenType.translate) {
				targetPosition = currentDoorInfo.openedPosition.position;
			}

			if (openType == doorOpenType.rotate) {
				targetRotation = currentDoorInfo.rotatedPosition.rotation;
			}

			openingDoor = true;
		}

		if (useSounds) {
			GKC_Utils.checkAudioSourcePitch (currentDoorInfo.mainAudioSource);

			if (openingDoor) {
				currentDoorInfo.mainAudioSource.PlayOneShot (currentDoorInfo.openSound);
			} else {
				currentDoorInfo.mainAudioSource.PlayOneShot (currentDoorInfo.closeSound);
			}
		}

		if (openingDoor) {
			spawnObjectsOnPlace (currentDoorInfo);

			for (int j = 0; j < currentDoorInfo.objectsInDoor.Count; j++) {
				if (currentDoorInfo.objectsInDoor [j] != null) {
					if (currentDoorInfo.objectsInDoor [j].activeSelf != true) {
						currentDoorInfo.objectsInDoor [j].SetActive (true);
					}
				} else {
					currentDoorInfo.objectsInDoor.RemoveAt (j);

					j = 0;
				}
			}

			if (!currentDoorInfo.onlyOneDoor) {
				for (int i = 0; i < currentDoorInfo.othersDoors.Count; i++) {
					for (int j = 0; j < closetDoorList.Count; j++) {
						if (closetDoorList [j].doorTransform == currentDoorInfo.othersDoors [i]) {

							spawnObjectsOnPlace (closetDoorList [j]);

							for (int k = 0; k < closetDoorList [j].objectsInDoor.Count; k++) {
								if (closetDoorList [j].objectsInDoor [k] != null) {
									if (closetDoorList [j].objectsInDoor [k].activeSelf != true) {
										closetDoorList [j].objectsInDoor [k].SetActive (true);
									}
								} else {
									closetDoorList [j].objectsInDoor.RemoveAt (k);

									k = 0;
								}
							}
						}
					}
				}
			}
		} 

		if ((currentDoorInfo.canBeOpened && openingDoor) || (currentDoorInfo.canBeClosed && !openingDoor)) {
			if (currentDoorInfo.deviceStringActionManager != null) {
				currentDoorInfo.deviceStringActionManager.changeActionName (openingDoor);
			}

			if (openType == doorOpenType.translate) {
				float dist = GKC_Utils.distance (currentDoorInfo.doorTransform.position, targetPosition);
				float duration = dist / currentDoorInfo.openSpeed;
				float t = 0;

				Vector3 currentDoorPosition = currentDoorInfo.doorTransform.position;

				while (t < 1) {
					t += Time.deltaTime / duration; 

					currentDoorInfo.doorTransform.position = Vector3.Slerp (currentDoorPosition, targetPosition, t);

					yield return null;
				}
			}

			if (openType == doorOpenType.rotate) {
				float t = 0;

				Quaternion currentDoorRotation = currentDoorInfo.doorTransform.rotation;

				while (t < 1 && currentDoorInfo.doorTransform.rotation != targetRotation) {
					t += Time.deltaTime * currentDoorInfo.openSpeed; 

					currentDoorInfo.doorTransform.rotation = Quaternion.Slerp (currentDoorRotation, targetRotation, t);

					yield return null;
				}
			}

			if (openingDoor) {
				currentDoorInfo.opened = true;
				currentDoorInfo.closed = false;
			} else {
				currentDoorInfo.opened = false;
				currentDoorInfo.closed = true;
			}
		}
			
		if (!openingDoor) {
			bool totallyClosed = false;

			if (currentDoorInfo.onlyOneDoor) {
				totallyClosed = true;
			} else {
				numberOfDoors = 1;

				for (int j = 0; j < currentDoorInfo.othersDoors.Count; j++) {
					for (int i = 0; i < closetDoorList.Count; i++) {
						if (closetDoorList [i].doorTransform == currentDoorInfo.othersDoors [j] && closetDoorList [i].closed) {
							numberOfDoors++;
						}
					}
				}

				if (numberOfDoors == closetDoorList.Count) {
					totallyClosed = true;
				}
			}

//			print ("is closed: " + totallyClosed);

			if (totallyClosed) {
				//only disable these objects if they still inside the closet
				//also, other objects placed inside must be disabled
				if (currentDoorInfo.onlyOneDoor) {
					for (int j = 0; j < currentDoorInfo.objectsInDoor.Count; j++) {
						if (currentDoorInfo.objectsInDoor [j] != null) {
							removeDeviceFromList (currentDoorInfo.objectsInDoor [j]);

							if (currentDoorInfo.objectsInDoor [j].activeSelf != false) {
								currentDoorInfo.objectsInDoor [j].SetActive (false);
							}
						} else {
							currentDoorInfo.objectsInDoor.RemoveAt (j);

							j = 0;
						}
					}
				} else {
					for (int i = 0; i < closetDoorList.Count; i++) {
						for (int j = 0; j < closetDoorList [i].objectsInDoor.Count; j++) {
							if (closetDoorList [i].objectsInDoor [j] != null) {
//								print (closetDoorList [i].objectsInDoor [j].name);

								removeDeviceFromList (closetDoorList [i].objectsInDoor [j]);

								if (closetDoorList [i].objectsInDoor [j].activeSelf != false) {
									closetDoorList [i].objectsInDoor [j].SetActive (false);
								}
							} else {
								closetDoorList [i].objectsInDoor.RemoveAt (j);

								j = 0;
							}
						}
					}
				}
			}
		} 
	}

	void spawnObjectsOnPlace (closetDoorInfo currentDoorInfo)
	{
		if (currentDoorInfo.spawnObjectsInsideAtStart && !currentDoorInfo.objectsInsideSpawned) {
			for (int j = 0; j < currentDoorInfo.objectToSpawInfoList.Count; j++) {
				GameObject newObject = (GameObject)Instantiate (currentDoorInfo.objectToSpawInfoList [j].objectToSpawn, 
					                       currentDoorInfo.objectToSpawInfoList [j].positionToSpawn.position, 
					                       currentDoorInfo.objectToSpawInfoList [j].positionToSpawn.rotation);

				newObject.transform.SetParent (currentDoorInfo.objectToSpawInfoList [j].positionToSpawn);

				currentDoorInfo.objectsInDoor.Add (newObject);
			}

			currentDoorInfo.objectsInsideSpawned = true;
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			currentUsingDevicesSystem = currentPlayer.GetComponent<usingDevicesSystem> ();
		}
	}

	public void removeDeviceFromList (GameObject objecToRemove)
	{
		if (currentPlayer != null) {
			currentUsingDevicesSystem.removeDeviceFromListUsingParent (objecToRemove);
		}
	}

	public void setObjectToAddToCloset (GameObject objectToAdd)
	{
		objectToAddToCloset = objectToAdd;
	}

	public void setObjectToRemoveFromCloset (GameObject objectToRemove)
	{
		objectToRemoveFromCloset = objectToRemove;
	}


	public void addObjectToCloset (Transform doorToAdd)
	{
		simpleActionButton currentSimpleActionButton = objectToAddToCloset.GetComponent<simpleActionButton> ();

		if (currentSimpleActionButton != null) {
			
			GameObject currentObjectToAdd = currentSimpleActionButton.objectToActive;

			for (int i = 0; i < closetDoorList.Count; i++) {
				for (int j = 0; j < closetDoorList [i].objectsInDoor.Count; j++) {
					if (closetDoorList [i].objectsInDoor.Contains (currentObjectToAdd)) {
						return;
					}
				}
			}

			for (int i = 0; i < closetDoorList.Count; i++) {
				if (closetDoorList [i].doorTransform == doorToAdd && closetDoorList [i].opened) {
					if (!closetDoorList [i].objectsInDoor.Contains (currentObjectToAdd)) {
						closetDoorList [i].objectsInDoor.Add (currentObjectToAdd);

						if (openType == doorOpenType.translate) {
							currentObjectToAdd.transform.SetParent (closetDoorList [i].doorTransform);
						} else {
							currentObjectToAdd.transform.SetParent (transform);
						}

						return;
					}
				}
			}
		}
	}

	public void removeObjectFromCloset (Transform doorToAdd)
	{
		GameObject currentObjectToAdd = objectToRemoveFromCloset.GetComponent<simpleActionButton> ().objectToActive;

		for (int i = 0; i < closetDoorList.Count; i++) {
			if (closetDoorList [i].doorTransform == doorToAdd) {
				if (closetDoorList [i].objectsInDoor.Contains (currentObjectToAdd)) {
					closetDoorList [i].objectsInDoor.Remove (currentObjectToAdd);

					return;
				}
			}
		}
	}

	public void addNewDoor ()
	{
		closetDoorInfo newClosetDoorInfo = new closetDoorInfo ();

		newClosetDoorInfo.Name = "New Door";

		closetDoorList.Add (newClosetDoorInfo);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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
			for (int i = 0; i < closetDoorList.Count; i++) {
				if (closetDoorList [i].doorTransform != null) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (closetDoorList [i].doorTransform.position, gizmoRadius);

					if (openType == doorOpenType.translate) {
						if (closetDoorList [i].openedPosition != null) {
							Gizmos.color = Color.green;

							Gizmos.DrawSphere (closetDoorList [i].openedPosition.position, gizmoRadius);

							Gizmos.color = Color.white;

							Gizmos.DrawLine (closetDoorList [i].doorTransform.position, closetDoorList [i].openedPosition.position);
						}
					}

					if (openType == doorOpenType.rotate) {
						if (closetDoorList [i].rotatedPosition != null) {
							Gizmos.color = Color.green;

							GKC_Utils.drawGizmoArrow (closetDoorList [i].rotatedPosition.position, closetDoorList [i].rotatedPosition.right * gizmoArrowLineLength, 
								gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

							Gizmos.color = Color.white;
							Gizmos.DrawLine (closetDoorList [i].doorTransform.position, closetDoorList [i].rotatedPosition.position);
						}

						if (closetDoorList [i].useCloseRotationTransform) {
							Gizmos.color = Color.green;

							GKC_Utils.drawGizmoArrow (closetDoorList [i].closeRotationTransform.position, closetDoorList [i].closeRotationTransform.right * gizmoArrowLineLength, 
								gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);
							
							Gizmos.color = Color.white;

							Gizmos.DrawLine (closetDoorList [i].doorTransform.position, closetDoorList [i].closeRotationTransform.position);
						}
					}
				}
			}
		}
	}

	[System.Serializable]
	public class closetDoorInfo
	{
		public string Name;
		public float openSpeed;
		public Transform doorTransform;
		public Transform openedPosition;
		public Transform rotatedPosition;
	
		public bool canBeOpened = true;
		public bool canBeClosed = true;
		public bool opened;
		public bool closed;

		public List<GameObject> objectsInDoor = new List<GameObject> ();

		public bool spawnObjectsInsideAtStart;
		public List<objectToSpawInfo> objectToSpawInfoList = new List<objectToSpawInfo> ();
		public bool objectsInsideSpawned;

		public bool onlyOneDoor;
		public List<Transform> othersDoors = new List<Transform> ();

		public AudioClip openSound;
		public AudioClip closeSound;

		public bool useCloseRotationTransform;
		public Transform closeRotationTransform;
		[HideInInspector] public AudioSource mainAudioSource;

		[HideInInspector] public Vector3 originalPosition;
		[HideInInspector] public Quaternion originalRotation;

		[HideInInspector] public Coroutine doorMovement;

		[HideInInspector] public deviceStringAction deviceStringActionManager;
	}

	[System.Serializable]
	public class objectToSpawInfo
	{
		public GameObject objectToSpawn;
		public Transform positionToSpawn;
	}
}
