using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sliceObject : MonoBehaviour
{
	public bool sliceActive;

	public Material defaultSliceMaterial;

	public Transform cutDirectionTransform;

	public float forceToApplyToCutPart;
	public ForceMode forceMode;
	public float forceRadius;
	public float forceUp;

	public bool cutMultipleTimesActive = true;

	public float disableTimeAfterCollision;

	public LayerMask targetToDamageLayer;

	public Collider mainCollider;

	public Collider triggerCollider;

	public List<GameObject> objectsDetected = new List<GameObject> ();

	public List<Collider> collidersToIgnoreList = new List<Collider> ();

	surfaceToSlice currentSurfaceToSlice;

	Collider currentColliderFound;

	public void processObject (GameObject obj)
	{
		if (cutMultipleTimesActive) {
			if (objectsDetected.Contains (obj)) {
				return;
			}
		}

		currentSurfaceToSlice = obj.GetComponent<surfaceToSlice> ();

		if (currentSurfaceToSlice) {
//			Physics.IgnoreCollision (mainCollider, currentColliderFound, true);

			print (obj.name);

			Material crossSectionMaterial = defaultSliceMaterial;

			if (currentSurfaceToSlice.crossSectionMaterial != null) {
				crossSectionMaterial = currentSurfaceToSlice.crossSectionMaterial;
			}
				
			sliceCurrentObject (obj, crossSectionMaterial);
		}
	}

	void OnCollisionEnter (Collision col)
	{
		print ("COLLISION " + col.collider.name);
	}

	void OnTriggerEnter (Collider col)
	{
		if (sliceActive) {
			currentColliderFound = col;

			checkObjectDetected (currentColliderFound);

			disableSliceActiveWithDelay ();
		}
	}

	public void setSliceActiveState (bool state)
	{
		sliceActive = state;

		stopDisableSliceActiveWithDelay ();

		if (sliceActive) {
			mainCollider.enabled = false;
		} else {
			disableIgnoreCollisionList ();
		}
	}

	public void checkObjectDetected (Collider col)
	{
		if ((1 << col.gameObject.layer & targetToDamageLayer.value) == 1 << col.gameObject.layer) {

			processObject (col.GetComponent<Collider> ().gameObject);
		}
	}

	public void sliceCurrentObject (GameObject obj, Material crossSectionMaterial)
	{
		bool objectSliced = false;

		GameObject object1 = null;
		GameObject object2 = null;

		// slice the provided object using the transforms of this object
		sliceSystemUtils.sliceObject (transform.position, obj, cutDirectionTransform.up, crossSectionMaterial, ref objectSliced, ref object1, ref object2);

		Vector3 objectPosition = obj.transform.position;
		Quaternion objectRotation = obj.transform.rotation;

		Transform objectParent = obj.transform.parent;

		if (objectSliced) {
			currentSurfaceToSlice.checkEventOnCut ();

			currentSurfaceToSlice.checkTimeBulletOnCut ();

			Rigidbody mainObject = obj.GetComponent<Rigidbody> ();
			bool mainObjectHasRigidbody = mainObject != null;

			object1.transform.position = objectPosition;
			object1.transform.rotation = objectRotation;

			object2.transform.position = objectPosition;
			object2.transform.rotation = objectRotation;

			if (objectParent != null) {
				object1.transform.SetParent (objectParent);
				object2.transform.SetParent (objectParent);
			}
				
			surfaceToSlice newSurfaceToSlice1 = object1.AddComponent<surfaceToSlice> ();
			surfaceToSlice newSurfaceToSlice2 = object2.AddComponent<surfaceToSlice> ();

			currentSurfaceToSlice.copySurfaceInfo (newSurfaceToSlice1);
			currentSurfaceToSlice.copySurfaceInfo (newSurfaceToSlice2);


			float distance1 = GKC_Utils.distance (obj.transform.position, object1.transform.position);
			float distance2 = GKC_Utils.distance (obj.transform.position, object2.transform.position);

			float currentForceToApply = forceToApplyToCutPart;

			if (mainObjectHasRigidbody) {

				if (currentSurfaceToSlice.useCustomForceAmount) {
					currentForceToApply = currentSurfaceToSlice.customForceAmount;
				}

				Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

				Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

				object2Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);

				object1Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);
			} else {
				if (currentSurfaceToSlice.useCustomForceAmount) {
					currentForceToApply = currentSurfaceToSlice.customForceAmount;
				}

				if (distance1 < distance2) {
					Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

					object2Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);
				} else {
					Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

					object1Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);
				}
			}

			if (currentSurfaceToSlice.useBoxCollider) {
				object1.AddComponent<BoxCollider> ();
				object2.AddComponent<BoxCollider> ();
			} else {
				MeshCollider object1Collider = object1.AddComponent<MeshCollider> ();
				MeshCollider object2Collider = object2.AddComponent<MeshCollider> ();

				object1Collider.convex = true;
				object2Collider.convex = true;
			}

			Collider collider1 = object1.GetComponent<Collider> ();
			Collider collider2 = object2.GetComponent<Collider> ();

			print (mainCollider.name);
			print (collider1.name);
			print (collider2.name);

			collidersToIgnoreList.Add (collider1);
			collidersToIgnoreList.Add (collider2);

//			Physics.IgnoreCollision (mainCollider, collider1, true);
//			Physics.IgnoreCollision (mainCollider, collider2, true);

			if (currentSurfaceToSlice.setNewLayerOnCut) {
				object1.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
				object1.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
			}

			if (currentSurfaceToSlice.setNewTagOnCut) {
				object1.tag = currentSurfaceToSlice.tag;
				object1.tag = currentSurfaceToSlice.tag;
			}

			if (cutMultipleTimesActive) {
				if (!objectsDetected.Contains (object1)) {
					objectsDetected.Add (object1);
				}

				if (!objectsDetected.Contains (object2)) {
					objectsDetected.Add (object2);
				}
			}

			obj.SetActive (false);
		}
	}

	Coroutine disableSliceCoroutine;

	public void stopDisableSliceActiveWithDelay ()
	{
		if (disableSliceCoroutine != null) {
			StopCoroutine (disableSliceCoroutine);
		}
	}

	public void disableSliceActiveWithDelay ()
	{
		stopDisableSliceActiveWithDelay ();

		disableSliceCoroutine = StartCoroutine (disableSliceActiveWithDelayCoroutine ());
	}

	IEnumerator disableSliceActiveWithDelayCoroutine ()
	{
		yield return new WaitForSeconds (0.5f);

//		mainCollider.enabled = true;

		yield return new WaitForSeconds (disableTimeAfterCollision);

		mainCollider.enabled = true;

		sliceActive = false;

		disableIgnoreCollisionList ();
	}

	public void disableIgnoreCollisionList ()
	{
//		for (int i = 0; i < collidersToIgnoreList.Count; i++) {
//			Physics.IgnoreCollision (mainCollider, collidersToIgnoreList [i], false);
//		}

		collidersToIgnoreList.Clear ();

		triggerCollider.enabled = false;

		objectsDetected.Clear ();
	}
}
