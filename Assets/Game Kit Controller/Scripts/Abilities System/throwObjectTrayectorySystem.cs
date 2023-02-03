using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwObjectTrayectorySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool throwObjectEnabled = true;

	public LayerMask layerToCheckThrowObject;
	public float maxRaycastDistance = 30;

	public bool useInfiniteRaycastDistance;

	public bool useMaxDistanceWhenNoSurfaceFound;
	public float maxDistanceWhenNoSurfaceFound;

	public bool addLaunchObjectComponent;
	public bool addGrabbedObjectStateComponent;

	public bool rotateObjectTowardThrowDirection;

	public bool checkIfLockedCameraActive;

	public bool ignoreCollisionsWithPlayerEnabled = true;

	[Space]

	public GameObject objectToThrow;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showGizmo;

	public GameObject currentObjectToThrow;
	public Vector3 currentThrowSpeed;

	[Space]
	[Header ("Components")]
	[Space]

	public playerCamera mainPlayerCamera;
	public GameObject playerControllerGameObject;
	public Transform throwPosition;
	public Transform targetPositionTransform;

	public Transform playerCameraGameObject;
	public Transform mainCameraTransform;

	public Collider playerCollider;

	Rigidbody newObjectToThrowRigidbody;

	RaycastHit hit;

	Vector3 currentTargetPosition;

	Vector3 raycastDirection;
	Vector3 raycastOrigin;

	Ray newRay;

	public void setCurrentObjectToThrow (GameObject newObject)
	{
		if (throwPosition == null) {
			throwPosition = transform;
		}

		currentObjectToThrow = newObject;

		currentObjectToThrow.transform.SetParent (throwPosition);

		currentObjectToThrow.transform.position = throwPosition.position;

		currentObjectToThrow.SetActive (true);

		newObjectToThrowRigidbody = currentObjectToThrow.GetComponent<Rigidbody> ();

		if (newObjectToThrowRigidbody != null) {
			newObjectToThrowRigidbody.isKinematic = true;
		}

		if (ignoreCollisionsWithPlayerEnabled) {
			if (playerCollider != null) {
				Component[] components = currentObjectToThrow.GetComponentsInChildren (typeof(Collider));

				foreach (Collider child in components) {
					Physics.IgnoreCollision (playerCollider, child, true);
				}
			}
		}
	}

	public void instantiateObject ()
	{
		if (currentObjectToThrow == null) {
			if (throwPosition == null) {
				throwPosition = transform;
			}

			GameObject newObjectToThrow = (GameObject)Instantiate (objectToThrow, throwPosition.position, throwPosition.rotation);

			setCurrentObjectToThrow (newObjectToThrow);
		}
	}

	public void calculateThrowDirection ()
	{
		raycastDirection = mainCameraTransform.TransformDirection (Vector3.forward);

		raycastOrigin = mainCameraTransform.position;

		if (checkIfLockedCameraActive) {
			if (mainPlayerCamera != null) {
				if (!mainPlayerCamera.isCameraTypeFree ()) {
					newRay = mainPlayerCamera.getCameraRaycastDirection ();

					raycastDirection = newRay.direction;

					raycastOrigin = newRay.origin;
				}
			}
		}

		currentTargetPosition = targetPositionTransform.position;

		float currentRaycastDistance = maxRaycastDistance;

		if (useInfiniteRaycastDistance) {
			currentRaycastDistance = Mathf.Infinity;
		}

		if (Physics.Raycast (raycastOrigin, raycastDirection, out hit, currentRaycastDistance, layerToCheckThrowObject)) {
			currentTargetPosition = hit.point;

			if (hit.collider == playerCollider) {
				Vector3 newRaycastPosition = hit.point + raycastDirection * 0.2f;

				if (Physics.Raycast (newRaycastPosition, raycastDirection, out hit, currentRaycastDistance, layerToCheckThrowObject)) {
					currentTargetPosition = hit.point;
				}
			}
		} else {
			if (useMaxDistanceWhenNoSurfaceFound) {
				currentTargetPosition = raycastOrigin + raycastDirection * maxDistanceWhenNoSurfaceFound;

			} else {
				if (Physics.Raycast (targetPositionTransform.position, raycastDirection, out hit, Mathf.Infinity, layerToCheckThrowObject)) {
					currentTargetPosition = hit.point;

					if (hit.collider == playerCollider) {
						Vector3 newRaycastPosition = hit.point + raycastDirection * 0.2f;

						if (Physics.Raycast (newRaycastPosition, raycastDirection, out hit, Mathf.Infinity, layerToCheckThrowObject)) {
							currentTargetPosition = hit.point;
						}
					}
				}
			}
		}

		if (showGizmo) {
			Debug.DrawLine (raycastOrigin, currentTargetPosition, Color.black, 5);
		}
	}

	public void throwObject ()
	{
		if (throwPosition == null) {
			throwPosition = transform;
		}

		if (currentObjectToThrow == null) {
			instantiateObject ();
		}

		currentThrowSpeed = getParableSpeed (currentObjectToThrow.transform.position, currentTargetPosition);

		if (showGizmo) {
			Debug.DrawLine (currentObjectToThrow.transform.position, currentTargetPosition, Color.black, 5);
		}

		if (rotateObjectTowardThrowDirection) {
			currentObjectToThrow.transform.LookAt (currentTargetPosition);
		}

		currentObjectToThrow.transform.SetParent (null);

		if (currentThrowSpeed == -Vector3.one) {
			currentThrowSpeed = currentObjectToThrow.transform.forward * 100;
		}

		if (newObjectToThrowRigidbody != null) {
			newObjectToThrowRigidbody.isKinematic = false;

			newObjectToThrowRigidbody.AddForce (currentThrowSpeed, ForceMode.VelocityChange);
		}

		if (addLaunchObjectComponent) {
			launchedObjects currentLaunchedObjects = currentObjectToThrow.GetComponent<launchedObjects> ();

			if (currentLaunchedObjects == null) {
				currentLaunchedObjects = currentObjectToThrow.AddComponent<launchedObjects> ();
			}

			currentLaunchedObjects.setCurrentPlayer (playerControllerGameObject);
		}

		if (addGrabbedObjectStateComponent) {
			applyDamage.checkGravityRoomForGrabbedObject (currentObjectToThrow, playerControllerGameObject);
		}

		currentObjectToThrow = null;
	}

	//calculate the speed applied to the launched projectile to make a parable according to a hit point
	Vector3 getParableSpeed (Vector3 origin, Vector3 target)
	{
		//get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;

		//remove the Y axis value
		toTargetXZ -= playerCameraGameObject.transform.InverseTransformDirection (toTargetXZ).y * playerCameraGameObject.transform.up;
		float y = playerCameraGameObject.transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;

		//get the velocity accoring to distance ang gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;

		//create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        

		//get direction of xz but with magnitude 1
		result *= v0xz;                            

		// set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= playerCameraGameObject.transform.InverseTransformDirection (result).y * playerCameraGameObject.transform.up;
		result += playerCameraGameObject.transform.up * v0y;

		return result;
	}

	public void inputThrowObject ()
	{
		if (throwObjectEnabled) {
			throwObject ();
		}
	}

	public void inputInstantiateObject ()
	{
		if (throwObjectEnabled) {
			instantiateObject ();
		}
	}

	public void inputCalculateThrowDirection ()
	{
		if (throwObjectEnabled) {
			calculateThrowDirection ();
		}
	}
}
