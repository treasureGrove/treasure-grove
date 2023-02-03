using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventParameters
{

	[System.Serializable]
	public class eventToCallWithAmount : UnityEvent<float>
	{

	}

	[System.Serializable]
	public class eventToCallWithBool : UnityEvent<bool>
	{

	}

	[System.Serializable]
	public class eventToCallWithVector3 : UnityEvent<Vector3>
	{

	}

	[System.Serializable]
	public class eventToCallWithGameObject : UnityEvent<GameObject>
	{

	}

	[System.Serializable]
	public class eventToCallWithTransform: UnityEvent<Transform>
	{

	}

	[System.Serializable]
	public class eventToCallWithString: UnityEvent<string>
	{

	}

	[System.Serializable]
	public class eventToCallWithInteger : UnityEvent<int>
	{

	}

	[System.Serializable]
	public class eventToCallWithTransformList : UnityEvent<List<Transform>>
	{

	}
}
