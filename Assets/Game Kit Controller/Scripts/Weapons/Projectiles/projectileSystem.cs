using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class projectileSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setContinuousSpeculativeIngame;

	public string abilityFunctionNameAtStart;
	public string abilityFunctionName;
	public bool useCustomValues;
	public projectileInfo currentProjectileInfo;

	public string armorSurfaceLayer = "Armor Surface";

	public bool disableProjectileMeshOnImpact = true;

	[HideInInspector] public GameObject objectToDamage;
	[HideInInspector] public RaycastHit hit;
	[HideInInspector] public bool projectileUsed;

	[HideInInspector] public bool projectilePaused;

	[HideInInspector] public Vector3 projectileLocalRotation;

	[Space]
	[Space]
	[Space]

	public bool useLayerMaskImpact;
	public LayerMask layerMaskImpact;

	public bool useEventOnImpact;
	public UnityEvent eventOnImpact;

	public bool sendObjectDetectedOnImpactEvent;
	public eventParameters.eventToCallWithGameObject objectDetectedOnImpactEvent;

	public bool useLayerMaskCollision;
	public LayerMask layerMaskCollision;

	public bool useEventOnCollision;
	public UnityEvent eventOnCollision;

	public bool useEventOnExplosion;
	public UnityEvent evenOnExplosion;

	[Space]
	[Space]
	[Space]

	public TrailRenderer mainTrailRenderer;
	public Collider mainCollider;
	public MeshCollider secondaryBulletMeshCollider;

	public mapObjectInformation mainMapObjectInformation;
	public audioClipBip audioClipBipManager;
	public Rigidbody mainRigidbody;

	public GameObject bulletMesh;
	public GameObject secondaryBulletMesh;

	public projectileImpactSystem mainProjectileImpactSystem;

	public fakeProjectileTrailSystem fakeProjectileTrail;

	bool explosionActivated;

	float currentRotationSpeed;
	float rotationSpeed = 10;
	float targetPositionOffset = 0.1f;
	Quaternion currentRotation;
	Vector3 velocityDirection;
	Vector3 currentTargetDirection;
	Vector3 currentTargetPosition;
	float currentAngleWithTarget;
	float currentDistanceToTarget;
	float currentDistance;

	bool projectileCaptured;

	bool impactParticlesCreated;

	healthManagement mainHealthManagement;

	bool projectileAttachedToSurface;

	int currentNumberOfImpacts;
	List<GameObject> listOfGameObjectImpacts = new List<GameObject> ();
	List<Collider> listOfColliderImpacts = new List<Collider> ();

	float orbitingTargetTime;
	Vector3 currentProjectilePosition;

	Coroutine surfaceCoroutine;

	Vector3 originalProjectilePosition;

	Vector3 movementThisStep;
	float movementSqrMagnitude;
	float movementMagnitude;

	float minimumExtent;
	float sqrMinimumExtent;
	Vector3 previousPosition;

	float lastTimeRaycastCheckingActive;

	int loop;

	bool collisionDetectionChecked;

	bool pauseCheckTriggerActive;

	bool projectileRemovedPreviously;

	public bool projectileInitialized;

	Transform currentTargetTransform;

	Vector3 targetPosition;

	float lastTimeFired = -1;


	void Update ()
	{
		if ((currentProjectileInfo.isHommingProjectile || currentProjectileInfo.isSeeker) && !mainRigidbody.isKinematic) {
			currentProjectilePosition = transform.position;

			mainRigidbody.velocity = transform.forward * currentProjectileInfo.projectileSpeed;

			if (currentTargetTransform != null) {
				targetPosition = currentTargetTransform.position;

				currentTargetPosition = targetPosition + (currentTargetTransform.up * targetPositionOffset);

				currentAngleWithTarget = Vector3.Angle (transform.up, currentTargetDirection);

				if (currentAngleWithTarget > 80 && currentAngleWithTarget < 100) {
					orbitingTargetTime += Time.deltaTime;
				}

				if (orbitingTargetTime > 0.8f) {
					orbitingTargetTime = 0;
					rotationSpeed *= 2;
				}

				currentDistanceToTarget = GKC_Utils.distance (targetPosition, currentProjectilePosition);

				currentRotationSpeed = rotationSpeed + (1 / currentDistanceToTarget);

				currentTargetDirection = currentTargetPosition - currentProjectilePosition;
				currentRotation = Quaternion.LookRotation (currentTargetDirection);
				transform.rotation = Quaternion.Lerp (transform.rotation, currentRotation, Time.deltaTime * currentRotationSpeed);
			}

			if (projectileCaptured) {
				if (currentProjectileInfo.target == null) {
					Collider[] colliders = Physics.OverlapSphere (currentProjectilePosition, currentProjectileInfo.explosionRadius, currentProjectileInfo.targetToDamageLayer);

					if (colliders.Length > 0) {
						float minDistance = Mathf.Infinity;

						for (int i = 0; i < colliders.Length; i++) {
							currentDistance = GKC_Utils.distance (colliders [i].transform.position, currentProjectilePosition);

							if (currentDistance < minDistance) {
								minDistance = currentDistance;
								currentProjectileInfo.target = colliders [i].gameObject;

								currentTargetTransform = currentProjectileInfo.target.transform;
							}
						}
					}
				}
			}
		}
			
		//destroy the projectile 
		if (currentProjectileInfo.useDisableTimer && !projectilePaused) {
			if (currentProjectileInfo.noImpactDisableTimer > 0) {
				currentProjectileInfo.noImpactDisableTimer -= Time.deltaTime;

				if (currentProjectileInfo.noImpactDisableTimer < 0) {
					if (!projectileUsed) {
						if (currentProjectileInfo.isExplosive || currentProjectileInfo.isImplosive) {
							setProjectileUsedState (true);

							checkExplodeProjectile ();

							return;
						}
					}

					destroyProjectile ();

					return;
				}
			}
		}

		if (currentProjectileInfo.launchProjectile) {
			if (mainRigidbody.velocity != Vector3.zero) {
				velocityDirection = mainRigidbody.velocity;
			}
		}

		if (projectileUsed) {
			if (projectileAttachedToSurface) {
				if (mainHealthManagement != null) {
					if (mainHealthManagement.checkIfDeadWithHealthManagement ()) {
//						print ("target dead");

						parentDestroyed ();
					}
				}
			}
		}
	}

	void FixedUpdate ()
	{
		if (currentProjectileInfo.useRaycastCheckingOnRigidbody && !projectilePaused && !projectileUsed) {
			if (previousPosition != Vector3.zero) {
				movementThisStep = mainRigidbody.position - previousPosition; 
				movementSqrMagnitude = movementThisStep.sqrMagnitude;

				if (movementSqrMagnitude > sqrMinimumExtent ||
				    (currentProjectileInfo.customRaycastCheckingRate > 0 && Time.time > currentProjectileInfo.customRaycastCheckingRate + lastTimeRaycastCheckingActive)) { 
					movementMagnitude = Mathf.Sqrt (movementSqrMagnitude);

//					Debug.DrawLine (previousPosition, (previousPosition + movementThisStep * (movementMagnitude + currentProjectileInfo.customRaycastCheckingDistance)), Color.red, 5);

					if (Physics.Raycast (previousPosition, movementThisStep, out hit, movementMagnitude + currentProjectileInfo.customRaycastCheckingDistance, currentProjectileInfo.targetToDamageLayer)) {
						checkSurface (hit.collider);
					}

					lastTimeRaycastCheckingActive = Time.time;
				} 
			}

			previousPosition = mainRigidbody.position;
		}
	}

	public void parentDestroyed ()
	{
		if (secondaryBulletMeshCollider != null) {
			setProjectileParent (null);

			enableOrDisableProjectileCollider (true);
		} else {
			destroyProjectile ();
		}
	}

	public void setEnemy (GameObject obj)
	{
		StartCoroutine (missileWaiting (obj));
	}

	IEnumerator missileWaiting (GameObject obj)
	{
		yield return new WaitForSeconds (currentProjectileInfo.waitTimeToSearchTarget);

		currentProjectileInfo.target = obj;

		currentTargetTransform = applyDamage.getPlaceToShoot (currentProjectileInfo.target);

		if (currentTargetTransform == null) {
			currentTargetTransform = obj.transform;
		}
	}
		
	//when the bullet touchs a surface, then
	void OnTriggerEnter (Collider col)
	{
		if (pauseCheckTriggerActive) {
			return;
		}

		checkSurface (col);
	}

	public void checkSurface (Collider col)
	{
		if (currentProjectileInfo.projectileWithAbility) {
			if (abilityFunctionName != "") {
				SendMessage (abilityFunctionName, col, SendMessageOptions.DontRequireReceiver);
			}

			return;
		}

		//if the layer of the object found is in the layers list, then

		bool canActivateEffectResult = canActivateEffect (col);

		if (canActivateEffectResult) {
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			if (currentProjectileInfo.projectileCanBeDeflected) {
				if (objectToDamage.layer == LayerMask.NameToLayer (armorSurfaceLayer)) {
					if (!projectileCaptured) {
						armorSurfaceSystem currentarmorSurfaceSystem = objectToDamage.GetComponent<armorSurfaceSystem> ();

						if (currentarmorSurfaceSystem != null) {
							if (currentarmorSurfaceSystem.armorActive) {
								if (mainTrailRenderer != null) {
									mainTrailRenderer.enabled = false;
								}

								setProjectileParent (objectToDamage.gameObject.transform.parent);

								checkCollisionDetectionMode ();

								setKinematicState (true);

								projectilePaused = true;
								projectileCaptured = true;

								currentarmorSurfaceSystem.addProjectile (this);
							} else {
								return;
							}
						}
					}

					if (projectileCaptured) {
						return;
					}
				}
			}

			bool isNewGameObjectImpact = false;

			if (currentProjectileInfo.breakThroughObjects) {
				
				GameObject character = applyDamage.getCharacterOrVehicle (objectToDamage);

				if (character == null) {
					character = objectToDamage;
				}

				if (!listOfGameObjectImpacts.Contains (character) || currentProjectileInfo.canDamageSameObjectMultipleTimes) {
					listOfGameObjectImpacts.Add (character);
					currentNumberOfImpacts++;
					isNewGameObjectImpact = true;
				}

				if (!listOfColliderImpacts.Contains (col)) {
					listOfColliderImpacts.Add (col);
				}

				if (!currentProjectileInfo.infiniteNumberOfImpacts) {
					if (currentNumberOfImpacts >= currentProjectileInfo.numberOfImpacts) {
						setProjectileUsedState (true);
					}
				}

				if (currentNumberOfImpacts == 1 && !currentProjectileInfo.useRayCastShoot) {
					transform.rotation = Quaternion.LookRotation (currentProjectileInfo.forwardDirection);
					mainRigidbody.velocity = transform.forward * currentProjectileInfo.projectileSpeed;
				}
			} else {
				setProjectileUsedState (true);

				isNewGameObjectImpact = true;
			}

			objectToDamage = col.GetComponent<Collider> ().gameObject;

			Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

			if (projectileUsed) {
				//set the bullet kinematic
				if (objectToDamageRigidbody == null && !currentProjectileInfo.useGravityOnLaunch && !currentProjectileInfo.useGraivtyOnImpact &&
				    !currentProjectileInfo.useExplosionDelay) {
					checkCollisionDetectionMode ();

					setKinematicState (true);
				}

				if (currentProjectileInfo.useGravityOnLaunch || currentProjectileInfo.useGraivtyOnImpact || currentProjectileInfo.useExplosionDelay) {
					if (secondaryBulletMesh != null) {
						secondaryBulletMesh.SetActive (true);
					}

					enableOrDisableSecondaryMeshCollider (true);
				}
			}

			checkCollisionType currentCheckCollisionType = objectToDamage.GetComponent<checkCollisionType> ();

			if (currentCheckCollisionType != null) {
				currentCheckCollisionType.checkTriggerWithGameObject (gameObject, true);
			}

			if (isNewGameObjectImpact) {
				//damage the surface where the proectile impacts
				if (currentProjectileInfo.killInOneShot) {
					Vector3 damageDirection = transform.forward;

					if (currentProjectileInfo.launchProjectile) {
						damageDirection = objectToDamage.transform.position - transform.position;

						damageDirection = damageDirection / damageDirection.magnitude;
					}

					applyDamage.killCharacter (gameObject, objectToDamage, -damageDirection, transform.position, 
						currentProjectileInfo.owner, false);
				} else {
					bool damageSurfaceMultipleTimes = false;

					if (currentProjectileInfo.breakThroughObjects && currentProjectileInfo.canDamageSameObjectMultipleTimes) {
						damageSurfaceMultipleTimes = true;
					}

					if (currentProjectileInfo.projectileDamage > 0 || currentProjectileInfo.damageTypeID > -1) {
						applyDamage.checkHealth (gameObject, objectToDamage, currentProjectileInfo.projectileDamage, -transform.forward, transform.position, 
							currentProjectileInfo.owner, damageSurfaceMultipleTimes, true, currentProjectileInfo.ignoreShield, false, 
							currentProjectileInfo.canActivateReactionSystemTemporally, currentProjectileInfo.damageReactionID, currentProjectileInfo.damageTypeID);
					}

					if (currentProjectileInfo.damageTargetOverTime) {
						applyDamage.setDamageTargetOverTimeState (objectToDamage, currentProjectileInfo.damageOverTimeDelay, currentProjectileInfo.damageOverTimeDuration, 
							currentProjectileInfo.damageOverTimeAmount,	currentProjectileInfo.damageOverTimeRate, currentProjectileInfo.damageOverTimeToDeath,
							currentProjectileInfo.damageTypeID);
					}

					if (currentProjectileInfo.removeDamageOverTimeState) {
						applyDamage.removeDamagetTargetOverTimeState (objectToDamage);
					}

					if (currentProjectileInfo.sedateCharacters) { 
						applyDamage.sedateCharacter (objectToDamage, transform.position, currentProjectileInfo.sedateDelay, currentProjectileInfo.useWeakSpotToReduceDelay, 
							currentProjectileInfo.sedateUntilReceiveDamage, currentProjectileInfo.sedateDuration);
					}

					if (currentProjectileInfo.pushCharacter) { 
						applyDamage.pushAnyCharacter (objectToDamage, currentProjectileInfo.pushCharacterForce, currentProjectileInfo.pushCharacterRagdollForce, transform.forward);
					}
				}

				if (currentProjectileInfo.useRemoteEventOnObjectsFound) {
					checkRemoteEvents (objectToDamage);
				}

				//apply force if the surface has a rigidbody
				if (currentProjectileInfo.applyImpactForceToVehicles) {
					Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

					if (objectToDamageMainRigidbody != null) {
						Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;

						bool vehicleDetected = applyDamage.isVehicle (objectToDamage);

						if (vehicleDetected) {
							force *= currentProjectileInfo.impactForceToVehiclesMultiplier;
						}

						float massForce = objectToDamageMainRigidbody.mass;

						if (currentProjectileInfo.forceMassMultiplier == 0) {
							massForce = 1;
						} else {
							massForce *= currentProjectileInfo.forceMassMultiplier;
						}

						objectToDamageMainRigidbody.AddForce (force * massForce, currentProjectileInfo.forceMode);
					}
				} else {
					if (applyDamage.canApplyForce (objectToDamage)) {
						Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;

						if (objectToDamageRigidbody == null) {
							objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();
						}

						float massForce = objectToDamageRigidbody.mass;

						if (currentProjectileInfo.forceMassMultiplier == 0) {
							massForce = 1;
						} else {
							massForce *= currentProjectileInfo.forceMassMultiplier;
						}

						objectToDamageRigidbody.AddForce (force * massForce, currentProjectileInfo.forceMode);
					}
				}

				externalDamageProperties currentExternalDamageProperties = objectToDamage.GetComponent<externalDamageProperties> ();

				if (currentExternalDamageProperties != null) {
					currentExternalDamageProperties.setDamageOwner (currentProjectileInfo.owner);
				}
			}

			if (currentProjectileInfo.isExplosive || currentProjectileInfo.isImplosive) {
				checkExplodeProjectile ();
			}

			if (currentProjectileInfo.launchProjectile) {
				transform.rotation = Quaternion.LookRotation (velocityDirection);
			}

			if (currentProjectileInfo.adhereToSurface) {
				attachProjectileToSurface ();

			} else {
				if (currentProjectileInfo.useGraivtyOnImpact) {
					setUseGravityState (true);
				}
			}

			if (isNewGameObjectImpact) {
				if (!currentProjectileInfo.isExplosive && !currentProjectileInfo.isImplosive) {
					setProjectileScorch ();

					disableBullet (currentProjectileInfo.impactDisableTimer);

					checkMapIcon ();
				}
			}
				
			if (currentProjectileInfo.breakThroughObjects && isNewGameObjectImpact && !projectileUsed && currentProjectileInfo.useRayCastShoot) {
				RaycastHit[] hits;

				hits = Physics.RaycastAll (transform.position, transform.forward, Mathf.Infinity, currentProjectileInfo.targetToDamageLayer);

				float maxDistance = Mathf.Infinity;
				int elementIndex = -1;
				Vector3 hitPoint = Vector3.zero;
				Vector3 currentPosition = transform.position;

				for (int i = 0; i < hits.Length; i++) {
					if (!listOfColliderImpacts.Contains (hits [i].collider)) {
						CharacterJoint boneJoint = hits [i].collider.GetComponent<CharacterJoint> ();

						if (boneJoint == null) {
							currentDistance = GKC_Utils.distance (currentPosition, hits [i].point);

							if (currentDistance < maxDistance) {
								maxDistance = currentDistance;
								elementIndex = i;

								hitPoint = hits [i].point;
							}
						}
					}
				}

				if (elementIndex > -1) {
					rayCastShoot (hits [elementIndex].collider, hitPoint, transform.forward);
				}
			}

			loop++;

			if (loop > 100) {
				print ("WARNING: loop in projectile system, stopping" + loop);

				return;
			}

			projectimeImpactState (objectToDamage);
		}
	}

	public void checkMapIcon ()
	{
		getProjectileMapInformation ();

		if (mainMapObjectInformation != null) {
			mainMapObjectInformation.removeMapObject ();
		}
	}

	public void createMapIconInfo ()
	{
		getProjectileMapInformation ();

		if (mainMapObjectInformation != null) {
			mainMapObjectInformation.createMapIconInfo ();
		}
	}

	Coroutine explosionCoroutine;

	public void checkExplodeProjectile ()
	{
		if (currentProjectileInfo == null) {
			return;
		}

		if (currentProjectileInfo.useExplosionDelay) {
			if (explosionCoroutine != null) {
				StopCoroutine (explosionCoroutine);
			}

			explosionCoroutine = StartCoroutine (explodeProjectileCoroutine ());
		} else {
			explodeProjectileFunction ();
		}
	}

	IEnumerator explodeProjectileCoroutine ()
	{
		currentProjectileInfo.noImpactDisableTimer = currentProjectileInfo.explosionDelay + 0.1f;

		getProjectileBipManager ();

		if (audioClipBipManager != null) {
			audioClipBipManager.increasePlayTime (currentProjectileInfo.explosionDelay);
		}

		yield return new WaitForSeconds (currentProjectileInfo.explosionDelay);

		explodeProjectileFunction ();
	}

	public void explodeProjectileFunction ()
	{
		if (!currentProjectileInfo.isExplosive && !currentProjectileInfo.isImplosive) {
			return;
		}

		explodeProjectile ();

		setProjectileScorch ();

		checkMapIcon ();

		disableBullet (currentProjectileInfo.impactDisableTimer);
	}

	void explodeProjectile ()
	{
		//explosion or implosion
		//get all the objects inside a radius in the impact position, applying to them an explosion force
		if (!explosionActivated) {
			creatImpactParticles ();

			Vector3 currentPosition = transform.position;

			Transform projectileOwner = transform;

			if (currentProjectileInfo.owner != null) {
				projectileOwner = currentProjectileInfo.owner.transform;
			}

			applyDamage.setExplosion (currentPosition, currentProjectileInfo.explosionRadius, true, currentProjectileInfo.targetToDamageLayer, currentProjectileInfo.owner, 
				currentProjectileInfo.canDamageProjectileOwner, gameObject, currentProjectileInfo.killInOneShot, currentProjectileInfo.isExplosive, currentProjectileInfo.isImplosive, 
				currentProjectileInfo.explosionDamage, currentProjectileInfo.pushCharacters, currentProjectileInfo.applyExplosionForceToVehicles, 
				currentProjectileInfo.explosionForceToVehiclesMultiplier, currentProjectileInfo.explosionForce, currentProjectileInfo.forceMode, false, 
				projectileOwner, currentProjectileInfo.ignoreShield, currentProjectileInfo.useRemoteEventOnObjectsFoundOnExplosion,
				currentProjectileInfo.remoteEventNameOnExplosion, currentProjectileInfo.damageTypeID);

			explosionActivated = true;

			if (useEventOnExplosion) {
				evenOnExplosion.Invoke ();
			}
		}
	}

	public void setProjectileScorch ()
	{
		bool scorchCreatedCorrectly = false;

		bool objectToDamageNotNull = objectToDamage != null;

		if (objectToDamageNotNull) {
			scorchCreatedCorrectly = decalManager.setImpactDecal (decalManager.checkIfHasDecalImpact (objectToDamage), 
				transform, objectToDamage, currentProjectileInfo.scorchRayCastDistance,
				currentProjectileInfo.targetForScorchLayer, true, currentProjectileInfo.projectilesPoolEnabled);
		} 
			
		if (!scorchCreatedCorrectly) {
			if (currentProjectileInfo.impactSoundEffect != null) {
				createImpactSound (currentProjectileInfo.impactSoundEffect);
			}

			if (currentProjectileInfo.scorch != null) {
				if (objectToDamageNotNull) {
					//the bullet fired is a simple bullet or a greanade, check the hit point with a raycast to set in it a scorch
					if (Physics.Raycast (transform.position - transform.forward * 0.4f, transform.forward, out hit, 
						    currentProjectileInfo.scorchRayCastDistance, currentProjectileInfo.targetForScorchLayer)) {

						decalManager.setScorch (transform.rotation, currentProjectileInfo.scorch, hit, objectToDamage, currentProjectileInfo.projectilesPoolEnabled);
					}
				}
			}

			creatImpactParticles ();
		} else {
			if (currentProjectileInfo.isExplosive || currentProjectileInfo.isImplosive) {
				createImpactSound (currentProjectileInfo.impactSoundEffect);
			}
		}
	}

	public void createImpactSound (AudioClip impactSoundEffect)
	{
		getMainProjectileImpactSystem ();

		if (mainProjectileImpactSystem != null) {
			if (currentProjectileInfo.projectilesPoolEnabled) {
				GameObject newProjectileImpactGameObject = GKC_PoolingSystem.Spawn (mainProjectileImpactSystem.gameObject, transform.position, transform.rotation, 30);

				projectileImpactSystem newProjectileImpactSystem = newProjectileImpactGameObject.GetComponent<projectileImpactSystem> ();

				newProjectileImpactSystem.setSendObjectToPoolSystemToDisableState (true);

				newProjectileImpactSystem.activateImpactElements (transform.position, impactSoundEffect);
			} else {

				if (currentProjectileInfo.projectilesPoolEnabled) {
					mainProjectileImpactSystem.changeDestroyForSetActiveFunction (true);
				}

				mainProjectileImpactSystem.activateImpactElements (transform.position, impactSoundEffect);
			}
		}
	}

	public bool canActivateEffect (Collider col)
	{
		if (col == null) {
			return false;
		}

		if (projectileUsed) {
			return false;
		}

		if (projectilePaused) {
			return false;
		}

		if ((1 << col.gameObject.layer & currentProjectileInfo.targetToDamageLayer.value) != 1 << col.gameObject.layer) {
			return false;
		}

		if (currentProjectileInfo.useCustomIgnoreTags && currentProjectileInfo.customTagsToIgnoreList.Contains (col.gameObject.tag)) {
			return false;
		}
			
		if (col.gameObject == currentProjectileInfo.owner) {
			if (!currentProjectileInfo.allowDamageForProjectileOwner) {
				if (!projectileCaptured) {
					return false;
				}
			}
		}

		if (currentProjectileInfo.launchProjectile && !currentProjectileInfo.useRayCastShoot && Time.time < lastTimeFired + 0.05f) {
			return false;
		}

		return true;
	}

	public void creatImpactParticles ()
	{
		if (currentProjectileInfo.impactParticles != null && !impactParticlesCreated) {
			if (currentProjectileInfo.projectilesPoolEnabled) {
				GKC_PoolingSystem.Spawn (currentProjectileInfo.impactParticles, transform.position, transform.rotation, currentProjectileInfo.maxAmountOfPoolElementsOnWeapon);
			} else {
				Instantiate (currentProjectileInfo.impactParticles, transform.position, transform.rotation);
			}

			impactParticlesCreated = true;
		}
	}

	//destroy the bullet according to the time value
	public void disableBullet (float time)
	{
		if (currentProjectileInfo.impactSoundEffect) {
			if (currentProjectileInfo.impactSoundEffect.length > time) {
				currentProjectileInfo.noImpactDisableTimer = currentProjectileInfo.impactSoundEffect.length;
			} else {
				currentProjectileInfo.noImpactDisableTimer = time;
			}
		} else {
			currentProjectileInfo.noImpactDisableTimer = time;
		}

		if (!currentProjectileInfo.breakThroughObjects) {
			checkCollisionDetectionMode ();

			setKinematicState (true);

			if (bulletMesh != null && disableProjectileMeshOnImpact) {
				bulletMesh.SetActive (false);
			}
		}
	}

	public void setKinematicState (bool state)
	{
		if (mainRigidbody != null) {
			mainRigidbody.isKinematic = state;
		}
	}
		
	//get the info of the current weapon selected, so the projectile has the correct behaviour
	public void setProjectileInfo (projectileInfo info)
	{
		if (useCustomValues) {
			return;
		}

		currentProjectileInfo = info;

		lastTimeFired = Time.time;
	}

	public void setProjectileOnwer (GameObject owner)
	{
		currentProjectileInfo.owner = owner;
	}

	public void setTargetToDamageLayer (LayerMask layer)
	{
		currentProjectileInfo.targetToDamageLayer = layer;
	}

	public void setUseCustomIgnoreTags (bool state, List<string> newCustomTagsToIgnoreList)
	{
		currentProjectileInfo.useCustomIgnoreTags = state;

		currentProjectileInfo.customTagsToIgnoreList = newCustomTagsToIgnoreList;
	}

	//if the projectiles is placed directly in the raycast hit point, place the projectile in the correct position
	public void rayCastShoot (Collider surface, Vector3 position, Vector3 forwardDirection)
	{
		originalProjectilePosition = transform.position;

		transform.position = position;

		getProjectileCollider ();

		currentProjectileInfo.useRayCastShoot = true;

		initializeProjectile ();

		transform.rotation = Quaternion.LookRotation (forwardDirection);

		if (currentProjectileInfo.useRaycastShootDelay) {
			if (bulletMesh != null && disableProjectileMeshOnImpact) {
				bulletMesh.SetActive (false);
			}

			pauseCheckTriggerActive = true;

			checkSurfaceWithDelay (surface);
		} else {
			checkSurface (surface);
		}
	}

	public void setProjectileLocalRotation (Vector3 newProjectileLocalRotation)
	{
		projectileLocalRotation = newProjectileLocalRotation;
	}

	public void checkSurfaceWithDelay (Collider surface)
	{
		if (surfaceCoroutine != null) {
			StopCoroutine (surfaceCoroutine);
		}

		surfaceCoroutine = StartCoroutine (checkSurfaceWithDelayCoroutine (surface));
	}

	IEnumerator checkSurfaceWithDelayCoroutine (Collider surface)
	{
		if (currentProjectileInfo.getDelayWithDistance) {

			float raycastShootDelay = GKC_Utils.distance (transform.position, originalProjectilePosition) / currentProjectileInfo.delayWithDistanceSpeed;

			if (currentProjectileInfo.maxDelayWithDistance > 0) {
				raycastShootDelay = Mathf.Clamp (raycastShootDelay, 0, currentProjectileInfo.maxDelayWithDistance);
			}

			if (currentProjectileInfo.useFakeProjectileTrails) {
				if (fakeProjectileTrail != null) {
					fakeProjectileTrail.setSpeedMultiplier (raycastShootDelay);
				}
			}

			yield return new WaitForSeconds (raycastShootDelay);
		} else {
			if (currentProjectileInfo.useFakeProjectileTrails) {
				if (fakeProjectileTrail != null) {
					fakeProjectileTrail.setSpeedMultiplier (currentProjectileInfo.raycastShootDelay);
				}
			}

			yield return new WaitForSeconds (currentProjectileInfo.raycastShootDelay);
		}

		pauseCheckTriggerActive = false;

		checkSurface (surface);

		if (currentProjectileInfo.useFakeProjectileTrails) {
			if (fakeProjectileTrail != null) {
				fakeProjectileTrail.stopTrailMovement ();
			}
		}
	}

	public void checkRemoteEvents (GameObject objectToCheck)
	{
		if (objectToCheck == null) {
			objectToCheck = objectToDamage;
		}

		if (objectToCheck == null) {
			return;
		}

		remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			for (int i = 0; i < currentProjectileInfo.remoteEventNameList.Count; i++) {
				currentRemoteEventSystem.callRemoteEvent (currentProjectileInfo.remoteEventNameList [i]);
			}
		}
	}

	public void checkRemoteEventOnProjectile (string remoteEventName)
	{
		remoteEventSystem currentRemoteEventSystem = GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			currentRemoteEventSystem.callRemoteEvent (remoteEventName);
		}
	}

	public void checkRemoteEventsOnProjectile (List<string> remoteEventNameList)
	{
		remoteEventSystem currentRemoteEventSystem = GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			for (int i = 0; i < remoteEventNameList.Count; i++) {
				currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
			}
		}
	}

	public void creatFakeProjectileTrail (Vector3 targetPosition)
	{
		if (fakeProjectileTrail != null) {
			fakeProjectileTrail.resetFakeProjectileTrail ();

			fakeProjectileTrail.transform.position = transform.position;

			fakeProjectileTrail.instantiateFakeProjectileTrail (targetPosition);

			if (currentProjectileInfo.projectilesPoolEnabled) {
				fakeProjectileTrail.changeDestroyForSetActiveFunction (true);
			}
		}
	}

	public void setDestroyTrailAfterTimeState (bool state)
	{
		if (fakeProjectileTrail != null) {
			fakeProjectileTrail.setDestroyTrailAfterTimeState (state);
		}
	}

	public void setFakeProjectileTrailSpeedMultiplier (float newValue)
	{
		if (fakeProjectileTrail != null) {
			fakeProjectileTrail.setSpeedMultiplier (newValue);
		}
	}

	public void destroyProjectile ()
	{
		checkMapIcon ();

		projectileRemovedPreviously = true;

		projectileInitialized = false;

		if (currentProjectileInfo.projectilesPoolEnabled) {
			GKC_PoolingSystem.Despawn (gameObject);

			resetProjectile ();

			setKinematicState (false);

			transform.position = Vector3.up * 3000;
		} else {
			Destroy (gameObject);
		}
	}

	public bool isProjectileUsed ()
	{
		return projectileUsed;
	}

	public void setProjectileUsedState (bool state)
	{
		projectileUsed = state;
	}

	public void initializeProjectile ()
	{
		if (projectileInitialized) {
			return;
		}

		projectileInitialized = true;

		getProjectileRigidbody ();

		if (transform.childCount > 0) {
			getProjectileMeshes ();

			if (bulletMesh != null && currentProjectileInfo.setProjectileMeshRotationToFireRotation) {
				bulletMesh.transform.localEulerAngles = projectileLocalRotation;
			}
		}

		getProjectileTrailRenderer ();

		if (mainTrailRenderer != null) {
			mainTrailRenderer.enabled = true;
		}

		if (currentProjectileInfo.launchProjectile) {
			setUseGravityState (true);
		} else {
			//the bullet moves on the camera direction
			if (!currentProjectileInfo.useRayCastShoot) {
				if (currentProjectileInfo.useGravityOnLaunch) {
					setUseGravityState (true);
				}

				mainRigidbody.velocity = transform.forward * currentProjectileInfo.projectileSpeed;
			}
		}

		if (currentProjectileInfo.projectileParticles != null) {
			GameObject projectileParticles = (GameObject)Instantiate (currentProjectileInfo.projectileParticles, transform.position, transform.rotation);

			projectileParticles.transform.SetParent (transform);
		}

		if (currentProjectileInfo.projectileWithAbility) {
			if (abilityFunctionNameAtStart != "") {
				SendMessage (abilityFunctionNameAtStart, SendMessageOptions.DontRequireReceiver);
			}
		}

		if (currentProjectileInfo.useRaycastCheckingOnRigidbody) {
			getProjectileCollider ();

			previousPosition = Vector3.zero; 

			minimumExtent = Mathf.Min (Mathf.Min (mainCollider.bounds.extents.x, mainCollider.bounds.extents.y), mainCollider.bounds.extents.z); 
			sqrMinimumExtent = minimumExtent * minimumExtent; 
		}
	}

	public void returnBullet (Vector3 direction, GameObject owner, bool ignoreProjectileOwner)
	{
		if (currentProjectileInfo.owner != null && !ignoreProjectileOwner) {
			Transform placeToShoot = applyDamage.getPlaceToShoot (currentProjectileInfo.owner);

			if (placeToShoot != null) {
				currentProjectileInfo.target = placeToShoot.gameObject;

				currentTargetTransform = currentProjectileInfo.target.transform;

				transform.LookAt (currentTargetTransform.position);
			}
		} else {
			transform.LookAt (direction);
		}

		//now the owner of the projectile is the player
		currentProjectileInfo.owner = owner;
		currentProjectileInfo.projectileSpeed = 15;

		if (mainTrailRenderer != null) {
			mainTrailRenderer.enabled = true;
		}

		currentProjectileInfo.useRayCastShoot = false;

		getProjectileCollider ();

		mainCollider.enabled = true;

		projectilePaused = false;

		setProjectileParent (null);

		checkCollisionDetectionMode ();

		setKinematicState (false);

		mainRigidbody.velocity = transform.forward * currentProjectileInfo.projectileSpeed;
	}

	void setProjectileParent (Transform newParent)
	{
		transform.SetParent (newParent);
	}

	public void attachProjectileToSurface ()
	{
		bool surfaceDetected = false;

		if (Physics.Raycast (transform.position - transform.forward * 0.7f, transform.forward, out hit, currentProjectileInfo.scorchRayCastDistance, currentProjectileInfo.targetToDamageLayer)) {
			if (secondaryBulletMesh != null) {
				secondaryBulletMesh.SetActive (true);
			}

			surfaceDetected = true;

		}

		if (!surfaceDetected) {
			Ray ray = new Ray (transform.position, transform.forward);

			if (Physics.SphereCast (ray, 0.3f, out hit, 0.35f, currentProjectileInfo.targetToDamageLayer)) {
				surfaceDetected = true;

//				Debug.DrawLine (transform.position, hit.point + transform.forward * 6, Color.red, 5);
			}

			if (!surfaceDetected) {
				ray = new Ray (transform.position, -transform.up);

				if (Physics.SphereCast (ray, 0.3f, out hit, 0.35f, currentProjectileInfo.targetToDamageLayer)) {
					surfaceDetected = true;

//					Debug.DrawLine (transform.position, hit.point - transform.up * 6, Color.red, 5);
				}
			}

			if (!surfaceDetected) {
				ray = new Ray (transform.position, transform.up);

				if (Physics.SphereCast (ray, 0.3f, out hit, 0.35f, currentProjectileInfo.targetToDamageLayer)) {
					surfaceDetected = true;

//					Debug.DrawLine (transform.position, hit.point + transform.up * 6, Color.red, 5);
				}
			}
		}

		if (surfaceDetected) {

			bool attachedCorrectly = decalManager.placeProjectileInSurface (transform.rotation, transform, hit, objectToDamage, 
				                         currentProjectileInfo.adhereToLimbs, currentProjectileInfo.ignoreSetProjectilePositionOnImpact);

			if (!attachedCorrectly) {
				enableOrDisableProjectileCollider (true);
			} else {
				checkProjectilesParent ();

				enableOrDisableProjectileCollider (false);

				if (useEventOnAdhereToSurface) {
					eventOnAdhereToSurface.Invoke (objectToDamage);
				}
			}
		}
	}

	public void checkEventOnProjectileFiredExternally ()
	{
		if (useEventOnProjectileFiredExternally) {
			eventOnProjectileFiredExternally.Invoke ();
		}
	}

	public bool useEventOnAdhereToSurface;
	public eventParameters.eventToCallWithGameObject eventOnAdhereToSurface;

	public bool useEventOnProjectileFiredExternally;
	public UnityEvent eventOnProjectileFiredExternally;

	public void enableOrDisableProjectileCollider (bool state)
	{
		checkCollisionDetectionMode ();

		setKinematicState (!state);

		setUseGravityState (state);

		if (secondaryBulletMesh != null) {
			enableOrDisableSecondaryMeshCollider (state);
		}
	}

	void setUseGravityState (bool state)
	{
		mainRigidbody.useGravity = state;
	}

	public void checkProjectilesParent ()
	{
		projectileAttachedToSurface = true;

		Transform transformParent = transform.parent;

		if (transformParent != null) {
			mainHealthManagement = transformParent.GetComponent<healthManagement> ();
		}
	}

	public void enableOrDisableSecondaryMeshCollider (bool state)
	{
		if (secondaryBulletMeshCollider != null) {
			secondaryBulletMeshCollider.enabled = state;
		}
	}

	public void projectimeImpactState (GameObject objectToDamage)
	{
		if (useEventOnImpact) {
			if (useLayerMaskImpact) {
				if ((1 << objectToDamage.layer & currentProjectileInfo.targetToDamageLayer.value) == 1 << objectToDamage.layer) {
					callEventOnImpact ();
				}
			} else {
				callEventOnImpact ();
			}
		}
	}

	public void callEventOnImpact ()
	{
		if (sendObjectDetectedOnImpactEvent && objectToDamage != null) {
			objectDetectedOnImpactEvent.Invoke (objectToDamage);
		}

		eventOnImpact.Invoke ();
	}

	public void checkCollisionDetectionMode ()
	{
		if (setContinuousSpeculativeIngame) {
			if (collisionDetectionChecked) {
				return;
			}

			if (mainRigidbody != null) {
				string[] collisionDetectionModeList = Enum.GetNames (typeof(CollisionDetectionMode));

				if (collisionDetectionModeList.Length > 3) {
					for (int i = 0; i < collisionDetectionModeList.Length; i++) {
						if (collisionDetectionModeList [i].Equals ("ContinuousSpeculative")) {
							CollisionDetectionMode currentMode = (CollisionDetectionMode)Enum.GetValues (typeof(CollisionDetectionMode)).GetValue (i);

							if (currentMode != 0) {
								mainRigidbody.collisionDetectionMode = currentMode;
							}

							collisionDetectionChecked = true;

							return;
						}
					}
				}
			}

			collisionDetectionChecked = true;
		}
	}

	public Rigidbody getProjectileRigibody ()
	{
		return mainRigidbody;
	}

	public void setNewProjectileDamage (float newValue)
	{
		currentProjectileInfo.projectileDamage = newValue;
	}

	public void setProjectileDamageMultiplier (float newValue)
	{
		currentProjectileInfo.projectileDamage *= newValue;
	}


	//INITIALIZE PROJECTILE FUNCTIONS
	public void getProjectileComponents (bool calledFromEditor)
	{
		getProjectileMapInformation ();

		getProjectileCollider ();

		getProjectileTrailRenderer ();

		getProjectileRigidbody ();

		getProjectileMeshes ();

		getProjectileBipManager ();

		getMainProjectileImpactSystem ();

		if (calledFromEditor) {
			GKC_Utils.updateComponent (this);
		}
	}

	void getProjectileMeshes ()
	{
		if (transform.childCount > 0) {
			if (bulletMesh == null) {
				bulletMesh = transform.GetChild (0).gameObject;
			}

			if (transform.childCount > 1) {
				if (secondaryBulletMesh == null) {
					secondaryBulletMesh = transform.GetChild (1).gameObject;
				}

				if (secondaryBulletMeshCollider == null) {
					secondaryBulletMeshCollider = secondaryBulletMesh.GetComponentInChildren<MeshCollider> ();
				}
			}
		}
	}

	void getProjectileCollider ()
	{
		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}

		if (mainCollider == null) {
			mainCollider = GetComponentInChildren<Collider> ();
		}
	}

	void getProjectileRigidbody ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}
	}

	void getProjectileMapInformation ()
	{
		if (mainMapObjectInformation == null) {
			mainMapObjectInformation = GetComponent<mapObjectInformation> ();
		}
	}

	void getProjectileTrailRenderer ()
	{
		if (mainTrailRenderer == null) {
			mainTrailRenderer = GetComponent<TrailRenderer> ();
		}
	}

	void getProjectileBipManager ()
	{
		if (audioClipBipManager == null) {
			audioClipBipManager = GetComponentInChildren<audioClipBip> ();
		}
	}

	void getMainProjectileImpactSystem ()
	{
		if (mainProjectileImpactSystem == null) {
			mainProjectileImpactSystem = GetComponentInChildren<projectileImpactSystem> ();
		}
	}

	public virtual void resetProjectile ()
	{
		if (bulletMesh != null && disableProjectileMeshOnImpact) {
			if (!bulletMesh.activeSelf) {
				bulletMesh.SetActive (true);
			}
		}

		if (secondaryBulletMesh != null) {
			if (secondaryBulletMesh.activeSelf) {
				secondaryBulletMesh.SetActive (false);
			}
		}

		projectileInitialized = false;

		currentProjectileInfo = null;

		objectToDamage = null;

		hit = new RaycastHit ();

		setProjectileUsedState (false);

		projectilePaused = false;

		explosionActivated = false;

		rotationSpeed = 10;

		projectileCaptured = false;

		impactParticlesCreated = false;

		mainHealthManagement = null;

		projectileAttachedToSurface = false;

		currentNumberOfImpacts = 0;

		if (listOfGameObjectImpacts.Count > 0) {
			listOfGameObjectImpacts.Clear ();
		}

		if (listOfColliderImpacts.Count > 0) {
			listOfColliderImpacts.Clear ();
		}

		orbitingTargetTime = 0;

		if (surfaceCoroutine != null) {
			StopCoroutine (surfaceCoroutine);
		}

		loop = 0;

		collisionDetectionChecked = false;

		pauseCheckTriggerActive = false;

		projectileRemovedPreviously = false;

		previousPosition = Vector3.zero;

		currentTargetTransform = null;

		if (mainTrailRenderer != null) {
			mainTrailRenderer.Clear ();
		}
	}

	public void checkToResetProjectile ()
	{
		if (projectileRemovedPreviously) {
			resetProjectile ();

			projectileRemovedPreviously = false;

			createMapIconInfo ();
		} else {
			projectileCaptured = false;
		}
	}

	public virtual void setProjectileSpecialActionActiveState (bool state)
	{

	}
}