using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addForceToObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string playerTag = "Player";

	public bool affectToPlayerOnlyOnAIr;
	public bool affectOnlyOnParagliderActive;

	public float forceAmountOnParaglider;

	public float forceAmountCharacters;
	public float forceAmountRegularObjects;

	public ForceMode forceModeCharacters;
	public ForceMode forceModeRegularObjects;

	public bool addForceInUpdate;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<rigidbodyInfo> rigidbodyInfoList = new List<rigidbodyInfo> ();

	public bool objectsDetected;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform forceDirection;

	Vector3 forceDirectionForward;

	rigidbodyInfo currentRigidbodyInfo;

	void Update ()
	{
		if (objectsDetected) {
			forceDirectionForward = forceDirection.forward;

			for (int i = 0; i < rigidbodyInfoList.Count; i++) {

				currentRigidbodyInfo = rigidbodyInfoList [i];

				if (currentRigidbodyInfo.mainRigidbody != null) {
					if (currentRigidbodyInfo.isPlayer) {
						if (affectToPlayerOnlyOnAIr) {
							bool currentPlayerOnAir = !currentRigidbodyInfo.mainExternalControllerBehavior.isCharacterOnGround ();

							if (currentPlayerOnAir) {
								if (affectOnlyOnParagliderActive) {
									currentRigidbodyInfo.mainExternalControllerBehavior.updateExternalForceActiveState (forceDirectionForward, forceAmountOnParaglider);
								} else {
									currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountCharacters, forceModeCharacters);
								}
							}
						} else {
							currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountCharacters, forceModeCharacters);
						}
					} else {
						currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountRegularObjects, forceModeRegularObjects);
					}
				}
			}
		}
	}

	public void addNewObject (GameObject newObject)
	{
		Rigidbody mainRigidbody = newObject.GetComponent<Rigidbody> ();

		if (mainRigidbody != null) {
			for (int i = 0; i < rigidbodyInfoList.Count; i++) {
				if (rigidbodyInfoList [i].mainObject == newObject) {
					return;
				}
			}

			rigidbodyInfo newRigidbodyInfo = new rigidbodyInfo ();

			newRigidbodyInfo.mainObject = newObject;
			newRigidbodyInfo.mainRigidbody = mainRigidbody;

			if (newObject.CompareTag (playerTag)) {
				newRigidbodyInfo.isPlayer = true;

				playerComponentsManager currentPlayerComponentsManager = newObject.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					externalControllerBehavior currentExternalControllerBehavior = currentPlayerComponentsManager.getParagliderSystem ();

					if (currentExternalControllerBehavior != null) {

						newRigidbodyInfo.mainExternalControllerBehavior = currentExternalControllerBehavior;

						newRigidbodyInfo.mainExternalControllerBehavior.setExternalForceActiveState (true);
					}
				}
			}

			rigidbodyInfoList.Add (newRigidbodyInfo);

			objectsDetected = true;
		}
	}

	public void removeObject (GameObject objectToRemove)
	{
		for (int i = rigidbodyInfoList.Count - 1; i >= 0; i--) {
			if (rigidbodyInfoList [i] == null) {
				rigidbodyInfoList.RemoveAt (i);
			}
		}

		if (rigidbodyInfoList.Count == 0) {
			objectsDetected = false;
		}

		for (int i = 0; i < rigidbodyInfoList.Count; i++) {
			if (rigidbodyInfoList [i].mainObject == objectToRemove) {

				if (rigidbodyInfoList [i].isPlayer) {
					if (affectOnlyOnParagliderActive && rigidbodyInfoList [i].mainExternalControllerBehavior != null) {
						rigidbodyInfoList [i].mainExternalControllerBehavior.setExternalForceActiveState (false);
					} 
				}

				rigidbodyInfoList.RemoveAt (i);

				if (rigidbodyInfoList.Count == 0) {
					objectsDetected = false;
				}

				return;
			}
		}
	}

	public void removeAllObjects ()
	{
		for (int i = rigidbodyInfoList.Count - 1; i >= 0; i--) {
			if (rigidbodyInfoList [i] == null) {
				rigidbodyInfoList.RemoveAt (i);
			}
		}

		for (int i = 0; i < rigidbodyInfoList.Count; i++) {
			if (rigidbodyInfoList [i].isPlayer) {
				if (affectOnlyOnParagliderActive && rigidbodyInfoList [i].mainExternalControllerBehavior != null) {
					rigidbodyInfoList [i].mainExternalControllerBehavior.setExternalForceActiveState (false);
				} 
			}
		}

		rigidbodyInfoList.Clear ();

		objectsDetected = false;
	}

	[System.Serializable]
	public class rigidbodyInfo
	{
		public string Name;
		public bool isPlayer;
		public GameObject mainObject;
		public Rigidbody mainRigidbody;

		public externalControllerBehavior mainExternalControllerBehavior;
	}
}
