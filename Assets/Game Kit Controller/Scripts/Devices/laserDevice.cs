using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class laserDevice : laser
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;
	public bool enablePlayerShield = true;
	public bool assigned;
	public GameObject laserConnector;
	public laserType lasertype;
	public float damageAmount;
	public bool ignoreShield;

	public int damageTypeID = -1;

	public bool canDamagePlayer;
	public bool canDamageCharacters;
	public bool canDamageVehicles;
	public bool canDamageEverything;
	public bool canKillWithOneHit;

	public bool sendMessageOnContact;

	public string shieldAbilityName = "Shield";

	public UnityEvent contantFunctions = new UnityEvent ();

	GameObject currentPlayer;

	playerShieldSystem currentPlayerShieldSystem;
	playerAbilitiesSystem currentPlayerAbilitiesSystem;

	bool forceFieldEnabled;
	RaycastHit hit;
	Vector3 hitPointPosition;
	float rayDistance;
	float hitDistance;
	bool hittingSurface;
	bool damageCurrentSurface;
	bool laserEnabled = true;

	public enum laserType
	{
		simple,
		refraction
	}

	GameObject lastObjectDetected;

	bool playerDetected;

	void Start ()
	{
		StartCoroutine (laserAnimation ());

		//get the initial raycast distance
		rayDistance = Mathf.Infinity;
	}

	void Update ()
	{
		if (laserEnabled) {
			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);



			//check if the hitted object is the player, enabling or disabling his shield
			if (Physics.Raycast (transform.position, transform.forward, out hit, rayDistance, layer)) {
				//if the laser has been deflected, then check if any object collides with it, to disable all the other reflections of the laser
				hittingSurface = true;

				laserDistance = hit.distance;
				hitPointPosition = hit.point;

				if (hit.collider.gameObject != lastObjectDetected) {
					lastObjectDetected = hit.collider.gameObject;

					if (sendMessageOnContact) {
						if (contantFunctions.GetPersistentEventCount () > 0) {
							contantFunctions.Invoke ();
						}
					}

					playerDetected = hit.transform.CompareTag ("Player");
				}

			} else {
				//the laser does not hit anything, so disable the shield if it was enabled
				hittingSurface = false;

				playerDetected = false;
			}

			if (hittingSurface) {

				if (currentPlayer == null && playerDetected) {
					currentPlayer = hit.collider.gameObject;

					playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

					if (currentPlayerComponentsManager != null) {
						currentPlayerAbilitiesSystem = currentPlayerComponentsManager.getPlayerAbilitiesSystem ();

						if (currentPlayerAbilitiesSystem != null) {
							currentPlayerShieldSystem = (playerShieldSystem)currentPlayerAbilitiesSystem.getAbilityByName (shieldAbilityName);
						}
					}
				}

				if (assigned) {
					forceFieldEnabled = false;

					if (enablePlayerShield) {
						if (currentPlayerShieldSystem != null) {
							currentPlayerShieldSystem.deactivateLaserForceField ();
						}
					}

					rayDistance = Mathf.Infinity;

					laserConnector.GetComponent<laserConnector> ().disableRefractionState ();
				} else {
					///the laser touchs the player, active his shield and set the laser that is touching him
					if (playerDetected && !hit.collider.isTrigger && !forceFieldEnabled) {
						if (currentPlayerShieldSystem != null) {
							currentPlayerShieldSystem.setLaser (gameObject, lasertype);
						}

						forceFieldEnabled = true;
					}

					if (forceFieldEnabled) {
						hitDistance = hit.distance;
						//set the position where this laser is touching the player
						Vector3 position = hit.point;

						if (enablePlayerShield) {
							if (currentPlayerShieldSystem != null) {
								currentPlayerShieldSystem.activateLaserForceField (position);
							}
						}

						//the laser has stopped to touch the player, so deactivate the player's shield
						if (!playerDetected) {
							forceFieldEnabled = false;

							if (enablePlayerShield) {
								if (currentPlayerShieldSystem != null) {
									currentPlayerShieldSystem.deactivateLaserForceField ();
								}
							}
						}
					}
				}

				if (canDamagePlayer && playerDetected) {
					damageCurrentSurface = true;
				} 

				if (canDamageCharacters) {
					if (applyDamage.isCharacter (hit.transform.gameObject)) {
						damageCurrentSurface = true;
					}
				}

				if (canDamageVehicles) {
					if (applyDamage.isVehicle (hit.transform.gameObject)) {
						damageCurrentSurface = true;
					}
				}

				if (canDamageEverything) {
					damageCurrentSurface = true;
				}

				if (damageCurrentSurface) {
					if (canKillWithOneHit) {
						applyDamage.killCharacter (gameObject, hit.transform.gameObject, -transform.forward, hit.point, gameObject, false);
					} else {
						applyDamage.checkHealth (gameObject, hit.transform.gameObject, damageAmount, -transform.forward, hit.point, 
							gameObject, true, true, ignoreShield, false, false, -1, damageTypeID);
					}
				}

				lRenderer.SetPosition (1, hitPointPosition);
			} else {
				if (!assigned) {
					if (forceFieldEnabled) {
						forceFieldEnabled = false;

						if (enablePlayerShield) {
							if (currentPlayerShieldSystem != null) {
								currentPlayerShieldSystem.deactivateLaserForceField ();
							}
						}

						//set to infinite the raycast distance again
						rayDistance = Mathf.Infinity;
					}		

					laserDistance = 1000;	
					lRenderer.SetPosition (1, (laserDistance * transform.forward));
				}
			}

			animateLaser ();
		}
	}

	void OnDisable ()
	{
		if (assigned) {
			forceFieldEnabled = false;

			if (currentPlayer != null) {
				if (enablePlayerShield) {
					if (currentPlayerShieldSystem != null) {
						currentPlayerShieldSystem.deactivateLaserForceField ();
					}
				}

				//set to infinite the raycast distance again
				rayDistance = Mathf.Infinity;
	
				//disable the laser connector
				laserConnector.GetComponent<laserConnector> ().disableRefractionState ();
			}
		}
	}

	//set the laser that it is touching the player, to assign it to the laser connector
	void assignLaser ()
	{
		assigned = true;
		rayDistance = hitDistance;

		if (enablePlayerShield) {
			if (currentPlayerShieldSystem != null) {
				currentPlayerShieldSystem.deactivateLaserForceField ();
			}
		}
	}

	public void setAssignLaserState (bool state)
	{
		assigned = state;
	}

	public void disableLaser ()
	{
		laserEnabled = false;
		lRenderer.enabled = false;
	}
}