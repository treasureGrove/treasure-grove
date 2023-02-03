using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[SerializeField]
public class vehicleWeaponSystem : MonoBehaviour
{
	public bool weaponsEnabled = true;

	public bool weaponsActivate;
	public AudioSource weaponsEffectsSource;
	public AudioClip outOfAmmo;

	public LayerMask layer;
	public LayerMask targetForScorchLayer;

	public bool useCustomIgnoreTags;
	public List<string> customTagsToIgnoreList = new List<string> ();

	public float minimumX = 25;
	public float maximumX = 315;
	public float minimumY = 360;
	public float maximumY = 360;
	public bool hasBaseRotation = true;

	public GameObject mainVehicleWeaponsGameObject;

	public GameObject baseX;
	public GameObject baseY;
	public Transform weaponLookDirection;
	public int weaponsSlotsAmount;

	public float minSwipeDist = 20;


	public bool projectilesPoolEnabled = true;

	public int maxAmountOfPoolElementsOnWeapon = 30;


	public bool reloading;
	public bool aimingCorrectly;
	public List<vehicleWeapons> weapons = new List<vehicleWeapons> ();
	public GameObject vehicle;

	vehicleWeapons currentWeapon;

	bool currentWeaponLocated;

	public Transform noInputWeaponDirection;
	public bool canRotateWeaponsWithNoCameraInput;
	public float noCameraInputWeaponRotationSpeed;
	public float weaponCursorMovementSpeed;
	public float weaponCursorHorizontalLimit;
	public float weaponCursorVerticalLimit;
	public LayerMask weaponCursorRaycastLayer;

	public bool useWeaponCursorScreenLimit;

	public Vector2 currentLockedCameraCursorSize;

	bool weaponCursorDisabled;

	float horizontaLimit;
	float verticalLimit;

	public float baseXRotationSpeed = 10;
	public float baseYRotationSpeed = 10;

	public string mainDecalManagerName = "Decal Manager";

	public string[] impactDecalList;
	decalManager impactDecalManager;

	List<GameObject> locatedEnemies = new List<GameObject> ();

	List<GameObject> shells = new List<GameObject> ();
	float rotationY = 0;
	float rotationX = 0;
	float lastShoot = 0;

	bool shellsOnSceneToRemove;

	float lastTimeFired;

	float destroyShellsTimer = 0;
	public int choosedWeapon;
	public int weaponIndexToStart;

	public string currentWeaponName;

	public bool aimingHomingProjectile;
	bool usingLaser;
	bool launchingProjectile;
	bool objectiveFound;
	bool touchPlatform;
	bool touchEnabled;
	bool touching;

	public bool shootingPreviously;
	GameObject currentProjectile;

	projectileSystem currentProjectileSystem;
	Rigidbody currentProjectileRigidbody;

	GameObject closestEnemy;

	Transform currentWeaponDirection;
	Transform vehicleCameraTransform;

	Vector3 baseXTargetRotation;
	Vector3 baseYTargetRotation;

	Quaternion noInputWeaponDirectionTargetRotation;
	Vector2 axisValues;
	float horizontalMouse;
	float verticalMouse;
	Vector2 lookAngle;

	public vehicleCameraController vehicleCameraManager;
	public Rigidbody mainRigidbody;
	public inputActionManager actionManager;
	public vehicleHUDManager hudManager;
	public launchTrayectory parable;
	public vehicleGravityControl gravityControlManager;

	Transform vehicleCameraManagerTransform;

	RaycastHit hit;
	Ray ray;

	GameObject parableGameObject;
	Touch currentTouch;

	Vector3 swipeStartPos;
	Vector3 aimedZone;

	GameObject currentDriver;
	Transform mainCameraTransform;
	Vector3 currentMainCameraPosition;

	Camera mainCamera;
	playerCamera playerCameraManager;
	playerHUDManager vehicleHUDInfoManager;
	playerInputManager playerInput;

	bool weaponsPaused;

	Coroutine muzzleFlashCoroutine;
	bool currentCameraUseRotationInput;

	RectTransform weaponCursor;
	Vector3 pointTargetPosition;
	Vector3 projectileDirectionToLook;
	RawImage weaponCursorImage;

	GameObject currentDetectedSurface;
	GameObject currentTargetDetected;
	Transform currentTargetToAim;

	float posX, posY;
	Ray newRay;
	Vector2 newCameraPosition;
	Vector3 targetDirection;
	Quaternion targetRotation;

	bool playerCameraIsLocked;
	bool usingCameraWithNoInput;
	Vector3 homingProjectileRayPosition;
	Vector3 autoShootRayPosition;

	bool pointTargetPositionFound;
	bool pointTargetPositionSurfaceFound;
	GameObject currentPointTargetSurfaceFound;
	GameObject previousPointTargetSurfaceFound;

	playerHUDManager.vehicleHUDElements currentHUDElements;
	bool usingCustomReticle;

	Vector3 screenPoint;
	bool targetOnScreen;

	bool usingScreenSpaceCamera;

	vehicleLaser currentVehicleLaser;

	float screenWidth;
	float screenHeight;

	GameObject shellClone;
	AudioClip newClipToShell;
	weaponShellSystem newWeaponShellSystem;

	GameObject muzzleParticlesClone;

	Transform currentProjectilePosition;

	bool componentsInitialized;

	bool autoShootOnTagActive;

	GameObject previousTargetDetectedOnAutoShootOnTag;
	GameObject currentTargetDetectedOnAutoShootOnTag;


	void Start ()
	{
		if (!weaponsEnabled) {
			return;
		}

		initializeComponents ();
	}

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (parable != null) {
			parableGameObject = parable.gameObject;
		}

		// Check the current type of platform
		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (weaponIndexToStart >= weapons.Count) {
			print (
				"WARNING: The weapon index configured is higher than the current amount of weapons in the vehicle, " +
				"check that the index is lower than the current amount of weapons."
			);
		}

		choosedWeapon = weaponIndexToStart;

		currentWeapon = weapons [choosedWeapon];

		currentWeaponLocated = true;

		currentWeaponName = currentWeapon.Name;

		if (vehicle == null) {
			vehicle = gameObject;
		}

		if (weaponsSlotsAmount < 10) {
			weaponsSlotsAmount++;
		}

		vehicleCameraManagerTransform = vehicleCameraManager.transform;

