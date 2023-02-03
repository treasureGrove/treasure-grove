using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleSetQuaternionRotation : MonoBehaviour
{
	public void resetRotation ()
	{
		transform.rotation = Quaternion.identity;
	}
}
