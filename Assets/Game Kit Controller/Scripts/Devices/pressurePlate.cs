using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class pressurePlate : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float minDistance;

	public List<string> tagsToIgnore = new List<string>{ "Player" };

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent unlockFunctionCall = new UnityEvent ();
	public UnityEvent lockFunctionCall = new UnityEvent ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingPlate;
	public bool activeFunctionCalled;
	public bool disableFunctionCalled;

	public List<GameObject> objects = new List<GameObject> ();

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject plate;
	public Transform finalPosition;

	List<Collider> colliders = new List<Collider> ();
	Coroutine setPlateState;

	void Start ()
	{
		Component[] components = GetComponentsInChildren (typeof(Collider));
		foreach (Collider c in components) {
			colliders.Add (c);
		}

		Collider plateCollider = plate.GetComponent<Collider> ();

		for (int i = 0; i < colliders.Count; i++) {
			if (colliders [i] != plate) {
				Physics.IgnoreCollision (plateCollider, colliders [i]);
			}
		}
	}

	void Update ()
	{
		if (usingPlate) {
			if ((Mathf.Abs (Mathf.Abs (plate.transform.position.y) - Mathf.Abs (finalPosition.position.y)) < minDistance) || plate.transform.position.y < finalPosition.position.y) {
				if (!activeFunctionCalled) {
					activeFunctionCalled = true;
					disableFunctionCalled = false;

					if (unlockFunctionCall.GetPersistentEventCount () > 0) {
						unlockFunctionCall.Invoke ();
					}
				}
			} else {
				if (activeFunctionCalled) {
					activeFunctionCalled = false;
					disableFunctionCalled = true;

					if (lockFunctionCall.GetPersistentEventCount () > 0) {
						lockFunctionCall.Invoke ();
					}
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.GetComponent<Rigidbody> () && !tagsToIgnore.Contains (col.gameObject.tag)) {
			checkCoroutine (true);

			if (!objects.Contains (col.gameObject) && col.gameObject != plate) {
				objects.Add (col.gameObject);
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.gameObject.GetComponent<Rigidbody> () && !tagsToIgnore.Contains (col.gameObject.tag)) {
			if (objects.Contains (col.gameObject)) {
				objects.Remove (col.gameObject);
			}

			for (int i = 0; i < objects.Count; i++) {
				if (!objects [i]) {
					objects.RemoveAt (i);
				}
			}

			if (objects.Count == 0) {
				checkCoroutine (false);
			}
		}
	}

	void checkCoroutine (bool state)
	{
		if (setPlateState != null) {
			StopCoroutine (setPlateState);
		}

		setPlateState = StartCoroutine (enableOrDisablePlate (state));
	}

	IEnumerator enableOrDisablePlate (bool state)
	{
		if (state) {
			usingPlate = true;
			yield return null;
		} else {
			yield return new WaitForSeconds (1);
			usingPlate = false;
		}
	}

	void OnCollisionEnter (Collision col)
	{

	}
}