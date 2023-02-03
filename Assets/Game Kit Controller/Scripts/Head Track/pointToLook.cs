using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointToLook : MonoBehaviour
{
	public Transform pointToLookTransform;

	void Start ()
	{
		if (pointToLookTransform == null) {
			pointToLookTransform = transform;
		}
	}

	public Transform getPointToLookTransform ()
	{
		return pointToLookTransform;
	}
}
