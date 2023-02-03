using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runtimeShatterRecursive : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool shatterEnabled = true;

	public Material crossSectionMaterial;
	public int numberOfShatters;

	public bool addSliceComponentToPieces;

	[Space]
	[Header ("Objects To Shatter List Settings")]
	[Space]

	public List<GameObject> objectsToShatter;

	public bool useCustomShatterPosition;
	public Transform customShatterPosition;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool applyForceOnShatter;
	public ForceMode forceMode;
	public float forceToApplyToCutPart;

	[Space]
	[Header ("Others Settings")]
	[Space]

	public bool activateShatterAtStart;

	void Start ()
	{
		if (activateShatterAtStart) {
			activateShatterOnObject ();
		}
	}

	public void activateShatterOnObject ()
	{ 
		if (!shatterEnabled) {
			return;
		}

		for (int i = 0; i < objectsToShatter.Count; i++) {
			if (objectsToShatter [i] != null) {
				if (objectsToShatter [i].transform.parent != null) {
					objectsToShatter [i].transform.SetParent (null);
				}
			}
		}

		for (int i = 0; i < numberOfShatters; i++) {
			randomShatter ();
		}
	}

	public void randomShatter ()
	{
		List<GameObject> adders = new List<GameObject> ();
		List<GameObject> removals = new List<GameObject> ();

		foreach (GameObject objectToShatter in objectsToShatter) {
			GameObject[] shatters = randomShatterSingle (objectToShatter);

			if (shatters != null) {
				foreach (GameObject add in shatters) {
					adders.Add (add);
				}

				removals.Add (objectToShatter);
			}
		}

		foreach (GameObject rem in removals) {
			objectsToShatter.Remove (rem);
		}

		foreach (GameObject add in adders) {
			objectsToShatter.Add (add);
		}
	}

	public GameObject[] randomShatterSingle (GameObject objectToShatter)
	{
		Vector3 shatterPosition = objectToShatter.transform.position;

		if (useCustomShatterPosition) {
			shatterPosition = customShatterPosition.position;
		}

		GameObject[] shatters = sliceSystemUtils.shatterObject (objectToShatter, shatterPosition, crossSectionMaterial);

		if (shatters != null && shatters.Length > 0) {
			objectToShatter.SetActive (false);

			Vector3 cutPosition = objectToShatter.transform.position;

			// add rigidbodies and colliders
			foreach (GameObject shatteredObject in shatters) {
				shatteredObject.AddComponent<MeshCollider> ().convex = true;

				if (applyForceOnShatter) {
					Rigidbody currentObjectRigidbody = shatteredObject.AddComponent<Rigidbody> ();

					currentObjectRigidbody.AddExplosionForce (forceToApplyToCutPart, cutPosition, 10, 1, forceMode);
				} else {
					shatteredObject.AddComponent<Rigidbody> ();
				}

				if (addSliceComponentToPieces) {
					surfaceToSlice newSurfaceToSlice = shatteredObject.AddComponent<surfaceToSlice> ();

					newSurfaceToSlice.crossSectionMaterial = crossSectionMaterial;
				}
			}
		}

		return shatters;
	}
}