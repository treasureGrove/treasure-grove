using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class durabilityInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string objectName;

	public float maxDurabilityAmount;

	public float durabilityAmount;

	public bool invulnerabilityActive;

	[Space]
	[Header ("Regeneration Settings")]
	[Space]

	public float regenerationSpeed = 4;

	public bool activateRegenerationAfterDelay;
	public float delayToActivateRegeneration;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectIsBroken;

	public bool regenerationActive;

	public bool regenerationAfterDelayActive;

	public bool characterLocated;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnEmptyDurability;
	public UnityEvent eventOnEmptyDurability;

	[Space]

	public bool useEventOnRefilledDurability;
	public UnityEvent eventOnRefilledDurability;

	[Space]

	public bool useEventOnInvulnerabilityActive;
	public UnityEvent eventOnInvulnerabilityActive;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnEmptyDurability;

	public List<string> remoteEventListOnEmptyDurability = new List<string> ();

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject mainCharacterGameObject;

	Coroutine regenerationCoroutine;

	Coroutine regenerationAfterDelayCoroutine;


	public virtual void initializeDurabilityValue (float newAmount)
	{
		durabilityAmount = newAmount;

		objectIsBroken = durabilityAmount <= 0;
	}

	public virtual float getDurabilityAmount ()
	{
		return durabilityAmount;
	}

	public virtual void repairObjectFully ()
	{
		addOrRemoveDurabilityAmountToObjectByName (maxDurabilityAmount, true);
	}

	public virtual void breakFullDurability ()
	{
		addOrRemoveDurabilityAmountToObjectByName (0, true);
	}

	public virtual void addOrRemoveDurabilityAmountToObjectByName (float newAmount, bool setAmountAsCurrentValue)
	{
		if (invulnerabilityActive) {
			return;
		}

		if (setAmountAsCurrentValue) {
			durabilityAmount = newAmount;
		} else {
			durabilityAmount += newAmount;
		}

		if (durabilityAmount <= 0) {
			durabilityAmount = 0;

			updateDurabilityAmountState ();
		} else {
			if (durabilityAmount >= maxDurabilityAmount) {
				durabilityAmount = maxDurabilityAmount;

				setFullDurabilityState ();
			}

			objectIsBroken = false;
		}

		if (newAmount < 0) {
			if (activateRegenerationAfterDelay) {
				if (regenerationAfterDelayActive) {
					stopUpdateRegenerationAfterDelayCoroutine ();
				}

				regenerationAfterDelayCoroutine = StartCoroutine (updateRegenerationAfterDelayCoroutine ());
			}
		}
	}

	public virtual void updateDurabilityAmountState ()
	{
		checkMainCharacterGameObject ();

		if (!characterLocated) {
			return;
		}

		if (durabilityAmount <= 0) {
			if (!objectIsBroken) {
				if (useEventOnEmptyDurability) {
					eventOnEmptyDurability.Invoke ();
				}

				if (useRemoteEventOnEmptyDurability) {
					remoteEventSystem currentRemoteEventSystem = mainCharacterGameObject.GetComponent<remoteEventSystem> ();
	
					if (currentRemoteEventSystem != null) {
						for (int i = 0; i < remoteEventListOnEmptyDurability.Count; i++) {

							currentRemoteEventSystem.callRemoteEvent (remoteEventListOnEmptyDurability [i]);
						}
					}
				}

				objectIsBroken = true;
			}
		}

		inventoryManager currentInventoryManager = mainCharacterGameObject.GetComponent<inventoryManager> ();

		if (currentInventoryManager != null) {
			currentInventoryManager.addOrRemoveDurabilityAmountToObjectByName (objectName, durabilityAmount, true);
		}
	}

	public virtual void setFullDurabilityState ()
	{
		checkMainCharacterGameObject ();

		if (!characterLocated) {
			return;
		}

		durabilityAmount = maxDurabilityAmount;

		objectIsBroken = false;

		inventoryManager currentInventoryManager = mainCharacterGameObject.GetComponent<inventoryManager> ();

		if (currentInventoryManager != null) {
			currentInventoryManager.addOrRemoveDurabilityAmountToObjectByName (objectName, maxDurabilityAmount, true);
		}

		if (useEventOnRefilledDurability) {
			eventOnRefilledDurability.Invoke ();
		}
	}

	public virtual void setInvulnerabilityActiveState (bool state)
	{
		invulnerabilityActive = state;

		if (state) {
			if (useEventOnInvulnerabilityActive) {
				eventOnInvulnerabilityActive.Invoke ();
			}
		}
	}

	public void setMainCharacterGameObject (GameObject newObject)
	{
		mainCharacterGameObject = newObject;
	}

	public void checkMainCharacterGameObject ()
	{
		if (characterLocated) {
			return;
		}

		if (mainCharacterGameObject == null) {
			getMainCharacterGameObject ();
		}

		characterLocated = mainCharacterGameObject != null;
	}

	public virtual void getMainCharacterGameObject ()
	{
		
	}
			
	//Coroutine functions
	public void activateDurabilityRegeneration ()
	{
		stopUpdateRegenerationCoroutine ();

		regenerationCoroutine = StartCoroutine (updateRegenerationCoroutine ());
	}

	public void stopUpdateRegenerationCoroutine ()
	{
		if (regenerationCoroutine != null) {
			StopCoroutine (regenerationCoroutine);
		}

		if (regenerationActive) {
			updateDurabilityAmountState ();
		}

		if (regenerationAfterDelayActive) {
			stopUpdateRegenerationAfterDelayCoroutine ();
		}

		regenerationActive = false;
	}

	IEnumerator updateRegenerationCoroutine ()
	{
		regenerationActive = true;

		bool targetReached = false;

		float timer = 0;

		objectIsBroken = false;
		
		while (targetReached) {
			timer = Time.deltaTime * regenerationSpeed;

			durabilityAmount += timer;

			if (durabilityAmount >= maxDurabilityAmount) {
				durabilityAmount = maxDurabilityAmount;

				targetReached = true;
			}

			yield return null;
		}

		regenerationActive = false;

		setFullDurabilityState ();
	}

	public void stopUpdateRegenerationAfterDelayCoroutine ()
	{
		if (regenerationAfterDelayCoroutine != null) {
			StopCoroutine (regenerationAfterDelayCoroutine);
		}

		regenerationAfterDelayActive = false;
	}

	IEnumerator updateRegenerationAfterDelayCoroutine ()
	{
		regenerationAfterDelayActive = true;

		yield return new WaitForSeconds (delayToActivateRegeneration);

		activateDurabilityRegeneration ();

		regenerationAfterDelayActive = false;
	}

	public bool isDurabilityEmpty ()
	{
		return durabilityAmount <= 0;
	}
}
