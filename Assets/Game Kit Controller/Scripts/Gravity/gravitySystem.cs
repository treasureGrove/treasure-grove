using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class gravitySystem : gravityObjectManager
{
	public bool gravityPowerEnabled;
	public bool gravityPowerInputEnabled;

	public Transform gravityCenter;
	public GameObject playerCameraGameObject;

	public bool liftToSearchEnabled;
	public bool randomRotationOnAirEnabled;
	public GameObject cursor;
	public LayerMask layer;
	public float searchSurfaceSpeed = 5;
	public float accelerateSpeed = 20;
	public float airControlSpeed = 5;

	bool cursorActive;
	bool cursorLocated;

	public bool preserveVelocityWhenDisableGravityPower;

	float extraMultiplierPreserveVelocity;

	public float highGravityMultiplier;
	public List<bool> materialToChange = new List<bool> ();
	public bool changeModelColor;
	public Color powerColor;
	public float hoverSpeed;
	public float hoverAmount;
	public float hoverSmooth;
	public GameObject arrow;

	bool arrowLocated;

	public bool powerActivated;
	public bool recalculatingSurface;
	public bool searchingSurface;
	public bool searchingNewSurfaceBelow;
	public bool searchAround;

	public Vector3 currentNormal = new Vector3 (0, 1, 0);
	public bool dead;
	public bool firstPersonView;
	public Renderer playerRenderer;
	public bool usedByAI;

	public string tagForCircumnavigate = "sphere";
	public string tagForMovingObjects = "moving";

	public float rotateToSurfaceSpeed = 2;
	public float rotateToRegularGravitySpeed = 2;

	public Vector3 regularGravity = new Vector3 (0, 1, 0);

	public bool gravityPowerActive;

	public bool startWithNewGravity;
	public bool usePlayerRotation;
	public bool adjustRotationToSurfaceFound;
	public Vector3 newGravityToStart;

	public bool zeroGravityModeOn;
	public bool startWithZeroGravityMode;
	public bool canResetRotationOnZeroGravityMode;
	public bool canAdjustToForwardSurface;
	public Transform forwardSurfaceRayPosition;
	public float maxDistanceToAdjust;
	public float resetRotationZeroGravitySpeed;
	public float adjustToForwardSurfaceSpeed;

	public bool canActivateFreeFloatingMode;
	public bool freeFloatingModeOn;

	public bool useEventsOnFreeFloatingModeStateChange;
	public UnityEvent evenOnFreeFloatingModeStateEnabled;
	public UnityEvent eventOnFreeFloatingModeStateDisabled;

	public bool useEventsOnZeroGravityModeStateChange;
	public UnityEvent evenOnZeroGravityModeStateEnabled;
	public UnityEvent eventOnZeroGravityModeStateDisabled;

	public bool useEventsOnUseGravityPowerStateChange;
	public UnityEvent eventsOnUseGravityPowerStateEnabled;
	public UnityEvent eventsOnUseGravityPowerStateDisabled;

	public float circumnavigateRotationSpeed = 10;
	public bool circumnavigateCurrentSurfaceActive;
	public bool useLerpRotation;

	public Transform gravityAdherenceRaycastParent;

	public bool checkSurfaceBelowLedge;
	public float surfaceBelowLedgeRaycastDistance = 3;
	public float belowLedgeRotationSpeed = 10;
	public Transform surfaceBelowRaycastTransform;

	public bool checkSurfaceInFront;
	public float surfaceInFrontRaycastDistance = 0.5f;
	public float surfaceInFrontRotationSpeed = 10;
	public Transform surfaceInFrontRaycastTransform;

	public bool checkCircumnavigateSurfaceOnZeroGravity = true;

	public bool searchNewSurfaceOnHighFallSpeed = true;
	public float minSpeedToSearchNewSurface = 15;

	bool searchNewSurfaceOnHighFallSpeedPaused;

	public bool pauseSearchNewSurfaceOnHighFallSpeedOnReverseGravityInput = true;

	public bool shakeCameraOnHighFallSpeed = true;
	public float minSpeedToShakeCamera = 15;
	bool cameraShakeCanBeUsed = true;

	public bool checkSurfaceBelowOnRegularState;
	public float timeToSetNullParentOnAir = 0.5f;
	float lastTimeParentFound;

	public bool stopAimModeWhenSearchingSurface;

	public Transform pivotCameraTransform;
	public SphereCollider gravityCenterCollider;
	public playerController playerControllerManager;
	public otherPowers powers;
	public CapsuleCollider playerCollider;
	public playerInputManager playerInput;

	public playerWeaponsManager weaponsManager;
	public Rigidbody mainRigidbody;
	public playerCamera playerCameraManager;
	public Transform mainCameraTransform;
	public grabObjects grabObjectsManager;

	public Vector3 debugGravityDirection;

	public bool hovering;
	public bool turning;

	public bool checkGravityArrowStateActive = true;


	public float raycastDistanceToCheckBelowPlayer = 10;

	public bool useInfiniteRaycastDistanceToCheckBelowPlayer;

	zeroGravityRoomSystem currentZeroGravityRoom;

	Transform playerCameraTransform;

	Vector3 currentPlayerPosition;
	Quaternion arrowRotation;
	Color originalPowerColor;
	bool accelerating;

	bool rotating = false;

	Vector3 gravityDirection;
	Vector3 rightAxis;
	float timer = 0.75f;
	float rotateAmount = 40;
	float normalGravityMultiplier;
	Vector3 surfaceNormal;
	Vector3 turnDirection;

	bool playerIsChildOfParentActive;
	GameObject fatherDetected;
	bool circumnavigableSurfaceFound;

	RaycastHit hit;
	Ray ray;

	public bool choosingDirection;
	bool onGround;
	bool lifting;

	Coroutine rotateCharacterState;
	Coroutine rotateToSurfaceState;

	Transform externalGravityCenter;

	Transform currentSurfaceBelowPlayer;
	Transform previousSurfaceBelowPlayer;

	Vector3 currentPosition;

	bool preservingVelocity;
	Vector3 previousRigidbodyVelocity;

	bool playerInsideGravityRoom;

	Vector3 rayPosition;
	Vector3 rayDirection;

	Vector3 currentRotatingNormal;
	Coroutine colorCoroutine;

	public setGravity currentSetGravityManager;

	bool surfaceFound;
	RaycastHit currentSurfaceFound;

	float currentCircumnagivateRotationSpeed;

	Vector2 axisValues;
	float verticalInput;
	float horizontalInput;
	Vector3 moveInput;
	float gravityAdherenceRaycastParentAngleRotation;

	Coroutine zeroGravityModeRotateCharacterCoroutine;
	Coroutine teleportCoroutine;

	Coroutine freeFloatingModeCoroutine;

	bool gravityArrowCurrentlyActive;

	Transform playerTransform;

	bool teleportInProcess;

	//Editor variables
	public bool showCircumnavigationhSettings;
	public bool showZeroGravitySettings;
	public bool showFreeFloatingModeSettings;
	public bool showEventsSettings;
	public bool showOtherSettings;
	public bool showDebugSettings;
	public bool showAllSettings;
	public bool showComponents;

	public bool showGizmo;

	Vector3 vector3Zero = Vector3.zero;
	Quaternion quaternionIdentity = Quaternion.identity;

	void Awake ()
	{
		normalGravityMultiplier = playerControllerManager.getGravityMultiplier ();

		playerTransform = transform;
	}

	void Start ()
	{
		//get the pivot of the camera
		playerCameraTransform = playerCameraGameObject.transform;

		mainRigidbody.freezeRotation = true; 

		//the gravity center has a sphere collider, that surrounds completly the player to use it when the player searchs a new surface, 
		//detecting the collision with any object to rotate the player to that surface
		//this is done like this to avoid that the player cross a collider when he moves while he searchs a new surface
		//get all the neccessary components in the player

		//get the original value of some parameters
		originalPowerColor = powerColor;

		GameObject newGravityCenter = new GameObject ();
		externalGravityCenter = newGravityCenter.transform;
		externalGravityCenter.name = "External Gravity Center";
		externalGravityCenter.SetParent (playerTransform);

		if (startWithNewGravity) {
			if (usePlayerRotation) {
				Vector3 normalDirection = -playerTransform.up;

				if (adjustRotationToSurfaceFound) {
					if (Physics.Raycast (playerTransform.position, -playerTransform.up, out hit, Mathf.Infinity, layer)) {
						normalDirection = hit.normal;
					}
				}
					
				if (playerTransform.up != currentNormal) {
					setNormal (normalDirection);
				}
			} else {
				setNormal (newGravityToStart);
			}
		}

		if (startWithZeroGravityMode) {
			setZeroGravityModeOnState (true);
		}

		currentRotatingNormal = currentNormal;

		if (arrow != null) {
			arrowLocated = true;

			if (checkGravityArrowStateActive) {
				if (arrow.activeSelf) {
					arrow.SetActive (false);
				}
			} else {
				gravityArrowCurrentlyActive = true;
			}
		}

		if (cursor != null) {
			cursorLocated = true;
		}
	}

	void Update ()
	{
		currentPosition = playerTransform.position;

		checkGravityArrowState ();

		//elevate the player above the ground when the gravity power is enabled and the player was in the ground before it
		if (lifting) {
			bool surfaceAbove = false;
			//check if there is any obstacle above the player while he is being elevated, to prevent he can cross any collider
			ray = new Ray (currentPosition + playerTransform.up * 1.5f, playerTransform.up);

			if (Physics.SphereCast (ray, 0.4f, out hit, 0.5f, layer)) {
				surfaceAbove = true;
			} else {
				//if the ray doesn't found any surface, keep lifting the player until the timer reachs its target value
				timer -= Time.deltaTime;

				playerTransform.Translate (Vector3.up * (Time.deltaTime * 4));

				playerCameraTransform.Translate (Vector3.up * (Time.deltaTime * 4));
			}

			//if the timer ends or a surface is found, stop the lifting and start rotate the player to float in the air
			if (surfaceAbove || timer < 0) {
				lifting = false;

				timer = 0.75f;

				searchingSurface = false;

				searchAround = false;

				if (canRotatePlayer ()) {
					rotateMeshPlayer ();
				}

				setHoverState (true);
			}
		}	

		//moving in the air with the power gravity activated looking for a new surface
		if (searchingSurface) {
			//parameters to store the position and the direction of the raycast that checks any close surface to the player
			rayPosition = vector3Zero;
			rayDirection = vector3Zero;

			//set the size of the ray
			float rayDistance = 0;

			//if the player has set the direction of the air movement, the raycast starts in camera pivot position
			//else the player is falling and reach certain amount of velocity, so the next surface that he will touch becomes in his new ground
			//and the raycast starts in the player position
			if (!searchingNewSurfaceBelow) {
				rayDistance = 2;

				Vector3 newVelocity = gravityDirection * (Mathf.Abs (playerControllerManager.getGravityForce ()) * mainRigidbody.mass * searchSurfaceSpeed);

				//when the player searchs a new surface using the gravity force, the player can moves like when he falls
				if (!playerControllerManager.isPlayerRunning ()) {
					//get the global input and convert it to the local direction, using the axis in the changegravity script
					Vector3 forwardAxis = Vector3.Cross (gravityDirection, rightAxis);
					Vector3 newMoveInput = playerControllerManager.getVerticalInput () * forwardAxis + playerControllerManager.getHorizontalInput () * rightAxis;

					if (newMoveInput.magnitude > 1) {
						newMoveInput.Normalize ();
					}

					if (newMoveInput.magnitude > 0) {
						newVelocity += newMoveInput * airControlSpeed;
					}
				}

				//apply and extra force if the player increase his movement
				if (accelerating) {
					newVelocity += gravityDirection * accelerateSpeed;
				}

				//make a lerp of the velocity applied to the player to move him smoothly
				mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, newVelocity, Time.deltaTime * 2);

				//set the direction of the ray that checks any surface
				rayPosition = pivotCameraTransform.position;
				rayDirection = gravityDirection;

				if (currentNormal == regularGravity) {
					rayDistance = GKC_Utils.distance (rayPosition, currentPosition) + 0.5f;
				}
			} 

			//else, the player is falling in his ground direction, so the ray to check a new surface is below his feet
			else {    
				rayPosition = currentPosition;
				rayDirection = -playerTransform.up;
				rayDistance = 0.6f;
			}

			if (showGizmo) {
				//launch a raycast to check any surface
				Debug.DrawRay (rayPosition, rayDirection * rayDistance, Color.yellow);
			}

			if (Physics.Raycast (rayPosition, rayDirection, out hit, rayDistance, layer)) {
				//if the object detected has not trigger and rigidbody, then
				if (!hit.collider.isTrigger && hit.rigidbody == null) {
					//disable the search of the surface and rotate the player to that surface

					playerControllerManager.setGravityPowerActiveState (false);

					playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

					setPowerActivateState (false);

					searchingNewSurfaceBelow = false;
					searchingSurface = false;
					searchAround = false;
					mainRigidbody.velocity = vector3Zero;

					//disable the collider in the gravity center and enable the capsule collider in the player
					gravityCenterCollider.enabled = false;
					playerCollider.isTrigger = false;

					//check if the object detected can be circumnavigate
					if (hit.collider.gameObject.CompareTag (tagForCircumnavigate)) {
						circumnavigableSurfaceFound = true;
					}

					//check if the object is moving to parent the player inside it
					if (hit.collider.gameObject.CompareTag (tagForMovingObjects)) {
						addParent (hit.collider.gameObject);
					}	

					//set the camera in its regular position
					playerCameraManager.changeCameraFov (false);

					//if the new normal is different from the previous normal in the gravity power, then rotate the player
					if (hit.normal != currentNormal) {
						checkRotateToSurface (hit.normal, rotateToSurfaceSpeed); 
					}

					//if the player back to the regular gravity value, change its color to the regular state
					if (hit.normal == regularGravity) {
						changeColor (false);
					}	
				}
			}

			if (!turning && canRotatePlayer ()) {
				rotateMeshPlayer ();
			}
		}

		//if the player falls and reachs certain velocity, the camera shakes and the mesh of the player rotates
		//also, if the gravity power is activated, look a new surface to change the gravity to the found surface
		if (!freeFloatingModeOn &&
		    !zeroGravityModeOn &&
		    !onGround &&
		    !searchingNewSurfaceBelow &&
		    !choosingDirection &&
		    !powerActivated &&
		    !playerControllerManager.isFlyingActive () &&
		    !playerControllerManager.isSwimModeActive () &&
		    !playerControllerManager.isPauseCameraShakeFromGravityActive ()) { 

			if (shakeCameraOnHighFallSpeed && playerTransform.InverseTransformDirection (mainRigidbody.velocity).y < -minSpeedToShakeCamera) {
				if (!playerControllerManager.isSlowFallExternallyActive ()) {

					enableCameraShake ();

					if (!weaponsManager.isCarryingWeaponInThirdPerson ()) {
						rotateMeshPlayer ();
					}
				}
			}

			if (searchNewSurfaceOnHighFallSpeed &&
			    !searchNewSurfaceOnHighFallSpeedPaused &&
			    playerTransform.InverseTransformDirection (mainRigidbody.velocity).y < -minSpeedToSearchNewSurface) {

				//if the gravity of the player is different from the regular gravity, start searchin the new surface
				if (currentNormal != regularGravity) {
					searchingNewSurfaceBelow = true;
					searchingSurface = true;
					recalculatingSurface = false;
					circumnavigableSurfaceFound = false;
				}
			}
		}

		//walk in spheres and moving objects, recalculating his new normal and lerping the player to the new rotation
		if (!lifting && !searchingSurface && (circumnavigableSurfaceFound || playerIsChildOfParentActive) && recalculatingSurface && !rotating) {
			float rayDistance = 0.5f;

			if (!onGround) {
				if (!freeFloatingModeOn && !zeroGravityModeOn) {
					rayDistance = raycastDistanceToCheckBelowPlayer;

					if (useInfiniteRaycastDistanceToCheckBelowPlayer) {
						rayDistance = 1000;
					}
				}
			}

			surfaceFound = false;

			//get the normal direction of the object below the player, to recalculate the rotation of the player
			if (Physics.Raycast (currentPosition + playerTransform.up * 0.1f, -playerTransform.up, out hit, rayDistance, layer)) {
				currentSurfaceFound = hit;
				surfaceFound = true;
				currentCircumnagivateRotationSpeed = circumnavigateRotationSpeed;
			} 

			//Get the correct raycast orientation according to input, for example, it is no the same ray position and direction if the player is walking forward or backward on 
			//first person or if he is aiming
			if (checkSurfaceBelowLedge || checkSurfaceInFront) {
				rayPosition = currentPosition + playerTransform.up * 0.1f;

				if (playerControllerManager.isLookingInCameraDirection ()) {

					axisValues = playerInput.getPlayerRawMovementAxis ();
					verticalInput = axisValues.y;
					horizontalInput = axisValues.x;

					rayPosition += playerTransform.forward * (verticalInput * 0.2f);
					rayPosition += playerTransform.right * (horizontalInput * 0.2f);

					moveInput = (verticalInput * playerTransform.forward + horizontalInput * playerTransform.right);	

					gravityAdherenceRaycastParentAngleRotation = Vector3.SignedAngle (playerTransform.forward, moveInput, playerTransform.up);

					if (verticalInput == 0 && horizontalInput == 0) {
						gravityAdherenceRaycastParentAngleRotation = 0;
					}

				} else {
					rayPosition += playerTransform.forward * 0.2f;
					gravityAdherenceRaycastParentAngleRotation = 0;
				}

				gravityAdherenceRaycastParent.localRotation = Quaternion.Euler (new Vector3 (0, gravityAdherenceRaycastParentAngleRotation, 0));
			}

			if (checkSurfaceBelowLedge) {
				rayDirection = -playerTransform.up;

				if (showGizmo) {
					Debug.DrawRay (rayPosition, rayDirection, Color.white);
				}

				if (!Physics.Raycast (rayPosition, rayDirection, out hit, 1, layer)) {
					if (showGizmo) {
						Debug.DrawRay (rayPosition, rayDirection, Color.green);
					}

					rayPosition = surfaceBelowRaycastTransform.position;
					rayDirection = surfaceBelowRaycastTransform.forward;

					if (Physics.Raycast (rayPosition, rayDirection, out hit, surfaceBelowLedgeRaycastDistance, layer)) {
						if (showGizmo) {
							Debug.DrawRay (rayPosition, rayDirection * hit.distance, Color.yellow);
						}

						currentSurfaceFound = hit;
						surfaceFound = true;
						currentCircumnagivateRotationSpeed = belowLedgeRotationSpeed;
					} else {
						if (showGizmo) {
							Debug.DrawRay (rayPosition, rayDirection * surfaceBelowLedgeRaycastDistance, Color.red);
						}
					}
				} else {
					if (showGizmo) {
						Debug.DrawRay (rayPosition, rayDirection * hit.distance, Color.red);
					}
				}
			}

			if (checkSurfaceInFront) {
				rayPosition = surfaceInFrontRaycastTransform.position;
				rayDirection = surfaceInFrontRaycastTransform.forward;

				if (Physics.Raycast (rayPosition, rayDirection, out hit, surfaceInFrontRaycastDistance, layer)) {
					currentSurfaceFound = hit;
					surfaceFound = true;
					currentCircumnagivateRotationSpeed = surfaceInFrontRotationSpeed;
				}
			}

			if (surfaceFound) {
				if (!currentSurfaceFound.collider.isTrigger && currentSurfaceFound.rigidbody == null) {
					//the object detected can be circumnavigate, so get the normal direction
					if (currentSurfaceFound.collider.gameObject.CompareTag (tagForCircumnavigate)) {
						surfaceNormal = currentSurfaceFound.normal;
					}

					//the object is moving, so get the normal direction and set the player as a children of the moving obejct
					else if (currentSurfaceFound.collider.gameObject.CompareTag (tagForMovingObjects)) {
						surfaceNormal = currentSurfaceFound.normal;

						if (!playerIsChildOfParentActive) {
							addParent (currentSurfaceFound.collider.gameObject);
						}
					} 
				}
			} else {
				if (playerIsChildOfParentActive) {
					removeParent ();
				}
			}

			if ((!zeroGravityModeOn && !freeFloatingModeOn) || onGround) {
				if (useLerpRotation) {
					//recalculate the rotation of the player and the camera according to the normal of the surface under the player
					currentNormal = Vector3.Lerp (currentNormal, surfaceNormal, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForward = Vector3.Cross (playerTransform.right, currentNormal);
					Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 

					playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, dstRot, currentCircumnagivateRotationSpeed * Time.deltaTime);

					Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, currentNormal);
					Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);

					playerCameraTransform.rotation = Quaternion.Lerp (playerCameraTransform.rotation, dstRotCamera, currentCircumnagivateRotationSpeed * Time.deltaTime);
				} else {
					currentNormal = Vector3.Slerp (currentNormal, surfaceNormal, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForward = Vector3.Cross (playerTransform.right, currentNormal);
					Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 

					playerTransform.rotation = Quaternion.Slerp (playerTransform.rotation, dstRot, currentCircumnagivateRotationSpeed * Time.deltaTime);

					Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, currentNormal);
					Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);

					playerCameraTransform.rotation = Quaternion.Slerp (playerCameraTransform.rotation, dstRotCamera, currentCircumnagivateRotationSpeed * Time.deltaTime);
				}

				updateCurrentRotatingNormal (currentNormal);
			}

			//set the normal in the playerController component
			playerControllerManager.setCurrentNormalCharacter (currentNormal);
		}

		if (zeroGravityModeOn && !onGround && !rotating) {
			currentNormal = playerTransform.up;

			playerControllerManager.setCurrentNormalCharacter (currentNormal);

			updateCurrentRotatingNormal (currentNormal);
		}

		//set a cursor in the screen when the character can choose a direction to change his gravity
		if (!usedByAI) {
			if (cursorLocated) {
				if (choosingDirection) {
					if (!cursorActive) {
						cursor.SetActive (true);

						cursorActive = true;
					}
				}

				if (!choosingDirection) {
					if (cursorActive) {
						cursor.SetActive (false);

						cursorActive = false;
					}
				}
			}
		}

		//if the player can choosed a direction, lerp his velocity to zero
		if (choosingDirection) {
			mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, vector3Zero, Time.deltaTime * 2);
		}

		if (rotating && !playerControllerManager.isPlayerRunning ()) {
			mainRigidbody.velocity = vector3Zero;
		}

		if ((gravityPowerActive || circumnavigateCurrentSurfaceActive || (zeroGravityModeOn && checkCircumnavigateSurfaceOnZeroGravity)) && !powerActivated) {
			currentSurfaceBelowPlayer = playerControllerManager.getCurrentSurfaceBelowPlayer ();
	
			if (currentSurfaceBelowPlayer != null) {
				//check if the object detected can be circumnavigate
				if (currentSurfaceBelowPlayer.CompareTag (tagForCircumnavigate)) {
					circumnavigableSurfaceFound = true;
				} else {
					circumnavigableSurfaceFound = false;

					if (checkSurfaceInFront) {
						rayPosition = surfaceInFrontRaycastTransform.position;
						rayDirection = surfaceInFrontRaycastTransform.forward;

						if (Physics.Raycast (rayPosition, rayDirection, out hit, surfaceInFrontRaycastDistance, layer)) {
							currentSurfaceBelowPlayer = hit.collider.transform;

							if (currentSurfaceBelowPlayer.CompareTag (tagForCircumnavigate)) {
								circumnavigableSurfaceFound = true;
							}
						}
					}
				}

				//check if the object is moving to parent the player inside it
				if (currentSurfaceBelowPlayer.CompareTag (tagForMovingObjects)) {
					if (previousSurfaceBelowPlayer != currentSurfaceBelowPlayer) {
						previousSurfaceBelowPlayer = currentSurfaceBelowPlayer;

						addParent (currentSurfaceBelowPlayer.gameObject);
					}
				} else if (playerIsChildOfParentActive && !currentSurfaceBelowPlayer.IsChildOf (fatherDetected.transform)) {
					removeParent ();
				}

				//if the surface where the player lands can be circumnavigated or an moving/rotating object, then keep recalculating the player throught the normal surface
				if (circumnavigableSurfaceFound || playerIsChildOfParentActive) {
					recalculatingSurface = true;
				} 
				//else disable this state
				else {
					recalculatingSurface = false;
				}
			} else {
				if (previousSurfaceBelowPlayer) {
					previousSurfaceBelowPlayer = null;
				}

				if (rotating) {
					circumnavigableSurfaceFound = false;
				}
			}
		} else {
			if ((circumnavigableSurfaceFound || playerIsChildOfParentActive) && rotating) {
				circumnavigableSurfaceFound = false;
			}

			if (checkSurfaceBelowOnRegularState && !gravityPowerActive && !zeroGravityModeOn && !playerControllerManager.isPlayerSetAsChildOfParent ()) {

				currentSurfaceBelowPlayer = playerControllerManager.getCurrentSurfaceBelowPlayer ();

				if (currentSurfaceBelowPlayer != null) {
					//check if the object is moving to parent the player inside it
					if (previousSurfaceBelowPlayer != currentSurfaceBelowPlayer) {
						previousSurfaceBelowPlayer = currentSurfaceBelowPlayer;

						if (currentSurfaceBelowPlayer.CompareTag (tagForMovingObjects)) {	
							addParent (currentSurfaceBelowPlayer.gameObject);

							lastTimeParentFound = Time.time;
						
						} else if (playerIsChildOfParentActive && !currentSurfaceBelowPlayer.IsChildOf (fatherDetected.transform)) {
							removeParent ();
						}
					}
				} else {
					if (previousSurfaceBelowPlayer != null) {
						previousSurfaceBelowPlayer = null;
					}

					if (Time.time > timeToSetNullParentOnAir + lastTimeParentFound) {
						if (playerIsChildOfParentActive) {
							removeParent ();
						}
					}
				}
			} else {
				if (currentSurfaceBelowPlayer != null) {
					currentSurfaceBelowPlayer = null;
				}

				if (previousSurfaceBelowPlayer != null) {
					previousSurfaceBelowPlayer = null;
				}
			}
		}
	}

	//rotate randomly the mesh of the player in the air, also make that mesh float while chooses a direction in the air
	void FixedUpdate ()
	{
		if (turning) {
			if (randomRotationOnAirEnabled || powerActivated) {
				gravityCenter.transform.Rotate (turnDirection * (rotateAmount * Time.deltaTime));
			}

			if (weaponsManager.isCarryingWeaponInThirdPerson () || powers.isAimingPowerInThirdPerson ()) {
				turning = false;

				checkRotateCharacter (vector3Zero);
			}
		}

		if (hovering) {
			float posTargetY = Mathf.Sin (Time.time * hoverSpeed) * hoverAmount;
			mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, mainRigidbody.position + posTargetY * playerTransform.up, Time.deltaTime * hoverSmooth);
		}
	}

	void checkGravityArrowState ()
	{
		//the arrow in the back of the player looks to the direction of the real gravity
		if (arrowLocated) {
			if (gravityArrowCurrentlyActive) {
				currentPlayerPosition = new Vector3 (currentPosition.x, 0, currentPosition.z);

				if (currentPlayerPosition != vector3Zero) {
					arrowRotation = Quaternion.LookRotation (currentPlayerPosition);

					if (arrowRotation.eulerAngles != vector3Zero) {
						arrow.transform.rotation = arrowRotation;
					}
				}

				if (checkGravityArrowStateActive) {
					if (!powerActivated && !searchingSurface && currentNormal == regularGravity) {
						gravityArrowCurrentlyActive = false;
						arrow.SetActive (false);
					}
				}
			} else {
				if (powerActivated || searchingSurface || currentNormal != regularGravity) {
					gravityArrowCurrentlyActive = true;
					arrow.SetActive (true);
				}
			}
		}
	}

	public void checkOnGroundOrAirState ()
	{
		onGroundOrOnAir (playerControllerManager.isPlayerOnGround ());
	}

	//playerController set the values of ground in this script and in the camera code
	public override void onGroundOrOnAir (bool state)
	{
		onGround = state;

		if (onGround) {
			//the player is on the ground
			//set the states in the camera, on ground, stop any shake of the camera, and back the camera to its regular position if it has been moved
			playerCameraManager.onGroundOrOnAir (true);

			disableCameraShake ();

			playerCameraManager.changeCameraFov (false);

			//stop rotate the player
			turning = false;

			//if the surface where the player lands can be circumnavigated or an moving/rotating object, then keep recalculating the player throught the normal surface
			if ((circumnavigableSurfaceFound || playerIsChildOfParentActive) && (gravityPowerActive || circumnavigateCurrentSurfaceActive || (zeroGravityModeOn && checkCircumnavigateSurfaceOnZeroGravity))) {
				recalculatingSurface = true;
			} 

			//else disable this state
			else {
				recalculatingSurface = false;
			}

			//set the gravity force applied to the player to its regular state
			playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

			//set the model rotation to the regular state
			checkRotateCharacter (vector3Zero);

			accelerating = false;

			if (currentNormal != regularGravity) {
				gravityPowerActive = true;
			}
		} else {
			//the player is on the air
			playerCameraManager.onGroundOrOnAir (false);
		}
	}

	//when the player searchs a new surface using the gravity power on button, check the collisions in the gravity center sphere collider, to change the
	//gravity of the player to the detected normal direction
	void OnCollisionEnter (Collision collision)
	{
		//check that the player is searchin a surface, the player is not running, and that he is searching around
		if (searchingSurface && turning && searchAround && !playerControllerManager.isPlayerRunning ()) {
			//check that the detected object is not a trigger or the player himself
			if (!collision.gameObject.CompareTag ("Player") && collision.rigidbody == null && !collision.collider.isTrigger) {
				//get the collision contant point to change the direction of the ray that searchs a new direction, setting the direction from the player 
				//to the collision point as the new direction
				Vector3 hitDirection = collision.contacts [0].point - pivotCameraTransform.position;
				hitDirection = hitDirection / hitDirection.magnitude;
				gravityDirection = hitDirection;

				searchAround = false;
			}
		}
	}

	//now the gravity power is in a function, so it can be called from keyboard and a touch button
	public void activateGravityPower ()
	{
		gravityPowerActive = true;

//		print ("activate gravity power");

		//if the option to lift the player when he uses the gravity power is disable, then searchs an new surface in the camera direction 
		if (!liftToSearchEnabled) {
			changeOnTrigger (mainCameraTransform.TransformDirection (Vector3.forward), mainCameraTransform.TransformDirection (Vector3.right));
		} 

		//else lift the player, and once that he has been lifted, then press again the gravit power on button to search an new surface
		//or disable the gravity power
		else {
			//enable the sphere collider in the gravity center
			gravityCenterCollider.enabled = true;

			//disable the capsule collider in the player
			playerCollider.isTrigger = true;

			//get the last time that the player was in the air
			playerControllerManager.lastTimeFalling = Time.time;

			recalculatingSurface = false;

			accelerating = false;

			//change the color of the player's textures
			changeColor (true);

			searchingNewSurfaceBelow = false;

			removeParent ();

			circumnavigableSurfaceFound = false;	

			playerControllerManager.setGravityPowerActiveState (true);

			playerCameraManager.sethorizontalCameraLimitActiveOnAirState (false);

			setPowerActivateState (true);

			//calibrate the accelerometer to rotate the camera in this mode
			playerCameraManager.calibrateAccelerometer ();

			//drop any object that the player is holding and disable aim mode
			if (stopAimModeWhenSearchingSurface) {
				checkKeepPower ();

				checkKeepWeapons ();
			}

			grabObjectsManager.checkIfDropObjectIfNotPhysical (false);

			//the player is in the ground, so he is elevated above it
			if (onGround) {
				lifting = true;
				choosingDirection = true;
			}

			//the player set the direction of the movement in the air to search a new surface
			if (!lifting && choosingDirection) {
				enableCameraShake ();

				setHoverState (false);

				if (canRotatePlayer ()) {
					rotateMeshPlayer ();
				}

				searchingSurface = true;

				circumnavigableSurfaceFound = false;	

				removeParent ();

				choosingDirection = false;

				searchAround = true;

				gravityDirection = mainCameraTransform.forward;

				//get direction and right axis of the camera, so when the player searchs a new surface, this is used to get the local movement, 
				//which allows to move the player in his local right, left, forward and back while he also displaces in the air
				rightAxis = mainCameraTransform.right;

				if (canRotatePlayer ()) {
					checkRotateCharacter (-gravityDirection);
				}
					
				return;
			} 

			//the player is in the air, so he is stopped in it to choose a direction
			if (!onGround && !choosingDirection && !lifting) {
				disableCameraShake ();

				playerCameraManager.changeCameraFov (false);

				setHoverState (true); 

				if (canRotatePlayer ()) {
					rotateMeshPlayer ();
				}

				choosingDirection = true; 	

				searchingSurface = false;	

				searchAround = false;
			}
		}
	}

	//now the gravity power is in a function, so it can be called from keyboard and a touch button
	public void deactivateGravityPower ()
	{
		//check that the power gravity is already enabled
		if ((choosingDirection || searchingSurface || currentNormal != regularGravity) && !playerControllerManager.isUsingDevice ()) {
//			print ("deactivate gravity power");

			//disable the sphere collider in the gravity center
			gravityCenterCollider.enabled = false;

			//enable the capsule collider in the player
			playerCollider.isTrigger = false;

			//get the last time that the player was in the air
			playerControllerManager.lastTimeFalling = Time.time;

			accelerating = false;

			//set the force of the gravity in the player to its regular state
			playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

			//change the color of the player
			changeColor (false);

			choosingDirection = false;

			circumnavigableSurfaceFound = false;	

			removeParent ();

			setHoverState (false);

			turning = false;

			searchingSurface = false;

			searchAround = false;

			lifting = false;

			recalculatingSurface = false;

			timer = 0.75f;

			//stop to shake the camera and set its position to the regular state
			disableCameraShake ();

			playerCameraManager.changeCameraFov (false);

			//if the normal of the player is different from the regular gravity, rotate the player
			if (currentNormal != regularGravity) {

				if (preserveVelocityWhenDisableGravityPower) {
					previousRigidbodyVelocity = mainRigidbody.velocity;
					preservingVelocity = true;

					extraMultiplierPreserveVelocity = 1;
				}

				checkRotateToSurface (regularGravity, rotateToRegularGravitySpeed);
			}

			//rotate the mesh of the player also
			checkRotateCharacter (regularGravity);

			playerControllerManager.setGravityPowerActiveState (false);

			playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

			setPowerActivateState (false);

			//set the value of the normal in the playerController component to its regular state
			playerControllerManager.setCurrentNormalCharacter (regularGravity);

			gravityPowerActive = false;

			currentSetGravityManager = null;

			searchNewSurfaceOnHighFallSpeedPaused = false;
		}
	}

	public void checkKeepWeapons ()
	{
		if (weaponsManager.isAimingInThirdPerson () || weaponsManager.isCarryingWeaponInThirdPerson ()) {
			weaponsManager.checkIfKeepSingleOrDualWeapon ();
		}
	}

	public void checkKeepPower ()
	{
		if (powers.isAimingPowerInThirdPerson ()) {
			powers.deactivateAimMode ();
		}
	}

	//now the change of velocity is in a function, so it can be called from keyboard and a touch button
	public void changeMovementVelocity (bool value)
	{
		//if the player is not choosing a gravity direction and he is searching a surface or the player is not in the ground and with a changed normal, then
		if (!choosingDirection && (powerActivated || (!playerControllerManager.isPlayerOnGround () && currentNormal != regularGravity))) {
			accelerating = value;

			//move the camera to a further away position, and add extra force to the player's velocity
			if (accelerating) {
				playerCameraManager.changeCameraFov (true);

				playerControllerManager.setGravityMultiplierValueFromExternalFunction (highGravityMultiplier);

				//when the player accelerates his movement in the air, the camera shakes
				//if the player accelerates his movement in the air and shake camera is enabled
				if (playerCameraManager.settings.enableShakeCamera) {
					playerCameraManager.accelerateShake (true);			
				}
			} 

			//else, set the camera to its regular position, reset the force applied to the player's velocity
			else {
				playerCameraManager.changeCameraFov (false);

				playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

				playerCameraManager.accelerateShake (false);
			}
		}
	}

	//convert the character in a child of the moving object
	public void addParent (GameObject obj)
	{
		fatherDetected = obj;

		parentAssignedSystem currentParentAssignedSystem = obj.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			fatherDetected = currentParentAssignedSystem.getAssignedParent ();
		}

		if (rotating) {
			externalGravityCenter.SetParent (fatherDetected.transform);
		} else {
			setFatherDetectedAsParent ();
		}

		if (fatherDetected != null) {
			playerIsChildOfParentActive = true;
		} else {
			playerIsChildOfParentActive = false;
		}
	}

	//remove the parent of the player, so he moves freely again
	public void removeParent ()
	{
		if (rotating) {
			setExternalGravityCenterAsParent ();
		} else {
			setNullParent ();
		}

		fatherDetected = null;

		playerIsChildOfParentActive = false;
	}

	public void setExternalGravityCenterAsParent ()
	{
		setNewParent (externalGravityCenter);
	}

	public void setFatherDetectedAsParent ()
	{
		setNewParent (fatherDetected.transform);
	}

	public void setNullParent ()
	{
		setNewParent (null);
	}

	public void setNewParent (Transform newParent)
	{
		if (playerTransform == null) {
			playerTransform = transform;
		}

//		if (newParent != null) {
//			print (newParent.name);
//		} else {
//			print ("removing parent");
//		}

		playerTransform.SetParent (newParent);
		playerCameraTransform.SetParent (newParent);
	}

	public void setCorrectParent ()
	{
		if (playerIsChildOfParentActive) {
			setFatherDetectedAsParent ();
		} else {
			setNullParent ();
		}
	}

	//the funcion to change camera view, to be called from a key or a touch button
	public void changeCameraView (bool state)
	{
		firstPersonView = state;

		//disable or enable the mesh of the player
		setGravityArrowState (!firstPersonView);

		if (firstPersonView) {
			//change to first person view
			grabObjectsManager.setAimingState (true);
		} else {
			//change to third person view
			grabObjectsManager.checkIfDropObjectIfNotPhysical (true);
		}
	}

	//set a random direction to rotate the character
	void rotateMeshPlayer ()
	{
		if (!turning) {
			turning = true;

			turnDirection = new Vector3 (Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1));

			if (turnDirection.magnitude == 0) {
				turnDirection.x = 1;
			}
		}
	}

	//set if the player is hovering or not
	void setHoverState (bool state)
	{
		hovering = state;
	}

	public bool canRotatePlayer ()
	{
		return (stopAimModeWhenSearchingSurface || (!weaponsManager.isCarryingWeaponInThirdPerson () && !powers.isAimingPowerInThirdPerson ()));
	}

	//change the gravity of the player when he touchs the arrow trigger
	public void changeOnTrigger (Vector3 dir, Vector3 right)
	{
		//set the parameters needed to change the player's gravity without using the gravity power buttons
		searchingNewSurfaceBelow = false;

		removeParent ();

		circumnavigableSurfaceFound = false;	

		searchingSurface = true;

		searchAround = true;

		changeColor (true);

		playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

		playerControllerManager.setGravityPowerActiveState (true);

		playerCameraManager.sethorizontalCameraLimitActiveOnAirState (false);

		setPowerActivateState (true);

		playerCameraManager.calibrateAccelerometer ();

		if (canRotatePlayer ()) {
			rotateMeshPlayer ();
		}

		enableCameraShake ();

		gravityDirection = dir;

		rightAxis = right;

		if (canRotatePlayer ()) {
			checkRotateCharacter (-gravityDirection);
		}

		gravityPowerActive = true;
	}

	//stop the gravity power when the player is going to drive a vehicle
	public void stopGravityPower ()
	{
		//disable the sphere collider in the gravity center
		gravityCenterCollider.enabled = false;

		//get the last time that the player was in the air
		playerControllerManager.lastTimeFalling = Time.time;
		accelerating = false;

		//set the force of the gravity in the player to its regular state
		playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

		choosingDirection = false;

		circumnavigableSurfaceFound = false;	

		removeParent ();

		setHoverState (false);

		turning = false;

		searchingSurface = false;

		searchAround = false;

		lifting = false;

		recalculatingSurface = false;

		timer = 0.75f;

		//stop to shake the camera and set its position to the regular state
		disableCameraShake ();

		playerCameraManager.changeCameraFov (false);

		playerControllerManager.setGravityPowerActiveState (false);

		playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

		setPowerActivateState (false);

		//reset the player's rotation
		playerTransform.rotation = quaternionIdentity;
		gravityCenter.transform.localRotation = quaternionIdentity;

		//set to 0 the current velocity of the player
		mainRigidbody.velocity = vector3Zero;
		gravityPowerActive = false;
	}

	//rotate the player, camera and mesh of the player to the new surface orientation
	//public
	public void checkRotateToSurface (Vector3 normal, float rotSpeed)
	{
		stopRotateToSurfaceCoroutine ();

		rotateCharacterState = StartCoroutine (rotateToSurface (normal, rotSpeed));
	}

	public void stopRotateToSurfaceCoroutine ()
	{
		//get the coroutine, stop it and play it again
		if (rotateCharacterState != null) {
			StopCoroutine (rotateCharacterState);
		}
	}

	public IEnumerator rotateToSurface (Vector3 normal, float rotSpeed)
	{
		updateCurrentRotatingNormal (normal);

		externalGravityCenter.SetParent (null);
		externalGravityCenter.position = gravityCenter.position;
		externalGravityCenter.rotation = playerTransform.rotation;

		setExternalGravityCenterAsParent ();

		setRotatingToSurfaceState (true);

		Quaternion currentPlayerRotation = externalGravityCenter.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (externalGravityCenter.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);

		Quaternion currentGravityCenterRotation = gravityCenter.localRotation;
		Quaternion gravityCenterTargetRotation = quaternionIdentity;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			externalGravityCenter.rotation = Quaternion.Slerp (currentPlayerRotation, playerTargetRotation, t);
			gravityCenter.transform.localRotation = Quaternion.Slerp (currentGravityCenterRotation, gravityCenterTargetRotation, t);
			yield return null;
		}

		setCorrectParent ();

		externalGravityCenter.SetParent (playerTransform);

		currentNormal = normal; 

		playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

		playerControllerManager.setCurrentNormalCharacter (normal);

		if (currentNormal == regularGravity) {
			gravityPowerActive = false;
		}

		setRotatingToSurfaceState (false);
	
		//adjust gravity rotation for locked camera
		playerCameraManager.setLockedMainCameraTransformRotation (normal);

		if (zeroGravityModeOn || freeFloatingModeOn) {
			disableCameraShake ();
		}

		if (preservingVelocity) {
			preservingVelocity = false;

			yield return new WaitForSeconds (0.02f);

			playerControllerManager.addExternalForce (previousRigidbodyVelocity * extraMultiplierPreserveVelocity);
		}

		checkOnGroundOrAirState ();
	}

	public override bool isPlayerSearchingGravitySurface ()
	{
		return powerActivated;
	}

	public bool isCharacterRotatingToSurface ()
	{
		return rotating;
	}

	void setRotatingToSurfaceState (bool state)
	{
		rotating = state;

		playerControllerManager.setCharacterRotatingToSurfaceState (state);
	}

	public void checkRotateToSurfaceWithoutParent (Vector3 normal, float rotSpeed)
	{
		stopRotateToSurfaceWithOutParentCoroutine ();

		rotateCharacterState = StartCoroutine (rotateToSurfaceWithOutParent (normal, rotSpeed));
	}

	public void stopRotateToSurfaceWithOutParentCoroutine ()
	{
		//get the coroutine, stop it and play it again
		if (rotateCharacterState != null) {
			StopCoroutine (rotateCharacterState);
		}
	}

	public IEnumerator rotateToSurfaceWithOutParent (Vector3 normal, float rotSpeed)
	{
		updateCurrentRotatingNormal (normal);

		setRotatingToSurfaceState (true);

		Quaternion currentPlayerRotation = playerTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerTransform.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);

		Quaternion currentCameraRotation = playerCameraGameObject.transform.rotation;
		Vector3 currentCameraForward = Vector3.Cross (playerCameraGameObject.transform.right, normal);
		Quaternion cameraTargetRotation = Quaternion.LookRotation (currentCameraForward, normal);

		Quaternion currentGravityCenterRotation = gravityCenter.localRotation;
		Quaternion gravityCenterTargetRotation = quaternionIdentity;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			playerTransform.rotation = Quaternion.Slerp (currentPlayerRotation, playerTargetRotation, t);

			playerCameraGameObject.transform.rotation = Quaternion.Slerp (currentCameraRotation, cameraTargetRotation, t);

			gravityCenter.transform.localRotation = Quaternion.Slerp (currentGravityCenterRotation, gravityCenterTargetRotation, t);

			yield return null;
		}

		currentNormal = normal;

		playerControllerManager.setGravityMultiplierValueFromExternalFunction (normalGravityMultiplier);

		playerControllerManager.setCurrentNormalCharacter (normal);

		if (currentNormal == regularGravity) {
			gravityPowerActive = false;
		}

		setRotatingToSurfaceState (false);

		//adjust gravity rotation for locked camera
		playerCameraManager.setLockedMainCameraTransformRotation (normal);

		if (zeroGravityModeOn || freeFloatingModeOn) {
			disableCameraShake ();
		}

		if (preservingVelocity) {
			preservingVelocity = false;
			mainRigidbody.velocity = previousRigidbodyVelocity * 2;
		}
	}

	public void setNormal (Vector3 normal)
	{
		Vector3 myForwardPlayer = Vector3.Cross (playerTransform.right, normal);
		Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, normal);
		Quaternion dstRotPlayer = Quaternion.LookRotation (myForwardPlayer, normal);		
		Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, normal);

		playerTransform.rotation = dstRotPlayer;
		playerCameraTransform.rotation = dstRotCamera;

		currentNormal = normal;

		playerControllerManager.setCurrentNormalCharacter (normal);

		if (currentNormal != regularGravity) {
			gravityPowerActive = true;

			changeColor (true);
		} else {
			changeColor (false);
		}

		updateCurrentRotatingNormal (currentNormal);

		currentSetGravityManager = null;
	}

	public override Vector3 getCurrentNormal ()
	{
		return currentNormal;
	}

	public void setCurrentNormal (Vector3 newValue)
	{
		currentNormal = newValue;
	}

	public bool isUsingRegularGravity ()
	{
		return currentNormal == regularGravity;
	}

	public Vector3 getRegularGravity ()
	{
		return regularGravity;
	}

	public void updateCurrentRotatingNormal (Vector3 newNormal)
	{
		currentRotatingNormal = newNormal;
	}

	public Vector3 getCurrentRotatingNormal ()
	{
		return currentRotatingNormal;
	}

	//rotate the mesh of the character in the direction of the camera when he selects a gravity direction in the air
	// and to the quaternion identity when he is on ground
	public void checkRotateCharacter (Vector3 normal)
	{
		float angle = Vector3.Angle (gravityCenter.up, playerTransform.up);

		if (Mathf.Abs (angle) > 0 || currentNormal == regularGravity) {
			//get the coroutine, stop it and play it again
			if (rotateToSurfaceState != null) {
				StopCoroutine (rotateToSurfaceState);
			}

			rotateToSurfaceState = StartCoroutine (rotateCharacter (normal));
		}
	}

	public IEnumerator rotateCharacter (Vector3 normal)
	{
		Quaternion orgRotCenter = gravityCenter.transform.localRotation;
		Quaternion dstRotCenter = new Quaternion (0, 0, 0, 1);

		//check that the normal is different from zero, to rotate the player's mesh in the direction of the new gravity when he use the gravity power button
		//and select the camera direction to search a new surface
		//else, the player's mesh is rotated to its regular state
		if (normal != vector3Zero) {
			orgRotCenter = gravityCenter.transform.rotation;
			Vector3 myForward = Vector3.Cross (gravityCenter.transform.right, normal);
			dstRotCenter = Quaternion.LookRotation (myForward, normal);
		}

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;
			if (normal == vector3Zero) {
				gravityCenter.transform.localRotation = Quaternion.Slerp (orgRotCenter, dstRotCenter, t);
			} else {
				gravityCenter.transform.rotation = Quaternion.Slerp (orgRotCenter, dstRotCenter, t);
			}

			yield return null;
		}
	}

	public void changeGravityDirectionDirectlyInvertedValue (Vector3 gravityDirection, bool preserveVelocity)
	{
		changeGravityDirectionDirectly (-gravityDirection, preserveVelocity);
	}

	public void changeGravityDirectionDirectly (Vector3 gravityDirection, bool preserveVelocity)
	{
		if (!rotating) {
			//disable the search of the surface and rotate the player to that surface
			playerControllerManager.setGravityPowerActiveState (false);

			playerCameraManager.sethorizontalCameraLimitActiveOnAirState (true);

			setPowerActivateState (false);

			searchingNewSurfaceBelow = false;
			searchingSurface = false;
			searchAround = false;

			if (preserveVelocity) {
				previousRigidbodyVelocity = mainRigidbody.velocity;
				preservingVelocity = true;

				extraMultiplierPreserveVelocity = 2;
			}

			mainRigidbody.velocity = vector3Zero;

			//disable the collider in the gravity center and enable the capsule collider in the player
			gravityCenterCollider.enabled = false;
			playerCollider.isTrigger = false;
	
			//set the camera in its regular position
			playerCameraManager.changeCameraFov (false);

			//if the new normal is different from the previous normal in the gravity power, then rotate the player
			if (gravityDirection != currentNormal) {
				checkRotateToSurface (gravityDirection, rotateToSurfaceSpeed); 
			}

			//if the player back to the regular gravity value, change its color to the regular state
			if (gravityDirection == regularGravity) {
				changeColor (false);
			} else {
				changeColor (true);
			}
		}
	}

	public void setCurrentSetGravityManager (setGravity currentSetGravity)
	{
		currentSetGravityManager = currentSetGravity;
	}

	public setGravity getCurrentSetGravityManager ()
	{
		return currentSetGravityManager;
	}

	public void changeColor (bool state)
	{
		if (!changeModelColor) {
			return;
		}

		if (colorCoroutine != null) {
			StopCoroutine (colorCoroutine);
		}

		colorCoroutine = StartCoroutine (changeColorCoroutine (state));
	}

	//change the mesh color of the character according to the gravity power
	public IEnumerator changeColorCoroutine (bool value)
	{
		if (playerRenderer != null) {
			if (value) {
				powerColor = originalPowerColor;
			} else {
				powerColor = Color.white;
			}

			int propertyNameID = Shader.PropertyToID ("_Color");

			for (float t = 0; t < 1;) {
				t += Time.deltaTime;

				for (int i = 0; i < materialToChange.Count; i++) {
					if (playerRenderer.materials.Length >= materialToChange.Count) {
						if (materialToChange [i] == true && playerRenderer.materials [i].HasProperty (propertyNameID)) {
							playerRenderer.materials [i].color = Color.Lerp (playerRenderer.materials [i].color, powerColor, t);
						}
					}
				}

				yield return null;
			}
		}
	}

	public void setMeshCharacter (SkinnedMeshRenderer currentMeshCharacter)
	{
		if (currentMeshCharacter != null) {
			playerRenderer = currentMeshCharacter.GetComponent<Renderer> ();
		}

		updateComponent ();
	}
		
	//change the object which the camera follows and disable or enabled the powers according to the player state
	public void death (bool state)
	{
		dead = state;

		if (state) {
			deactivateGravityPower ();

			turning = false;

			setHoverState (false);

			checkRotateCharacter (vector3Zero);
		} 
	}

	public void setGravityArrow (GameObject obj)
	{
		arrow = obj;
	}

	public void setGravityArrowState (bool state)
	{
		if (arrow != null) {
			if (arrow.activeSelf != state) {
				arrow.SetActive (state);
			}

			gravityArrowCurrentlyActive = state;
		}
	}

	public void setFirstPersonView (bool state)
	{
		firstPersonView = state;
	}

	public override Transform getGravityCenter ()
	{
		return gravityCenter;
	}

	public bool isCurcumnavigating ()
	{
		return circumnavigableSurfaceFound;
	}

	public override bool isSearchingSurface ()
	{
		return searchingSurface;
	}

	public bool isGravityPowerActive ()
	{
		return gravityPowerActive;
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
		usedByAI = !state;
	}

	public bool isZeroGravityModeOn ()
	{
		return zeroGravityModeOn;
	}

	public void setZeroGravityModeOnState (bool state)
	{
		if (freeFloatingModeOn) {
			setfreeFloatingModeOnState (false);
		}

		zeroGravityModeOn = state;

		playerControllerManager.setZeroGravityModeOnState (zeroGravityModeOn);

		playerCameraManager.setZeroGravityModeOnState (zeroGravityModeOn);

		if (!zeroGravityModeOn) {
			if (currentNormal != regularGravity) {
				deactivateGravityPower ();
			}
		}

		checkEventsOnZeroGravityModeStateChange (zeroGravityModeOn);
	}

	public void setZeroGravityModeOnStateWithOutRotation (bool state)
	{
		if (freeFloatingModeOn) {
			setfreeFloatingModeOnState (false);
		}

		zeroGravityModeOn = state;

		playerControllerManager.setZeroGravityModeOnState (zeroGravityModeOn);

		playerCameraManager.setZeroGravityModeOnState (zeroGravityModeOn);

		checkEventsOnZeroGravityModeStateChange (zeroGravityModeOn);
	}

	public void setfreeFloatingModeOnState (bool state)
	{
		if (zeroGravityModeOn) {
			return;
		}

		if (!canActivateFreeFloatingMode) {
			return;
		}

		freeFloatingModeOn = state;

		checkEventsOnFreeFloatingModeStateChange (freeFloatingModeOn);

		playerControllerManager.setfreeFloatingModeOnState (freeFloatingModeOn);

		playerCameraManager.setfreeFloatingModeOnState (freeFloatingModeOn);

		stopSetfreeFloatingModeOnStateWithDelayCoroutine ();
	}

	public void checkEventsOnFreeFloatingModeStateChange (bool state)
	{
		if (useEventsOnFreeFloatingModeStateChange) {
			if (state) {
				evenOnFreeFloatingModeStateEnabled.Invoke ();
			} else {
				eventOnFreeFloatingModeStateDisabled.Invoke ();
			}
		}
	}

	public void checkEventsOnZeroGravityModeStateChange (bool state)
	{
		if (useEventsOnZeroGravityModeStateChange) {
			if (state) {
				evenOnZeroGravityModeStateEnabled.Invoke ();
			} else {
				eventOnZeroGravityModeStateDisabled.Invoke ();
			}
		}
	}

	public void checkEventsOnUseGravityPowerStateChange (bool state)
	{
		if (useEventsOnUseGravityPowerStateChange) {
			if (state) {
				eventsOnUseGravityPowerStateEnabled.Invoke ();
			} else {
				eventsOnUseGravityPowerStateDisabled.Invoke ();
			}
		}
	}

	public void setPowerActivateState (bool state)
	{
		powerActivated = state;

		checkEventsOnUseGravityPowerStateChange (state);
	}

	public void changeFreeFloatingModeOnState ()
	{
		setfreeFloatingModeOnState (!freeFloatingModeOn);
	}

	public void setfreeFloatingModeOnStateWithDelay (float delayAmount, bool state)
	{
		stopSetfreeFloatingModeOnStateWithDelayCoroutine ();

		freeFloatingModeCoroutine = StartCoroutine (setfreeFloatingModeOnStateWithDelayCoroutine (delayAmount, state));
	}

	public void stopSetfreeFloatingModeOnStateWithDelayCoroutine ()
	{
		if (freeFloatingModeCoroutine != null) {
			StopCoroutine (freeFloatingModeCoroutine);
		}
	}

	public IEnumerator setfreeFloatingModeOnStateWithDelayCoroutine (float delayAmount, bool state)
	{
		yield return new WaitForSeconds (delayAmount);

		setfreeFloatingModeOnState (state);
	}

	public void setCanActivateFreeFloatingModeState (bool state)
	{
		canActivateFreeFloatingMode = state;
	}

	public void rotateCharacterInZeroGravityMode (Vector3 normal, float rotSpeed)
	{
		//get the coroutine, stop it and play it again
		if (zeroGravityModeRotateCharacterCoroutine != null) {
			StopCoroutine (zeroGravityModeRotateCharacterCoroutine);
		}

		zeroGravityModeRotateCharacterCoroutine = StartCoroutine (rotateCharacterInZeroGravityModeCoroutine (normal, rotSpeed));
	}

	public IEnumerator rotateCharacterInZeroGravityModeCoroutine (Vector3 normal, float rotSpeed)
	{
		externalGravityCenter.SetParent (null);
		externalGravityCenter.position = playerTransform.position + playerTransform.up;
		externalGravityCenter.rotation = playerCameraTransform.rotation;

		setExternalGravityCenterAsParent ();

		setRotatingToSurfaceState (true);

		Quaternion currentPlayerRotation = playerCameraTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerCameraTransform.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);

		Quaternion currentGravityCenterRotation = gravityCenter.localRotation;
		Quaternion gravityCenterTargetRotation = quaternionIdentity;
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			externalGravityCenter.rotation = Quaternion.Lerp (currentPlayerRotation, playerTargetRotation, t);
			gravityCenter.transform.localRotation = Quaternion.Lerp (currentGravityCenterRotation, gravityCenterTargetRotation, t);

			yield return null;
		}
			
		setCorrectParent ();

		externalGravityCenter.SetParent (playerTransform);

		setRotatingToSurfaceState (false);
	}

	public void teleportPlayer (Vector3 teleportPosition, float teleportSpeed, Vector3 normal, float rotSpeed)
	{
		stopTeleporting ();

		teleportCoroutine = StartCoroutine (teleportPlayerCoroutine (teleportPosition, teleportSpeed, normal, rotSpeed));
	}

	public void stopTeleporting ()
	{
		if (teleportCoroutine != null) {
			StopCoroutine (teleportCoroutine);
		}

		setPlayerControlState (true);
	}

	public void setPlayerControlState (bool state)
	{
		playerControllerManager.changeScriptState (state);
		playerControllerManager.setGravityForcePuase (!state);
		playerControllerManager.setRigidbodyVelocityToZero ();
		playerControllerManager.setPhysicMaterialAssigmentPausedState (!state);

		if (!state) {
			playerControllerManager.setZeroFrictionMaterial ();

			if (!playerControllerManager.isPauseCheckOnGroundStateZGActive ()) {
				playerControllerManager.setCheckOnGrundStatePausedFFOrZGState (false);
			}
		}
	}

	IEnumerator teleportPlayerCoroutine (Vector3 targetPosition, float currentTeleportSpeed, Vector3 normal, float rotSpeed)
	{
		teleportInProcess = true;

		externalGravityCenter.SetParent (null);
		externalGravityCenter.position = playerTransform.position + playerTransform.up;
		externalGravityCenter.rotation = playerCameraTransform.rotation;

		setExternalGravityCenterAsParent ();

		setPlayerControlState (false);

		setRotatingToSurfaceState (true);

		Quaternion currentPlayerRotation = playerCameraTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerCameraTransform.right, normal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, normal);


		float dist = GKC_Utils.distance (externalGravityCenter.position, targetPosition);
		float duration = dist / currentTeleportSpeed;
		float translateTimer = 0;
		float rotateTimer = 0;

		float teleportTimer = 0;

		float normalAngle = 0;

		Vector3 targetPositionDirection = targetPosition - externalGravityCenter.position;
		targetPositionDirection = targetPositionDirection / targetPositionDirection.magnitude;

		bool targetReached = false;

		while (!targetReached) {
			translateTimer += Time.deltaTime / duration;
			externalGravityCenter.position = Vector3.Lerp (externalGravityCenter.position, targetPosition, translateTimer);

			rotateTimer += Time.deltaTime * rotSpeed;

			externalGravityCenter.rotation = Quaternion.Lerp (currentPlayerRotation, playerTargetRotation, rotateTimer);

			teleportTimer += Time.deltaTime;

			normalAngle = Vector3.Angle (playerTransform.up, normal);   

			if ((GKC_Utils.distance (externalGravityCenter.position, targetPosition) < 0.2f && normalAngle < 1) || teleportTimer > (duration + 5)) {
				targetReached = true;
			}
			yield return null;
		}

		setPlayerControlState (true);

		setRotatingToSurfaceState (false);
			
		setCorrectParent ();

		externalGravityCenter.SetParent (playerTransform);

		teleportInProcess = false;
	}

	public bool isTeleportInProcess ()
	{
		return teleportInProcess;
	}

	public void setGravityPowerEnabledState (bool state)
	{
		gravityPowerEnabled = state;
	}

	public void setGravityPowerState (bool enablePower)
	{
		//activate the power of change gravity
		//one press=the player elevates above the surface if he was in the ground or stops him in the air if he was not in the ground
		//two press=make the player moves in straight direction of the camera, looking a new surface
		//three press=stops the player again in the air
		if (!dead && !playerControllerManager.isUsingDevice () && gravityPowerEnabled) {
			if (enablePower) {
				activateGravityPower ();
			} else {
				deactivateGravityPower ();
			}
		}
	}

	public void disableGravityPowerIfOnAir ()
	{
		if (choosingDirection || searchingSurface) {
			deactivateGravityPower ();
		}
	}

	public void enableCameraShake ()
	{
		if (cameraShakeCanBeUsed) {
			playerCameraManager.startShakeCamera ();
		}
	}

	public void disableCameraShake ()
	{
		if (cameraShakeCanBeUsed) {
			playerCameraManager.stopShakeCamera ();
		}
	}

	public void setCameraShakeCanBeUsedState (bool state)
	{
		cameraShakeCanBeUsed = state;

		if (!cameraShakeCanBeUsed) {
			playerCameraManager.stopShakeCamera ();
		}
	}

	public void setGravityPowerInputEnabledState (bool state)
	{
		gravityPowerInputEnabled = state;
	}

	//CALL INPUT FUNCTIONS
	public void inputSetGravityPowerState (bool enablePower)
	{
		if (gravityPowerInputEnabled) {
			setGravityPowerState (enablePower);
		}
	}

	public void inputChangeGravitySpeed (bool increaseSpeed)
	{
		if (!dead && !playerControllerManager.isUsingDevice () && gravityPowerEnabled) {
			if (increaseSpeed) {
				changeMovementVelocity (true);
			} else {
				changeMovementVelocity (false);
			}
		}
	}

	public void inputAdjustToSurfaceOnZeroGravity ()
	{
		if (!dead && !playerControllerManager.isUsingDevice () && zeroGravityModeOn && !onGround && !rotating && canAdjustToForwardSurface) {
			if (Physics.Raycast (forwardSurfaceRayPosition.position, forwardSurfaceRayPosition.forward, out hit, maxDistanceToAdjust, layer)) {
				if (currentNormal != regularGravity) {
					teleportPlayer (hit.point + hit.normal * 0.6f, adjustToForwardSurfaceSpeed, hit.normal, resetRotationZeroGravitySpeed);
				}
			}
		}
	}

	public void inputResetRotationOnZeroGravity ()
	{
		if (!dead && !playerControllerManager.isUsingDevice () && zeroGravityModeOn && !onGround && !rotating && canResetRotationOnZeroGravityMode) {
			if (currentNormal != regularGravity) {
				rotateCharacterInZeroGravityMode (regularGravity, resetRotationZeroGravitySpeed);
			}
		}
	}

	public void inputToggleReverseGravity ()
	{
		if (!dead && !playerControllerManager.isUsingDevice () && gravityPowerEnabled) {
			if (currentNormal != regularGravity) {
				deactivateGravityPower ();
			} else {
				changeGravityDirectionDirectlyInvertedValue (currentNormal, true);

				if (pauseSearchNewSurfaceOnHighFallSpeedOnReverseGravityInput) {
					searchNewSurfaceOnHighFallSpeedPaused = true;
				}
			}
		}
	}

	public void setCurrentZeroGravityRoom (zeroGravityRoomSystem gravityRoom)
	{
		currentZeroGravityRoom = gravityRoom;

		if (currentZeroGravityRoom != null) {
			playerInsideGravityRoom = true;
		} else {
			playerInsideGravityRoom = false;
		}
	}

	public bool isPlayerInsiderGravityRoom ()
	{
		return playerInsideGravityRoom;
	}

	public zeroGravityRoomSystem getCurrentZeroGravityRoom ()
	{
		return currentZeroGravityRoom;
	}

	public Transform getCurrentSurfaceBelowPlayer ()
	{
		return currentSurfaceBelowPlayer;
	}

	public void addObjectToCurrentZeroGravityRoomSystem (GameObject objectToAdd)
	{
		if (currentZeroGravityRoom != null) {
			currentZeroGravityRoom.addObjectToRoom (objectToAdd);
		}
	}

	public void setCircumnavigateSurfaceState (bool state)
	{
		circumnavigateCurrentSurfaceActive = state;

		if (!circumnavigateCurrentSurfaceActive) {
			if (circumnavigableSurfaceFound || playerIsChildOfParentActive) {
				circumnavigableSurfaceFound = false;

				if (playerIsChildOfParentActive) {
					removeParent ();	
				}
			}

			recalculatingSurface = false;
		}
	}

	public void setCheckSurfaceInFrontState (bool state)
	{
		checkSurfaceInFront = state;
	}

	public void setCheckSurfaceBelowLedgeState (bool state)
	{
		checkSurfaceBelowLedge = state;
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}