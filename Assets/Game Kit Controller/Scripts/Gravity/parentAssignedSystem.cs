using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parentAssignedSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public GameObject parentGameObject;

	[Space]
	[Header ("Main Settings")]
	[Space]

	public bool parentAssigned;


	public void assignParent (GameObject newParent)
	{
		parentGameObject = newParent;

		parentAssigned = parentGameObject != null;
	}

	public GameObject getAssignedParent ()
	{
		if (!parentAssigned) {
			if (parentGameObject == null) {
				parentGameObject = gameObject;
			}

			parentAssigned = parentGameObject != null;
		}

		return parentGameObject;
	}

	public Transform getAssignedParentTransform ()
	{
		if (!parentAssigned) {
			if (parentGameObject == null) {
				parentGameObject = gameObject;
			}

			parentAssigned = parentGameObject != null;
		}

		if (parentAssigned) {
			return parentGameObject.transform;
		}

		return null;
	}
}
