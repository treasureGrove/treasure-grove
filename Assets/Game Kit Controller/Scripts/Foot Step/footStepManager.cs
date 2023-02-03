using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class footStepManager : MonoBehaviour
{
	public bool stepsEnabled = true;
	public bool soundsEnabled;

	public bool useFeetVolumeRangeClamps;
	public Vector2 feetVolumeRangeClamps = new Vector2 (0.8f, 1.2f);

	public bool useFootStepStateList;
	public string currentFootStepStateName;
	public LayerMask noiseDetectionLayer;
	public List<footStepState> footStepStateList = new List<footStepState> ();

	footStepState currentFootStepState;
	footStepState temporalFootStepState;

	bool currentFootStepStateAssigned;

	public string defaultSurfaceName = "Concrete";
	public string defaultGroundFootStepState = "Walking";

	public footStepType typeOfFootStep;
	public LayerMask layer;

	public bool usePoolingSystemEnabled = true;

	public bool useFootPrints;
	public GameObject rightFootPrint;
	public GameObject leftFootPrint;
	public bool useFootPrintsFromStates;

	public bool useFootPrintMaxAmount;
	public int footPrintMaxAmount;
	public float timeToRemoveFootPrints;
	public float maxFootPrintDistance;
	public float distanceBetweenPrintsInFisrtPerson;
	public bool removeFootPrintsInTime;
	public bool vanishFootPrints;
	public float vanishSpeed;

	public bool useFootParticles;
	public GameObject footParticles;
	public bool useFootParticlesFromStates;

	public footStepsLayer[] footSteps;
	public GameObject leftFoot;
	public GameObject rightFoot;

	public bool useNoiseMesh = true;
	public bool showNoiseDetectionGizmo;

	public string mainNoiseMeshManagerName = "Noise Mesh Manager";

	public playerController playerManager;
	public footStep leftFootStep;
	public footStep rightFootStep;

	public Collider leftFootCollider;
	public Collider rightFootCollider;
	public AudioSource cameraAudioSource;

	int surfaceIndex;
	float lastFootstepTime = 0;
	GameObject currentSurface;
	RaycastHit hit;

	bool usingAnimator = true;

	Transform footPrintsParent;

	public enum footStepType
	{
		triggers,
		raycast
	}

	List<GameObject> footPrints = new List<GameObject> ();
	float destroyFootPrintsTimer;
	bool soundFound = false;
	int footStepIndex = -1;
	int poolSoundIndex = -1;
	bool footPrintsNotEmpty;

	GameObject footPrintToRemove;

	noiseMeshSystem noiseMeshManager;
	bool noiseMeshManagerFound;

	Transform currentFootPrintParent;

	GameObject currentObjectBelowPlayer;
	GameObject previousObjectBelowPlayer;

	Terrain currentTerrain;
	TerrainData terrainData;
	Vector3 currentTerrainPosition;

	AudioClip currentSoundEffect;
	bool terrainDetected;

	footStepSurfaceSystem currentFootStepSurfaceSystem;
	string currentSurfaceName;

	Vector3 raycastDirection;

	bool wallRunningActive;
	bool wallRunningOnRightSide;

	bool footStepPaused;

	bool footStepsCanBePlayed;

	Vector2 currentVolumeRange;
	float currentVolumeValue;

	Vector2 newVolumeRange;

	GameObject currentFootPrint;

	GameObject currentSurfaceOnRightFoot;
	GameObject currentSurfaceOnLeftFoot;
	GameObject previousSurfaceOnRightFoot;
	GameObject previousSurfaceOnLeftFoot;

	Transform currentSurfaceParentOnRightFoot;
	Transform currentSurfaceParentOnLeftFoot;

	bool delayToNewStateActive;

	Coroutine setNewStateAfterDelayCoroutine;

	bool usingCenterSideToLeft;

	footStepsLayer currentFooStep;

	GameObject newFootPrint;
	GameObject newFootParticle;

	int footStepStateListCount;

	int footStepsLength;

	void Start ()
	{
		if (useFootPrints || useFootParticles) {
			GameObject printsParent = GameObject.Find ("Foot Prints Parent");

			if (printsParent != null) {
				footPrintsParent = printsParent.transform;
			} else {
				printsParent = new GameObject ();
				printsParent.name = "Foot Prints Parent";
				footPrintsParent = printsParent.transform;
			}
		}

		footStepStateListCount = footStepStateList.Count;

		footStepsLength = footSteps.Length;

		if (useFootStepStateList) {
			setFootStepState (currentFootStepStateName);
		} else {
			setDefaultFeelVolumeRange ();
		}

		currentSurfaceName = defaultSurfaceName;
	}

	void Update ()
	{
		//if the player doesn't use the animator when the first person view is enabled, the footsteps in the feet of the player are disabled
		//so checkif the player is moving, and then play the steps sounds according to the stepInterval and the surface detected with a raycast under the player
		if (stepsEnabled && !footStepPaused) {
			if (wallRunningActive) {
				if (wallRunningOnRightSide) {
					raycastDirection = transform.right;
				} else {
					raycastDirection = -transform.right;
				}
			} else {
				raycastDirection = -transform.up;
			}

			if (!usingAnimator && currentFootStepStateAssigned) {
				if (((playerManager.isPlayerOnGround () || wallRunningActive || currentFootStepState.ignoreOnGround) &&
				    (playerManager.isPlayerMoving (0)) && !playerManager.isObstacleToAvoidMovementFound ()) ||
				    (currentFootStepState.playSoundOnce && !currentFootStepState.soundPlayed)) {
			
					if (Physics.Raycast (transform.position + transform.up * .1f, raycastDirection, out hit, .5f, layer) ||
					    !currentFootStepState.checkPlayerOnGround) {
						//get the gameObject under the player's feet
						if (currentFootStepState.checkPlayerOnGround) {
							currentSurface = hit.collider.gameObject;
						}

						//check the footstep frequency
						if (Time.time > lastFootstepTime + currentFootStepState.stepInterval / playerManager.animSpeedMultiplier) {
							//get the audio clip according to the type of surface, mesh or terrain
							if (currentFootStepState.playCustomSound) {
								if (currentFootStepState.useCustomSoundList) {
									int randomClipIndex = Random.Range (0, currentFootStepState.poolCustomSounds.Length);

									currentSoundEffect = currentFootStepState.poolCustomSounds [randomClipIndex];
								} else {
									currentSoundEffect = currentFootStepState.customSound;

								}
							} else {
								if (currentSurface != null) {
									currentSoundEffect = getSound (transform.position, currentSurface, footStep.footType.center);
								}
							}

							if (currentSoundEffect != null) {
								if (soundsEnabled) {
									currentVolumeRange = currentFootStepState.feetVolumeRange;
									currentVolumeValue = Random.Range (currentVolumeRange.x, currentVolumeRange.y);

									if (useFeetVolumeRangeClamps) {
										currentVolumeValue = Mathf.Clamp (currentVolumeValue, feetVolumeRangeClamps.x, feetVolumeRangeClamps.y);
									}

									//play one shot of the audio
									cameraAudioSource.PlayOneShot (currentSoundEffect, currentVolumeValue);
								}

								lastFootstepTime = Time.time;
							}
						
							if (currentFootStepState.playSoundOnce) {
								currentFootStepState.soundPlayed = true;
							}
						}
					}
				}
			}
		}

		if (useFootPrints && removeFootPrintsInTime && !usePoolingSystemEnabled) {
			if (footPrintsNotEmpty) {
				destroyFootPrintsTimer += Time.deltaTime;

				if (destroyFootPrintsTimer > timeToRemoveFootPrints) {
					int footPrintsCount = footPrints.Count;

					for (int i = 0; i < footPrintsCount; i++) {
						if (footPrints [i] != null) {
							Destroy (footPrints [i]);
						}
					}

					footPrints.Clear ();

					destroyFootPrintsTimer = 0;

					footPrintsNotEmpty = false;
				}
			}
		}
	}

	public void changeFootStespType (bool state)
	{
		if (typeOfFootStep == footStepType.raycast) {
			state = false;
		}

		if (leftFoot.activeSelf != state) {
			leftFoot.SetActive (state);
		}

		if (rightFoot.activeSelf != state) {
			rightFoot.SetActive (state);
		}

		usingAnimator = state;
	}

	public int GetMainTexture (Vector3 playerPos)
	{
		//get the index of the current texture of the terrain where the player is walking
		//calculate which splat map cell the playerPos falls within
		int mapX = (int)(((playerPos.x - currentTerrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
		int mapZ = (int)(((playerPos.z - currentTerrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

		//get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
		float[,,] splatmapData = terrainData.GetAlphamaps (mapX, mapZ, 1, 1);

		//change the 3D array data to a 1D array:
		float[] cellMix = new float[splatmapData.GetUpperBound (2) + 1];

		int cellMixLength = cellMix.Length;

		for (int n = 0; n < cellMixLength; n++) {
			cellMix [n] = splatmapData [0, 0, n];    
		}

		float maxMix = 0;
		int maxIndex = 0;

		//loop through each mix value and find the maximum
		for (int n = 0; n < cellMixLength; n++) {
			if (cellMix [n] > maxMix) {
				maxIndex = n;
				maxMix = cellMix [n];
			}
		}

		//print (terrainData.splatPrototypes [maxIndex].texture.name + " " + maxIndex);
		return maxIndex;
	}

	//get the audio clip, according to the layer of the object under the player, the position of the player, and the ground itself
	public AudioClip getSound (Vector3 pos, GameObject ground, footStep.footType footSide)
	{
		if (playerManager.isPlayerDead ()) {
			return null;
		}

		if (!footStepsCanBePlayed) {
			if (Time.time > 1) {
				footStepsCanBePlayed = true;
			} else {
				return null;
			}
		}

		soundFound = false;
		footStepIndex = -1;
		poolSoundIndex = -1;

		currentObjectBelowPlayer = ground;

		if (previousObjectBelowPlayer != currentObjectBelowPlayer) {
			previousObjectBelowPlayer = currentObjectBelowPlayer;

			currentTerrain = ground.GetComponent<Terrain> ();
		
			terrainDetected = currentTerrain != null;

			if (!terrainDetected) {
				currentFootStepSurfaceSystem = currentObjectBelowPlayer.GetComponent<footStepSurfaceSystem> ();

				if (currentFootStepSurfaceSystem != null) {
					currentSurfaceName = currentFootStepSurfaceSystem.getSurfaceName ();
				} else {
					currentSurfaceName = defaultSurfaceName;
				}
			} else {
				currentTerrainPosition = currentTerrain.transform.position;

				terrainData = currentTerrain.terrainData;
			}
		}

		//if the player is in a terrain
		if (terrainDetected) {
			//get the current texture index of the terrain under the player.
			surfaceIndex = GetMainTexture (pos);

			for (int i = 0; i < footStepsLength; i++) {
				currentFooStep = footSteps [i];

				//check if that terrain texture has a sound
				if (currentFooStep.checkTerrain && surfaceIndex == currentFooStep.terrainTextureIndex) {
					int index = -1;

					if (currentFooStep.randomPool) {
						//get a random sound
						index = Random.Range (0, currentFooStep.poolSounds.Length);
					} else {
						//get the next sound in the list
						currentFooStep.poolIndex++;

						if (currentFooStep.poolIndex > currentFooStep.poolSounds.Length - 1) {
							currentFooStep.poolIndex = 0;
						}

						index = currentFooStep.poolIndex;
					}

					soundFound = true;
					footStepIndex = i;
					poolSoundIndex = index;
				}
			}
		} 

		//else, the player is above a mesh
		else {
			surfaceIndex = -1;

			for (int i = 0; i < footStepsLength; i++) {
				currentFooStep = footSteps [i];

				//check if the layer of the mesh has a sound 
				if (currentFooStep.checkSurfaceSystem && currentSurfaceName.Equals (currentFooStep.Name)) {
					int index = -1;

					if (currentFooStep.randomPool) {
						//get a random sound
						index = Random.Range (0, currentFooStep.poolSounds.Length);
					} else {
						//get the next sound in the list
						currentFooStep.poolIndex++;

						if (currentFooStep.poolIndex > currentFooStep.poolSounds.Length - 1) {
							currentFooStep.poolIndex = 0;
						}

						index = currentFooStep.poolIndex;
					}

					soundFound = true;
					footStepIndex = i;
					poolSoundIndex = index;
				}
			}
		}

		if (soundFound) {
			placeFootPrint (footSide, footStepIndex);

			createParticles (footSide, footStepIndex);

			setNoiseOnStep (pos);
			//return the audio selected
			return footSteps [footStepIndex].poolSounds [poolSoundIndex];
		}

		return null;
	}

	public void placeFootPrint (footStep.footType footSide, int footStepIndex)
	{
		if (useFootPrints) {
			Vector3 footPrintPosition = Vector3.zero;
			bool isLeftFoot = false;

			if (footSide == footStep.footType.left) {
				footPrintPosition = leftFoot.transform.position;
				isLeftFoot = true;
			} else if (footSide == footStep.footType.right) {
				footPrintPosition = rightFoot.transform.position;
			} else {
				usingCenterSideToLeft = !usingCenterSideToLeft;

				if (usingCenterSideToLeft) {
					footPrintPosition = transform.position - transform.right * distanceBetweenPrintsInFisrtPerson;
					isLeftFoot = true;
				} else {
					footPrintPosition = transform.position + transform.right * distanceBetweenPrintsInFisrtPerson;
				}
			}

			if (Physics.Raycast (footPrintPosition, raycastDirection, out hit, 5, layer)) {
				if (hit.distance < maxFootPrintDistance) {
					Vector3 placePosition = hit.point + transform.up * 0.013f;

					if (isLeftFoot) {
						currentFootPrint = leftFootPrint;

						if (useFootPrintsFromStates) {
							if (footSteps [footStepIndex].useFootPrints) {
								currentFootPrint = footSteps [footStepIndex].leftFootPrint;
							}
						}

						currentSurfaceOnRightFoot = hit.collider.gameObject;
						if (currentSurfaceOnRightFoot != previousSurfaceOnRightFoot) {
							previousSurfaceOnRightFoot = currentSurfaceOnRightFoot;

							parentAssignedSystem currentParentAssgined = currentSurfaceOnRightFoot.GetComponent<parentAssignedSystem> ();
					
							if (currentParentAssgined != null) {
								currentSurfaceParentOnRightFoot = currentParentAssgined.getAssignedParent ().transform;
							} else {
								currentSurfaceParentOnRightFoot = null;
							}
						}

						if (currentFootPrint != null) {
							createFootPrint (currentFootPrint, placePosition, leftFoot.transform.rotation, hit.normal, currentSurfaceParentOnRightFoot);
						}
					} else {
						currentFootPrint = rightFootPrint;

						if (useFootPrintsFromStates) {
							if (footSteps [footStepIndex].useFootPrints) {
								currentFootPrint = footSteps [footStepIndex].rightFootPrint;
							}
						}

						currentSurfaceOnLeftFoot = hit.collider.gameObject;

						if (currentSurfaceOnLeftFoot != previousSurfaceOnLeftFoot) {
							previousSurfaceOnLeftFoot = currentSurfaceOnLeftFoot;

							parentAssignedSystem currentParentAssgined = currentSurfaceOnLeftFoot.GetComponent<parentAssignedSystem> ();

							if (currentParentAssgined != null) {
								currentSurfaceParentOnLeftFoot = currentParentAssgined.getAssignedParent ().transform;
							} else {
								currentSurfaceParentOnLeftFoot = null;
							}
						}

						if (currentFootPrint != null) {
							createFootPrint (currentFootPrint, placePosition, rightFoot.transform.rotation, hit.normal, currentSurfaceParentOnLeftFoot);
						}
					}
				}
			}
		}
	}

	public void createFootPrint (GameObject foot, Vector3 position, Quaternion rotation, Vector3 normal, Transform currentSurfaceParent)
	{
		if (usePoolingSystemEnabled) {
			newFootPrint = GKC_PoolingSystem.Spawn (foot, position, rotation);
		} else {
			newFootPrint = (GameObject)Instantiate (foot, position, rotation);
		}

		currentFootPrintParent = footPrintsParent.transform;

		if (currentSurfaceParent != null) {
			currentFootPrintParent = currentSurfaceParent;
		} 

		if (currentFootPrintParent != null) {
			newFootPrint.transform.SetParent (currentFootPrintParent);
		} else {
			if (usePoolingSystemEnabled) {
				newFootPrint.transform.SetParent (null);
			}
		}

		Vector3 myForward = Vector3.Cross (newFootPrint.transform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, normal);
		newFootPrint.transform.rotation = dstRot;

		footPrints.Add (newFootPrint);

		footPrintsNotEmpty = true;

		if (vanishFootPrints) {
			fadeObject currentFadeObject = newFootPrint.GetComponent<fadeObject> ();

			if (currentFadeObject != null) {
				currentFadeObject.activeVanish (vanishSpeed);
			}
		}

		if (!usePoolingSystemEnabled && useFootPrintMaxAmount && footPrintMaxAmount > 0 && footPrints.Count > footPrintMaxAmount) {
			footPrintToRemove = footPrints [0];

			footPrints.RemoveAt (0);

			Destroy (footPrintToRemove);

			if (footPrints.Count == 0) {
				footPrintsNotEmpty = false;
			}
		}
	}

	public void createParticles (footStep.footType footSide, int footStepIndex)
	{
		if (useFootParticles) {
			Vector3 footPrintPosition = Vector3.zero;

			if (footSide == footStep.footType.left) {
				footPrintPosition = leftFoot.transform.position;
			} else {
				footPrintPosition = rightFoot.transform.position;
			}

			GameObject currentFootParticles = footParticles;

			if (useFootParticlesFromStates) {
				if (footSteps [footStepIndex].useFootParticles) {
					currentFootParticles = footSteps [footStepIndex].footParticles;
				}
			}

			if (currentFootParticles != null) {
				if (usePoolingSystemEnabled) {
					newFootParticle = GKC_PoolingSystem.Spawn (currentFootParticles, footPrintPosition, transform.rotation);
				} else {
					newFootParticle = (GameObject)Instantiate (currentFootParticles, footPrintPosition, transform.rotation);
				}

				newFootParticle.transform.SetParent (footPrintsParent.transform);
			}
		}
	}

	public void enableOrDisableFootSteps (bool state)
	{
		if (leftFootCollider != null) {
			leftFootCollider.enabled = state;
		}

		if (rightFootCollider != null) {
			rightFootCollider.enabled = state;
		}
	}

	public void enableOrDisableFootStepsWithDelay (bool state, float delayAmount)
	{
		if (leftFootStep != null && leftFootStep.gameObject.activeSelf) {
			leftFootStep.setFooStepStateWithDelay (state, delayAmount);
		}

		if (rightFootStep != null && rightFootStep.gameObject.activeSelf) {
			rightFootStep.setFooStepStateWithDelay (state, delayAmount);
		}
	}

	public void enableOrDisableFootStepsComponents (bool state)
	{
		if (leftFootStep != null) {
			leftFootStep.enableOrDisableFootStep (state);
		}

		if (rightFootStep != null) {
			rightFootStep.enableOrDisableFootStep (state);
		}
	}

	public void setRightFootStep (GameObject rigth)
	{
		rightFoot = rigth;
	}

	public void setLeftFootStep (GameObject left)
	{
		leftFoot = left;
	}

	public void setDefaultGroundFootStepState ()
	{
		setFootStepState (defaultGroundFootStepState);
	}

	public void setFootStepState (string stateName)
	{
		if (!useFootStepStateList) {
			return;
		}

		if (currentFootStepStateName.Equals (stateName)) {
			return;
		}

		bool stateFound = false;

		for (int i = 0; i < footStepStateListCount; i++) {
			temporalFootStepState = footStepStateList [i];

			if (temporalFootStepState.stateEnabled && temporalFootStepState.Name.Equals (stateName)) {
				currentFootStepStateAssigned = true;

				currentFootStepState = temporalFootStepState;

				currentFootStepStateName = currentFootStepState.Name;

				newVolumeRange = currentFootStepState.feetVolumeRange;

				if (useFeetVolumeRangeClamps) {
					newVolumeRange.x = Mathf.Clamp (newVolumeRange.x, 0, feetVolumeRangeClamps.x);
					newVolumeRange.y = Mathf.Clamp (newVolumeRange.y, 0, feetVolumeRangeClamps.y);
				}

				leftFootStep.setStepVolumeRange (newVolumeRange);

				rightFootStep.setStepVolumeRange (newVolumeRange);

				currentFootStepState.isCurrentState = true;

				currentFootStepState.soundPlayed = false;

				stateFound = true;

				if (currentFootStepState.setNewStateAfterDelay) {
					setNewFootStepStateAfterDelay (currentFootStepState.newStateName, currentFootStepState.newStateDelay);
				} else {
					stopSetNewFootStepStateAfterDelayCoroutine ();
				}
			} else {
				temporalFootStepState.isCurrentState = false;
			}
		}

		if (!stateFound) {
			setDefaultFeelVolumeRange ();
		}
	}

	public void setNewFootStepStateAfterDelay (string newState, float delayAmount)
	{
		stopSetNewFootStepStateAfterDelayCoroutine ();

		setNewStateAfterDelayCoroutine = StartCoroutine (setNewFootStepStateAfterDelayCoroutine (newState, delayAmount));
	}

	public void stopSetNewFootStepStateAfterDelayCoroutine ()
	{
		if (setNewStateAfterDelayCoroutine != null) {
			StopCoroutine (setNewStateAfterDelayCoroutine);
		}
	}

	IEnumerator setNewFootStepStateAfterDelayCoroutine (string newState, float delayAmount)
	{
		delayToNewStateActive = true;

		yield return new WaitForSeconds (delayAmount);

		setFootStepState (newState);

		delayToNewStateActive = false;
	}

	public bool isDelayToNewStateActive ()
	{
		return delayToNewStateActive;
	}

	public void setUseNoiseMeshState (bool state)
	{
		useNoiseMesh = state;
	}

	public void setNoiseOnStep (Vector3 footPosition)
	{
		if (currentFootStepStateAssigned && useFootStepStateList && currentFootStepState.stateEnabled && currentFootStepState.useNoise) {
			if (currentFootStepState.useNoiseDetection) {
				applyDamage.sendNoiseSignal (currentFootStepState.noiseRadius, footPosition, noiseDetectionLayer, 
					currentFootStepState.noiseDecibels, currentFootStepState.forceNoiseDetection, showNoiseDetectionGizmo, currentFootStepState.noiseID);
			}

			if (currentFootStepState.useNoiseMesh && useNoiseMesh) {
				if (!noiseMeshManagerFound) {
					GKC_Utils.instantiateMainManagerOnSceneWithType (mainNoiseMeshManagerName, typeof(noiseMeshSystem));

					noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();

					if (noiseMeshManager != null) {
						noiseMeshManagerFound = true;
					}
				}

				if (noiseMeshManagerFound) {
					noiseMeshManager.addNoiseMesh (currentFootStepState.noiseRadius, footPosition + Vector3.up, currentFootStepState.noiseExpandSpeed);
				}
			}
		}
	}

	public void setDefaultFeelVolumeRange ()
	{
		rightFootStep.setStepVolumeRange (feetVolumeRangeClamps);

		leftFootStep.setStepVolumeRange (feetVolumeRangeClamps);
	}

	public void removeSound (int footStepIndex, int soundIndex)
	{
		//footSteps [footStepIndex].poolSounds.remove
		updateComponent ();
	}

	public void setWallRunningState (bool wallRunningActiveValue, bool wallRunningOnRightSideValue)
	{
		wallRunningActive = wallRunningActiveValue;
		wallRunningOnRightSide = wallRunningOnRightSideValue;
	}

	public void setFootStepPausedState (bool state)
	{
		footStepPaused = state;
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	//class to create every type of surface
	//selecting layerName and checkLayer to set that type of step in a mesh
	//if the current step is for a terrain, then set a terrainTextureName, checkTerrain and terrainTextureIndex according to the order in the terrain textures
	//set to true randomPool to play the sounds in a random order, else the sounds are played in the same order
	[System.Serializable]
	public class footStepsLayer
	{
		public string Name;
		public AudioClip[] poolSounds;
		public bool checkSurfaceSystem = true;
		public string terrainTextureName;
		public bool checkTerrain;
		public int terrainTextureIndex;
		public bool randomPool;
		[HideInInspector] public int poolIndex;

		public bool useFootPrints;
		public GameObject rightFootPrint;
		public GameObject leftFootPrint;

		public bool useFootParticles;
		public GameObject footParticles;
	}

	[System.Serializable]
	public class footStepState
	{
		public string Name;
		public bool stateEnabled = true;
		public float stepInterval = 0.3f;
		public Vector2 feetVolumeRange = new Vector2 (0.8f, 1.2f);

		public bool playSoundOnce;
		public bool soundPlayed;
		public bool checkPlayerOnGround = true;

		public bool ignoreOnGround;

		public bool isCurrentState;

		public bool useNoise;
		public float noiseExpandSpeed;
		public bool useNoiseDetection;
		public float noiseRadius;
		public bool setNewStateAfterDelay;
		public float newStateDelay;
		public string newStateName;
		public bool useNoiseMesh = true;
		[Range (0, 2)] public float noiseDecibels = 1;

		public bool forceNoiseDetection;
		public int noiseID = -1;

		public bool playCustomSound;
		public AudioClip customSound;
		public bool useCustomSoundList;
		public AudioClip[] poolCustomSounds;
	}
}