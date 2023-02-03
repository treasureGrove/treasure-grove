using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class arrowProjectile : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool rotateArrowWithGravity = true;
	public float gravityForceRotationSpeed = 5;
	public float extraRotationAmount;

	public string movingObjectsTag = "moving";

	public bool useGravityOnArrow = true;

	public float gravityForceDownAmount = 5;

	public float adjustToSurfaceSpeed = 4;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnArrowImpact;
	public UnityEvent eventOnArrowImpact;

	public UnityEvent eventToDropArrowPickupOnBounce;

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public projectileSystem mainProjectileSystem;
	public Rigidbody mainRigidbody;
	public spawnObject mainSpawnObject;

	Quaternion targetRotation;

	bool applyArrowForces = true;

	bool checkSurfaceTypeDetected;

	List<bowSystem.arrowSurfaceTypeInfo> arrowSurfaceTypeInfoList = new List<bowSystem.arrowSurfaceTypeInfo> ();

	Coroutine adjustArrowCoroutine;

	float currentArrowDownForce = 1;


	void OnEnable ()
	{
		resetProjectile ();
	}

	void FixedUpdate ()
	{
		if (!mainProjectileSystem.isProjectileUsed ()) {
			if (showDebugPrint) {
				print ("searching");
			}

			if (applyArrowForces) {
				if (mainProjectileSystem.currentProjectileInfo.isSeeker || mainProjectileSystem.currentProjectileInfo.isHommingProjectile) {
					applyArrowForces = false;
				}
			}

			if (applyArrowForces) {
				if (!useGravityOnArrow && gravityForceDownAmount > 0) {
					mainRigidbody.AddForce (-Vector3.up * (gravityForceDownAmount * currentArrowDownForce * mainRigidbody.mass));
				}

				if (rotateArrowWithGravity) {
					if (gravityForceRotationSpeed > 0) {
						if (!mainRigidbody.isKinematic) {
							targetRotation = Quaternion.LookRotation (mainRigidbody.velocity - Vector3.up * extraRotationAmount);

							transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * gravityForceRotationSpeed * currentArrowDownForce);
						}
					}
				}
			}
		}

		if (mainProjectileSystem.disableProjectileMeshOnImpact) {
			mainProjectileSystem.disableProjectileMeshOnImpact = false;
		}
	}

	//when the bullet touchs a surface, then
	public void checkObjectDetected (GameObject objectToDamage)
	{
		if (showDebugPrint) {
			print ("detected " + objectToDamage.name);
		}

		mainProjectileSystem.setProjectileUsedState (true);

		mainProjectileSystem.projectilePaused = true;

		//set the bullet kinematic

		mainRigidbody.isKinematic = true;

		Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

		bool isAttachedToCharacter = false;

		bool surfaceDetectedIsDead = false;

		if (applyDamage.objectCanBeDamaged (objectToDamage)) {
			isAttachedToCharacter = applyDamage.attachObjectToSurfaceFound (objectToDamage.transform, transform, transform.position, true);

			surfaceDetectedIsDead = applyDamage.checkIfDead (objectToDamage);
		}
				
		mainRigidbody.useGravity = false;
		mainRigidbody.isKinematic = true;

		if (!surfaceDetectedIsDead && !isAttachedToCharacter && (objectToDamage.CompareTag (movingObjectsTag) || objectToDamageRigidbody != null)) {
			applyDamage.checkParentToAssign (transform, objectToDamage.transform);
		}

		mainProjectileSystem.checkProjectilesParent ();

		if (useEventOnArrowImpact) {
			eventOnArrowImpact.Invoke ();
		}

		if (checkSurfaceTypeDetected) {
			meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = objectToDamage.GetComponent<meleeAttackSurfaceInfo> ();

			if (currentMeleeAttackSurfaceInfo != null) {
				bool surfaceLocated = false;

				string surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

				for (int i = 0; i < arrowSurfaceTypeInfoList.Count; i++) {
					if (!surfaceLocated && arrowSurfaceTypeInfoList [i].Name.Equals (surfaceName)) {
						surfaceLocated = true;

						if (arrowSurfaceTypeInfoList [i].isObstacle) {
							if (arrowSurfaceTypeInfoList [i].arrowBounceOnSurface) {
								if (arrowSurfaceTypeInfoList [i].dropArrowPickupOnBounce) {
									gameObject.SetActive (false);

									eventToDropArrowPickupOnBounce.Invoke ();

									if (arrowSurfaceTypeInfoList [i].addExtraForceOnBounce) {
										GameObject arrowPickup = mainSpawnObject.getLastObjectSpawned ();

										if (arrowPickup != null) {
											Rigidbody mainArrowRigidbody = arrowPickup.GetComponent<Rigidbody> ();

											if (mainArrowRigidbody != null) {
												mainArrowRigidbody.AddForce (-mainArrowRigidbody.transform.forward * arrowSurfaceTypeInfoList [i].extraForceOnBounce, ForceMode.Impulse);

												float randomRightDirection = Random.Range (0, 1);

												if (randomRightDirection == 0) {
													randomRightDirection = -1;
												}

												mainArrowRigidbody.AddTorque (mainRigidbody.transform.right * 10 * randomRightDirection, ForceMode.Impulse);

												mainArrowRigidbody.AddTorque (mainRigidbody.transform.up * 5 * (-1 * randomRightDirection), ForceMode.Impulse);
											}
										}
									}
								} else {
									mainRigidbody.useGravity = true;
									mainRigidbody.isKinematic = false;

									mainProjectileSystem.enableOrDisableSecondaryMeshCollider (true);
								}
							}
						}
					}
				}
			}
		}

		playerComponentsManager mainPlayerComponentsManager = objectToDamage.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			projectilesOnCharacterBodyManager mainProjectilesOnCharacterBodyManager = mainPlayerComponentsManager.getProjectilesOnCharacterBodyManager ();

			if (mainProjectilesOnCharacterBodyManager != null) {
				mainProjectilesOnCharacterBodyManager.addProjectileToCharacterBody (gameObject);
			}
		} else {
			projectilesOnCharacterBodyManager mainProjectilesOnCharacterBodyManager = objectToDamage.GetComponent<projectilesOnCharacterBodyManager> ();

			if (mainProjectilesOnCharacterBodyManager != null) {
				mainProjectilesOnCharacterBodyManager.addProjectileToCharacterBody (gameObject);
			}
		}

		if (isAttachedToCharacter) {
			stopAdjustArrowToSurfaceDetectedCoroutine ();

			adjustArrowCoroutine = StartCoroutine (adjustArrowToSurfaceDetectedCoroutine ());
		}

		mainProjectileSystem.currentProjectileInfo.isSeeker = false;
		mainProjectileSystem.currentProjectileInfo.isHommingProjectile = false;
	}

	public void resetProjectile ()
	{
		applyArrowForces = true;

		stopAdjustArrowToSurfaceDetectedCoroutine ();

		transform.SetParent (null);

		currentArrowDownForce = 1;
	}

	public void setNewArrowDownForce (float newValue)
	{
		currentArrowDownForce = newValue;
	}

	public void setArrowSurfaceTypeInfoList (List<bowSystem.arrowSurfaceTypeInfo> newArrowSurfaceTypeInfoList)
	{
		arrowSurfaceTypeInfoList = newArrowSurfaceTypeInfoList;

		checkSurfaceTypeDetected = true;
	}

	public void disableCheckSurfaceTypeDetected ()
	{
		checkSurfaceTypeDetected = false;
	}

	void stopAdjustArrowToSurfaceDetectedCoroutine ()
	{
		if (adjustArrowCoroutine != null) {
			StopCoroutine (adjustArrowCoroutine);
		}
	}

	IEnumerator adjustArrowToSurfaceDetectedCoroutine ()
	{
		bool targetReached = false;

		float t = 0;

		float positionDifference = 0;

		float movementTimer = 0;

		Vector3 targetPosition = -transform.parent.forward * 0.15f;

		while (!targetReached) {
			t += Time.deltaTime / adjustToSurfaceSpeed; 

			transform.localPosition = Vector3.Lerp (transform.localPosition, targetPosition, t);

			positionDifference = GKC_Utils.distance (transform.localPosition, targetPosition);

			movementTimer += Time.deltaTime;

			if (positionDifference < 0.01f || movementTimer > 3) {
				targetReached = true;

				if (showDebugPrint) {
					print ("target reached " + targetPosition);
				}
			}

			yield return null;
		}
	}
}
