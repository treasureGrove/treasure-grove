using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setObjectParentSystem : MonoBehaviour
{
	public Transform parentTransform;

	public bool resetChildLocalPosition;

	public List<Transform> childList = new List<Transform> ();

	public void setObjectsParent ()
	{
		for (int i = 0; i < childList.Count; i++) {
			childList [i].SetParent (parentTransform);

			if (resetChildLocalPosition) {
				childList [i].localPosition = Vector2.zero;
			}
		}
	}
}
