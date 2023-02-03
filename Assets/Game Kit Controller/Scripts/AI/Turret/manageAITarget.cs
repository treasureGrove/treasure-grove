using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class manageAITarget : MonoBehaviour
{
	[Header ("AI Settings")]
	[Space]

	public float timeToCheckSuspect;

	public LayerMask layerMask;
	public LayerMask layerToCheckTargets;

	public string functionToShoot;

	public float extraFieldOfViewRadiusOnSpot;
		
	public bool checkRaycastToViewTarget = true;
	public LayerMask layerToCheckRaycastToViewTarget;

	[Space]
	[Header ("Range Vision Settings")]
	[Space]

	public float visionRange = 90;
	public float minDistanceToAdquireTarget = 2;
	public bool allowDetectionWhenTooCloseEvenNotVisible;

	public bool ignoreVisionRangeActive;
	public float checkIfTargetIsPhysicallyVisibleRadius = 0.2f;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public string factionToChangeName = "Friend Turrets";

	public string surprisedCharacterStateName = "Surprised";
	public string wonderingCharacterStateName = "Wondering";

	public bool alertFactionOnSpotted;
	public float alertFactionRadius = 10;

	[Space]
	[Header ("AI State")]
	[Space]

	public bool onSpotted;
	public bool paused;
	public bool hacking;
	public bool checkingThreat;
	public bool targetIsCharacter;
	public bool targetIsVehicle;
	public bool threatInfoStored;

	public bool seeingCurrentTarget;

	public GameObject enemyToShoot;

	public GameObject posibleThreat;

	public Transform placeToShoot;

	public bool differentEnemy;

	public GameObject currentObjectDetectedByRaycast;

	[Space]
	[Header ("Targets Debug")]
	[Space]

	public List<GameObject> enemies = new List<GameObject> ();
	public List<GameObject> notEnemies = new List<GameObject> ();

	public List<GameObject> fullEnemyList = new List<GameObject> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventsOnSpotted;
	public UnityEvent eventOnSpotted;
	public UnityEvent eventOnNoTargetsToAttack;

	[Space]
	[Header ("AI Elements")]
	[Space]

	public Transform rayCastPosition;
	public SphereCollider fovTrigger;

	public GameObject hackDevice;

	public checkCollisionType viewTrigger;
	public characterFactionManager factionManager;
	public characterStateIconSystem characterStateIconManager;

	RaycastHit hit;
	float originalFOVRadius;
	float timeToCheck = 0;
	float speedMultiplier = 1;

	bool hackFailed;

	playerController currentPlayerController;
	vehicleHUDManager currentVehicleHUDManager;
	characterFactionManager characterFactionToCheck;

	bool hasBeenAttacked;
	Vector3 targetDirection;
	bool enemyAlertActive;

	float timeWithoutThreatFound;

	Vector3 currentPosition;

	bool targetIsDriving;

	GameObject previousEnemy;

	float currentDistance;

	Vector3 targetToCheckDirection;
	Transform temporalPlaceToShoot;
	Vector3 targetToCheckPosition;
	Vector3 targetToCheckRaycastPosition;
	vehicleHUDManager temporalVehicleHUDManagerToCheck;

	void Start ()
	{
		originalFOVRadius = fovTrigger.radius;

		if (hackDevice != null) {
			if (tag.Equals ("friend")) {
				if (hackDevice.activeSelf) {
					hackDevice.SetActive (false);
				}
			}
		}
	}

	void Update ()
	{
		if (!hacking && !paused) { 
			
			closestTarget ();

			if (onSpotted) {
				
				lootAtTarget (placeToShoot);

				if (enemyToShoot != null) {
					if (Physics.Raycast (rayCastPosition.position, rayCastPosition.forward, out hit, Mathf.Infinity, layerMask)) {
						if (hit.collider.gameObject == enemyToShoot || hit.collider.gameObject.transform.IsChildOf (enemyToShoot.transform)) {
							targetDirection = enemyToShoot.transform.position - transform.position;

							float angleWithTarget = Vector3.SignedAngle (rayCastPosition.forward, targetDirection, enemyToShoot.transform.up);

							if (Mathf.Abs (angleWithTarget) < visionRange / 2 || ignoreVisionRangeActive) { 
								shootTarget ();
							}
						}
					}
				}
			}

			//if the turret detects a target, it will check if it is an enemy, and this will take 2 seconds, while the enemy choose to leave or stay in the place
			else if (checkingThreat) {
				if (posibleThreat != null) {
					if (!placeToShoot) {
						//every object with a health component, has a place to be shoot, to avoid that a enemy shoots the player in his foot, so to center the shoot
						//it is used the gameObject placetoshoot in the health script
						if (applyDamage.checkIfDead (posibleThreat)) {
							cancelCheckSuspect (posibleThreat);

							return;
						} else {
							placeToShoot = applyDamage.getPlaceToShoot (posibleThreat);
						}
					}

					if (!threatInfoStored) {
						currentPlayerController = posibleThreat.GetComponent<playerController> ();

						if (currentPlayerController != null) {
							if (currentPlayerController.isCharacterVisibleToAI ()) {
								setCharacterStateIcon (wonderingCharacterStateName);
							}
						} else {
							setCharacterStateIcon (wonderingCharacterStateName);
						}

						threatInfoStored = true;
					} 

					if (placeToShoot != null) {
						//look at the target position
						lootAtTarget (placeToShoot);
					}

					//uses a raycast to check the posible threat
					if (Physics.Raycast (rayCastPosition.position, rayCastPosition.forward, out hit, Mathf.Infinity, layerMask)) {
						if (hit.collider.gameObject == posibleThreat || hit.collider.gameObject.transform.IsChildOf (posibleThreat.transform)) {
							timeToCheck += Time.deltaTime * speedMultiplier;
						} else {
							timeWithoutThreatFound += Time.deltaTime * speedMultiplier;
						}

						//when the turret look at the target for a while, it will open fire 
						if (timeToCheck > timeToCheckSuspect) {
							timeToCheck = 0;

							checkingThreat = false;

							addEnemy (posibleThreat);

							posibleThreat = null;

							disableCharacterStateIcon ();
						}

						if (timeWithoutThreatFound > timeToCheckSuspect) {
							resetCheckThreatValues ();
						}
					}
				}
			}
		}
	}

	public void resetCheckThreatValues ()
	{
		placeToShoot = null;
		posibleThreat = null;
		checkingThreat = false;
		timeToCheck = 0;
		timeWithoutThreatFound = 0;

		disableCharacterStateIcon ();
	}

	//follow the enemy position, to rotate torwards his direction
	void lootAtTarget (Transform objective)
	{
		if (showGizmo) {
			Debug.DrawRay (rayCastPosition.position, rayCastPosition.forward, Color.red);
		}

		if (objective != null) {
			Vector3 targetDir = objective.position - rayCastPosition.position;
			Quaternion targetRotation = Quaternion.LookRotation (targetDir, transform.up);
			rayCastPosition.rotation = Quaternion.Slerp (rayCastPosition.rotation, targetRotation, 10 * Time.deltaTime);
		}
	}

	public bool checkCharacterFaction (GameObject character, bool damageReceived)
	{
		if (fullEnemyList.Contains (character)) {
			return true;
		}

		characterFactionToCheck = character.GetComponent<characterFactionManager> ();

		if (characterFactionToCheck != null) {
			bool isEnemy = false;

			if (damageReceived) {
				isEnemy = factionManager.isAttackerEnemy (characterFactionToCheck.getFactionName ());
			} else {
				isEnemy = factionManager.isCharacterEnemy (characterFactionToCheck.getFactionName ());
			}

			return isEnemy;
		} else {
			currentVehicleHUDManager = character.GetComponent<vehicleHUDManager> ();

			if (currentVehicleHUDManager != null) {
				if (currentVehicleHUDManager.isVehicleBeingDriven ()) {
					GameObject currentDriver = currentVehicleHUDManager.getCurrentDriver ();

					if (currentDriver == null) {
						return false;
					}

					characterFactionToCheck = currentDriver.GetComponent<characterFactionManager> ();

					if (characterFactionToCheck != null) {
						bool isEnemy = false;

						if (damageReceived) {
							isEnemy = factionManager.isAttackerEnemy (characterFactionToCheck.getFactionName ());
						} else {
							isEnemy = factionManager.isCharacterEnemy (characterFactionToCheck.getFactionName ());
						}

						targetIsDriving = true;

						return isEnemy;
					} 
				}
			}
		}

		return false;
	}

	//check if the object which has collided with the viewTrigger (the capsule collider in the head of the turret) is an enemy checking the tag of that object
	void checkSuspect (GameObject currentSuspect)
	{
		if (canCheckSuspect (currentSuspect.layer)) {
			if (checkCharacterFaction (currentSuspect, false) && !onSpotted && posibleThreat == null) {

				if (targetIsDriving || applyDamage.isVehicle (currentSuspect)) {
					targetIsDriving = false;
					targetIsVehicle = true;
				}

				if (!checkRaycastToViewTarget || checkIfTargetIsPhysicallyVisible (currentSuspect, true)) {
					posibleThreat = currentSuspect;
					checkingThreat = true;
					hacking = false;
				}
			}
		}
	}

	//in the object exits from the viewTrigger, the turret rotates again to search more enemies
	void cancelCheckSuspect (GameObject col)
	{
		if (checkCharacterFaction (col, false) && !onSpotted && posibleThreat != null) {
			placeToShoot = null;
			posibleThreat = null;
			checkingThreat = false;
			timeToCheck = 0;

			SendMessage ("cancelCheckSuspectTurret", SendMessageOptions.DontRequireReceiver);

			disableCharacterStateIcon ();
		}
	}

	//the sphere collider with the trigger of the turret has detected an enemy, so it is added to the list of enemies
	void enemyDetected (GameObject col)
	{
		if (checkCharacterFaction (col, false)) {
			addEnemy (col.gameObject);
		}
	}

	//one of the enemies has left, so it is removed from the enemies list
	void enemyLost (GameObject col)
	{
		//if (onSpotted) {
		removeEnemy (col.gameObject);
		//}
	}

	void enemyAlert (GameObject target)
	{
		enemyDetected (target);

		enemyAlertActive = true;
	}

	//if anyone shoot the turret, increase its field of view to search any enemy close to it
	public void checkShootOrigin (GameObject attacker)
	{
		if (!onSpotted) {
			if (checkCharacterFaction (attacker, true)) {
				addEnemy (attacker);

				factionManager.addDetectedEnemyFromFaction (attacker);

				hasBeenAttacked = true;
			}
		}
	}

	//add an enemy to the list, checking that that enemy is not already in the list
	void addEnemy (GameObject enemy)
	{
		if (!enemies.Contains (enemy)) {
			enemies.Add (enemy);

			if (!fullEnemyList.Contains (enemy)) {
				fullEnemyList.Add (enemy);
			}
		}
	}

	//remove an enemy from the list
	void removeEnemy (GameObject enemy)
	{
		//remove this enemy from the faction system detected enemies for the faction of this character
		factionManager.removeDetectedEnemyFromFaction (enemy);

		enemies.Remove (enemy);
	}

	void addNotEnemey (GameObject notEnemy)
	{
		if (!notEnemies.Contains (notEnemy)) {
			characterFactionToCheck = notEnemy.GetComponent<characterFactionManager> ();

			if (characterFactionToCheck != null) {
				notEnemies.Add (notEnemy);
			}
		}
	}

	void removeNotEnemy (GameObject notEnemy)
	{
		if (notEnemies.Contains (notEnemy)) {
			notEnemies.Remove (notEnemy);
		}
	}

	//when there is one enemy or more, check which is the closest to shoot it.
	void closestTarget ()
	{
		if (enemies.Count > 0) {
			currentPosition = transform.position;

			float min = Mathf.Infinity;
			int index = -1;

			int enemiesCount = enemies.Count;

			for (int i = 0; i < enemiesCount; i++) {
				if (enemies [i] != null) {
					
					currentDistance = GKC_Utils.distance (enemies [i].transform.position, currentPosition);

					if (currentDistance < min) {
						min = currentDistance;
						index = i;
					}
				} else {
					enemies.RemoveAt (i);

					i = 0;

					enemiesCount = enemies.Count;
				}
			}

			if (index < 0) {
				return;
			}

			enemyToShoot = enemies [index];

			if (enemyToShoot != previousEnemy) {
				differentEnemy = true;
			} else {
				differentEnemy = false;
			}

			if (placeToShoot == null || differentEnemy) {
				placeToShoot = applyDamage.getPlaceToShoot (enemyToShoot);
			}

			if (applyDamage.checkIfDead (enemyToShoot)) {
				removeEnemy (enemyToShoot);

				return;
			}

			if (differentEnemy) {
				currentPlayerController = enemyToShoot.GetComponent<playerController> ();
			}

			if (currentPlayerController != null) {
				if (currentPlayerController.isPlayerDriving ()) {
					removeEnemy (enemyToShoot);

					targetIsCharacter = false;

					return;
				} else {
					targetIsCharacter = true;
					targetIsVehicle = false;
				}
			} else {
				if (differentEnemy) {
					currentVehicleHUDManager = enemyToShoot.GetComponent<vehicleHUDManager> ();
				}

				if (currentVehicleHUDManager != null) {
					if (!currentVehicleHUDManager.isVehicleBeingDriven ()) {
						
						removeEnemy (enemyToShoot);

						return;
					} else {
						targetIsCharacter = false;
						targetIsVehicle = true;
					}
				} else {
					targetIsCharacter = false;
				}
			}

			if (previousEnemy != enemyToShoot) {
				previousEnemy = enemyToShoot;
			}

			if (!onSpotted) {
				//the player can hack the turrets, but for that he has to crouch, so he can reach the back of the turret and activate the panel
				// if the player fails in the hacking or he gets up, the turret will detect the player and will start to fire him
				//check if the player fails or get up

				seeingCurrentTarget = false;

				if (checkRaycastToViewTarget) {
					seeingCurrentTarget = checkIfTargetIsPhysicallyVisible (enemyToShoot, false);
				} else {
					seeingCurrentTarget = true;
				}

				if (seeingCurrentTarget) {
					//if an enemy is inside the trigger, check its position with respect the AI, if the target is in the vision range, adquire it as target
					targetDirection = enemyToShoot.transform.position - currentPosition;

					float angleWithTarget = Vector3.SignedAngle (rayCastPosition.forward, targetDirection, enemyToShoot.transform.up);

					if (Mathf.Abs (angleWithTarget) < visionRange / 2 || hasBeenAttacked || enemyAlertActive || ignoreVisionRangeActive) { 
						if (currentPlayerController != null) {
							if ((!currentPlayerController.isCrouching () || hackFailed || checkingThreat || enemyAlertActive || hasBeenAttacked) &&
							    currentPlayerController.isCharacterVisibleToAI ()) {

								hasBeenAttacked = false;

								enemyAlertActive = false;

								hackFailed = false;
								targetAdquired ();
							}
						} else {
							//else, the target is a friend of the player, so shoot him
							targetAdquired ();
						}
					} else {
						//else check the distance, if the target is too close, adquire it as target too
						float distanceToTarget = GKC_Utils.distance (enemyToShoot.transform.position, currentPosition);

						if (distanceToTarget < minDistanceToAdquireTarget && (currentPlayerController.isCharacterVisibleToAI () || allowDetectionWhenTooCloseEvenNotVisible)) {
							targetAdquired ();
						}
						//print ("out of range of vision");
					}
				}
			}
		} 

		//if there are no enemies
		else {
			if (onSpotted) {
				placeToShoot = null;
				enemyToShoot = null;
				previousEnemy = null;

				onSpotted = false;

				fovTrigger.radius = originalFOVRadius;

				viewTrigger.gameObject.SetActive (true);

				hackFailed = false;

				SendMessage ("setOnSpottedState", false, SendMessageOptions.DontRequireReceiver);

				checkEventsOnSpotted (false);
			}
		}
	}

	public bool checkIfTargetIsPhysicallyVisible (GameObject targetToCheck, bool checkingSuspect)
	{
		targetToCheckRaycastPosition = transform.position + transform.up;

		if (placeToShoot != null) {
			targetToCheckDirection = placeToShoot.transform.position - targetToCheckRaycastPosition;
		} else {
			temporalPlaceToShoot = applyDamage.getPlaceToShoot (targetToCheck);
		
			if (temporalPlaceToShoot != null) {
				targetToCheckDirection = temporalPlaceToShoot.position - targetToCheckRaycastPosition;
			} else {
				targetToCheckDirection = (targetToCheck.transform.position + targetToCheck.transform.up) - targetToCheckRaycastPosition;
			}
		}

		targetToCheckPosition = targetToCheckRaycastPosition + targetToCheckDirection * checkIfTargetIsPhysicallyVisibleRadius;

		if (targetIsVehicle) {
			temporalVehicleHUDManagerToCheck = applyDamage.getVehicleHUDManager (targetToCheck);
		}

		currentObjectDetectedByRaycast = null;

		//Debug.DrawRay (targetToCheckPosition, targetToCheckDirection * 0.2f, Color.green);
		if (Physics.Raycast (targetToCheckPosition, targetToCheckDirection, out hit, Mathf.Infinity, layerToCheckRaycastToViewTarget)) {
			//print (hit.collider.gameObject.name + " " + targetIsVehicle);
			//print (hit.collider.name + " " + hit.transform.IsChildOf (targetToCheck.transform) + " " + targetToCheck.name);

			if ((!targetIsVehicle && (hit.collider.gameObject == targetToCheck || hit.transform.IsChildOf (targetToCheck.transform))) ||
			    (targetIsVehicle && temporalVehicleHUDManagerToCheck &&
			    ((temporalVehicleHUDManagerToCheck.gameObject == targetToCheck && checkingSuspect) || temporalVehicleHUDManagerToCheck.checkIfDetectSurfaceBelongToVehicle (hit.collider)))) {

				currentObjectDetectedByRaycast = hit.collider.gameObject;

				if (showGizmo) {
					Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.green, 2);
				}

				return true;
			} else {
				if (showGizmo) {
					Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.red, 2);
				}

				return false;
			}
		} else {
			if (showGizmo) {
				Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.black, 2);
			}
		}

		return false;
	}

	public void targetAdquired ()
	{
		//print ("target adquired");

		onSpotted = true;

		fovTrigger.radius = GKC_Utils.distance (enemyToShoot.transform.position, transform.position) + extraFieldOfViewRadiusOnSpot;

		if (fovTrigger.radius < originalFOVRadius) {
			fovTrigger.radius = originalFOVRadius;
		}

		viewTrigger.gameObject.SetActive (false);

		SendMessage ("setOnSpottedState", true, SendMessageOptions.DontRequireReceiver);

		//send this enemy to faction system for the detected enemies list
		factionManager.addDetectedEnemyFromFaction (enemyToShoot);

		setCharacterStateIcon (surprisedCharacterStateName);

		if (alertFactionOnSpotted) {
			factionManager.alertFactionOnSpotted (alertFactionRadius, enemyToShoot, transform.position);
		}

		checkEventsOnSpotted (true);
	}

	//active the fire mode
	public void shootTarget ()
	{
		//SendMessage (functionToShoot, true);
	}

	public void pauseAI (bool state)
	{
		paused = state;

		//SendMessage ("setPauseState", paused);
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkPossibleTarget (GameObject objectToCheck)
	{
		Collider currentCollider = objectToCheck.GetComponent<Collider> ();

		if (currentCollider != null) {
			checkTriggerInfo (currentCollider, true);
		}
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (canCheckSuspect (col.gameObject.layer)) {
			if (isEnter) {
				if (!paused) {
					if (checkCharacterFaction (col.gameObject, false)) {
						enemyDetected (col.gameObject);
					} else {
						addNotEnemey (col.gameObject);
					}
				}
			} else {
				if (checkCharacterFaction (col.gameObject, false)) {
					enemyLost (col.gameObject);
				} else {
					removeNotEnemy (col.gameObject);
				}
			}
		}
	}

	public bool canCheckSuspect (int suspectLayer)
	{
		if ((1 << suspectLayer & layerToCheckTargets.value) == 1 << suspectLayer) {
			return true;
		}

		return false;
	}

	public void setCorrectlyHackedState ()
	{
		setHackResult (true);
	}

	public void setIncorrectlyHackedState ()
	{
		setHackResult (false);
	}

	//check the result of the hacking, true the turret now is an ally, else, the turret detects the player
	public void setHackResult (bool state)
	{
		hacking = false;

		if (state) {
			enemyHackPanel currentEnemyHackPanel = hackDevice.GetComponent<enemyHackPanel> ();

			if (currentEnemyHackPanel != null) {
				currentEnemyHackPanel.disablePanelHack ();
			}

			tag = "friend";

			factionManager.changeCharacterToFaction (factionToChangeName);

			//if the turret becomes an ally, change its icon color in the radar
			mapObjectInformation currentMapObjectInformation = GetComponent<mapObjectInformation> ();

			if (currentMapObjectInformation != null) {
				currentMapObjectInformation.addMapObject ("Friend");
			}

			//set in the health slider the new name and slider color
			health currentHealth = GetComponent<health> ();

			if (currentHealth != null) {
				currentHealth.hacked ();
			}

			enemies.Clear ();

			notEnemies.Clear ();

			fullEnemyList.Clear ();
		} else {
			hackFailed = true;
		}
	}

	//the turret is been hacked
	public void activateHack ()
	{
		hacking = true;
	}

	public void checkCharactersAroundAI ()
	{
		for (int i = 0; i < notEnemies.Count; i++) {
			enemyDetected (notEnemies [i]);
		}
	}

	public void setCharacterStateIcon (string stateName)
	{
		if (characterStateIconManager != null) {
			characterStateIconManager.setCharacterStateIcon (stateName);
		}
	}

	public void disableCharacterStateIcon ()
	{
		if (characterStateIconManager != null) {
			characterStateIconManager.disableCharacterStateIcon ();
		}
	}

	//disable ai when it dies
	public void setAIStateToDead ()
	{
		enabled = false;
	}

	public void checkEventsOnSpotted (bool state)
	{
		if (useEventsOnSpotted) {
			if (state) {
				eventOnSpotted.Invoke ();
			} else {
				eventOnNoTargetsToAttack.Invoke ();
			}
		}
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			Gizmos.color = gizmoColor;

			Gizmos.DrawWireSphere (transform.position, alertFactionRadius);
		}
	}
}