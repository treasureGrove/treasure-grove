using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class armorSurfaceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool armorActive = true;
	public GameObject armorOwner;

	public bool useMinAngleToReturnProjectileToOwner;
	public float minAngleToReturnProjectileToOwner;

	[Space]
	[Header ("Return Projectiles Settings")]
	[Space]

	public bool throwProjectilesToPreviousOwnerEnabled = true;

	public bool useNewDamageLayerOnReturnProjectiles;
	public LayerMask newDamageLayerOnReturnProjectiles;

	public bool setUseCustomIgnoreTagsOnReturnProjectilesState;
	public bool useCustomIgnoreTagsOnReturnProjectilesState;
	public List<string> customTagsToIgnoreOnReturnProjectilesList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public List<projectileSystem> projectilesStored = new List<projectileSystem> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool returnProjectilesOnContact;

	public UnityEvent eventToReturnProjectilesOnContact;


	public bool useEventOnDeflectProjectilesActivated;

	public UnityEvent eventOnDeflectProjectilesActivated;

	[Space]
	[Header ("Components")]
	[Space]

	public BoxCollider mainCollider;

	Coroutine armorSurfaceStateChangeCoroutine;

	bool changeArmorActiveStateCoroutineActive;


	void Start ()
	{
		if (armorOwner == null) {
			armorOwner = gameObject;
		}
	}

	public void addProjectile (projectileSystem newProjectile)
	{
		if (!projectilesStored.Contains (newProjectile)) {
			projectilesStored.Add (newProjectile);

			if (returnProjectilesOnContact) {
				eventToReturnProjectilesOnContact.Invoke ();
			}
		}
	}

	public void throwProjectilesStored (Vector3 throwDirection)
	{
		bool ignoreProjectileOwner = !throwProjectilesToPreviousOwnerEnabled;

		for (int i = 0; i < projectilesStored.Count; i++) {
			if (projectilesStored [i] != null) {
				
				projectilesStored [i].returnBullet (throwDirection, armorOwner, ignoreProjectileOwner);
			}
		}

		projectilesStored.Clear ();
	}

	public void throwProjectilesStoredCheckingDirection (Transform playerTransform, Transform mainCameraTransform)
	{
		if (projectilesStored.Count > 0) {
			if (useEventOnDeflectProjectilesActivated) {
				eventOnDeflectProjectilesActivated.Invoke ();
			}

			for (int i = 0; i < projectilesStored.Count; i++) {
				if (projectilesStored [i] != null) {

					Vector3 projectileDirection = playerTransform.position - projectilesStored [i].transform.position;

					projectileDirection = projectileDirection / projectileDirection.magnitude;

					float angleForward = Vector3.SignedAngle (playerTransform.forward, projectileDirection, playerTransform.up);

					bool returnProjectileToOwner = true;

					if (useMinAngleToReturnProjectileToOwner) {
						if (Mathf.Abs (angleForward) > minAngleToReturnProjectileToOwner) { 
							returnProjectileToOwner = false;
						}
					}

					bool ignoreProjectileOwner = false;

					if (!returnProjectileToOwner) {
						ignoreProjectileOwner = true;
					}
					
					Debug.DrawRay (projectilesStored [i].transform.position, projectileDirection * 5, Color.red, 5);

					Vector3 targetPositionToLook = playerTransform.position + playerTransform.up + playerTransform.forward;

					if (useNewDamageLayerOnReturnProjectiles) {
						projectilesStored [i].setTargetToDamageLayer (newDamageLayerOnReturnProjectiles);
					}

					if (setUseCustomIgnoreTagsOnReturnProjectilesState) {
						projectilesStored [i].setUseCustomIgnoreTags (useCustomIgnoreTagsOnReturnProjectilesState, customTagsToIgnoreOnReturnProjectilesList);
					}
					
					projectilesStored [i].returnBullet (targetPositionToLook, armorOwner, ignoreProjectileOwner);
				}
			}

			projectilesStored.Clear ();
		}
	}

	public bool thereAreProjectilesStored ()
	{
		return projectilesStored.Count > 0;
	}

	public bool isArmorEnabled ()
	{
		return armorActive;
	}

	public void setNewArmorOwner (GameObject newObject)
	{
		armorOwner = newObject;
	}

	public void setArmorActiveState (bool state)
	{
		armorActive = state;

		if (changeArmorActiveStateCoroutineActive) {
			stopSetArmorSurfaceStateAfterDelayCoroutine ();
		}
	}

	public void setTriggerScaleValues (Vector3 newCenterValues, Vector3 newSizeValues)
	{
		if (mainCollider != null) {
			mainCollider.center = newCenterValues;

			mainCollider.size = newSizeValues;
		}
	}

	public void setEnableArmorSurfaceStateWithDuration (float delayDuration, bool stateBeforeDelay, bool stateAfterDelay)
	{
		stopSetArmorSurfaceStateAfterDelayCoroutine ();

		if (gameObject.activeSelf && gameObject.activeInHierarchy) {
			armorSurfaceStateChangeCoroutine = StartCoroutine (setArmorSurfaceStateAfterDelayCoroutine (delayDuration, stateBeforeDelay, stateAfterDelay));
		}
	}

	public void stopSetArmorSurfaceStateAfterDelayCoroutine ()
	{
		if (armorSurfaceStateChangeCoroutine != null) {
			StopCoroutine (armorSurfaceStateChangeCoroutine);
		}

		changeArmorActiveStateCoroutineActive = false;
	}

	IEnumerator setArmorSurfaceStateAfterDelayCoroutine (float delayDuration, bool stateBeforeDelay, bool stateAfterDelay)
	{
		changeArmorActiveStateCoroutineActive = true;

		armorActive = stateBeforeDelay;

		yield return new WaitForSeconds (delayDuration);

		armorActive = stateAfterDelay;

		changeArmorActiveStateCoroutineActive = false;
	}
}