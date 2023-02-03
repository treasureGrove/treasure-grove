using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITurret : MonoBehaviour
{
	[Header ("Turret Settings")]
	[Space]

	public LayerMask layer;
	public LayerMask layerForGravity;
	public LayerMask layerToDamage;
	public GameObject regularProjectile;
	public GameObject hommingMissile;
	public GameObject bulletShell;
	public weaponType currentWeapon;
	public float rotationSpeed = 10;
	public Shader transparent;
	public AudioClip locatedEnemy;
	public AudioClip machineGunSound;
	public AudioClip laserSound;
	public AudioClip cannonSound;
	public bool randomWeaponAtStart;

	public float inputRotationSpeed = 5;

	public Vector2 rangeAngleX;
	public float overrideRotationSpeed = 10;

	public float raycastPositionRotationSpeed = 10;

	public Transform overridePositionToLook;

	[Space]
	[Header ("Weapons Settings")]
	[Space]

	public string machineGunAnimationName = "activateMachineGun";
	public string cannonAnimationName = "activateCannon";

	public bool projectilesPoolEnabled = true;

	public int maxAmountOfPoolElementsOnWeapon = 30;

	public float missileFireRate = 0.7f;
	public float machineGunFireRate = 0.1f;

	[Space]
	[Header ("Shells Settings")]
	[Space]

	public bool removeDroppedShellsAfterTime = true;
	public int maxAmountOfShellsBeforeRemoveThem = 30;
	public float shellEjectionForce = 100;
	public List<AudioClip> shellDropSoundList = new List<AudioClip> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool dead;
	public bool shootingWeapons;
	public bool weaponsActive;

	public Transform currentCameraTransformDirection;

	[Space]
	[Header ("Override Elements")]
	[Space]

	public bool controlOverriden;

	public bool useCustomInputCameraDirection;

	[Space]
	[Header ("Turret Elements")]
	[Space]

	public GameObject rotatingBase;
	public GameObject head;
	public GameObject rayCastPosition;
	public GameObject laserShootPosition;
	public GameObject laserBeam;

	public GameObject cannon;
	public GameObject cannonShootPosition;
	public GameObject cannonShellPosition;
	public GameObject aimCannon;

	public GameObject machineGun;
	public GameObject machineGunShootPosition;
	public GameObject machineGunShellPosition;
	public GameObject aimMachineGun;

	public GameObject rotor;

	public manageAITarget targetManager;
	public Rigidbody mainRigidbody;

	public Animation machineGunAnim;
	public Animation cannonAnim;
	public AudioSource audioSource;
	public overrideInputManager overrideInput;

	public GameObject turretAttacker;

	Quaternion currentRaycastPositionRotation;

	Vector2 currentLookAngle;
	Vector2 axisValues;
	float horizontalInput;
	float verticalInput;

	GameObject shootPosition;

	GameObject shellPosition;

	RaycastHit hit;
	int typeWeaponChoosed;

	List<Renderer> rendererParts = new List<Renderer> ();
	bool cannonDeployed;
	bool machineGunDeployed;
	bool kinematicActive;
	float timer;
	float shootTimerLimit;

	bool shellsOnSceneToRemove;
	bool shellsActive;
	float destroyShellsTimer = 0;

	float orignalRotationSpeed;
	float speedMultiplier = 1;

	GameObject currentEnemyToShoot;

	float currentRotationSpeed;

	float lastTimeWeaponsActivated;

	string untaggedName = "Untagged";

	List<GameObject> shells = new List<GameObject> ();

	GameObject newProjectileGameObject;
	projectileSystem currentProjectileSystem;
	Rigidbody currentProjectileRigidbody;

	GameObject newShellClone;
	weaponShellSystem newWeaponShellSystem;
	AudioClip newClipToShell;
	GameObject currentShellToRemove;

	Coroutine disableOverrideCoroutine;

	public enum weaponType
	{
		//type of current weapon that the turret is using, you can change it in run time
		cannon = 0,
		laser = 1,
		machineGun = 2
	}

	void Start ()
	{
		if (turretAttacker == null) {
			turretAttacker = gameObject;
		}

		//set the selected weapon
		setWeapon ();

		//set the parameters of the turret laser
		enemyLaser currentEnemyLaser = laserBeam.GetComponent<enemyLaser> ();

		if (currentEnemyLaser != null) {
			currentEnemyLaser.setOwner (turretAttacker);
		}

		if (laserBeam.activeSelf) {
			laserBeam.SetActive (false);
		}

		currentRotationSpeed = rotationSpeed;
		orignalRotationSpeed = rotationSpeed;

		if (aimMachineGun.activeSelf) {
			aimMachineGun.SetActive (false);
		}

		if (aimCannon.activeSelf) {
			aimCannon.SetActive (false);
		}

		if (randomWeaponAtStart) {
			setRandomWeapon ();
		}
	}

	void Update ()
	{
		if (dead) {
			//if the turrets is destroyed, set it to transparent smoothly to disable it from the scene
			for (int i = 0; i < rendererParts.Count; i++) {
				
				Color alpha = rendererParts [i].material.color;

				alpha.a -= Time.deltaTime / 5;

				rendererParts [i].material.color = alpha;

				if (alpha.a <= 0) {
					Destroy (gameObject);
				}
			}
		}

		//if the turret is not destroyed, or being hacked, or paused by a black hole, then
		if (!dead && !targetManager.paused) {
			if (targetManager.onSpotted) {
				lootAtTurretTarget (targetManager.placeToShoot);

				shootWeapon (targetManager.enemyToShoot, targetManager.placeToShoot, true);

				//if the laser is selected, activate it
				if (typeWeaponChoosed == 1) {
					if (!laserBeam.activeSelf) {
						laserBeam.SetActive (true);
					}
				}

				//if in run time the weapon is changed, set the new weapon
				if (typeWeaponChoosed != (int)currentWeapon) {
					activateWeapon ();
				}
			}
			//if there are no enemies in the field of view, rotate in Y local axis to check new targets
			else if (!targetManager.checkingThreat) {
				rotatingBase.transform.Rotate (0, currentRotationSpeed * Time.deltaTime * 3, 0);
			}

			//disable the machine gun and the cannon renderers while they are not used
			if (!machineGunAnim.IsPlaying (machineGunAnimationName) && !cannonAnim.IsPlaying (cannonAnimationName) && !targetManager.onSpotted) {
				if (aimMachineGun.activeSelf) {
					aimMachineGun.SetActive (false);
				}

				if (aimCannon.activeSelf) {
					aimCannon.SetActive (false);
				}
			}
		}

		//remove the shells of the bullet when the turret is not shooting
		checkDroppedShellsRemoval ();

		//if the turret has been hacked, the player can grab it, so when he drops it, the turret will be set in the first surface that will touch
		//also checking if the gravity of the turret has been modified
		if (tag.Equals (untaggedName)) {
			if (!mainRigidbody.isKinematic && !mainRigidbody.freezeRotation) {
				mainRigidbody.freezeRotation = true;

				StartCoroutine (rotateElement (gameObject));
			}
		} else {
			if (mainRigidbody.freezeRotation) {
				mainRigidbody.freezeRotation = false;
				kinematicActive = true;
			}
		}

		//when the kinematicActive has been enabled, the turret has a regular gravity again, so the first ground surface that will find, will be its new ground
		//enabling the kinematic rigidbody of the turret
		if (kinematicActive) {
			if (Physics.Raycast (transform.position, -Vector3.up, out hit, 1.2f, layerForGravity)) {
				if (!mainRigidbody.isKinematic && kinematicActive && !GetComponent<artificialObjectGravity> () && !hit.collider.isTrigger) {
					StartCoroutine (rotateToSurface (hit));
				}
			}
		}

		if (controlOverriden) {

			if (shootingWeapons) {
				if (Physics.Raycast (rayCastPosition.transform.position, rayCastPosition.transform.forward, out hit, Mathf.Infinity, layer)) {
					currentEnemyToShoot = hit.collider.gameObject;
				} else {
					currentEnemyToShoot = null;
				}

				shootWeapon (currentEnemyToShoot, overridePositionToLook.transform, false);
			}

			if (useCustomInputCameraDirection) {
				currentRaycastPositionRotation = Quaternion.LookRotation (currentCameraTransformDirection.forward);
			} else {
				axisValues = overrideInput.getCustomMovementAxis ();
				horizontalInput = axisValues.x;
				verticalInput = axisValues.y;

				currentLookAngle.x += horizontalInput * inputRotationSpeed;
				currentLookAngle.y -= verticalInput * inputRotationSpeed;

				currentLookAngle.y = Mathf.Clamp (currentLookAngle.y, rangeAngleX.x, rangeAngleX.y);

				currentRaycastPositionRotation = Quaternion.Euler (0, currentLookAngle.x, 0);

				currentRaycastPositionRotation *= Quaternion.Euler (currentLookAngle.y, 0, 0);
			}

			rayCastPosition.transform.rotation = Quaternion.Slerp (rayCastPosition.transform.rotation, currentRaycastPositionRotation, Time.deltaTime * raycastPositionRotationSpeed);

			lootAtTurretTarget (overridePositionToLook);
		}
	}

	public void checkDroppedShellsRemoval ()
	{
		//if the amount of shells from the projectiles is higher than 0, check the time to remove then
		if (shellsOnSceneToRemove && removeDroppedShellsAfterTime) {
			if (shellsActive) {
				destroyShellsTimer += Time.deltaTime;

				if (destroyShellsTimer > 3) {
					for (int i = 0; i < shells.Count; i++) {
						if (projectilesPoolEnabled) {
							GKC_PoolingSystem.Despawn (shells [i]);
						} else {
							Destroy (shells [i]);
						}
					}

					shells.Clear ();

					destroyShellsTimer = 0;

					shellsOnSceneToRemove = false;

					shellsActive = false;
				}
			}
		}
	}

	public void chooseNextWeapon ()
	{
		typeWeaponChoosed++;

		if (typeWeaponChoosed > 2) {
			typeWeaponChoosed = 0;
		}

		if (typeWeaponChoosed != (int)currentWeapon) {
			setWeapon (typeWeaponChoosed);
			activateWeapon ();
		}
	}

	public void choosePreviousWeapon ()
	{
		typeWeaponChoosed--;

		if (typeWeaponChoosed < 0) {
			typeWeaponChoosed = 2;
		}

		if (typeWeaponChoosed != (int)currentWeapon) {
			setWeapon (typeWeaponChoosed);
			activateWeapon ();
		}
	}

	public void enableOrDisableWeapons (bool state)
	{
		weaponsActive = state;

		if (weaponsActive) {
			if (typeWeaponChoosed == 1) {
				if (!laserBeam.activeSelf) {
					laserBeam.SetActive (true);
				}
			}

			activateWeapon ();
		} else {
			deactivateWeapon ();
		}
	}

	void cancelCheckSuspectTurret ()
	{
		setOnSpottedState (false);
	}

	public void setOnSpottedState (bool state)
	{
		if (state) {
			activateWeapon ();
		} else {
			StartCoroutine (rotateElement (head));
			deactivateWeapon ();
		}
	}
		
	//active the fire mode
	void shootWeapon (GameObject enemyToShoot, Transform placeToShoot, bool checkTargetOnRaycast)
	{
		//if the current weapon is the machine gun or the cannon, check with a ray if the player is in front of the turret
		//if the cannon is selected, the time to shoot is 1 second, the machine gun shoots every 0.1 seconds
		if ((typeWeaponChoosed == 0 || typeWeaponChoosed == 2) && !machineGunAnim.IsPlaying (machineGunAnimationName) && !cannonAnim.IsPlaying (cannonAnimationName)) {
			if (checkTargetOnRaycast) {
				if (Physics.Raycast (rayCastPosition.transform.position, rayCastPosition.transform.forward, out hit, Mathf.Infinity, layer)) {
					Debug.DrawLine (rayCastPosition.transform.position, hit.point, Color.red, 200, true);

					if (hit.collider.gameObject == enemyToShoot || hit.collider.gameObject.transform.IsChildOf (enemyToShoot.transform)) {
						timer += Time.deltaTime * speedMultiplier;
					}
				}
			} else {
				timer += Time.deltaTime * speedMultiplier;
			}
		}

		//check the current weapon selected, to aim it in the direction of the closest enemy
		if (typeWeaponChoosed == 0) {
			Vector3 targetDir = placeToShoot.position - aimCannon.transform.position;

			Quaternion qTo = Quaternion.LookRotation (targetDir);

			aimCannon.transform.rotation = Quaternion.Slerp (aimCannon.transform.rotation, qTo, currentRotationSpeed * Time.deltaTime);
		}

		if (typeWeaponChoosed == 2) {
			Vector3 targetDir = placeToShoot.position - aimMachineGun.transform.position;

			Quaternion qTo = Quaternion.LookRotation (targetDir);

			aimMachineGun.transform.rotation = Quaternion.Slerp (aimMachineGun.transform.rotation, qTo, currentRotationSpeed * Time.deltaTime);

			rotor.transform.Rotate (0, 0, 800 * Time.deltaTime * speedMultiplier);
		}

		//if the timer ends, shoot
		if (timer >= shootTimerLimit) {
			timer = 0;

			destroyShellsTimer = 0;

			GameObject currentProjectilePrefab = regularProjectile;

			if (typeWeaponChoosed == 0) {
				aimCannon.GetComponent<Animation> ().Play ("cannonRecoil");
				audioSource.Play ();
				currentProjectilePrefab = hommingMissile;
			} 

			if (typeWeaponChoosed == 2) {
				audioSource.Play ();
			}

			//create the projectile in the position of the current weapon
			if (projectilesPoolEnabled) {
				newProjectileGameObject = GKC_PoolingSystem.Spawn (currentProjectilePrefab, shootPosition.transform.position, shootPosition.transform.rotation, maxAmountOfPoolElementsOnWeapon);
			} else {
				newProjectileGameObject = (GameObject)Instantiate (currentProjectilePrefab, shootPosition.transform.position, shootPosition.transform.rotation);
			}

			if (controlOverriden) {
				if (currentCameraTransformDirection != null) {
					bool surfaceFound = false;

					Vector3 raycastDirection = currentCameraTransformDirection.forward;

					if (Physics.Raycast (currentCameraTransformDirection.position, raycastDirection, out hit, Mathf.Infinity, layer)) {
						
						if (hit.collider.gameObject != turretAttacker) {
							surfaceFound = true;
						} else {
							Vector3 raycastPosition = hit.point + raycastDirection * 0.2f;

							if (Physics.Raycast (raycastPosition, raycastDirection, out hit, Mathf.Infinity, layer)) {
								surfaceFound = true;
							}
						}
					}

					if (surfaceFound) {
						newProjectileGameObject.transform.LookAt (hit.point);
					}
				}
			}

			currentProjectileSystem = newProjectileGameObject.GetComponent<projectileSystem> ();

			currentProjectileSystem.checkToResetProjectile ();

			//configure the fired projectile
			currentProjectileSystem.setProjectileOnwer (turretAttacker);

			currentProjectileSystem.setTargetToDamageLayer (layerToDamage);

			currentProjectileSystem.setEnemy (enemyToShoot);

			currentProjectileSystem.initializeProjectile ();

			//create the shell bullet
			if (projectilesPoolEnabled) {
				newShellClone = GKC_PoolingSystem.Spawn (bulletShell, shellPosition.transform.position, shellPosition.transform.rotation, maxAmountOfPoolElementsOnWeapon);
			} else {
				newShellClone = (GameObject)Instantiate (bulletShell, shellPosition.transform.position, shellPosition.transform.rotation);
			}

			newWeaponShellSystem = newShellClone.GetComponent<weaponShellSystem> ();

			if (shellDropSoundList.Count > 0) {
				newClipToShell = shellDropSoundList [Random.Range (0, shellDropSoundList.Count - 1)];
			}

			newWeaponShellSystem.setShellValues (shellPosition.transform.forward * (shellEjectionForce * GKC_Utils.getCurrentScaleTime ()), null, newClipToShell);

			if (removeDroppedShellsAfterTime) {
				shells.Add (newShellClone);

				if (shells.Count > maxAmountOfShellsBeforeRemoveThem) {
					currentShellToRemove = shells [0];

					shells.RemoveAt (0);

					if (projectilesPoolEnabled) {
						GKC_PoolingSystem.Despawn (currentShellToRemove);
					} else {
						Destroy (currentShellToRemove);
					}
				}

				shellsActive = true;

				shellsOnSceneToRemove = true;
			}
		}
	}

	//follow the enemy position, to rotate torwards his direction
	void lootAtTurretTarget (Transform objective)
	{
		if (objective != null) {
			//there are two parts in the turret that move, the head and the middle body
			Vector3 targetDir = objective.position - rotatingBase.transform.position;

			targetDir = targetDir - transform.up * transform.InverseTransformDirection (targetDir).y;

			targetDir = targetDir.normalized;

			Quaternion targetRotation = Quaternion.LookRotation (targetDir, transform.up);

			rotatingBase.transform.rotation = Quaternion.Slerp (rotatingBase.transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);

			Vector3 targetDir2 = objective.position - head.transform.position;

			Quaternion targetRotation2 = Quaternion.LookRotation (targetDir2, transform.up);

			head.transform.rotation = Quaternion.Slerp (head.transform.rotation, targetRotation2, currentRotationSpeed * Time.deltaTime);	
		}
	}

	//the gravity of the turret is regular again
	void dropCharacter (bool state)
	{
		kinematicActive = state;
	}

	//when the turret detects a ground surface, will rotate according to the surface normal
	IEnumerator rotateToSurface (RaycastHit hit)
	{
		//it works like the player gravity
		kinematicActive = false;

		mainRigidbody.useGravity = true;
		mainRigidbody.isKinematic = true;

		Quaternion rot = transform.rotation;
		Vector3 myForward = Vector3.Cross (transform.right, hit.normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);
		Vector3 pos = hit.point;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;

			transform.rotation = Quaternion.Slerp (rot, dstRot, t);
			//set also the position of the turret to the hit point

			transform.position = Vector3.MoveTowards (transform.position, pos + transform.up * 0.5f, t);

			yield return null;
		}

		gameObject.layer = 0;
	}

	//return the head of the turret to its original rotation
	IEnumerator rotateElement (GameObject element)
	{
		Quaternion rot = element.transform.localRotation;

		Vector3 myForward = Vector3.Cross (element.transform.right, Vector3.up);
		Quaternion dstRot = Quaternion.LookRotation (myForward, Vector3.up);

		dstRot.y = 0;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3 * speedMultiplier;

			element.transform.localRotation = Quaternion.Slerp (rot, dstRot, t);

			yield return null;
		}
	}

	//if one enemy or more are inside of the turret's trigger, activate the weapon selected in the inspector: machine gun, laser or cannon
	void activateWeapon ()
	{
		if (locatedEnemy != null) {
			audioSource.PlayOneShot (locatedEnemy, Random.Range (0.8f, 1.2f));
		}

		typeWeaponChoosed = (int)currentWeapon;

		//cannon
		if (typeWeaponChoosed == 0) {
			aimCannon.SetActive (true);

			setWeapon ();

			//the turret has an animation to activate and deactivate the machinge gun and the cannon
			//also if the machine gun is activated, and the cannon was activate before, this is disabled
			if (machineGunDeployed) {
				machineGunAnim [machineGunAnimationName].speed = -1; 

				machineGunAnim [machineGunAnimationName].time = machineGunAnim [machineGunAnimationName].length;

				machineGunAnim.Play (machineGunAnimationName);

				machineGunDeployed = false;
			}

			laserBeam.SetActive (false);

			cannonAnim [cannonAnimationName].speed = 1; 

			cannonAnim.Play (cannonAnimationName);

			cannonDeployed = true;

			audioSource.clip = cannonSound;
		}

		//laser
		if (typeWeaponChoosed == 1) {
			setWeapon ();

			deactivateWeapon ();

			laserBeam.SetActive (true);

			audioSource.clip = laserSound;
			audioSource.loop = true;
			audioSource.Play ();
		}

		//machine gun
		if (typeWeaponChoosed == 2) {
			aimMachineGun.SetActive (true);

			setWeapon ();

			if (cannonDeployed) {
				cannonAnim [cannonAnimationName].speed = -1; 
				cannonAnim [cannonAnimationName].time = cannonAnim [cannonAnimationName].length;
				cannonAnim.Play (cannonAnimationName);

				cannonDeployed = false;
			}

			laserBeam.SetActive (false);

			machineGunAnim [machineGunAnimationName].speed = 1; 
			machineGunAnim.Play (machineGunAnimationName);

			machineGunDeployed = true;
			audioSource.clip = machineGunSound;
		}
	}

	//if all the enemies in the trigger of the turret are gone, deactivate the weapons
	void deactivateWeapon ()
	{
		audioSource.loop = false;

		if (cannonDeployed) {
			cannonAnim [cannonAnimationName].speed = -1; 
			cannonAnim [cannonAnimationName].time = cannonAnim [cannonAnimationName].length;
			cannonAnim.Play (cannonAnimationName);
			cannonDeployed = false;
		}

		if (machineGunDeployed) {
			machineGunAnim [machineGunAnimationName].speed = -1; 
			machineGunAnim [machineGunAnimationName].time = machineGunAnim [machineGunAnimationName].length;
			machineGunAnim.Play (machineGunAnimationName);
			machineGunDeployed = false;
		}

		laserBeam.SetActive (false);
	}

	//at the start, set the rate of shooting, the position where the bullet are shooted and the position where the bullet shells are released
	void setWeapon ()
	{
		typeWeaponChoosed = (int)currentWeapon;

		if (typeWeaponChoosed == 0) {
			shootTimerLimit = missileFireRate;
			shootPosition = cannonShootPosition;
			shellPosition = cannonShellPosition;
		} else if (typeWeaponChoosed == 1) {
			shootPosition = laserShootPosition;
		} else {
			shootTimerLimit = machineGunFireRate;
			shootPosition = machineGunShootPosition;
			shellPosition = machineGunShellPosition;
		}
	}

	//the turret is destroyed, so disable all the triggers, the AI, and add a rigidbody to every object with a render, and add force to them
	public void setDeathState ()
	{
		audioSource.loop = false;

		dead = true;

		laserBeam.SetActive (false);

		Component[] components = GetComponentsInChildren (typeof(Transform));

		int layerToIgnoreIndex = LayerMask.NameToLayer ("Scanner");

		int ignoreRaycastLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

		foreach (Component c in components) {
			Renderer currentRenderer = c.GetComponent<Renderer> ();

			if (currentRenderer != null && c.gameObject.layer != layerToIgnoreIndex) {
				rendererParts.Add (currentRenderer);

				currentRenderer.material.shader = transparent;
				c.transform.SetParent (transform);

				c.gameObject.layer = ignoreRaycastLayerIndex;

				Rigidbody currentRigidbody = c.gameObject.GetComponent<Rigidbody> ();

				if (currentRigidbody == null) {
					currentRigidbody = c.gameObject.AddComponent<Rigidbody> ();
				}

				c.gameObject.AddComponent<BoxCollider> ();

				currentRigidbody.AddExplosionForce (500, transform.position + transform.up, 50, 3);
			} else {
				Collider currentCollider = c.gameObject.GetComponent<Collider> ();

				if (currentCollider != null) {
					currentCollider.enabled = false;
				}
			}
		}
	}

	//if the player uses the power of slow down, reduces the rotation speed of the turret, the rate fire and the projectile velocity
	void setReducedVelocity (float speedMultiplierValue)
	{
		currentRotationSpeed = speedMultiplierValue;

		speedMultiplier = speedMultiplierValue;
	}

	//set the turret speed to its normal state
	void setNormalVelocity ()
	{
		currentRotationSpeed = orignalRotationSpeed;

		speedMultiplier = 1;
	}

	public void setRandomWeapon ()
	{
		int random = Random.Range (0, 2);

		setWeapon (random);
	}

	public void setWeapon (int weaponNumber)
	{
		switch (weaponNumber) {
		case 0:
			currentWeapon = weaponType.cannon;
			break;
		case 1:
			currentWeapon = weaponType.laser;
			break;
		case 2:
			currentWeapon = weaponType.machineGun;
			break;
		}

		setWeapon ();
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		if (state) {
			currentLookAngle = new Vector2 (rayCastPosition.transform.eulerAngles.y, rayCastPosition.transform.eulerAngles.x);
		} else {
			currentLookAngle = Vector2.zero;
			axisValues = Vector2.zero;
			shootingWeapons = false;
		}

		controlOverriden = state;

		targetManager.pauseAI (controlOverriden);

		if (controlOverriden) {
			currentRotationSpeed = overrideRotationSpeed;
		} else {
			currentRotationSpeed = orignalRotationSpeed;

			StartCoroutine (rotateElement (head));

			deactivateWeapon ();
		}

		stopDisableOverrideAfterDelayCoroutine ();
	}

	public void disableOverrideAfterDelay (float delayDuration)
	{
		if (gameObject.activeSelf) {
			stopDisableOverrideAfterDelayCoroutine ();

			disableOverrideCoroutine = StartCoroutine (updatedDisableOverrideAfterDelayCoroutine (delayDuration));
		}
	}

	public void stopDisableOverrideAfterDelayCoroutine ()
	{
		if (disableOverrideCoroutine != null) {
			StopCoroutine (disableOverrideCoroutine);
		}
	}

	IEnumerator updatedDisableOverrideAfterDelayCoroutine (float delayDuration)
	{
		yield return new WaitForSecondsRealtime (delayDuration);

		stopOverride ();
	}

	public void setNewCurrentCameraTransformDirection (Transform newTransform)
	{
		currentCameraTransformDirection = newTransform;
	}

	public void setNewTurretAttacker (GameObject newAttacker)
	{
		turretAttacker = newAttacker;
	}

	//INPUT FUNCTIONS
	public void inputSetShootState (bool state)
	{
		if (weaponsActive) {
			if (Time.time > lastTimeWeaponsActivated + 0.6f) {
				shootingWeapons = state;
			}
		}
	}

	public void inputSetWeaponsState ()
	{
		enableOrDisableWeapons (!weaponsActive);

		if (weaponsActive) {
			lastTimeWeaponsActivated = Time.time;
		}
	}

	public void inputSetNextOrPreviousWeapon (bool state)
	{
		if (state) {
			chooseNextWeapon ();
		} else {
			choosePreviousWeapon ();
		}
	}
}