using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class destroyGameObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool destroyObjectEnabled = true;

	public bool destroyObjectOnEnable;

	public float timer = 0.6f;
	public bool destroyObjectAtStart = true;

	public bool disableInsteadOfDestroyActive;

	public bool sendObjectToPoolSystemToDisable;

	public bool destroyJustAllChildObjects;

	[Space]
	[Header ("Objects To Destroy Settings")]
	[Space]

	public GameObject objectToDestroy;

	[Space]
	[Space]

	public bool useGameObjectList;
	public List<GameObject> gameObjectList = new List<GameObject> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool destroyCoroutineActive;


	Coroutine destroyObjectCoroutine;


	void Start ()
	{
		checkToDestroyObjectInTime (true);
	}

	public void destroyObjectInTime ()
	{
		stopDestroyObjectCoroutine ();

//		print (gameObject.name + " " + timer);

		destroyObjectCoroutine = StartCoroutine (destroyObjectInTimeCoroutine ());
	}

	public void stopDestroyObjectCoroutine ()
	{
		destroyCoroutineActive = false;

		if (destroyObjectCoroutine != null) {
			StopCoroutine (destroyObjectCoroutine);
		}
	}

	IEnumerator destroyObjectInTimeCoroutine ()
	{
		destroyCoroutineActive = true;

//		print (gameObject.name + " " + timer);

		yield return new WaitForSeconds (timer);

		destroyCoroutineActive = false;

		destroy ();
	}

	public void setTimer (float timeToDestroy)
	{
		timer = timeToDestroy;
	}

	void OnDisable ()
	{
		if (timer > 0) {
			if (!disableInsteadOfDestroyActive) {

//				print ("disabling objects " + Application.isPlaying + " time " + Time.timeScale + " " + Time.deltaTime);

				if (Time.timeScale > 0 || Time.deltaTime > 0) {
//					print ("DESTROYING OBJECT");
					destroy ();
				} else {
//					print ("TRYING TO DESTROY OBJECT OUT OF PLAY TIME");
				}
			}
		}

//		print (gameObject.name + " " + timer + " " + destroyCoroutineActive);
	}

	void OnEnable ()
	{
		if (destroyObjectOnEnable) {
			checkToDestroyObjectInTime (false);
		}
	}

	public void destroy ()
	{
		if (!destroyObjectEnabled) {
			return;
		}

		if (objectToDestroy == null) {
			objectToDestroy = gameObject;
		}

		if (disableInsteadOfDestroyActive) {
			if (sendObjectToPoolSystemToDisable) {
				GKC_PoolingSystem.Despawn (objectToDestroy);
			} else {
				objectToDestroy.SetActive (false);
			}

			if (useGameObjectList) {
				for (int i = 0; i < gameObjectList.Count; i++) {
					if (gameObjectList [i] != null) {
						if (sendObjectToPoolSystemToDisable) {
							GKC_PoolingSystem.Despawn (gameObjectList [i]);
						} else {
							if (gameObjectList [i].activeSelf) {
								gameObjectList [i].SetActive (false);
							}
						}
					}
				}
			}
		} else {
			if (GKC_Utils.isApplicationPlaying () && Time.deltaTime > 0) {

				if (destroyJustAllChildObjects) {
					Component[] components = objectToDestroy.GetComponentsInChildren (typeof(Transform));

					foreach (Transform child in components) {
						if (child != objectToDestroy.transform) {
							Destroy (child.gameObject);
						}
					}
				} else {
					Destroy (objectToDestroy);

					if (useGameObjectList) {
						for (int i = 0; i < gameObjectList.Count; i++) {
							if (gameObjectList [i] != null) {
								Destroy (gameObjectList [i]);
							}
						}
					}
				}
			}
		}
	}

	public void setDestroyObjectEnabledState (bool state)
	{
		destroyObjectEnabled = state;
	}

	public void changeDestroyForSetActiveFunction (bool state)
	{
		disableInsteadOfDestroyActive = state;
	}

	public void setSendObjectToPoolSystemToDisableState (bool state)
	{
		sendObjectToPoolSystemToDisable = state;

		if (sendObjectToPoolSystemToDisable) {
			disableInsteadOfDestroyActive = true;
		}
	}

	public void checkToDestroyObjectInTime (bool callingFromStart)
	{
		if (!destroyCoroutineActive) {
			if ((destroyObjectAtStart && callingFromStart) || !callingFromStart) {
				destroyObjectInTime ();
			}
		}
	}

	public void cancelDestroy ()
	{
		stopDestroyObjectCoroutine ();
	}
}