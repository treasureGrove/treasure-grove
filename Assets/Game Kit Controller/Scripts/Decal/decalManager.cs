using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class decalManager : MonoBehaviour
{
	public bool disableFullDecallSystem;
	public bool decalsPlacedOnSurfacesEnabled = true;
	public bool particlesPlacedOnSurfaceEnabled = true;
	public bool soundsPlacedOnSurfaceEnabled = true;

	public bool fadeDecals;
	public float fadeSpeed;
	public static Transform decalParent;
	public static bool fadeDecalsValue;
	public static float fadeSpeedValue;

	public List<impactInfo> impactListInfo = new List<impactInfo> ();
	public static List<impactInfo> impactListInfoValue = new List<impactInfo> ();

	public GameObject projectileImpactSoundPrefab;
	public static GameObject projectileImpactSoundPrefabObject;

	public static bool disableFullDecallSystemValue;
	public static bool decalsPlacedOnSurfacesEnabledValue;
	public static bool particlesPlacedOnSurfaceEnabledValue;
	public static bool soundsPlacedOnSurfaceEnabledValue;

	void Start ()
	{
		if (decalParent == null) {
			decalParent = new GameObject ().transform;
			decalParent.name = "Decal Parent";
		}

		fadeDecalsValue = fadeDecals;
		fadeSpeedValue = fadeSpeed;
		impactListInfoValue = impactListInfo;
		projectileImpactSoundPrefabObject = projectileImpactSoundPrefab;

		disableFullDecallSystemValue = disableFullDecallSystem;
		decalsPlacedOnSurfacesEnabledValue = decalsPlacedOnSurfacesEnabled;
		particlesPlacedOnSurfaceEnabledValue = particlesPlacedOnSurfaceEnabled;
		soundsPlacedOnSurfaceEnabledValue = soundsPlacedOnSurfaceEnabled;
	}

	public static void setScorch (Quaternion rotation, GameObject scorch, RaycastHit hit, GameObject collision, bool projectilesPoolEnabled)
	{
		//set the position of the scorch according to the hit point
		if (!collision.GetComponent<characterDamageReceiver> ()) {
			GameObject newScorch = null;

			if (projectilesPoolEnabled) {
				newScorch = GKC_PoolingSystem.Spawn (scorch, Vector3.zero, Quaternion.identity, 30);
			} else {
				newScorch = Instantiate (scorch);
			}

			newScorch.transform.rotation = rotation;
			newScorch.transform.position = hit.point + hit.normal * 0.03f;

			//get the surface normal to rotate the scorch to that angle
			Vector3 myForward = Vector3.Cross (newScorch.transform.right, hit.normal);
			Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);
			newScorch.transform.rotation = dstRot;

			setScorchParent (collision.transform, newScorch, hit.point);

			if (fadeDecalsValue) {
				fadeObject currentFadeObject = newScorch.GetComponent<fadeObject> ();

				if (currentFadeObject != null) {
					currentFadeObject.activeVanish (fadeSpeedValue);
				}
			}
		}
	}

	public static bool placeProjectileInSurface (Quaternion rotation, Transform projectile, RaycastHit hit, GameObject objectToDamage, 
	                                             bool attachToLimbs, bool ignoreSetProjectilePositionOnImpact)
	{
		if (!ignoreSetProjectilePositionOnImpact) {
			projectile.rotation = rotation;
			projectile.position = hit.point + hit.normal * 0.03f;
		
			//get the surface normal to rotate the scorch to that angle
			Vector3 myForward = Vector3.Cross (projectile.forward, hit.normal);
			Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);

			projectile.rotation = dstRot;
		}
	
		Rigidbody currentRigidbody = objectToDamage.GetComponent<Rigidbody> ();

		if (currentRigidbody != null) {
			ragdollActivator currentRagdollActivator = objectToDamage.GetComponent<ragdollActivator> ();

			if (currentRagdollActivator != null) {
				if (attachToLimbs) {
					List<ragdollActivator.bodyPart> bones = currentRagdollActivator.getBodyPartsList ();

					float distance = Mathf.Infinity;
					int index = -1;

					for (int i = 0; i < bones.Count; i++) {
						float currentDistance = GKC_Utils.distance (bones [i].transform.position, projectile.position);
						if (currentDistance < distance) {
							distance = currentDistance;
							index = i;
						}
					}

					if (index != -1) {
						projectile.SetParent (bones [index].transform);
					}

					return true;
				}
			} 

			characterDamageReceiver currentCharacterDamageReceiver = objectToDamage.GetComponent<characterDamageReceiver> ();

			if (currentCharacterDamageReceiver != null) {
				projectile.SetParent (currentCharacterDamageReceiver.character.transform);

				return true;
			} 

			projectile.SetParent (objectToDamage.transform);

			return true;
		} 

		vehicleDamageReceiver currentVehicleDamageReceiver = objectToDamage.GetComponent<vehicleDamageReceiver> ();

		if (currentVehicleDamageReceiver != null) {
			projectile.SetParent (currentVehicleDamageReceiver.getPlaceToShootWithHealthManagement ());

			return true;
		} 

		parentAssignedSystem currentParentAssignedSystem = objectToDamage.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			projectile.SetParent (currentParentAssignedSystem.getAssignedParentTransform ());

			return true;
		}

		projectile.SetParent (decalParent);

		return true;
	}

	public static bool setImpactDecal (int impactIndex, Transform projectile, GameObject objectToDamage, 
	                                   float projectileRayDistance, LayerMask projectilelayer, bool useSorch, bool projectilesPoolEnabled)
	{
		if (disableFullDecallSystemValue) {
			return false;
		}

		bool hasImpactDecal = false;

		Vector3 projectilePosition = projectile.position;

		Vector3 projectileForward = projectile.forward;

		if (impactIndex == -2) {
			bool terrainFound = false;

			//get the current texture index of the terrain under the player.
			int surfaceIndex = GetMainTexture (projectilePosition, objectToDamage.GetComponent<Terrain> ());

			for (int i = 0; i < impactListInfoValue.Count; i++) {
				if (!terrainFound && impactListInfoValue [i].checkTerrain && impactListInfoValue [i].decalEnabled) {
					if (impactListInfoValue [i].terrainTextureIndex == surfaceIndex) {
						impactIndex = i;

						terrainFound = true;
					}
				}
			}
		}

		if (impactIndex >= 0 && impactIndex < impactListInfoValue.Count) {
			
			impactInfo newImpactInfo = impactListInfoValue [impactIndex];

			if (newImpactInfo.decalEnabled) {
				hasImpactDecal = true;

				if (soundsPlacedOnSurfaceEnabledValue) {
					if (newImpactInfo.impactSound != null) {
						projectileSystem currentProjectileSystem = projectile.GetComponent<projectileSystem> ();

						if (currentProjectileSystem != null) {
							currentProjectileSystem.createImpactSound (newImpactInfo.impactSound);
						}
					}
				}

				bool scorchAvailable = useSorch && newImpactInfo.scorch != null;

				bool particlesAvailable = newImpactInfo.impactParticles != null;

				if (scorchAvailable || particlesAvailable) {
					RaycastHit hit = new RaycastHit ();

					bool surfaceDetected = false;

					bool raycastLaunched = false;

					if (scorchAvailable) {
						surfaceDetected = Physics.Raycast (projectilePosition - projectileForward * 0.4f, projectileForward, 
							out hit, projectileRayDistance, projectilelayer);

						raycastLaunched = true;

						//the bullet fired is a simple bullet or a greanade, check the hit point with a raycast to set in it a scorch
						if (surfaceDetected) {
							if (decalsPlacedOnSurfacesEnabledValue) {
								setScorchFromList (projectile.rotation, newImpactInfo.scorch, hit, objectToDamage,
									newImpactInfo.scorchScale, newImpactInfo.fadeScorch, newImpactInfo.timeToFade, projectilesPoolEnabled);
							}
						}
					}

					if (particlesPlacedOnSurfaceEnabledValue) {
						if (particlesAvailable) {
							if (!raycastLaunched) {
								surfaceDetected = Physics.Raycast (projectilePosition - projectileForward * 0.4f, projectileForward, 
									out hit, projectileRayDistance, projectilelayer);
							}

							if (surfaceDetected) {
								Vector3 particlesPosition = hit.point + hit.normal * 0.03f;

								Vector3 myForward = Vector3.Cross (projectileForward, hit.normal);
								Quaternion particlesRotation = Quaternion.LookRotation (hit.normal, myForward);

								GameObject impactParticles = null;

								if (projectilesPoolEnabled) {
									impactParticles = GKC_PoolingSystem.Spawn (newImpactInfo.impactParticles, particlesPosition, particlesRotation, 30);
								} else {
									impactParticles = Instantiate (newImpactInfo.impactParticles, particlesPosition, particlesRotation);
								}
									
								healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

								if (currentHealthManagement != null) {
									impactParticles.transform.SetParent (currentHealthManagement.getPlaceToShootWithHealthManagement ());

									return hasImpactDecal;
								}

								Rigidbody currentRigidbody = objectToDamage.GetComponent<Rigidbody> ();

								if (currentRigidbody != null) {
									impactParticles.transform.SetParent (objectToDamage.transform);

									return hasImpactDecal;
								} 

								vehicleDamageReceiver currentVehicleDamageReceiver = objectToDamage.GetComponent<vehicleDamageReceiver> ();

								if (currentVehicleDamageReceiver != null) {
									impactParticles.transform.SetParent (currentVehicleDamageReceiver.getPlaceToShootWithHealthManagement ());

									return hasImpactDecal;
								} 

								impactParticles.transform.SetParent (decalParent);
							}
						}
					}
				}
			}
		}

		return hasImpactDecal;
	}

	public static void setImpactSoundParent (Transform newImpactSound)
	{
		newImpactSound.SetParent (decalParent);
	}

	public static void setScorchFromList (Quaternion rotation, GameObject scorch, RaycastHit hit, GameObject collision, 
	                                      float scorchScale, bool fadeScorch, float timeToFade, bool projectilesPoolEnabled)
	{
		//set the position of the scorch according to the hit point
		GameObject newScorch = null;

		if (projectilesPoolEnabled) {
			newScorch = GKC_PoolingSystem.Spawn (scorch, Vector3.zero, Quaternion.identity, 30);
		} else {
			newScorch = Instantiate (scorch);
		}

		newScorch.transform.position = hit.point + hit.normal * 0.03f;
		newScorch.transform.rotation = rotation;

		//get the surface normal to rotate the scorch to that angle
		Vector3 myForward = Vector3.Cross (newScorch.transform.right, hit.normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);
		newScorch.transform.rotation = dstRot;

		setScorchParent (collision.transform, newScorch, hit.point);
	
		newScorch.transform.localScale *= scorchScale;

		if (fadeScorch) {
			fadeObject currentFadeObject = newScorch.GetComponent<fadeObject> ();

			if (currentFadeObject != null) {
				currentFadeObject.activeVanish (timeToFade);
			}
		}
	}

	public static int checkIfHasDecalImpact (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getDecalImpactIndexWithHealthManagement ();
		}

		decalTypeInformation currentDecalTypeInformation = objectToCheck.GetComponent<decalTypeInformation> ();

		if (currentDecalTypeInformation != null) {
			return currentDecalTypeInformation.getDecalImpactIndex ();
		} 

		if (objectToCheck.GetComponent<Terrain> ()) {
			return -2;
		} 

		return -1;
	}

	public static void setScorchParent (Transform objectDetected, GameObject newScorch, Vector3 impactPosition)
	{
		healthManagement currentHealthManagement = objectDetected.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			newScorch.transform.SetParent (currentHealthManagement.getPlaceToShootWithHealthManagement ());

			return;
		}

		Rigidbody currentRigidbody = objectDetected.GetComponent<Rigidbody> ();

		if (currentRigidbody != null) {
			newScorch.transform.SetParent (objectDetected);

			return;
		} 

		decalTypeInformation currentdecalTypeInformation = objectDetected.GetComponent<decalTypeInformation> ();

		if (currentdecalTypeInformation != null) {
			if (currentdecalTypeInformation.isParentScorchOnThisObjectEnabled ()) {
				newScorch.transform.SetParent (objectDetected);

				return;
			} 
		}

		vehicleDamageReceiver currentVehicleDamageReceiver = objectDetected.GetComponent<vehicleDamageReceiver> ();

		if (currentVehicleDamageReceiver != null) {
			newScorch.transform.SetParent (currentVehicleDamageReceiver.getPlaceToShootWithHealthManagement ());

			return;
		} 

		parentAssignedSystem currentParentAssignedSystem = objectDetected.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			newScorch.transform.SetParent (currentParentAssignedSystem.getAssignedParentTransform ());

			return;
		}

		newScorch.transform.SetParent (decalParent);
	}

	public static int GetMainTexture (Vector3 playerPos, Terrain terrain)
	{
		//get the index of the current texture of the terrain where the player is walking
		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainPos = terrain.transform.position;

		//calculate which splat map cell the playerPos falls within
		int mapX = (int)(((playerPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
		int mapZ = (int)(((playerPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

		//get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
		float[,,] splatmapData = terrainData.GetAlphamaps (mapX, mapZ, 1, 1);

		//change the 3D array data to a 1D array:
		float[] cellMix = new float[splatmapData.GetUpperBound (2) + 1];
		for (int n = 0; n < cellMix.Length; n++) {
			cellMix [n] = splatmapData [0, 0, n];    
		}

		float maxMix = 0;
		int maxIndex = 0;
		//loop through each mix value and find the maximum
		for (int n = 0; n < cellMix.Length; n++) {
			if (cellMix [n] > maxMix) {
				maxIndex = n;
				maxMix = cellMix [n];
			}
		}

		return maxIndex;
	}

	public bool surfaceUseNoise (int surfaceIndex)
	{
		if (surfaceIndex >= 0 && surfaceIndex < impactListInfo.Count) {
			return impactListInfo [surfaceIndex].useNoise;
		} 

		return false;
	}

	[System.Serializable]
	public class impactInfo
	{
		public string name;
		public bool decalEnabled = true;
		public string surfaceName;
		public AudioClip impactSound;
		public GameObject scorch;
		[Range (1, 5)] public float scorchScale;
		public bool fadeScorch;
		public float timeToFade;
		public GameObject impactParticles;
		public bool checkTerrain;
		public int terrainTextureIndex;
		public bool useNoise = true;
	}
}