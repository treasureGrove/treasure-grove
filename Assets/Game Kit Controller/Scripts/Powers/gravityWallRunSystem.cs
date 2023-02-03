using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class gravityWallRunSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool wallWalkEnabled;

	public float wallWalkRotationSpeed = 10;

	public bool setAnimSpeedMultiplierOnWallRun = true;
	public float animSpeedMultiplierOnWallRun = 2;

	public bool useExtraRaycastDetectionOnWallWalkEnabled = true;

	public bool stopGravityAdherenceWhenStopRun = true;

	public bool avoidRigidbodiesOnRaycastEnabled;

	public bool rotateToOriginalNormalOnAirEnabled = true;

	[Space]
	[Header ("Player Render Setting")]
	[Space]

	public Material runMat;

	public LayerMask layer;

	public bool trailsActive = true;
	public bool changePlayerMaterialsOnWallRun;

	[Space]
	[Header ("Player GameObject List")]
	[Space]

	public List<GameObject> playerGameObjectList = new List<GameObject> ();
	public List<GameObject> playerGameObjectWithChildList = new List<GameObject> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool wallWalkActive;

	public bool mainCoroutineActive;

	public List<Renderer> rendererParts = new List<Renderer> ();
	public List<Material> materialList = new List<Material> ();

	public List<TrailRenderer> trails = new List<TrailRenderer> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnWallWalkStart;
	public bool useEventOnWallWalkEnd;

	[Space]

	public UnityEvent eventOnWallWalkStart;
	public UnityEvent eventOnWallWalkEnd;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;
	public gravitySystem mainGravitySystem;

	public ragdollActivator mainRagdollActivator;

	public GameObject trailsPrefab;

	public Transform playerTransform;


	float lastTimeRunActivated;

	float lastTimeWallWalkActive;

	bool resetingSurfaceRotationAfterWallWalkActive;

	float lastTimeResetSurfaceRotationBelow;

	float trailTimer = -1;

	RaycastHit hit;

	bool checkRunGravity;

	Vector3 originalNormal;

	bool materialsStored;

	Coroutine mainCoroutine;

	float originalAnimSpeedMultiplierOnWallRun;

	bool forceWalkWallActiveExternally;

	void Start ()
	{
		if (wallWalkEnabled) {
			mainCoroutine = StartCoroutine (updateCoroutine ());

			mainCoroutineActive = true;
		}

		originalAnimSpeedMultiplierOnWallRun = animSpeedMultiplierOnWallRun;
	}

	public void stopUpdateCoroutine ()
	{
		if (mainCoroutine != null) {
			StopCoroutine (mainCoroutine);
		}

		mainCoroutineActive = false;
	}

	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			if (!mainPlayerController.playerIsBusy ()) {
				checkIfActivateWallWalk ();

				updateWallWalkState ();
			}

			//just a configuration to the trails in the player
			if (trailsActive) {
				if (trailTimer > 0) {
					trailTimer -= Time.deltaTime;

					for (int j = 0; j < trails.Count; j++) {
						if (trails [j] != null) {
							trails [j].time -= Time.deltaTime;
						}
					}
				}

				if (trailTimer <= 0 && trailTimer > -1) {
					for (int j = 0; j < trails.Count; j++) {
						if (trails [j] != null) {
							trails [j].enabled = false;
						}
					}

					trailTimer = -1;
				}

				if (trailTimer == -1) {
					if (!wallWalkEnabled) {
						stopUpdateCoroutine ();
					}
				}
			}

			yield return waitTime;
		}
	}

	void updateWallWalkState ()
	{
		if (wallWalkActive && wallWalkEnabled) {
			if (resetingSurfaceRotationAfterWallWalkActive) {
				if (mainGravitySystem.isCharacterRotatingToSurface ()) {
					return;
				} else {
					resetingSurfaceRotationAfterWallWalkActive = false;

					lastTimeResetSurfaceRotationBelow = Time.time;
				}
			}

			if (lastTimeResetSurfaceRotationBelow > 0) {
				if (Time.time > lastTimeResetSurfaceRotationBelow + 1) {
					lastTimeResetSurfaceRotationBelow = 0;
				} else {
					return;
				}
			}

			if (mainPlayerController.isWallRunningActive () || mainPlayerController.isExternalControlBehaviorForAirTypeActive ()) {
				stopWallWalk ();
			}

			//check a surface in front of the player, to rotate to it
			bool surfaceInFrontDetected = false;

			Vector3 playerPosition = playerTransform.position;

			Vector3 playerTransformUp = playerTransform.up;
			Vector3 playerTransforForward = playerTransform.forward;

			Vector3 raycastPosition = playerPosition + playerTransformUp;
			Vector3 raycastDirection = playerTransforForward;

			bool ignoreSurfaceDetected = false;

			if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 2, layer)) {
				if (avoidRigidbodiesOnRaycastEnabled) {
					if (hit.rigidbody != null) {
						ignoreSurfaceDetected = true;
					}
				}

				if (!hit.collider.isTrigger && !ignoreSurfaceDetected) {
					mainGravitySystem.checkRotateToSurfaceWithoutParent (hit.normal, wallWalkRotationSpeed);

					mainPlayerController.setCurrentNormalCharacter (hit.normal);

					surfaceInFrontDetected = true;
				}
			} 

			if (useExtraRaycastDetectionOnWallWalkEnabled) {
				if (!surfaceInFrontDetected && mainPlayerController.isPlayerOnGround () && !mainGravitySystem.isCharacterRotatingToSurface ()) {
					Vector3 heading = (playerPosition + playerTransforForward) - raycastPosition;

					float distance = heading.magnitude;

					raycastDirection = heading / distance;

					ignoreSurfaceDetected = false;

					if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 4, layer)) {
						if (avoidRigidbodiesOnRaycastEnabled) {
							if (hit.rigidbody != null) {
								ignoreSurfaceDetected = true;
							}
						}

						if (!ignoreSurfaceDetected && !hit.collider.isTrigger && mainGravitySystem.getCurrentNormal () != hit.normal) {
							mainGravitySystem.checkRotateToSurfaceWithoutParent (hit.normal, wallWalkRotationSpeed);

							mainPlayerController.setCurrentNormalCharacter (hit.normal);

							surfaceInFrontDetected = true;
						}
					}

					if (surfaceInFrontDetected) {
						raycastPosition = playerPosition + playerTransformUp + playerTransforForward;

						raycastDirection = -playerTransformUp;

						ignoreSurfaceDetected = false;

						if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 2, layer)) {
							if (avoidRigidbodiesOnRaycastEnabled) {
								if (hit.rigidbody != null) {
									ignoreSurfaceDetected = true;
								}
							}

							if (!ignoreSurfaceDetected && !hit.collider.isTrigger && mainGravitySystem.getCurrentNormal () != hit.normal) {
								mainGravitySystem.checkRotateToSurfaceWithoutParent (hit.normal, wallWalkRotationSpeed);

								mainPlayerController.setCurrentNormalCharacter (hit.normal);

								surfaceInFrontDetected = true;
							}
						}
					}
				}
			}

			//check if the player is too far from his current ground, to rotate to his previous normal
			if (rotateToOriginalNormalOnAirEnabled) {
				if (!surfaceInFrontDetected && !Physics.Raycast (playerPosition + playerTransformUp, -playerTransformUp, out hit, 5, layer)) {
					if (mainGravitySystem.getCurrentRotatingNormal () != originalNormal && !checkRunGravity) {
						checkRunGravity = true;

						if (mainGravitySystem.isCharacterRotatingToSurface ()) {
							mainGravitySystem.stopRotateToSurfaceWithOutParentCoroutine ();
						}

						mainGravitySystem.checkRotateToSurface (originalNormal, 2);

						mainPlayerController.setCurrentNormalCharacter (originalNormal);

						resetingSurfaceRotationAfterWallWalkActive = true;
					}

					if (checkRunGravity && mainGravitySystem.getCurrentRotatingNormal () == originalNormal) {
						checkRunGravity = false;
					}
				}
			}
		}
	}

	public void checkIfActivateWallWalk ()
	{
		if (wallWalkEnabled) {
			if (!wallWalkActive && !mainPlayerController.isWallRunningActive () || mainPlayerController.isExternalControlBehaviorForAirTypeActive ()) {
				if (forceWalkWallActiveExternally && mainPlayerController.isPlayerMoving (1) && mainPlayerController.isPlayerOnGround ()) {
					mainPlayerController.inputStartToRun ();

					originalNormal = mainGravitySystem.getCurrentRotatingNormal ();

					lastTimeRunActivated = Time.time;

					wallWalkActive = true;	

					activateWallWalk ();

					return;
				}

				if (lastTimeWallWalkActive > 0) {
					if (Time.time > lastTimeWallWalkActive + 1) {
						lastTimeWallWalkActive = 0;
					}
				}

				//check the amount of time that the button is being pressed
				if (lastTimeRunActivated == 0) {

					if (mainPlayerController.isPlayerRunning ()) {
						originalNormal = mainGravitySystem.getCurrentRotatingNormal ();

						lastTimeRunActivated = Time.time;
					}
				}

				if (lastTimeRunActivated > 0) {

					if (mainPlayerController.getRunButtonPressedTimer () > 0.5f) {
						if (!resetingSurfaceRotationAfterWallWalkActive || !mainGravitySystem.isCharacterRotatingToSurface ()) {
							if (!mainGravitySystem.isCurcumnavigating ()) {
								wallWalkActive = true;	

								activateWallWalk ();
							}
						}
					}

					if (!mainPlayerController.isPlayerRunning ()) {
						lastTimeRunActivated = 0;
					}
				}
			} else {
				if (!mainPlayerController.isPlayerRunning ()) {

					lastTimeRunActivated = 0;

					stopWallWalk ();
				}
			}
		}
	}

	//if the player runs, a set of parameters are changed, like the speed of movement, animation, jumppower....
	public void activateWallWalk ()
	{
		if (wallWalkEnabled) {
			if (rendererParts.Count == 0) {
				storePlayerRenderer ();
			}

			if (!materialsStored) {
				getMaterialList ();

				materialsStored = true;
			}

			bool firstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

			if (trailsActive && !firstPersonActive) {
				for (int j = 0; j < trails.Count; j++) {
					if (trails [j] != null) {
						trails [j].enabled = true;
						trails [j].time = 1;
					}
				}

				trailTimer = -1;
			}

			if (changePlayerMaterialsOnWallRun) {
				int rendererPartsCount = rendererParts.Count;

				for (int i = 0; i < rendererPartsCount; i++) {
					Renderer currentRenderer = rendererParts [i];

					if (currentRenderer != null) {

						int materialsLength = currentRenderer.materials.Length;

						Material[] allMats = currentRenderer.materials;

						for (int j = 0; j < materialsLength; j++) {
							allMats [j] = runMat;
						}

						currentRenderer.materials = allMats;
					}
				}
			}

			if (setAnimSpeedMultiplierOnWallRun) {
				mainPlayerController.setReducedVelocity (animSpeedMultiplierOnWallRun);
			}

			checkEventsOnWallWalk (true);
		}
	}

	//when the player stops running, those parameters back to their normal values
	public void stopWallWalk ()
	{
		if (wallWalkActive) {
			resetingSurfaceRotationAfterWallWalkActive = false;

			if (trailsActive) {
				trailTimer = 2;
			}

			if (changePlayerMaterialsOnWallRun) {

				int rendererPartsCount = rendererParts.Count;

				int currentMaterialIndex = 0;

				for (int i = 0; i < rendererPartsCount; i++) {
					Renderer currentRenderer = rendererParts [i];

					if (currentRenderer != null) {

						int materialsLength = currentRenderer.materials.Length;

						Material[] allMats = currentRenderer.materials;

						for (int j = 0; j < materialsLength; j++) {
							allMats [j] = materialList [currentMaterialIndex];

							currentMaterialIndex++;
						}

						currentRenderer.materials = allMats;
					}
				}
			}

			if (stopGravityAdherenceWhenStopRun) {
				if (mainGravitySystem.getCurrentRotatingNormal () != originalNormal) {
					if (mainGravitySystem.isCharacterRotatingToSurface ()) {
						mainGravitySystem.stopRotateToSurfaceWithOutParentCoroutine ();
					}

					mainGravitySystem.checkRotateToSurface (originalNormal, 2);

					resetingSurfaceRotationAfterWallWalkActive = true;
				}

				mainPlayerController.setCurrentNormalCharacter (originalNormal);
			} else {
				if (mainGravitySystem.isUsingRegularGravity ()) {
					mainGravitySystem.changeColor (false);
				} else {
					mainGravitySystem.changeColor (true);
				}
			}

			wallWalkActive = false;

			lastTimeWallWalkActive = Time.time;

			lastTimeResetSurfaceRotationBelow = 0;

			if (setAnimSpeedMultiplierOnWallRun) {
				mainPlayerController.setNormalVelocity ();
			}

			checkEventsOnWallWalk (false);
		}
	}

	public void setAnimSpeedMultiplierOnWallRunValue (float newValue)
	{
		animSpeedMultiplierOnWallRun = newValue;
	}

	public void setOriginalAnimSpeedMultiplierOnWallRunValue ()
	{
		setAnimSpeedMultiplierOnWallRunValue (originalAnimSpeedMultiplierOnWallRun);
	}

	public bool isWallWalkActive ()
	{
		return wallWalkActive;
	}

	public void setWalkWallEnabledState (bool state)
	{
		wallWalkEnabled = state;

		if (wallWalkEnabled) {
			mainCoroutine = StartCoroutine (updateCoroutine ());

			mainCoroutineActive = true;
		} else {
			if (wallWalkActive) {
				stopWallWalk ();
			}

			if (!trailsActive) {
				stopUpdateCoroutine ();
			}
		}
	}

	public void toggleWalkWallEnabledState ()
	{
		setWalkWallEnabledState (!wallWalkEnabled);
	}

	public void disableWalkWallState ()
	{
		if (wallWalkActive) {
			stopWallWalk ();
		}

		setWalkWallEnabledState (false);
	}

	public void setForceWalkWallActiveExternallyState (bool state)
	{
		forceWalkWallActiveExternally = state;
	}

	public void enableOrDisableWalkWallExternally (bool state, bool activateRunAction)
	{
		if (state) {
			toggleWalkWallEnabledState ();

			if (activateRunAction) {
				if (!mainPlayerController.isPlayerRunning ()) {
					mainPlayerController.inputStartToRun ();

					originalNormal = mainGravitySystem.getCurrentRotatingNormal ();

					lastTimeRunActivated = Time.time;
			
					wallWalkActive = true;	

					activateWallWalk ();
				}
			}
		} else {
			disableWalkWallState ();

			lastTimeRunActivated = 0;

			if (!activateRunAction) {
				if (mainPlayerController.isPlayerRunning ()) {
					mainPlayerController.forceStopRun ();

				}
			}
		}
	}

	void checkEventsOnWallWalk (bool state)
	{
		if (state) {
			if (useEventOnWallWalkStart) {
				eventOnWallWalkStart.Invoke ();
			}
		} else {
			if (useEventOnWallWalkEnd) {
				eventOnWallWalkEnd.Invoke ();
			}
		}
	}

	public void setMeshesInfo ()
	{
		trails.Clear ();

		if (trailsPrefab != null) {

			List<Transform> trailsPositions = mainRagdollActivator.getBodyPartsTransformList ();

			for (int i = 0; i < trailsPositions.Count; i++) {
				GameObject trailClone = (GameObject)Instantiate (trailsPrefab, Vector3.zero, Quaternion.identity);

				//remove the clone string inside the instantiated object
				trailClone.name = "Run Power Trail " + (i + 1).ToString ();
				trailClone.transform.SetParent (trailsPositions [i]);
				trailClone.transform.localPosition = Vector3.zero;

				//trailClone.transform.localRotation = Quaternion.identity;
				TrailRenderer currentTrailRenderer = trailClone.GetComponent<TrailRenderer> ();
				currentTrailRenderer.enabled = false;

				trails.Add (currentTrailRenderer);
			}
		}

		storePlayerRendererFromEditor ();

		updateComponent ();
	}

	public void getMaterialList ()
	{
		if (materialList.Count == 0) {
			int rendererPartsCount = rendererParts.Count;

			for (int i = 0; i < rendererPartsCount; i++) {
				Renderer currentRenderer = rendererParts [i];

				if (currentRenderer != null) {

					int materialsLength = currentRenderer.materials.Length;

					for (int j = 0; j < materialsLength; j++) {

						Material currentMaterial = currentRenderer.materials [j];

						if (currentMaterial != null) {

							materialList.Add (currentMaterial);
						}
					}
				}
			}
		}
	}

	public void clearRendererList ()
	{
		rendererParts.Clear ();
	}

	public void storePlayerRenderer ()
	{
		clearRendererList ();

		for (int i = 0; i < playerGameObjectList.Count; i++) {
			GameObject currentObject = playerGameObjectList [i];

			if (currentObject != null) {
				Renderer currentRenderer = currentObject.GetComponent<Renderer> ();

				rendererParts.Add (currentRenderer);
			}
		}

		for (int i = 0; i < playerGameObjectWithChildList.Count; i++) {
			GameObject currentObject = playerGameObjectWithChildList [i];

			if (currentObject != null) {
				Component[] components = currentObject.GetComponentsInChildren (typeof(Renderer));
				foreach (Renderer child in components) {

					rendererParts.Add (child);
				}
			}
		}

		print ("Total amount of " + rendererParts.Count + " render found");
	}

	public void storePlayerRendererFromEditor ()
	{
		storePlayerRenderer ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}