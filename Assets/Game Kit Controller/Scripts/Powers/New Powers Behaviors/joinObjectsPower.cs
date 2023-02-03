using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joinObjectsPower : MonoBehaviour
{
	public bool powerEnabled = true;
	public otherPowers powersManager;

	public float joinObjectsForce = 40;
	public LayerMask layer;
	public Transform mainCameraTransform;

	public GameObject impactParticles;

	public GameObject playerGameObject;

	public float timeToRemoveJoin = 5;

	public float extraForceOnVehicles = 1;

	public bool useForceMode;
	public ForceMode forceModeObject1;
	public ForceMode forceModeObject2;

	public Transform joinObject1;
	public Transform joinObject2;
	GameObject joinParticles1;
	GameObject joinParticles2;
	Vector3 joinDirection1;
	Vector3 joinDirection2;
	Vector3 joinPosition1;
	Vector3 joinPosition2;

	public bool joinKinematic1;
	public bool joinKinematic2;

	public bool joinObjects;

	RaycastHit hit;

	checkCollisionType currentCheckCollisionType1;
	checkCollisionType currentCheckCollisionType2;
	public Rigidbody currentRigidbody1;
	public Rigidbody currentRigidbody2;
	Vector3 heading1;
	Vector3 heading2;
	ParticleSystem particles1;
	ParticleSystem particles2;

	bool currentRigidbody1IsVehicle;
	bool currentRigidbody2IsVehicle;

	float lastTimeJoined;

	void FixedUpdate ()
	{
		//two objects are going to be attracted each other
		if (joinObjects) {
			if (Time.time > lastTimeJoined + timeToRemoveJoin) {
				removeObjectsjoin ();
				return;
			}

			//when both objects are stored, then it is checked if any of them have a rigidbody, to add force to them or not
			//to this, it is used checkCollisionType, a script that allows to check any type of collision with collider or triggers and enter or exit
			//and also can be configurated if the player want to check if the collision is with a particular object, in this case to both join object
			//the collision to check is the opposite object

			//once the script is added to every object, then the direction of the force to applied is calculated, and checking to which object can be applied
			if (currentCheckCollisionType1 && currentCheckCollisionType2) {
				//check if the player has not rigidbody, so the position to follow is the hit point
				if (joinKinematic1) {
					joinParticles2.transform.transform.LookAt (joinPosition1);
					heading1 = joinObject2.position - joinPosition1;
				} else {
					//else, the position of the direction is the position of the object 
					//also a couple of particles are added
					joinParticles2.transform.transform.LookAt (joinObject1.position);
					heading1 = joinObject2.position - joinObject1.position;
				}

				joinDirection1 = heading1 / heading1.magnitude;

				if (joinKinematic2) {
					joinParticles1.transform.transform.LookAt (joinPosition2);
					heading2 = joinObject1.position - joinPosition2;
				} else {
					joinParticles1.transform.transform.LookAt (joinObject2.position);
					heading2 = joinObject1.position - joinObject2.position;
				}

				joinDirection2 = heading2 / heading2.magnitude; 

				if (!particles1) {
					particles1 = joinParticles1.GetComponent<ParticleSystem> ();
				}

				if (particles1) {
					var particles1Main = particles1.main;
					particles1Main.startSpeed = GKC_Utils.distance (joinParticles1.transform.position, joinObject2.position) / 2;
				}

				if (!particles2) {
					particles2 = joinParticles2.GetComponent<ParticleSystem> ();
				}

				if (particles2) {
					var particles2Main = particles2.main;
					particles2Main.startSpeed = GKC_Utils.distance (joinParticles2.transform.position, joinObject1.position) / 2;
				}

				//add force to the object, according to the direction of the other object
				if (currentRigidbody1 && currentRigidbody2) {
					addForce (-joinDirection2 * (joinObjectsForce * currentRigidbody1.mass), currentRigidbody1, forceModeObject1, currentRigidbody1IsVehicle);
					addForce (-joinDirection1 * (joinObjectsForce * currentRigidbody2.mass), currentRigidbody2, forceModeObject2, currentRigidbody2IsVehicle);
				} else if (currentRigidbody1 && !currentRigidbody2) {
					addForce (-joinDirection2 * (joinObjectsForce * currentRigidbody1.mass), currentRigidbody1, forceModeObject1, currentRigidbody1IsVehicle);
				} else if (!currentRigidbody1 && currentRigidbody2) {
					addForce (-joinDirection1 * (joinObjectsForce * currentRigidbody2.mass), currentRigidbody2, forceModeObject2, currentRigidbody2IsVehicle);
				} else {
					//if both objects have not rigidbodies, then cancel the join
					removeObjectsjoin ();
					return;
				}

				//if the collision happens, the scripts are removed, and every object return to their normal situation
				if (currentCheckCollisionType1.active || currentCheckCollisionType2.active) {
					removeObjectsjoin ();
				}
			}
		}
	}

	public void addForce (Vector3 direction, Rigidbody rigidbodyAffected, ForceMode forceModeToUsse, bool isVehicle)
	{
		if (isVehicle) {
			direction *= extraForceOnVehicles;
		}
		if (useForceMode) {
			rigidbodyAffected.AddForce (direction, forceModeToUsse);
		} else {
			rigidbodyAffected.AddForce (direction);
		}	
	}

	public void activatePower ()
	{
		if (!powerEnabled) {
			return;
		}

		//this power allows the player to join two objects, and add force to both in the position of the other, checking if any of the objects 
		//has rigidbody or not, and when both objects collide, the join is disabled
		if (!joinObject1 || !joinObject2) {
			powersManager.createShootParticles ();
			//get every object using a raycast
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layer)) {
				if (!joinObject1) {
					joinObject1 = hit.collider.transform;

					GameObject characterDetected = applyDamage.getCharacterOrVehicle (joinObject1.gameObject);

					if (characterDetected != null) {
						currentRigidbody1IsVehicle = applyDamage.isVehicle (characterDetected);
						joinObject1 = characterDetected.transform;
					} 

					if (!applyDamage.canApplyForce (joinObject1.gameObject)) {
						joinKinematic1 = true;
						joinPosition1 = hit.point;
					}
					joinParticles1 = (GameObject)Instantiate (impactParticles, hit.point, Quaternion.LookRotation (hit.normal));
					joinParticles1.transform.SetParent (joinObject1);
					joinParticles1.SetActive (true);
					return;
				}

				if (!joinObject2 && joinObject1 != hit.collider.transform && !hit.collider.transform.IsChildOf (joinObject1)) {
					joinObject2 = hit.collider.transform;

					GameObject characterDetected = applyDamage.getCharacterOrVehicle (joinObject2.gameObject);

					if (characterDetected != null) {
						currentRigidbody2IsVehicle = applyDamage.isVehicle (characterDetected);
						joinObject2 = characterDetected.transform;
					} 

					if (!applyDamage.canApplyForce (joinObject2.gameObject)) {
						joinKinematic2 = true;
						joinPosition2 = hit.point;
					}
					joinParticles2 = (GameObject)Instantiate (impactParticles, hit.point, Quaternion.LookRotation (hit.normal));
					joinParticles2.transform.SetParent (joinObject2);
					joinParticles2.SetActive (true);
				}
			}
		} 

		if (joinObject1 && joinObject2) {
			lastTimeJoined = Time.time;
			joinObjects = true;

			currentCheckCollisionType1 = joinObject1.GetComponent<checkCollisionType> ();
			currentCheckCollisionType2 = joinObject2.GetComponent<checkCollisionType> ();
			currentRigidbody1 = applyDamage.applyForce (joinObject1.gameObject);
			currentRigidbody2 = applyDamage.applyForce (joinObject2.gameObject);

			if (!currentCheckCollisionType1 && !currentCheckCollisionType2) {
				joinObject1.gameObject.AddComponent<checkCollisionType> ();
				joinObject2.gameObject.AddComponent<checkCollisionType> ();
				currentCheckCollisionType1 = joinObject1.GetComponent<checkCollisionType> ();
				currentCheckCollisionType2 = joinObject2.GetComponent<checkCollisionType> ();

				currentCheckCollisionType1.onCollisionEnter = true;
				currentCheckCollisionType2.onCollisionEnter = true;
				currentCheckCollisionType1.objectToCollide = joinObject2.gameObject;
				currentCheckCollisionType2.objectToCollide = joinObject1.gameObject;

				if (currentRigidbody1) {
					currentRigidbody1.useGravity = false;
				}

				if (currentRigidbody2) {
					currentRigidbody2.useGravity = false;
				}

				//a join object can be used to be launched to an enemy, hurting him, to check this, it is used launchedObjects
				if (!joinKinematic1) {
					joinObject1.gameObject.AddComponent<launchedObjects> ().setCurrentPlayer (playerGameObject);
				}
				if (!joinKinematic2) {
					joinObject2.gameObject.AddComponent<launchedObjects> ().setCurrentPlayer (playerGameObject);
				}
			}
		}
	}

	//if none of the objects join have rigidbody, the join is cancelled
	public void removeObjectsjoin ()
	{
		if (currentCheckCollisionType1) {
			Destroy (currentCheckCollisionType1);
		}
			
		if (currentCheckCollisionType2) {
			Destroy (currentCheckCollisionType2);
		}

		launchedObjects currentLaunchedObjects1 = joinObject1.GetComponent<launchedObjects> ();
		if (currentLaunchedObjects1) {
			Destroy (currentLaunchedObjects1);
		}

		launchedObjects currentLaunchedObjects2 = joinObject2.GetComponent<launchedObjects> ();
		if (currentLaunchedObjects2) {
			Destroy (currentLaunchedObjects2);
		}
			
		if (currentRigidbody1) {
			currentRigidbody1.useGravity = true;
			currentRigidbody1 = null;
		}
	
		if (currentRigidbody2) {
			currentRigidbody2.useGravity = true;
			currentRigidbody2 = null;
		}

		joinObjects = false;
		joinObject1 = null;
		joinObject2 = null;
		joinKinematic1 = false;
		joinKinematic2 = false;
		Destroy (joinParticles1);
		Destroy (joinParticles2);

		particles1 = null;
		particles2 = null;

		currentRigidbody1IsVehicle = false;
		currentRigidbody2IsVehicle = false;
	}
}