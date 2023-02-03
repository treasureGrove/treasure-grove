using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObjectPositionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool followObjectActive = true;
	public Transform objectToFollow;
	public bool followPosition = true;
	public bool followRotation = true;
	public Transform mainTransform;

	public bool checkIfObjectToFollowNull;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectToFollowLocated;

	public bool showDebugPrint;

	bool initialized;

	void Start ()
	{
		initializeComponents ();
	}

	void initializeComponents ()
	{
		if (!initialized) {
			if (mainTransform == null) {
				mainTransform = transform;
			}

			objectToFollowLocated = objectToFollow != null;

			initialized = true;
		}
	}

	void FixedUpdate ()
	{
		if (followObjectActive && objectToFollowLocated) {
			if (checkIfObjectToFollowNull) {
				if (objectToFollow == null) {
					followObjectActive = false;

					return;
				}
			}

			if (followPosition && followRotation) {
				mainTransform.SetPositionAndRotation (objectToFollow.position, objectToFollow.rotation);
			} else {
				if (followPosition) {
					mainTransform.position = objectToFollow.position;
				}

				if (followRotation) {
					mainTransform.rotation = objectToFollow.rotation;
				}
			}

			if (showDebugPrint) {
				print ("Following target active");
			}
		}
	}

	public void setFollowObjectActiveState (bool state)
	{
		followObjectActive = state;
	}

	public void setEnabledState (bool state)
	{
		enabled = state;
	}

	public void updatePositionOnce ()
	{
		if (objectToFollow == null) {
			return;
		}

		if (followPosition) {
			mainTransform.position = objectToFollow.position;
		}

		if (followRotation) {
			mainTransform.rotation = objectToFollow.rotation;
		}
	}

	public void setObjectToFollow (Transform newObject)
	{
		objectToFollow = newObject;
	}

	public void setObjectToFollowFromEditor (Transform newObject)
	{
		setObjectToFollow (newObject);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Set Object To Follow From Editor", gameObject);
	}
}
