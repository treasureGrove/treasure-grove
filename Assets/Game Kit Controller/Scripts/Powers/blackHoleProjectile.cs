using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class blackHoleProjectile : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public float getObjectsRadius = 40;
	public float blackHoleAttractionForce = 150;
	public GameObject openingBlackHoleParticles;
	public GameObject closingBlackHoleParticles;
	public List<string> tagToLocate = new List<string> ();


	public List<GameObject> elementsToAbsorve = new List<GameObject> ();

	public List<Rigidbody> rigidbodiesToAbsorve = new List<Rigidbody> ();

	public float minDistanceToStopAttraction = 4;

	public float damageRate = 0.5f;

	[Space]
	[Header ("Black Hole Duration Settings")]
	[Space]

	public float stopBulletTimer = 1;
	public float blackHoleDuration = 10;

	public float timeToOpenBlackHoleAfterShoot = 0.5f;
	public float timeToActivateOpenParticles = 3.5f;
	public float timeToActivateCloseParticles = 4;
	public float timeToCloseBlackHole = 0.5f;

	public float timeToDestroyParticles = 10;

	bool abilityActivated;
	bool stopBullet;

	bool openingBlackHoleParticlesCreated;
	bool closingBlackHoleParticlesCreated;

	bool openParticlesActivated;


	Vector3 blackHoleDirection;
	Vector3 blackHoleScale;
	float lastTimeDamage;

	bool canDamage;

	Vector3 currentPosition;

	List<Collider> colliders = new List<Collider> ();
	Rigidbody currentRigidbody;

	float lastTimeProjectileFired;

	float lastTimeBlackHoleActive;

	bool objectsStored;

	bool blackHoleActivated;

	bool blackHoleCloseActivated;

	void Update ()
	{
		if (abilityActivated) {

			currentPosition = transform.position;

			//black hole
			//when the bullet touchs a surface, or the timer reachs the limit, set the bullet kinematic, and activate the black hole
			if (blackHoleActivated) {
				if (Time.time > lastTimeBlackHoleActive + timeToOpenBlackHoleAfterShoot) {
					if (!openingBlackHoleParticlesCreated) {
						openingBlackHoleParticles = createParticles (openingBlackHoleParticles, timeToDestroyParticles);

						openingBlackHoleParticlesCreated = true;
					}

					//get all the objects inside a radius
					if (!objectsStored) {
						if (colliders.Count > 0) {
							colliders.Clear ();
						}

						colliders.AddRange (Physics.OverlapSphere (currentPosition, getObjectsRadius, currentProjectileInfo.targetToDamageLayer));

						foreach (Collider hit in colliders) {
							if (applyDamage.checkIfCharacterCanBePushedOnExplosions (hit.gameObject)) {
								Vector3 explosionDirection = currentPosition - hit.gameObject.transform.position;
								explosionDirection = explosionDirection / explosionDirection.magnitude;

								applyDamage.pushRagdoll (hit.gameObject, explosionDirection);
							}
						}

						if (colliders.Count > 0) {
							objectsStored = true;
						}

						if (rigidbodiesToAbsorve.Count > 0) {
							rigidbodiesToAbsorve.Clear ();
						}

						if (elementsToAbsorve.Count > 0) {
							elementsToAbsorve.Clear ();
						}

						colliders.Clear ();

						colliders.AddRange (Physics.OverlapSphere (currentPosition, getObjectsRadius, currentProjectileInfo.targetToDamageLayer));

						foreach (Collider hit in colliders) {
							if (hit != null) {
								GameObject currentGameObject = hit.gameObject;

								bool isCharacterOrVehicle = false;

								bool isRigidbody = false;

								GameObject target = applyDamage.getCharacterOrVehicle (currentGameObject);

								if (target != null) {
									currentGameObject = target;
									isCharacterOrVehicle = true;

								} else {
									if (applyDamage.canApplyForce (currentGameObject)) {
										isRigidbody = true;
									}
								}

								if (!elementsToAbsorve.Contains (currentGameObject) && tagToLocate.Contains (currentGameObject.tag) && (isCharacterOrVehicle || isRigidbody)) {
									currentRigidbody = currentGameObject.GetComponent<Rigidbody> ();

									if (currentRigidbody != null) {
										currentRigidbody.isKinematic = false;
									}

									//set the kinematic rigigbody of the enemies to false, to attract them
									currentGameObject.SendMessage ("pauseAI", true, SendMessageOptions.DontRequireReceiver);

									elementsToAbsorve.Add (currentGameObject);

									rigidbodiesToAbsorve.Add (currentRigidbody);
								}
							}
						}
					}

					if (objectsStored && !blackHoleCloseActivated) {
						for (int i = 0; i < rigidbodiesToAbsorve.Count; i++) {
							currentRigidbody = rigidbodiesToAbsorve [i];

							if (currentRigidbody != null) {

								if (GKC_Utils.distance (currentPosition, currentRigidbody.position) < minDistanceToStopAttraction) {
									currentRigidbody.velocity = Vector3.zero;
									currentRigidbody.useGravity = false;
								} else {
									blackHoleDirection = currentPosition - currentRigidbody.position;

									blackHoleScale = Vector3.Scale (blackHoleDirection.normalized, gameObject.transform.localScale);

									currentRigidbody.AddForce (blackHoleScale * (blackHoleAttractionForce * currentRigidbody.mass), currentProjectileInfo.forceMode);
								}

								if (canDamage) {
									applyDamage.checkHealth (gameObject, elementsToAbsorve [i], currentProjectileInfo.projectileDamage, -transform.forward, 
										currentRigidbody.position, currentProjectileInfo.owner, true, false, currentProjectileInfo.ignoreShield, false, false, -1,
										currentProjectileInfo.damageTypeID);
								}
							}
						}

						if (canDamage) {
							canDamage = false;
						}

						if (Time.time > damageRate + lastTimeDamage) {
							lastTimeDamage = Time.time;

							canDamage = true;
						}
					}
				}

				//activate the particles, they are activated and deactivated according to the timer value
				if (Time.time > lastTimeBlackHoleActive + timeToActivateCloseParticles) {
					if (!closingBlackHoleParticlesCreated) {
						closingBlackHoleParticles = createParticles (closingBlackHoleParticles, timeToDestroyParticles);

						closingBlackHoleParticlesCreated = true;

						if (openingBlackHoleParticles.activeSelf) {
							openingBlackHoleParticles.SetActive (false);
						}
					}
				}

				if (!openParticlesActivated) {
					if (Time.time > lastTimeBlackHoleActive + timeToActivateOpenParticles && openingBlackHoleParticles) {
						if (!openingBlackHoleParticles.activeSelf) {
							openingBlackHoleParticles.SetActive (true);
						}

						openParticlesActivated = true;
					}
				}

				//when the time is finishing, apply an explosion force to all the objects inside the black hole, and make an extra damage to all of them
				if (!blackHoleCloseActivated && Time.time > lastTimeBlackHoleActive + timeToCloseBlackHole && blackHoleActivated) {
					for (int i = 0; i < rigidbodiesToAbsorve.Count; i++) {
						currentRigidbody = rigidbodiesToAbsorve [i];

						if (currentRigidbody != null) {

							currentRigidbody.useGravity = true;

							currentRigidbody.AddExplosionForce (currentProjectileInfo.explosionForce * currentRigidbody.mass, currentPosition, currentProjectileInfo.explosionRadius, 3);	

							currentRigidbody.SendMessage ("pauseAI", false, SendMessageOptions.DontRequireReceiver);

							applyDamage.checkHealth (gameObject, currentRigidbody.gameObject, currentProjectileInfo.projectileDamage * 10, -transform.forward, 
								currentRigidbody.position, currentProjectileInfo.owner, false, false, currentProjectileInfo.ignoreShield, false, false, -1,
								currentProjectileInfo.damageTypeID);
						}
					}

					blackHoleCloseActivated = true;
				}

				//destroy the black hole bullet
				if (Time.time > lastTimeBlackHoleActive + blackHoleDuration) {
					Destroy (gameObject);
				}
			}

			//when a black hole bullet is shooted, if it does not touch anything in a certain amount of time, set it kinematic and open the black hole
			if (stopBullet) {
				if (lastTimeProjectileFired == 0) {
					lastTimeProjectileFired = Time.time;
				}
			
				if (Time.time > lastTimeProjectileFired + stopBulletTimer) {
					mainRigidbody.isKinematic = true;
					mainRigidbody.useGravity = false;

					stopBullet = false;

					lastTimeBlackHoleActive = Time.time;

					blackHoleActivated = true;
				}
			}
		}
	}

	public void dropBlackHoleObjects ()
	{
		for (int i = 0; i < rigidbodiesToAbsorve.Count; i++) {
			currentRigidbody = rigidbodiesToAbsorve [i];

			if (currentRigidbody != null) {

				if (currentRigidbody != null) {
					currentRigidbody.useGravity = true;
					currentRigidbody.AddExplosionForce (currentProjectileInfo.explosionForce, transform.position, currentProjectileInfo.explosionRadius, 3);
				}
			}
		}

		if (openingBlackHoleParticles) {
			openingBlackHoleParticles.SetActive (false);
		}
	}

	GameObject createParticles (GameObject particles, float timeToDestroy)
	{
		GameObject newParticles = (GameObject)Instantiate (particles, transform.position, transform.rotation);
		//newParticles.transform.SetParent (transform.parent);
		newParticles.AddComponent<destroyGameObject> ().setTimer (timeToDestroy);

		return newParticles;
	}

	//when the bullet touchs a surface, then
	void OnTriggerEnter (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;
			//set the bullet kinematic

			objectToDamage = col.GetComponent<Collider> ().gameObject;

			if (!objectToDamage.GetComponent<Rigidbody> () && stopBullet) {
				mainRigidbody.isKinematic = true;
			}
		}
	}

	//set these bools when the bullet is a black hole
	public void stopBlackHole ()
	{
		stopBullet = true;
	}

	public void activateProjectileAbility ()
	{
		abilityActivated = true;

		//if the bullet type is a black hole, remove any other black hole in the scene and set the parameters in the bullet script 
		//the bullet with the black hole has activated the option useGravity in its rigidbody
		blackHoleProjectile[] blackHoleList = FindObjectsOfType<blackHoleProjectile> ();

		foreach (blackHoleProjectile blackHole in blackHoleList) {
			if (blackHole.gameObject != gameObject) {
				
				blackHole.dropBlackHoleObjects ();
				Destroy (blackHole.gameObject);
			}
		}

		mainRigidbody.useGravity = true;

		stopBlackHole ();

		GetComponent<SphereCollider> ().radius *= 5;

		TrailRenderer currentTrailRenderer = GetComponent<TrailRenderer> ();

		currentTrailRenderer.startWidth = 4;
		currentTrailRenderer.time = 2;
		currentTrailRenderer.endWidth = 3;
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();

		abilityActivated = false;
		stopBullet = false;

		openingBlackHoleParticlesCreated = false;
		closingBlackHoleParticlesCreated = false;

		openParticlesActivated = false;

		lastTimeDamage = 0;

		canDamage = false;

		lastTimeProjectileFired = 0;

		lastTimeBlackHoleActive = 0;

		objectsStored = false;

		blackHoleActivated = false;

		blackHoleCloseActivated = false;
	}
}