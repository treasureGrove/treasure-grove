using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GKCConditionInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool conditionCheckEnabled;

	public bool searchPlayerOnSceneIfNotAssigned;

	public LayerMask layermaskToCheck;

	public bool checkConditionCompleteOnTriggerEnterEnabled = true;

	[Space]
	[Header ("Coroutine Settings")]
	[Space]

	public bool checkConditionOnCoroutine;

	public bool stopCoroutineOnConditionComplete;
	public bool stopCoroutineOnConditionNotComplete;


	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnConditionComplete;

	public UnityEvent eventOnConditionNotComplete;

	[Space]
	[Space]

	public bool useEventOnTriggerEnter;
	public UnityEvent eventOnTriggerEnter;

	public bool useEventOnTriggerExit;
	public UnityEvent eventOnTriggerExit;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool checkConditionPaused;

	public GameObject currentPlayer;

	public bool coroutineUpdateInProcess;

	bool playerAssignedProperly;


	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			setCurrentPlayer (GKC_Utils.findMainPlayerOnScene ());
		}
	}

	public bool checkIfPlayerAssigned ()
	{
		if (playerAssignedProperly) {
			return true;
		}

		if (currentPlayer == null) {

			findPlayerOnScene ();

			if (currentPlayer == null) {
				print ("WARNING: no player controller has been assigned to the mission." +
				" Make sure to use a trigger to activate the mission or assign the player manually");
			} else {
				return true;
			}
		}

		return false;
	}

	public virtual void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;

		if (currentPlayer != null) {
			playerAssignedProperly = true;
		} else {
			playerAssignedProperly = false;
		}
	}

	public virtual void checkIfConditionComplete ()
	{

	}

	public void setConditionResult (bool state)
	{
		if (showDebugPrint) {
			print ("Condition result: " + state);
		}

		if (state) {
			eventOnConditionComplete.Invoke ();
		} else {
			eventOnConditionNotComplete.Invoke ();
		}

		if (state) {
			if (stopCoroutineOnConditionComplete) {
				if (checkConditionOnCoroutine) {
					stopUpdateCoroutine ();
				}
			}
		} else {
			if (stopCoroutineOnConditionNotComplete) {
				if (checkConditionOnCoroutine) {
					stopUpdateCoroutine ();
				}
			}
		}
	}

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
		if (!conditionCheckEnabled) {
			return;
		}

		if (checkConditionPaused) {
			return;
		}

		if ((1 << col.gameObject.layer & layermaskToCheck.value) == 1 << col.gameObject.layer) {

			if (isEnter) {
				setCurrentPlayer (col.gameObject);

				playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					GKCConditionSystem mainGKCConditionSystem = mainPlayerComponentsManager.getGKCConditionSystem ();

					if (mainGKCConditionSystem != null) {
						mainGKCConditionSystem.setCurrentGKCConditionInfo (this);

						if (checkConditionCompleteOnTriggerEnterEnabled) {
							if (checkConditionOnCoroutine) {
								stopUpdateCoroutine ();

								conditionCoroutine = StartCoroutine (updateCoroutine ());
							} else {
								mainGKCConditionSystem.checkIfConditionComplete ();
							}
						}

						if (useEventOnTriggerEnter) {
							eventOnTriggerEnter.Invoke ();
						}
					}
				}
			} else {
				if (col.gameObject == currentPlayer) {
					setCurrentPlayer (null);

					checkFunctionOnTriggerExit ();

					if (useEventOnTriggerExit) {
						eventOnTriggerExit.Invoke ();
					}
				}
			}
		}
	}

	public void setCheckConditionPausedState (bool state)
	{
		checkConditionPaused = state;
	}

	public void stopUpdateCoroutine ()
	{
		if (conditionCoroutine != null) {
			StopCoroutine (conditionCoroutine);
		}

		coroutineUpdateInProcess = false;
	}

	public virtual void checkFunctionOnTriggerExit ()
	{
		if (checkConditionOnCoroutine) {
			stopUpdateCoroutine ();

		}
	}

	public Coroutine conditionCoroutine;

	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			checkConditionOnUpdateState ();

			coroutineUpdateInProcess = true;

			yield return waitTime;
		}
	}

	public virtual void checkConditionOnUpdateState ()
	{

	}
}
