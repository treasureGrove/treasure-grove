using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShieldSystem : abilityInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool shieldEnabled;

	public bool shieldActive;

	public bool laserActive;

	public bool laserAbilityEnabled = true;

	public LayerMask layerToDamage;

	public bool returnProjectilesDetectedEnabled = true;

	[Space]
	[Header ("Component Elements")]
	[Space]

	public GameObject shield;

	public armorSurfaceSystem armorSurfaceManager;

	public GameObject playerLaserGameObject;

	public laserPlayer laserPlayerManager;

	public otherPowers mainOtherPowers;

	public Transform pivotCameraTransform;
	public Transform playerCameraTransform;
	public Transform mainCameraTransform;

	public Collider playerCollider;

	GameObject currentLaser;

	Vector3 laserPosition;
	laserDevice.laserType lasertype;

	RaycastHit hit;


	public override void updateAbilityState ()
	{
		//enable shield when the player touch a laser
		if (shield.activeSelf && currentLaser != null) {
			Vector3 targetDir = currentLaser.transform.position - shield.transform.position;

			Quaternion qTo = Quaternion.LookRotation (targetDir);

			shield.transform.rotation = Quaternion.Slerp (shield.transform.rotation, qTo, 10 * Time.deltaTime);
		}

		//if the shield is enabled, the power decreases
		if (shield.activeSelf && shieldActive && !laserActive) {
			//also, rotates the shield towards the camera direction
			if (pivotCameraTransform.localRotation.x < 0) {
				shield.transform.rotation = pivotCameraTransform.rotation;
			} else {
				shield.transform.rotation = Quaternion.Euler (playerCameraTransform.eulerAngles);
			}

			mainPlayerAbilitiesSystem.checkAbilityUseEnergyInUpdate ();
		}

		if (mainOtherPowers.isAimingPower ()) {
			//if the player is touching by a laser device, enable the laser in the player
			if (laserActive && !playerLaserGameObject.activeSelf) {
				playerLaserGameObject.SetActive (true);
			}

			//else disable the laser
			if (!laserActive && playerLaserGameObject.activeSelf) {
				playerLaserGameObject.SetActive (false);
			}
		} else {
			if (playerLaserGameObject.activeSelf) {
				playerLaserGameObject.SetActive (false);
			}
		}
	}

	//enable and disable the shield when the player want to stop attacks or when he touchs a laser
	public void setActiveShieldState (bool state)
	{
		//enable or disable the shield
		if (!mainPlayerAbilitiesSystem.playerCurrentlyBusy) {
			setShieldState (state);
		}
	}

	public void setShieldState (bool state)
	{
		if (!laserActive && mainPlayerAbilitiesSystem.canMove && shieldEnabled) {
			shieldActive = state;

			shield.SetActive (shieldActive);
		}
	}

	//enable disable the laser in the hand of the player, when he is in the range of one
	public void activateLaserForceField (Vector3 pos)
	{
		if (!laserAbilityEnabled) {
			return;
		}

		if (!isCurrentAbility) {
			return;
		}

		//print ("enable laser force field");
		shieldActive = false;
		laserActive = true;

		disableAbilityCurrentActiveFromPressState ();

		if (laserActive) {
			if (!shield.activeSelf) {
				shield.SetActive (true);
			}

			laserPosition = pos;
		}

		if (playerLaserGameObject.activeSelf) {
			laserPlayerManager.setLaserInfo (lasertype, currentLaser, laserPosition);
		}
	}

	public void deactivateLaserForceField ()
	{
		//print ("disable laser force field");
		laserActive = false;

		if (shield.activeSelf) {
			shield.SetActive (false);
		}

		if (playerLaserGameObject.activeSelf) {
			playerLaserGameObject.SetActive (false);
		}

		laserPlayerManager.removeLaserInfo ();
	}

	public void setShieldEnabledState (bool state)
	{
		shieldEnabled = state;
	}

	//shoot the bullets and missiles catched by the shield
	public void returnEnemyProjectiles ()
	{
		//the bullets and missiles from the enemies are stored in the shield, so if the player press the right button of the mouse
		//the shoots are sent to its owners if they still alive, else, the shoots are launched in the camera direction
		if (!mainPlayerAbilitiesSystem.playerCurrentlyBusy && shield.activeSelf && shieldActive && !laserActive) {
			if (armorSurfaceManager.thereAreProjectilesStored ()) {
				//check if a raycast hits a surface from the center of the screen to forward
				//to set the direction of the projectiles in the shield
				Vector3 direction = mainCameraTransform.TransformDirection (Vector3.forward);

				bool surfaceFound = false;

				Vector3 raycastPosition = mainCameraTransform.position;

				if (Physics.Raycast (raycastPosition, direction, out hit, Mathf.Infinity, layerToDamage)) {
					if (hit.collider != playerCollider) {
						surfaceFound = true;
					} else {
						raycastPosition = hit.point + direction * 0.2f;

						if (Physics.Raycast (raycastPosition, direction, out hit, Mathf.Infinity, layerToDamage)) {
							surfaceFound = true;
						}
					}
				}

				if (surfaceFound) {
					direction = hit.point;
				}

				armorSurfaceManager.throwProjectilesStored (direction);
			}
		}
	}

	//get the laser device that touch the player, not enemy lasers, and if the laser reflects in other surfaces or not
	public void setLaser (GameObject l, laserDevice.laserType type)
	{
		currentLaser = l;

		lasertype = type;
	}

	//set the number of refractions in the laser in another function
	public void setLaserRefractionLimit (int value)
	{
		laserPlayerManager.reflactionLimit = value + 1;
	}

	public void setLaserAbilityEnabledState (bool state)
	{
		laserAbilityEnabled = state;
	}

	public override void enableAbility ()
	{
		shieldEnabled = true;
	}

	public override void disableAbility ()
	{
		shieldEnabled = false;

		if (shieldActive) {
			setShieldState (false);
		}

		deactivateLaserForceField ();
	}

	public override void deactivateAbility ()
	{
		if (shieldActive) {
			setShieldState (false);
		}

		deactivateLaserForceField ();
	}

	public override void activateSecondaryActionOnAbility ()
	{
		returnEnemyProjectiles ();
	}

	public override void useAbilityPressDown ()
	{
		if (shieldActive) {
			if (armorSurfaceManager.thereAreProjectilesStored ()) {
				if (returnProjectilesDetectedEnabled) {
					returnEnemyProjectiles ();

					return;
				}
			}
		}

		setActiveShieldState (!shieldActive);

		checkUseEventOnUseAbility ();
	}

	public override void useAbilityPressHold ()
	{

	}

	public override void useAbilityPressUp ()
	{

	}
}