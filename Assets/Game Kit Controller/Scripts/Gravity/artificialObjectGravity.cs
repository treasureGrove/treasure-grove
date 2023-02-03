using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class artificialObjectGravity : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;
	public float rayDistance;
	public PhysicMaterial highFrictionMaterial;

	public float gravityForce = 9.8f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool onGround;

	public bool active = true;

	public Vector3 normal;
	public Vector3 hitPoint;
	public Vector3 auxNormal;

	public bool normalAssigned;

	[Space]
	[Header ("Center Point Settings")]
	[Space]

	public bool useCenterPointActive;
	public Transform currentCenterPoint;
	public bool useInverseDirectionToCenterPoint;

	public bool useCenterPointListForRigidbodies;
	public List<Transform> centerPointList = new List<Transform> ();

	RaycastHit hit;
	bool onGroundChecked;
	float groundAdherence = 10;
	Rigidbody mainRigidbody;
	Collider mainCollider;
	bool objectActivated;
	float originalGravityForce;

	Vector3 currentNormalDirection;

	float minDistance;
	float currentDistance;

	float downVelocity;

	bool useGravityActive;

	Vector3 currentPosition;

	bool mainRigidbodyLocated;


	public void getComponents ()
	{
		if (!objectActivated) {
			if (mainRigidbody == null) {
				mainRigidbody = GetComponent<Rigidbody> ();
			}

			if (mainRigidbody != null) {
				mainRigidbodyLocated = true;
			}

			if (mainCollider == null) {
				mainCollider = GetComponent<Collider> ();
			}

			objectActivated = true;
			originalGravityForce = gravityForce;
		}
	}

	//this script is added to an object with a rigidbody, to change its gravity, disabling the useGravity parameter, and adding force in a new direction
	//checking in the object is in its new ground or not
	void FixedUpdate ()
	{
		//if nothing pauses the script and the gameObject has rigidbody and it is not kinematic
		if (active && mainRigidbodyLocated) {
			if (!mainRigidbody.isKinematic) {
				//check if the object is on ground or in the air, to apply or not force in its gravity direction

				currentNormalDirection = normal;

				currentPosition = transform.position;

				if (useCenterPointActive) {

					if (useCenterPointListForRigidbodies) {
						minDistance = Mathf.Infinity;

						for (int i = 0; i < centerPointList.Count; i++) {
							currentDistance = GKC_Utils.distance (currentPosition, centerPointList [i].position);

							if (currentDistance < minDistance) {
								minDistance = currentDistance;
								currentCenterPoint = centerPointList [i];
							}
						}
					}

					if (useInverseDirectionToCenterPoint) {
						currentNormalDirection = currentPosition - currentCenterPoint.position;
					} else {
						currentNormalDirection = currentCenterPoint.position - currentPosition;
					}

					currentNormalDirection = currentNormalDirection / currentNormalDirection.magnitude;
				}

				if (onGround) {
					if (!onGroundChecked) {
						onGroundChecked = true;

						mainCollider.material = highFrictionMaterial;
					}
				} else {
					if (onGroundChecked) {
						onGroundChecked = false;

						mainCollider.material = null;
					}

					mainRigidbody.AddForce (gravityForce * mainRigidbody.mass * currentNormalDirection);

					if (useGravityActive) {
						mainRigidbody.useGravity = false;

						useGravityActive = false;
					}
				}
					
				//use a raycast to check the ground
				if (Physics.Raycast (currentPosition, currentNormalDirection, out hit, (rayDistance + transform.localScale.x / 2), layer)) {
					if (!hit.collider.isTrigger && hit.rigidbody == null) {
						onGround = true;

						downVelocity = transform.InverseTransformDirection (mainRigidbody.velocity).y;

						if (downVelocity > .5f) {
							mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, hit.point, Time.deltaTime * groundAdherence);
						}

						if (downVelocity < .01f) {
							mainRigidbody.velocity = Vector3.zero;
						}
					}
				} else {
					onGround = false;
				}		
			}
		}

		//if the gameObject has not rigidbody, remove the script
		if (!mainRigidbodyLocated) {
			gameObject.layer = LayerMask.NameToLayer ("Default");

			Destroy (this);
		}
	}

	//when the object is dropped, set its forward direction to move until a surface will be detected
	public void enableGravity (LayerMask layer, PhysicMaterial frictionMaterial, Vector3 normalDirection)
	{
		getComponents ();

		this.layer = layer;

		highFrictionMaterial = frictionMaterial;

		mainRigidbody.useGravity = false;

		useGravityActive = false;

		normal = normalDirection;

		normalAssigned = false;
	}

	public void removeGravity ()
	{
		//set the layer again to default, active the gravity and remove the script
		gameObject.layer = LayerMask.NameToLayer ("Default");

		if (mainRigidbody != null) {
			mainRigidbody.useGravity = true;

			useGravityActive = true;
		}

		Destroy (this);
	}

	public void removeGravityComponent ()
	{
		gameObject.layer = LayerMask.NameToLayer ("Default");

		Destroy (this);
	}

	public void removeJustGravityComponent ()
	{
		Destroy (this);
	}

	void OnCollisionEnter (Collision collision)
	{
		//when the objects collides with anything, use the normal of the collision
		if (active && collision.gameObject.layer != LayerMask.NameToLayer ("Ignore Raycast") && !normalAssigned && !mainRigidbody.isKinematic) {
			//get the normal of the collision
			Vector3 direction = collision.contacts [0].normal;

			//Debug.DrawRay (transform.position,-direction, Color.red, 200,false);
			if (Physics.Raycast (transform.position, -direction, out hit, 3, layer)) {
				if (!hit.collider.isTrigger && hit.rigidbody == null) {
					normal = -hit.normal;
					//the hit point is used for the turret rotation
					hitPoint = hit.point;

					// @todo Tag doesn't exist, layer used now in stead. Fix me when refactoring this code.
					bool isTurret = gameObject.CompareTag ("turret");

					//check the type of object
					if (isTurret) {
						//if the direction is the actual ground, remove the script to set the regular gravity
						if (normal == -Vector3.up) {
							removeGravity ();

							return;
						}

						normalAssigned = true;
					}

					//if the object is an ally turret, call a function to set it kinematic when it touch the ground
					if (isTurret) {
						if (!mainRigidbody.isKinematic) {
							StartCoroutine (rotateToSurface ());
						}
					}
				}
			}
		}
	}

	//when an ally turret hits a surface, rotate the turret to that surface, so the player can set a turret in any place to help him
	IEnumerator rotateToSurface ()
	{
		mainRigidbody.useGravity = true;
		mainRigidbody.isKinematic = true;

		useGravityActive = true;

		//it rotates the turret in the same way that the player rotates with his gravity power
		Quaternion rot = transform.rotation;
		Vector3 myForward = Vector3.Cross (transform.right, -normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, -normal);

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;

			transform.rotation = Quaternion.Slerp (rot, dstRot, t);
			transform.position = Vector3.MoveTowards (transform.position, hitPoint + transform.up * 0.5f, t);

			yield return null;
		}

		gameObject.layer = LayerMask.NameToLayer ("Default");

		//if the surface is the regular ground, remove the artificial gravity, and make the turret stays kinematic when it will touch the ground
		if (-normal == Vector3.up) {
			SendMessage ("enabledKinematic", false);

			removeGravity ();
		}
	}

	//set directly a new normal
	public void setCurrentGravity (Vector3 newNormal)
	{
		getComponents ();

		mainRigidbody.useGravity = false;

		normal = newNormal;

		normalAssigned = true;

		useGravityActive = false;
	}

	public void setUseCenterPointActiveState (bool state, Transform newCenterPoint)
	{
		useCenterPointActive = state;

		currentCenterPoint = newCenterPoint;
	}

	public void setUseInverseDirectionToCenterPointState (bool state)
	{
		useInverseDirectionToCenterPoint = state;
	}

	public void setGravityForceValue (bool setOriginal, float newValue)
	{
		if (setOriginal) {
			gravityForce = originalGravityForce;
		} else {
			gravityForce = newValue;
		}
	}

	public void setUseCenterPointListForRigidbodiesState (bool state, List<Transform> newCenterPointList)
	{
		useCenterPointListForRigidbodies = state;
		centerPointList = newCenterPointList;
	}
}