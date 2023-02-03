using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class applyEffectOnArea : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool effectEnabled = true;

	public bool effectActive;

	public bool useEffectRate;
	public float effectRate;

	public float effectAmount;

	public bool disableEffectAreaTriggerOnDisableArea = true;

	public bool applyValueAtOnce;

	public float areaEffectDuration;

	public bool activateAreaEffectAtStart;

	public bool applyEffectOnExit;

	[Space]
	[Header ("Object Detection Settings")]
	[Space]

	public bool useColliderTriggerForDetection = true;

	public bool useOverlapSphereForDetection;
	public float overlapSphereRadius;

	public Transform overlapCenterPosition;

	public bool useOverlapOnUpdate;

	public bool checkIfObjectHasRigidbody = true;

	[Space]
	[Header ("Objects To Affect Settings")]
	[Space]

	public LayerMask layerToAffect;
	public List<string> tagToAffectList = new List<string> ();

	public bool useObjectsToIgnoreList;
	public List<GameObject> objectsToIgnoreList = new List<GameObject> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public List<GameObject> detectedObjectList = new List<GameObject> ();

	public List<playerStatsSystem> playerStatsSystemList = new List<playerStatsSystem> ();

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> removeEventNameList = new List<string> ();

	[Space]
	[Header ("Stats To Affect Settings")]
	[Space]

	public List<statInfo> statInfoList = new List<statInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnEffectActive;
	public UnityEvent eventOnEffectDeactivate;

	public bool sendObjectsDetectedOnEvent;
	public eventParameters.eventToCallWithGameObject eventToSendObjectsDetected;

	[Space]
	[Header ("Components")]
	[Space]

	public Collider mainCollider;

	bool objectsInside;
	float lastTime;
	[HideInInspector] public float valueToAdd;

	GameObject currentObject;

	Coroutine areaDurationCoroutine;

	void Start ()
	{
		if (activateAreaEffectAtStart) {
			if (areaEffectDuration > 0) {
				setEffectActiveStateWithDuration ();
			} else {
				setEffectActiveState (true);
			}
		}
	}

	void Update ()
	{
		if (!effectActive) {
			return;
		}

		if (objectsInside) {

			if (useEffectRate) {
				if (Time.time > lastTime + effectRate) {
					lastTime = Time.time;

					checkEffectToDetectedObjects ();

					applyEffectOnStats ();
				} 
			}
		}

		if (useOverlapOnUpdate) {
			checkObjectsWithOverlap ();
		}
	}

	public void checkEffectToDetectedObjects ()
	{
		for (int i = 0; i < detectedObjectList.Count; i++) {
			currentObject = detectedObjectList [i];

			if (currentObject != null) {
				valueToAdd = effectAmount;

				applyEffect (currentObject);
			}
		}
	}

	public virtual void applyEffect (GameObject objectToAffect)
	{
		if (sendObjectsDetectedOnEvent) {
			eventToSendObjectsDetected.Invoke (objectToAffect);
		}
	}

	public virtual void checkApplyEffectOnExit (GameObject objectToAffect)
	{

	}

	public void applyEffectOnStats ()
	{
		for (int i = 0; i < playerStatsSystemList.Count; i++) {
			for (int k = 0; k < statInfoList.Count; k++) {
				playerStatsSystemList [i].addOrRemovePlayerStatAmount (statInfoList [k].Name, statInfoList [k].amountToAdd);
			}
		}
	}

	public void setObjectToCheckManually (GameObject objectToCheck)
	{
		Collider newCollider = objectToCheck.GetComponent<Collider> ();

		if (newCollider != null) {
			OnTriggerEnter (newCollider);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (useColliderTriggerForDetection) {
			checkTriggerInfo (col, true);
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (useColliderTriggerForDetection) {
			checkTriggerInfo (col, false);
		}
	}

	public void checkObjectsWithOverlap ()
	{
		if (useOverlapSphereForDetection) {
			if (overlapCenterPosition == null) {
				overlapCenterPosition = transform;
			}

			//if the power selected is push objects, check the objects close to pushObjectsCenter and add force to them in camera forward direction
			Collider[] colliders = Physics.OverlapSphere (overlapCenterPosition.position, overlapSphereRadius, layerToAffect);

			for (int i = 0; i < colliders.Length; i++) {
				if (!colliders [i].isTrigger) {
					checkTriggerInfo (colliders [i], true);
				}
			}
		}
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (isEnter) {
			if (tagToAffectList.Contains (col.gameObject.tag) || ((1 << col.gameObject.layer & layerToAffect.value) == 1 << col.gameObject.layer)) {
				if (useObjectsToIgnoreList) {
					if (objectsToIgnoreList.Contains (col.gameObject)) {
						return;
					}
				}

				GameObject objectDetected = applyDamage.getCharacterOrVehicle (col.gameObject);

				if (objectDetected != null) {
					if (!detectedObjectList.Contains (objectDetected)) {
						detectedObjectList.Add (objectDetected);

						if (showDebugPrint) {
							print ("Object detected " + objectDetected.name);
						}

						playerComponentsManager currentPlayerComponetsManager = objectDetected.GetComponent<playerComponentsManager> ();

						if (currentPlayerComponetsManager != null) {
							playerStatsSystem currentPlayerStatsSystem = currentPlayerComponetsManager.getPlayerStatsSystem ();

							if (currentPlayerStatsSystem != null) {
								playerStatsSystemList.Add (currentPlayerStatsSystem);
							}
						}

						objectsInside = true;

						if (useRemoteEventOnObjectsFound) {
							remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

							if (currentRemoteEventSystem == null && currentPlayerComponetsManager != null) {
								currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();
							}

							if (currentRemoteEventSystem != null) {
								for (int i = 0; i < removeEventNameList.Count; i++) {

									currentRemoteEventSystem.callRemoteEvent (removeEventNameList [i]);
								}
							}
						}

						if (!useEffectRate) {
							checkEffectToDetectedObjects ();

							applyEffectOnStats ();

							removeDetectedObject (objectDetected);
						}
					}
				} else {
					if (!checkIfObjectHasRigidbody || col.gameObject.GetComponent<Rigidbody> ()) {
						objectDetected = col.gameObject;

						if (!detectedObjectList.Contains (objectDetected)) {
							detectedObjectList.Add (objectDetected);

							if (showDebugPrint) {
								print ("Object detected " + objectDetected.name);
							}

							objectsInside = true;

							if (useRemoteEventOnObjectsFound) {
								remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

								if (currentRemoteEventSystem != null) {
									for (int i = 0; i < removeEventNameList.Count; i++) {

										currentRemoteEventSystem.callRemoteEvent (removeEventNameList [i]);
									}
								}
							}

							if (!useEffectRate) {
								checkEffectToDetectedObjects ();

								applyEffectOnStats ();

								removeDetectedObject (objectDetected);
							}
						}
					}
				}
			}
		} else {
			if (detectedObjectList.Contains (col.gameObject)) {
				int objectToRemoveIndex = detectedObjectList.IndexOf (col.gameObject);

				checkApplyEffectOnExit (col.gameObject);

				if (detectedObjectList.Count > objectToRemoveIndex) {
					detectedObjectList.RemoveAt (objectToRemoveIndex);
				}

				playerComponentsManager currentPlayerComponetsManager = col.gameObject.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponetsManager != null) {
					playerStatsSystem currentPlayerStatsSystem = currentPlayerComponetsManager.getPlayerStatsSystem ();

					if (currentPlayerStatsSystem != null) {
						for (int i = 0; i < playerStatsSystemList.Count; i++) {
							if (playerStatsSystemList [i] == currentPlayerStatsSystem) {
								playerStatsSystemList.RemoveAt (i);
							}
						}
					}
				}

				if (detectedObjectList.Count == 0) {
					objectsInside = false;
				}
			}
		}
	}

	public void toggleEffectActive ()
	{
		setEffectActiveState (!effectActive);
	}

	public void setEffectActiveState (bool state)
	{
		if (!effectEnabled) {
			return;
		}

		effectActive = state;

		if (state || disableEffectAreaTriggerOnDisableArea) {
			if (mainCollider != null) {
				mainCollider.enabled = state;

				if (showDebugPrint) {
					print ("Setting main collider on area effect as " + state);
				}
			}
		}

		detectedObjectList.Clear ();

		playerStatsSystemList.Clear ();

		objectsInside = false;

		lastTime = 0;

		if (effectActive) {
			eventOnEffectActive.Invoke ();
		} else {
			eventOnEffectDeactivate.Invoke ();
		}

		if (effectActive) {
			if (!useOverlapOnUpdate) {
				checkObjectsWithOverlap ();
			}
		}
	}

	public void removeDetectedObject (GameObject objectToRemove)
	{
		if (detectedObjectList.Contains (objectToRemove)) {
			int objectToRemoveIndex = detectedObjectList.IndexOf (objectToRemove);

			if (detectedObjectList.Count > objectToRemoveIndex) {
				detectedObjectList.RemoveAt (objectToRemoveIndex);
			}

			playerComponentsManager currentPlayerComponetsManager = objectToRemove.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponetsManager != null) {
				playerStatsSystem currentPlayerStatsSystem = currentPlayerComponetsManager.getPlayerStatsSystem ();

				if (currentPlayerStatsSystem != null) {
					for (int i = 0; i < playerStatsSystemList.Count; i++) {
						if (playerStatsSystemList [i] == currentPlayerStatsSystem) {
							playerStatsSystemList.RemoveAt (i);
						}
					}
				}
			}

			if (detectedObjectList.Count == 0) {
				objectsInside = false;
			}
		}
	}

	public void setEffectActiveStateWithDuration ()
	{
		if (areaDurationCoroutine != null) {
			StopCoroutine (areaDurationCoroutine);
		}

		areaDurationCoroutine = StartCoroutine (setEffectActiveStateWithDurationCoroutine ());
	}

	IEnumerator setEffectActiveStateWithDurationCoroutine ()
	{
		setEffectActiveState (true);

		yield return new WaitForSeconds (areaEffectDuration);

		setEffectActiveState (false);
	}

	[System.Serializable]
	public class statInfo
	{
		public string Name;
		public float amountToAdd;
	}
}