using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setFixedRotation : MonoBehaviour
{
	public Vector3 rotationValue;

	void Update ()
	{
		transform.eulerAngles = rotationValue;
	}
}
