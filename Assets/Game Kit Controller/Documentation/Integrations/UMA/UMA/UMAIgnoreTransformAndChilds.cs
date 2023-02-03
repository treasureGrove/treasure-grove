using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAIgnoreTransformAndChilds : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<Transform> childsToIgnore = new List<Transform> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public List<Transform> objectsToIgnoreChildren = new List<Transform> ();

	public int numberOfChilds;


	public void removeChildList ()
	{
		objectsToIgnoreChildren.Clear ();

		numberOfChilds = 0;
	}

	public List<Transform> getChildrenList ()
	{
		return objectsToIgnoreChildren;
	}

	public void setChildrenList ()
	{
		objectsToIgnoreChildren.Clear ();

		numberOfChilds = 0;

		for (int i = 0; i < childsToIgnore.Count; i++) {
			Transform child = childsToIgnore [i];

			Component[] childrens = child.GetComponentsInChildren (typeof(Transform), true);

			foreach (Transform currentTransform in childrens) {
				objectsToIgnoreChildren.Add (currentTransform);
			}
		}

		numberOfChilds = objectsToIgnoreChildren.Count;
	}
}