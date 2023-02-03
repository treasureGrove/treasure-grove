using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class objectDistributorSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool distributorEnabled = true;
	public GameObject objectToSpawn;

	public bool useSpawnLimit;
	public int spawnLimit;
	public int currentUnitsAmount;

	public bool useUnitsText;
	public TextMesh unitsText;

	public Transform positionToSpawn;
	public string animationName;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnOpenDistributor;
	public UnityEvent eventOnCloseDistributor;

	public UnityEvent eventOnEmptyUnits;

	public bool useEventOnObjectInstatiated;
	public eventParameters.eventToCallWithGameObject evenOnObjectInstantiated;

	[Space]
	[Header ("Debug")]
	[Space]

	public GameObject currentObjectSpawned;
	public bool objectSpawned;

	[Space]
	[Header ("Components")]
	[Space]

	public Animation mainAnimation;

	public Collider mainCollider;

	public electronicDevice electronicDeviceManager;

	GameObject currentPlayer;
	usingDevicesSystem usingDevicesManager;

	Coroutine playAnimationCoroutine;

	void Start ()
	{
		if (useSpawnLimit) {
			currentUnitsAmount = spawnLimit;

			if (useUnitsText) {
				unitsText.text = currentUnitsAmount.ToString ();
			}
		} else {
			unitsText.gameObject.SetActive (false);
		}
	}

	public void spawnObject ()
	{
		if (!distributorEnabled) {
			return;
		}

		if (useSpawnLimit) {
			currentUnitsAmount--;

			if (useUnitsText) {
				unitsText.text = currentUnitsAmount.ToString ();
			}
		}

		stopDistributorCoroutine ();

		playAnimationCoroutine = StartCoroutine (spawnObjectCoroutine ());
	}

	public void stopDistributorCoroutine ()
	{
		if (playAnimationCoroutine != null) {
			StopCoroutine (playAnimationCoroutine);
		}
	}

	IEnumerator spawnObjectCoroutine ()
	{
		objectSpawned = true;

		//yield return new WaitForSeconds (0.1f);

		removeDeviceFromList ();

		mainCollider.enabled = false;

		eventOnOpenDistributor.Invoke ();

		currentObjectSpawned = (GameObject)Instantiate (objectToSpawn, positionToSpawn.position, positionToSpawn.rotation);

		yield return new WaitForSeconds (0.01f);

		if (useEventOnObjectInstatiated) {
			evenOnObjectInstantiated.Invoke (currentObjectSpawned);
		}

		while (mainAnimation.IsPlaying (animationName)) {
			yield return null;
		}
	}

	public void OnTriggerExit (Collider col)
	{
		if (objectSpawned) {
			if (col.gameObject == currentObjectSpawned) {
				objectSpawned = false;

				currentObjectSpawned = null;

				stopDistributorCoroutine ();

				playAnimationCoroutine = StartCoroutine (closeObjectDistributor ());
			}
		}
	}

	IEnumerator closeObjectDistributor ()
	{
		eventOnCloseDistributor.Invoke ();
	
		while (mainAnimation.IsPlaying (animationName)) {
			yield return null;
		}

		if (useSpawnLimit) {
			if (currentUnitsAmount == 0) {
				eventOnEmptyUnits.Invoke ();

				removeDeviceFromList ();

				mainCollider.enabled = false;
			} else {
				mainCollider.enabled = true;
			}
		} else {
			mainCollider.enabled = true;
		}
	}

	public void removeDeviceFromList ()
	{
		currentPlayer = electronicDeviceManager.getCurrentPlayer ();

		if (currentPlayer != null) {
			usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
			usingDevicesManager.removeDeviceFromListExternalCall (electronicDeviceManager.gameObject);
		}
	}
}