		componentsInitialized = true;
	}

	void FixedUpdate ()
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			if (hasBaseRotation) {
				// Rotate every transform of the weapon base
				baseY.transform.localRotation = Quaternion.Lerp (baseY.transform.localRotation, Quaternion.Euler (baseXTargetRotation), Time.deltaTime * baseYRotationSpeed);
				baseX.transform.localRotation = Quaternion.Lerp (baseX.transform.localRotation, Quaternion.Euler (baseYTargetRotation), Time.deltaTime * baseXRotationSpeed);
			}
		}
	}

	void Update ()
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			getWeaponBaseRotation ();

			checkWeaponTouchInputActions ();

			setVehicleWeaponCameraDirection ();

			checkAutoShootOnTag ();

			checkOtherWeaponElements ();

			checkIfDisableWeaponCursor ();

			if (autoShootOnTagActive) {
				shootWeapon (true);
			}
		}

		// If the amount of shells from the projectiles is higher than 0, check the time to remove then
		if (shellsOnSceneToRemove && shells.Count > 0) {
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
			}
		}
	}

	void getWeaponBaseRotation ()
	{
		if (hasBaseRotation) {
			playerCameraIsLocked = !playerCameraManager.isCameraTypeFree ();

			if (currentCameraUseRotationInput && !playerCameraIsLocked) {
				currentWeaponDirection = vehicleCameraTransform;

				currentMainCameraPosition = mainCameraTransform.position;

				if (usingCameraWithNoInput) {
					resetWeaponCursorPosition ();

					usingCameraWithNoInput = false;
				}
			} else {
				usingCameraWithNoInput = true;

				currentWeaponDirection = noInputWeaponDirection;

				if (playerCameraIsLocked) {
					vehicleCameraTransform = playerCameraManager.getCurrentLockedCameraTransform ();

					mainCameraTransform = vehicleCameraTransform;

					currentCameraUseRotationInput = false;
				}

				if (weaponCursor != null) {
					if (currentWeapon.fireWeaponForward) {
						currentWeaponDirection = vehicleCameraTransform;

						currentMainCameraPosition = mainCameraTransform.position;

						weaponCursor.anchoredPosition = Vector2.zero;
					} else if (canRotateWeaponsWithNoCameraInput && !actionManager.input.gameCurrentlyPaused) {
						axisValues = actionManager.getPlayerMouseAxis ();

						horizontalMouse = axisValues.x;
						verticalMouse = axisValues.y;

						weaponCursor.Translate (new Vector3 (horizontalMouse, verticalMouse, 0) * weaponCursorMovementSpeed);

						newCameraPosition = weaponCursor.anchoredPosition;

						if (useWeaponCursorScreenLimit) {
							posX = Mathf.Clamp (newCameraPosition.x, -weaponCursorHorizontalLimit, weaponCursorHorizontalLimit);
							posY = Mathf.Clamp (newCameraPosition.y, -weaponCursorVerticalLimit, weaponCursorVerticalLimit);
							weaponCursor.anchoredPosition = new Vector2 (posX, posY);
						} else {
							newCameraPosition = weaponCursor.position;
							newCameraPosition.x = Mathf.Clamp (newCameraPosition.x, currentLockedCameraCursorSize.x, horizontaLimit);
							newCameraPosition.y = Mathf.Clamp (newCameraPosition.y, currentLockedCameraCursorSize.y, verticalLimit);
							weaponCursor.position = new Vector3 (newCameraPosition.x, newCameraPosition.y, 0);
						}

						newRay = mainCamera.ScreenPointToRay (weaponCursor.position);

						if (Physics.Raycast (newRay, out hit, Mathf.Infinity, weaponCursorRaycastLayer)) {
							pointTargetPositionFound = false;

							// Check if the object found is the current vehicle, to avoid to aim at it and aim in the proper direction even if this vehicle is found
							if (hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider)) {
								if (currentWeapon.useRaycastAllToCheckSurfaceFound && !currentWeapon.fireWeaponForward) {
									RaycastHit[] hits = Physics.RaycastAll (newRay, currentWeapon.maxDistanceToRaycastAll, layer);
									System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));

									pointTargetPositionSurfaceFound = false;

									foreach (RaycastHit rh in hits) {
										if (!pointTargetPositionSurfaceFound && !hudManager.checkIfDetectSurfaceBelongToVehicle (rh.collider)) {
											pointTargetPosition = rh.point;
											currentDetectedSurface = rh.collider.gameObject;
											pointTargetPositionSurfaceFound = true;
										}
									}
								} else {
									pointTargetPositionFound = true;
								}
							} else {
								pointTargetPositionFound = true;
							}

							if (pointTargetPositionFound) {
								pointTargetPosition = hit.point;
								currentDetectedSurface = hit.collider.gameObject;
							}
						} else {
							pointTargetPosition = new Vector3 (newRay.origin.x, currentWeaponDirection.position.y, newRay.origin.z);
							currentDetectedSurface = null;
						}

						if (currentTargetDetected != currentDetectedSurface) {
							currentTargetDetected = currentDetectedSurface;

							if (currentTargetDetected != null) {
								currentTargetToAim = applyDamage.getPlaceToShoot (currentTargetDetected);

//								if (currentTargetToAim) {
//									print (currentTargetToAim.name);
//								}
							} else {
								currentTargetToAim = null;
							}
						}

						if (currentTargetToAim != null) {
							pointTargetPosition = currentTargetToAim.position;
						}

						targetDirection = pointTargetPosition - currentWeaponDirection.position;
						targetRotation = Quaternion.LookRotation (targetDirection);

						lookAngle.x = targetRotation.eulerAngles.y;
						lookAngle.y = targetRotation.eulerAngles.x;

						noInputWeaponDirectionTargetRotation = Quaternion.Euler (lookAngle.y, lookAngle.x, 0);

						noInputWeaponDirection.rotation = Quaternion.Slerp (noInputWeaponDirection.rotation, noInputWeaponDirectionTargetRotation,
							noCameraInputWeaponRotationSpeed * Time.deltaTime);

						currentMainCameraPosition = pointTargetPosition;
					}
				}
			}

			// Rotate the weapon to look in the camera direction
			Quaternion cameraDirection = Quaternion.LookRotation (currentWeaponDirection.forward);
			weaponLookDirection.rotation = cameraDirection;
			float angleX = weaponLookDirection.localEulerAngles.x;
			// Clamp the angle of the weapon, to avoid a rotation higher that the camera

			// In X axis
			if (angleX >= 0 && angleX <= minimumX) {
				rotationX = angleX;
				aimingCorrectly = true;
			} else if (angleX >= maximumX && angleX <= 360) {
				rotationX = angleX;
				aimingCorrectly = true;
			} else {
				aimingCorrectly = false;
			}

			// In Y axis
			float angleY = weaponLookDirection.localEulerAngles.y;
			if (angleY >= 0 && angleY <= minimumY) {
				rotationY = angleY;
			} else if (angleY >= maximumY && angleY <= 360) {
				rotationY = angleY;
			}

			baseXTargetRotation = new Vector3 (0, rotationY, 0);
			baseYTargetRotation = new Vector3 (rotationX, 0, 0);
		}
	}

	void checkWeaponTouchInputActions ()
	{
		// Check if a key number has been pressed, to change the current weapon for the key pressed, if there is a weapon using that key
		if (!touchEnabled) {
			int currentNumberInput = playerInput.checkNumberInput (weaponsSlotsAmount);

			if (currentNumberInput > -1) {
				for (int k = 0; k < weapons.Count; k++) {
					if (choosedWeapon != k && weapons [k].numberKey == currentNumberInput) {
						if (weapons [k].enabled) {
							choosedWeapon = k;

							weaponChanged ();
						}
					}
				}
			}
		}

		// If the touch controls are enabled, activate the swipe option
		if (touchEnabled) {
			// Select the weapon by swiping the finger in the right corner of the screen, above the weapon info
			int touchCount = Input.touchCount;
			if (!touchPlatform) {
				touchCount++;
			}

			for (int i = 0; i < touchCount; i++) {
				if (!touchPlatform) {
					currentTouch = touchJoystick.convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (i);
				}

				// Get the start position of the swipe
				if (vehicleHUDInfoManager.isFingerPressingTouchPanel () && !touching) {
					swipeStartPos = currentTouch.position;
					touching = true;
				}

				// And the final position, and get the direction, to change to the previous or the next power
				if (currentTouch.phase == TouchPhase.Ended) {
					if (touching) {
						float swipeDistHorizontal = (new Vector3 (currentTouch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;

						if (swipeDistHorizontal > minSwipeDist) {
							float swipeValue = Mathf.Sign (currentTouch.position.x - swipeStartPos.x);
							if (swipeValue > 0) {
								// Right swipe
								choosePreviousWeapon ();
							} else if (swipeValue < 0) {
								// Left swipe
								chooseNextWeapon ();
							}
						}

						touching = false;
					}

					vehicleHUDInfoManager.setTouchingMenuPanelState (false);
				}
			}
		}
	}

	void checkOtherWeaponElements ()
	{
		// If the current weapon is the homing missiles
		if (aimingHomingProjectile) {
			// Check if the amount of located enemies is equal or lower that the number of remaining projectiles
			if (locatedEnemies.Count < currentWeapon.projectilePosition.Count) {
				// Uses a ray to detect enemies, to locked them

				homingProjectileRayPosition = mainCameraTransform.position;

				if (usingCameraWithNoInput) {
					homingProjectileRayPosition = weaponLookDirection.position;
				}

				if (Physics.Raycast (homingProjectileRayPosition, projectileDirectionToLook, out hit, Mathf.Infinity, layer)) {
					Debug.DrawLine (homingProjectileRayPosition, hit.point, Color.yellow, 2);

					GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

					if (target != null) {
						if (target != vehicle) {
							if (currentWeapon.tagToLocate.Contains (target.tag)) {
								GameObject placeToShoot = applyDamage.getPlaceToShootGameObject (target);

								if (placeToShoot != null) {
									if (!locatedEnemies.Contains (placeToShoot)) {
										// If an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
										locatedEnemies.Add (placeToShoot);

										GKC_Utils.addElementToPlayerScreenObjectivesManager (currentDriver, placeToShoot.gameObject, currentWeapon.locatedEnemyIconName);
									}
								}
							}
						}
					}
				}
			}
		}

		// If the current weapon is the laser
		if (usingLaser) {
			if (currentWeapon.clipSize > 0) {
				// Play the animation
				if (currentWeapon.weaponAnimation != null) {
					currentWeapon.weaponAnimation.Play (currentWeapon.animation);
				}

				// Play the sound 
				if (Time.time > lastShoot + currentWeapon.fireRate) {
					lastShoot = Time.time;

					checkIfEnableWeaponCursor ();

					playWeaponSoundEffect (true);

					// Reduce the amount of ammo
					useAmmo ();
				}

				checkShotShake ();
			} else {
				if (currentWeapon.autoReloadWhenClipEmpty) {
					autoReload ();
				}
			}
		}

		// If the current weapon launched the projectile
		if (launchingProjectile) {
			// If the launcher animation is not being played
			if (currentWeapon.weapon != null && !currentWeapon.animation.Equals ("")) {
				if (currentWeapon.weaponAnimation != null && !currentWeapon.weaponAnimation.IsPlaying (currentWeapon.animation)) {
					// Reverse it and play it again
					if (currentWeapon.weaponAnimation [currentWeapon.animation].speed == 1) {
						currentWeapon.weaponAnimation [currentWeapon.animation].speed = -1;
						currentWeapon.weaponAnimation [currentWeapon.animation].time = currentWeapon.weaponAnimation [currentWeapon.animation].length;
						currentWeapon.weaponAnimation.Play (currentWeapon.animation);

						currentProjectile.transform.SetParent (null);

						launchCurrentProjectile (mainCameraTransform.TransformDirection (Vector3.forward));

						return;
					}

					// The launcher has thrown a projectile and the animation is over
					if (currentWeapon.weaponAnimation [currentWeapon.animation].speed == -1) {
						launchingProjectile = false;

						if (currentWeapon.projectileModel != null) {
							currentWeapon.projectileModel.SetActive (true);
						}
					}
				}
			} else {
				launchCurrentProjectile (mainCameraTransform.TransformDirection (Vector3.forward));
				launchingProjectile = false;
			}
		}
	}

	public void checkIfEnableWeaponCursor ()
	{
		if (currentWeapon.disableWeaponCursorWhileNotShooting) {
			if (weaponCursor != null && weaponCursorDisabled) {
				weaponCursor.gameObject.SetActive (true);
				weaponCursorDisabled = false;
			}

			lastTimeFired = Time.time;
		}
	}

	public void checkIfDisableWeaponCursor ()
	{
		if (currentWeapon.disableWeaponCursorWhileNotShooting) {
			if (weaponCursor != null && !weaponCursorDisabled) {
				if (Time.time > currentWeapon.delayToDisableWeaponCursor + lastTimeFired) {
					weaponCursor.gameObject.SetActive (false);
					weaponCursorDisabled = true;
				}
			}
		} else {
			if (weaponCursor != null && weaponCursorDisabled) {
				weaponCursor.gameObject.SetActive (true);
				weaponCursorDisabled = false;
			}
		}
	}

	public void launchCurrentProjectile (Vector3 cameraDirection)
	{
		// Launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		Rigidbody currentProjectileRigidbody = currentProjectile.GetComponent<Rigidbody> ();

		currentProjectileRigidbody.isKinematic = false;

		if (currentWeapon.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone, cameraDirection);

			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
			}

			currentProjectileRigidbody.AddForce (newVel, ForceMode.VelocityChange);
		} else {
			currentProjectileRigidbody.AddForce (currentWeapon.parableDirectionTransform.forward * currentWeapon.projectileSpeed, ForceMode.Impulse);
		}
	}

	/// <summary>
	/// If the homing missile weapon has been fired or change when enemies were locked, remove the icons from the screen.
	/// </summary>
	void removeLocatedEnemiesIcons ()
	{
		for (int i = 0; i < locatedEnemies.Count; i++) {
			if (locatedEnemies [i] != null) {
				GKC_Utils.removeElementToPlayerScreenObjectivesManager (currentDriver, locatedEnemies [i].gameObject);
			}
		}

		locatedEnemies.Clear ();
	}

	/// <summary>
	/// Play the fire sound or the empty clip sound.
	/// </summary>
	void playWeaponSoundEffect (bool hasAmmo)
	{
		if (hasAmmo) {
			if (currentWeapon.shootSoundEffect != null) {
				weaponsEffectsSource.clip = currentWeapon.shootSoundEffect;
				weaponsEffectsSource.Play ();
				//weaponsEffectsSource.PlayOneShot (weapons [choosedWeapon].soundEffect);
			}
		} else {
			if (Time.time > lastShoot + currentWeapon.fireRate) {
				weaponsEffectsSource.PlayOneShot (outOfAmmo);
				lastShoot = Time.time;

				checkIfEnableWeaponCursor ();
			}
		}
	}

	/// <summary>
	/// Calculate the speed applied to the launched projectile to make a parable according to a hit point.
	/// </summary>
	Vector3 getParableSpeed (Vector3 origin, Vector3 target, Vector3 cameraDirection)
	{
		// If a hit point is not found, return
		if (!objectiveFound) {
			if (currentWeapon.useMaxDistanceWhenNoSurfaceFound) {
				target = origin + cameraDirection * currentWeapon.maxDistanceWhenNoSurfaceFound;

				Debug.DrawLine (origin, target, Color.gray, 5);
			} else {
				return -Vector3.one;
			}
		}

		// Get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;

		// Remove the Y axis value
		toTargetXZ -= vehicleCameraManagerTransform.transform.InverseTransformDirection (toTargetXZ).y * vehicleCameraManagerTransform.transform.up;
		float y = vehicleCameraManagerTransform.transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;

		// Get the velocity according to distance and gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;

		// Create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;

		// Get direction of xz but with magnitude 1
		result *= v0xz;

		// Set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= vehicleCameraManagerTransform.transform.InverseTransformDirection (result).y * vehicleCameraManagerTransform.transform.up;
		result += vehicleCameraManagerTransform.transform.up * v0y;
		return result;
	}

	/// <summary>
	/// Fire the current weapon.
	/// </summary>
	public void shootWeapon (bool shootAtKeyDown)
	{
		if (!shootAtKeyDown && !aimingHomingProjectile) {
			if (shootingPreviously) {
				shootingPreviously = false;
			}

			return;
		}

		// If the weapon system is active and the clip size higher than 0
		if (!weaponsActivate || weaponsPaused) {
			return;
		}

		if (currentWeapon.clipSize > 0) {
			// Else, fire the current weapon according to the fire rate
			if (Time.time > lastShoot + currentWeapon.fireRate) {
				shootingPreviously = true;

				// If the current weapon is the homing missile, set to true and return
				// If the current projectile is homing type, check when the shoot button is pressed and release
				if ((currentWeapon.isHommingProjectile && shootAtKeyDown)) {
					aimingHomingProjectile = true;
					//print ("1 "+ shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHomingProjectile);
					return;
				}

				if ((currentWeapon.isHommingProjectile && !shootAtKeyDown && locatedEnemies.Count <= 0) ||
				    (!currentWeapon.isHommingProjectile && !shootAtKeyDown)) {
					aimingHomingProjectile = false;
					shootingPreviously = false;

					checkShotShake ();

					//print ("2 "+shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHomingProjectile);
					return;
				}


				checkShotShake ();

				// If the current weapon is the laser, enable it and return
				if (currentWeapon.isLaser) {
					changeWeaponLaserState (true);

					return;
				}

				// Play the fire sound
				playWeaponSoundEffect (true);

				// Create the muzzle flash
				createMuzzleFlash ();

				// Play the fire animation
				if (currentWeapon.weapon != null && currentWeapon.animation != "" && currentWeapon.weaponAnimation) {
					if (currentWeapon.launchProjectile) {
						currentWeapon.weaponAnimation [currentWeapon.animation].speed = 1;

						// Disable the projectile model in the weapon
						if (currentWeapon.projectileModel != null) {
							currentWeapon.projectileModel.SetActive (false);
						}
					}

					currentWeapon.weaponAnimation.Play (currentWeapon.animation);
				}

				Vector3 currentUp = Vector3.up;

				if (gravityControlManager != null) {
					currentUp = gravityControlManager.currentNormal;
				}

				// Every weapon can shoot 1 or more projectiles at the same time, so for every projectile position to instantiate
				for (int j = 0; j < currentWeapon.projectilePosition.Count; j++) {
					Transform currentProjectilePosition = currentWeapon.projectilePosition [j];

					for (int l = 0; l < currentWeapon.projectilesPerShoot; l++) {
						// Create the projectile
						if (projectilesPoolEnabled) {
							currentProjectile = GKC_PoolingSystem.Spawn (currentWeapon.projectileToShoot, currentProjectilePosition.position, currentProjectilePosition.rotation,
								maxAmountOfPoolElementsOnWeapon);
						} else {
							currentProjectile = (GameObject)Instantiate (currentWeapon.projectileToShoot, currentProjectilePosition.position, currentProjectilePosition.rotation);
						}

						// Set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();

						if (currentProjectileSystem != null) {
							currentProjectileSystem.checkToResetProjectile ();

							currentProjectileRigidbody = currentProjectileSystem.getProjectileRigibody ();
						}

						if (!currentWeapon.launchProjectile) {
							// Set its direction in the weapon forward or the camera forward according to if the weapon is aimed correctly or not
							if (Physics.Raycast (currentMainCameraPosition, projectileDirectionToLook, out hit, Mathf.Infinity, layer) && aimingCorrectly &&
							    !currentWeapon.fireWeaponForward) {
								if (!hit.collider.isTrigger) {
									currentProjectile.transform.LookAt (hit.point);
								}
							}
						}

                        // If the current weapon launches projectiles instead of shooting
                        else {
							// If the projectile is not being launched, then 
							if (!launchingProjectile) {
								if (currentWeapon.projectileModel != null) {
									currentProjectile.transform.SetParent (currentProjectilePosition);
									currentProjectile.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
								}

								if (currentProjectileRigidbody == null) {
									currentProjectileRigidbody = currentProjectile.GetComponent<Rigidbody> ();
								}

								currentProjectileRigidbody.isKinematic = true;

								// If the vehicle has a gravity control component, and the current gravity is not the regular one, add an artificial gravity component to the projectile
								// like this, it can make a parable in any surface and direction, setting its gravity in the same of the vehicle
								if (currentUp != Vector3.up) {
									currentProjectile.AddComponent<artificialObjectGravity> ().setCurrentGravity (-currentUp);
								}

								explosiveBarrel currentExplosiveBarrel = currentProjectile.GetComponent<explosiveBarrel> ();

								if (currentExplosiveBarrel != null) {
									currentExplosiveBarrel.canExplodeState (true);
									currentExplosiveBarrel.setExplosiveBarrelOwner (vehicle);
									currentExplosiveBarrel.setExplosionValues (currentWeapon.explosionForce, currentWeapon.explosionRadius);
								}

								if (currentWeapon.useParableSpeed) {
									// Get the ray hit point where the projectile will fall
									if (Physics.Raycast (currentMainCameraPosition, projectileDirectionToLook, out hit, Mathf.Infinity, layer)) {
										aimedZone = hit.point;
										objectiveFound = true;
									} else {
										objectiveFound = false;
									}
								}

								launchingProjectile = true;
							}
						}

						// Add spread to the projectile
						Vector3 spreadAmount = Vector3.zero;

						if (currentWeapon.useProjectileSpread) {
							spreadAmount = setProjectileSpread ();
							currentProjectile.transform.Rotate (spreadAmount);
						}

						if (currentProjectileSystem != null) {
							currentProjectileSystem.setProjectileInfo (setProjectileInfo ());
						}

						if (currentWeapon.isSeeker) {
							closestEnemy = setSeekerProjectileInfo (currentProjectilePosition.position);

							if (closestEnemy != null) {
								print (closestEnemy.name);

								currentProjectileSystem.setEnemy (closestEnemy);
							}
						}

						bool projectileFiredByRaycast = false;

						bool destroyProjectile = false;

						// If the weapon shoots setting directly the projectile in the hit point, place the current projectile in the hit point position
						if (currentWeapon.useRayCastShoot) {
							Vector3 forwardDirection = projectileDirectionToLook;
							Vector3 forwardPositon = currentMainCameraPosition;

							if (!aimingCorrectly || currentWeapon.fireWeaponForward) {
								forwardDirection = currentWeapon.weapon.transform.forward;
								forwardPositon = currentProjectilePosition.position;
							}

							if (!currentCameraUseRotationInput && !currentWeapon.fireWeaponForward) {
								forwardPositon = weaponLookDirection.position;
								forwardDirection = projectileDirectionToLook;
							}

							if (!currentCameraUseRotationInput && !hasBaseRotation) {
								forwardDirection = currentWeapon.weapon.transform.forward;
							}

							if (spreadAmount.magnitude != 0) {
								forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
							}

							//Debug.DrawLine (forwardPositon, currentMainCameraPosition, Color.white, 3);

							// Check what element the projectile will impact, if the vehicle is found, destroy the projectile to avoid shooting at your own vehicle
							if (Physics.Raycast (forwardPositon, forwardDirection, out hit, Mathf.Infinity, layer)) {
								//Debug.DrawLine (forwardPositon, hit.point, Color.red, 3);
								//print (hit.collider.name + " " + hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider));

								if (hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider)) {
									if (currentWeapon.useRaycastAllToCheckSurfaceFound && !currentWeapon.fireWeaponForward) {
										ray = new Ray ();
										if (currentCameraUseRotationInput) {
											if (aimingCorrectly) {
												ray.direction = projectileDirectionToLook;
												ray.origin = currentMainCameraPosition;
											} else {
												ray.origin = currentProjectilePosition.position;
												ray.direction = currentWeapon.weapon.transform.forward;
											}
										} else {
											ray.origin = weaponLookDirection.position;
											ray.direction = projectileDirectionToLook;
										}

										if (!hasBaseRotation && !currentCameraUseRotationInput) {
											ray.direction = mainCameraTransform.TransformDirection (Vector3.forward);
										}

										if (spreadAmount.magnitude != 0) {
											ray.direction = Quaternion.Euler (spreadAmount) * ray.direction;
										}

										RaycastHit[] hits = Physics.RaycastAll (ray, currentWeapon.maxDistanceToRaycastAll, layer);
										System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));

										bool surfaceFound = false;
										foreach (RaycastHit rh in hits) {
											if (!surfaceFound && !hudManager.checkIfDetectSurfaceBelongToVehicle (rh.collider)) {
												//Debug.DrawLine (ray.origin, rh.point, Color.yellow, 3);
												//print (rh.collider.name + " " + hudManager.checkIfDetectSurfaceBelongToVehicle (rh.collider));

												if (currentWeapon.useFakeProjectileTrails) {
													currentProjectileSystem.creatFakeProjectileTrail (rh.point);
												}

												currentProjectileSystem.rayCastShoot (rh.collider, rh.point + rh.normal * 0.02f, projectileDirectionToLook);

												surfaceFound = true;

												projectileFiredByRaycast = true;
											}
										}

										if (!surfaceFound) {
											destroyProjectile = true;
										}
									} else {
										if (Physics.Raycast (currentProjectilePosition.position, forwardDirection, out hit, Mathf.Infinity, layer)) {
											if (hudManager.checkIfDetectSurfaceBelongToVehicle (hit.collider)) {
												destroyProjectile = true;
											} else {
												if (currentWeapon.useFakeProjectileTrails) {
													currentProjectileSystem.creatFakeProjectileTrail (hit.point);
												}

												currentProjectileSystem.rayCastShoot (hit.collider, hit.point + hit.normal * 0.02f, forwardDirection);

												projectileFiredByRaycast = true;
											}
										} else {
											destroyProjectile = true;
										}
									}
								} else {
									currentProjectileSystem.rayCastShoot (hit.collider, hit.point + hit.normal * 0.02f, forwardDirection);

									projectileFiredByRaycast = true;
								}
							} else {
								destroyProjectile = true;
							}

							if (destroyProjectile) {
								currentProjectileSystem.initializeProjectile ();

								if (currentWeapon.useFakeProjectileTrails) {
									currentProjectileSystem.creatFakeProjectileTrail (forwardPositon + forwardDirection * 50);
									currentProjectileSystem.setFakeProjectileTrailSpeedMultiplier (0.3f);
									currentProjectileSystem.setDestroyTrailAfterTimeState (true);
								}

								currentProjectileSystem.destroyProjectile ();
							}

							//Debug.DrawLine (forwardPositon, hit.point, Color.red, 2);
						}

						if (!projectileFiredByRaycast && !destroyProjectile) {
							if (currentProjectileSystem != null) {
								currentProjectileSystem.initializeProjectile ();
							}
						}

						if (aimingHomingProjectile) {
							// Check that the located enemies are higher that 0
							if (locatedEnemies.Count > 0) {
								// Shoot the missiles
								if (j < locatedEnemies.Count) {
									currentProjectileSystem.setProjectileInfo (setProjectileInfo ());
									currentProjectileSystem.setEnemy (locatedEnemies [j]);

									currentProjectileSystem.initializeProjectile ();
								} else {
									currentProjectileSystem.destroyProjectile ();
								}
							}
						}
					}

					useAmmo ();

					lastShoot = Time.time;

					checkIfEnableWeaponCursor ();

					if (currentWeapon.applyForceAtShoot) {
						Transform currentVehicleCameraTransform = vehicleCameraManager.getCurrentCameraTransform ();

						if (currentWeapon.useCustomTransformToForceShoot) {
							currentVehicleCameraTransform = currentWeapon.customTransformToForceShoot;
						}

						Vector3 forceDirection = (currentVehicleCameraTransform.right * currentWeapon.forceDirection.x +
						                         currentVehicleCameraTransform.up * currentWeapon.forceDirection.y +
						                         currentVehicleCameraTransform.forward * currentWeapon.forceDirection.z) * currentWeapon.forceAmount;

						mainRigidbody.AddForce (mainRigidbody.mass * forceDirection, ForceMode.Impulse);
					}
				}

				// If the current weapon drops shells, create them
				createShells ();

				if (currentWeapon.isHommingProjectile && !shootAtKeyDown && aimingHomingProjectile) {
					// If the button to shoot is released, shoot a homing projectile for every located enemy
					// Check that the located enemies are higher that 0
					if (locatedEnemies.Count > 0) {
						// Remove the icons in the screen
						removeLocatedEnemiesIcons ();
					}

					aimingHomingProjectile = false;
				}
			}
		}
        // Else, the clip in the weapon is over, so check if there is remaining ammo
        else {
			if (currentWeapon.autoReloadWhenClipEmpty) {
				autoReload ();
			}
		}
	}

	public void setVehicleWeaponCameraDirection ()
	{
		projectileDirectionToLook = mainCameraTransform.TransformDirection (Vector3.forward);

		if (!currentCameraUseRotationInput) {
			projectileDirectionToLook = currentMainCameraPosition - weaponLookDirection.position;
			//Debug.DrawLine (weaponLookDirection.position, currentMainCameraPosition, Color.yellow, 2);
		}
	}

	public void autoReload ()
	{
		// Disable the laser
		changeWeaponLaserState (false);

		// If the weapon is not being reloaded, do it
		if (!reloading) {
			StartCoroutine (waitToReload (currentWeapon.reloadTime));
		}
		//checkRemainAmmo ();
	}

	public void changeWeaponLaserState (bool state)
	{
		if (currentWeapon.weapon != null) {
			if (currentVehicleLaser == null) {
				currentVehicleLaser = currentWeapon.weapon.GetComponentInChildren<vehicleLaser> ();
			}

			if (currentVehicleLaser != null) {
				currentVehicleLaser.changeLaserState (state);
				usingLaser = state;
			}
		}
	}

	public void createShells ()
	{
		if (currentWeapon.ejectShellOnShot) {
			for (int j = 0; j < currentWeapon.shellPosition.Count; j++) {
				if (currentWeapon.shell != null) {
					if (projectilesPoolEnabled) {
						shellClone = GKC_PoolingSystem.Spawn (currentWeapon.shell, currentWeapon.shellPosition [j].position, currentWeapon.shellPosition [j].rotation,
							maxAmountOfPoolElementsOnWeapon);
					} else {
						shellClone = (GameObject)Instantiate (currentWeapon.shell, currentWeapon.shellPosition [j].position, currentWeapon.shellPosition [j].rotation);
					}

					newWeaponShellSystem = shellClone.GetComponent<weaponShellSystem> ();

					if (currentWeapon.shellDropSoundList.Count > 0) {
						newClipToShell = currentWeapon.shellDropSoundList [Random.Range (0, currentWeapon.shellDropSoundList.Count - 1)];
					}

					newWeaponShellSystem.setShellValues (currentWeapon.shellPosition [j].right * (currentWeapon.shellEjectionForce * GKC_Utils.getCurrentScaleTime ()), null,
						newClipToShell);

					shells.Add (shellClone);

					if (shells.Count > 15) {
						GameObject shellToRemove = shells [0];

						shells.RemoveAt (0);

						if (projectilesPoolEnabled) {
							GKC_PoolingSystem.Despawn (shellToRemove);
						} else {
							Destroy (shellToRemove);
						}
					}

					shellsOnSceneToRemove = true;
				}
			}

			destroyShellsTimer = 0;
		}
	}

	public void testWeaponShake (int weaponIndex)
	{
		vehicleWeapons weaponToTest = weapons [weaponIndex];

		if (weaponToTest.shootShakeInfo.useDamageShake) {
			if (weaponToTest.shootShakeInfo.sameValueBothViews) {
				vehicleCameraManager.setCameraExternalShake (weaponToTest.shootShakeInfo.thirdPersonDamageShake);
			} else {
				if (vehicleCameraManager.currentState.firstPersonCamera) {
					if (weaponToTest.shootShakeInfo.useDamageShakeInFirstPerson) {
						vehicleCameraManager.setCameraExternalShake (weaponToTest.shootShakeInfo.firstPersonDamageShake);
					}
				} else {
					if (weaponToTest.shootShakeInfo.useDamageShakeInThirdPerson) {
						vehicleCameraManager.setCameraExternalShake (weaponToTest.shootShakeInfo.thirdPersonDamageShake);
					}
				}
			}
		}
	}

	public void checkShotShake ()
	{
		if (currentWeapon.shootShakeInfo.useDamageShake) {
			if (currentWeapon.shootShakeInfo.sameValueBothViews) {
				vehicleCameraManager.setCameraExternalShake (currentWeapon.shootShakeInfo.thirdPersonDamageShake);
			} else {
				if (vehicleCameraManager.currentState.firstPersonCamera) {
					if (currentWeapon.shootShakeInfo.useDamageShakeInFirstPerson) {
						vehicleCameraManager.setCameraExternalShake (currentWeapon.shootShakeInfo.firstPersonDamageShake);
					}
				} else {
					if (currentWeapon.shootShakeInfo.useDamageShakeInThirdPerson) {
						vehicleCameraManager.setCameraExternalShake (currentWeapon.shootShakeInfo.thirdPersonDamageShake);
					}
				}
			}
		}
	}

	/// <summary>
	/// Create the muzzle flash particles if the weapon has it.
	/// </summary>
	void createMuzzleFlash ()
	{
		if (currentWeapon.muzzleParticles != null) {
			for (int j = 0; j < currentWeapon.projectilePosition.Count; j++) {
				currentProjectilePosition = currentWeapon.projectilePosition [j];

				if (projectilesPoolEnabled) {
					muzzleParticlesClone = GKC_PoolingSystem.Spawn (currentWeapon.muzzleParticles, currentProjectilePosition.position, currentProjectilePosition.rotation,
						maxAmountOfPoolElementsOnWeapon);
				} else {
					muzzleParticlesClone = (GameObject)Instantiate (currentWeapon.muzzleParticles, currentProjectilePosition.position, currentProjectilePosition.rotation);
				}

				if (projectilesPoolEnabled) {
				} else {
					Destroy (muzzleParticlesClone, 1);
				}

				muzzleParticlesClone.transform.SetParent (currentProjectilePosition);
			}
		}
	}

	/// <summary>
	/// Decrease the amount of ammo in the clip.
	/// </summary>
	void useAmmo ()
	{
		currentWeapon.clipSize--;

		updateAmmo ();
	}

	void updateAmmo ()
	{
		if (!currentWeapon.infiniteAmmo) {
			hudManager.useAmmo (currentWeapon.clipSize, currentWeapon.remainAmmo);
		} else {
			hudManager.useAmmo (currentWeapon.clipSize, -1);
		}
	}

	/// <summary>
	/// Check the amount of ammo.
	/// </summary>
	void checkRemainAmmo ()
	{
		// If the weapon has not infinite ammo
		if (!currentWeapon.infiniteAmmo) {
			if (currentWeapon.clipSize == 0) {
				// If the remaining ammo is lower that the ammo per clip, set the final projectiles in the clip 
				if (currentWeapon.remainAmmo < currentWeapon.ammoPerClip) {
					currentWeapon.clipSize = currentWeapon.remainAmmo;
				}
                // Else, refill it
                else {
					currentWeapon.clipSize = currentWeapon.ammoPerClip;
				}

				// If the remaining ammo is higher than 0, remove the current projectiles added in the clip
				if (currentWeapon.remainAmmo > 0) {
					currentWeapon.remainAmmo -= currentWeapon.clipSize;
				}
			} else {
				int usedAmmo = 0;

				if (currentWeapon.removePreviousAmmoOnClip) {
					currentWeapon.clipSize = 0;
					if (currentWeapon.remainAmmo < (currentWeapon.ammoPerClip)) {
						usedAmmo = currentWeapon.remainAmmo;
					} else {
						usedAmmo = currentWeapon.ammoPerClip;
					}
				} else {
					if (currentWeapon.remainAmmo < (currentWeapon.ammoPerClip - currentWeapon.clipSize)) {
						usedAmmo = currentWeapon.remainAmmo;
					} else {
						usedAmmo = currentWeapon.ammoPerClip - currentWeapon.clipSize;
					}
				}

				currentWeapon.remainAmmo -= usedAmmo;
				currentWeapon.clipSize += usedAmmo;
			}
		} else {
			// Else, the weapon has infinite ammo, so refill it
			currentWeapon.clipSize = currentWeapon.ammoPerClip;
		}

		currentWeapon.auxRemainAmmo = currentWeapon.remainAmmo;
	}

	/// <summary>
	/// A delay for reload the weapon.
	/// </summary>
	IEnumerator waitToReload (float amount)
	{
		// If the remaining ammo is higher than 0 or infinite
		if (currentWeapon.remainAmmo > 0 || currentWeapon.infiniteAmmo) {
			// Reload
			reloading = true;

			// Play the reload sound
			if (currentWeapon.reloadSoundEffect != null) {
				weaponsEffectsSource.PlayOneShot (currentWeapon.reloadSoundEffect);
			}

			// Wait an amount of time
			yield return new WaitForSeconds (amount);

			// Check the ammo values
			checkRemainAmmo ();

			// Stop reload
			reloading = false;

			updateAmmo ();
		} else {
			// Else, the ammo is over, play the empty weapon sound
			playWeaponSoundEffect (false);
		}

		yield return null;
	}

	/// <summary>
	/// The vehicle has used an ammo pickup, so increase the correct weapon by name.
	/// </summary>
	public void getAmmo (string ammoName, int amount)
	{
		bool empty = false;

		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].Name.Equals (ammoName)) {
				if (weapons [i].remainAmmo == 0 && weapons [i].clipSize == 0) {
					empty = true;
				}

				weapons [i].remainAmmo += amount;

				weaponChanged ();

				if (empty && weapons [i].autoReloadWhenClipEmpty) {
					autoReload ();
				}

				weapons [i].auxRemainAmmo = weapons [i].remainAmmo;

				return;
			}
		}
	}

	public void addAuxRemainAmmo (vehicleWeapons weaponToCheck, int amount)
	{
		weaponToCheck.auxRemainAmmo += amount;
	}

	/// <summary>
	/// Select next or previous weapon.
	/// </summary>
	public void chooseNextWeapon ()
	{
		if (!weaponsActivate) {
			return;
		}

		// Check the index and get the correctly weapon 
		int max = 0;
		int numberKey = currentWeapon.numberKey;
		numberKey++;

		if (numberKey > weaponsSlotsAmount) {
			numberKey = 0;
		}

		bool exit = false;

		while (!exit) {
			for (int k = 0; k < weapons.Count; k++) {
				if (weapons [k].enabled && weapons [k].numberKey == numberKey) {
					choosedWeapon = k;
					exit = true;
				}
			}

			max++;
			if (max > 100) {
				return;
			}

			// Get the current weapon index
			numberKey++;

			if (numberKey > weaponsSlotsAmount) {
				numberKey = 0;
			}
		}

		// Set the current weapon 
		weaponChanged ();
	}

	public void choosePreviousWeapon ()
	{
		if (!weaponsActivate) {
			return;
		}

		int max = 0;

		int numberKey = currentWeapon.numberKey;

		numberKey--;

		if (numberKey < 0) {
			numberKey = weaponsSlotsAmount;
		}

		bool exit = false;

		while (!exit) {
			for (int k = weapons.Count - 1; k >= 0; k--) {
				if (weapons [k].enabled && weapons [k].numberKey == numberKey) {
					choosedWeapon = k;
					exit = true;
				}
			}

			max++;
			if (max > 100) {
				return;
			}

			numberKey--;

			if (numberKey < 0) {
				numberKey = weaponsSlotsAmount;
			}
		}

		weaponChanged ();
	}

	/// <summary>
	/// Set the info of the selected weapon in the hud.
	/// </summary>
	void weaponChanged ()
	{
		currentWeapon = weapons [choosedWeapon];

		currentWeaponLocated = true;

		currentWeaponName = currentWeapon.Name;

		hudManager.setWeaponName (currentWeapon.Name, currentWeapon.ammoPerClip, currentWeapon.clipSize);

		if (!currentWeapon.infiniteAmmo) {
			hudManager.useAmmo (currentWeapon.clipSize, currentWeapon.remainAmmo);
		} else {
			hudManager.useAmmo (currentWeapon.clipSize, -1);
		}

		checkParableTrayectory (true);

		// Remove the located enemies icon
		removeLocatedEnemiesIcons ();

		checkCurrentWeaponCustomReticle ();

		checkIfEnableWeaponCursor ();
	}

	public void checkCurrentWeaponCustomReticle ()
	{
		if (weaponCursorImage != null) {
			if (currentWeapon.useCustomReticle) {
				weaponCursorImage.texture = currentWeapon.customReticle;

				if (currentWeapon.useCustomReticleColor) {
					weaponCursorImage.color = currentWeapon.customReticleColor;
				}

				if (currentWeapon.useCustomReticleSize) {
					weaponCursor.sizeDelta = currentWeapon.customReticleSize;
				}

				usingCustomReticle = true;
			} else {
				if (usingCustomReticle) {
					weaponCursorImage.texture = currentHUDElements.defaultVehicleCursor;
					weaponCursorImage.color = currentHUDElements.defaultVehicleCursorColor;
					weaponCursor.sizeDelta = currentHUDElements.defaultVehicleCursorSize;
				}
			}
		}
	}

	public void checkParableTrayectory (bool parableState)
	{
		// Enable or disable the parable line renderer
		if (currentWeapon.activateLaunchParable && parableState && currentWeapon.launchProjectile) {
			if (parable != null) {
				if (currentWeapon.parableTrayectoryPosition) {
					parableGameObject.transform.position = currentWeapon.parableTrayectoryPosition.position;
					parableGameObject.transform.rotation = currentWeapon.parableTrayectoryPosition.rotation;

					if (currentWeapon.projectilePosition.Count > 0) {
						parable.shootPosition = currentWeapon.projectilePosition [0];
					}
				}

				parable.setMainCameraTransform (vehicleCameraManager.getCurrentCameraTransform ());

				parable.changeParableState (true);
			}
		} else {
			if (parable != null) {
				parable.changeParableState (false);
			}
		}
	}

	/// <summary>
	/// Enable or disable the weapons in the vehicle according to if it is being driven or not.
	/// </summary>
	public void changeWeaponState (bool state)
	{
		if (!weaponsEnabled) {
			return;
		}

		weaponsActivate = state;

		// If the player gets in, set the info in the hud
		if (weaponsActivate) {
			vehicleHUDInfoManager = hudManager.getCurrentVehicleHUDInfo ();

			touchEnabled = actionManager.input.isUsingTouchControls ();

			weaponChanged ();

			// Get player info
			currentDriver = hudManager.getCurrentDriver ();

			if (currentDriver != null) {
				playerComponentsManager mainPlayerComponentsManager = currentDriver.GetComponent<playerComponentsManager> ();

				playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

				mainCamera = playerCameraManager.getMainCamera ();

				usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

				playerInput = mainPlayerComponentsManager.getPlayerInputManager ();
			}

			checkCurrentWeaponCustomReticle ();

			getWeaponCursorElements ();

			checkIfEnableWeaponCursor ();
		}

        // Else, the player is getting off
        else {
			rotationX = 0;
			rotationY = 0;

			if (hasBaseRotation) {
				StartCoroutine (rotateWeapon ());
			}

			// If the laser is being used, disable it
			changeWeaponLaserState (false);
		}

		// Disable the parable line renderer
		checkParableTrayectory (weaponsActivate);

		if (!weaponsActivate) {
			if (locatedEnemies.Count > 0) {
				aimingHomingProjectile = false;

				// Remove the icons in the screen
				removeLocatedEnemiesIcons ();
			}
		}
	}

	/// <summary>
	/// Get the camera info of the vehicle.
	/// </summary>
	public void getCameraInfo (Transform camera, bool useRotationInput)
	{
		vehicleCameraTransform = camera;
		mainCameraTransform = vehicleCameraTransform;
		currentCameraUseRotationInput = useRotationInput;

		getWeaponCursorElements ();
	}

	public void getWeaponCursorElements ()
	{
		if (vehicleHUDInfoManager != null && vehicleHUDInfoManager.getVehicleCursor ()) {
			if (weaponCursor == null) {
				weaponCursor = vehicleHUDInfoManager.getVehicleCursor ().GetComponent<RectTransform> ();

				weaponCursorImage = weaponCursor.GetComponent<RawImage> ();

				currentHUDElements = vehicleHUDInfoManager.getHudElements ();
			}

			resetWeaponCursorPosition ();

			currentLockedCameraCursorSize = weaponCursor.sizeDelta;

			horizontaLimit = Screen.currentResolution.width - currentLockedCameraCursorSize.x;
			verticalLimit = Screen.currentResolution.height - currentLockedCameraCursorSize.y;
		}
	}

	void resetWeaponCursorPosition ()
	{
		if (weaponCursor != null) {
			weaponCursor.anchoredPosition = Vector2.zero;
		}
	}

	/// <summary>
	/// Reset the weapon rotation when the player gets off.
	/// </summary>
	IEnumerator rotateWeapon ()
	{
		Quaternion currentBaseXRotation = baseX.transform.localRotation;
		Quaternion currentBaseYRotation = baseY.transform.localRotation;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			baseX.transform.localRotation = Quaternion.Slerp (currentBaseXRotation, Quaternion.identity, t);
			baseY.transform.localRotation = Quaternion.Slerp (currentBaseYRotation, Quaternion.identity, t);

			yield return null;
		}
	}

	public GameObject setSeekerProjectileInfo (Vector3 shootPosition)
	{
		return applyDamage.setSeekerProjectileInfo (shootPosition, currentWeapon.tagToLocate, usingScreenSpaceCamera,
			currentWeapon.targetOnScreenForSeeker, mainCamera, layer, transform.position, false, null);
	}

	public Vector3 setProjectileSpread ()
	{
		float spreadAmount = currentWeapon.spreadAmount;

		if (spreadAmount > 0) {
			Vector3 randomSpread = Vector3.zero;
			randomSpread.x = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.y = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.z = Random.Range (-spreadAmount, spreadAmount);

			return randomSpread;
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Shoot to any object with the tag configured in the inspector, in case the option is enabled.
	/// </summary>
	public void checkAutoShootOnTag ()
	{
		if (currentWeapon.autoShootOnTag) {
			Vector3 raycastPosition = mainCameraTransform.position;
			Vector3 raycastDirection = mainCameraTransform.TransformDirection (Vector3.forward);

			if (Physics.Raycast (raycastPosition, raycastDirection, out hit, currentWeapon.maxDistanceToRaycast, currentWeapon.layerToAutoShoot)) {

				currentTargetDetectedOnAutoShootOnTag = hit.collider.gameObject;

				if (previousTargetDetectedOnAutoShootOnTag == null || previousTargetDetectedOnAutoShootOnTag != currentTargetDetectedOnAutoShootOnTag) {

					previousTargetDetectedOnAutoShootOnTag = currentTargetDetectedOnAutoShootOnTag;

					GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

					if (target == null) {
						target = hit.collider.gameObject;
					}

					if (currentWeapon.autoShootTagList.Contains (target.tag) ||
					    (currentWeapon.shootAtLayerToo &&
					    (1 << target.layer & currentWeapon.layerToAutoShoot.value) == 1 << target.layer)) {
						autoShootOnTagActive = true;

					} else {
						if (autoShootOnTagActive) {
							resetAutoShootValues ();

							autoShootOnTagActive = false;
						}
					}
				}
			} else {
				resetAutoShootValues ();
			}
		} else {
			resetAutoShootValues ();
		}
	}

	void resetAutoShootValues ()
	{
		if (autoShootOnTagActive) {
			shootWeapon (false);

			previousTargetDetectedOnAutoShootOnTag = null;
			currentTargetDetectedOnAutoShootOnTag = null;

			autoShootOnTagActive = false;
		}
	}

	public bool checkIfWeaponIsAvailable (string weaponName)
	{
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].enabled && weapons [i].Name.Equals (weaponName)) {
				return true;
			}
		}

		return false;
	}

	public bool canIncreaseRemainAmmo (vehicleWeapons weaponToCheck)
	{
		if (weaponToCheck.auxRemainAmmo < weaponToCheck.ammoLimit) {
			return true;
		} else {
			return false;
		}
	}

	public bool hasMaximumAmmoAmount (vehicleWeapons weaponToCheck)
	{
		if (weaponToCheck.useAmmoLimit) {
			print (weaponToCheck.Name + " " + weaponToCheck.remainAmmo + " " + weaponToCheck.ammoLimit);

			if (weaponToCheck.remainAmmo >= weaponToCheck.ammoLimit) {
				return true;
			}

			return false;
		}

		return false;
	}

	public bool hasAmmoLimit (string weaponName)
	{
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].enabled && weapons [i].Name.Equals (weaponName) && weapons [i].useAmmoLimit) {
				return true;
			}
		}

		return false;
	}

	public int ammoAmountToMaximumLimit (vehicleWeapons weaponToCheck)
	{
		return weaponToCheck.ammoLimit - weaponToCheck.auxRemainAmmo;
	}

	public bool hasMaximumAmmoAmount (string weaponName)
	{
		for (int i = 0; i < weapons.Count; i++) {
			if (weapons [i].enabled && weapons [i].Name.Equals (weaponName) && hasMaximumAmmoAmount (weapons [i])) {
				return true;
			}
		}

		return false;
	}

	public string getCurrentWeaponName ()
	{
		if (currentWeaponLocated) {
			return currentWeaponName;
		}

		return "";
	}

	public int getCurrentWeaponClipSize ()
	{
		if (currentWeaponLocated) {
			return currentWeapon.clipSize;
		}

		return 0;
	}

	public int getCurrentWeaponRemainAmmo ()
	{
		if (currentWeaponLocated) {
			return currentWeapon.remainAmmo;
		}

		return 0;
	}

	public projectileInfo setProjectileInfo ()
	{
		projectileInfo newProjectile = new projectileInfo ();

		newProjectile.isHommingProjectile = currentWeapon.isHommingProjectile;

		newProjectile.isSeeker = currentWeapon.isSeeker;
		newProjectile.targetOnScreenForSeeker = currentWeapon.targetOnScreenForSeeker;

		newProjectile.waitTimeToSearchTarget = currentWeapon.waitTimeToSearchTarget;
		newProjectile.useRayCastShoot = currentWeapon.useRayCastShoot;

		newProjectile.useRaycastCheckingOnRigidbody = currentWeapon.useRaycastCheckingOnRigidbody;
		newProjectile.customRaycastCheckingRate = currentWeapon.customRaycastCheckingRate;
		newProjectile.customRaycastCheckingDistance = currentWeapon.customRaycastCheckingDistance;

		newProjectile.useRaycastShootDelay = currentWeapon.useRaycastShootDelay;
		newProjectile.raycastShootDelay = currentWeapon.raycastShootDelay;
		newProjectile.getDelayWithDistance = currentWeapon.getDelayWithDistance;
		newProjectile.delayWithDistanceSpeed = currentWeapon.delayWithDistanceSpeed;
		newProjectile.maxDelayWithDistance = currentWeapon.maxDelayWithDistance;

		newProjectile.useFakeProjectileTrails = currentWeapon.useFakeProjectileTrails;

		newProjectile.projectileDamage = currentWeapon.projectileDamage;
		newProjectile.projectileSpeed = currentWeapon.projectileSpeed;

		newProjectile.impactForceApplied = currentWeapon.impactForceApplied;
		newProjectile.forceMode = currentWeapon.forceMode;
		newProjectile.applyImpactForceToVehicles = currentWeapon.applyImpactForceToVehicles;
		newProjectile.impactForceToVehiclesMultiplier = currentWeapon.impactForceToVehiclesMultiplier;

		newProjectile.projectileWithAbility = currentWeapon.projectileWithAbility;

		newProjectile.impactSoundEffect = currentWeapon.impactSoundEffect;

		newProjectile.scorch = currentWeapon.scorch;
		newProjectile.scorchRayCastDistance = currentWeapon.scorchRayCastDistance;

		newProjectile.owner = vehicle;

		newProjectile.projectileParticles = currentWeapon.projectileParticles;
		newProjectile.impactParticles = currentWeapon.impactParticles;

		newProjectile.isExplosive = currentWeapon.isExplosive;
		newProjectile.isImplosive = currentWeapon.isImplosive;
		newProjectile.useExplosionDelay = currentWeapon.useExplosionDelay;
		newProjectile.explosionDelay = currentWeapon.explosionDelay;
		newProjectile.explosionForce = currentWeapon.explosionForce;
		newProjectile.explosionRadius = currentWeapon.explosionRadius;
		newProjectile.explosionDamage = currentWeapon.explosionDamage;
		newProjectile.pushCharacters = currentWeapon.pushCharacters;
		newProjectile.applyExplosionForceToVehicles = currentWeapon.applyExplosionForceToVehicles;
		newProjectile.explosionForceToVehiclesMultiplier = currentWeapon.explosionForceToVehiclesMultiplier;

		newProjectile.killInOneShot = currentWeapon.killInOneShot;

		newProjectile.useDisableTimer = currentWeapon.useDisableTimer;
		newProjectile.noImpactDisableTimer = currentWeapon.noImpactDisableTimer;
		newProjectile.impactDisableTimer = currentWeapon.impactDisableTimer;

		newProjectile.targetToDamageLayer = layer;
		newProjectile.targetForScorchLayer = targetForScorchLayer;

		newProjectile.useCustomIgnoreTags = useCustomIgnoreTags;
		newProjectile.customTagsToIgnoreList = customTagsToIgnoreList;

		newProjectile.impactDecalIndex = currentWeapon.impactDecalIndex;

		newProjectile.launchProjectile = currentWeapon.launchProjectile;

		newProjectile.adhereToSurface = currentWeapon.adhereToSurface;
		newProjectile.adhereToLimbs = currentWeapon.adhereToLimbs;
		newProjectile.ignoreSetProjectilePositionOnImpact = currentWeapon.ignoreSetProjectilePositionOnImpact;

		newProjectile.useGravityOnLaunch = currentWeapon.useGravityOnLaunch;
		newProjectile.useGraivtyOnImpact = currentWeapon.useGraivtyOnImpact;

		newProjectile.breakThroughObjects = currentWeapon.breakThroughObjects;
		newProjectile.infiniteNumberOfImpacts = currentWeapon.infiniteNumberOfImpacts;
		newProjectile.numberOfImpacts = currentWeapon.numberOfImpacts;
		newProjectile.canDamageSameObjectMultipleTimes = currentWeapon.canDamageSameObjectMultipleTimes;
		newProjectile.forwardDirection = projectileDirectionToLook;

		newProjectile.damageTargetOverTime = currentWeapon.damageTargetOverTime;
		newProjectile.damageOverTimeDelay = currentWeapon.damageOverTimeDelay;
		newProjectile.damageOverTimeDuration = currentWeapon.damageOverTimeDuration;
		newProjectile.damageOverTimeAmount = currentWeapon.damageOverTimeAmount;
		newProjectile.damageOverTimeRate = currentWeapon.damageOverTimeRate;
		newProjectile.damageOverTimeToDeath = currentWeapon.damageOverTimeToDeath;
		newProjectile.removeDamageOverTimeState = currentWeapon.removeDamageOverTimeState;

		newProjectile.sedateCharacters = currentWeapon.sedateCharacters;
		newProjectile.sedateDelay = currentWeapon.sedateDelay;
		newProjectile.useWeakSpotToReduceDelay = currentWeapon.useWeakSpotToReduceDelay;
		newProjectile.sedateDuration = currentWeapon.sedateDuration;
		newProjectile.sedateUntilReceiveDamage = currentWeapon.sedateUntilReceiveDamage;

		newProjectile.pushCharacter = currentWeapon.pushCharacter;
		newProjectile.pushCharacterForce = currentWeapon.pushCharacterForce;
		newProjectile.pushCharacterRagdollForce = currentWeapon.pushCharacterRagdollForce;

		newProjectile.useRemoteEventOnObjectsFound = currentWeapon.useRemoteEventOnObjectsFound;
		newProjectile.remoteEventNameList = currentWeapon.remoteEventNameList;

		newProjectile.useRemoteEventOnObjectsFoundOnExplosion = currentWeapon.useRemoteEventOnObjectsFoundOnExplosion;
		newProjectile.remoteEventNameOnExplosion = currentWeapon.remoteEventNameOnExplosion;

		newProjectile.ignoreShield = currentWeapon.ignoreShield;
		newProjectile.canActivateReactionSystemTemporally = currentWeapon.canActivateReactionSystemTemporally;
		newProjectile.damageReactionID = currentWeapon.damageReactionID;

		newProjectile.damageTypeID = currentWeapon.damageTypeID;

		newProjectile.projectilesPoolEnabled = projectilesPoolEnabled;

		newProjectile.maxAmountOfPoolElementsOnWeapon = maxAmountOfPoolElementsOnWeapon;

		newProjectile.projectileCanBeDeflected = currentWeapon.projectileCanBeDeflected;

		return newProjectile;
	}

	public void enableMuzzleFlashLight ()
	{
		if (!currentWeapon.useMuzzleFlash) {
			return;
		}

		if (muzzleFlashCoroutine != null) {
			StopCoroutine (muzzleFlashCoroutine);
		}

		muzzleFlashCoroutine = StartCoroutine (enableMuzzleFlashCoroutine ());
	}

	IEnumerator enableMuzzleFlashCoroutine ()
	{
		currentWeapon.muzzleFlahsLight.gameObject.SetActive (true);

		yield return new WaitForSeconds (currentWeapon.muzzleFlahsDuration);

		currentWeapon.muzzleFlahsLight.gameObject.SetActive (false);

		yield return null;
	}

	public void setWeaponsPausedState (bool state)
	{
		weaponsPaused = state;
	}

	public void setWeaponsEnabedStateAndWeaponMeshes (bool state)
	{
		setWeaponsEnabledState (state);

		if (mainVehicleWeaponsGameObject != null) {
			mainVehicleWeaponsGameObject.SetActive (state);
		}
	}

	public void setWeaponsEnabledState (bool state)
	{
		weaponsEnabled = state;

		if (weaponsEnabled) {
			initializeComponents ();
		}
	}

	public void setMainVehicleWeaponsGameObjectParent (Transform newParent)
	{
		if (mainVehicleWeaponsGameObject != null) {
			mainVehicleWeaponsGameObject.transform.SetParent (newParent);
		}
	}

	// CALL INPUT FUNCTIONS
	public void inputShootWeapon ()
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			// Fire the current weapon
			if (currentWeapon.automatic) {
				shootWeapon (true);
			}
		}
	}

	public void inputHoldOrReleaseShootWeapon (bool holdingButton)
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			if (holdingButton) {
				shootWeapon (true);
			} else {
				// If the shoot button is released, reset the last shoot timer
				shootWeapon (false);
				lastShoot = 0;

				if (usingLaser) {
					if (currentWeapon.clipSize > 0) {
						changeWeaponLaserState (false);
					}
				}
			}
		}
	}

	public void inputNextOrPreviousWeapon (bool setNextWeapon)
	{
		if (!weaponsEnabled) {
			return;
		}

		if (weaponsActivate) {
			// Select the power using the mouse wheel or the change power buttons
			if (setNextWeapon) {
				chooseNextWeapon ();
			} else {
				choosePreviousWeapon ();
			}
		}
	}

	public void setVehicleWeaponsComponents ()
	{
		// Get every the ammo per clip of every weapon according to their initial clip size
		for (int i = 0; i < weapons.Count; i++) {
			weapons [i].ammoPerClip = weapons [i].clipSize;

			if (weapons [i].weapon != null) {
				weapons [i].weaponAnimation = weapons [i].weapon.GetComponent<Animation> ();
			}

			if (weapons [i].startWithEmptyClip) {
				weapons [i].clipSize = 0;
			}

			weapons [i].auxRemainAmmo = weapons [i].remainAmmo;
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	public void getImpactListInfo ()
	{
		if (impactDecalManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainDecalManagerName, typeof(decalManager));

			impactDecalManager = FindObjectOfType<decalManager> ();
		}

		if (impactDecalManager != null) {
			impactDecalList = new string[impactDecalManager.impactListInfo.Count + 1];

			for (int i = 0; i < impactDecalManager.impactListInfo.Count; i++) {
				string name = impactDecalManager.impactListInfo [i].name;
				impactDecalList [i] = name;
			}

			updateComponent ();
		}
	}

	[System.Serializable]
	public class vehicleWeapons
	{
		public string Name;
		public int numberKey;

		public bool useCustomReticle;
		public Texture customReticle;
		public bool useCustomReticleColor;
		public Color customReticleColor = Color.white;
		public bool useCustomReticleSize;
		public Vector2 customReticleSize = new Vector2 (60, 60);

		public bool disableWeaponCursorWhileNotShooting;
		public float delayToDisableWeaponCursor;

		public bool useRayCastShoot;
		public bool useRaycastAllToCheckSurfaceFound = true;
		public float maxDistanceToRaycastAll = 200;

		public bool useRaycastCheckingOnRigidbody;

		public float customRaycastCheckingRate;

		public float customRaycastCheckingDistance = 0.1f;

		public bool fireWeaponForward;
		public bool enabled;

		public bool useRaycastShootDelay;
		public float raycastShootDelay;
		public bool getDelayWithDistance;
		public float delayWithDistanceSpeed;
		public float maxDelayWithDistance;

		public bool useFakeProjectileTrails;

		public int ammoPerClip;
		public bool removePreviousAmmoOnClip;
		public bool infiniteAmmo;
		public int remainAmmo;
		public int clipSize;
		public bool startWithEmptyClip;
		public bool autoReloadWhenClipEmpty = true;
		public bool useAmmoLimit;
		public int ammoLimit;
		public int auxRemainAmmo;

		public bool shootAProjectile;
		public bool launchProjectile;

		public bool projectileWithAbility;

		public bool automatic;

		public float fireRate;
		public float reloadTime;
		public float projectileDamage;
		public float projectileSpeed;
		public int projectilesPerShoot;

		public bool useProjectileSpread;
		public float spreadAmount;

		public bool isImplosive;
		public bool isExplosive;
		public float explosionForce;
		public float explosionRadius;
		public bool useExplosionDelay;
		public float explosionDelay;
		public float explosionDamage;
		public bool pushCharacters;
		public bool applyExplosionForceToVehicles = true;
		public float explosionForceToVehiclesMultiplier = 0.2f;

		public List<Transform> projectilePosition = new List<Transform> ();

		public bool ejectShellOnShot;
		public GameObject shell;
		public List<Transform> shellPosition = new List<Transform> ();
		public float shellEjectionForce = 200;
		public List<AudioClip> shellDropSoundList = new List<AudioClip> ();

		public GameObject weapon;
		public string animation;
		public Animation weaponAnimation;
		public GameObject scorch;
		public float scorchRayCastDistance;

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		public bool applyForceAtShoot;
		public Vector3 forceDirection;
		public bool useCustomTransformToForceShoot;
		public Transform customTransformToForceShoot;
		public float forceAmount;

		public bool isHommingProjectile;
		public bool isSeeker;
		public bool targetOnScreenForSeeker = true;
		public float waitTimeToSearchTarget;

		public float impactForceApplied;
		public ForceMode forceMode;
		public bool applyImpactForceToVehicles;
		public float impactForceToVehiclesMultiplier = 1;

		public float forceMassMultiplier = 1;

		public AudioClip reloadSoundEffect;
		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;
		public AudioClip outOfAmmo;

		public GameObject muzzleParticles;
		public GameObject projectileParticles;
		public GameObject impactParticles;

		public bool killInOneShot;

		public bool useDisableTimer;
		public float noImpactDisableTimer;
		public float impactDisableTimer;

		public string locatedEnemyIconName = "Homing Located Enemy";
		public List<string> tagToLocate = new List<string> ();

		public bool activateLaunchParable;
		public bool useParableSpeed;
		public Transform parableTrayectoryPosition;
		public Transform parableDirectionTransform;
		public bool useMaxDistanceWhenNoSurfaceFound;
		public float maxDistanceWhenNoSurfaceFound;

		public bool adhereToSurface;
		public bool adhereToLimbs;

		public bool ignoreSetProjectilePositionOnImpact;

		public bool useGravityOnLaunch;
		public bool useGraivtyOnImpact;

		public GameObject projectileModel;

		public GameObject projectileToShoot;

		public bool showShakeSettings;
		public vehicleCameraController.shakeSettingsInfo shootShakeInfo;

		public int impactDecalIndex;
		public string impactDecalName;

		public bool isLaser;

		public bool breakThroughObjects;
		public bool infiniteNumberOfImpacts;
		public int numberOfImpacts;
		public bool canDamageSameObjectMultipleTimes;

		public bool useMuzzleFlash;
		public Light muzzleFlahsLight;
		public float muzzleFlahsDuration;

		public bool damageTargetOverTime;
		public float damageOverTimeDelay;
		public float damageOverTimeDuration;
		public float damageOverTimeAmount;
		public float damageOverTimeRate;
		public bool damageOverTimeToDeath;
		public bool removeDamageOverTimeState;

		public bool sedateCharacters;
		public float sedateDelay;
		public bool useWeakSpotToReduceDelay;
		public bool sedateUntilReceiveDamage;
		public float sedateDuration;

		public bool pushCharacter;
		public float pushCharacterForce;
		public float pushCharacterRagdollForce;

		public bool useRemoteEventOnObjectsFound;
		public List<string> remoteEventNameList = new List<string> ();

		public bool useRemoteEventOnObjectsFoundOnExplosion;
		public string remoteEventNameOnExplosion;

		public bool ignoreShield;

		public bool canActivateReactionSystemTemporally;
		public int damageReactionID = -1;

		public int damageTypeID = -1;

		public bool projectileCanBeDeflected = true;
	}
}