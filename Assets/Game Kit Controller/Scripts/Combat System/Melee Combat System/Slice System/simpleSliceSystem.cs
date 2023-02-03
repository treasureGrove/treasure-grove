using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleSliceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public GameObject objectToSlice;

	public GameObject alternatePrefab;

	public Material infillMaterial;

	public surfaceToSlice mainSurfaceToSlice;

	public Transform[] severables = new Transform[0];

	[Space]
	[Header ("Slice Ragdoll Settings")]
	[Space]

	public string prefabsPath = "";

	public Material sliceMaterial;

	public bool setTagOnSkeletonRigidbodies = true;

	public string tagOnSkeletonRigidbodies = "box";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	void Awake ()
	{
		initializeValuesOnHackableComponent ();
	}

	public void initializeValuesOnHackableComponent ()
	{
		sliceSystemUtils.initializeValuesOnHackableComponent (gameObject, this);
	}

	public void activateSlice (Collider objectCollider, Vector3 newNormalInWorldSpaceValue, Vector3 positionInWorldSpace, Vector3 slicePosition)
	{
		Vector3 point = objectCollider.ClosestPointOnBounds (positionInWorldSpace);

		if (mainSurfaceToSlice.useParticlesOnSlice) {
			Quaternion particlesRotation = Quaternion.LookRotation (newNormalInWorldSpaceValue);

			Instantiate (mainSurfaceToSlice.particlesOnSlicePrefab, slicePosition, particlesRotation);
		}

		sliceSystemUtils.sliceCharacter (objectToSlice, point, newNormalInWorldSpaceValue);

		if (showDebugPrint) {
			print ("Activate Slice on Character " + objectToSlice.name);
		}
	}

	public void searchBodyParts ()
	{
		if (objectToSlice == null) {
			objectToSlice = gameObject;
		}

		List<GameObject> bodyPartsList = new List<GameObject> ();

		Component[] childrens = objectToSlice.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Rigidbody child in childrens) {
			Collider currentCollider = child.GetComponent<Collider> ();

			if (currentCollider && !currentCollider.isTrigger) {

				bodyPartsList.Add (child.gameObject);
			}
		}

		severables = new Transform[bodyPartsList.Count];

		for (int i = 0; i < bodyPartsList.Count; i++) {
			severables [i] = bodyPartsList [i].transform;
		}

	}

	public void setTagOnBodyParts (string newTag)
	{
		for (int i = 0; i < severables.Length; i++) {
			if (severables [i] != null) {
				severables [i].tag = newTag;
			}
		}
	}

	public GameObject getMainAlternatePrefab ()
	{
		return alternatePrefab;
	}

	public Transform[] getSeverables ()
	{
		return severables;
	}

	public void setNewSeverablesList (GameObject newObject)
	{
		if (newObject != null) {
			genericRagdollBuilder currentGenericRagdollBuilder = newObject.GetComponent<genericRagdollBuilder> ();

			if (currentGenericRagdollBuilder != null) {
				int bonesCount = currentGenericRagdollBuilder.bones.Count;

				severables = new Transform[bonesCount];

				for (int i = 0; i < bonesCount; i++) {
					severables [i] = currentGenericRagdollBuilder.bones [i].anchor;
				}
			}
		}
	}

	public void createRagdollPrefab ()
	{
		if (alternatePrefab == null) {
			prefabsPath = pathInfoValues.getSliceObjectsPrefabsPath ();

			alternatePrefab = GKC_Utils.createSliceRagdollPrefab (objectToSlice, prefabsPath, sliceMaterial, setTagOnSkeletonRigidbodies, tagOnSkeletonRigidbodies);
		}
	}
}